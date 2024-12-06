using Tecnobank_Jiraia_Api.Domain.Enum;

namespace Tecnobank_Jiraia_Api.Domain.Interfaces.Util
{
    public interface IUtilidade
    {
        string RegexWords(string chatDocumento, string text);

        string RetornarBase64Arquivo(string diretorio);
    }
}
