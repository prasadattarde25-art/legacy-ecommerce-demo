<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'

const router = useRouter()
const roots = ref([])

async function loadRoots() {
  const { data } = await api.get('/categories')
  roots.value = data
}

function goToCategory(id) {
  router.push({ name: 'products', query: { categoryId: id } })
}

onMounted(loadRoots)
</script>

<template>
  <div class="sidebar-card">
    <h3 class="sidebar-title">Categories</h3>
    <ul class="category-list">
      <li>
        <router-link :to="{ name: 'products' }">All Products</router-link>
      </li>
      <li v-for="cat in roots" :key="cat.id">
        <a href="#" @click.prevent="goToCategory(cat.id)">{{ cat.name }}</a>
      </li>
    </ul>
  </div>
</template>

<style scoped>
.sidebar-card {
  background: #fff;
  border: 1px solid #e1e1e1;
  border-radius: 6px;
  padding: 16px;
}
.sidebar-title {
  margin: 0 0 12px;
  font-size: 1rem;
  border-bottom: 1px solid #eee;
  padding-bottom: 8px;
}
.category-list {
  list-style: none;
  margin: 0;
  padding: 0;
}
.category-list li {
  margin-bottom: 6px;
}
.category-list a {
  color: #2c3e50;
  text-decoration: none;
}
.category-list a:hover {
  color: #3498db;
  text-decoration: underline;
}
</style>
