/*
© Copyright 2014 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.GerencieExtrato.Modelo
{
    /// <summary>
    /// Turquia - Extrato Relatório Preço Único
    /// </summary>
    public class ExtratoRelatorioPrecoUnico : ExtratoBase
    {
        /// <summary>
        /// Flag utilizado para solicitar o relatório detalhado.
        /// </summary>
        public Int16 FlagVsam { get; set; }
    }
}
