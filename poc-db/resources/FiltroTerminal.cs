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
    /// Classe Modelo FiltroTerminal
    /// </summary>
    [DataContract]
    public class FiltroTerminal
    {
        /// <summary>
        /// Número Lógico
        /// </summary>
        [DataMember]
        public String NumeroLogico { get; set; }

        /// <summary>
        /// Ponto venda
        /// </summary>
        [DataMember]
        public String PontoVenda { get; set; }

        /// <summary>
        /// Situação
        /// </summary>
        [DataMember]
        public TipoTerminalStatus? Situacao { get; set; }

        /// <summary>
        /// Família equipamento
        /// </summary>
        [DataMember]
        public String FamiliaEquipamento { get; set; }

        /// <summary>
        /// Tipo equipamento
        /// </summary>
        [DataMember]
        public String TipoEquipamento { get; set; }
    }
}
