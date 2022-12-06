using FinalTercero.Models;

namespace FinalTercero.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> Authenticate(string username, string password);
        Task<string> GenerateJwt(Usuario user);
    }
}
