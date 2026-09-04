<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '../stores/cart'

const cart = useCartStore()
const router = useRouter()
const couponInput = ref('')
const couponMsg = ref('')

const fmt = (n) => n.toLocaleString('en-US', { style: 'currency', currency: 'USD' })

async function changeQty(line, qt) {
  if (qt < 1) return
  await cart.update(line.productId, qt)
}

async function removeLine(productId) {
  await cart.remove(productId)
}

async function applyCoupon() {
  couponMsg.value = ''
  if (!couponInput.value.trim()) return
  try {
    await cart.applyCoupon(couponInput.value.trim())
    couponMsg.value = cart.couponCode ? 'Coupon applied.' : 'Coupon not valid.'
  } catch (e) {
    couponMsg.value = 'Coupon not valid.'
  }
}

function checkout() {
  router.push({ name: 'checkout' })
}
</script>

<template>
  <div>
    <h1 class="page-title">Your Cart</h1>

    <div v-if="!cart.hasItems" class="empty">
      Your cart is empty.
      <router-link :to="{ name: 'products' }">Browse products</router-link>
    </div>

    <div v-else class="cart-layout">
      <div class="lines">
        <div v-for="line in cart.lines" :key="line.productId" class="line">
          <div class="line-name">
            <router-link :to="{ name: 'product-detail', params: { id: line.productId } }">
              {{ line.productName }}
            </router-link>
            <span class="line-meta">{{ fmt(line.unitPrice) }} each</span>
          </div>
          <div class="line-qty">
            <input
              type="number"
              :value="line.quantity"
              min="1"
              @change="changeQty(line, Number($event.target.value))"
            />
          </div>
          <div class="line-total">{{ fmt(line.lineTotal) }}</div>
          <button class="btn btn-danger" @click="removeLine(line.productId)">Remove</button>
        </div>
      </div>

      <aside class="summary">
        <h3>Order Summary</h3>
        <div class="row"><span>Subtotal</span><span>{{ fmt(cart.subtotal) }}</span></div>
        <div v-if="cart.couponCode" class="row">
          <span>Coupon ({{ cart.couponCode }})</span>
          <span class="discount">−{{ fmt(cart.discount) }}</span>
        </div>
        <div class="row"><span>Shipping</span><span>{{ fmt(cart.shippingTotal) }}</span></div>
        <div class="row"><span>Tax</span><span>{{ fmt(cart.taxTotal) }}</span></div>
        <div class="row total"><span>Total</span><span>{{ fmt(cart.grandTotal) }}</span></div>

        <form class="coupon" @submit.prevent="applyCoupon">
          <input v-model="couponInput" placeholder="Coupon code" />
          <button type="submit" class="btn">Apply</button>
        </form>
        <p v-if="couponMsg" class="coupon-msg">{{ couponMsg }}</p>

        <button class="btn btn-primary btn-block" @click="checkout">Proceed to Checkout</button>
      </aside>
    </div>
  </div>
</template>

<style scoped>
.page-title {
  font-size: 1.4rem;
  border-bottom: 2px solid #3498db;
  padding-bottom: 8px;
  margin: 0 0 18px;
}
.cart-layout {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: 24px;
  align-items: start;
}
.lines {
  background: #fff;
  border: 1px solid #e1e1e1;
  border-radius: 8px;
}
.line {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 14px 16px;
  border-bottom: 1px solid #eee;
}
.line:last-child {
  border-bottom: none;
}
.line-name {
  flex: 1;
}
.line-name a {
  color: #2c3e50;
  font-weight: 600;
  text-decoration: none;
}
.line-meta {
  display: block;
  font-size: 0.85rem;
  color: #95a5a6;
}
.line-qty input {
  width: 56px;
  padding: 6px;
  border: 1px solid #ccc;
  border-radius: 4px;
}
.line-total {
  font-weight: 700;
  min-width: 80px;
  text-align: right;
}
.summary {
  background: #fff;
  border: 1px solid #e1e1e1;
  border-radius: 8px;
  padding: 18px;
}
.summary h3 {
  margin: 0 0 12px;
  border-bottom: 1px solid #eee;
  padding-bottom: 8px;
}
.row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
  font-size: 0.95rem;
}
.row.total {
  font-weight: 700;
  font-size: 1.1rem;
  border-top: 1px solid #eee;
  padding-top: 10px;
}
.discount {
  color: #27ae60;
}
.coupon {
  display: flex;
  gap: 6px;
  margin-top: 14px;
}
.coupon input {
  flex: 1;
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 4px;
}
.coupon-msg {
  font-size: 0.85rem;
  color: #7f8c8d;
}
.btn-block {
  width: 100%;
  margin-top: 14px;
}
.empty {
  color: #95a5a6;
  padding: 30px 0;
}
</style>
