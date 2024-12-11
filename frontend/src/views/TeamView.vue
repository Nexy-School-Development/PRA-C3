<template>
    <div class="min-h-screen bg-gray-100">
      <header class="bg-blue-500 text-white p-5 shadow-lg">
        <h1 class="text-3xl font-bold text-center">Team Management</h1>
      </header>
  
      <main class="container mx-auto p-5">
        <section class="mb-10">
          <h2 class="text-xl font-bold mb-3">Create a Team</h2>
          <div class="flex flex-col gap-4 bg-white shadow-md p-5 rounded-md">
            <input
              v-model="token"
              type="text"
              placeholder="Enter your token"
              class="border p-2 rounded-md"
            />
            <input
              v-model="newTeam.name"
              type="text"
              placeholder="Team Name"
              class="border p-2 rounded-md"
            />
            <button @click="createTeam" class="bg-blue-500 text-white p-2 rounded-md">
              Create Team
            </button>
          </div>
        </section>
  
        <section>
          <h2 class="text-xl font-bold mb-3">Team List</h2>
          <div v-if="teams.length" class="bg-white shadow-md p-5 rounded-md">
            <div
              v-for="team in teams"
              :key="team.id"
              class="mb-4 p-4 border rounded-md bg-gray-50"
            >
              <h3 class="text-lg font-bold mb-2">{{ team.name }} (Points: {{ team.points }})</h3>
              <p class="mb-2 text-sm">Players:</p>
              <ul class="mb-2 text-sm">
                <li v-for="player in team.players" :key="player.id">
                  {{ player.email }}
                </li>
              </ul>
              <button
                @click="deleteTeam(team.id)"
                class="bg-red-500 text-white p-2 rounded-md"
              >
                Delete Team
              </button>
            </div>
          </div>
          <div v-else class="text-center text-gray-500 mt-5">
            No teams available. Create one to get started.
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
        newTeam: {
          name: "",
          points: 0,
        },
        teams: [],
      };
    },
    methods: {
      async fetchTeams() {
        try {
          const response = await axios.get("http://localhost:5117/api/team", {
            headers: { token: this.token },
          });
          this.teams = response.data;
        } catch (error) {
          alert(error.response?.data || "Failed to fetch teams.");
        }
      },
      async createTeam() {
        if (!this.newTeam.name) {
          alert("Team name is required.");
          return;
        }
        try {
          await axios.post("http://localhost:5117/api/team", this.newTeam, {
            headers: { token: this.token },
          });
          alert("Team created successfully!");
          this.newTeam.name = "";
          this.fetchTeams();
        } catch (error) {
          alert(error.response?.data || "Failed to create team.");
        }
      },
      async deleteTeam(teamId) {
        try {
          await axios.delete(`http://localhost:5117/api/team/${teamId}`, {
            headers: { token: this.token },
          });
          alert("Team deleted successfully!");
          this.fetchTeams();
        } catch (error) {
          alert(error.response?.data || "Failed to delete team.");
        }
      },
    },
    created() {
      this.fetchTeams();
    },
  };
  </script>
  
  <style>
  body {
    font-family: Arial, sans-serif;
  }
  </style>
  