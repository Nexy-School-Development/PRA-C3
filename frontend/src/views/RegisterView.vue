<template>
  <div>
    <h1>Register</h1>
    <form @submit.prevent="register">
      <input v-model="email" type="email" placeholder="Email" required />
      <input v-model="password" type="password" placeholder="Password" required />
      <input v-model="confirmPassword" type="password" placeholder="Confirm Password" required />
      <button type="submit">Register</button>
    </form>
    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    <p>
      Ask a yes/no question:
      <input v-model="question" :disabled="loading" />
    </p>
    <p>{{ answer }}</p>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import axios from 'axios'

const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const errorMessage = ref('')

function register() {
  if (password.value !== confirmPassword.value) {
    errorMessage.value = 'Passwords do not match'
    return
  }

  axios.post('http://localhost:5116/api/User/register', { email: email.value, password: password.value })
    .then(() => {
      errorMessage.value = ''
      email.value = ''
      password.value = ''
      confirmPassword.value = ''
    })
    .catch(error => {
      errorMessage.value = error.response.data.message
    })
}

</script>

<style scoped>
.error {
  color: red;
  margin-top: 10px;
}
</style>