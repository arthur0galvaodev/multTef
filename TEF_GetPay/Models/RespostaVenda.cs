namespace MultiTef.TEF_GetPay.Models
{
    public class RespostaVenda
    {
        public string Comando { get; set; }
        //000‐000 = CRT
        public string Identificacao { get; set; }
        //001‐000 = 34430576
        public string DocumentoFiscal { get; set; }
        //002‐000 = 223546
        public string ValorTotal { get; set; }
        //003‐000 = 12000
        public string Moeda { get; set; }
        //004‐000 = 0
        public string Status { get; set; }
        //009‐000 = 0
        public string RedeAdquirente { get; set; }
        //010‐000 = NOVAREDE
        public string TipoTransacao { get; set; }
        //011‐000 = 20
        public string NSU { get; set; }
        //012‐000 = 19100205783
        public string CodigoAutorizacao { get; set; }
        //013‐000 = 022167
        public string DataComprovante { get; set; }
        //022‐000 = 17012011
        public string HoraComprovante { get; set; }
        //023‐000 = 191002
        public string CodigoControle { get; set; }
        //027‐000 = 11011719100219100205783
        public string MensagemOperador { get; set; } 
        //030‐000 = AUTORIZADA 022167
        public string NomeCartãoAdm { get; set; }
        //040‐000 = DEMOCARD
        public string ValorOriginal { get; set; } 
        //707‐000 = 10000
        public string ValorTroco { get; set; }
        //708‐000 = 2000             
        public string NumeroLogicoTerminal { get; set; }
        //718‐000 = 03876463
        public string CodigoEstabelecimento { get; set; }
        //719‐000 = 823982346832235
        public string StatusConfirmacao { get; set; }
        //729‐000 = 2
        public string Operacao { get; set; }
        //730‐000 = 1
        public string TipoDeCartao { get; set; }
        //731‐000 = 2
        public string TipoDeFinanciamento { get; set; }
        //732‐000 = 1
        
        public string RegistroFinalizador { get; set; }
        //999‐999 = 0
    }
}

