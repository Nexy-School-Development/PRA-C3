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
          <!-- Single table for all users -->
          <table class="table-auto w-full text-left bg-gray-900 text-white rounded-lg overflow-hidden">
            <thead>
              <tr class="bg-blue-600">
                <th class="p-3">ID</th>
                <th class="p-3">Email</th>
                <th class="p-3">Admin</th>
                <th class="p-3">Balance</th>
                <th class="p-3">Actions</th>
              </tr>
            </thead>
            <tbody>
              <!-- Iterate over users and create a row for each -->
              <tr v-for="user in users" :key="user.Id" class="border-t border-gray-700">
                <td class="p-3">{{ user.Id }}</td>
                <td class="p-3">{{ user.Email }}</td>
                <td class="p-3">{{ formatAdmin(user.IsAdmin) }}</td>
                <td class="p-3">{{ formatBalance(user.Balance) }}</td>
                <td class="p-3">
                  <button @click="editUser(user.Id)" class="btn-primary mr-2">Edit</button>
                  <button @click="deleteUser(user.Id)" class="btn-danger">Delete</button>
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
import axios from 'axios';
import { mapActions } from 'vuex';

export default {
  data() {
    return {
      users: [], // Will hold user data fetched from API
    };
  },
  methods: {
    ...mapActions(['logout', 'checkUser']),
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
    async deleteUser(id) {
      try {
        await axios.delete(`http://localhost:5116/api/User/${id}`);
        this.fetchDashboardData(); // Refresh the user list after deletion
      } catch (error) {
        console.error('Error deleting user:', error.response || error.message);
      }
    },
    editUser(id) {
      this.$router.push({ name: 'UserEdit', params: { id } });
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
  padding: 5px 10px;
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
  background-color: #2563eb;
  color: white;
}
tbody tr {
  border-top: 1px solid #4b5563;
}
tbody td {
  background-color: #1f2937;
  color: white;
}
</style>