using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using System;
using Tecnobank_Jiraia_Api.Domain.Dto;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Tecnobank_Jiraia_Api.Application.Controllers
{
    [ApiController]
    [Route("api/monitor-normativo")]
    [Produces("application/json")]
    public class MonitorNormativoController : ControllerBase
    {
        private readonly ILogger<MonitorNormativoController> _logger;
        private readonly IMonitorNormativoService _monitorNormativoService;

        public MonitorNormativoController(ILogger<MonitorNormativoController> logger,
            IMonitorNormativoService monitorNormativoService)
        {
            _logger = logger;
            _monitorNormativoService = monitorNormativoService;
        }

        /// <summary>Envia arquivo compilado para processamento</summary>
        /// <returns></returns>
        /// <response code="201">Arquivo enviado com sucesso</response>
        /// <response code="400">Exceção para requisição</response>
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.BadRequest)]
        [HttpPost("arquivo-compilado")]
        public async Task<IActionResult> EnviarArquivoCompilado(IFormFile arquivo)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("MonitorNormativoController :: EnviarArquivoCompilado -> ErrorMessage: Modelo inválido");
                return BadRequest(ModelState);
            }

            try
            {
                await _monitorNormativoService.EnviarArquivoCompilado(arquivo);

                return StatusCode((int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResponseDto { status = (int)HttpStatusCode.BadRequest, isvalid = false, message = ex.Message });
            }
        }

        /// <summary>Obtem a lista de arquivos compilados enviados</summary>
        /// <returns></returns>
        /// <response code="200">Dados da requisição</response>
        /// <response code="400">Exceção para requisição</response>
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.BadRequest)]
        [HttpGet("arquivo-compilado")]
        public async Task<IActionResult> ObterListaArquivoCompilado()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("MonitorNormativoController :: ObterListaArquivoCompilado -> ErrorMessage: Modelo inválido");
                return BadRequest(ModelState);
            }

            try
            {
                return StatusCode((int)HttpStatusCode.OK, await _monitorNormativoService.ObterListaArquivoCompilado());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResponseDto { status = (int)HttpStatusCode.BadRequest, isvalid = false, message = ex.Message });
            }
        }

        /// <summary>Obtem a lista de arquivos normativos para aprovacao</summary>
        /// <returns></returns>
        /// <response code="200">Dados da requisição</response>
        /// <response code="400">Exceção para requisição</response>
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.BadRequest)]
        [HttpGet("arquivo-normativo/aprovacao")]
        public async Task<IActionResult> ObterListaArquivoNormativoAprovado()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("MonitorNormativoController :: ObterListaArquivoNormativoAprovado -> ErrorMessage: Modelo inválido");
                return BadRequest(ModelState);
            }

            try
            {
                return StatusCode((int)HttpStatusCode.OK, await _monitorNormativoService.ObterListaArquivoNormativoAprovacao());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResponseDto { status = (int)HttpStatusCode.BadRequest, isvalid = false, message = ex.Message });
            }
        }

        /// <summary>Obtem a lista de arquivos normativos rejeitados</summary>
        /// <returns></returns>
        /// <response code="200">Dados da requisição</response>
        /// <response code="400">Exceção para requisição</response>
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.BadRequest)]
        [HttpGet("arquivo-normativo/rejeitado")]
        public async Task<IActionResult> ObterListaArquivoNormativoRejeitado()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("MonitorNormativoController :: ObterListaArquivoNormativoRejeitado -> ErrorMessage: Modelo inválido");
                return BadRequest(ModelState);
            }

            try
            {
                return StatusCode((int)HttpStatusCode.OK, await _monitorNormativoService.ObterListaArquivoNormativoRejeitado());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResponseDto { status = (int)HttpStatusCode.BadRequest, isvalid = false, message = ex.Message });
            }
        }

        /// <summary>Obtem a lista de arquivos normativos aprovados</summary>
        /// <returns></returns>
        /// <response code="200">Dados da requisição</response>
        /// <response code="400">Exceção para requisição</response>
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.BadRequest)]
        [HttpGet("arquivo-normativo/aprovados")]
        public async Task<IActionResult> ObterListaArquivoNormativoAprovados()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("MonitorNormativoController :: ObterListaArquivoNormativoAprovados -> ErrorMessage: Modelo inválido");
                return BadRequest(ModelState);
            }

            try
            {
                return StatusCode((int)HttpStatusCode.OK, await _monitorNormativoService.ObterListaArquivoNormativoAprovados());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResponseDto { status = (int)HttpStatusCode.BadRequest, isvalid = false, message = ex.Message });
            }
        }

        /// <summary>Aprova um arquivo de normativo pendente de aprovacao</summary>
        /// <returns></returns>
        /// <response code="200">Dados da requisição</response>
        /// <response code="400">Exceção para requisição</response>
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.BadRequest)]
        [HttpPost("arquivo-normativo/aprovar")]
        public async Task<IActionResult> AprovarArquivoNormativo(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("MonitorNormativoController :: AprovarArquivoNormativo -> ErrorMessage: Modelo inválido");
                return BadRequest(ModelState);
            }

            try
            {
                return StatusCode((int)HttpStatusCode.OK, await _monitorNormativoService.AprovarArquivoNormativo(id));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResponseDto { status = (int)HttpStatusCode.BadRequest, isvalid = false, message = ex.Message });
            }
        }

        /// <summary>Obtem a lista de arquivos normativos aprovados</summary>
        /// <returns></returns>
        /// <response code="200">Dados da requisição</response>
        /// <response code="400">Exceção para requisição</response>
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.BadRequest)]
        [HttpPost("arquivo-normativo/rejeitar")]
        public async Task<IActionResult> RejeitarArquivoNormativo(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("MonitorNormativoController :: RejeitarArquivoNormativo -> ErrorMessage: Modelo inválido");
                return BadRequest(ModelState);
            }

            try
            {
                return StatusCode((int)HttpStatusCode.OK, await _monitorNormativoService.RejeitarArquivoNormativo(id));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResponseDto { status = (int)HttpStatusCode.BadRequest, isvalid = false, message = ex.Message });
            }
        }

        /// <summary>Obtem o arquivo de um normativo</summary>
        /// <returns></returns>
        /// <response code="200">Dados da requisição</response>
        /// <response code="400">Exceção para requisição</response>
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.BadRequest)]
        [HttpGet("arquivo-normativo/arquivo")]
        public async Task<IActionResult> ObterArquivoNormativo(int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("MonitorNormativoController :: ObterArquivoNormativo -> ErrorMessage: Modelo inválido");
                return BadRequest(ModelState);
            }

            try
            {
                return StatusCode((int)HttpStatusCode.OK, await _monitorNormativoService.ObterArquivoNormativo(id));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ResponseDto { status = (int)HttpStatusCode.BadRequest, isvalid = false, message = ex.Message });
            }
        }
    }
}
