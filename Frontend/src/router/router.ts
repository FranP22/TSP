import { createWebHistory, createRouter } from 'vue-router'
import LoginView from './components/LoginView.vue'
import HomeView from './components/HomeView.vue'
import ProfileView from './components/ProfileView.vue'

const routes = [
    { path: '/', component: HomeView },
    { path: '/login', component: LoginView }
    { path: '/profile', component: ProfileView }
]

export const router = createRouter({
    history: createWebHistory(),
    routes
})