<script setup>
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const route = useRoute()
const router = useRouter()

const email = ref('')
const password = ref('')
const error = ref('')
const submitting = ref(false)

async function submit() {
  submitting.value = true
  error.value = ''
  try {
    await auth.login({ email: email.value, password: password.value, rememberMe: false })
    router.push(route.query.redirect || { name: 'home' })
  } catch (e) {
    error.value = e.response?.data?.message || e.response?.data || 'Invalid email or password.'
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div class="auth">
    <h1 class="page-title">Login</h1>
    <form class="auth-form" @submit.prevent="submit">
      <label>Email<input v-model="email" type="email" required /></label>
      <label>Password<input v-model="password" type="password" required /></label>
      <p v-if="error" class="error">{{ error }}</p>
      <button type="submit" class="btn btn-primary" :disabled="submitting">
        {{ submitting ? 'Signing in...' : 'Login' }}
      </button>
      <p class="muted">No account? <router-link :to="{ name: 'register' }">Register</router-link></p>
    </form>
  </div>
</template>

<style scoped>
.auth {
  max-width: 400px;
}
.page-title {
  font-size: 1.4rem;
  border-bottom: 2px solid #3498db;
  padding-bottom: 8px;
  margin: 0 0 18px;
}
.auth-form {
  background: #fff;
  border: 1px solid #e1e1e1;
  border-radius: 8px;
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 14px;
}
.auth-form label {
  display: flex;
  flex-direction: column;
  gap: 4px;
  font-size: 0.9rem;
}
.auth-form input {
  padding: 9px;
  border: 1px solid #ccc;
  border-radius: 4px;
}
.error {
  color: #e74c3c;
  margin: 0;
}
.muted {
  color: #7f8c8d;
  font-size: 0.9rem;
  margin: 0;
}
.muted a {
  color: #3498db;
}
</style>
