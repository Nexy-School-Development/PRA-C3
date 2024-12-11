<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-blue-600 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Admin Dashboard</h1>
    </header>

    <main class="container mx-auto p-5">
      <section class="mb-10">
        <h2 class="text-xl font-bold mb-4">Users</h2>
        <table class="table-auto w-full bg-white shadow-md rounded-lg text-left">
          <thead>
            <tr class="bg-blue-600 text-white">
              <th class="p-3">ID</th>
              <th class="p-3">Email</th>
              <th class="p-3">Balance</th>
              <th class="p-3">Is Admin</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="user in users" :key="user.id" class="border-t">
              <td class="p-3">{{ user.id }}</td>
              <td class="p-3">{{ user.email }}</td>
              <td class="p-3">{{ user.balance }}</td>
              <td class="p-3">{{ user.isAdmin }}</td>
            </tr>
          </tbody>
        </table>
      </section>
    </main>
  </div>
</template>

<script>
import apiClient from "../axios";

export default {
  data() {
    return {
      users: [],
    };
  },
  methods: {
    async fetchUsers() {
      try {
        const response = await apiClient.get("/user");
        this.users = response.data;
      } catch (error) {
        console.error("Error fetching users", error);
      }
    },
  },
  created() {
    this.fetchUsers();
  },
};
</script>

<style>
body {
  font-family: "Inter", sans-serif;
}
table {
  width: 100%;
  border-collapse: collapse;
}
th,
td {
  border: 1px solid #e2e8f0;
  padding: 12px;
}
th {
  background-color: #2b6cb0;
  color: white;
}
</style>
