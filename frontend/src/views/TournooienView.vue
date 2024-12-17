<template>
  <div class="min-h-screen bg-gray-100">
    <div class="container mx-auto p-5">
      <!-- Generate Matches Section -->
      <section class="mb-10">
        <h2 class="text-xl font-bold mb-4">Generate All Matches</h2>
        <div class="bg-white shadow-md p-6 rounded-lg">
          <button @click="generateAndSendMatches" class="btn-primary">Create All Matches</button>
          <p v-if="successMessage" class="text-green-600 mt-4">{{ successMessage }}</p>
          <p v-if="errorMessage" class="text-red-600 mt-4">{{ errorMessage }}</p>
        </div>
      </section>

<!-- Filter Section -->
<section class="mb-10">
  <h2 class="text-xl font-bold mb-4">Filter Matches</h2>
  <div class="bg-white shadow-md p-6 rounded-lg">
    <!-- Search Input -->
    <input
      v-model="searchQuery"
      type="text"
      placeholder="Search by team name or ID"
      class="w-full p-2 border border-gray-300 rounded mb-4"
    />

    <!-- Checkbox Filters -->
    <div class="flex items-center space-x-4">
      <label class="flex items-center">
        <input type="checkbox" v-model="onlyFinished" class="mr-2" />
        <span>Only Finished</span>
      </label>

      <label class="flex items-center">
        <input type="checkbox" v-model="onlyUpcoming" class="mr-2" />
        <span>Only Upcoming</span>
      </label>
    </div>
  </div>
</section>


      <!-- Match Grid Section -->
      <section>
        <h2 class="text-xl font-bold mb-4">Match Grid</h2>
        <div class="bg-white shadow-md p-6 rounded-lg">
          <div class="grid grid-cols-4 gap-4">
            <!-- Display Filtered Matches -->
            <div v-for="match in filteredMatches" :key="match.Id" class="match-card p-4 border border-gray-300 rounded-lg">
              <div class="font-bold">{{ match.HomeTeamName }} vs {{ match.AwayTeamName }}</div>
              <div class="text-gray-500">{{ new Date(match.Starttime).toLocaleString() }}</div>
              <div class="mt-2">
                <span :class="{'text-green-600': match.IsFinished, 'text-red-600': !match.IsFinished}">
                  {{ match.IsFinished ? 'Finished' : 'Upcoming' }}
                </span>
              </div>
              <button @click="deleteMatch(match.Id)" class="mt-2 text-red-600">Gok</button>
            </div>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      matches: [], // Store matches
      teams: [], // Store teams
      searchQuery: "", // Store search input
      onlyFinished: false, // Checkbox state for finished matches
      onlyUpcoming: false, // Checkbox state for upcoming matches
      successMessage: "",
      errorMessage: "",
    };
  },
  computed: {
    // Filter matches based on search input and checkboxes
    filteredMatches() {
      let filtered = this.matches;

      // Apply search query filter
      if (this.searchQuery) {
        const query = this.searchQuery.toLowerCase();
        filtered = filtered.filter((match) => {
          const homeTeamName = match.HomeTeamName.toLowerCase();
          const awayTeamName = match.AwayTeamName.toLowerCase();
          const homeTeamId = match.HomeTeamId.toString();
          const awayTeamId = match.AwayTeamId.toString();

          return (
            homeTeamName.includes(query) ||
            awayTeamName.includes(query) ||
            homeTeamId.includes(query) ||
            awayTeamId.includes(query)
          );
        });
      }

      // Apply checkbox filters
      if (this.onlyFinished) {
        filtered = filtered.filter((match) => match.IsFinished);
      }

      if (this.onlyUpcoming) {
        filtered = filtered.filter((match) => !match.IsFinished);
      }

      return filtered;
    },
  },
  methods: {
    // Fetch teams and matches
    async fetchTeams() {
      try {
        const response = await axios.get("http://localhost/pra-c3/frontend/database/getTeams.php");
        this.teams = response.data;
        this.fetchMatches();
      } catch (error) {
        console.error("Error fetching teams:", error);
      }
    },

    async fetchMatches() {
      try {
        const response = await axios.get("http://localhost/pra-c3/frontend/database/getMatches.php");
        this.matches = response.data.map((match) => {
          const currentTime = new Date();
          const matchStartTime = new Date(match.Starttime);
          match.IsFinished = currentTime > matchStartTime;

          match.HomeTeamName = this.getTeamName(match.HomeTeamId);
          match.AwayTeamName = this.getTeamName(match.AwayTeamId);
          return match;
        });
      } catch (error) {
        console.error("Error fetching matches:", error);
      }
    },

    getTeamName(teamId) {
      const team = this.teams.find((t) => t.Id === teamId);
      return team ? team.Name : "Unknown Team";
    },

    async generateAndSendMatches() {
      try {
        const fieldsAvailable = 3;
        const response = await axios.post(
          "http://localhost:5116/api/Tourney/generate-schedule",
          fieldsAvailable,
          { headers: { "Content-Type": "application/json" } }
        );

        if (response.status === 200) {
          this.successMessage = "All matches were successfully created!";
          this.errorMessage = "";
          this.fetchMatches();
        } else {
          this.errorMessage = "Failed to create matches. Please try again.";
          this.successMessage = "";
        }
      } catch (error) {
        console.error("Error creating matches:", error);
        this.errorMessage = "Failed to create matches. Please try again.";
        this.successMessage = "";
      }
    },

    async deleteMatch(matchId) {
      try {
        const response = await axios.delete(`http://localhost:5116/api/Tourney/match/${matchId}`);
        if (response.status === 200) {
          this.successMessage = "Match deleted successfully!";
          this.fetchMatches();
        }
      } catch (error) {
        console.error("Error deleting match:", error);
        this.errorMessage = "Failed to delete match.";
        this.successMessage = "";
      }
    },
  },
  created() {
    this.fetchTeams();
  },
};
</script>


<style>
label {
  font-size: 14px;
  color: #333;
}
/* Match card styling */
.match-card {
  background-color: grey;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  padding: 12px;
  text-align: center;
  font-size: 14px;
  transition: transform 0.2s ease;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  height: 180px;
}

.match-card:hover {
  transform: scale(1.05);
}

.match-card button {
  background-color: #007bff;
  color: white;
  padding: 8px 14px;
  border-radius: 5px;
  border: none;
  font-size: 12px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.match-card button:hover {
  background-color: #0056b3;
}

input[type="text"] {
  width: 100%;
  padding: 10px;
  margin-top: 10px;
  border-radius: 5px;
  border: 1px solid #ccc;
  font-size: 14px;
}
</style>
