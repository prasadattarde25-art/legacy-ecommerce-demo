import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
  withCredentials: true
})

// Attach JWT from localStorage when present
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// On 401, clear the token (session expired)
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('customer')
    }
    return Promise.reject(error)
  }
)

export default api
