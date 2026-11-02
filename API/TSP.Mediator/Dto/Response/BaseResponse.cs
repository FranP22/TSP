namespace TSP.Mediator.Dto.Response
{
    public class BaseResponse<T> : BaseResponse
    {
        public T Value { get; set; }
    }

    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
