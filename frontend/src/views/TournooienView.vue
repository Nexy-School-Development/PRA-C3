<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-purple-600 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Tournament Management</h1>
    </header>

    <main class="container mx-auto p-5">
      <section class="mb-10">
        <h2 class="text-xl font-bold mb-4">Generate Schedule</h2>
        <div class="bg-white shadow-md p-6 rounded-lg">
          <input v-model.number="fieldsAvailable" type="number" placeholder="Fields Available" class="input-field" />
          <button @click="generateSchedule" class="btn-primary">Generate</button>
        </div>
      </section>

      <section>
        <h2 class="text-xl font-bold mb-4">View Schedule</h2>
        <div v-if="tourney" class="bg-white shadow-md p-6 rounded-lg">
          <h3 class="text-lg font-bold mb-4">{{ tourney.name }}</h3>
          <table class="table-auto w-full text-left">
            <thead>
              <tr class="bg-purple-600 text-white">
                <th class="p-3">Home Team</th>
                <th class="p-3">Away Team</th>
                <th class="p-3">Start Time</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="match in tourney.matches" :key="match.id" class="border-t">
                <td class="p-3">{{ match.HomeTeam }}</td>
                <td class="p-3">{{ match.AwayTeam }}</td>
                <td class="p-3">{{ new Date(match.StartTime).toLocaleString() }}</td>
              </tr>
            </tbody>
          </table>
        </div>
        <div v-else class="text-center text-gray-500 mt-5">No schedule available. Generate one to view it here.</div>
      </section>
    </main>
  </div>
</template>

<script>
import apiClient from "@/axios";

export default {
  data() {
    return {
      fieldsAvailable: 1,
      tourney: null,
    };
  },
  methods: {
    async generateSchedule() {
      try {
        await apiClient.post("/api/tourney/generate-schedule", {
          fieldsAvailable: this.fieldsAvailable,
        });
        this.fetchSchedule();
      } catch (error) {
        console.error("Error generating schedule", error);
      }
    },
    async fetchSchedule() {
      try {
        const response = await apiClient.get("/api/tourney/schedule");
        this.tourney = response.data;
      } catch (error) {
        console.error("Error fetching schedule", error);
      }
    },
  },
  created() {
    this.fetchSchedule();
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
  background-color: #6f42c1;
  color: white;
  padding: 10px 15px;
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
  background-color: #6f42c1;
  color: white;
}
</style>
