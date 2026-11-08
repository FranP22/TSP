export interface BaseResponse<T = null> {
    success: boolean,
    message: string,
    errors: Array<string>
    value: T
}