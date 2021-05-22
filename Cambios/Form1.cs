namespace Cambios
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Windows.Forms;
    using Newtonsoft.Json;
    using Modelos;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadRates(); /*cria as taxas*/
        }

        //async - método assincrono
        private async void LoadRates()
        {
            //bool load; /*variável para controlar se o load das taxas já foi feito*/

            var client = new HttpClient(); /*Conexão externa via HTTP*/
            client.BaseAddress = new Uri("http://cambios.somee.com"); /*Endereço base da API*/

            //await - tarefa assincrona para que vá buscar as taxas sem que a aplicação pare
            var response = await client.GetAsync("/api/rates"); /*Controlador da API*/
            var result = await response.Content.ReadAsStringAsync(); /*Lê o conteudo da resposta como uma string*/

            if (!response.IsSuccessStatusCode) /*Caso alguma coisa correr mal*/
            {
                MessageBox.Show(response.ReasonPhrase); /*avisa do erro que ocorreu*/
                return;
            }

            var rates = JsonConvert.DeserializeObject<List<Rate>>(result);

            ComboBoxOrigem.DataSource = rates;
        }
    }
}
