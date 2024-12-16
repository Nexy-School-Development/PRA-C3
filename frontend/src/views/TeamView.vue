<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-blue-500 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Team Management</h1>
    </header>

    <main class="container mx-auto p-5">
      <section>
        <h2 class="text-xl font-bold mb-4">Teams</h2>
        <div v-if="teams.length === 0" class="bg-white shadow-md p-6 rounded-lg text-center">
          <p>Not found</p>
        </div>
        <div v-else>
          <!-- Single table for all teams -->
          <table class="table-auto w-full text-left bg-gray-900 text-white rounded-lg overflow-hidden">
            <thead>
              <tr class="bg-blue-600">
                <th class="p-3">ID</th>
                <th class="p-3">Name</th>
                <th class="p-3">Points</th>
                <th class="p-3">Creator ID</th>
                <th class="p-3">Actions</th>
              </tr>
            </thead>
            <tbody>
              <!-- Iterate over teams and create a row for each -->
              <tr v-for="team in teams" :key="team.Id" class="border-t border-gray-700">
                <td class="p-3">{{ team.Id }}</td>
                <td class="p-3">{{ team.Name }}</td>
                <td class="p-3">{{ team.Points }}</td>
                <td class="p-3">{{ team.CreatorId }}</td>
                <td class="p-3">
                  <button @click="deleteTeam(team.Id)" class="btn-danger">Delete</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <section class="bg-white shadow-md p-6 rounded-lg max-w-md mx-auto mt-10">
        <h2 class="text-xl font-bold mb-4">Create New Team</h2>
        <form @submit.prevent="createTeam">
          <input v-model="newTeam.name" type="text" placeholder="Team Name" class="input-field" required />
          <input v-model="newTeam.points" type="number" placeholder="Points" class="input-field" required />
          <input v-model="newTeam.creatorId" type="number" placeholder="Creator ID" class="input-field" required />
          <button type="submit" class="btn-primary">Create Team</button>
        </form>
        <p v-if="errorMessage" class="text-red-500 mt-3">{{ errorMessage }}</p>
        <p v-if="successMessage" class="text-green-500 mt-3">{{ successMessage }}</p>
      </section>
    </main>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      teams: [], // Will hold team data fetched from API
      newTeam: {
        name: '',
        points: 0,
        creatorId: 0
      },
      errorMessage: '',
      successMessage: ''
    };
  },
  methods: {
    async fetchTeams() {
      try {
        // Fetch team data from API
        const response = await axios.get('http://localhost/pra-c3/frontend/database/getTeams.php');
        console.log('API response:', response.data); // Log the response data
        if (response.data && Array.isArray(response.data)) {
          this.teams = response.data; // Bind fetched data to 'teams'
          console.log('Teams data:', this.teams); // Log the teams data
        } else {
          console.error('Invalid API response format:', response.data);
        }
      } catch (error) {
        console.error('Error fetching team data:', error);
        this.teams = []; // Reset teams in case of error
      }
    },
    async createTeam() {
      try {
        const response = await axios.post('http://localhost:5116/api/Team/createteam', this.newTeam, {
          headers: {
            'Content-Type': 'application/json'
          }
        });
        this.successMessage = 'Team created successfully!';
        this.errorMessage = '';
        this.newTeam.name = '';
        this.newTeam.points = 0;
        this.newTeam.creatorId = 0;
        this.fetchTeams(); // Refresh the team list
      } catch (error) {
        this.errorMessage = error.response?.data?.message || 'Failed to create team.';
        this.successMessage = '';
      }
    },
    async deleteTeam(id) {
      try {
        const response = await axios.delete(`http://localhost:5116/api/Team/Delete/${id}`);
        this.successMessage = `Team with ID ${id} deleted successfully.`;
        this.errorMessage = '';
        this.fetchTeams(); 
      } catch (error) {
        console.error('Error deleting team:', error.response || error.message);
        this.errorMessage = error.response?.data || 'Failed to delete team. Make sure you have admin permissions.';
        this.successMessage = '';
      }
    }
  },
  created() {
    this.fetchTeams();
  }
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