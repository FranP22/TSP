namespace TSP.Services.Service.Interface
{
    public interface IAuthService
    {
        Task LoginAsync(string email,  string password);
    }
}
