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
using System.Text.Json;

namespace MultiTef.TEF_GetCard
{
    public class GetCardPassivo
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
        Pagamento dadosPagamento = new Pagamento();
        StatusConfirmacao statusConfirmacao = new StatusConfirmacao();
        RespostaConfirmacao respostaConfirmacao =  new RespostaConfirmacao();
        string menssagemErro = "";

        public JsonResult RealizarVenda(ModeloPagamento pagamento)
        {
            //Realizar Venda
            //Para não haver erro de leitura de arquivo que ficou anterior deve excluir eles se existirem
            LimparArquivosExistentes(pagamento.CaminhoArquivo);
            
            //Inicia processo de venda;

            //1º Criar conteudo do arquivo e Criar arquivo na pasta
            bool ArquivoIniciar = CriarArvivoComunicacao(pagamento); //TODO: antes de criar um arquivo novo verificar se existe um anterior, se existir excluir ele antes
            //2º Ler arquivo de retorno e Validar retorno
            //3º Ler arquivo de Resposta
            if (ArquivoIniciar)
            {
                if (VerificarExisteArquivo(pagamento, 10))
                {
                    bool ArquivoRetorno = LerArquivoRetorno(pagamento.CaminhoArquivo);
                    if (ArquivoRetorno)
                    {
                        //4º Confirmar Pagamento
                        if (ValidarRespota(pagamento) == true) //TODO: qualquer erro daqui para frente tem que cancelar ou desfazer
                        {
                            //5º Criar Arquivo de Confirmação de Venda
                            //6º Ler Arquivo de Status de Venda
                            if (ConfirmarPagamento(pagamento))
                            {
                                //Ler Arquivo Confirmacao
                                if (VerificarExisteArquivoConfirmacao(pagamento, 3))
                                {
                                    if (LerArquivoConfirmacao(pagamento.CaminhoArquivo))
                                    {//Validar Resposta
                                        if (ValidarConfirmacao(pagamento) == true)
                                        {
                                            //7º Retonar dados da venda para o sistema
                                            dadosPagamento.Erro = "0000"; 
                                            return new JsonResult(JsonSerializer.Serialize(dadosPagamento));
                                        }
                                        else
                                        {
                                            dadosPagamento.MensagemOperador = menssagemErro;
                                            dadosPagamento.Erro = "0008";
                                            return new JsonResult(JsonSerializer.Serialize(dadosPagamento));
                                        }
                                    }
                                    else
                                    {
                                        dadosPagamento.MensagemOperador = menssagemErro;
                                        dadosPagamento.Erro = "0007";
                                        return new JsonResult(JsonSerializer.Serialize(dadosPagamento));
                                    }
                                }
                                else
                                {
                                    dadosPagamento.MensagemOperador = menssagemErro;
                                    dadosPagamento.Erro = "0006";
                                    return new JsonResult(JsonSerializer.Serialize(dadosPagamento));
                                }                                    
                            }
                            else
                            {
                                dadosPagamento.MensagemOperador = menssagemErro;
                                dadosPagamento.Erro = "0005";
                                return new JsonResult(JsonSerializer.Serialize(dadosPagamento));
                            }
                        }
                        else
                        {
                            dadosPagamento.MensagemOperador = menssagemErro;
                            dadosPagamento.Erro = "0004";
                            return new JsonResult(JsonSerializer.Serialize(dadosPagamento));
                        }
                    }
                    else
                    {
                        //Erro ao ler arquivo de resposta
                        dadosPagamento.MensagemOperador = menssagemErro;
                        dadosPagamento.Erro = "0003";
                        return new JsonResult(JsonSerializer.Serialize(dadosPagamento));
                    }
                }
                else
                {
                    //Sem Resposta do arquivo de pagamento - Tempo limite atingido
                    dadosPagamento.MensagemOperador = menssagemErro;
                    dadosPagamento.Erro = "0002";
                    return new JsonResult(JsonSerializer.Serialize(dadosPagamento));
                }

                
            }else {
                dadosPagamento.MensagemOperador = "Erro ao criar o arquivo, Verificar caminho ou autorização da pasta!";
                dadosPagamento.Erro = "0001";
                return new JsonResult(JsonSerializer.Serialize(dadosPagamento));
            }
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
                                                                                Convert.ToInt32((pagamento.TipoParcelamento ?? 0)),
                                                                                Convert.ToInt32((pagamento.QtdParcelas ?? 0)),
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
        private bool VerificarExisteArquivo(ModeloPagamento pagamento, int minutos)
        {
            try
            {
                string caminho = pagamento.CaminhoArquivo + "/Resp"; //caminho do arquivo de status e resposta
                string nomeArquivoSTS = "intpos.sts";//caminho do arquivo de status
                string nomeArquivo001 = "intpos.001";//caminho do arquivo de resposta
                TimeSpan tempoLimite = TimeSpan.FromMinutes(minutos);
                DateTime tempoInicio = DateTime.Now;
                while (DateTime.Now - tempoInicio < tempoLimite)
                {
                    string caminhoCompletoSTS = Path.Combine(caminho, nomeArquivoSTS);
                    string caminhoCompleto001 = Path.Combine(caminho, nomeArquivo001);
                    if (System.IO.File.Exists(caminhoCompletoSTS))
                    {
                        if (System.IO.File.Exists(caminhoCompleto001))
                        {
                            // Arquivos encontrados
                            return true;
                        }                            
                    }

                    // Aguardar um curto período antes de verificar novamente
                    Thread.Sleep(1000); // Aguarda 1 segundo
                }

                // Tempo limite atingido, o arquivo não foi encontrado
                menssagemErro = "Tempo limite atingido, resposta não foi encontrada!";
                return false;
            }
            catch (Exception ex)
            {
                menssagemErro = "Erro: Sem resposta " + ex.Message;
                return false;
            }
        }
        private bool VerificarExisteArquivoConfirmacao(ModeloPagamento pagamento, int minutos)
        {
            try
            {
                string caminho = pagamento.CaminhoArquivo + "/Resp"; //caminho do arquivo de status e resposta
                string nomeArquivoSTS = "intpos.sts";//caminho do arquivo de status
                TimeSpan tempoLimite = TimeSpan.FromMinutes(minutos);
                DateTime tempoInicio = DateTime.Now;
                while (DateTime.Now - tempoInicio < tempoLimite)
                {
                    string caminhoCompletoSTS = Path.Combine(caminho, nomeArquivoSTS);
                    if (System.IO.File.Exists(caminhoCompletoSTS))
                    {   // Arquivos encontrados
                        return true;
                    }

                    // Aguardar um curto período antes de verificar novamente
                    Thread.Sleep(1000); // Aguarda 1 segundo
                }

                // Tempo limite atingido, o arquivo não foi encontrado
                menssagemErro = "Tempo limite atingido, resposta não foi encontrada!";
                return false;
            }
            catch (Exception ex)
            {
                menssagemErro = "Erro: Sem resposta " + ex.Message;
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
                ArquivoTexto ArquivodadosPagamento = new ArquivoTexto
                {
                    NomeArquivo = "intpos",
                    CaminhoArquivo = caminhoArquivo + "/Resp",
                    ExtensaoArquivo = "001",
                };
                ArquivodadosPagamento.Conteudo = arquivo.LerArquivo(ArquivodadosPagamento);

                if (ConverterdadosPagamento(ArquivoStatusVenda, ArquivodadosPagamento))
                {
                    return true;
                }
                else
                {
                    menssagemErro = "Erro ao ler arquivo de resposta!";
                    return false;
                }
            }
            catch (Exception ex) {
                menssagemErro = "Erro ao ler arquivo de resposta!";
                return false;
            }
        }
        private bool ValidarRespota(ModeloPagamento pagamento)
        {
            try
            {
                //Verificar status da resposta bate com o valor Identificação Com o Numero do Documento
                int identificacaoStatus = Convert.ToInt32(statusVenda.Identificacao);
                int identificacaoResposta = Convert.ToInt32(dadosPagamento.Identificacao);
                if (pagamento.NumeroDoc == identificacaoStatus && identificacaoResposta == identificacaoStatus)
                {
                    //TODO: Implementar verificação:
                    //Analisar mais opções a ser implementados
                    if(statusVenda.Comando != "CRT")
                    {
                        menssagemErro = "Codigo de comando invalido!";
                        return false;
                    }
                    if(dadosPagamento.Comando != "CRT")
                    {
                        menssagemErro = "Codigo de comando invalido!";
                        return false;
                    }
                    if(dadosPagamento.MensagemOperador == "")
                    {
                        menssagemErro = "OPERAÇÃO CANCELADA";
                        return false;
                    }
                    if((statusVenda.Identificacao != Convert.ToString(pagamento.NumeroDoc))||( dadosPagamento.Identificacao != Convert.ToString(pagamento.NumeroDoc)))
                    {
                        menssagemErro = "Codigo de Identificação invalido!\n"+dadosPagamento.MensagemOperador;
                        return false;
                    }
                    return true;
                }
                else
                {
                    //TODO Implementar
                    return false;
                }
            }
            catch (Exception ex) {
                menssagemErro = "Erro validação do arquivo!";
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

            conteudo = "000-000 = CRT\r\n" +            //Comando - 000-000 = CRT - Realiza uma transação de venda

            "001-000 = " + indentificacao + "\r\n" +    //Número de controle gerado pela Automação Comercial,
                                                      //devendo o valor ser diferente para cada nova operação de
                                                      //TEF.É ecoado pelo Gerenciador Padrão nos arquivos de
                                                      //status e de resposta, e deve ser consistido pelo Automação
                                                      //Comercial.

            "002-000 = " + documentoFiscal + "\r\n" +   //Número do documento fiscal ao qual a operação de TEF está
                                                      //vinculada.Caso seja usada uma Impressora Fiscal, o
                                                      //preenchimento deste campo é obrigatório para transações de venda.

            "003-000 = " + valorTotal + "\r\n" +        //Valor total da operação, em centavos da moeda informada
                                                      //no campo 004‐000, incluindo todas as taxas cobradas do
                                                      //Cliente(serviço, embarque, etc.).
                                                      //No arquivo de resposta para transações de venda, este
                                                      //campo indica o valor efetivamente debitado do Cliente e
                                                      //creditado para o Estabelecimento(desconsiderando taxas
                                                      //referentes ao uso da solução, descontadas pela Rede
                                                      //Adquirente).

            "004-000 = " + "0" + "\r\n" +              //0: Real
                                                     //1: Dólar americano

            "011-000 = " + operacaoTef + "\r\n";       //Venda ou cancelamento (de acordo com o campo 000‐000):
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
                "017 - 000 = " + parcelamentoEstabelecimento + "\r\n" + //0: parcelado pelo Estabelecimento; 1: parcelado pelo Emissor.

                "018-000 = " + quantidadeParcela + "\r\n";            //Quantidade de parcelas, para transações parceladas.
            }

            if (RegistroCertificacao != null)
            {
                conteudo +=
                    "706-000 = " + "3" + "\r\n" +                 //Soma dos seguintes valores, identificando as
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


                    "716-000 = " + nomeSoftHouse + "\r\n" +        //Razão social da empresa responsável pelo desenvolvimento
                                                                 //da aplicação de Automação Comercial.
                                                                 //Exemplo: KND SISTEMAS LTDA.

                    "717-000 = " + data + "\r\n" +                 //Data / hora registrada no cupom fiscal, no formato AAMMDDhhmmss
                                                                 //Caso seja usada uma Impressora Fiscal, o preenchimento
                                                                 //deste campo é obrigatório para transações de venda.

                    "733-000 = " + versaoSistema + "\r\n" +        //Valor fixo, identificando a versão deste documento
                                                                 //implementada pela Automação Comercial(somente
                                                                 //números, por exemplo, 210 para “v2.10”).

                    "735-000 = " + NomeDaAutomacao + "\r\n" +       //Nome da aplicação de Automação Comercial.
                    "736-000 = " + VersaoDeAutomacao + "\r\n" +     //Versão da aplicação de Automação Comercial, conforme
                                                                  //nomenclatura utilizada pelo desenvolvedor.
                    "738-000 = " + RegistroCertificacao + "\r\n";   //Registro de Certificação
            }
            conteudo += "999-999 = 0\r\n";                           //Conteudo fixo: 0 (Zero)
            return conteudo;
        }

        private bool ConverterdadosPagamento(ArquivoTexto ArquivoStatusVenda, ArquivoTexto ArquivodadosPagamento)
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
                string[] respostaLinhas = ArquivodadosPagamento.Conteudo.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var linha in respostaLinhas)
                {
                    // Dividir cada linha em partes usando o sinal de igual como delimitador
                    string[] partes = linha.Split('=');

                    if (partes.Length == 2)
                    {
                        switch (partes[0].Trim())
                        {
                            case "000-000":
                                dadosPagamento.Comando = partes[1].Trim();
                                break;
                            case "001-000":
                                dadosPagamento.Identificacao = partes[1].Trim();
                                break;
                            case "002-000":
                                dadosPagamento.DocumentoFiscal = partes[1].Trim();
                                break;
                            case "003-000":
                                dadosPagamento.ValorTotal = partes[1].Trim();
                                break;
                            case "004-000":
                                dadosPagamento.Moeda = partes[1].Trim();
                                break;
                            case "009-000":
                                dadosPagamento.Status = partes[1].Trim();
                                break;
                            case "010-000":
                                dadosPagamento.RedeAdquirente = partes[1].Trim();
                                break;
                            case "011-000":
                                dadosPagamento.TipoTransacao = partes[1].Trim();
                                break;
                            case "012-000":
                                dadosPagamento.NSU = partes[1].Trim();
                                break;
                            case "013-000":
                                dadosPagamento.CodigoAutorizacao = partes[1].Trim();
                                break;
                            case "022-000":
                                dadosPagamento.DataComprovante = partes[1].Trim();
                                break;
                            case "023-000":
                                dadosPagamento.HoraComprovante = partes[1].Trim();
                                break;
                            case "027-000":
                                dadosPagamento.CodigoControle = partes[1].Trim();
                                break;
                            case "030-000":
                                dadosPagamento.MensagemOperador = partes[1].Trim();
                                break;
                            case "040-000":
                                dadosPagamento.NomeCartaoAdm = partes[1].Trim();
                                break;
                            case "707-000":
                                dadosPagamento.ValorOriginal = partes[1].Trim();
                                break;
                            case "708-000":
                                dadosPagamento.ValorTroco = partes[1].Trim();
                                break;
                            case "718-000":
                                dadosPagamento.NumeroLogicoTerminal = partes[1].Trim();
                                break;
                            case "719-000":
                                dadosPagamento.CodigoEstabelecimento = partes[1].Trim();
                                break;
                            case "729-000":
                                dadosPagamento.StatusConfirmacao = partes[1].Trim();
                                break;
                            case "730-000":
                                dadosPagamento.Operacao = partes[1].Trim();
                                break;
                            case "731-000":
                                dadosPagamento.TipoDeCartao = partes[1].Trim();
                                break;
                            case "732-000":
                                dadosPagamento.TipoDeFinanciamento = partes[1].Trim();
                                break;
                            case "715-009":
                                dadosPagamento.BandeiraNCartao = partes[1].Trim();
                                break;
                            case "715-016":
                                dadosPagamento.TipoDeAutorizacao1 = partes[1].Trim();
                                break;
                            case "715-017":
                                dadosPagamento.TipoDeAutorizacao2 = partes[1].Trim();
                                break;
                            case "999-999":
                                dadosPagamento.RegistroFinalizador = partes[1].Trim();
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
        private bool ConverterRespostaConfirmacao(ArquivoTexto ArquivoStatusConfirmacao)
        {
            try
            {
                // Dividir a string em linhas
                string[] statusLinhas = ArquivoStatusConfirmacao.Conteudo.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var linha in statusLinhas)
                {
                    // Dividir cada linha em partes usando o sinal de igual como delimitador
                    string[] partes = linha.Split('=');

                    if (partes.Length == 2)
                    {
                        switch (partes[0].Trim())
                        {
                            case "000-000":
                                statusConfirmacao.Comando = partes[1].Trim();
                                break;
                            case "001-000":
                                statusConfirmacao.Identificacao = partes[1].Trim();
                                break;
                            case "999-999":
                                statusConfirmacao.RegistroFinalizador = partes[1].Trim();
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
        private bool ValidarConfirmacao(ModeloPagamento pagamento)
        {
            //Verificar status da resposta bate com o valor Identificação Com o Numero do Documento
            int identificacaoStatus = Convert.ToInt32(statusVenda.Identificacao);
            int identificacaoResposta = Convert.ToInt32(dadosPagamento.Identificacao);
            if (pagamento.NumeroDoc == identificacaoStatus && identificacaoResposta == identificacaoStatus)
            {
                //TODO: Implementar verificação:
                //Analisar mais opções a ser implementados
                if (statusConfirmacao.Comando != "CNF")
                {
                    menssagemErro = "Codigo de comando invalido!";
                    return false;
                }
                if ((statusConfirmacao.Identificacao != Convert.ToString(pagamento.NumeroDoc)) || (statusConfirmacao.Identificacao != Convert.ToString(pagamento.NumeroDoc)))
                {
                    menssagemErro = "Codigo de Identificação invalido!\n" + dadosPagamento.MensagemOperador;
                    return false;
                }
                return true;
            }
            else
            {
                menssagemErro = "Erro ao validar respota entrar em contato com o suporte";
                return false;
            }
        }
        private bool ConfirmarPagamento(ModeloPagamento pagamento)
        {
            try
            {
                //Cria o Arquivo de confirmação de pagamento
                string conteudo = "000-000 = CNF \r\n" +                                  //Confirma a última Transação realizada
                                    "001-000 = " + dadosPagamento.Identificacao + "\r\n" +  //Identificação
                                    "002-000 = " + dadosPagamento.DocumentoFiscal+"\r\n" +  //Documento Fiscal
                                    "010-000 = " + dadosPagamento.RedeAdquirente+"\r\n" +   //Rede Adquirente
                                    "027-000 = " + dadosPagamento.CodigoControle+"\r\n" +   //Código de controle
                                                                                            //Estes campos não são obrigatorios
                                                                                            //"733-000 = " + dadosPagamento."\r\n" + //Versão Da interface
                                                                                            //"735-000 = " + dadosPagamento"\r\n" + //Nome Da Automação
                                                                                            //"736-000 = " + dadosPagamento"\r\n" + //Versão De Automação
                                                                                            //"738-000 = " + dadosPagamento"\r\n" + //Registro de Certificação
                                    "999-999 = 0";                                          //Registro Finalizador
                ArquivoTexto arquivo = new ArquivoTexto
                {
                    NomeArquivo = pagamento.NomeArquivo,
                    CaminhoArquivo = pagamento.CaminhoArquivo + "/Req",
                    ExtensaoArquivo = pagamento.ExtensaoArquivo,
                    Conteudo = conteudo
                };
                ArquivoTXT Arquivo = new ArquivoTXT();
                var result = Arquivo.CriarArquivo(arquivo);

                return true;
            }catch (Exception ex) {
                return false;
            }            
        }
        private bool LerArquivoConfirmacao(string caminhoArquivo)
        {
            try
            {
                //Ler arquivo status de venda
                ArquivoTexto ArquivoStatusConfirmacao = new ArquivoTexto
                {
                    NomeArquivo = "intpos",
                    CaminhoArquivo = caminhoArquivo + "/Resp",
                    ExtensaoArquivo = "sts",
                };
                ArquivoTXT arquivo = new ArquivoTXT();
                ArquivoStatusConfirmacao.Conteudo = arquivo.LerArquivo(ArquivoStatusConfirmacao);
                
                if (ConverterRespostaConfirmacao(ArquivoStatusConfirmacao))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private void LimparArquivosExistentes(string caminho)
        {
            // Especifica o caminho completo do arquivo que você deseja verificar e excluir
            string resp001 = caminho + "/resp/intpos.001";
            string respSTS = caminho + "/resp/intpos.sts";
            string req001 = caminho + "/req/intpos.001";
            string reqSTS = caminho + "/req/intpos.sts";

            // Verifica se os arquivos existes
            if (File.Exists(resp001))
            {
                File.Delete(resp001);
            }
            if (File.Exists(respSTS))
            {
                File.Delete(respSTS);
            }
            if (File.Exists(req001))
            {
                File.Delete(req001);
            }
            if (File.Exists(reqSTS))
            {
                File.Delete(reqSTS);
            }


        }
        
    }
}
