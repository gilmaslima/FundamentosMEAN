using System;
using System.Runtime.Serialization;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Request
{
    /// <summary>
    /// Classe ConciliadorRequest
    /// </summary>
    [DataContract]
    public class ConciliadorRequest
    {
        /// <summary>
        /// Número PV do Estabelecimento Matriz
        /// </summary>
        [DataMember]
        public Int64 PvMatriz { get; set; }

        /// <summary>
        /// 
        /// 0206 - Conciliador
        /// 0207 - Retroativo
        /// </summary>
        [DataMember]
        public Int32 CodigoServico { get; set; }

        /// <summary>
        /// Regime Solicitado
        /// </summary>
        [DataMember]
        public Int32 CodigoRegime { get; set; }

        /// <summary>
        /// Canal da Operação
        /// </summary>
        [DataMember]
        public Int32 Canal { get; set; }

        /// <summary>
        /// Célula da Operação
        /// </summary>
        [DataMember]
        public Int32 Celula { get; set; }

        /// <summary>
        /// Código do Vendedor
        /// </summary>
        [DataMember]
        public Int32 CodigoVendedor { get; set; }

        /// <summary>
        /// Usuário Logado
        /// </summary>
        [DataMember]
        public String Usuario { get; set; }

        /// <summary>
        /// 1 - Contratação
        /// 2 - Cancelamento
        /// </summary>
        [DataMember]
        public Int32 TipoSolicitacao { get; set; }
    }
}
