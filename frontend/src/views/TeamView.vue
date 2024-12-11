<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-green-600 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Team Management</h1>
    </header>

    <main class="container mx-auto p-5">
      <section class="mb-10">
        <h2 class="text-xl font-bold mb-4">Create a Team</h2>
        <div class="bg-white shadow-md p-6 rounded-lg">
          <input v-model="newTeam.name" type="text" placeholder="Team Name" class="input-field" />
          <button @click="createTeam" class="btn-primary">Create Team</button>
        </div>
      </section>

      <section>
        <h2 class="text-xl font-bold mb-4">Team List</h2>
        <div class="bg-white shadow-md p-6 rounded-lg">
          <table class="table-auto w-full text-left">
            <thead>
              <tr class="bg-green-600 text-white">
                <th class="p-3">Name</th>
                <th class="p-3">Points</th>
                <th class="p-3">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="team in teams" :key="team.id" class="border-t">
                <td class="p-3">{{ team.name }}</td>
                <td class="p-3">{{ team.points }}</td>
                <td class="p-3">
                  <button @click="deleteTeam(team.id)" class="btn-danger">Delete</button>
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
import apiClient from "@/axios";

export default {
  data() {
    return {
      teams: [],
      newTeam: {
        name: "",
      },
    };
  },
  methods: {
    async fetchTeams() {
      try {
        const response = await apiClient.get("/api/team");
        this.teams = response.data;
      } catch (error) {
        console.error("Error fetching teams", error);
      }
    },
    async createTeam() {
      if (!this.newTeam.name.trim()) {
        alert("Team name is required.");
        return;
      }
      try {
        await apiClient.post("/api/team", this.newTeam);
        this.newTeam.name = "";
        this.fetchTeams();
      } catch (error) {
        console.error("Error creating team", error);
      }
    },
    async deleteTeam(teamId) {
      try {
        await apiClient.delete(`/api/team/${teamId}`);
        this.fetchTeams();
      } catch (error) {
        console.error("Error deleting team", error);
      }
    },
  },
  created() {
    this.fetchTeams();
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
  background-color: #28a745;
  color: white;
  padding: 10px 15px;
  border: none;
  border-radius: 5px;
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
th,
td {
  padding: 10px;
  border: 1px solid #ddd;
}
th {
  background-color: #28a745;
  color: white;
}
</style>
