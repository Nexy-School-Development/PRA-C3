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
            <th>Password</th>
            <th>Token</th>
            <th>IsAdmin</th>
            <th>Balance</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.id">
            <td>{{ user.id }}</td>
            <td>{{ user.email }}</td>
            <td>{{ user.password }}</td>
            <td>{{ user.token }}</td>
            <td>{{ user.isAdmin === 1 ? 'yes' : 'no' }}</td>
            <td>{{ user.balance }}</td>
          </tr>
        </tbody>
      </table>
      <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    </section>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const users = ref([])
const errorMessage = ref('')

const fetchData = async () => {
  try {
    const usersResponse = await axios.get('http://localhost:5116/api/User')
    users.value = usersResponse.data
  } catch (error) {
    console.error('Error fetching data:', error)
    errorMessage.value = 'Error fetching data: ' + (error.response?.data?.message || error.message)
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
  background-color: black;
  color: white;
}

.error {
  color: red;
  margin-top: 10px;
}
</style>