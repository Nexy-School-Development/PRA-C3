<!-- filepath: /c:/Coderen/PRA-C3/frontend/src/App.vue -->
<template>
  <div>
    <header>
      <nav>
        <router-link to="/">Home</router-link>
        <router-link to="/dashboard">Dashboard</router-link>
        <router-link to="/match">Match</router-link>
        <router-link to="/team">Team</router-link>
        <router-link to="/tournooi">Tournooi</router-link>
        <router-link to="/bet">Bet</router-link>
        <router-link to="/scoreboard">Scoreboard</router-link>
        <router-link to="/ManageBets">manage beds</router-link>
      </nav>
    </header>
    <router-view />
  </div>
</template>

<style scoped>
header {
  line-height: 1.5;
  max-height: 100vh;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

nav {
  width: 100%;
  font-size: 12px;
  text-align: center;
  margin-top: 2rem;
}

nav a {
  margin: 0 1rem;
}
</style>