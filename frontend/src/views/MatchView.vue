<template>
  <div class="min-h-screen bg-gray-100">

    <!-- Modal for Editing Match -->
    <div v-if="showModal" class="fixed inset-0 flex items-center justify-center bg-gray-500 bg-opacity-75 z-50">
      <div class="bg-white p-8 rounded-lg shadow-xl max-w-lg w-full transform transition-all">
        <h3 class="text-2xl font-bold text-center mb-6">Edit Match</h3>

        <div class="mb-4">
          <label for="homeTeam" class="block text-lg font-semibold">Home Team</label>
          <select v-model="editMatch.homeTeamId" id="homeTeam" class="input-field">
            <option v-for="team in teams" :key="team.Id" :value="team.Id">{{ team.Name }} (id: {{ team.Id }})</option>
          </select>
        </div>

        <div class="mb-4">
          <label for="awayTeam" class="block text-lg font-semibold">Away Team</label>
          <select v-model="editMatch.awayTeamId" id="awayTeam" class="input-field">
            <option v-for="team in teams" :key="team.Id" :value="team.Id">{{ team.Name }} (id: {{ team.Id }})</option>
          </select>
        </div>

        <div class="mb-6">
          <label for="starttime" class="block text-lg font-semibold">Start Time</label>
          <input v-model="editMatch.starttime" type="datetime-local" id="starttime" class="input-field" />
        </div>

        <div class="flex justify-center gap-4">
          <button @click="updateMatch" class="btn-primary" type="button">Save Changes</button>
          <button @click="closeModal" class="btn-secondary" type="button">Cancel</button>
        </div>
      </div>
    </div>

    <!-- Match List Section -->
    <div v-if="!showModal" class="container mx-auto p-5">
      <section class="mb-10">
        <h2 class="text-xl font-bold mb-4">Create a Match</h2>
        <div class="bg-white shadow-md p-6 rounded-lg">
          <select v-model="newMatch.homeTeamId" class="input-field">
            <option v-for="team in teams" :key="team.Id" :value="team.Id">{{ team.Name }} (id: {{ team.Id }})</option>
          </select>
          <select v-model="newMatch.awayTeamId" class="input-field">
            <option v-for="team in teams" :key="team.Id" :value="team.Id">{{ team.Name }} (id: {{ team.Id }})</option>
          </select>
          <input v-model="newMatch.starttime" type="datetime-local" class="input-field" />
          <button @click="createMatch" class="btn-primary">Create Match</button>
          <p v-if="successMessage" class="text-green-600">{{ successMessage }}</p>
          <p v-if="errorMessage" class="text-red-600">{{ errorMessage }}</p>
        </div>
      </section>

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
                <th class="p-3">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="match in matches" :key="match.Id" class="border-t">
                <td class="p-3">{{ getTeamName(match.HomeTeamId) }}</td>
                <td class="p-3">{{ getTeamName(match.AwayTeamId) }}</td>
                <td class="p-3">{{ new Date(match.Starttime).toLocaleString() }}</td>
                <td class="p-3">
                  <span
                    :class="{
                      'text-green-600': match.IsFinished,
                      'text-red-600': !match.IsFinished,
                    }"
                  >
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
      },
    };
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
        this.matches = response.data.map(match => {
          const currentTime = new Date();
          const matchStartTime = new Date(match.Starttime);
          match.IsFinished = currentTime > matchStartTime;
          return match;
        });
      } catch (error) {
        console.error("Error fetching matches", error);
      }
    },
    openEditModal(match) {
      this.editMatch = { ...match };
      this.showModal = true;
    },
    closeModal() {
      this.showModal = false;
    },
    async updateMatch() {
      const { id, homeTeamId, awayTeamId, starttime } = this.editMatch;
      if (!id) {
        console.error("Match id is undefined or null");
        this.errorMessage = "Invalid match id";
        return;
      }

      try {
        const response = await axios.put(`http://localhost:5116/api/Match/update/${id}`, {
          HomeTeamId: homeTeamId,
          AwayTeamId: awayTeamId,
          Starttime: starttime,
        });
        
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
    getTeamName(teamId) {
      const team = this.teams.find(t => t.Id === teamId);
      return team ? `${team.Name} (${team.Id})` : 'Unknown';
    }
  },
  created() {
    this.fetchTeams();
    this.fetchMatches();
  },
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
