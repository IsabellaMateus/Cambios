namespace Cambios
{
    using Modelos;
    using Modelos.Servicos;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        #region Atributos
        private NetWorkService netWorkService;
        private ApiService apiService;
        private List<Rate> Rates;
        private DialogService dialogService;
        #endregion

        public Form1()
        {
            InitializeComponent();
            netWorkService = new NetWorkService();
            apiService = new ApiService();
            dialogService = new DialogService();
            LoadRates(); /*cria as taxas*/
        }

        //async - método assincrono
        private async void LoadRates()
        {
            bool load; /*variável para controlar se o load das taxas já foi feito*/

            //Fazer o check da connection
            LabelResultado.Text = "A atualizar taxas...";
            var connection = netWorkService.CheckConnection(); /*variavel para testar a conecção*/
            if (!connection.IsSuccecc)
            {
                //Carregar as taxas da base de dados
                LoadLocalRates();
                load = false;
            }
            else
            {
                //Vou chamar a API através de um método
                await LoadApiRates();
                load = true;
            }

            //as listas não foram carregadas
            if (Rates.Count == 0)
            {
                LabelResultado.Text = "Não há ligação à internet" + Environment.NewLine +
                    "e não foram carregadas previamente as taxas." + Environment.NewLine +
                    "Tente mais tarde!";
                return;
            }

            ComboBoxOrigem.DataSource = Rates;
            ComboBoxOrigem.DisplayMember = "Name";

            // Corrige bug da microsoft
            // Para que possa alterar a taxa de uma ComboBox sem que altere na outra ComboBox
            // BindingContext - Classe que liga os objetos do interface ao código
            ComboBoxDestino.BindingContext = new BindingContext();

            ComboBoxDestino.DataSource = Rates;
            ComboBoxDestino.DisplayMember = "Name";

            if (load) /*taxas carregadas da API*/
            {
                LabelStatus.Text = string.Format("Taxas carregadas da internet em {0:F}", DateTime.Now);
            }
            else /*taxas carregadas da base de dados*/
            {
                LabelStatus.Text = "Taxas carregadas da Base de Dados.";
            }

            ProgressBar1.Value = 100;
            LabelResultado.Text = "Taxas atualizadas...";
            ButtonConverter.Enabled = true;
            ButtonTrocar.Enabled = true;
        }

        private void LoadLocalRates()
        {
            MessageBox.Show("Não está implementado");
        }

        private async Task LoadApiRates()
        {
            ProgressBar1.Value = 0;

            //Parametros de entrada: endereço base da API e seu controlador
            var response = await apiService.getRates("http://cambios.somee.com", "/api/rates");

            Rates = (List<Rate>)response.Result;
        }

        private void ButtonConverter_Click(object sender, EventArgs e)
        {
            Converter(); /*método que agarra nos valores e faz a conversão*/ 
        }

        private void Converter()
        {
            //Validação da TextBox
            if (string.IsNullOrEmpty(TextBoxValor.Text))
            {
                dialogService.ShowMessage("Erro","Insira um valor a converter!");
                return;
            }

            decimal valor;
            if (!decimal.TryParse(TextBoxValor.Text, out valor))
            {
                dialogService.ShowMessage("Erro de conversão", "O valor terá de ser numérico!");
                return;
            }

            //Validação das ComboBox's
            //esta parte não é obrigatória pois a ComboBox coloca sempre um valor por defeito
            if (ComboBoxOrigem.SelectedItem == null)
            {
                dialogService.ShowMessage("Erro", "Tem de escolher uma moeda a cenverter!");
                return;
            }

            if (ComboBoxDestino.SelectedItem == null)
            {
                dialogService.ShowMessage("Erro", "Tem de escolher uma moeda de destino para converter!");
                return;
            }

            var taxaOrigem = (Rate) ComboBoxOrigem.SelectedItem;
            var taxaDestino = (Rate) ComboBoxDestino.SelectedItem;

            var valorConvertido = valor / (decimal)taxaOrigem.TaxRate * (decimal)taxaDestino.TaxRate;

            LabelResultado.Text = string.Format("{0} {1:C2} = {2} {3:C2}", 
                taxaOrigem.Code, valor, taxaDestino.Code, valorConvertido);
        }

        private void ButtonTrocar_Click(object sender, EventArgs e)
        {
            Trocar();
        }

        private void Trocar()
        {
            var aux = ComboBoxOrigem.SelectedItem;
            ComboBoxOrigem.SelectedItem = ComboBoxDestino.SelectedItem;
            ComboBoxDestino.SelectedItem = aux;
            Converter();
        }

    }
}
