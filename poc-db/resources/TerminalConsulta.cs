/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.Terminal
{
    /// <summary>
    /// Classe Modelo TeminalConsulta
    /// </summary>
    [DataContract]
    public class TerminalConsulta
    {
        /// <summary>
        /// Terminal
        /// </summary>
        [DataMember]
        public Terminal Terminal { get; set; }

        /// <summary>
        /// Ordem serviço
        /// </summary>
        [DataMember]
        public OSAtendimentoBase OrdemServico { get; set; }

        /// <summary>
        /// Ponto venda
        /// </summary>
        [DataMember]
        public String PontoVenda { get; set; }
    }
}
