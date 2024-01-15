using System;
using System.Data.SQLite;

namespace MultiTef.Log
{
    public class SqLite
    {
        private string connectionString = "Data Source=logTEF.db;Version=3;";

        // Criação da tabela
        private string createTableQuery = "CREATE TABLE IF NOT EXISTS MyTable (Id INTEGER PRIMARY KEY, Name TEXT NOT NULL);";

        // Inserção de dados de exemplo
        private string insertDataQuery = "INSERT INTO MyTable (Name) VALUES ('John Doe'), ('Jane Doe');";

        // Consulta de dados
        private string selectDataQuery = "SELECT * FROM MyTable;";

        public void CriarTabela()
        {
            // Conexão com o banco de dados
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Criação da tabela
                using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                }
            }
        }

        public void InserirDadosExemplo()
        {
            // Conexão com o banco de dados
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Inserção de dados
                using (SQLiteCommand insertDataCommand = new SQLiteCommand(insertDataQuery, connection))
                {
                    insertDataCommand.ExecuteNonQuery();
                }
            }
        }

        public void ConsultarDados()
        {
            // Conexão com o banco de dados
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Consulta de dados
                using (SQLiteCommand selectDataCommand = new SQLiteCommand(selectDataQuery, connection))
                {
                    using (SQLiteDataReader reader = selectDataCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}");
                        }
                    }
                }
            }
        }
    }
}