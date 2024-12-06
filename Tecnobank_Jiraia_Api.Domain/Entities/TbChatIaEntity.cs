using System;
using System.ComponentModel.DataAnnotations;

namespace Tecnobank_Jiraia_Api.Domain.Entities
{
    public class TbChatIaEntity
    {
        [Key]
        public int Id { get; set; }

        public int? IdAgrupador { get; set; }

        public int TipoInteracao { get; set; }

        public string Interacao { get; set; }

        public DateTime DtHrInteracao { get; set; }
    }
}
