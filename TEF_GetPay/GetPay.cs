using Microsoft.AspNetCore.Mvc;
using MultiTef.Library;
using MultiTef.Models;
using MultiTef.TEF_GetPay.Models;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Cryptography.Xml;
using System.Collections.Generic;
using System.Linq;

namespace MultiTef.TEF_GetPay
{
    public class GetPay
    {
        //Anotações 

        // Comando  - Status    - Resposta  - Descrção
        // ATV      - USA       -           - Verifica se o GP está ativo
        // CRT      - USA       - OK        - Realiza uma transação de venda
        // ADM      - USA       - OK        - Realiza uma transação administrativa
        // CNC      - USA       - OK        - Realiza uma transação de cancelamento
        // CNF      - USA       -           - Confirma a última Transação realizada
        // NCN      - USA       -           - Desfaz a última transação realizada
        StatusVenda statusVenda = new StatusVenda();
        RespostaVenda respostaVenda = new RespostaVenda();

        public JsonResult RealizarVenda(ModeloPagamento pagamento)
        {
            //Realizar Venda
            //1º Criar conteudo do arquivo e Criar arquivo na pasta
            bool ArquivoIniciar = CriarArvivoComunicacao(pagamento);
            //2º Ler arquivo de retorno e Validar retorno
            //3º Ler arquivo de Resposta
            if (ArquivoIniciar)
            {                
                bool ArquivoRetorno = LerArquivoRetorno(pagamento.CaminhoArquivo);
                if (ArquivoRetorno)
                {
                    //4º Confirmar Pagamento
                    if (ValidarRespota(pagamento) == "ok")
                    {
                        //5º Criar Arquivo de Confirmação de Venda
                        //6º Ler Arquivo de Status de Venda
                        ConfirmarPagamento(pagamento);
                    }
                    else
                    {
                        //TODO: Implementar Erro
                        return new JsonResult("");
                    }
                }
            }else {
                //TODO: Implementar Erro
                return new JsonResult("");
            }
            
            
            
            //7º Retonar dados da venda para o sistema

            return new JsonResult ("");
        }

