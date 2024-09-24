using System.ComponentModel.DataAnnotations;

namespace AplicacionToDoList.Models
{
    public class Todos
    {
        [Key]
        public int id { get; set; }
        [Required]
        public String title { get; set; } = "";
        
        [Required]
        public String description { get; set; } = "";

        public bool estado { get; set; } = false;

        public int conteoComple { get; set; } = 0;

        public int conteoIncomple { get; set; } = 0;

        public int conteoElimi { get; set; } = 0;

    }
}
