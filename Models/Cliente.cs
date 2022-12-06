using System.ComponentModel.DataAnnotations;

namespace FinalTercero.Models
{
    public class Cliente
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Range(2, 25)]
        public string Nombres { get; set; }
        [Required]
        [Range(2, 25)]
        public string Apellidos { get; set; }
        [Required]
        [Range(2, 10)]
        public int Documento { get; set; }
        [Required]
        public int Telefono { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FechaNacimiento { get; set; }
        [Required]
        public string Ciudad { get; set; }
        [Required]
        public string Nacionalidad { get; set; }
        [Required]
        public string Cargo { get; set; }
        [Required]
        public string Antiguedad { get; set; }


    }
}
