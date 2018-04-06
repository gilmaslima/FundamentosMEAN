﻿/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.OrdemServico
{
    /// <summary>
    /// Classe Modelo OSTerminal
    /// </summary>
    [DataContract]
    public class OSTerminal
    {
        /// <summary>
        /// Ação
        /// </summary>
        [DataMember]
        public TipoAcaoTerminal? Acao { get; set; }

        /// <summary>
        /// Número lógico
        /// </summary>
        [DataMember]
        public String NumeroLogico { get; set; }

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

        /// <summary>
        /// Lacrado
        /// </summary>
        [DataMember]
        public Boolean? Lacrado { get; set; }

        /// <summary>
        /// Renpac
        /// </summary>
        [DataMember]
        public String Renpac { get; set; }

        /// <summary>
        /// Venda digitada
        /// </summary>
        [DataMember]
        public VendaDigitadaTerminal VendaDigitada { get; set; }

        /// <summary>
        /// Quantidade checkout
        /// </summary>
        [DataMember]
        public Int32? QuantidadeCheckout { get; set; }

        /// <summary>
        /// Software house
        /// </summary>
        [DataMember]
        public String SoftwareHouse { get; set; }

        /// <summary>
        /// Integrador
        /// </summary>
        [DataMember]
        public String Integrador { get; set; }
    }
}
