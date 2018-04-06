using System.Collections.Generic;

namespace Rede.PN.ApiLogin.Sharepoint.ISAPI.Login.Models.Response
{
    public class ListarEstabelecimentosResponse
    {
        /// <summary>
        /// Entidades para retornar
        /// </summary>
        public IEnumerable<Entidade> Entidades { get; set; }

        /// <summary>
        /// Status de Retorno
        /// </summary>
        public StatusRetorno StatusRetorno { get; set; }
    }
}