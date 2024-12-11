<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100">
    <div class="w-full max-w-md bg-white shadow-md rounded-lg p-6">
      <h1 class="text-2xl font-bold text-center mb-6">Register</h1>
      <form @submit.prevent="register" class="flex flex-col gap-4">
        <input
          v-model="email"
          type="email"
          placeholder="Email"
          class="border border-gray-300 p-3 rounded-lg"
          required
        />
        <input
          v-model="password"
          type="password"
          placeholder="Password"
          class="border border-gray-300 p-3 rounded-lg"
          required
        />
        <input
          v-model="confirmPassword"
          type="password"
          placeholder="Confirm Password"
          class="border border-gray-300 p-3 rounded-lg"
          required
        />
        <button
          type="submit"
          class="bg-green-600 text-white py-3 px-5 rounded-lg hover:bg-green-700"
        >
          Register
        </button>
        <p v-if="errorMessage" class="text-sm text-red-600">{{ errorMessage }}</p>
      </form>
    </div>
  </div>
</template>

<script>
import apiClient from "../axios";

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
        await apiClient.post("/user/register", {
          email: this.email,
          password: this.password,
        });
        this.$router.push("/login");
      } catch (error) {
        this.errorMessage = error.response?.data || "Registration failed.";
      }
    },
  },
};
</script>

<style>
body {
  font-family: "Inter", sans-serif;
}
</style>
