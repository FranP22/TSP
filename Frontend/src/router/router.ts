import { createWebHistory, createRouter } from 'vue-router'
import HomePage from '@/pages/HomePage.vue'
import LoginPage from '@/pages/LoginPage.vue'
import ProfilePage from '@/pages/ProfilePage.vue'

const routes = [
    { path: '/', component: HomePage },
    { path: '/login', component: LoginPage },
    { path: '/profile', component: ProfilePage }
]

export const router = createRouter({
    history: createWebHistory(),
    routes
})