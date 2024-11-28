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

const question = ref('')
const answer = ref('Questions usually contain a question mark. ;-)')
const loading = ref(false)

const register = async () => {
  if (password.value !== confirmPassword.value) {
    errorMessage.value = 'Passwords do not match'
    return
  }

  const timeout = new Promise((_, reject) => 
    setTimeout(() => reject(new Error('Request timed out')), 60000)
  )

  try {
    const response = await Promise.race([
      axios.post('http://localhost:5116/api/User/register', {
        email: email.value,
        password: password.value
      }),
      timeout
    ])
    console.log('User registered:', response.data)
    errorMessage.value = ''
  } catch (error) {
    console.error('Error registering user:', error)
    if (error.message === 'Request timed out') {
      errorMessage.value = 'Request timed out'
    } else if (error.response) {
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

watch(question, async (newQuestion) => {
  if (newQuestion.includes('?')) {
    loading.value = true
    answer.value = 'Thinking...'
    try {
      const res = await fetch('http://localhost:5116/api/User/register')
      answer.value = (await res.json()).answer
    } catch (error) {
      answer.value = 'Error! Could not reach the API. ' + error
    } finally {
      loading.value = false
    }
  }
})
</script>

<style scoped>
.error {
  color: red;
  margin-top: 10px;
}
</style>