using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Runtime.Intrinsics.Arm;

namespace MultiTef.Models
{
    public class ModeloPagamento
    {
        //Dados da operação
        public TipoTefHouse TefHouse { get; set; }
        public TipoTransacaoTef TipoTransacaoTef { get; set; }
        public TipoPagamento? TipoPagamento { get; set; }
        public int? NumeroDoc { get; set; } //Identificação
        public int? DocumentoFiscal { get; set; }
        public int? ValorTotal { get; set; }
        public int? TipoParcelamento { get; set; }
        public int? QtdParcelas { get; set; }
        //Dados da SoftHouse
        public string? NomeSoftHouse {  get; set; }
        public string? NomeDaAutomacao { get; set; }
        public string? VersaoDeAutomacao { get; set; }
        public string? RegistroCertificacao { get; set; }
        //Dados de arquivo quando usando metodo passivo
        public string? CaminhoArquivo { get; set; }
        public string? NomeArquivo { get; set; }
        public string? ExtensaoArquivo { get; set; }
        //Outros
        private int? EntidadeCliente { get; set; }
        private int? IdentificadorCliente { get; set; }
        private int? Status { get; set; }
        private string? RedeAdquirente { get; set; }
        private int? NSU { get; set; }
        private int? CodigoAutorizacao { get; set; }
        private DateTime? DataNoComprovante { get; set; }
        private DateTime? DataPreDatado { get; set; }
        private int? NSUOriginal { get; set; }
        private string? CodigoControle { get; set; }
        private string? MensagemOperador { get; set; }
        private string? NomeCartaoOuAdm { get; set; }

    }
    public enum TipoTefHouse
    {
        [Display(Name = "Elgin", Description = "Elgin")] Elgin = 1,
        [Display(Name = "GetPay", Description = "GetPay")] GetPay = 2,
        [Display(Name = "Paygo", Description = "Paygo")] Paygo = 3,
        [Display(Name = "Sitef", Description = "Sitef")] Sitef = 4,
        [Display(Name = "Scoope", Description = "Scoope")] Scoope = 5,
    }
    public enum TipoTransacaoTef
    {
        [Display(Name = "Cartão de crédito – à vista", Description = "Cartão de crédito – à vista")] CartaoDeCreditoAVista = 1,
        [Display(Name = "Cartão de crédito – parcelado pelo Estabelecimento", Description = "Cartão de crédito – parcelado pelo Estabelecimento")] CartaoDeCreditoParceladoPeloEstabelecimento = 2,
        [Display(Name = "Cartão de crédito – parcelado pelo Emissor", Description = "Cartão de crédito – parcelado pelo Emissor")] CartaoDeCreditoParceladoPeloEmissor = 3,
        [Display(Name = "Cartão de débito – à vista", Description = "Cartão de débito – à vista")] CartaoDeDebitoAVista = 4,
        [Display(Name = "Cartão de débito – parcelado pelo Estabelecimento", Description = "Cartão de débito – parcelado pelo Estabelecimento")] CartaoDeDebitoParceladoPeloEstabelecimento = 5,
        [Display(Name = "Cartão de débito – pré‐datado", Description = "Cartão de débito – pré‐datado")] CartaoDeDebitoPreDatado = 6,
        [Display(Name = "Cartão de débito – pré‐datado forçada", Description = "Cartão de débito – pré‐datado forçada")] CartaoDeDebitoPreDatadoForcada = 7,
        [Display(Name = "PIX", Description = "PIX")] PIX = 8,
        [Display(Name = "CDC / débito parcelado pelo Emissor", Description = "CDC / débito parcelado pelo Emissor")] CDC_DebitoParceladoPeloEmissor = 9,
        [Display(Name = "Voucher / PAT", Description = "Voucher / PAT")] Voucher_PAT = 10,
        [Display(Name = "Outro tipo de cartão", Description = "Outro tipo de cartão")] OutroTipoDeCartao = 11,
        [Display(Name = "Não definido (a operação não foi concluída)", Description = "Não definido (a operação não foi concluída)")] NaoDefinido_AOperacaoNaoFoiConcluida = 12,
        [Display(Name = "Pré‐autorização com cartão de crédito", Description = "Pré‐autorização com cartão de crédito")] PreAutorizacaoComCartaoDeCredito = 13,
        [Display(Name = "Consulta CDC / débito parcelado pelo Emissor", Description = "Consulta CDC / débito parcelado pelo Emissor")] ConsultaCDC_DebitoParceladoPeloEmissor = 14,
        [Display(Name = "Consulta de cheque", Description = "Consulta de cheque")] ConsultaDeCheque = 15,
        [Display(Name = "Garantia de cheque", Description = "Garantia de cheque")] GarantiaDeCheque = 16,
        [Display(Name = "Fechamento / Finalização", Description = "Fechamento / Finalização")] Fechamento_Finalização = 17,
        [Display(Name = "Outra operação administrativa", Description = "Outra operação administrativa")] OutraOperacaoAdministrativa = 18,

    }
    public enum TipoPagamento
{
        [Display(Name = "Credito", Description = "Credito")] Credito = 1,
        [Display(Name = "Debito", Description = "Debito")] Debito = 2,
        [Display(Name = "Voucher", Description = "Voucher")] Voucher = 3,
        [Display(Name = "PIX", Description = "PIX")] PIX = 4,
        [Display(Name = "Outro", Description = "Outro")] Outro = 5,
    }
}
