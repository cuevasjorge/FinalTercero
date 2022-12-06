namespace FinalTercero.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string usuario { get; set; }
        public string password { get; set; }
        public string rol { get; set; }
        public string email { get; set; }

        public static List<Usuario> DB()
        {
            var list = new List<Usuario>()
        {
            new Usuario
            {
                Id = 1,
                usuario = "alan",
                password = "123456",
                rol = "admin",
                email = "alan@gmail.com"
            },
            new Usuario
            {
                Id = 2,
                usuario = "juan",
                password = "123456",
                rol = "empleado",
                email = "juan@gmail.com"
            },
        };

            return list;
        }

    }
}

