<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-gray-800 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Admin Dashboard</h1>
    </header>

    <main class="container mx-auto p-5">
      <section>
        <h2 class="text-xl font-bold mb-4">Users</h2>
        <div class="bg-white shadow-md p-6 rounded-lg">
          <table class="table-auto w-full text-left">
            <thead>
              <tr class="bg-gray-800 text-white">
                <th class="p-3">ID</th>
                <th class="p-3">Email</th>
                <th class="p-3">Admin</th>
                <th class="p-3">Balance</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="user in users" :key="user.id" class="border-t">
                <td class="p-3">{{ user.id }}</td>
                <td class="p-3">{{ user.email }}</td>
                <td class="p-3">{{ user.isAdmin }}</td>
                <td class="p-3">{{ user.balance }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <section class="mt-8">
        <h2 class="text-xl font-bold mb-4">Teams</h2>
        <div class="bg-white shadow-md p-6 rounded-lg">
          <table class="table-auto w-full text-left">
            <thead>
              <tr class="bg-gray-800 text-white">
                <th class="p-3">ID</th>
                <th class="p-3">Name</th>
                <th class="p-3">Points</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="team in teams" :key="team.id" class="border-t">
                <td class="p-3">{{ team.id }}</td>
                <td class="p-3">{{ team.name }}</td>
                <td class="p-3">{{ team.points }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <section class="mt-8">
        <h2 class="text-xl font-bold mb-4">Matches</h2>
        <div class="bg-white shadow-md p-6 rounded-lg">
          <table class="table-auto w-full text-left">
            <thead>
              <tr class="bg-gray-800 text-white">
                <th class="p-3">ID</th>
                <th class="p-3">Home Team</th>
                <th class="p-3">Away Team</th>
                <th class="p-3">Start Time</th>
                <th class="p-3">Finished</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="match in matches" :key="match.id" class="border-t">
                <td class="p-3">{{ match.id }}</td>
                <td class="p-3">{{ match.homeTeam.name }}</td>
                <td class="p-3">{{ match.awayTeam.name }}</td>
                <td class="p-3">{{ new Date(match.starttime).toLocaleString() }}</td>
                <td class="p-3">{{ match.isFinished ? 'Yes' : 'No' }}</td>
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
      users: [],
      teams: [],
      matches: [],
    };
  },
  methods: {
    async fetchDashboardData() {
      try {
        const [usersResponse, teamsResponse, matchesResponse] = await Promise.all([
          apiClient.get("/api/User"),
          apiClient.get("/api/Team"),
          apiClient.get("/api/Match"),
        ]);

        this.users = usersResponse.data;
        this.teams = teamsResponse.data;
        this.matches = matchesResponse.data;
      } catch (error) {
        console.error("Error fetching dashboard data", error);
      }
    },
  },
  created() {
    this.fetchDashboardData();
  },
};
</script>

<style>
.table-auto {
  width: 100%;
  border-collapse: collapse;
}
th,
td {
  padding: 10px;
  border: 1px solid #ddd;
}
th {
  background-color: #333;
  color: white;
}
</style>
