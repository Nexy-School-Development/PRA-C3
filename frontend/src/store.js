// filepath: /c:/laragon/www/pra-c3/frontend/src/store.js
import { createStore } from 'vuex';

export default createStore({
  state: {
    user: null,
    token: null,
  },
  mutations: {
    setUser(state, user) {
      state.user = user;
    },
    setToken(state, token) {
      state.token = token;
    },
  },
  actions: {
    login({ commit }, { user, token }) {
      commit('setUser', user);
      commit('setToken', token);
      localStorage.setItem('user', JSON.stringify(user));
      localStorage.setItem('token', token);
    },
    logout({ commit }) {
      commit('setUser', null);
      commit('setToken', null);
      localStorage.removeItem('user');
      localStorage.removeItem('token');
    },
    checkUser({ commit }) {
      const user = JSON.parse(localStorage.getItem('user'));
      const token = localStorage.getItem('token');
      if (user && token) {
        commit('setUser', user);
        commit('setToken', token);
      }
    },
  },
});