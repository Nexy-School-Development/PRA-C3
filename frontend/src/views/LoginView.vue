<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100">
    <div class="w-full max-w-md bg-white shadow-md rounded-lg p-6">
      <h1 class="text-2xl font-bold text-center mb-6">Login</h1>
      <form @submit.prevent="login" class="flex flex-col gap-4">
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
        <button
          type="submit"
          class="bg-blue-600 text-white py-3 px-5 rounded-lg hover:bg-blue-700"
        >
          Login
        </button>
        <p v-if="errorMessage" class="text-sm text-red-600">{{ errorMessage }}</p>
      </form>
    </div>
  </div>
</template>

<script>
import apiClient from "../axios";
import { useRouter } from "vue-router";

export default {
  data() {
    return {
      email: "",
      password: "",
      errorMessage: "",
    };
  },
  setup() {
    const router = useRouter();
    return { router };
  },
  methods: {
    async login() {
      try {
        const response = await apiClient.post("/user/login", {
          email: this.email,
          password: this.password,
        });
        localStorage.setItem("user", JSON.stringify(response.data));
        this.router.push("/");
      } catch (error) {
        this.errorMessage = "Invalid email or password.";
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
