using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public class EntityBase
    {
        [Key]
        public int Id { get; set; }
    }
}
