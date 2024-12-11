<template>
  <div class="min-h-screen bg-gray-50">
    <header class="bg-blue-600 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Team Management</h1>
    </header>

    <main class="container mx-auto p-5">
      <!-- Create Team -->
      <section class="mb-10">
        <h2 class="text-xl font-semibold mb-4">Create a New Team</h2>
        <div class="flex flex-col gap-4 bg-white shadow-md p-6 rounded-lg">
          <input
            v-model="newTeam.name"
            type="text"
            placeholder="Team Name"
            class="border border-gray-300 p-3 rounded-lg"
          />
          <button
            @click="createTeam"
            class="bg-blue-600 text-white py-3 px-5 rounded-lg hover:bg-blue-700"
          >
            Create Team
          </button>
          <p v-if="feedback" class="text-sm text-green-600">{{ feedback }}</p>
        </div>
      </section>

      <!-- Team List -->
      <section>
        <h2 class="text-xl font-semibold mb-4">Team List</h2>
        <div v-if="teams.length" class="grid gap-6 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3">
          <div
            v-for="team in teams"
            :key="team.id"
            class="bg-white shadow-md p-6 rounded-lg border border-gray-200"
          >
            <h3 class="text-lg font-bold mb-2">{{ team.name }}</h3>
            <p class="text-gray-600">Points: {{ team.points }}</p>
            <ul class="mt-3">
              <li
                v-for="player in team.players"
                :key="player.id"
                class="text-sm text-gray-500"
              >
                {{ player.email }}
              </li>
            </ul>
            <button
              @click="deleteTeam(team.id)"
              class="mt-4 text-sm text-red-600 hover:underline"
            >
              Delete Team
            </button>
          </div>
        </div>
        <p v-else class="text-center text-gray-500 mt-5">
          No teams found. Start by creating one!
        </p>
      </section>
    </main>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      teams: [],
      newTeam: {
        name: "",
      },
      feedback: "",
    };
  },
  methods: {
    async fetchTeams() {
      try {
        const response = await axios.get("http://localhost:5117/api/team", {
          headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
        });
        this.teams = response.data;
      } catch (error) {
        console.error("Failed to fetch teams", error);
      }
    },
    async createTeam() {
      if (!this.newTeam.name) {
        this.feedback = "Team name is required.";
        return;
      }
      try {
        await axios.post(
          "http://localhost:5117/api/team",
          this.newTeam,
          {
            headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
          }
        );
        this.feedback = "Team created successfully!";
        this.newTeam.name = "";
        this.fetchTeams();
      } catch (error) {
        console.error("Failed to create team", error);
      }
    },
    async deleteTeam(teamId) {
      try {
        await axios.delete(`http://localhost:5117/api/team/${teamId}`, {
          headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
        });
        this.fetchTeams();
      } catch (error) {
        console.error("Failed to delete team", error);
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
  font-family: "Inter", sans-serif;
}
</style>
