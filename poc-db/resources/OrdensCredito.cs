using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Redecard.PN.Extrato.Modelo.OrdensCredito
{
    #region [ Ordens de Crédito - Totalizadores - WACA1334 / WA1334 / ISHZ ]

    public class CreditoTotalizador
    {
        public List<CreditoTotalizadorValor> Valores { get; set; }
        public CreditoTotalizadorTotais Totais { get; set; }

        public CreditoTotalizador()
        {
            this.Valores = new List<CreditoTotalizadorValor>();
            this.Totais = new CreditoTotalizadorTotais();
        }
    }

    public class CreditoTotalizadorValor
    {
        public String TipoRegistro { get; set; }
        public String TipoBandeira { get; set; }
        public Decimal ValorLiquido { get; set; }

        /// <summary>Código da bandeira</summary>
        public Int32 CodigoBandeira { get; set; }
    }

    public class CreditoTotalizadorTotais
    {
        public String TipoRegistro { get; set; }
        public Decimal TotalValorLiquido { get; set; }
    }

    #endregion

    #region [ Ordens de Crédito - WACA1335 / WA1335 / ISHW ]

    public class Credito
    {
        public String TipoRegistro { get; set; }
        public DateTime DataEmissao { get; set; }
        public String TipoBandeira { get; set; }
        public Decimal ValorOc { get; set; }
        public Int32 CodigoBandeira { get; set; }
    }

    #endregion

    #region [ Ordens de Crédito Detalhe - Totalizadores - WACA1336 / WA1336 / ISH0 ]

    public class CreditoDetalheTotalizador
    {
        public List<CreditoDetalheTotalizadorValor> Valores { get; set; }
        public CreditoDetalheTotalizadorTotais Totais { get; set; }

        public CreditoDetalheTotalizador()
        {
            this.Valores = new List<CreditoDetalheTotalizadorValor>();
            this.Totais = new CreditoDetalheTotalizadorTotais();
        }
    }

    public class CreditoDetalheTotalizadorValor
    {
        public String TipoRegistro { get; set; }
        public String TipoBandeira { get; set; }
        public Decimal ValorLiquido { get; set; }
        public Decimal ValorBruto { get; set; }
    }

    public class CreditoDetalheTotalizadorTotais
    {
        public String TipoRegistro { get; set; }
        public Decimal TotalValorLiquido { get; set; }
        public Decimal TotalValorBruto { get; set; }
    }

    #endregion

    #region [ Ordens de Crédito Detalhe - WACA1337 / WA1337 / ISH1 ]

    [XmlInclude(typeof(CreditoDetalheDT)),
    XmlInclude(typeof(CreditoDetalheD1))]
    public class CreditoDetalhe
    {
        public String TipoRegistro { get; set; }
    }

    public class CreditoDetalheDT : CreditoDetalhe
    {
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public Int32 NumeroPV { get; set; }
        public Int32 NumeroResumo { get; set; }
        public String TipoBandeira { get; set; }
        public String StatusOc { get; set; }
        public String DescricaoResumoAjuste { get; set; }
        public Decimal ValorCredito { get; set; }
        public String IndicadorSinalValor { get; set; }
        public Int32 BancoCredito { get; set; }
        public Int32 AgenciaCredito { get; set; }
        public String ContaCredito { get; set; }
        public Int32 Prazo { get; set; }
        public Decimal ValorBruto { get; set; }
        public Int32 QuantidadeCVs { get; set; }
    }

    public class CreditoDetalheD1 : CreditoDetalhe
    {
        public String TipoBandeira { get; set; }
        public String DescricaoAjuste { get; set; }        
    }

    #endregion
}
