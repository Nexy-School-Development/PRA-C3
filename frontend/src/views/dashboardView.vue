<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-gray-800 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Admin Dashboard</h1>
    </header>

    <main class="container mx-auto p-5">
      <section>
        <h2 class="text-xl font-bold mb-4">Users</h2>
        <div v-if="users.length === 0" class="bg-white shadow-md p-6 rounded-lg text-center">
          <p>Not found</p>
        </div>
        <div v-else>
          <!-- Iterate over users and create a table for each -->
          <table
            v-for="user in users"
            :key="user.Id"
            class="table-auto w-full text-left mb-6 bg-gray-900 text-white rounded-lg overflow-hidden"
          >
            <thead>
              <tr class="bg-blue-600">
                <th class="p-3">ID</th>
                <th class="p-3">Email</th>
                <th class="p-3">Admin</th>
                <th class="p-3">Balance</th>
              </tr>
            </thead>
            <tbody>
              <tr class="border-t border-gray-700">
                <td class="p-3">{{ user.Id }}</td>
                <td class="p-3">{{ user.Email }}</td>
                <td class="p-3">{{ formatAdmin(user.IsAdmin) }}</td>
                <td class="p-3">{{ formatBalance(user.Balance) }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>
    </main>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      users: [], // Will hold user data fetched from API
    };
  },
  methods: {
    async fetchDashboardData() {
      try {
        // Fetch user data from API
        const response = await axios.get('http://localhost/pra-c3/frontend/database/getUsers.php');
        if (response.data && Array.isArray(response.data)) {
          this.users = response.data; // Bind fetched data to 'users'
        } else {
          console.error('Invalid API response format:', response.data);
        }
      } catch (error) {
        console.error('Error fetching user data:', error);
        this.users = []; // Reset users in case of error
      }
    },
    formatBalance(balance) {
      // Format balance to 2 decimal places or return '0.00' if null
      return balance !== null
        ? parseFloat(balance).toFixed(2)
        : '0.00';
    },
    formatAdmin(isAdmin) {
      // Return 'No' if IsAdmin is null
      return isAdmin !== null ? 'Yes' : 'No';
    },
  },
  created() {
    // Fetch data when component is created
    this.fetchDashboardData();
  },
};
</script>

<style>
.table-auto {
  width: 100%;
  border-collapse: collapse;
  margin-bottom: 1.5rem;
}
th,
td {
  padding: 10px;
  text-align: left;
}
thead th {
  background-color: #2563eb; /* Blue header */
  color: white;
}
tbody tr {
  border-top: 1px solid #4b5563; /* Dark gray for row borders */
}
tbody td {
  background-color: #1f2937; /* Darker gray for rows */
  color: white;
}
</style>
