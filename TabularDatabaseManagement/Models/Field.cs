using System.ComponentModel.DataAnnotations;

namespace TabularDatabaseManagement.Models
{
    public class Field
    {
        [Required]
        public string Name { get; set; }
        public DataType Type { get; set; }
    }
}
