<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'
import ProductCard from '../components/ProductCard.vue'

const featured = ref([])
const loading = ref(true)

onMounted(async () => {
  try {
    const { data } = await api.get('/home/featured')
    featured.value = data
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div>
    <section class="hero">
      <h1>Welcome to Legacy Store</h1>
      <p>A classic eCommerce experience rebuilt with Vue 3 and .NET 10.</p>
    </section>

    <h2 class="section-title">Featured Products</h2>
    <p v-if="loading">Loading products...</p>
    <div v-if="!loading && featured.length === 0" class="empty">No featured products yet.</div>
    <div class="grid">
      <ProductCard v-for="p in featured" :key="p.id" :product="p" />
    </div>
  </div>
</template>

<style scoped>
.hero {
  background: linear-gradient(135deg, #3498db, #2c3e50);
  color: #fff;
  border-radius: 8px;
  padding: 40px 28px;
  margin-bottom: 28px;
}
.hero h1 {
  margin: 0 0 8px;
}
.hero p {
  margin: 0;
  color: #ecf0f1;
}
.section-title {
  font-size: 1.3rem;
  border-bottom: 2px solid #3498db;
  padding-bottom: 8px;
  margin: 24px 0 18px;
}
.grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 18px;
}
.empty {
  color: #95a5a6;
  padding: 20px 0;
}
</style>
