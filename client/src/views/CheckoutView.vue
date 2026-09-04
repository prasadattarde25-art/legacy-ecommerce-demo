<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'
import { useCartStore } from '../stores/cart'

const router = useRouter()
const cart = useCartStore()

const step = ref(1)
const submitting = ref(false)
const error = ref('')

const address = ref({
  email: '',
  firstName: '',
  lastName: '',
  addressLine1: '',
  addressLine2: '',
  city: '',
  state: '',
  postalCode: '',
  country: '',
  phone: ''
})

const shipping = ref({
  shippingMethod: 'Standard',
  deliveryNotes: ''
})

const payment = ref({
  paymentMethod: 'Card',
  cardHolderName: '',
  cardNumber: '',
  expiryMonth: null,
  expiryYear: null,
  cvv: ''
})

const shippingMethods = ['Standard', 'Express', 'Overnight']

function nextFromAddress() {
  step.value = 2
}
function backToAddress() {
  step.value = 1
}
function nextFromShipping() {
  step.value = 3
}
function backToShipping() {
  step.value = 2
}

async function placeOrder() {
  submitting.value = true
  error.value = ''
  try {
    const { data } = await api.post('/checkout', {
      address: address.value,
      shipping: shipping.value,
      payment: payment.value
    })
    if (!data.success) {
      error.value = data.message || 'Checkout failed.'
      return
    }
    await cart.clear()
    router.push({ name: 'order-confirmation', params: { id: data.order.id } })
  } catch (e) {
    error.value = e.response?.data?.message || 'Checkout failed. Please try again.'
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div class="checkout">
    <h1 class="page-title">Checkout</h1>

    <div class="stepper">
      <div :class="['step', { active: step === 1, done: step > 1 }]">1 · Address</div>
      <div :class="['step', { active: step === 2, done: step > 2 }]">2 · Shipping</div>
      <div :class="['step', { active: step === 3 }]">3 · Payment</div>
    </div>

    <div v-if="!cart.hasItems && step === 1" class="empty">
      Your cart is empty. <router-link :to="{ name: 'products' }">Browse products</router-link>
    </div>

    <!-- Step 1: Address -->
    <form v-else-if="step === 1" class="step-form" @submit.prevent="nextFromAddress">
      <h2>Shipping Address</h2>
      <div class="grid">
        <label>Email *<input v-model="address.email" type="email" required /></label>
        <label>First Name *<input v-model="address.firstName" required /></label>
        <label>Last Name *<input v-model="address.lastName" required /></label>
        <label class="full">Address Line 1 *<input v-model="address.addressLine1" required /></label>
        <label class="full">Address Line 2<input v-model="address.addressLine2" /></label>
        <label>City *<input v-model="address.city" required /></label>
        <label>State<input v-model="address.state" /></label>
        <label>Postal Code *<input v-model="address.postalCode" required /></label>
        <label>Country<input v-model="address.country" /></label>
        <label>Phone<input v-model="address.phone" /></label>
      </div>
      <button type="submit" class="btn btn-primary">Continue to Shipping</button>
    </form>

    <!-- Step 2: Shipping -->
    <form v-else-if="step === 2" class="step-form" @submit.prevent="nextFromShipping">
      <h2>Shipping Method</h2>
      <label v-for="m in shippingMethods" :key="m" class="radio">
        <input v-model="shipping.shippingMethod" type="radio" :value="m" />
        {{ m }}
      </label>
      <label class="full">
        Delivery Notes
        <textarea v-model="shipping.deliveryNotes" rows="3"></textarea>
      </label>
      <div class="btn-row">
        <button type="button" class="btn" @click="backToAddress">Back</button>
        <button type="submit" class="btn btn-primary">Continue to Payment</button>
      </div>
    </form>

    <!-- Step 3: Payment -->
    <form v-else class="step-form" @submit.prevent="placeOrder">
      <h2>Payment</h2>
      <div class="grid">
        <label class="full">Payment Method *
          <select v-model="payment.paymentMethod">
            <option>Card</option>
            <option>Cash on Delivery</option>
          </select>
        </label>
        <template v-if="payment.paymentMethod === 'Card'">
          <label class="full">Name on Card<input v-model="payment.cardHolderName" /></label>
          <label class="full">Card Number<input v-model="payment.cardNumber" placeholder="4242 4242 4242 4242" /></label>
          <label>Expiry Month<input v-model.number="payment.expiryMonth" type="number" placeholder="12" /></label>
          <label>Expiry Year<input v-model.number="payment.expiryYear" type="number" placeholder="2026" /></label>
          <label>CVV<input v-model="payment.cvv" /></label>
        </template>
      </div>

      <div class="summary">
        <div class="row"><span>Subtotal</span><span>... </span></div>
        <div class="row total"><span>Total</span><span>...</span></div>
      </div>

      <p v-if="error" class="error">{{ error }}</p>

      <div class="btn-row">
        <button type="button" class="btn" @click="backToShipping">Back</button>
        <button type="submit" class="btn btn-primary" :disabled="submitting">
          {{ submitting ? 'Placing order...' : 'Place Order' }}
        </button>
      </div>
    </form>

    <router-link v-if="step > 1" :to="{ name: 'cart' }" class="back-to-cart">← Back to cart</router-link>
  </div>
</template>

<style scoped>
.page-title {
  font-size: 1.4rem;
  border-bottom: 2px solid #3498db;
  padding-bottom: 8px;
  margin: 0 0 18px;
}
.stepper {
  display: flex;
  gap: 8px;
  margin-bottom: 22px;
}
.step {
  padding: 8px 16px;
  border: 1px solid #ccc;
  border-radius: 20px;
  color: #7f8c8d;
}
.step.active {
  background: #3498db;
  color: #fff;
  border-color: #3498db;
}
.step.done {
  background: #27ae60;
  color: #fff;
  border-color: #27ae60;
}
.step-form {
  background: #fff;
  border: 1px solid #e1e1e1;
  border-radius: 8px;
  padding: 24px;
  max-width: 640px;
}
.step-form h2 {
  margin: 0 0 16px;
  font-size: 1.1rem;
}
.grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 14px;
}
.grid label {
  display: flex;
  flex-direction: column;
  gap: 4px;
  font-size: 0.9rem;
}
.grid .full {
  grid-column: span 2;
}
.grid input,
.grid select,
.grid textarea {
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 4px;
}
.radio {
  display: block;
  margin-bottom: 8px;
  background: #fafafa;
  border: 1px solid #e1e1e1;
  padding: 10px 12px;
  border-radius: 4px;
  cursor: pointer;
}
.btn-row {
  display: flex;
  justify-content: space-between;
  margin-top: 20px;
}
.summary {
  margin-top: 20px;
  border-top: 1px solid #eee;
  padding-top: 12px;
}
.row {
  display: flex;
  justify-content: space-between;
}
.row.total {
  font-weight: 700;
  font-size: 1.1rem;
}
.error {
  color: #e74c3c;
}
.back-to-cart {
  display: inline-block;
  margin-top: 16px;
  color: #3498db;
}
.empty {
  color: #95a5a6;
  padding: 30px 0;
}
</style>
