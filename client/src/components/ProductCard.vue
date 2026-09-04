<script setup>
import { computed } from 'vue'
import { useCartStore } from '../stores/cart'

const props = defineProps({
  product: { type: Object, required: true }
})

const cart = useCartStore()

const formattedPrice = computed(() =>
  props.product.price.toLocaleString('en-US', { style: 'currency', currency: 'USD' })
)

const hasDiscount = computed(() => props.product.listPrice && props.product.listPrice > props.product.price)

async function addToCart() {
  await cart.add(props.product.id, 1)
}
</script>

<template>
  <div class="product-card">
    <router-link :to="{ name: 'product-detail', params: { id: product.id } }" class="thumb">
      <img v-if="product.thumbnailUrl" :src="product.thumbnailUrl" :alt="product.name" loading="lazy" />
      <div v-else class="thumb-placeholder">🛒</div>
      <span v-if="hasDiscount" class="badge">Sale</span>
    </router-link>
    <div class="body">
      <router-link :to="{ name: 'product-detail', params: { id: product.id } }" class="name">
        {{ product.name }}
      </router-link>
      <p v-if="product.shortDescription" class="desc">{{ product.shortDescription }}</p>
      <div class="price-row">
        <span class="price">{{ formattedPrice }}</span>
        <span v-if="hasDiscount" class="list-price">
          {{ product.listPrice.toLocaleString('en-US', { style: 'currency', currency: 'USD' }) }}
        </span>
      </div>
      <button class="btn btn-primary" @click="addToCart">Add to Cart</button>
    </div>
  </div>
</template>

<style scoped>
.product-card {
  background: #fff;
  border: 1px solid #e1e1e1;
  border-radius: 6px;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}
.thumb {
  display: block;
  height: 160px;
  background: #f5f5f5;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  overflow: hidden;
}
.thumb img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.thumb-placeholder {
  font-size: 2.5rem;
}
.badge {
  position: absolute;
  top: 8px;
  left: 8px;
  background: #e74c3c;
  color: #fff;
  font-size: 0.7rem;
  padding: 2px 8px;
  border-radius: 3px;
}
.body {
  padding: 12px;
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.name {
  font-weight: 600;
  color: #2c3e50;
  text-decoration: none;
}
.name:hover {
  color: #3498db;
}
.desc {
  font-size: 0.85rem;
  color: #7f8c8d;
  margin: 0;
  flex: 1;
}
.price-row {
  display: flex;
  align-items: center;
  gap: 8px;
}
.price {
  color: #c0392b;
  font-weight: 700;
}
.list-price {
  color: #95a5a6;
  text-decoration-line: line-through;
  font-size: 0.85rem;
}
</style>
