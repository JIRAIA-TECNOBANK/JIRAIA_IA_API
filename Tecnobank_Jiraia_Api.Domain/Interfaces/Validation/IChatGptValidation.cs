using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Tecnobank_Jiraia_Api.Domain.Dto;

namespace Tecnobank_Jiraia_Api.Domain.Interfaces.Validation
{
    public interface IChatGptValidation
    {
        RetornarDetalhesArquivoDto ResumirDocumentoValidation(IFormFile documento);

        List<string> EncontrarDocumento(string palavra);
    }
}
