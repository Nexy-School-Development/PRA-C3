<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-blue-600 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Match Management</h1>
    </header>

    <main class="container mx-auto p-5">
      <section class="mb-10">
        <h2 class="text-xl font-bold mb-4">Create a Match</h2>
        <div class="bg-white shadow-md p-6 rounded-lg">
          <input v-model="newMatch.homeTeamId" type="number" placeholder="Home Team ID" class="input-field" />
          <input v-model="newMatch.awayTeamId" type="number" placeholder="Away Team ID" class="input-field" />
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
                <td class="p-3">{{ match.HomeTeamId }}</td>
                <td class="p-3">{{ match.AwayTeamId }}</td>
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
                  <button v-if="!match.IsFinished" @click="updateMatchResult(match.Id)" class="btn-secondary">Update</button>
                  <button @click="deleteMatch(match.Id)" class="btn-danger">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>
    </main>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      matches: [],
      newMatch: {
        homeTeamId: null,
        awayTeamId: null,
        starttime: "",
      },
      successMessage: '',
      errorMessage: ''
    };
  },
  methods: {
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
    async createMatch() {
      try {
        const currentTime = new Date();
        const matchStartTime = new Date(this.newMatch.starttime);
        const isFinished = currentTime > matchStartTime;

        const response = await axios.post('http://localhost:5116/api/Match/create', {
          HomeTeamId: this.newMatch.homeTeamId,
          AwayTeamId: this.newMatch.awayTeamId,
          Starttime: this.newMatch.starttime,
          IsFinished: isFinished
        }, {
          headers: {
            'Content-Type': 'application/json'
          }
        });
        this.successMessage = 'Match created successfully!';
        this.errorMessage = '';
        this.newMatch.homeTeamId = null;
        this.newMatch.awayTeamId = null;
        this.newMatch.starttime = "";
        this.fetchMatches();
      } catch (error) {
        this.errorMessage = error.response?.data?.message || 'Failed to create match.';
        this.successMessage = '';
        console.error("Error creating match", error);
      }
    },
    async updateMatchResult(matchId) {
      const team1Score = prompt("Enter Home Team score:");
      const team2Score = prompt("Enter Away Team score:");
      if (team1Score === null || team2Score === null) return;

      try {
        await axios.put(`http://localhost:5116/api/Match/${matchId}/result`, {
          team1Score: parseInt(team1Score),
          team2Score: parseInt(team2Score),
        });
        this.fetchMatches();
      } catch (error) {
        console.error("Error updating match result", error);
      }
    },
    async deleteMatch(matchId) {
      try {
        await axios.delete(`http://localhost:5116/api/Match/${matchId}`);
        this.fetchMatches();
      } catch (error) {
        console.error("Error deleting match", error);
      }
    },
  },
  created() {
    this.fetchMatches();
  },
};
</script>

<style>
.input-field {
  width: 100%;
  border: 1px solid #ddd;
  padding: 10px;
  margin-bottom: 10px;
  border-radius: 5px;
}
.btn-primary {
  background-color: #007bff;
  color: white;
  padding: 10px 15px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}
.btn-secondary {
  background-color: #6c757d;
  color: white;
  padding: 5px 10px;
  border: none;
  border-radius: 5px;
  margin-right: 5px;
  cursor: pointer;
}
.btn-danger {
  background-color: #dc3545;
  color: white;
  padding: 5px 10px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}
table {
  width: 100%;
  border-collapse: collapse;
}
th, td {
  padding: 10px;
  border: 1px solid #ddd;
}
th {
  background-color: #007bff;
  color: white;
}
</style>