        private bool CriarArvivoComunicacao(ModeloPagamento pagamento)
        {
            try
            {
                
                int operacaoTef = VarificarOperacaoTef(pagamento.TipoTransacaoTef);

                if (operacaoTef > 0)
                {
                    var conteudoArquivo = CriarConteudoSolicitacaoPagamento((int)pagamento.NumeroDoc,
                                                                                (int)pagamento.DocumentoFiscal,
                                                                                operacaoTef,
                                                                                (int)pagamento.ValorTotal,
                                                                                (int)pagamento.TipoParcelamento,
                                                                                (int)pagamento.QtdParcelas,
                                                                                pagamento.NomeSoftHouse,
                                                                                pagamento.NomeDaAutomacao,
                                                                                pagamento.VersaoDeAutomacao,
                                                                                pagamento.RegistroCertificacao);
                    ArquivoTexto arquivo = new ArquivoTexto
                    {
                        NomeArquivo = pagamento.NomeArquivo,
                        CaminhoArquivo = pagamento.CaminhoArquivo + "/Req",
                        ExtensaoArquivo = pagamento.ExtensaoArquivo,
                        Conteudo = conteudoArquivo
                    };
                    ArquivoTXT Arquivo = new ArquivoTXT();
                    var result = Arquivo.CriarArquivo(arquivo);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar o arquivo: {ex.Message}");
                return false;
            }

        }
        
        public bool LerArquivoRetorno(string caminhoArquivo)
        {
            try
            {
                //Ler arquivo status de venda
                ArquivoTexto ArquivoStatusVenda = new ArquivoTexto
                {
                    NomeArquivo = "intpos",
                    CaminhoArquivo = caminhoArquivo + "/Resp",
                    ExtensaoArquivo = "sts",
                };
                ArquivoTXT arquivo = new ArquivoTXT();
                ArquivoStatusVenda.Conteudo = arquivo.LerArquivo(ArquivoStatusVenda);
                ArquivoTexto ArquivoRespostaVenda = new ArquivoTexto
                {
                    NomeArquivo = "intpos",
                    CaminhoArquivo = caminhoArquivo + "/Resp",
                    ExtensaoArquivo = "001",
                };
                ArquivoRespostaVenda.Conteudo = arquivo.LerArquivo(ArquivoRespostaVenda);

                if (ConverterRespostaVenda(ArquivoStatusVenda, ArquivoRespostaVenda))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex) { 
                return false;
            }
        }
        private int VarificarOperacaoTef(TipoTransacaoTef tipoTransacaoTef)
        {
            //Venda ou cancelamento (de acordo com o campo 000‐000):
            int operacao = 0;
            if (tipoTransacaoTef == TipoTransacaoTef.CartaoDeCreditoAVista)
            {
                //10: Cartão de crédito – à vista
                return operacao = 10;
            }
            else if (tipoTransacaoTef == TipoTransacaoTef.CartaoDeCreditoParceladoPeloEstabelecimento)
            {
                //11: Cartão de crédito – parcelado pelo Estabelecimento
                return operacao = 11;
            }
            else if (tipoTransacaoTef == TipoTransacaoTef.CartaoDeCreditoParceladoPeloEmissor)
            {
                //12: Cartão de crédito – parcelado pelo Emissor
                return operacao = 12;
            }
            else if (tipoTransacaoTef == TipoTransacaoTef.CartaoDeDebitoAVista)
            {
                //20: Cartão de débito – à vista
                return operacao = 20;
            }
            else if (tipoTransacaoTef == TipoTransacaoTef.CartaoDeDebitoParceladoPeloEstabelecimento)
            {
                //22: Cartão de débito – parcelado pelo Estabelecimento
                return operacao = 22;
            }
            else if (tipoTransacaoTef == TipoTransacaoTef.CartaoDeDebitoPreDatado)
            {
                //21: Cartão de débito – pré‐datado
                return operacao = 21;
            }
            else if (tipoTransacaoTef == TipoTransacaoTef.CartaoDeDebitoPreDatadoForcada)
            {
                //24: Cartão de débito – pré‐datado forçada
                return operacao = 24;
            }
            else if (tipoTransacaoTef == TipoTransacaoTef.CDC_DebitoParceladoPeloEmissor)
            {
                //40: CDC / débito parcelado pelo Emissor
                return operacao = 40;
            }
            else if (tipoTransacaoTef == TipoTransacaoTef.Voucher_PAT)
            {
                //60: Voucher / PAT
                return operacao = 60;
            }
            else if (tipoTransacaoTef == TipoTransacaoTef.OutroTipoDeCartao)
            {
                //30: Outro tipo de cartão
                return operacao = 30;
            }
            else if (tipoTransacaoTef == TipoTransacaoTef.PIX)
            {
                //Verificar Pix
                //Verificar implementacao pela documentacao
                return operacao;
            }
            else
            {
                return operacao;
            }
        }

        private string CriarConteudoSolicitacaoPagamento(   int indentificacao, 
                                                            int documentoFiscal,
                                                            int operacaoTef,
                                                            int valorTotal,
                                                            int parcelamentoEstabelecimento,
                                                            int quantidadeParcela,
                                                            string nomeSoftHouse,
                                                            string NomeDaAutomacao,
                                                            string VersaoDeAutomacao,
                                                            string RegistroCertificacao)
        {
            string conteudo;
            DateTime dateTime = DateTime.Now;
            string data = "";
            Version versao = Assembly.GetExecutingAssembly().GetName().Version;
            string versaoSistema = versao.ToString();

            conteudo = "000-000 = CRT\r" +            //Comando - 000-000 = CRT - Realiza uma transação de venda

            "001-000 = " + indentificacao + "\r" +    //Número de controle gerado pela Automação Comercial,
                                                      //devendo o valor ser diferente para cada nova operação de
                                                      //TEF.É ecoado pelo Gerenciador Padrão nos arquivos de
                                                      //status e de resposta, e deve ser consistido pelo Automação
                                                      //Comercial.

            "002-000 = " + documentoFiscal + "\r" +   //Número do documento fiscal ao qual a operação de TEF está
                                                      //vinculada.Caso seja usada uma Impressora Fiscal, o
                                                      //preenchimento deste campo é obrigatório para transações de venda.

            "003-000 = " + valorTotal + "\r" +        //Valor total da operação, em centavos da moeda informada
                                                      //no campo 004‐000, incluindo todas as taxas cobradas do
                                                      //Cliente(serviço, embarque, etc.).
                                                      //No arquivo de resposta para transações de venda, este
                                                      //campo indica o valor efetivamente debitado do Cliente e
                                                      //creditado para o Estabelecimento(desconsiderando taxas
                                                      //referentes ao uso da solução, descontadas pela Rede
                                                      //Adquirente).

            "004-000 = " + "0" + "\r" +              //0: Real
                                                     //1: Dólar americano

            "011-000 = " + operacaoTef + "\r";       //Venda ou cancelamento (de acordo com o campo 000‐000):
                                                     //10: Cartão de crédito – à vista
                                                     //11: Cartão de crédito – parcelado pelo Estabelecimento
                                                     //12: Cartão de crédito – parcelado pelo Emissor
                                                     //20: Cartão de débito – à vista
                                                     //22: Cartão de débito – parcelado pelo Estabelecimento
                                                     //21: Cartão de débito – pré‐datado
                                                     //24: Cartão de débito – pré‐datado forçada
                                                     //40: CDC / débito parcelado pelo Emissor
                                                     //60: Voucher / PAT
                                                     //30: Outro tipo de cartão
                                                     //99: Não definido(a operação não foi concluída)

            if (operacaoTef == 11 || 
                operacaoTef == 12 || 
                operacaoTef == 20 || 
                operacaoTef == 22 || 
                operacaoTef == 21 || 
                operacaoTef == 24 || 
                operacaoTef == 40) { //As operações que tem divisão de pagamento deve usar o codigo 017 informar que poga o parcelamento e 018 que quantidade

                conteudo +=
                "017 - 000 = " + parcelamentoEstabelecimento + "\r" + //0: parcelado pelo Estabelecimento; 1: parcelado pelo Emissor.

                "018-000 = " + quantidadeParcela + "\r";            //Quantidade de parcelas, para transações parceladas.
            }

            if (RegistroCertificacao != null)
            {
                conteudo +=
                    "706-000 = " + "3" + "\r" +                 //Soma dos seguintes valores, identificando as
                                                                //funcionalidades suportadas pela Automação Comercial:
                                                                //1: funcionalidade de troco(ver campo 708‐000)
                                                                //2: funcionalidade de desconto(ver campo 709‐000) 
                                                                //3: solicitacao de venda
                                                                //4: valor fixo, sempre incluir 
                                                                //8: vias diferenciadas do comprovante para Cliente / Estabelecimento(campos 712‐000 a 715‐000)
                                                                //16: cupom reduzido(campos 710‐000 e 711‐000)
                                                                //Caso este campo não seja informado pela Automação
                                                                //Comercial(versões anteriores), considera‐se que nenhuma
                                                                //das funcionalidades é suportada.
                                                                //Importante: na certificação da CIELO, é exigido que a
                                                                //Automação Comercial implemente a funcionalidade de
                                                                //desconto.


                    "716-000 = " + nomeSoftHouse + "\r" +        //Razão social da empresa responsável pelo desenvolvimento
                                                                 //da aplicação de Automação Comercial.
                                                                 //Exemplo: KND SISTEMAS LTDA.

                    "717-000 = " + data + "\r" +                 //Data / hora registrada no cupom fiscal, no formato AAMMDDhhmmss
                                                                 //Caso seja usada uma Impressora Fiscal, o preenchimento
                                                                 //deste campo é obrigatório para transações de venda.

                    "733-000 = " + versaoSistema + "\r" +        //Valor fixo, identificando a versão deste documento
                                                                 //implementada pela Automação Comercial(somente
                                                                 //números, por exemplo, 210 para “v2.10”).

                    "735-000 = " + NomeDaAutomacao + "\r" +       //Nome da aplicação de Automação Comercial.
                    "736-000 = " + VersaoDeAutomacao + "\r" +     //Versão da aplicação de Automação Comercial, conforme
                                                                  //nomenclatura utilizada pelo desenvolvedor.
                    "738-000 = " + RegistroCertificacao + "\r";   //Registro de Certificação
            }
            conteudo += "999-999 = 0\r";                           //Conteudo fixo: 0 (Zero)
            return conteudo;
        }

        private bool ConverterRespostaVenda(ArquivoTexto ArquivoStatusVenda, ArquivoTexto ArquivoRespostaVenda)
        {
            try
            {   
                // Dividir a string em linhas
                string[] statusLinhas = ArquivoStatusVenda.Conteudo.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var linha in statusLinhas)
                {
                    // Dividir cada linha em partes usando o sinal de igual como delimitador
                    string[] partes = linha.Split('=');

                    if (partes.Length == 2)
                    {
                        switch (partes[0].Trim())
                        {
                            case "000-000":
                                statusVenda.Comando = partes[1].Trim();
                                break;
                            case "001-000":
                                statusVenda.Identificacao = partes[1].Trim();
                                break;
                            case "999-999":
                                statusVenda.RegistroFinalizador = partes[1].Trim();
                                break;
                        }
                    }
                }
                // Dividir a string em linhas
                string[] respostaLinhas = ArquivoStatusVenda.Conteudo.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var linha in respostaLinhas)
                {
                    // Dividir cada linha em partes usando o sinal de igual como delimitador
                    string[] partes = linha.Split('=');

                    if (partes.Length == 2)
                    {
                        switch (partes[0].Trim())
                        {
                            case "000-000":
                                respostaVenda.Comando = partes[1].Trim();
                                break;
                            case "001-000":
                                respostaVenda.Identificacao = partes[1].Trim();
                                break;
                            case "002-000":
                                respostaVenda.DocumentoFiscal = partes[1].Trim();
                                break;
                            case "003-000":
                                respostaVenda.ValorTotal = partes[1].Trim();
                                break;
                            case "004-000":
                                respostaVenda.Moeda = partes[1].Trim();
                                break;
                            case "009-000":
                                respostaVenda.Status = partes[1].Trim();
                                break;
                            case "010-000":
                                respostaVenda.RedeAdquirente = partes[1].Trim();
                                break;
                            case "011-000":
                                respostaVenda.TipoTransacao = partes[1].Trim();
                                break;
                            case "012-000":
                                respostaVenda.NSU = partes[1].Trim();
                                break;
                            case "013-000":
                                respostaVenda.CodigoAutorizacao = partes[1].Trim();
                                break;
                            case "022-000":
                                respostaVenda.DataComprovante = partes[1].Trim();
                                break;
                            case "023-000":
                                respostaVenda.HoraComprovante = partes[1].Trim();
                                break;
                            case "027-000":
                                respostaVenda.CodigoControle = partes[1].Trim();
                                break;
                            case "030-000":
                                respostaVenda.MensagemOperador = partes[1].Trim();
                                break;
                            case "040-000":
                                respostaVenda.NomeCartãoAdm = partes[1].Trim();
                                break;
                            case "707-000":
                                respostaVenda.ValorOriginal = partes[1].Trim();
                                break;
                            case "708-000":
                                respostaVenda.ValorTroco = partes[1].Trim();
                                break;
                            case "718-000":
                                respostaVenda.NumeroLogicoTerminal = partes[1].Trim();
                                break;
                            case "719-000":
                                respostaVenda.CodigoEstabelecimento = partes[1].Trim();
                                break;
                            case "729-000":
                                respostaVenda.StatusConfirmacao = partes[1].Trim();
                                break;
                            case "730-000":
                                respostaVenda.Operacao = partes[1].Trim();
                                break;
                            case "731-000":
                                respostaVenda.TipoDeCartao = partes[1].Trim();
                                break;
                            case "732-000":
                                respostaVenda.TipoDeFinanciamento = partes[1].Trim();
                                break;
                            case "999-999":
                                respostaVenda.RegistroFinalizador = partes[1].Trim();
                                break;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string ValidarRespota(ModeloPagamento pagamento)
        {
            //Verificar status da resposta bate com o valor Identificação Com o Numero do Documento
            int identificacaoStatus = Convert.ToInt32(statusVenda.Identificacao);
            int identificacaoResposta = Convert.ToInt32(respostaVenda.Identificacao);
            if (pagamento.NumeroDoc == identificacaoStatus && identificacaoResposta == identificacaoStatus)
            {
                //TODO: Implementar verificação:
                //      Pelo Valor
                //      Mensagem do Operador
                //      Quantidade de parcela
                //      Tipo de Operacao

                return "ok";
            }
            else
            {
                //TODO Implementar
                return "erro";
            }
        }

        private void ConfirmarPagamento(ModeloPagamento pagamento)
        {
            //Cria o Arquivo de confirmação de pagamento
            string conteudo = "000‐000 = CNF \n" +
                                "001‐000 = " + "\n" +
                                "002‐000 = " + "\n" +
                                "010‐000 = " + "\n" +
                                "027‐000 = " + "\n" +
                                "733‐000 = " + "\n" +
                                "735‐000 = " + "\n" +
                                "736‐000 = " + "\n" +
                                "738‐000 = " + "\n" +
                                "999‐999 = 0";
            ArquivoTexto arquivo = new ArquivoTexto
            {
                NomeArquivo = pagamento.NomeArquivo,
                CaminhoArquivo = pagamento.CaminhoArquivo + "/Req",
                ExtensaoArquivo = pagamento.ExtensaoArquivo,
                Conteudo = conteudo
            };
            ArquivoTXT Arquivo = new ArquivoTXT();
            var result = Arquivo.CriarArquivo(arquivo);
        }

        //Modelo de Solicitação de Venda
        //000‐000 = CRT
        //001‐000 = 34430576
        //002‐000 = 223546
        //003‐000 = 10000
        //004‐000 = 0
        //706‐000 = 3
        //716‐000 = Gerenciador Padrão.
        //717‐000 = 11011719053
        //733‐000 = 210
        //735‐000 = KiWi
        //736‐000 = v1, 14, 0, 0
        //738‐000 = G45J35G3JH45B435
        //999‐999 = 0

        //Status de venda(Resp\intpos.sts)
        //000‐000 = CRT
        //001‐000 = 34430576
        //999‐999 = 0

        //Resposta de venda(Resp\intpos.001)
        //000‐000 = CRT
        //001‐000 = 34430576
        //002‐000 = 223546
        //003‐000 = 12000
        //004‐000 = 0
        //009‐000 = 0
        //010‐000 = NOVAREDE
        //011‐000 = 20
        //012‐000 = 19100205783
        //013‐000 = 022167
        //015‐000 = 1701191002
        //016‐000 = 1701191002
        //022‐000 = 17012011
        //023‐000 = 191002
        //027‐000 = 11011719100219100205783
        //028‐000 = 18
        //029‐001 = " *** DEMONSTRACAO GERENCIADOR PADRÃO ***"
        //029‐002 = " COMPROVANTE DE TEF"
        //029‐003 = " "
        //029‐004 = " ESTABELECIMENTO DE TESTE"
        //029‐005 = " 823982346832235/03876463"
        //029‐006 = " "
        //029‐007 = " 17/01/2011 19:10:02"
        //029‐008 = " REF.FISCAL:223546"
        //029‐009 = " DOC:026982 AUTORIZ:022167"
        //029‐010 = " REF.HOST:19100205783"
        //029‐011 = " "
        //029‐012 = " DEMOCARD ************1111"
        //029‐013 = " VENDA DEBITO A VISTA"
        //029‐014 = " VALOR FINAL: R$ 120,00"
        //029‐015 = " SAQUE: R$ 20,00"
        //029‐016 = " "
        //029‐017 = " TRANSACAO AUTORIZADA MEDIANTE"
        //029‐018 = " USO DA SENHA PESSOAL."
        //030‐000 = AUTORIZADA 022167
        //040‐000 = DEMOCARD
        //707‐000 = 10000
        //708‐000 = 2000
        //710‐000 = 4
        //711‐001 = "DEMOCARD ************1111"
        //711‐002 = "POS:03876463 DOC:026982 AUTORIZ:022167"
        //711‐003 = "VENDA DEBITO A VISTA"
        //711‐004 = "VALOR FINAL: R$ 120,00"
        //712‐000 = 16
        //713‐001 = " *** DEMONSTRACAO GERENCIADOR PADRÃO ***"
        //713‐002 = " COMPROVANTE DE TEF"
        //713‐003 = " VIA: CLIENTE"
        //713‐004 = " "
        //713‐005 = " ESTABELECIMENTO DE TESTE"
        //713‐006 = " 823982346832235/03876463"
        //713‐007 = " "
        //713‐008 = " 17/01/2011 19:10:02"
        //713‐009 = " REF.FISCAL:34430576"
        //713‐010 = " DOC:026982 AUTORIZ:022167"
        //713‐011 = " REF.HOST:19100205783"
        //713‐012 = " "
        //713‐013 = " DEMOCARD ************1111"
        //713‐014 = " VENDA DEBITO A VISTA"
        //713‐015 = " VALOR FINAL: R$ 120,00"
        //713‐016 = " SAQUE: R$ 20,00"
        //714‐000 = 19
        //715‐001 = " *** DEMONSTRACAO GERENCIADOR PADRÃO ***"
        //715‐002 = " COMPROVANTE DE TEF"
        //715‐003 = " VIA: ESTABELECIMENTO"
        //715‐004 = " "
        //715‐005 = " ESTABELECIMENTO DE TESTE"
        //715‐006 = " 823982346832235/03876463"
        //715‐007 = " "
        //715‐008 = " 17/01/2011 19:10:02"
        //715‐009 = " REF.FISCAL:34430576"
        //715‐010 = " DOC:026982 AUTORIZ:022167"
        //715‐011 = " REF.HOST:19100205783"
        //715‐012 = " "
        //715‐013 = " DEMOCARD ************1111"
        //715‐014 = " VENDA DEBITO A VISTA"
        //715‐015 = " VALOR FINAL: R$ 120,00"
        //715‐016 = " SAQUE: R$ 20,00"
        //715‐017 = " "
        //715‐018 = " TRANSACAO AUTORIZADA MEDIANTE"
        //715‐019 = " USO DA SENHA PESSOAL."
        //718‐000 = 03876463
        //719‐000 = 823982346832235
        //729‐000 = 2
        //730‐000 = 1
        //731‐000 = 2
        //732‐000 = 1
        //737‐000 = 3
        //999‐999 = 0

        //Confirmação de venda(Req\intpos.001)
        //000‐000 = CNF
        //001‐000 = 34430576
        //002‐000 = 223546
        //010‐000 = NOVAREDE
        //027‐000 = 11011719100219100205783
        //733‐000 = 210
        //735‐000 = KiWi
        //736‐000 = v1, 14, 0, 0
        //738‐000 = G45J35G3JH45B435
        //999‐999 = 0

        //Status de confirmação(Resp\intpos.sts)
        //000‐000 = CNF
        //001‐000 = 34430576
        //999‐999 = 0
    }
}
