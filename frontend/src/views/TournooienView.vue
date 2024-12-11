<template>
    <div class="min-h-screen bg-gray-100">
      <header class="bg-sporty text-white p-5 shadow-lg">
        <h1 class="text-3xl font-bold text-center">Tournament Scheduler</h1>
      </header>
  
      <main class="container mx-auto p-5">
        <section class="mb-10">
          <h2 class="text-xl font-bold mb-3">Generate Schedule</h2>
          <div class="flex flex-col gap-4 bg-white shadow-md p-5 rounded-md">
            <input
              v-model="token"
              type="text"
              placeholder="Enter Admin Token"
              class="border p-2 rounded-md"
            />
            <input
              v-model.number="fieldsAvailable"
              type="number"
              placeholder="Fields Available"
              class="border p-2 rounded-md"
            />
            <button
              @click="generateSchedule"
              class="bg-sporty text-white p-2 rounded-md"
            >
              Generate
            </button>
          </div>
        </section>
  
        <section>
          <h2 class="text-xl font-bold mb-3">View Schedule</h2>
          <div v-if="tourney" class="bg-white shadow-md p-5 rounded-md">
            <h3 class="text-lg font-bold mb-2">{{ tourney.name }}</h3>
            <table class="table-auto w-full text-left border-collapse">
              <thead>
                <tr class="bg-sporty text-white">
                  <th class="p-2">Home Team</th>
                  <th class="p-2">Away Team</th>
                  <th class="p-2">Start Time</th>
                  <th class="p-2">Status</th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="(match, index) in tourney.matches"
                  :key="index"
                  class="border-t"
                >
                  <td class="p-2">{{ match.HomeTeam }}</td>
                  <td class="p-2">{{ match.AwayTeam }}</td>
                  <td class="p-2">{{ new Date(match.StartTime).toLocaleString() }}</td>
                  <td class="p-2">
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
          <div v-else class="text-center text-gray-500 mt-5">
            No schedule available. Generate one or check your token.
          </div>
        </section>
      </main>
    </div>
  </template>
  
  <script>
  import axios from "axios";
  
  export default {
    data() {
      return {
        token: "",
        fieldsAvailable: 1,
        tourney: null,
      };
    },
    methods: {
      async generateSchedule() {
        try {
          const response = await axios.post(
            "http://localhost:5117/api/tourney/generate-schedule",
            { fieldsAvailable: this.fieldsAvailable },
            { headers: { token: this.token } }
          );
          alert("Schedule generated successfully!");
          this.getSchedule();
        } catch (error) {
          alert(error.response?.data || "Failed to generate schedule.");
        }
      },
      async getSchedule() {
        try {
          const response = await axios.get(
            "http://localhost:5117/api/tourney/schedule",
            { headers: { token: this.token } }
          );
          this.tourney = response.data;
        } catch (error) {
          alert(error.response?.data || "Failed to fetch schedule.");
        }
      },
    },
  };
  </script>
  
  <style>
  body {
    font-family: Arial, sans-serif;
  }
  </style>
  