using System.ComponentModel;

namespace Tecnobank_Jiraia_Api.Domain.Enum
{
    public enum TipoInteracao
    {
        [Description("Pergunta")]
        Pergunta = 1,

        [Description("Resposta")]
        Resposta = 2
    }
}
