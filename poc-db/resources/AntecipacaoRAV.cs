using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Redecard.PN.Extrato.Modelo.AntecipacaoRAV
{
    #region [ Antecipação RAV - Totalizadores - WACA1330 / WA1330 / ISHU ]

    public class RAVTotalizador
    {
        public List<RAVTotalizadorValor> Valores { get; set; }
        public RAVTotalizadorTotais Totais { get; set; }

        public RAVTotalizador()
        {
            this.Valores = new List<RAVTotalizadorValor>();
            this.Totais = new RAVTotalizadorTotais();
        }
    }

    public class RAVTotalizadorValor
    {
        public String TipoRegistro { get; set; }
        public String TipoBandeira { get; set; }
        public Decimal ValorLiquido { get; set; }

        /// <summary>Código da bandeira</summary>
        public Int32 CodigoBandeira { get; set; }
    }

    public class RAVTotalizadorTotais
    {
        public String TipoRegistro { get; set; }
        public Decimal TotalValorLiquido { get; set; }
    }

    #endregion

    #region [ Antecipação RAV - WACA1331 / WA1331 / ISHV ]

    public class RAV
    {
        public String TipoRegistro { get; set; }
        public DateTime DataAntecipacao { get; set; }
        public String TipoBandeira { get; set; }
        public Decimal ValorAntecipacao { get; set; }
        public Int32 CodigoBandeira { get; set; }
    }

    #endregion

    #region [ Antecipação RAV - Detalhe - Totalizadores - WACA1332 / WA1332 / ISHX ]

    public class RAVDetalheTotalizador
    {
        public List<RAVDetalheTotalizadorValor> Valores { get; set; }
        public RAVDetalheTotalizadorTotais Totais { get; set; }

        public RAVDetalheTotalizador()
        {
            this.Valores = new List<RAVDetalheTotalizadorValor>();
            this.Totais = new RAVDetalheTotalizadorTotais();
        }
    }

    public class RAVDetalheTotalizadorValor
    {
        public String TipoRegistro { get; set; }
        public String TipoBandeira { get; set; }
        public Decimal ValorBruto { get; set; }
        public Decimal ValorLiquido { get; set; }
        public Int32 CodigoBandeira { get; set; }
    }

    public class RAVDetalheTotalizadorTotais
    {
        public String TipoRegistro { get; set; }
        public Decimal TotalValorLiquido { get; set; }
        public Decimal TotalValorBruto { get; set; }
        public Decimal TotalValorDisponivel { get; set; }
    }

    #endregion

    #region [ Antecipação RAV - Detalhe - WACA1333 / WA1333 / ISHY ]

    [XmlInclude(typeof(RAVDetalheA1)),
    XmlInclude(typeof(RAVDetalheA2)),
    XmlInclude(typeof(RAVDetalheDT))]
    public class RAVDetalhe
    {
        public String TipoRegistro { get; set; }
    }

    public class RAVDetalheDT : RAVDetalhe
    {
        public DateTime DataAntecipacao { get; set; }
        public DateTime DataApresentacao { get; set; }
        public DateTime DataVencimento { get; set; }
        public Int32 NumeroPV { get; set; }
        public Int32 NumeroResumo { get; set; }
        public String StatusOc { get; set; }
        public Int32 QuantidadeTransacoesRV { get; set; }
        public String Bandeira { get; set; }
        public String DescricaoResumo { get; set; }
        public Decimal ValorTotalResumo { get; set; }
        public Decimal ValorDisponivelRAV { get; set; }
        public Decimal ValorDesconto { get; set; }
        public Decimal ValorLiquidoAntecipado { get; set; }
        public String IndicadorSinalValor { get; set; }
        public Int32 BancoCredito { get; set; }
        public Int32 AgenciaCredito { get; set; }
        public String ContaCredito { get; set; }
        public Int32 CodigoBandeira { get; set; }
        public String TmsOc { get; set; }
        public Int16 CodigoProdutoAntecipacao { get; set; }
        public String DescricaoCessaoCredito { get; set; }
    }

    public class RAVDetalheA1 : RAVDetalhe
    {
        public String Bandeira { get; set; }
        public String DescricaoTotalDiarioBandeira { get; set; }
        public Decimal TotalValorTotalResumo { get; set; }
        public Decimal TotalValorDisponivelRAV { get; set; }
        public Decimal TotalValorDesconto { get; set; }
        public Decimal TotalValorLiquidoAntecipado { get; set; }
        public String IndicadorSinalValor { get; set; }
        public Int32 BancoCredito { get; set; }
        public Int32 AgenciaCredito { get; set; }
        public String ContaCredito { get; set; }
        public Int32 CodigoBandeira { get; set; }
        public String TmsOcd { get; set; }
        public Int16 CodigoProdutoAntecipacao { get; set; }
        public String DescricaoCessaoCredito { get; set; }
    }

    public class RAVDetalheA2 : RAVDetalhe
    {
        public String Bandeira { get; set; }
        public String DescricaoTotalDiarioBandeira { get; set; }
        public Decimal TotalValorTotalResumo { get; set; }
        public Decimal TotalValorDisponivelRAV { get; set; }
        public Decimal TotalValorDesconto { get; set; }
        public Decimal TotalValorLiquidoAntecipado { get; set; }
        public String IndicadorSinalValor { get; set; }
        public Int32 BancoCredito { get; set; }
        public Int32 AgenciaCredito { get; set; }
        public String ContaCredito { get; set; }
        public Int32 CodigoBandeira { get; set; }
        public String TmsOcd { get; set; }
        public Int16 CodigoProdutoAntecipacao { get; set; }
        public String DescricaoCessaoCredito { get; set; }
    }

    #endregion
}
