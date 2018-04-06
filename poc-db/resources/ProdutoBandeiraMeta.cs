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
    /// Classe Modelo de Produtos Bandeira da Oferta com suas Taxas
    /// </summary>
    public class ProdutoBandeiraMeta
    {
        /// <summary>
        /// Código da Bandeira
        /// </summary>
        public Int32 CodigoBandeira { get; set; }

        /// <summary>
        /// Nome da Bandeira
        /// </summary>
        public String DescricaoBandeira { get; set; }

        /// <summary>
        /// Taxas da Bandeira
        /// </summary>
        public List<TaxaMeta> Taxas { get; set; }
    }
}
