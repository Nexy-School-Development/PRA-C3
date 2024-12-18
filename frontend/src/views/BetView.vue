<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-indigo-600 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Welcome to the Tournament Platform</h1>
      <h3 v-if="errorMessage" class="alert text-red-500">{{ errorMessage }}</h3>
    </header>

    <main class="container mx-auto p-5">
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

              <div v-if="match.selectedTeam">
                <label for="bet-amount">Amount to Bet:</label>
                <input type="number" v-model="match.betAmount" id="bet-amount" min="1" :max="userBalance" placeholder="Enter bet amount" />
              </div>

              <button @click="placeBet(match)" :disabled="!match.selectedTeam || !match.betAmount || match.betAmount <= 0">
                Place Bet
              </button>
            </div>

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

          // Ensure HomeTeamId and AwayTeamId are integers
          match.HomeTeamId = parseInt(match.HomeTeamId, 10);
          match.AwayTeamId = parseInt(match.AwayTeamId, 10);

          // Set team names after fetching teams
          match.HomeTeamName = this.getTeamName(match.HomeTeamId);
          match.AwayTeamName = this.getTeamName(match.AwayTeamId);
          
          match.selectedTeam = ""; 
          match.betAmount = "";
          return match;
        });
      } catch (error) {
        console.error("Error fetching matches:", error); 
        this.errorMessage = "Error fetching matches.";
      }
    },

    async fetchTeams() {
      try {
        console.log("Fetching teams...");
        const response = await axios.get("http://localhost/pra-c3/frontend/database/getTeams.php");
        console.log("Fetched teams:", response.data);
        this.teams = response.data.map(team => ({
          ...team,
          Id: parseInt(team.Id, 10) // Ensure team Id is an integer
        }));
        this.fetchMatches();
      } catch (error) {
        console.error("Error fetching teams:", error); 
        this.errorMessage = "Error fetching teams.";
      }
    },

    getTeamName(teamId) {
      const team = this.teams.find(t => t.Id === teamId);
      return team ? team.Name : "Unknown Team";
    },

async placeBet(match) {
  if (!match.selectedTeam || !match.betAmount || match.betAmount <= 0) {
    alert("Please select a team and enter a valid bet amount.");
    return;
  }

  if (match.betAmount > this.userBalance) {
    alert("Insufficient balance!");
    return;
  }

  // Subtract the bet amount from the user's balance on the frontend
  this.userBalance -= match.betAmount;

  // Prepare the bet object
  const bet = {
    MatchId: parseInt(match.Id, 10), // Convert match Id to integer
    Prediction: match.selectedTeam,
    Amount: match.betAmount
  };

  console.log("Placing bet:", bet);
  try {
    const token = localStorage.getItem("userToken");
    console.log("Token:", token);

    const response = await axios.post("http://localhost:5116/api/bet", bet, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
      params: {
        userId: this.user.id  // Pass user ID in the params
      }
    });

    // If the bet is successfully placed, update the user balance in localStorage
    this.user.balance = this.userBalance;
    localStorage.setItem("user", JSON.stringify(this.user));

    console.log("Bet placed successfully:", response.data);
    alert("Bet placed successfully!");
  } catch (error) {
    console.error("Error placing bet:", error);
    // Revert the balance update if there's an error
    this.userBalance += match.betAmount;  // Add back the bet amount
    this.errorMessage = error.response?.data || "An error occurred while placing the bet.";
  }
},

    fetchUserData() {
      const user = JSON.parse(localStorage.getItem("user"));
      if (user) {
        this.user = user;
        this.userBalance = user.balance; 
        console.log("Fetched user data:", this.user); 
      } else {
        this.errorMessage = "User data not found!";
        console.error("User data not found!");
      }
    },

    logout() {
      localStorage.removeItem("user");
      localStorage.removeItem("userToken");
      this.user = null;
      this.userBalance = 69; 
      console.log("User logged out."); 
    }
  },
  created() {
    console.log("Component created.");
    this.fetchTeams();
    this.fetchUserData();
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
