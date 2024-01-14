namespace MultiTef.Models
{
    public class Pagamento
    {
        public string Comando { get; set; }
        //000‐000
        public string Identificacao { get; set; }
        //001‐000
        public string DocumentoFiscal { get; set; }
        //002‐000
        public string ValorTotal { get; set; }
        //003‐000
        public string Moeda { get; set; }
        //004‐000
        public string Status { get; set; }
        //009‐000
        public string RedeAdquirente { get; set; }
        //010‐000
        public string TipoTransacao { get; set; }
        //011‐000
        public string NSU { get; set; }
        //012‐000
        public string CodigoAutorizacao { get; set; }
        //013‐000
        public string DataComprovante { get; set; }
        //022‐000
        public string HoraComprovante { get; set; }
        //023‐000
        public string CodigoControle { get; set; }
        //027‐000
        public string MensagemOperador { get; set; }
        //030‐000
        public string NomeCartãoAdm { get; set; }
        //040‐000
        public string ValorOriginal { get; set; }
        //707‐000
        public string ValorTroco { get; set; }
        //708‐000           
        public string NumeroLogicoTerminal { get; set; }
        //718‐000
        public string CodigoEstabelecimento { get; set; }
        //719‐000
        public string StatusConfirmacao { get; set; }
        //729‐000
        public string Operacao { get; set; }
        //730‐000
        public string TipoDeCartao { get; set; }
        //731‐000
        public string TipoDeFinanciamento { get; set; }
        //732‐000
        public string BandeiraNCartao { get; set; }
        //715-013
        public string TipoDeAutorizacao1 { get; set; }
        //715-018
        public string TipoDeAutorizacao2 { get; set; }
        //715-019
        public string RegistroFinalizador { get; set; }
        //999‐999
        public string Erro {  get; set; }
        //0000 - Sem Erro
        //0001 - Erro ao criar o arquivo
        //0002 - Sem Resposta do arquivo de pagamento - Tempo limite atingido
        //0003 - Erro ao ler arquivo de resposta
        //0004 - Erro ao validar os dados de resposta
        //0005 - 
        //0006 - 
        //0007 - 
        //0008 - 


        //////Dados do comprovante de pagamento
        ///029-000 a 029-018
        ///
        /// Recibo a Imprimir               //ok - texto fixo
        /// Nome da via                     //ok - texto fixo
        /// Comprovante de pagamento        //ok - texto fixo
        /// Via Cliente/Estabelicimento     //ok - texto fixo
        /// quebra de linha*
        /// Nome do Estabelecimento         //ok - texto fixo
        /// Cnpj do Estabelecimento         //ok - texto fixo
        /// quebra de linha*
        /// DATA / HORA                     //ok
        /// DOC Fiscal                      //ok
        /// DOC (NSU) / Autorizacao         //ok
        /// quebra de linha*    
        /// numero cartão                   //ok
        /// Tipo Pagamento                  //ok
        /// Valor Total                     //ok
        /// Quantida de Parcela se tiver    //ok
        /// Tipo De Autorização 1           //ok
        /// Tipo De Autorização 2           //ok
        /// 
    }
}
