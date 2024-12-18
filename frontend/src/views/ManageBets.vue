<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-indigo-600 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">View Bets</h1>
    </header>

    <main class="container mx-auto p-5">
      <section v-if="finishedMatches.length">
        <h2 class="text-2xl font-bold mb-3">Finished Matches</h2>

        <div v-for="match in finishedMatches" :key="match.MatchId" class="match-card bg-white p-4 mb-4 shadow-md rounded-lg">
          <p>{{ match.HomeTeamName }} vs {{ match.AwayTeamName }}</p>
          <p>Status: Finished</p>

          <button @click="openWinnerPopup(match)" class="btn-primary mt-4 px-4 py-2">
            What team won?
          </button>

          <div v-if="match.showPopup" class="popup-overlay">
            <div class="popup">
              <h3>Select the winning team</h3>
              <button @click="selectWinner(match, 'home')" class="btn-primary mt-4">Home</button>
              <button @click="selectWinner(match, 'away')" class="btn-primary mt-4">Away</button>
              <button @click="selectWinner(match, 'draw')" class="btn-secondary mt-4">Draw</button>
              <button @click="closeWinnerPopup(match)" class="btn-danger mt-4">Cancel</button>
            </div>
          </div>
        </div>

        <!-- Button to resolve all bets -->
        <button @click="resolveBets" class="btn-primary mt-4">Resolve All Bets</button>
      </section>

      <p v-else>No finished matches to display.</p>
    </main>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      finishedMatches: [],
      teams: [],  // To store teams
      errorMessage: "",
    };
  },
  methods: {
    // Fetch the finished matches from the backend (using getMatches.php)
    async fetchFinishedMatches() {
      try {
        // Fetch finished matches
        const matchesResponse = await axios.get("http://localhost/pra-c3/frontend/database/getMatches.php", {
          params: { status: "finished" }  // Send a query parameter to filter finished matches
        });
        const matches = matchesResponse.data;

        // Fetch teams for home and away teams
        const teamsResponse = await axios.get("http://localhost/pra-c3/frontend/database/getTeams.php");
        this.teams = teamsResponse.data;  // Store teams in the component

        // Map the team names to the finished matches
        this.finishedMatches = matches.map(match => {
          const homeTeam = this.teams.find(team => team.TeamId === match.HomeTeamId);
          const awayTeam = this.teams.find(team => team.TeamId === match.AwayTeamId);
          
          return {
            ...match,
            HomeTeamName: homeTeam ? homeTeam.Name : 'Unknown',  // Set team name or default to 'Unknown'
            AwayTeamName: awayTeam ? awayTeam.Name : 'Unknown',  // Set team name or default to 'Unknown'
            showPopup: false  // Initially hide the popup
          };
        });
      } catch (error) {
        console.error("Error fetching finished matches or teams:", error);
        this.errorMessage = "Error fetching finished matches or teams.";
      }
    },

    // Open the winner selection popup for a match
    openWinnerPopup(match) {
      match.showPopup = true;
    },

    // Close the winner selection popup
    closeWinnerPopup(match) {
      match.showPopup = false;
    },

    // Handle the selection of the winning team
    async selectWinner(match, winner) {
      try {
        // Call the backend to process the bets and update balances
        const response = await axios.post("http://localhost:5116/api/processBets", {
          matchId: match.MatchId,
          winner: winner
        });

        // Show the result of the processing
        alert(response.data.message);

        // Refresh the list of finished matches after processing
        this.fetchFinishedMatches();
      } catch (error) {
        console.error("Error selecting winner:", error);
        this.errorMessage = "Error selecting the winner.";
      }

      // Close the popup after selection
      match.showPopup = false;
    },

    // Resolve all bets by calling the backend
    async resolveBets() {
      try {
        const response = await axios.post("http://localhost:5116/api/resolve-bets");
        alert(response.data);  // Show success message
      } catch (error) {
        console.error("Error resolving bets:", error);
        alert("There was an error resolving the bets.");
      }
    }
  },
  created() {
    // Fetch the finished matches when the component is created
    this.fetchFinishedMatches();
  }
};
</script>

<style scoped>
.match-card {
  margin-bottom: 20px;
}

.popup-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
}

.popup {
  background-color: white;
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
}

button {
  display: inline-block;
  padding: 10px;
  margin: 5px;
  text-align: center;
  border-radius: 5px;
  font-weight: bold;
}

.btn-primary {
  background-color: #4c6ef5;
  color: white;
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
}

.btn-danger {
  background-color: #dc3545;
  color: white;
}

button:hover {
  opacity: 0.9;
}
</style>
