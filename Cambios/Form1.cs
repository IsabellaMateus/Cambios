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

            ProgressBar1.Value = 0;

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

            // Converte o resultado numa lista de objectos do tipo 'Rate'
            var rates = JsonConvert.DeserializeObject<List<Rate>>(result); 

            ComboBoxOrigem.DataSource = rates;
            ComboBoxOrigem.DisplayMember = "Name";

            //Corrige bug da microsoft
            // Para que possa alterar a taxa de uma ComboBox sem que altere na outra ComboBox
            // BindingContext - Classe que liga os objetos do interface ao código
            ComboBoxDestino.BindingContext = new BindingContext(); 

            ComboBoxDestino.DataSource = rates;
            ComboBoxDestino.DisplayMember = "Name";

            ProgressBar1.Value = 100;
        }
    }
}
