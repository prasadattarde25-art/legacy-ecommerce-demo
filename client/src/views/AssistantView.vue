<script setup>
import { ref, nextTick } from 'vue'
import api from '../services/api'

const messages = ref([
  {
    role: 'assistant',
    content: "Hi! I'm the Legacy Store assistant. Ask me about products, shipping, coupons, or anything else about the store."
  }
])
const input = ref('')
const busy = ref(false)
const error = ref('')
const chatEl = ref(null)

async function scrollToBottom() {
  await nextTick()
  if (chatEl.value) chatEl.value.scrollTop = chatEl.value.scrollHeight
}

async function send() {
  const text = input.value.trim()
  if (!text || busy.value) return

  messages.value.push({ role: 'user', content: text })
  input.value = ''
  error.value = ''
  busy.value = true
  await scrollToBottom()

  try {
    const { data } = await api.post('/ai/chat', {
      messages: messages.value.map((m) => ({ role: m.role, content: m.content }))
    })
    if (data.success) {
      messages.value.push({ role: data.role || 'assistant', content: data.content })
    } else {
      error.value = data.message || 'No reply.'
    }
  } catch (e) {
    const msg = e.response?.data?.message
    error.value =
      msg && msg.includes('API key')
        ? 'OpenRouter API key is not set. Ask to configure OpenRouter:ApiKey to enable the assistant.'
        : (msg || 'Sorry, the assistant is unavailable right now.')
  } finally {
    busy.value = false
    await scrollToBottom()
  }
}
</script>

<template>
  <div class="assistant">
    <h1 class="page-title">Store Assistant</h1>
    <p class="sub">AI-powered help · powered by OpenRouter</p>

    <div ref="chatEl" class="chat">
      <div
        v-for="(m, i) in messages"
        :key="i"
        class="msg"
        :class="m.role === 'user' ? 'user' : 'assistant'"
      >
        <div class="bubble">{{ m.content }}</div>
      </div>
      <div v-if="busy" class="msg assistant">
        <div class="bubble typing">…</div>
      </div>
    </div>

    <p v-if="error" class="error">{{ error }}</p>

    <form class="composer" @submit.prevent="send">
      <input
        v-model="input"
        placeholder="Ask about products, shipping, coupons..."
        :disabled="busy"
      />
      <button type="submit" class="btn btn-primary" :disabled="busy || !input.trim()">
        Send
      </button>
    </form>
  </div>
</template>

<style scoped>
.page-title {
  font-size: 1.4rem;
  border-bottom: 2px solid #3498db;
  padding-bottom: 8px;
  margin: 0 0 4px;
}
.sub {
  color: #7f8c8d;
  margin: 0 0 16px;
  font-size: 0.9rem;
}
.chat {
  background: #fff;
  border: 1px solid #e1e1e1;
  border-radius: 8px;
  padding: 16px;
  height: 460px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.msg {
  display: flex;
}
.msg.user {
  justify-content: flex-end;
}
.bubble {
  max-width: 78%;
  padding: 10px 14px;
  border-radius: 14px;
  line-height: 1.5;
  white-space: pre-wrap;
}
.msg.assistant .bubble {
  background: #f0f4f8;
  border-bottom-left-radius: 4px;
}
.msg.user .bubble {
  background: #3498db;
  color: #fff;
  border-bottom-right-radius: 4px;
}
.typing {
  color: #95a5a6;
}
.error {
  color: #e74c3c;
  font-size: 0.9rem;
  margin: 8px 0 0;
}
.composer {
  display: flex;
  gap: 8px;
  margin-top: 12px;
}
.composer input {
  flex: 1;
  padding: 10px 12px;
  border: 1px solid #ccc;
  border-radius: 6px;
}
</style>
