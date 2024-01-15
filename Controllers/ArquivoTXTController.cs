using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiTef.Models;
using MultiTef.Library;
using MultiTef.TEF_GetCard;

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

            GetCardPassivo get = new GetCardPassivo();
            get.LerArquivoRetorno(arquivo.CaminhoArquivo);
            return Ok("");
        }
        [HttpPost("VerificarArquivoTxt")]
        public IActionResult VerificarArquivoTxt()
        {
            string pasta = "C:/Tef_Dial/Resp";
            string nomeArquivo = "intpos.001";

            bool arquivoExiste = VerificarArquivo(pasta, nomeArquivo, TimeSpan.FromMinutes(1));

            if (arquivoExiste)
            {
                return Ok("O arquivo existe!");
            }
            else
            {
                return NotFound("O arquivo não foi encontrado dentro do tempo especificado.");
            }
        }

        static bool VerificarArquivo(string pasta, string nomeArquivo, TimeSpan tempoLimite)
        {
            DateTime tempoInicio = DateTime.Now;

            while (DateTime.Now - tempoInicio < tempoLimite)
            {
                string caminhoCompleto = Path.Combine(pasta, nomeArquivo);

                if (System.IO.File.Exists(caminhoCompleto))
                {
                    // Arquivo encontrado
                    return true;
                }

                // Aguardar um curto período antes de verificar novamente
                Thread.Sleep(1000); // Aguarda 1 segundo
            }

            // Tempo limite atingido, o arquivo não foi encontrado
            return false;
        }
    }
}
