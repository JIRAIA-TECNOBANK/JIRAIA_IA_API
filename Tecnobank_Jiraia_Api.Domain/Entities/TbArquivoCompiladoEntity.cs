using System.ComponentModel.DataAnnotations;
using System;

namespace Tecnobank_Jiraia_Api.Domain.Entities
{
    public class TbArquivoCompiladoEntity : BaseEntity
    {
        public string Nome { get; set; }

        public string Diretorio { get; set; }

        public StatusArquivoCompilado Status { get; set; }

        public DateTime DtHrCriado { get; set; }

        public DateTime? DtHrModificado { get; set; }
    }

    public enum StatusArquivoCompilado
    {
        AguardandoProcessamento = 0,
        Processando = 1,
        ProcessadoComSucesso = 2,
        ProcessadoComErro = 3
    }
}
