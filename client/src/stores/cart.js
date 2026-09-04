import { defineStore } from 'pinia'
import api from '../services/api'

export const useCartStore = defineStore('cart', {
  state: () => ({
    lines: [],
    couponCode: '',
    discount: 0,
    shippingTotal: 0,
    taxTotal: 0,
    subtotal: 0,
    grandTotal: 0,
    itemCount: 0,
    loaded: false
  }),
  getters: {
    hasItems: (state) => state.lines.length > 0
  },
  actions: {
    normalize(cart) {
      if (!cart) return
      this.lines = cart.lines || []
      this.couponCode = cart.couponCode || ''
      this.discount = cart.discount || 0
      this.shippingTotal = cart.shippingTotal || 0
      this.taxTotal = cart.taxTotal || 0
      this.subtotal = cart.subtotal || 0
      this.grandTotal = cart.grandTotal || 0
      this.itemCount = cart.itemCount || 0
      this.loaded = true
    },
    async fetchCart() {
      const { data } = await api.get('/cart')
      this.normalize(data)
      return data
    },
    async add(productId, quantity = 1) {
      const { data } = await api.post('/cart/add', { productId, quantity })
      this.normalize(data.cart)
      return data
    },
    async update(productId, quantity) {
      const { data } = await api.post('/cart/update', { productId, quantity })
      this.normalize(data)
      return data
    },
    async remove(productId) {
      const { data } = await api.post('/cart/remove', { productId })
      this.normalize(data)
    },
    async applyCoupon(couponCode) {
      const { data } = await api.post('/cart/coupon', { couponCode })
      this.normalize(data)
      return data
    },
    async clear() {
      await api.delete('/cart')
      this.lines = []
      this.couponCode = ''
      this.discount = 0
      this.shippingTotal = 0
      this.taxTotal = 0
      this.subtotal = 0
      this.grandTotal = 0
      this.itemCount = 0
    }
  }
})
