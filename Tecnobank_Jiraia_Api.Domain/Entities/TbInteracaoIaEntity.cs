using System;
using System.ComponentModel.DataAnnotations;

namespace Tecnobank_Jiraia_Api.Domain.Entities
{
    public class TbInteracaoIaEntity
    {
        [Key]
        public int Id { get; set; }

        public string Consulta { get; set; }

        public string Diretorio { get; set; }

        public string TextoOriginal { get; set; }

        public string Resposta { get; set; }

        public DateTime DtHrInteracao { get; set; }
    }
}
