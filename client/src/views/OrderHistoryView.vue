<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const history = ref(null)
const loading = ref(true)
const error = ref('')

const fmt = (n) => Number(n).toLocaleString('en-US', { style: 'currency', currency: 'USD' })
const fmtDate = (d) => new Date(d).toLocaleDateString()

onMounted(async () => {
  try {
    const { data } = await api.get('/checkout/orders')
    history.value = data
  } catch (e) {
    error.value = e.response?.data?.message || 'Could not load orders.'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div>
    <h1 class="page-title">Order History</h1>

    <p v-if="loading" class="hint">Loading orders...</p>
    <p v-if="error" class="error">{{ error }}</p>

    <div v-if="history && history.orders && history.orders.length === 0" class="empty">
      You have not placed any orders yet.
    </div>

    <table v-if="history && history.orders && history.orders.length" class="orders">
      <thead>
        <tr>
          <th>Order #</th>
          <th>Date</th>
          <th>Status</th>
          <th>Items</th>
          <th>Total</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="o in history.orders" :key="o.id">
          <td>{{ o.orderNumber || o.id }}</td>
          <td>{{ fmtDate(o.orderDate) }}</td>
          <td><span class="status" :class="o.status">{{ o.status }}</span></td>
          <td>{{ o.itemCount }}</td>
          <td class="total">{{ fmt(o.grandTotal) }}</td>
          <td>
            <router-link
              :to="{ name: 'order-confirmation', params: { id: o.id } }"
              class="view"
            >View</router-link>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
.page-title {
  font-size: 1.4rem;
  border-bottom: 2px solid #3498db;
  padding-bottom: 8px;
  margin: 0 0 18px;
}
.orders {
  width: 100%;
  border-collapse: collapse;
  background: #fff;
  border: 1px solid #e1e1e1;
  border-radius: 8px;
}
.orders th,
.orders td {
  text-align: left;
  padding: 12px;
  border-bottom: 1px solid #eee;
}
.orders th {
  background: #f8f9fa;
  font-size: 0.85rem;
  text-transform: uppercase;
  color: #7f8c8d;
}
.status {
  text-transform: capitalize;
  padding: 3px 10px;
  border-radius: 12px;
  background: #f0f8ff;
  border: 1px solid #b3d9f7;
  font-size: 0.85rem;
}
.total {
  font-weight: 700;
}
.view {
  color: #3498db;
}
.hint,
.empty {
  color: #95a5a6;
}
.error {
  color: #e74c3c;
}
</style>
