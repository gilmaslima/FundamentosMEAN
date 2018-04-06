using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rede.PN.ApiLogin.Modelo.Response
{
    public class ListarEntidadeResponse
    {
        /// <summary>
        /// Lista de Entidades
        /// </summary>
        public List<Modelo.Entidade> Entidades { get; set; }

        /// <summary>
        /// Status Retorno
        /// </summary>
        public StatusResponse StatusRetorno { get; set; }
    }
}
