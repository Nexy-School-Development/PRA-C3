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
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue'
  import axios from 'axios'
  
  const email = ref('')
  const password = ref('')
  const confirmPassword = ref('')
  const errorMessage = ref('')
  
  const register = async () => {
    if (password.value !== confirmPassword.value) {
      errorMessage.value = 'Passwords do not match'
      return
    }
  
    try {
      const response = await axios.post('http://localhost:5173/api/user/register', {
        email: email.value,
        password: password.value
      })
      console.log('User registered:', response.data)
      errorMessage.value = ''
    } catch (error) {
      console.error('Error registering user:', error)
      if (error.response) {
        console.error('Response data:', error.response.data)
        console.error('Response status:', error.response.status)
        console.error('Response headers:', error.response.headers)
        errorMessage.value = error.response.data || 'Error registering user'
      } else if (error.request) {
        console.error('Request data:', error.request)
        errorMessage.value = 'No response received from server'
      } else {
        console.error('Error message:', error.message)
        errorMessage.value = 'Error registering user'
      }
    }
  }
  </script>
  
  <style scoped>
  .error {
    color: red;
    margin-top: 10px;
  }
  </style>