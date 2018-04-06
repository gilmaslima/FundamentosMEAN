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
    /// Classe modelo para Período de Sazonalidade da Oferta
    /// </summary>
    [DataContract]
    public class SazonalidadeOferta
    {
        /// <summary>
        /// Ano início
        /// </summary>
        [DataMember]
        public Int32 AnoInicio { get; set; }

        /// <summary>
        /// Mês final da sazonalidade
        /// </summary>
        [DataMember]
        public Int32 MesFinal { get; set; }

        /// <summary>
        /// Mês de início da sazonalidade
        /// </summary>
        [DataMember]
        public Int32 MesInicio { get; set; }

        /// <summary>
        /// Descrição do Mês de Fim
        /// </summary>
        [DataMember]
        public String MesAnoFimDescricao { get; set; }

        /// <summary>
        /// Descrição do Mês de Início
        /// </summary>
        [DataMember]
        public String MesAnoInicioDescricao { get; set; }

        /// <summary>
        /// Mês e Ano de Início em formato dd/yyyy
        /// </summary>
        [DataMember]
        public String MesAnoInicio { get; set; }

        /// <summary>
        /// Mês e Ano de Início em formato dd/yyyy
        /// </summary>
        [DataMember]
        public String MesAnoInicioDescricaoNaoAbreviada { get; set; }
    }
}