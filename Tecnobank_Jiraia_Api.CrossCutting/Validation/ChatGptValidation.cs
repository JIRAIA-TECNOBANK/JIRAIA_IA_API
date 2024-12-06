using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Tecnobank_Jiraia_Api.Domain.Dto;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Validation;

namespace Tecnobank_Jiraia_Api.CrossCutting.Validation
{
    public class ChatGptValidation : IChatGptValidation
    {
        public ChatGptValidation() { }

        public RetornarDetalhesArquivoDto ResumirDocumentoValidation(IFormFile documento)
        {
            if (documento == null)
                throw new ArgumentNullException("documento", "Documento deve ser informado");

            return RetornarDetalhesArquivo(documento);
        }

        public List<string> EncontrarDocumento(string palavra)
        {
            List<string> listaPalavras = new();
            string[] palavrasArray = palavra.Trim().Split(" ");

            foreach (var item in palavrasArray)
                if (item.Length > 3)
                    listaPalavras.Add(item);

            return listaPalavras;
        }

        private RetornarDetalhesArquivoDto RetornarDetalhesArquivo(IFormFile documento)
        {
            string[] auxArray = documento.FileName.Split('.');
            string extensao = auxArray.Last().ToLower();
            string nomeArquivo = documento.FileName.ToLower().Replace(".txt", "").Replace(".pdf", "");

            if (!extensao.Equals("txt") && !extensao.Equals("pdf"))
                throw new FormatException("Formato de documento inválido. Apenas arquivos .TXT e .PDF são aceitos");

            return new RetornarDetalhesArquivoDto { nome = nomeArquivo, extensao = extensao };
        }
    }
}
