import type { JwtTokenResponse, LoginRequest } from "@/types/api/authTypes";
import api from "../client";
import type { BaseResponse } from "@/types/api/apiTypes";
import axios from "axios";

export const authService = {
    async login(request: LoginRequest) {
        const response = await api.post<BaseResponse<JwtTokenResponse>>("/auth/login", request)

        return response
    },

    async logout() {
        const response = await api.post<BaseResponse>("/auth/logout")

        return response
    },

    async refresh() {
        const response = await api.post<BaseResponse<JwtTokenResponse>>("auth/refresh")

        return response
    }
}