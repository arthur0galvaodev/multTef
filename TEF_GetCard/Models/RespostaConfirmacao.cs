namespace MultiTef.TEF_GetPay.Models
{
    public class RespostaConfirmacao
    {
        public string? Comando { get; set; }
        //000‐000 = CRT
        public string? Identificacao { get; set; }
        //001‐000 = 34430576
        public string? DocumentoFiscal { get; set; }
        //002‐000 = 223546
        public string? RedeAdquirente { get; set; }
        //010‐000 = NOVAREDE
        public string? CodigoControle { get; set; }
        //027‐000 = 11011719100219100205783
        public string? VersaoInterface { get; set; }
        //733‐000 = 
        public string? NomeAutomacao { get; set; }
        //735‐000 = 
        public string? VersaoAutomacao { get; set; }
        //736‐000 = 
        public string? RegistroCertificacao { get; set; }
        //738‐000 = 
        public string? RegistroFinalizador { get; set; }
        //999‐999 = 0
    }
}
