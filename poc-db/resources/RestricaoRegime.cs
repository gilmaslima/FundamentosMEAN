#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Agnaldo Costa]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [16/07/2012] – [Agnaldo Costa] – [Criação]
*/
#endregion

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Restrição de Regime da Entidade
    /// </summary>
    [DataContract]
    public class RestricaoRegime
    {
        /// <summary>
        /// Código da Restrição
        /// </summary>
        [DataMember]
        public Int32 CodigoRestricao { get; set; }

        /// <summary>
        /// Código de Versão da Restrição
        /// </summary>
        [DataMember]
        public Int32 CodigoVersao { get; set; }

        /// <summary>
        /// Tipo da Restrição
        /// </summary>
        [DataMember]
        public String TipoRestricao { get; set; }

        /// <summary>
        /// Descrição da Restrição
        /// </summary>
        [DataMember]
        public String DescricaoRestricao { get; set; }

        /// <summary>
        /// Corpo do Contrato Restrição
        /// </summary>
        [DataMember]
        public String CorpoContrato { get; set; }
    }
}