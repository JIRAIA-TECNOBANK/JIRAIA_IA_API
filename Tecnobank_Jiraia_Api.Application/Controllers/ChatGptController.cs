using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Tecnobank_Jiraia_Api.Domain.Dto;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Services;

namespace Tecnobank_Jiraia_Api.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]

    public class ChatGptController : ControllerBase
    {
        private readonly ILogger<ChatGptController> _logger;
        private readonly IChatGptService _service;

        public ChatGptController(ILogger<ChatGptController> logger, IChatGptService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>Realizar consulta</summary>
        /// <param name="idAgrupador">Id agrupador de contexto</param>
        /// <param name="mensagem">Como posso ajudar?</param>
        /// <returns></returns>
        /// <response code="200">Dados da requisição</response>
        /// <response code="400">Exceção para requisição</response>
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.BadRequest)]
        [HttpPost("chat/{mensagem}")]
        public async Task<IActionResult> Chat(int? idAgrupador, string mensagem)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ChatGptController :: Chat -> ErrorMessage: Modelo inválido");
                return BadRequest(ModelState);
            }

            try
            {
                return StatusCode((int)HttpStatusCode.OK, await _service.Chat(idAgrupador, mensagem));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResponseDto { status = (int)HttpStatusCode.BadRequest, isvalid = false, message = ex.Message });
            }
        }

        /// <summary>Resumir documento</summary>
        /// <param name="documento">Documento a ser resumido</param>
        /// <returns></returns>
        /// <response code="200">Dados da requisição</response>
        /// <response code="400">Exceção para requisição</response>
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.BadRequest)]
        [HttpPost("resumirdocumento")]
        public async Task<IActionResult> ResumirDocumento(IFormFile documento)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ChatGptController :: ResumirDocumento -> ErrorMessage: Modelo inválido");
                return BadRequest(ModelState);
            }

            try
            {
                return StatusCode((int)HttpStatusCode.OK, await _service.ResumirDocumento(documento));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResponseDto { status = (int)HttpStatusCode.BadRequest, isvalid = false, message = ex.Message });
            }
        }

        /// <summary>Encontrar documento por palavras</summary>
        /// <param name="palavra">O que deseja encontrar?</param>
        /// <returns></returns>
        /// <response code="200">Dados da requisição</response>
        /// <response code="400">Exceção para requisição</response>
        [ProducesResponseType(typeof(DadosArquivoResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.BadRequest)]
        [HttpPost("encontrardocumento/{palavra}")]
        public async Task<IActionResult> EncontrarDocumento(string palavra)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ChatGptController :: EncontrarDocumento -> ErrorMessage: Modelo inválido");
                return BadRequest(ModelState);
            }

            try
            {
                return StatusCode((int)HttpStatusCode.OK, await _service.EncontrarDocumento(palavra));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResponseDto { status = (int)HttpStatusCode.BadRequest, isvalid = false, message = ex.Message });
            }
        }
    }
}