////TODO: Verificar 
//public string Comando { get; set; }
//Tamanho via única
//        //028‐000 = 18
//#region Via única do comprovante
//        public string Comando { get; set; }
//Via única do comprovante
//        //029‐001 = " *** DEMONSTRACAO GERENCIADOR PADRÃO ***"
//        public string Comando { get; set; }
////029‐002 = " COMPROVANTE DE TEF"
//public string Comando { get; set; }
////029‐003 = " "
//public string Comando { get; set; }
////029‐004 = " ESTABELECIMENTO DE TESTE"
//public string Comando { get; set; }
////029‐005 = " 823982346832235/03876463"
//public string Comando { get; set; }
////029‐006 = " "
//public string Comando { get; set; }
////029‐007 = " 17/01/2011 19:10:02"
//public string Comando { get; set; }
////029‐008 = " REF.FISCAL:223546"
//public string Comando { get; set; }
////029‐009 = " DOC:026982 AUTORIZ:022167"
//public string Comando { get; set; }
////029‐010 = " REF.HOST:19100205783"
//public string Comando { get; set; }
////029‐011 = " "
//public string Comando { get; set; }
////029‐012 = " DEMOCARD ************1111"
//public string Comando { get; set; }
////029‐013 = " VENDA DEBITO A VISTA"
//public string Comando { get; set; }
////029‐014 = " VALOR FINAL: R$ 120,00"
//public string Comando { get; set; }
////029‐015 = " SAQUE: R$ 20,00"
//public string Comando { get; set; }
////029‐016 = " "
//public string Comando { get; set; }
////029‐017 = " TRANSACAO AUTORIZADA MEDIANTE"
//public string Comando { get; set; }
////029‐018 = " USO DA SENHA PESSOAL."
//#endregion Via única do comprovante
//public string Comando { get; set; }
//Tamanho Cupom Reduzido
//        //710‐000 = 4
//#region Cupom reduzido
//        public string Comando { get; set; }
////711‐001 = "DEMOCARD ************1111"
//public string Comando { get; set; }
////711‐002 = "POS:03876463 DOC:026982 AUTORIZ:022167"
//public string Comando { get; set; }
////711‐003 = "VENDA DEBITO A VISTA"
//public string Comando { get; set; }
////711‐004 = "VALOR FINAL: R$ 120,00"
//#endregion Cupom reduzido
//public string Comando { get; set; }
//Tamanho Via Cliente
//        //712‐000 = 16
//#region Via Comprovante do Cliente
//        public string Comando { get; set; }
////713‐001 = " *** DEMONSTRACAO GERENCIADOR PADRÃO ***"
//public string Comando { get; set; }
////713‐002 = " COMPROVANTE DE TEF"
//public string Comando { get; set; }
////713‐003 = " VIA: CLIENTE"
//public string Comando { get; set; }
////713‐004 = " "
//public string Comando { get; set; }
////713‐005 = " ESTABELECIMENTO DE TESTE"
//public string Comando { get; set; }
////713‐006 = " 823982346832235/03876463"
//public string Comando { get; set; }
////713‐007 = " "
//public string Comando { get; set; }
////713‐008 = " 17/01/2011 19:10:02"
//public string Comando { get; set; }
////713‐009 = " REF.FISCAL:34430576"
//public string Comando { get; set; }
////713‐010 = " DOC:026982 AUTORIZ:022167"
//public string Comando { get; set; }
////713‐011 = " REF.HOST:19100205783"
//public string Comando { get; set; }
////713‐012 = " "
//public string Comando { get; set; }
////713‐013 = " DEMOCARD ************1111"
//public string Comando { get; set; }
////713‐014 = " VENDA DEBITO A VISTA"
//public string Comando { get; set; }
////713‐015 = " VALOR FINAL: R$ 120,00"
//public string Comando { get; set; }
////713‐016 = " SAQUE: R$ 20,00"
//#endregion Via Comprovante do Cliente
//public string Comando { get; set; }
//Tamanho via Estabelecimento
//        //714‐000 = 19
//#region Via Estabelecimento do comprovante
//        public string Comando { get; set; }
////715‐001 = " *** DEMONSTRACAO GERENCIADOR PADRÃO ***"
//public string Comando { get; set; }
////715‐002 = " COMPROVANTE DE TEF"
//public string Comando { get; set; }
////715‐003 = " VIA: ESTABELECIMENTO"
//public string Comando { get; set; }
////715‐004 = " "
//public string Comando { get; set; }
////715‐005 = " ESTABELECIMENTO DE TESTE"
//public string Comando { get; set; }
////715‐006 = " 823982346832235/03876463"
//public string Comando { get; set; }
////715‐007 = " "
//public string Comando { get; set; }
////715‐008 = " 17/01/2011 19:10:02"
//public string Comando { get; set; }
////715‐009 = " REF.FISCAL:34430576"
//public string Comando { get; set; }
////715‐010 = " DOC:026982 AUTORIZ:022167"
//public string Comando { get; set; }
////715‐011 = " REF.HOST:19100205783"
//public string Comando { get; set; }
////715‐012 = " "
//public string Comando { get; set; }
////715‐013 = " DEMOCARD ************1111"
//public string Comando { get; set; }
////715‐014 = " VENDA DEBITO A VISTA"
//public string Comando { get; set; }
////715‐015 = " VALOR FINAL: R$ 120,00"
//public string Comando { get; set; }
////715‐016 = " SAQUE: R$ 20,00"
//public string Comando { get; set; }
////715‐017 = " "
//public string Comando { get; set; }
////715‐018 = " TRANSACAO AUTORIZADA MEDIANTE"
//public string Comando { get; set; }
////715‐019 = " USO DA SENHA PESSOAL."
//#endregion Via Estabelecimento do comprovante
//public string Comando { get; set; }
//Vias do comprovante
//        //737‐000 = 3