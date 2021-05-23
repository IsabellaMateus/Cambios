namespace Cambios.Modelos.Servicos
{
    using Modelos;
    using System.Net;

    public class NetWorkService
    {

        // método para checkar se tenho ou não ligação à internet
        public Response CheckConnection()
        {
            var client = new WebClient();

            try
            {
                // Uma forma de testar se tenho ligação à net, é fazer um 'ping' ao servidor da google
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return new Response
                    { 
                        IsSuccecc = true
                    };
                }
            }
            catch /*caso as coisas não corram bem ou eu não consiga fazer a ligação*/
            {

                return new Response
                {
                    IsSuccecc = false,
                    Message = "Configure a sua ligação à internet"
                };
            }
        }
    }
}
