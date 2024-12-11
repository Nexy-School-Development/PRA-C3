<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-purple-600 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Tournament Management</h1>
    </header>

    <main class="container mx-auto p-5">
      <section class="mb-10">
        <h2 class="text-xl font-bold mb-4">Generate Tournament Schedule</h2>
        <div class="bg-white shadow-md p-6 rounded-lg">
          <input
            v-model.number="fieldsAvailable"
            type="number"
            placeholder="Number of Fields Available"
            class="input-field"
          />
          <button @click="generateSchedule" class="btn-primary">Generate Schedule</button>
        </div>
      </section>

      <section>
        <h2 class="text-xl font-bold mb-4">Tournament Schedule</h2>
        <div class="bg-white shadow-md p-6 rounded-lg">
          <div v-if="tourney && tourney.matches.length" class="overflow-auto">
            <table class="table-auto w-full text-left">
              <thead>
                <tr class="bg-purple-600 text-white">
                  <th class="p-3">Home Team</th>
                  <th class="p-3">Away Team</th>
                  <th class="p-3">Start Time</th>
                  <th class="p-3">Status</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(match, index) in tourney.matches" :key="index" class="border-t">
                  <td class="p-3">{{ match.HomeTeam }}</td>
                  <td class="p-3">{{ match.AwayTeam }}</td>
                  <td class="p-3">{{ new Date(match.StartTime).toLocaleString() }}</td>
                  <td class="p-3">
                    <span
                      :class="{
                        'text-green-600': match.IsFinished,
                        'text-red-600': !match.IsFinished,
                      }"
                    >
                      {{ match.IsFinished ? "Finished" : "Upcoming" }}
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div v-else class="text-gray-500 text-center mt-4">No schedule available. Generate one to get started.</div>
        </div>
      </section>
    </main>
  </div>
</template>

<script>
import apiClient from "../axios";

export default {
  data() {
    return {
      fieldsAvailable: 1,
      tourney: {
        matches: [],
      },
    };
  },
  methods: {
    async generateSchedule() {
      if (this.fieldsAvailable < 1) {
        alert("Please specify at least one field.");
        return;
      }

      try {
        const response = await apiClient.post("/tourney/generate-schedule", {
          fieldsAvailable: this.fieldsAvailable,
        });
        this.tourney = response.data;
        alert("Tournament schedule generated successfully!");
      } catch (error) {
        console.error("Error generating schedule", error.response?.data || error);
        alert("Failed to generate schedule.");
      }
    },
    async fetchSchedule() {
      try {
        const response = await apiClient.get("/tourney/schedule");
        this.tourney = response.data;
      } catch (error) {
        console.error("Error fetching schedule", error.response?.data || error);
        alert("Failed to fetch schedule.");
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
.btn-primary:hover {
  background-color: #5a369d;
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
