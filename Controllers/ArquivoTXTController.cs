using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiTef.Models;
using MultiTef.Library;
using MultiTef.TEF_GetPay;

namespace MultiTef.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArquivoTXTController : ControllerBase
    {
        [HttpPost("CriarArquivoTxt")]
        public IActionResult CriarArquivoTxt(ArquivoTexto arquivo)
        {
            ArquivoTXT Arquivo = new ArquivoTXT();
            var result = Arquivo.CriarArquivo(arquivo);
            return Ok(result);
        }
        [HttpPost("LerArquivoTxt")]
        public IActionResult LerArquivoTxt(ArquivoTexto arquivo)
        {
            //ArquivoTXT Arquivo = new ArquivoTXT();
            //var result = Arquivo.LerArquivo(arquivo);

            GetPay get = new GetPay();
            get.LerArquivoRetorno(arquivo.CaminhoArquivo);
            return Ok("");
        }
    }
}
