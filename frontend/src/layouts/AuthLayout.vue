<!-- filepath: /c:/Coderen/PRA-C3/frontend/src/layouts/AuthLayout.vue -->
<template>
    <div class="auth-layout">
      <nav>
        <router-link to="/auth/login">Login</router-link>
        <router-link to="/auth/register">Register</router-link>
      </nav>
      <router-view></router-view>
    </div>
  </template>
  
  <script>
  export default {
    name: 'AuthLayout',
  };
  </script>
  
  <style scoped>
  .auth-layout {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 100vh;
    background-color: #f5f5f5;
  }
  
  nav {
    margin-bottom: 2rem;
  }
  
  nav a {
    margin: 0 1rem;
  }
  </style>