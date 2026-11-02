import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import { createPinia } from 'pinia'
import { useAuthStore } from './stores/authStore.ts'
import { router } from './router/router.ts'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)

app.use(router)
app.mount('#app')

