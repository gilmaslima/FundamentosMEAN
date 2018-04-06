#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Agnaldo Costa]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [31/07/2012] – [Agnaldo Costa] – [Criação]
*/
#endregion
using System;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Classe modelo de Pré-requisitos de uma Solicitação cadastradas
    /// </summary>
    [DataContract]
    public class PreRequisitoSolicitacao
    {
        /// <summary>
        /// Nome do campo
        /// </summary>
        [DataMember]
        public String NomeCampo { get; set; }

        /// <summary>
        /// Valor do campo
        /// </summary>
        [DataMember]
        public String ValorCampo { get; set; }

        /// <summary>
        /// Tipo do campo
        /// </summary>
        [DataMember]
        public String TipoCampo { get; set; }
    }
}