<template>
  <div class="min-h-screen bg-gray-100">
    <header class="bg-indigo-600 text-white p-5 shadow-lg">
      <h1 class="text-3xl font-bold text-center">Welcome to the Tournament Platform</h1>
      <h3 v-if="errorMessage" class="alert">{{ errorMessage }}</h3>
    </header>

    <main class="container mx-auto p-5">
      <section class="bg-white shadow-md p-6 rounded-lg">
        <div v-if="user">
          <h2 class="text-xl font-bold mb-3">Hello, {{ user.email }}</h2>
          <p>Your current balance: <strong>{{ user.balance }}</strong></p>
          <button @click="logout" class="btn-danger mt-4">Logout</button>
        </div>
        <div v-else>
          <p>You are not logged in.</p>
          <router-link to="/login" class="btn-primary mr-3">Login</router-link>
          <router-link to="/register" class="btn-secondary">Register</router-link>
        </div>
      </section>
    </main>
  </div>

</template>

<script>
export default {
  data() {
    return {
      user: null,
      errorMessage: "",
    };
  },
  methods: {
    checkUser() {
      const storedUser = localStorage.getItem("user");
      if (storedUser) {
        this.user = JSON.parse(storedUser);
      }
    },
    logout() {
      localStorage.removeItem("user");
      this.user = null;
    },
  },
  created() {
    // Fetch error query from route
    if (this.$route.query.error === "admin") {
      this.errorMessage = "You need to be admin to view this!";
    }
    // Check user information
    this.checkUser();
  },
};
</script>
<style>
.btn-primary {
  background-color: #4c51bf;
  color: white;
  padding: 10px 15px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}
.btn-secondary {
  background-color: #6c757d;
  color: white;
  padding: 10px 15px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}
.btn-danger {
  background-color: #dc3545;
  color: white;
  padding: 10px 15px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

</style>