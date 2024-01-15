using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiTef.Library;
using MultiTef.Models;
using MultiTef.TEF_GetCard;
using MultiTef.TEF_GetPay;

namespace MultiTef.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TEFController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MultiTef.Models.Pagamento))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult NovaVenda(ModeloPagamento pagamento)
        {

            if(pagamento.TefHouse == TipoTefHouse.ElginPassivo)
            {
                return Ok("");
            }
            else if (pagamento.TefHouse == TipoTefHouse.ElginAtivo)
            {
                return Ok("");
            }
            else if(pagamento.TefHouse == TipoTefHouse.GetCardPassivo)
            {
                GetCardPassivo getPay = new GetCardPassivo();
                var result = getPay.RealizarVenda(pagamento);
                return Ok(result);
            }
            else if (pagamento.TefHouse == TipoTefHouse.GetCardAtivo)
            {
                return Ok("");
            }
            else if (pagamento.TefHouse == TipoTefHouse.PayGo)
            {
                return Ok("");
            }
            else if (pagamento.TefHouse == TipoTefHouse.Auttar)
            {
                return Ok("");
            }
            else
            {
                return Ok("TefHouse informada e incoreta!");
            }
            
        }
    }
}
