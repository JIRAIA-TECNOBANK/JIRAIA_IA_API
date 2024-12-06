using System;
using System.Collections.Generic;
using Tecnobank_Jiraia_Api.Domain.Entities;

namespace Tecnobank_Jiraia_Api.Domain.Dto
{
    public class ArquivoCompiladoResponseDto
    {
        public string Nome { get; set; }
        public StatusArquivoCompilado Status { get; set;}
        public DateTime DtHrCriado { get; set;}
        public DateTime? DtHrModificado { get; set;}
    }
}
