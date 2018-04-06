using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rede.PN.ApiLogin.Modelo.Response
{
    /// <summary>
    /// Classe modelo de status da resposta
    /// </summary>
    public class StatusResponse
    {
        /// <summary>
        /// Código de Retorno
        /// </summary>
        public Int32 Codigo { get; set; }

        /// <summary>
        /// Mensagem de retorno
        /// </summary>
        public String Mensagem { get; set; }
    }
}
