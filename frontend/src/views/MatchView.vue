<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-blue-500 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Match Management</h1>
    </header>

    <main class="container mx-auto p-5">
      <section class="mb-10">
        <h2 class="text-xl font-bold mb-3">Create a Match</h2>
        <div class="flex flex-col gap-4 bg-white shadow-md p-5 rounded-md">
          <input v-model="newMatch.homeTeamId" type="number" placeholder="Home Team ID" class="border p-2 rounded-md" />
          <input v-model="newMatch.awayTeamId" type="number" placeholder="Away Team ID" class="border p-2 rounded-md" />
          <input v-model="newMatch.starttime" type="datetime-local" placeholder="Match Start Time"
            class="border p-2 rounded-md" />
          <button @click="createMatch" class="bg-blue-500 text-white p-2 rounded-md">
            Create Match
          </button>
        </div>
      </section>

      <section>
        <h2 class="text-xl font-bold mb-3">Match List</h2>
        <div v-if="matches.length" class="bg-white shadow-md p-5 rounded-md">
          <div v-for="match in matches" :key="match.id" class="mb-4 p-4 border rounded-md bg-gray-50">
            <h3 class="text-lg font-bold mb-2">
              {{ match.homeTeam.name }} vs. {{ match.awayTeam.name }}
            </h3>
            <p class="text-sm mb-2">
              {{ new Date(match.starttime).toLocaleString() }}
            </p>
            <p class="text-sm mb-2">
              Score: {{ match.team1Score }} - {{ match.team2Score }}
            </p>
            <p class="text-sm mb-2">
              Status:
              <span :class="{
                'text-green-600': match.isFinished,
                'text-red-600': !match.isFinished,
              }">
                {{ match.isFinished ? "Finished" : "Upcoming" }}
              </span>
            </p>
            <button v-if="!match.isFinished && isAdmin" @click="updateMatchResult(match.id)"
              class="bg-green-500 text-white p-2 rounded-md mr-2">
              Update Result
            </button>
            <button v-if="isAdmin" @click="deleteMatch(match.id)" class="bg-red-500 text-white p-2 rounded-md">
              Delete Match
            </button>
          </div>
        </div>
        <div v-else class="text-center text-gray-500 mt-5">
          No matches available. Create one to get started.
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
      token: "",
      isAdmin: false,
      matches: [],
      newMatch: {
        homeTeamId: null,
        awayTeamId: null,
        starttime: "",
      },
    };
  },
  methods: {
    async fetchMatches() {
      try {
        const response = await axios.get("http://localhost:5117/api/match", {
          headers: { Authorization: `Bearer ${this.token}` },
        });
        this.matches = response.data;
      } catch (error) {
        alert(error.response?.data || "Failed to fetch matches.");
      }
    },
    async createMatch() {
      try {
        await axios.post("http://localhost:5117/api/match", this.newMatch, {
          headers: { Authorization: `Bearer ${this.token}` },
        });
        alert("Match created successfully!");
        this.fetchMatches();
      } catch (error) {
        alert(error.response?.data || "Failed to create match.");
      }
    },
    async updateMatchResult(matchId) {
      const team1Score = prompt("Enter score for Home Team:");
      const team2Score = prompt("Enter score for Away Team:");
      if (team1Score === null || team2Score === null) return;

      try {
        await axios.put(
          `http://localhost:5000/api/match/${matchId}/result`,
          { team1Score: parseInt(team1Score), team2Score: parseInt(team2Score) },
          { headers: { Authorization: `Bearer ${this.token}` } }
        );
        alert("Match result updated successfully!");
        this.fetchMatches();
      } catch (error) {
        alert(error.response?.data || "Failed to update match result.");
      }
    },
    async deleteMatch(matchId) {
      try {
        await axios.delete(`http://localhost:5000/api/match/${matchId}`, {
          headers: { Authorization: `Bearer ${this.token}` },
        });
        alert("Match deleted successfully!");
        this.fetchMatches();
      } catch (error) {
        alert(error.response?.data || "Failed to delete match.");
      }
    },
  },
  created() {
    this.fetchMatches();
  },
};
</script>

<style>
body {
  font-family: Arial, sans-serif;
}
</style>