<script setup lang="ts">
    import { useAuthStore } from '@/stores/authStore'
    import type { LoginRequest } from '@/types/auth'
    import { ref, reactive } from 'vue'
    import { RouterLink, useRouter } from 'vue-router'

    const router = useRouter()

    const showPassword = ref(false)

    const error = ref('')
    const loading = ref(false)

    const form = reactive<LoginRequest>({
        email: '',
        password: ''
    })

    async function handleSubmit() {
        error.value = ''

        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
            error.value = 'Please enter a valid email address.'
            return
        }

        if (form.password.length < 8) {
            error.value = 'Password must contain at least 8 characters.'
            return
        }

        try {
            const auth = useAuthStore()
            const response = await auth.login(form)

            if (response.status == 401) {
                error.value = 'Invalid email or password.'
                return
            }

            router.push('/')
            return

            
        } catch (err) {
            error.value = 'An issue has occured.'
        }
    }
</script>

<template>
    <section class="w-full max-w-md rounded-2xl border border-slate-200 bg-white p-8 shadow-sm sm:p-10">
        <!-- Header -->
        <div class="mb-8 text-center">
            <h1 class="text-2xl font-bold tracking-tight text-slate-900">
                Welcome back!
            </h1>

            <p class="mt-2 text-sm text-slate-500">
                Sign in to your account
            </p>
        </div>

        <form class="space-y-5" @submit.prevent="handleSubmit">
            <!-- Email -->
            <div>
                <label for="email" class="mb-2 block text-sm font-medium text-slate-700">
                    Email address
                </label>

                <input id="email" v-model="form.email" type="email" autocomplete="email" placeholder="you@example.com" required class="w-full rounded-lg border border-slate-300 px-4 py-3 text-sm outline-none transition placeholder:text-slate-400 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"/>
            </div>

            <!-- Password -->
            <div>
                <div class="mb-2 flex items-center justify-between">
                    <label for="password" class="block text-sm font-medium text-slate-700">
                        Password
                    </label>

                    <RouterLink to="/forgot-password" class="text-xs font-medium text-indigo-600 hover:text-indigo-700">
                        Forgot password?
                    </RouterLink>
                </div>

                <div class="relative">
                    <input id="password" v-model="form.password" :type="showPassword ? 'text' : 'password'" :autocomplete="'current-password'" placeholder="At least 8 characters" minlength="8" required class="w-full rounded-lg border border-slate-300 px-4 py-3 pr-20 text-sm outline-none transition placeholder:text-slate-400 focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"/>

                    <button type="button" class="absolute inset-y-0 right-3 text-xs font-medium text-slate-500 hover:text-indigo-600" @click="showPassword = !showPassword">
                        {{ showPassword ? 'Hide' : 'Show' }}
                    </button>
                </div>
            </div>

            <!-- Error message -->
            <p v-if="error" role="alert" class="rounded-lg bg-red-50 p-3 text-sm text-red-600">
                {{ error }}
            </p>

            <!-- Submit -->
            <button type="submit" :disabled="loading" class="w-full rounded-lg bg-indigo-600 px-4 py-3 text-sm font-semibold text-white transition hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-60">
                {{ loading ? 'Please wait...' : 'Sign in' }}
            </button>
        </form>

        <p class="mt-6 text-center text-xs leading-5 text-slate-400">
            Don't have an account?
            <RouterLink to="/register" class="font-semibold text-indigo-600 hover:text-indigo-700">
                Register
            </RouterLink>
        </p>

        <p class="mt-6 text-center text-xs leading-5 text-slate-400">
            By continuing, you agree to our Terms of Service and Privacy Policy.
        </p>
    </section>
</template>