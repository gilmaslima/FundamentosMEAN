using System;
using System.ComponentModel;

namespace Redecard.PN.Request.SharePoint.Model
{
    /// <summary>
    /// Definição de IdPesquisa para persistência da pesquisa no lado servidor
    /// </summary>
    [Serializable]
    public class ComprovanteRequestIdPesquisa
    {
        /// <summary>
        /// Status do comprovante (histórico/pendente)
        /// </summary>
        public StatusComprovante Status { get; set; }

        /// <summary>
        /// Tipo da venda (crédito/débito)
        /// </summary>
        public TipoVendaComprovante TipoVenda { get; set; }

        /// <summary>
        /// Identificador da pesquisa para persistência em cache no lado servidor
        /// </summary>
        public Guid IdPesquisa { get; set; }
    }
}
