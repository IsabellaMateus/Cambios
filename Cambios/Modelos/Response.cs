namespace Cambios.Modelos
{
    public class Response
    {
        #region Propriedades
        public bool IsSuccecc { get; set; } /*correu tudo bem ou não*/

        public string Message { get; set; } /*caso as coisas corram mal, diz o que se passou*/

        public object Result { get; set; } /*objeto com o resultado do meu 'Request' à API*/
        #endregion
    }
}
