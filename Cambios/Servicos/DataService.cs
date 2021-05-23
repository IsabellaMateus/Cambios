namespace Cambios.Servicos
{
    using Modelos;
    using Modelos.Servicos;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;

    public class DataService
    {
        private SQLiteConnection connection;
        private SQLiteCommand command;
        private DialogService dialogService;

        //Vou criar na raiz uma pasta chamada 'Data' a onde coloco a base de dados
        //Se a pasta não existir, ele vai criar
        public DataService()
        {
            dialogService = new DialogService();

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            var path = @"Data/Rates.sqlite"; /*caminho da base de dados*/

            try
            {
                //conexão à base de dados
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open(); /*cria a base de dados se ela não existir*/

                //comando em SQL
                string sqlcommand =
                    "create table if not exists Rates (RateId int, Code varchar(5), TaxRate real, Name varchar(250))";

                //vou instanciar o meu comando
                command = new SQLiteCommand(sqlcommand, connection);

                //vou executar o comando
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        //Método para gravar a lista de taxas na base de dados
        public void SaveData(List<Rate> Rates)
        {
            try
            {
                foreach (Rate rate in Rates)
                {
                    //Comando de SQL
                    string sql = string.Format("insert into Rates (RateId, Code, TaxRate, Name) values({0}, '{1}', '{2}', '{3}')",
                        rate.RateId, rate.Code, rate.TaxRate, rate.Name);

                    //Executar o comando
                    command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();
                }

                //Fechar a conexao
                connection.Close();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }

        //Método para buscar os valores da tabela de dados
        public List<Rate> GetData()
        {
            List<Rate> Rates = new List<Rate>();

            try
            {
                //Comando de SQL
                string sql = "select * from Rates";

                //Instancia o comando
                command = new SQLiteCommand(sql, connection);

                //Lê cada registo
                SQLiteDataReader reader = command.ExecuteReader(); /*para ler os dados*/

                while (reader.Read())
                {
                    Rates.Add(new Rate
                    {
                        RateId = (int)reader["RateId"],
                        Code = (string)reader["Code"],
                        Name = (string)reader["Name"],
                        TaxRate = (double)reader["TaxRate"]
                    });
                }

                //Fechar a conexao
                connection.Close();
                return Rates;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
                return null; /*Tenho de colocar isto pois digo que a função retorna uma lista*/
            }
        }

        //Método para apagar dados antigos / limpa a base de dados
        public void DeleteData()
        {
            try
            {
                //Comando de SQL
                string sql = "delete from Rates";

                //Executar o comando
                command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Erro", e.Message);
            }
        }
    }
}
