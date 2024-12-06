using System;

namespace Tecnobank_Jiraia_Api.Domain.Entities
{
    public class TbArquivoNormativoEntity : BaseEntity
    {
        public string Diretorio { get; set; }

        public StatusArquivoNormativo Status { get; set; }

        public string NomePortaria { get; set; }

        public bool? EhVisaoEstadual { get; set; }

        public int? TipoPortaria { get; set; }

        public int? TipoRegistro { get; set; }

        public string Estado { get; set; }

        public DateTime? DataVigencia { get; set; }

        public DateTime DtHrCriado { get; set; }

        public DateTime? DtHrModificado { get; set; }
    }

    public enum StatusArquivoNormativo
    {
        Processando = 0,
        AguardandoAprovacao = 1,
        Aprovado = 2,
        Rejeitado = 3
    }
}
