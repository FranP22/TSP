import { authService } from "@/api/endpoints/authService";
import { decodeJwtToken, scheduleTokenRefresh } from "@/functions/jwt";
import { type User, type LoginRequest } from "@/types/auth";
import type { JwtPayload } from "@/types/jwt";
import { defineStore } from "pinia";
import { computed, ref } from "vue";


export const useAuthStore = defineStore('auth', () => {
    const user = ref<JwtPayload | null>(null)
    const accessToken = ref<string | null>(null)

    const isAuthenticated = computed(() => !!accessToken.value)
    const getName = computed(() => user.value?.email.split('@')[0] ?? 'User')

    const login = async (request: LoginRequest) => {
        const response = await authService.login(request)

        if (response.status != 200) return response

        const decoded = decodeJwtToken(response.data.accessToken)

        if (decoded != null) {
            accessToken.value = response.data.accessToken
            user.value = decoded

            scheduleTokenRefresh(response.data.accessToken)
        }

        return response
    }

    const logout = async () => {
        const response = await authService.logout()

        if (response.status != 200) return response

        accessToken.value = null
        user.value = null

        return response
    }

    const refresh = async () => {
        const response = await authService.refresh()

        if (response.status != 200) return response

        const decoded = decodeJwtToken(response.data.accessToken)

        if (decoded != null) {
            accessToken.value = response.data.accessToken
            user.value = decoded

            scheduleTokenRefresh(response.data.accessToken)
        }

        return response
    }

    return {
        user,
        accessToken,
        isAuthenticated,
        getName,
        login,
        logout,
        refresh,
    }
})