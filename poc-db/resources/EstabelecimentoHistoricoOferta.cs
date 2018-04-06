using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrosServicos.Modelo
{
    /// <summary>
    /// Classe modelo de Estabelecimentos do Histórico de Ofeta
    /// </summary>
    public class EstabelecimentoHistoricoOferta
    {
        /// <summary>
        /// Número do Estabelecimento
        /// </summary>
        public Int64 NumeroEstabelecimento { get; set; }

        /// <summary>
        /// Razão social do Estabelecimento
        /// </summary>
        public String RazaoSocial { get; set; }

        /// <summary>
        /// Indica se o estabelecimento atingiu a Meta
        /// </summary>
        public Boolean AtingiuMeta { get; set; }
    }
}
