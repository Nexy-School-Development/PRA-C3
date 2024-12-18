import { createRouter, createWebHistory } from 'vue-router';
import HomeView from '../views/HomeView.vue';
import LoginView from '../views/LoginView.vue';
import RegisterView from '../views/RegisterView.vue';
import DashboardView from '../views/dashboardView.vue';
import TeamView from '../views/TeamView.vue';
import TournooienView from '../views/TournooienView.vue';
import MatchView from '../views/MatchView.vue';
import BetView from '../views/BetView.vue';
import ScoreboardView from '../views/ScoreboardView.vue';
  
const routes = [
  { path: '/', component: HomeView, meta: { requiresAuth: true } },
  { path: '/login', component: LoginView },
  { path: '/register', component: RegisterView, meta: { requiresAuth: true } },
  { path: '/dashboard', component: DashboardView, meta: { requiresAuth: true } },
  { path: '/tournooi', component: TournooienView, meta: { requiresAuth: true } },
  { path: '/team', component: TeamView, meta: { requiresAuth: true } },
  { path: '/match', component: MatchView, meta: { requiresAuth: true } },
  { path: '/bet', component: BetView, meta: { requiresAuth: true } },
  { path: '/scoreboard', component: ScoreboardView, meta: { requiresAuth: true } }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from, next) => {
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth);
  const user = JSON.parse(localStorage.getItem('user'));

  if (requiresAuth && !user) {
    next('/login');
  } else {
    next();
  }
});

export default router;