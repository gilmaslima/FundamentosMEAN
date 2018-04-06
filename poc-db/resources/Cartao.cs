#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [17/07/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Retorna dados de um cartão consultado pelo Emissor
    /// </summary>
    [DataContract]
    public class Cartao
    {
        /// <summary>
        /// Nome do emissor do cartão
        /// </summary>
        [DataMember]
        public String NomeEmissor { get; set; }

        /// <summary>
        /// País de Origem do Cartão
        /// </summary>
        [DataMember]
        public String PaisEmissor { get; set; }

        /// <summary>
        /// Tipo do Cartão
        /// </summary>
        [DataMember]
        public String TipoCartao { get; set; }
    }
}
