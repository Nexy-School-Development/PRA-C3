<template>
  <div class="min-h-screen bg-gray-100">
    <!-- Modal for Editing Match -->
    <div v-if="showModal" class="fixed inset-0 flex items-center justify-center bg-gray-500 bg-opacity-75 z-50">
      <div class="bg-white p-8 rounded-lg shadow-xl max-w-lg w-full transform transition-all">
        <h3 class="text-2xl font-bold text-center mb-6">Edit Match</h3>

        <!-- Edit Form Fields... -->
      </div>
    </div>

    <!-- Match List Section -->
    <div v-if="!showModal" class="container mx-auto p-5">
      <section class="mb-10">
        <h2 class="text-xl font-bold mb-4">Create a Match</h2>
        <!-- Create Match Form... -->
      </section>

      <section>
        <h2 class="text-xl font-bold mb-4">Match List</h2>
        <!-- Filters for Upcoming and Finished Matches -->
        <div class="bg-white shadow-md p-6 rounded-lg mb-4">
          <label class="inline-flex items-center mr-6">
            <input type="checkbox" v-model="filter.upcoming" class="mr-2" />
            <span>Upcoming</span>
          </label>
          <label class="inline-flex items-center">
            <input type="checkbox" v-model="filter.finished" class="mr-2" />
            <span>Finished</span>
          </label>
        </div>

        <div class="bg-white shadow-md p-6 rounded-lg">
          <table class="table-auto w-full text-left" v-if="filteredMatches.length > 0">
            <thead>
              <tr class="bg-blue-600 text-white">
                <th class="p-3">Home Team</th>
                <th class="p-3">Away Team</th>
                <th class="p-3">Start Time</th>
                <th class="p-3">Status</th>
                <th class="p-3">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="match in filteredMatches" :key="match.Id" class="border-t">
                <td class="p-3">{{ getTeamName(match.HomeTeamId) }}</td>
                <td class="p-3">{{ getTeamName(match.AwayTeamId) }}</td>
                <td class="p-3">{{ new Date(match.Starttime).toLocaleString() }}</td>
                <td class="p-3">
                  <span :class="{'text-green-600': match.IsFinished, 'text-red-600': !match.IsFinished}">
                    {{ match.IsFinished ? "Finished" : "Upcoming" }}
                  </span>
                </td>
                <td class="p-3">
                  <button v-if="!match.IsFinished" @click="openEditModal(match)" class="btn-secondary">Update</button>
                  <button @click="deleteMatch(match.Id)" class="btn-danger">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
          <p v-else>No matches available</p>
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
      matches: [],
      teams: [],
      newMatch: {
        homeTeamId: null,
        awayTeamId: null,
        starttime: "",
      },
      successMessage: '',
      errorMessage: '',
      showModal: false,
      editMatch: { 
        id: null,
        homeTeamId: null,
        awayTeamId: null,
        starttime: "",
        team1Score: null,  // Home team score
        team2Score: null,  // Away team score
      },
      filter: {
        upcoming: true,
        finished: true,
      },
    };
  },
  mounted() {
    this.fetchTeams();
    this.fetchMatches();  // Fetch matches on component load
  },
  methods: {
    async fetchTeams() {
      try {
        const response = await axios.get('http://localhost/pra-c3/frontend/database/getTeams.php');
        this.teams = response.data;
      } catch (error) {
        console.error("Error fetching teams", error);
      }
    },

    async fetchMatches() {
      try {
        const response = await axios.get('http://localhost/pra-c3/frontend/database/getMatches.php');
        console.log(response.data);
        if (Array.isArray(response.data)) {
          this.matches = response.data.map(match => {
            const currentTime = new Date();
            const matchStartTime = new Date(match.Starttime);
            match.IsFinished = currentTime > matchStartTime;
            return match;
          });
        }
      } catch (error) {
        console.error("Error fetching matches", error);
      }
    },

    getTeamName(teamId) {
      const team = this.teams.find(t => t.Id === teamId);
      return team ? `${team.Name} (${team.Id})` : 'Unknown';
    },

    openEditModal(match) {
      this.editMatch = { 
        ...match,
        team1Score: match.Team1Score,
        team2Score: match.Team2Score,
      };
      this.showModal = true;
    },

    closeModal() {
      this.showModal = false;
    },

    async updateMatch() {
      const { id, homeTeamId, awayTeamId, starttime, team1Score, team2Score } = this.editMatch;
      
      if (!id) {
        console.error("Match id is undefined or null");
        this.errorMessage = "Invalid match id";
        return;
      }

      const updatedMatchData = {
        HomeTeamId: homeTeamId,
        AwayTeamId: awayTeamId,
        Starttime: starttime,
        Team1Score: team1Score,
        Team2Score: team2Score,
      };

      try {
        const response = await axios.put(`http://localhost:5116/api/Match/update/${id}`, updatedMatchData);
        this.successMessage = "Match updated successfully!";
        this.errorMessage = "";
        this.fetchMatches();
        this.closeModal();
      } catch (error) {
        console.error("Error updating match", error);
        this.errorMessage = error.response?.data?.message || 'Failed to update match.';
        this.successMessage = '';
      }
    },

    async deleteMatch(matchId) {
      try {
        await axios.delete(`http://localhost:5116/api/Match/delete/${matchId}`);
        this.fetchMatches();
      } catch (error) {
        console.error("Error deleting match", error);
      }
    },

    getFilteredMatches() {
      return this.matches.filter(match => {
        if (this.filter.upcoming && !match.IsFinished) return true;
        if (this.filter.finished && match.IsFinished) return true;
        return false;
      });
    }
  },

  computed: {
    filteredMatches() {
      return this.getFilteredMatches();
    }
  }
};
</script>

<style>
.input-field {
  width: 100%;
  border: 1px solid #ddd;
  padding: 12px;
  margin-bottom: 15px;
  border-radius: 8px;
  box-shadow: 0 0 5px rgba(0, 0, 0, 0.1);
}
.btn-primary {
  background-color: #007bff;
  color: white;
  padding: 12px 20px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
}
.btn-secondary {
  background-color: #6c757d;
  color: white;
  padding: 12px 20px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
}
.btn-danger {
  background-color: #dc3545;
  color: white;
  padding: 12px 20px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
}
</style>