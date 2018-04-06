/*
© Copyright 2017 Redecard S.A.
Autor : MNE
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.Terminal
{
    /// <summary>
    /// Classe Modelo TerminalConsulta
    /// </summary>
    [DataContract]
    public class TerminalConsultaRest
    {
        /// <summary>
        /// Terminal
        /// </summary>
        [DataMember]
        public TerminalRest Terminal { get; set; }

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
