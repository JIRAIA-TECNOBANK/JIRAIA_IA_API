using System.Collections.Generic;

namespace Tecnobank_Jiraia_Api.Domain.Dto
{
    public class DadosArquivoResponseDto
    {
        public int quantidade { get; set; }

        public  List<ListaArquivos> lista { get; set; }
    }

    public class ListaArquivos
    {
        public string nome { get; set; }

        public string base64 { get; set; }
    }
}
