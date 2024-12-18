<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-blue-500 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Register</h1>
    </header>

    <main class="container mx-auto p-5">
      <section class="bg-white shadow-md p-6 rounded-lg max-w-md mx-auto">
        <form @submit.prevent="register">
          <input v-model="email" type="email" placeholder="Email" class="input-field" required />
          <input v-model="password" type="password" placeholder="Password" class="input-field" required />
          <input v-model="confirmPassword" type="password" placeholder="Confirm Password" class="input-field" required />
          <button type="submit" class="btn-primary">Register</button>
        </form>
        <p v-if="errorMessage" class="text-red-500 mt-3">{{ errorMessage }}</p>
      </section>
    </main>
  </div>
</template>

<script>
import apiClient from "@/axios";

export default {
  data() {
    return {
      email: "",
      password: "",
      confirmPassword: "",
      errorMessage: "",
    };
  },
  methods: {
    async register() {
      if (this.password !== this.confirmPassword) {
        this.errorMessage = "Passwords do not match.";
        return;
      }
      try {
        await apiClient.post("/User/register", {
          email: this.email,
          password: this.password,
        });
        this.email = "";
        this.password = "";
        this.confirmPassword = "";
        this.errorMessage = "Registration successful!";
      } catch (error) {
        this.errorMessage = error.response?.data || "Failed to register.";
      }
    },
  },
};
</script>

<style>
.input-field {
  width: 100%;
  border: 1px solid #ddd;
  padding: 10px;
  margin-bottom: 10px;
  border-radius: 5px;
}
.btn-primary {
  background-color: #007bff;
  color: white;
  padding: 10px 15px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}
</style>
