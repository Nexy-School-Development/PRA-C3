<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-blue-500 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Login</h1>
    </header>

    <main class="container mx-auto p-5">
      <section class="bg-white shadow-md p-6 rounded-lg max-w-md mx-auto">
        <form @submit.prevent="login">
          <input v-model="email" type="email" placeholder="Email" class="input-field" required />
          <input v-model="password" type="password" placeholder="Password" class="input-field" required />
          <button type="submit" class="btn-primary">Login</button>
        </form>
        <p v-if="errorMessage" class="text-red-500 mt-3">{{ errorMessage }}</p>
      </section>
    </main>
  </div>
</template>

<script>
import apiClient from "@/axios";
import { useRouter } from "vue-router";

export default {
  data() {
    return {
      email: "",
      password: "",
      errorMessage: "",
    };
  },
  methods: {
    async login() {
      try {
        const response = await apiClient.post("/User/login", null, {
          params: {
            email: this.email,
            password: this.password,
          },
        });
        localStorage.setItem("user", JSON.stringify(response.data));
        this.$router.push("/");
      } catch (error) {
        this.errorMessage = "Invalid email or password.";
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
