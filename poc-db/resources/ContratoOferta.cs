/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Classe modelo de Contrato da Oferta
    /// </summary>
    [DataContract]
    public class ContratoOferta
    {
        /// <summary>
        /// Código do Contrato
        /// </summary>
        [DataMember]
        public Int64 CodigoContrato { get; set; }

        /// <summary>
        /// Código da Estrutura de Meta
        /// </summary>
        [DataMember]
        public Int32 CodigoEstruturaMeta { get; set; }

        /// <summary>
        /// Código da Oferta do Contrato
        /// </summary>
        [DataMember]
        public Int32 CodigoOferta { get; set; }


        /// <summary>
        /// Código da Proposta
        /// </summary>
        [DataMember]
        public Int64 CodigoProposta { get; set; }

        /// <summary>
        /// Indica se a Oferta possui Ramo de Atividade
        /// </summary>
        [DataMember]
        public Boolean PossuiRamo { get; set; }

        /// <summary>
        /// Indica se a Oferta possui Produto Crédito - Meta
        /// </summary>
        [DataMember]
        public Boolean PossuiProdutoCreditoMeta { get; set; }

        /// <summary>
        /// Indica se a Oferta possui Produto Débito - Meta
        /// </summary>
        [DataMember]
        public Boolean PossuiProdutoDebitoMeta { get; set; }
    }
}