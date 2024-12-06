using System.Collections.Generic;

namespace Tecnobank_Jiraia_Api.Domain.Dto
{
    public class ArquivoNormativoProcessadoDto
    {
        public string Ato { get; set; }
        public string Link { get; set; }
        public string Data { get; set; }
        public string Diario { get; set; }
        public string Ementa { get; set; }
        public string Observacoes { get; set; }
        public string DataVigencia { get; set; }
        public string Impacto { get; set; }
    }
}
