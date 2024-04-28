using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities
{
    public class EntityBase
    {
        [Key]
        public int Id { get; set; }
    }
}
