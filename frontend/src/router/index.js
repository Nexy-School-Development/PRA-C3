import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import DashboardView from '../views/dashboardView.vue'
import TeamView from '../views/TeamView.vue'
import TournooienView from '../views/TournooienView.vue'
import MatchView from '../views/MatchView.vue'




const routes = [
  { path: '/', component: HomeView },
  { path: '/login', component: LoginView },
  { path: '/register', component: RegisterView },
  { path: '/dashboard', component: DashboardView },
  { path: '/tournooi', component: TournooienView },
  { path: '/team', component: TeamView },
  { path: '/match', component: MatchView },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router