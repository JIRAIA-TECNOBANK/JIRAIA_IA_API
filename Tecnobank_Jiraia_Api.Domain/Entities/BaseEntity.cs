using System.ComponentModel.DataAnnotations;

namespace Tecnobank_Jiraia_Api.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
