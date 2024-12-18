<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-indigo-600 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">View Bets</h1>
    </header>

    <main class="container mx-auto p-5">
      <!-- Finished Matches Section -->
      <section v-if="finishedMatches.length">
        <h2 class="text-2xl font-bold mb-3">Finished Matches</h2>

        <div v-for="match in finishedMatches" :key="match.MatchId" class="match-card bg-white p-4 mb-4 shadow-md rounded-lg">
          <p>{{ match.HomeTeamName }} vs {{ match.AwayTeamName }}</p>
          <p>Status: Finished</p>

          <button @click="openWinnerPopup(match)" class="btn-primary mt-4 px-4 py-2">
            What team won?
          </button>

          <!-- Winner selection popup -->
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
      teams: [],  // To store teams with full details
      errorMessage: "",
    };
  },
  methods: {
    async fetchMatches() {
      try {
        // Fetching matches
        const response = await axios.get("http://localhost/pra-c3/frontend/database/getMatches.php");
        const matches = response.data;

        // Fetching teams for match data
        const teamsResponse = await axios.get("http://localhost/pra-c3/frontend/database/getTeams.php");
        this.teams = teamsResponse.data;

        // Filter finished matches and map team names to the match data
        this.finishedMatches = matches
          .filter((match) => new Date(match.Starttime) < new Date()) // Ensure the match is finished
          .map((match) => {
            const homeTeam = this.teams.find((team) => team.Id === match.HomeTeamId);
            const awayTeam = this.teams.find((team) => team.Id === match.AwayTeamId);
            return {
              ...match,
              HomeTeamName: homeTeam ? homeTeam.Name : "Unknown",
              AwayTeamName: awayTeam ? awayTeam.Name : "Unknown",
              showPopup: false, // Initially, no popup is shown
            };
          });
      } catch (error) {
        console.error("Error fetching matches or teams:", error);
        this.errorMessage = "Error fetching matches or teams.";
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

      async selectWinner(match, winner) {
      try {
        const response = await axios.post("http://localhost:5116/api/bet/resolve-bets", {
          matchId: match.MatchId, 
          winner: winner,
        });

        alert(response.data.message);

        this.fetchMatches();
      } catch (error) {
        console.error("Error selecting winner:", error);
        this.errorMessage = "Error selecting the winner.";
      }

      // Close the popup after selection
      match.showPopup = false;
    },

    // Resolve all bets for all matches
    async resolveBets() {
      try {
        const matchIds = this.finishedMatches.map(match => match.MatchId); 

        const response = await axios.post("http://localhost:5116/api/bet/resolve-all-bets", {
          matchIds: matchIds,  
        });

        alert(response.data); 
      } catch (error) {
        console.error("Error resolving bets:", error);
        alert("There was an error resolving the bets.");
      }
    },
  },
  created() {
    this.fetchMatches();
  },
};
</script>

<style scoped>
/* Same CSS as before */
</style>


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
  background-color: black;
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
