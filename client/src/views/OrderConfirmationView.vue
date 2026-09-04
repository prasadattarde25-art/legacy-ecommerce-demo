<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import api from '../services/api'

const route = useRoute()
const order = ref(null)
const loading = ref(true)
const error = ref('')

const fmt = (n) => Number(n).toLocaleString('en-US', { style: 'currency', currency: 'USD' })

onMounted(async () => {
  try {
    const { data } = await api.get(`/checkout/${route.params.id}`)
    order.value = data
  } catch (e) {
    error.value = 'Order not found.'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div>
    <div v-if="loading" class="hint">Loading order...</div>
    <div v-else-if="error" class="error">{{ error }}</div>
    <div v-else-if="order" class="confirmation">
      <div class="success">✓ Order Confirmed</div>
      <h1>Order #{{ order.orderNumber || order.id }}</h1>
      <p class="meta">Placed on {{ order.orderDate }}</p>

      <div class="status" :class="order.status">
        Status: {{ order.status }}
      </div>

      <div class="summary">
        <h3>Items</h3>
        <div v-for="line in order.lines || []" :key="line.id" class="line">
          <span class="lname">{{ line.productName }}</span>
          <span class="lqty">× {{ line.quantity }}</span>
          <span class="lprice">{{ fmt(line.lineTotal) }}</span>
        </div>
        <div class="totals">
          <div class="row"><span>Subtotal</span><span>{{ fmt(order.subtotal) }}</span></div>
          <div v-if="order.discount" class="row"><span>Discount</span><span>−{{ fmt(order.discount) }}</span></div>
          <div class="row"><span>Shipping</span><span>{{ fmt(order.shippingTotal) }}</span></div>
          <div class="row"><span>Tax</span><span>{{ fmt(order.taxTotal) }}</span></div>
          <div class="row total"><span>Total</span><span>{{ fmt(order.grandTotal) }}</span></div>
        </div>
      </div>

      <router-link :to="{ name: 'products' }" class="btn btn-primary">Continue Shopping</router-link>
    </div>
  </div>
</template>

<style scoped>
.confirmation {
  max-width: 640px;
}
.success {
  display: inline-block;
  background: #27ae60;
  color: #fff;
  padding: 8px 16px;
  border-radius: 20px;
  font-weight: 700;
}
h1 {
  margin: 16px 0 4px;
}
.meta {
  color: #95a5a6;
}
.status {
  margin: 12px 0;
  padding: 8px 12px;
  border-radius: 4px;
  background: #f0f8ff;
  border: 1px solid #b3d9f7;
  text-transform: capitalize;
}
.summary {
  background: #fff;
  border: 1px solid #e1e1e1;
  border-radius: 8px;
  padding: 18px;
  margin: 20px 0;
}
.summary h3 {
  margin: 0 0 12px;
  border-bottom: 1px solid #eee;
  padding-bottom: 8px;
}
.line {
  display: flex;
  justify-content: space-between;
  padding: 6px 0;
}
.lname { flex: 1; }
.totals {
  border-top: 1px solid #eee;
  margin-top: 12px;
  padding-top: 12px;
}
.row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 6px;
}
.row.total {
  font-weight: 700;
  font-size: 1.1rem;
}
.error {
  color: #e74c3c;
}
.hint {
  color: #95a5a6;
}
</style>
