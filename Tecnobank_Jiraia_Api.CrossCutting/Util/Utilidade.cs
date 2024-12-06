using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Util;

namespace Tecnobank_Jiraia_Api.CrossCutting.Util
{
    public class Utilidade : IUtilidade
    {
        public Utilidade() { }

        public string RegexWords(string chatDocumento, string text)
        {
            var normalizedText = text.Normalize(NormalizationForm.FormD);
            var regex = new Regex(@"\p{M}");
            string cleanText = regex.Replace(normalizedText, "").Normalize(NormalizationForm.FormC);

            if (chatDocumento.Equals("chat"))
                return Regex.Replace(cleanText, @"[^\w\s]", "").Trim().ToLower();
            else
                return Regex.Replace(cleanText, @"[^\w\s]", "").Replace(" ", "-").Trim().ToLower();
        }

        public string RetornarBase64Arquivo(string diretorio) =>
            Convert.ToBase64String(File.ReadAllBytes(diretorio));
    }
}
