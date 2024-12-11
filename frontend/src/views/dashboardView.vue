<template>
  <div>
    <h1>Dashboard</h1>

    <section>
      <h2>Users</h2>
      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>Email</th>
            <th>IsAdmin</th>
            <th>Balance</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.id">
            <td>{{ user.id }}</td>
            <td>{{ user.email }}</td>
            <td>{{ user.isAdmin }}</td>
            <td>{{ user.balance }}</td>
          </tr>
        </tbody>
      </table>
    </section>

    <section>
      <h2>Teams</h2>
      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>Name</th>
            <th>Points</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="team in teams" :key="team.id">
            <td>{{ team.id }}</td>
            <td>{{ team.name }}</td>
            <td>{{ team.points }}</td>
          </tr>
        </tbody>
      </table>
    </section>

    <section>
      <h2>Matches</h2>
      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>HomeTeamId</th>
            <th>AwayTeamId</th>
            <th>Team1Score</th>
            <th>Team2Score</th>
            <th>Starttime</th>
            <th>IsFinished</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="match in matches" :key="match.id">
            <td>{{ match.id }}</td>
            <td>{{ match.homeTeamId }}</td>
            <td>{{ match.awayTeamId }}</td>
            <td>{{ match.team1Score }}</td>
            <td>{{ match.team2Score }}</td>
            <td>{{ match.starttime }}</td>
            <td>{{ match.isFinished }}</td>
          </tr>
        </tbody>
      </table>
    </section>

    <section>
      <h2>Bets</h2>
      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>Amount</th>
            <th>Prediction</th>
            <th>IsResolved</th>
            <th>Payout</th>
            <th>MatchId</th>
            <th>UserId</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="bet in bets" :key="bet.id">
            <td>{{ bet.id }}</td>
            <td>{{ bet.amount }}</td>
            <td>{{ bet.prediction }}</td>
            <td>{{ bet.isResolved }}</td>
            <td>{{ bet.payout }}</td>
            <td>{{ bet.matchId }}</td>
            <td>{{ bet.userId }}</td>
          </tr>
        </tbody>
      </table>
    </section>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const users = ref([])
const teams = ref([])
const matches = ref([])
const bets = ref([])

const fetchData = async () => {
  try {
    const [usersResponse, teamsResponse, matchesResponse, betsResponse] = await Promise.all([
      axios.get('http://localhost:5116//api/User'),
      axios.get('http://localhost:5116//api/Team'),
      axios.get('http://localhost:5116//api/Match'),
      axios.get('http://localhost:5116//api/Bet')
    ])
    users.value = usersResponse.data
    teams.value = teamsResponse.data
    matches.value = matchesResponse.data
    bets.value = betsResponse.data
  } catch (error) {
    console.error('Error fetching data:', error)
  }
}

onMounted(fetchData)
</script>

<style scoped>
table {
  width: 100%;
  border-collapse: collapse;
  margin-bottom: 20px;
}

th, td {
  border: 1px solid #ddd;
  padding: 8px;
}

th {
  background-color: #f2f2f2;
}
</style>