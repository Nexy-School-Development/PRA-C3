<template>
  <div class="min-h-screen bg-gray-100">
    <div class="container mx-auto p-5">
      <section class="mb-10">
        <h2 class="text-xl font-bold mb-4">Generate All Matches</h2>
        <div class="bg-white shadow-md p-6 rounded-lg">
          <button @click="generateAndSendMatches" class="btn-primary">Create All Matches</button>
          <p v-if="successMessage" class="text-green-600 mt-4">{{ successMessage }}</p>
          <p v-if="errorMessage" class="text-red-600 mt-4">{{ errorMessage }}</p>
        </div>
      </section>

      <!-- Display Match List -->
      <section>
        <h2 class="text-xl font-bold mb-4">Match List</h2>
        <div class="bg-white shadow-md p-6 rounded-lg">
          <table class="table-auto w-full text-left">
            <thead>
              <tr class="bg-blue-600 text-white">
                <th class="p-3">Home Team</th>
                <th class="p-3">Away Team</th>
                <th class="p-3">Start Time</th>
                <th class="p-3">Status</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="match in matches" :key="match.Id" class="border-t">
                <td class="p-3">{{ getTeamName(match.HomeTeamId) }}</td>
                <td class="p-3">{{ getTeamName(match.AwayTeamId) }}</td>
                <td class="p-3">{{ new Date(match.Starttime).toLocaleString() }}</td>
                <td class="p-3">
                  <span :class="{'text-green-600': match.IsFinished, 'text-red-600': !match.IsFinished}">
                    {{ match.IsFinished ? "Finished" : "Upcoming" }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
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
      teams: [],
      matches: [],
      successMessage: '',
      errorMessage: '',
    };
  },
  methods: {
    async fetchTeams() {
      try {
        const response = await axios.get("http://localhost/pra-c3/frontend/database/getTeams.php");
        this.teams = response.data;
      } catch (error) {
        console.error("Error fetching teams:", error);
      }
    },

    async fetchMatches() {
      try {
        const response = await axios.get("http://localhost/pra-c3/frontend/database/getMatches.php");
        this.matches = response.data;
      } catch (error) {
        console.error("Error fetching matches:", error);
      }
    },

    async generateAndSendMatches() {
      if (this.teams.length < 2) {
        this.errorMessage = "Not enough teams to create matches.";
        return;
      }

      // Generate matches
      const matches = [];
      const starttime = "2024-12-17T12:32:00.000000"; // Fixed start time for all matches
      for (let i = 0; i < this.teams.length; i++) {
        for (let j = 0; j < this.teams.length; j++) {
          if (i !== j) { // Ensuring a team doesn't play against itself
            matches.push({
              HomeTeamId: this.teams[i].Id,
              AwayTeamId: this.teams[j].Id,
              Team1Score: 0,
              Team2Score: 0,
              Starttime: starttime,
              IsFinished: false,
              TourneyId: 1, // Static tournament ID
            });
          }
        }
      }

      try {
        // Send matches to the backend
        const response = await axios.post("http://localhost/pra-c3/frontend/database/createMatches.php", matches);
        if (response.status === 200) {
          this.successMessage = "All matches were successfully created!";
          this.errorMessage = "";
          this.fetchMatches(); // Refresh match list
        }
      } catch (error) {
        console.error("Error creating matches:", error);
        this.errorMessage = "Failed to create matches. Please try again.";
        this.successMessage = "";
      }
    },

    getTeamName(teamId) {
      const team = this.teams.find((t) => t.Id === teamId);
      return team ? team.Name : "Unknown";
    },
  },
  created() {
    this.fetchTeams();
    this.fetchMatches();
  },
};
</script>

<style>
.btn-primary {
  background-color: #007bff;
  color: white;
  padding: 12px 20px;
  border-radius: 8px;
  cursor: pointer;
  font-weight: bold;
}
.table-auto th,
.table-auto td {
  border: 1px solid #ddd;
  padding: 8px;
  text-align: center;
}
.text-green-600 {
  color: #28a745;
}
.text-red-600 {
  color: #dc3545;
}
</style>
