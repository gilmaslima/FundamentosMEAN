using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rede.PN.ApiLogin.Modelo
{
    /// <summary>
    /// Classe de status
    /// </summary>
    public class Status
    {
        /// <summary>
        /// Código do Status
        /// </summary>
        public Int32? Codigo { get; set; }

        /// <summary>
        /// Descrição do Status
        /// </summary>
        public String Descricao { get; set; }
    }
}
