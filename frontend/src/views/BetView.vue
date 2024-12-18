<template>
  <div class="min-h-screen bg-gray-100">
    <!-- Header -->
    <header class="bg-indigo-600 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Welcome to the Tournament Platform</h1>
      <h3 v-if="errorMessage" class="alert text-red-500">{{ errorMessage }}</h3>
    </header>

    <!-- Main Content -->
    <main class="container mx-auto p-5">
      <!-- User Information Section -->
      <section class="bg-white shadow-md p-6 rounded-lg">
        <div v-if="user">
          <h2 class="text-xl font-bold mb-3">Hello, {{ user.email }}</h2>
          <p>Your current balance: <strong>{{ user.balance }}</strong></p>
          <button @click="logout" class="btn-danger mt-4 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-800">
            Logout
          </button>
        </div>
        <div v-else>
          <p>You are not logged in.</p>
          <router-link to="/login" class="btn-primary mr-3 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-800">Login</router-link>
          <router-link to="/register" class="btn-secondary px-4 py-2 bg-gray-600 text-white rounded-lg hover:bg-gray-800">Register</router-link>
        </div>
      </section>

      <section class="mt-6">
        <h1>Available Matches</h1>

        <div class="filters">
          <label>
            <input type="checkbox" v-model="filterFinished" />
            Show Finished Matches
          </label>
          <label>
            <input type="checkbox" v-model="filterUpcoming" />
            Show Upcoming Matches
          </label>
        </div>

        <!-- Match Display -->
        <div v-if="matches.length">
          <div v-for="match in filteredMatches" :key="match.Id" class="match-card bg-white p-4 mb-4 shadow-md rounded-lg">
            <p>{{ match.HomeTeamName }} vs {{ match.AwayTeamName }}</p>
            <p>{{ new Date(match.Starttime).toLocaleString() }}</p>
            <p>Status: {{ match.IsFinished ? 'Finished' : 'Upcoming' }}</p>

            <div v-if="!match.IsFinished">
              <label for="team-select">Bet on:</label>
              <select v-model="match.selectedTeam" id="team-select">
                <option disabled value="">Select a team</option>
                <option :value="'home'">{{ match.HomeTeamName }}</option>
                <option :value="'away'">{{ match.AwayTeamName }}</option>
              </select>

              <!-- Input field for betting amount -->
              <div v-if="match.selectedTeam">
                <label for="bet-amount">Amount to Bet:</label>
                <input type="number" v-model="match.betAmount" id="bet-amount" min="1" :max="userBalance" placeholder="Enter bet amount" />
              </div>

              <!-- Button to place bet -->
              <button @click="placeBet(match.Id, match.selectedTeam, match.betAmount)" :disabled="!match.selectedTeam || !match.betAmount || match.betAmount <= 0">
                Place Bet
              </button>
            </div>

            <!-- Message for finished matches -->
            <div v-else>
              <p>Betting is closed for this match as it is finished.</p>
            </div>
          </div>
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
      matches: [],
      teams: [],
      user: null,
      userBalance: 1000, 
      filterFinished: false,
      filterUpcoming: false,
      errorMessage: "",
    };
  },
  computed: {
    filteredMatches() {
      return this.matches.filter(match => {
        const showFinished = this.filterFinished && match.IsFinished;
        const showUpcoming = this.filterUpcoming && !match.IsFinished;

        return (showFinished || showUpcoming); 
      });
    }
  },
  methods: {
    async fetchMatches() {
      try {
        console.log("Fetching matches...");
        const response = await axios.get("http://localhost/pra-c3/frontend/database/getMatches.php");
        console.log("Fetched matches:", response.data);
        this.matches = response.data.map((match) => {
          const currentTime = new Date();
          const matchStartTime = new Date(match.Starttime);
          match.IsFinished = currentTime > matchStartTime;

          // Set team names after fetching teams
          match.HomeTeamName = this.getTeamName(match.HomeTeamId);
          match.AwayTeamName = this.getTeamName(match.AwayTeamId);
          
          match.selectedTeam = ""; // Initially no team selected
          match.betAmount = ""; // Initially no bet amount entered
          return match;
        });
      } catch (error) {
        console.error("Error fetching matches:", error); // Log error
        this.errorMessage = "Error fetching matches.";
      }
    },

    async fetchTeams() {
      try {
        console.log("Fetching teams..."); // Log before API request
        const response = await axios.get("http://localhost/pra-c3/frontend/database/getTeams.php");
        console.log("Fetched teams:", response.data); // Log the response
        this.teams = response.data;
        // After teams are fetched, proceed with fetching matches
        this.fetchMatches();
      } catch (error) {
        console.error("Error fetching teams:", error); // Log error
        this.errorMessage = "Error fetching teams.";
      }
    },

    getTeamName(teamId) {
      const team = this.teams.find(t => t.Id === teamId);
      return team ? team.Name : "Unknown Team";
    },

    async placeBet(matchId, selectedTeam, betAmount) {
      if (!selectedTeam || !betAmount || betAmount <= 0) {
        alert("Please select a team and enter a valid bet amount.");
        return;
      }

      const bet = {
        Match: { Id: matchId },
        Prediction: selectedTeam,
        Amount: betAmount,
        UserId: this.user.Id 
      };

      console.log("Placing bet:", bet); // Log bet details before sending API request
      try {
        const token = localStorage.getItem("userToken");
        console.log("Token:", token); // Log token being sent for authentication

        const response = await axios.post("http://localhost:5116/api/bet", bet, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        console.log("Bet placed successfully:", response.data); // Log API response
        alert("Bet placed successfully!");
      } catch (error) {
        console.error("Error placing bet:", error); // Log error if bet fails
        this.errorMessage = error.response?.data || "An error occurred while placing the bet.";
      }
    },

    fetchUserData() {
      const user = JSON.parse(localStorage.getItem("user"));
      if (user) {
        this.user = user; // Get user data from localStorage (or API)
        this.userBalance = user.balance; // Set user balance
        console.log("Fetched user data:", this.user); // Log the fetched user data
      } else {
        this.errorMessage = "User data not found!";
        console.error("User data not found!"); // Log error if no user data
      }
    },

    logout() {
      localStorage.removeItem("user");
      localStorage.removeItem("userToken"); // Remove the token upon logout
      this.user = null;
      this.userBalance = 1000; // Reset balance
      console.log("User logged out."); // Log logout action
    }
  },
  created() {
    console.log("Component created."); // Log when the component is created
    this.fetchTeams(); // Fetch teams first
    this.fetchUserData(); // Fetch user data on load
  }
};
</script>

<style scoped>
.error-message {
  color: red;
  font-size: 16px;
}

select, input[type="number"] {
  margin: 5px;
  padding: 5px;
}

.filters {
  margin-bottom: 20px;
}

.filters label {
  margin-right: 20px;
}

button:disabled {
  background-color: #ccc;
}

.user-info {
  margin-bottom: 20px;
  font-size: 18px;
}

.match-card {
  margin-bottom: 20px;
}

.btn-primary {
  display: inline-block;
  text-align: center;
  background-color: #4c6ef5;
  padding: 0.5rem 1rem;
  border-radius: 5px;
  color: white;
  font-weight: bold;
  text-decoration: none;
}

.btn-secondary {
  display: inline-block;
  text-align: center;
  background-color: #6c757d;
  padding: 0.5rem 1rem;
  border-radius: 5px;
  color: white;
  font-weight: bold;
  text-decoration: none;
}

.btn-danger {
  display: inline-block;
  text-align: center;
  background-color: #dc3545;
  padding: 0.5rem 1rem;
  border-radius: 5px;
  color: white;
  font-weight: bold;
  cursor: pointer;
}
</style>
