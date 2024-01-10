using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiTef.Library;
using MultiTef.Models;
using MultiTef.TEF_GetPay;

namespace MultiTef.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TEFController : ControllerBase
    {
        [HttpPost]
        public IActionResult NovaVenda(ModeloPagamento pagamento)
        {
            if(pagamento.TefHouse == TipoTefHouse.GetPay)
            {
                GetPay getPay = new GetPay();
                //Validar os Dados do Modelo de Pagamento.
                var result = getPay.RealizarVenda(pagamento);
                return Ok(result);
            }
            else
            {
                return Ok("TefHouse informada e incoreta!");
            }
            
        }
        //[HttpPost]
        //public IActionResult CancelarVenda(ArquivoTexto arquivo)
        //{
        //    return Ok(result);
        //}
        //[HttpPost]
        //public IActionResult ServicoADM(ArquivoTexto arquivo)
        //{
        //    return Ok(result);
        //}
    }
}
