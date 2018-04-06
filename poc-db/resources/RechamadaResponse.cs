using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Classe modelo de resposta com as informações de rechamada para obter as novas páginas
    /// </summary>
    [DataContract]
    public class RechamadaResponse
    {
        /// <summary>
        /// Total de Registros
        /// </summary>
        [DataMember]
        public Int64 TotalRegistros { get; set; }

        /// <summary>
        /// Último registro da página retornada para rechamada
        /// </summary>
        [DataMember]
        public Int64 RegistroFinal { get; set; }

    }
}