<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import api from '../services/api'
import { useCartStore } from '../stores/cart'
import ProductCard from '../components/ProductCard.vue'

const route = useRoute()
const cart = useCartStore()
const detail = ref(null)
const qty = ref(1)
const loading = ref(true)

const product = computed(() => (detail.value ? detail.value.product : null))
const images = computed(() => (detail.value ? detail.value.images : []))
const related = computed(() => (detail.value ? detail.value.relatedProducts : []))

const formattedPrice = computed(() =>
  product.value
    ? product.value.price.toLocaleString('en-US', { style: 'currency', currency: 'USD' })
    : ''
)

const max = computed(() => (product.value && product.value.stockQuantity > 0 ? product.value.stockQuantity : 999))
const hasDiscount = computed(() => product.value && product.value.listPrice > product.value.price)

async function addToCart() {
  await cart.add(product.value.id, qty.value)
}

onMounted(async () => {
  try {
    const { data } = await api.get(`/products/${route.params.id}`)
    detail.value = data
    qty.value = 1
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div v-if="loading" class="hint">Loading product...</div>
  <div v-else-if="product" class="detail">
    <div class="gallery">
      <img
        v-if="images.length"
        :src="images[0].url || product.thumbnailUrl"
        :alt="images[0].altText || product.name"
      />
      <img v-else-if="product.thumbnailUrl" :src="product.thumbnailUrl" :alt="product.name" />
      <div v-else class="placeholder">🛒</div>
    </div>
    <div class="info">
      <h1>{{ product.name }}</h1>
      <p v-if="product.category && product.category.name" class="cat">{{ product.category.name }}</p>
      <p v-if="product.sku" class="sku">SKU: {{ product.sku }}</p>
      <div class="price">
        <span v-if="hasDiscount" class="old">
          {{ product.listPrice.toLocaleString('en-US', { style: 'currency', currency: 'USD' }) }}
        </span>
        <span>{{ formattedPrice }}</span>
      </div>
      <p class="desc">{{ product.shortDescription }}</p>
      <div v-if="product.description" class="desc long" v-html="product.description"></div>

      <div class="stock" :class="{ low: product.stockQuantity > 0 && product.stockQuantity < 10 }">
        {{ product.stockQuantity > 0 ? `${product.stockQuantity} in stock` : 'Out of stock' }}
      </div>

      <div class="buy-row" v-if="product.stockQuantity > 0">
        <label>
          Qty
          <input v-model.number="qty" type="number" :min="1" :max="max" class="qty" />
        </label>
        <button class="btn btn-primary btn-lg" @click="addToCart">Add to Cart</button>
      </div>
    </div>
  </div>
  <div v-else class="empty">Product not found.</div>

  <section v-if="related.length" class="related">
    <h2>Related Products</h2>
    <div class="grid">
      <ProductCard v-for="p in related" :key="p.id" :product="p" />
    </div>
  </section>
</template>

<style scoped>
.detail {
  display: flex;
  gap: 28px;
  background: #fff;
  border: 1px solid #e1e1e1;
  border-radius: 8px;
  padding: 24px;
}
.gallery {
  width: 340px;
  height: 340px;
  background: #f5f5f5;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 8px;
  overflow: hidden;
  flex-shrink: 0;
}
.gallery img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.placeholder {
  font-size: 4rem;
}
.info {
  flex: 1;
}
.info h1 {
  margin: 0 0 4px;
}
.cat,
.sku {
  color: #7f8c8d;
  margin: 0 0 4px;
  font-size: 0.9rem;
}
.price {
  font-size: 1.6rem;
  font-weight: 700;
  color: #c0392b;
  margin: 16px 0;
  display: flex;
  gap: 12px;
  align-items: center;
}
.price .old {
  color: #95a5a6;
  text-decoration: line-through;
  font-size: 1.1rem;
  font-weight: 400;
}
.desc {
  color: #34495e;
  line-height: 1.6;
}
.desc.long {
  font-size: 0.95rem;
}
.stock {
  margin-top: 12px;
  font-weight: 600;
  color: #27ae60;
}
.stock.low {
  color: #e67e22;
}
.buy-row {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-top: 12px;
}
.qty {
  width: 64px;
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 4px;
}
.btn-lg {
  padding: 12px 24px;
  font-size: 1rem;
}
.related {
  margin-top: 32px;
}
.related h2 {
  font-size: 1.2rem;
  border-bottom: 2px solid #3498db;
  padding-bottom: 8px;
}
.grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 18px;
}
</style>
