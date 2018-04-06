/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrosServicos.Modelo
{
    /// <summary>
    /// Classe Modelo de Faixas das Metas de Oferta
    /// </summary>
    public class FaixaMetaOferta
    {
        /// <summary>
        /// Código da Faixa
        /// </summary>
        public Int64 Codigo { get; set; }

        /// <summary>
        /// Descrição da Faixa
        /// </summary>
        public String DescricaoFaixaMeta { get; set; }

        /// <summary>
        /// Valor inicial da Faixa
        /// </summary>
        public Double? ValorInicial { get; set; }

        /// <summary>
        /// Valor final da Faixa
        /// </summary>
        public Double? ValorFinal { get; set; }
    }
}
