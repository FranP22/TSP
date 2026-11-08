
<script setup lang="ts">
    import { router } from '@/router/router'
    import { useAuthStore } from '@/stores/authStore'
    import { computed, ref } from 'vue'
    import { RouterLink } from 'vue-router'

    const authStore = useAuthStore()

    const menuOpen = ref(false)
    
    const isLoggedIn = computed(() => authStore.isAuthenticated)

    const links = [
        { name: 'Home', to: '/' },
        //{ name: 'Login', to: '/login' },
        //{ name: 'About', to: '/about' },
        //{ name: 'Contact', to: '/contact' },
    ]

    const logout = async () => {
        await authStore.logout()

        router.push('/')
    }
</script>

<template>
    <nav class="border-b border-slate-200 bg-white">
        <div class="mx-auto flex max-w-7xl items-center justify-between px-6 py-4">
            <!-- Logo -->
            <RouterLink to="/" class="text-2xl font-bold tracking-tight text-indigo-600" @click="menuOpen = false">
                App
            </RouterLink>

            <!-- Desktop navigation -->
            <div class="hidden items-center gap-8 md:flex">
                <RouterLink v-for="link in links" :key="link.name" :to="link.to" class="text-sm font-medium text-slate-600 transition hover:text-indigo-600" active-class="!text-indigo-600">
                    {{ link.name }}
                </RouterLink>

                <RouterLink v-if="isLoggedIn" to="/profile" class="rounded-lg bg-indigo-600 px-5 py-2.5 text-sm font-semibold text-white transition hover:bg-indigo-700">
                    {{ authStore.getName }}
                </RouterLink>

                <button v-if="isLoggedIn" class="rounded-lg bg-indigo-600 px-5 py-2.5 text-sm font-semibold text-white transition hover:bg-indigo-700" @click="logout()">
                    Logout
                </button>

                <RouterLink v-else to="/login" class="rounded-lg bg-indigo-600 px-5 py-2.5 text-sm font-semibold text-white transition hover:bg-indigo-700">
                    Login
                </RouterLink>
            </div>

            <!-- Mobile menu button -->
            <button type="button" class="rounded-lg p-2 text-slate-700 hover:bg-slate-100 md:hidden" :aria-expanded="menuOpen" aria-label="Toggle navigation menu" @click="menuOpen = !menuOpen">
                <svg v-if="!menuOpen" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2" class="h-6 w-6" >
                    <path stroke-linecap="round" stroke-linejoin="round" d="M4 6h16M4 12h16M4 18h16"/>
                </svg>

                <svg v-else xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2" class="h-6 w-6" >
                    <path stroke-linecap="round" stroke-linejoin="round" d="M6 18 18 6M6 6l12 12"/>
                </svg>
            </button>
        </div>

        <!-- Mobile navigation -->
        <div v-if="menuOpen" class="space-y-1 border-t border-slate-200 px-6 py-4 md:hidden">
            <RouterLink v-for="link in links" :key="link.name" :to="link.to" class="block rounded-lg px-3 py-2.5 text-sm font-medium text-slate-600 hover:bg-indigo-50 hover:text-indigo-600" active-class="!bg-indigo-50 !text-indigo-600" @click="menuOpen = false">
                {{ link.name }}
            </RouterLink>

            <RouterLink v-if="isLoggedIn" to="/profile" class="mt-3 block rounded-lg bg-indigo-600 px-4 py-3 text-center font-semibold text-white hover:bg-indigo-700" @click="menuOpen = false">
                {{ authStore.getName }}
            </RouterLink>

            <button v-if="isLoggedIn" class="mt-3 block rounded-lg bg-indigo-600 px-4 py-3 text-center font-semibold text-white hover:bg-indigo-700" @click="menuOpen = false; logout();">
                Logout
            </button>

            <RouterLink v-else to="/login" class="mt-3 block rounded-lg bg-indigo-600 px-4 py-3 text-center font-semibold text-white hover:bg-indigo-700" @click="menuOpen = false">
                Login
            </RouterLink>
        </div>
    </nav>
</template>
