namespace Cambios
{
    using Modelos;
    using Modelos.Servicos;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        #region Atributos
        private NetWorkService netWorkService;
        private ApiService apiService;
        #endregion

        #region Propriedade
        //Estou aqui a instanciar a lista pois apenas a vou usar uma vez,
        //mas também podia instanciá-la dentro do construtor
        public List<Rate> Rates { get; set; } = new List<Rate>();
        #endregion

        public Form1()
        {
            InitializeComponent();
            netWorkService = new NetWorkService();
            apiService = new ApiService();
            LoadRates(); /*cria as taxas*/
        }

        //async - método assincrono
        private async void LoadRates()
        {
            //bool load; /*variável para controlar se o load das taxas já foi feito*/

            //Fazer o check da connection
            LabelResultado.Text = "A atualizar taxas...";
            var connection = netWorkService.CheckConnection(); /*variavel para testar a conecção*/
            if (!connection.IsSuccecc)
            {
                //Carregar as taxas da base de dados (no futuro)
                MessageBox.Show(connection.Message);
                return;
            }
            else
            {
                //Vou chamar a API através de um método
                await LoadApiRates();
            }

            ComboBoxOrigem.DataSource = Rates;
            ComboBoxOrigem.DisplayMember = "Name";

            // Corrige bug da microsoft
            // Para que possa alterar a taxa de uma ComboBox sem que altere na outra ComboBox
            // BindingContext - Classe que liga os objetos do interface ao código
            ComboBoxDestino.BindingContext = new BindingContext();

            ComboBoxDestino.DataSource = Rates;
            ComboBoxDestino.DisplayMember = "Name";

            ProgressBar1.Value = 100;
            LabelResultado.Text = "Taxas carregadas...";
        }

        private async Task LoadApiRates()
        {
            ProgressBar1.Value = 0;

            //Parametros de entrada: endereço base da API e seu controlador
            var response = await apiService.getRates("http://cambios.somee.com", "/api/rates");

            Rates = (List<Rate>)response.Result;
        }
    }
}
