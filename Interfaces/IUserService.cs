using FinalTercero.Models;

namespace FinalTercero.Interfaces
{
    public interface IUserService
    {
        Task<Usuario> GetByCredentials(string username, string password);
    }
}
