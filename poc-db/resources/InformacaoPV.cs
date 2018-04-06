using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    /// <summary>
    /// Informação PV
    /// </summary>
    public class InformacaoPV
    {
        /// <summary>
        /// Numero do pv
        /// </summary>
        public String PontoVenda { get; set; }
        /// <summary>
        /// Tipo Estabelecimento
        /// </summary>
        public int CodigoTipoEstabelecimento { get; set; }
        /// <summary>
        /// Centralização
        /// </summary>
        public String Centralizacao { get; set; }
        /// <summary>
        /// Codigo Tipo Consignação
        /// </summary>
        public String CodigoTipoConsigancao { get; set; }
        /// <summary>
        /// Codigo centralizador
        /// </summary>
        public int Centralizador { get; set; }

    }
}
