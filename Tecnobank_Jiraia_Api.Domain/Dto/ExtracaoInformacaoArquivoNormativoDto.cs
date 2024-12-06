using System;

namespace Tecnobank_Jiraia_Api.Domain.Dto
{
    public class ExtracaoInformacaoArquivoNormativoDto
    {
        public string? NomePortaria { get; set; }

        public bool? EhVisaoEstadual { get; set; }

        public string? TipoPortaria { get; set; }

        public string? TipoRegistro { get; set; }

        public string? Estado { get; set; }

        public string? DataVigencia { get; set; }
    }
}
