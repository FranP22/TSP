import { useAuthStore } from "@/stores/authStore";
import type { JwtPayload } from "@/types/app/jwtTypes";
import { jwtDecode } from "jwt-decode";

export const decodeAccessToken = (token: string): JwtPayload | null => {
    try {
        return jwtDecode<JwtPayload>(token)
    } catch {
        return null
    }
}

let refreshTimeout: ReturnType<typeof setTimeout> | null = null;

export const scheduleTokenRefresh = (
    token: string, 
    refresh: () => Promise<unknown>, 
    bufferSeconds = 60): void => {

    if (refreshTimeout) {
        clearTimeout(refreshTimeout)
    }

    const auth = useAuthStore()
    const user = auth.user

    if (!user?.exp) return

    const expiresIn = user.exp * 1000 - Date.now()
    const refreshIn = expiresIn - bufferSeconds * 1000

    if (refreshIn <= 0) {
        void refresh()
        return
    }

    refreshTimeout = setTimeout(() => {
        void refresh()
    }, refreshIn)
}

export const clearTokenRefresh = (): void => {
    if (refreshTimeout) {
        clearTimeout(refreshTimeout)
        refreshTimeout = null
    }
}