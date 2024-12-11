<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-blue-600 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Welcome to Your Profile</h1>
    </header>

    <main class="container mx-auto p-5">
      <section class="mb-10">
        <h2 class="text-xl font-bold mb-4">Your Details</h2>
        <div class="bg-white shadow-md p-6 rounded-lg">
          <p><strong>Email:</strong> {{ user.email }}</p>
          <p><strong>Balance:</strong> ${{ user.balance }}</p>
        </div>
      </section>

      <section>
        <h2 class="text-xl font-bold mb-4">Actions</h2>
        <button @click="logout" class="bg-red-600 text-white py-2 px-4 rounded-md hover:bg-red-700">
          Logout
        </button>
      </section>
    </main>
  </div>
</template>

<script>
import apiClient from "../axios";

export default {
  data() {
    return {
      user: {},
    };
  },
  methods: {
    async fetchUser() {
      try {
        const response = await apiClient.get("/user/profile");
        this.user = response.data;
      } catch (error) {
        console.error("Error fetching user details:", error);
      }
    },
    logout() {
      localStorage.removeItem("user");
      this.$router.push("/login");
    },
  },
  created() {
    this.fetchUser();
  },
};
</script>

<style>
body {
  font-family: "Inter", sans-serif;
}
</style>
