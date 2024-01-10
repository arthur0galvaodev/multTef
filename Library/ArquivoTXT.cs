using Microsoft.AspNetCore.Mvc;
using MultiTef.Models;

namespace MultiTef.Library
{
    public class ArquivoTXT
    {
        public JsonResult CriarArquivo(ArquivoTexto arquivo)
        {
            try
            {
                string caminhoCompleto = Path.Combine(arquivo.CaminhoArquivo, $"{arquivo.NomeArquivo}.{arquivo.ExtensaoArquivo}");

                using (StreamWriter escritor = new StreamWriter(caminhoCompleto))
                {
                    escritor.Write(arquivo.Conteudo);
                }

                return new JsonResult($"Arquivo {caminhoCompleto} criado com sucesso.");
            }
            catch (Exception e)
            {
                return new JsonResult($"Erro ao criar o arquivo: {e.Message}");
            }
        }

        public string LerArquivo(ArquivoTexto arquivo)
        {
            try
            {
                string caminhoArquivo = Path.Combine(arquivo.CaminhoArquivo, $"{arquivo.NomeArquivo}.{arquivo.ExtensaoArquivo}");

                // Lê o conteúdo do arquivo e retorna como uma string
                string conteudo = File.ReadAllText(caminhoArquivo);
                File.Delete(caminhoArquivo);
                return conteudo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler o arquivo: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
