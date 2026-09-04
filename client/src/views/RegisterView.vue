<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const router = useRouter()

const email = ref('')
const firstName = ref('')
const lastName = ref('')
const password = ref('')
const confirmPassword = ref('')
const phone = ref('')
const error = ref('')
const submitting = ref(false)

async function submit() {
  submitting.value = true
  error.value = ''
  try {
    await auth.register({
      email: email.value,
      firstName: firstName.value,
      lastName: lastName.value,
      password: password.value,
      confirmPassword: confirmPassword.value,
      phone: phone.value
    })
    router.push({ name: 'home' })
  } catch (e) {
    error.value = e.response?.data?.message || e.response?.data || 'Registration failed.'
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div class="auth">
    <h1 class="page-title">Create Account</h1>
    <form class="auth-form" @submit.prevent="submit">
      <label>Email<input v-model="email" type="email" required /></label>
      <label>First Name<input v-model="firstName" required /></label>
      <label>Last Name<input v-model="lastName" required /></label>
      <label>Password<input v-model="password" type="password" minlength="6" required /></label>
      <label>Confirm Password<input v-model="confirmPassword" type="password" required /></label>
      <label>Phone<input v-model="phone" /></label>
      <p v-if="error" class="error">{{ error }}</p>
      <button type="submit" class="btn btn-primary" :disabled="submitting">
        {{ submitting ? 'Creating account...' : 'Register' }}
      </button>
      <p class="muted">Already have an account? <router-link :to="{ name: 'login' }">Login</router-link></p>
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
