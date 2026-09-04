import { defineStore } from 'pinia'
import api from '../services/api'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || null,
    customer: JSON.parse(localStorage.getItem('customer') || 'null')
  }),
  getters: {
    isAuthenticated: (state) => !!state.token,
    fullName: (state) =>
      state.customer ? `${state.customer.firstName} ${state.customer.lastName}`.trim() : ''
  },
  actions: {
    async login(credentials) {
      const { data } = await api.post('/account/login', credentials)
      this.setAuth(data)
      return data
    },
    async register(payload) {
      const { data } = await api.post('/account/register', payload)
      this.setAuth(data)
      return data
    },
    async logout() {
      try {
        await api.post('/account/logout')
      } catch (e) {
        // ignore network errors on logout
      }
      this.token = null
      this.customer = null
      localStorage.removeItem('token')
      localStorage.removeItem('customer')
    },
    setAuth(data) {
      if (data && data.token) {
        this.token = data.token
        this.customer = data.customer
        localStorage.setItem('token', data.token)
        localStorage.setItem('customer', JSON.stringify(data.customer))
      }
    }
  }
})
