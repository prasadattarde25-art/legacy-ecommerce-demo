<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useCartStore } from '../stores/cart'

const router = useRouter()
const auth = useAuthStore()
const cart = useCartStore()

const search = ref('')

function onSearch() {
  router.push({ name: 'products', query: { q: search.value } })
}

async function onLogout() {
  await auth.logout()
  router.push({ name: 'home' })
}
</script>

<template>
  <header class="app-header">
    <div class="container header-inner">
      <router-link :to="{ name: 'home' }" class="brand">
        <span class="brand-mark">🛒</span>
        <span>
          <strong>Legacy</strong> Store
        </span>
      </router-link>

      <form class="search-form" @submit.prevent="onSearch">
        <input v-model="search" type="search" placeholder="Search products..." aria-label="Search" />
        <button type="submit">Search</button>
      </form>

      <nav class="account-nav">
        <router-link :to="{ name: 'assistant' }">Assistant</router-link>
        <template v-if="auth.isAuthenticated">
          <span class="welcome">Hi, {{ auth.fullName }}</span>
          <router-link :to="{ name: 'orders' }">Orders</router-link>
          <a href="#" @click.prevent="onLogout">Logout</a>
        </template>
        <template v-else>
          <router-link :to="{ name: 'login' }">Login</router-link>
          <router-link :to="{ name: 'register' }">Register</router-link>
        </template>

        <router-link :to="{ name: 'cart' }" class="cart-link">
          Cart
          <span v-if="cart.itemCount > 0" class="cart-badge">{{ cart.itemCount }}</span>
        </router-link>
      </nav>
    </div>
  </header>
</template>

<style scoped>
.app-header {
  background: #2c3e50;
  color: #fff;
  padding: 14px 0;
}
.header-inner {
  display: flex;
  align-items: center;
  gap: 24px;
}
.brand {
  color: #fff;
  text-decoration: none;
  font-size: 1.2rem;
  display: flex;
  align-items: center;
  gap: 6px;
}
.brand-mark {
  font-size: 1.4rem;
}
.search-form {
  flex: 1;
  display: flex;
  max-width: 460px;
}
.search-form input {
  flex: 1;
  padding: 8px 12px;
  border-radius: 4px 0 0 4px;
  border: none;
}
.search-form button {
  padding: 8px 16px;
  border: none;
  cursor: pointer;
  background: #3498db;
  color: #fff;
  border-radius: 0 4px 4px 0;
}
.account-nav {
  display: flex;
  align-items: center;
  gap: 16px;
  color: #ecf0f1;
}
.account-nav a {
  color: #ecf0f1;
  text-decoration: none;
}
.account-nav a:hover {
  text-decoration: underline;
}
.welcome {
  font-size: 0.9rem;
}
.cart-link {
  position: relative;
  font-weight: 600;
}
.cart-badge {
  position: absolute;
  top: -8px;
  right: -12px;
  background: #e74c3c;
  color: #fff;
  border-radius: 50%;
  font-size: 0.7rem;
  width: 18px;
  height: 18px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}
</style>
