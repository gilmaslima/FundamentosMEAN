/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Collections.Generic;

namespace Redecard.PN.Extrato.Modelo.HomePage
{
    /// <summary>
    /// Vendas a Crédito
    /// </summary>
    public class VendasCredito
    { 
        /// <summary>Vendas por Bandeira</summary>
        public List<Vendas> Vendas { get; set; }

        /// <summary>Total de vendas das Outras Bandeiras</summary>
        public Vendas OutrasBandeiras { get; set; }

        /// <summary>Total de vendas no período de todas as Bandeiras</summary>
        public Vendas Totalizador { get; set; }        
    }
}