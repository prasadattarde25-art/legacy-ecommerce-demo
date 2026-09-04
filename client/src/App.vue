<script setup>
import AppHeader from './components/AppHeader.vue'
import AppSidebar from './components/AppSidebar.vue'
import AppFooter from './components/AppFooter.vue'
import { useAuthStore } from './stores/auth'
import { useCartStore } from './stores/cart'
import { onMounted } from 'vue'

const auth = useAuthStore()
const cart = useCartStore()

onMounted(async () => {
  try {
    await cart.fetchCart()
  } catch (e) {
    // cart may not be reachable until the API is up; ignore for now
  }
})
</script>

<template>
  <div class="app-shell">
    <AppHeader />
    <div class="container layout">
      <aside class="sidebar">
        <AppSidebar />
      </aside>
      <main class="content">
        <router-view />
      </main>
    </div>
    <AppFooter />
  </div>
</template>

<style scoped>
.layout {
  display: flex;
  gap: 24px;
  padding: 24px 0 48px;
}
.sidebar {
  width: 240px;
  flex-shrink: 0;
}
.content {
  flex: 1;
  min-width: 0;
}
.app-shell {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}
.layout {
  flex: 1;
}
</style>
