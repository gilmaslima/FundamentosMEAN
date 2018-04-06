#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [12/07/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Representa uma remessa (Envio de Material de Vendas)
    /// </summary>
    [DataContract]
    public class Remessa
    {
        /// <summary>
        /// Data da Remessa
        /// </summary>
        [DataMember]
        public DateTime DataRemessa { get; set; }

        /// <summary>
        /// Número do Protocolo da Remessa
        /// </summary>
        [DataMember]
        public Decimal NumeroProtocolo { get; set; }

        /// <summary>
        /// Descrição do KIT
        /// </summary>
        [DataMember]
        public Kit Kit { get; set; }

        /// <summary>
        /// Quantidade de Itens da Remessa
        /// </summary>
        [DataMember]
        public Int32 Quantidade { get; set; }

        /// <summary>
        /// Motivo do Envio
        /// </summary>
        [DataMember]
        public Motivo Motivo { get; set; }

        /// <summary>
        /// Origem da Remessa
        /// </summary>
        [DataMember]
        public String Origem { get; set; }
    }
}