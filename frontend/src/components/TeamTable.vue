<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const teams = ref([])

onMounted(async () => {
  const response = await axios.get('/team.json')
  teams.value = response.data.teams
})
</script>

<template>
  <div>
    <h1>Voetbal Wedstrijden</h1>
    <table>
      <thead>
        <tr>
          <th>Team</th>
          <th>Coach</th>
          <th>Punten</th>
          <th>Overwinningen</th>
          <th>Verliezen</th>
          <th>Gespeelde Wedstrijden</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="team in teams" :key="team.name">
          <td>{{ team.name }}</td>
          <td>{{ team.coach }}</td>
          <td>{{ team.points }}</td>
          <td>{{ team.wins }}</td>
          <td>{{ team.loses }}</td>
          <td>{{ team.matches_played }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
table {
  width: 100%;
  border-collapse: collapse;
}
th, td {
  border: 1px solid #ddd;
  padding: 8px;
}
th {
  background-color: #f2f2f2;
}
</style>