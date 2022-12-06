using FinalTercero.Models;

namespace FinalTercero.Interfaces
{
    public interface IUserRepository
    {
        Task<Usuario?> GetUserByCredentials(string username, string password);
    }
}
