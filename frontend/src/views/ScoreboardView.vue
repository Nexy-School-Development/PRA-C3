<template>
  <div class="min-h-screen bg-gray-100 p-6">
    <div class="container mx-auto">
      <h1 class="text-3xl font-bold text-center mb-8">Team Scoreboard</h1>
      <div class="bg-white shadow-md p-6 rounded-lg">
        <table class="table-auto w-full text-left border-collapse">
          <thead>
            <tr class="bg-blue-600 text-white">
              <th class="p-3 border">Place</th>
              <th class="p-3 border">Name</th>
              <th class="p-3 border">Points</th>
              <th v-if="isLoggedIn" class="p-3 border">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(team, index) in sortedTeams" :key="team.Id" class="border-t">
              <td class="p-3 border">{{ index + 1 }}</td>
              <td class="p-3 border">{{ team.Name }}</td>
              <td class="p-3 border">
                <div v-if="editMode[team.Id]">
                  <input
                    v-model.number="team.Points"
                    type="number"
                    class="border px-2 py-1 rounded w-16"
                  />
                </div>
                <div v-else>
                  {{ team.Points }}
                </div>
              </td>
              <td v-if="isLoggedIn" class="p-3 border">
                <button
                  v-if="!editMode[team.Id]"
                  @click="enableEdit(team.Id)"
                  class="btn-secondary"
                >
                  Edit Points
                </button>
                <button
                  v-else
                  @click="savePoints(team)"
                  class="btn-primary"
                >
                  Save
                </button>
                <button
                  v-if="editMode[team.Id]"
                  @click="cancelEdit(team.Id)"
                  class="btn-danger ml-2"
                >
                  Cancel
                </button>
              </td>
            </tr>
          </tbody>
        </table>
        <p v-if="sortedTeams.length === 0" class="text-center text-gray-500 mt-4">
          No teams available
        </p>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      teams: [], // Array to hold the team data
      editMode: {}, // Track which teams are in edit mode
      isLoggedIn: true, // Simulating user logged-in status
    };
  },
  computed: {
    // Sort teams by points in descending order
    sortedTeams() {
      return this.teams.sort((a, b) => b.Points - a.Points);
    },
  },
  methods: {
    // Fetch teams from the API
    async fetchTeams() {
      try {
        const response = await axios.get("http://localhost/pra-c3/frontend/database/getTeams.php");
        this.teams = response.data;
        // Initialize edit mode for all teams as false
        this.editMode = this.teams.reduce((acc, team) => {
          acc[team.Id] = false;
          return acc;
        }, {});
      } catch (error) {
        console.error("Error fetching team data:", error);
      }
    },
    // Enable editing for a specific team
    enableEdit(teamId) {
      this.editMode[teamId] = true;
    },
    // Save updated points for a specific team
    async savePoints(team) {
      try {
        await axios.put(
          `http://localhost:5116/api/Team/changePoints/${team.Id}`,
          team.Points,
          {
            headers: {
              "Content-Type": "application/json",
            },
          }
        );
        console.log(`Points updated successfully for team ${team.Id}`);
        this.editMode[team.Id] = false; // Disable edit mode after saving
      } catch (error) {
        console.error("Error saving points:", error);
      }
    },
    // Cancel editing for a specific team
    cancelEdit(teamId) {
      this.editMode[teamId] = false;
      // Re-fetch teams to reset the points
      this.fetchTeams();
    },
  },
  mounted() {
    this.fetchTeams();
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
  border: 1px solid #ddd;
  text-align: left;
}

th {
  background-color: #f8f9fa;
}

.p-3 {
  padding: 12px;
}

.bg-blue-600 {
  background-color: #007bff;
}

.btn-primary {
  background-color: #007bff;
  color: white;
  padding: 6px 12px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
  padding: 6px 12px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
}

.btn-danger {
  background-color: #dc3545;
  color: white;
  padding: 6px 12px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
}
</style>
