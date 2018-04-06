/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;

namespace Redecard.PN.Extrato.Modelo.Vendas
{
    /// <summary>
    /// Classe modelo para Relatório de Vendas - Débito Totalizadores
    /// </summary>
    public class DebitoTotalizador
    {
        /// <summary>
        /// Quantidade de Registros
        /// </summary>
        public Int32 QuantidadeRegistros { get; set; }

        /// <summary>
        /// Lista de Registros
        /// </summary>
        public List<DebitoTotalizadorValor> Valores { get; set; }

        /// <summary>
        /// Tipo de Registro
        /// </summary>
        public String TipoRegistro { get; set; }

        /// <summary>
        /// Valor Apresentado
        /// </summary>
        public Decimal ValorApresentado { get; set; }

        /// <summary>
        /// Valor Líquido
        /// </summary>
        public Decimal ValorLiquido { get; set; }

        /// <summary>
        /// Valor Descontado
        /// </summary>
        public Decimal ValorDescontado { get; set; }
    }
}