<template>
  <div>
    <h1>Login</h1>
    <form @submit.prevent="login">
      <input v-model="email" type="email" placeholder="Email" required />
      <input v-model="password" type="password" placeholder="Password" required />
      <button type="submit">Login</button>
    </form>
    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import axios from 'axios'
import { useRouter } from 'vue-router'

const email = ref('')
const password = ref('')
const errorMessage = ref('')
const router = useRouter()

const login = async () => {
  try {
    const response = await axios.post(`http://localhost:5000/api/user/login?email=${encodeURIComponent(email.value)}&password=${encodeURIComponent(password.value)}`)
    localStorage.setItem('user', JSON.stringify(response.data))
    console.log('User logged in:', response.data)
    router.push('/')
  } catch (error) {
    console.error('Error logging in user:', error)
    errorMessage.value = 'Invalid email or password'
  }
}
</script>

<style scoped>
.error {
  color: red;
  margin-top: 10px;
}
</style>