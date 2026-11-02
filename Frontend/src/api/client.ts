import { useAuthStore } from '@/stores/authStore'
import axios, { AxiosError } from 'axios'

const LOGIN_URL = "/auth/login";
const REFRESH_URL = "/auth/refresh";

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
    timeout: 10000,
    withCredentials: true,
    headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json',
    },
})

api.interceptors.request.use(
    (config) => {
        const authStore = useAuthStore();

        if (authStore.accessToken) {
            config.headers.Authorization = `Bearer ${authStore.accessToken}`
        }

        return config
    },
    (error) => Promise.reject(error),
)

type RetryableConfig = NonNullable<AxiosError["config"]> & {
    _retry?: boolean;
};

api.interceptors.response.use(
    (response) => response,
    async (error: AxiosError) => {
        const originalRequest = error.config as RetryableConfig | undefined;

        if (!originalRequest || error.response?.status !== 401) {
            return Promise.reject(error);
        }

        const url = originalRequest.url ?? "";

        // Don't refresh after a failed login or refresh request.
        if (url.includes(LOGIN_URL) || url.includes(REFRESH_URL)) {
            return Promise.reject(error);
        }

        // Don't retry the same request more than once.
        if (originalRequest._retry) {
            return Promise.reject(error);
        }

        originalRequest._retry = true;

        const authStore = useAuthStore()

        try {
            await authStore.refresh()

            return api(originalRequest);
        } catch (refreshError) {

            authStore.logout()

            return Promise.reject(refreshError);
        }
    }
)

export default api