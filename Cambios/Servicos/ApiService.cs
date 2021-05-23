namespace Cambios.Modelos.Servicos
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ApiService
    {
        public async Task<Response> getRates(string urlBase, string controller)
        {
            //Usar sempre um "try... catch ..." quando se faz ligações à internet
            try
            {
                var client = new HttpClient(); /*Conexão externa via HTTP*/
                client.BaseAddress = new Uri(urlBase); /*Endereço base da API*/

                //await - tarefa assincrona para que vá buscar as taxas sem que a aplicação pare
                var response = await client.GetAsync(controller); /*Controlador da API*/
                var result = await response.Content.ReadAsStringAsync(); /*Lê o conteudo da resposta como uma string*/

                if (!response.IsSuccessStatusCode) /*Caso alguma coisa correr mal*/
                {
                    return new Response
                    {
                        IsSuccecc = false,
                        Message = result /*resultado que o Json vai devolver*/
                    };
                }

                // Converte o resultado numa lista de objectos do tipo 'Rate'
                var rates = JsonConvert.DeserializeObject<List<Rate>>(result);

                return new Response
                {
                    // o resultado se tudo correr bem, é a lista de taxas
                    IsSuccecc = true,
                    Result = rates
                };
            }
            catch (Exception ex)
            {

                return new Response
                {
                    IsSuccecc = false,
                    Message = ex.Message /*mensagem da exceção*/
                };
            }
        }
    }
}
