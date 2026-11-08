import { authService } from "@/api/service/authService";
import type { LoginRequest } from "@/types/api/authTypes";
import type { JwtPayload } from "@/types/app/jwtTypes";
import { clearTokenRefresh, decodeAccessToken, scheduleTokenRefresh } from "@/utils/jwtUtils";
import axios from "axios";
import { defineStore } from "pinia";
import { computed, ref } from "vue";


export const useAuthStore = defineStore('auth', () => {
    const user = ref<JwtPayload | null>(null)
    const accessToken = ref<string | null>(null)

    const isAuthenticated = computed(() => !!accessToken.value)
    const getName = computed(() => user.value?.email.split('@')[0] ?? 'User')

    const clearAuth = () => {
        accessToken.value = null
        user.value = null

        clearTokenRefresh()
    }

    const login = async (request: LoginRequest) => {
        try {
            const response = await authService.login(request)

            if (response.status != 200) return response

            const decoded = decodeAccessToken(response.data.value.accessToken)

            if (decoded != null) {
                accessToken.value = response.data.value.accessToken
                user.value = decoded

                scheduleTokenRefresh(response.data.value.accessToken, refresh)
            }

            return response
        }
        catch (error) {
            if (axios.isAxiosError(error) && error.response?.status === 401) {
                return error.response
            }

            throw error;
        }
    }

    const logout = async () => {
        try {
            return await authService.logout()
        }
        finally {
            clearAuth()
        }
    }

    const refresh = async () => {
        const response = await authService.refresh()

        if (!response.data?.value?.accessToken) {
            clearAuth()
            return response
        }

        const decoded = decodeAccessToken(response.data.value.accessToken)

        if (!decoded) {
            clearAuth()
            return response
        }

        accessToken.value = response.data.value.accessToken
        user.value = decoded

        scheduleTokenRefresh(response.data.value.accessToken, refresh)

        return response
    }

    return {
        user,
        accessToken,
        isAuthenticated,
        getName,
        clearAuth,
        login,
        logout,
        refresh,
    }
})