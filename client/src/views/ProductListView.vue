<script setup>
import { ref, watch, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '../services/api'
import ProductCard from '../components/ProductCard.vue'

const route = useRoute()
const router = useRouter()

const products = ref([])
const categories = ref([])
const paged = ref({ page: 1, pageSize: 12, totalItems: 0, totalPages: 0, hasNext: false, hasPrevious: false })
const loading = ref(true)
const query = ref(route.query.q || '')
const categoryId = ref(route.query.categoryId ? Number(route.query.categoryId) : null)

async function load() {
  loading.value = true
  try {
    const params = {
      page: paged.value.page
    }
    if (categoryId.value) params.categoryId = categoryId.value
    if (query.value) params.q = query.value

    const { data } = await api.get('/products', { params })
    products.value = data.products || []
    categories.value = data.categories || []
    paged.value = data.pagedResult || {}
  } finally {
    loading.value = false
  }
}

function changeCategory(id) {
  categoryId.value = id
  paged.value.page = 1
  router.push({ name: 'products', query: { ...(id ? { categoryId: id } : {}), ...(query.value ? { q: query.value } : {}) } })
  load()
}

function goToPage(page) {
  paged.value.page = page
  load()
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

function onSearch() {
  paged.value.page = 1
  router.push({ name: 'products', query: { ...(categoryId.value ? { categoryId: categoryId.value } : {}), ...(query.value ? { q: query.value } : {}) } })
  load()
}

onMounted(load)
</script>

<template>
  <div>
    <div class="toolbar">
      <h1 class="page-title">Products</h1>
      <form class="inline-search" @submit.prevent="onSearch">
        <input v-model="query" type="search" placeholder="Search..." />
        <button type="submit" class="btn btn-primary">Go</button>
      </form>
    </div>

    <div v-if="categories.length" class="category-filter">
      <button
        :class="{ active: !categoryId }"
        class="chip"
        @click="changeCategory(null)"
      >All</button>
      <button
        v-for="c in categories"
        :key="c.id"
        :class="{ active: categoryId === c.id }"
        class="chip"
        @click="changeCategory(c.id)"
      >{{ c.name }}</button>
    </div>

    <p v-if="loading" class="hint">Loading products...</p>
    <div v-if="!loading && products.length === 0" class="empty">No products found.</div>

    <div class="grid">
      <ProductCard v-for="p in products" :key="p.id" :product="p" />
    </div>

    <div v-if="paged.totalPages > 1" class="pagination">
      <button class="btn" :disabled="!paged.hasPrevious" @click="goToPage(paged.page - 1)">‹ Prev</button>
      <span>Page {{ paged.page }} of {{ paged.totalPages }}</span>
      <button class="btn" :disabled="!paged.hasNext" @click="goToPage(paged.page + 1)">Next ›</button>
    </div>
  </div>
</template>

<style scoped>
.toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
}
.page-title {
  font-size: 1.4rem;
  margin: 0;
}
.inline-search {
  display: flex;
  gap: 6px;
}
.inline-search input {
  padding: 6px 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
}
.category-filter {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-bottom: 18px;
}
.chip {
  padding: 6px 14px;
  border: 1px solid #ccc;
  border-radius: 20px;
  background: #fff;
  cursor: pointer;
}
.chip.active {
  background: #3498db;
  color: #fff;
  border-color: #3498db;
}
.grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 18px;
}
.pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 16px;
  margin: 24px 0 0;
}
.hint,
.empty {
  color: #95a5a6;
}
</style>
