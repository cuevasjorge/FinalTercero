using FinalTercero.Interfaces;
using FinalTercero.Models;

namespace FinalTercero.Repositories
{
    public class UsersInMemoryRepository : IUserRepository
    {
        private readonly List<Usuario> _users = new List<Usuario>
    {
        new()
        {
                Id = 1,
                usuario = "alan",
                password = "123456",
                rol = "admin",
                email = "alan@gmail.com"
            },
        new()
        {
                Id = 2,
                usuario = "juan",
                password = "123456",
                rol = "empleado",
                email = "juan@gmail.com"
            },
    };

        public async Task<Usuario?> GetUserByCredentials(string username, string password)
        {
            return _users.FirstOrDefault(p => p.usuario.Equals(username) && p.password.Equals(password));
        }
    }

}

