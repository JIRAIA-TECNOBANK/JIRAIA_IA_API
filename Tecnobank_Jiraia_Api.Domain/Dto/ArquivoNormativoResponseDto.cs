using System;
using Tecnobank_Jiraia_Api.Domain.Entities;

namespace Tecnobank_Jiraia_Api.Domain.Dto
{
    public class ArquivoNormativoResponseDto
    {
        public int Id { get; set; }
        public StatusArquivoNormativo Status { get; set; }
        public string? NomePortaria { get; set; }
        public bool? EhVisaoEstadual { get; set; }
        public int? TipoPortaria { get; set; }
        public int? TipoRegistro { get; set; }
        public string? Estado { get; set; }
        public DateTime? DataVigencia { get; set; }
        public DateTime DtHrCriado { get; set; }
        public DateTime? DtHrModificado { get; set; }
    }
}
