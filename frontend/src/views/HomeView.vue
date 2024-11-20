<script setup>
import { ref } from 'vue'
import TeamsTable from '../components/TeamTable.vue'

const showTeamsTable = ref(false)

const toggleTeamsTable = () => {
  showTeamsTable.value = !showTeamsTable.value
}
</script>

<template>
  <main>
    <TheWelcome />
    <button @click="toggleTeamsTable">
      {{ showTeamsTable ? 'Hide TeamTable' : 'Show TeamTable' }}
    </button>
    <TeamsTable v-if="showTeamsTable" />
  </main>
</template>

<style scoped>
button {
  margin: 20px;
  padding: 10px 20px;
  font-size: 16px;
}
</style>