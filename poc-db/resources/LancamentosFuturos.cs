using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Redecard.PN.Extrato.Servicos.Modelo;
using DTO = Redecard.PN.Extrato.Modelo.LancamentosFuturos;
using AutoMapper;
using System.ComponentModel;

namespace Redecard.PN.Extrato.Servicos.LancamentosFuturos
{
    #region [ Lançamentos Futuros - Crédito - Totalizadores - WACA1324 / WA1324 / ISHO ]

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos totalizadores do Relatório de Lançamentos Futuros.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1324 / Programa WA1324 / TranID ISHO
    /// </remarks>
    [DataContract]
    public class CreditoTotalizador : BasicContract
    {
        /// <summary>Totais por Bandeira</summary>
        [DataMember]
        public List<CreditoTotalizadorValor> Valores { get; set; }

        /// <summary>Totais no Período</summary>
        [DataMember]
        public CreditoTotalizadorTotais Totais { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static CreditoTotalizador FromDTO(DTO.CreditoTotalizador dto)
        {
            Mapper.CreateMap<DTO.CreditoTotalizador, CreditoTotalizador>();
            Mapper.CreateMap<DTO.CreditoTotalizadorTotais, CreditoTotalizadorTotais>();
            Mapper.CreateMap<DTO.CreditoTotalizadorValor, CreditoTotalizadorValor>();
            return Mapper.Map<CreditoTotalizador>(dto);
        }
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos totalizadores do Relatório de Lançamentos Futuros.<br/>
    /// Representa os Totais por Bandeira.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1324 / Programa WA1324 / TranID ISHO
    /// </remarks>
    [DataContract]
    public class CreditoTotalizadorValor : BasicContract
    {        
        /// <summary>Descrição da bandeira</summary>
        [DataMember]
        public String TipoBandeira { get; set; }

        /// <summary>Valor líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }

        /// <summary>Código da bandeira</summary>
        [DataMember]
        public Int32 CodigoBandeira { get; set; }
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos totalizadores do Relatório de Lançamentos Futuros.<br/>
    /// Representa os Totais no Período.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1324 / Programa WA1324 / TranID ISHO
    /// </remarks>
    [DataContract]
    public class CreditoTotalizadorTotais : BasicContract
    {        
        /// <summary>Valor líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }
    }

    #endregion

    #region [ Lançamentos Futuros - Crédito - WACA1325 / WA1325 / ISHP ]

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Lançamentos Futuros.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1325 / Programa WA1325 / TranID ISHP
    /// </remarks>
    [DataContract]
    public class Credito : BasicContract
    {        
        /// <summary>Data de vencimento</summary>
        [DataMember]
        public DateTime DataVencimento { get; set; }

        /// <summary>Código da bandeira</summary>
        [DataMember]
        public Int32 CodigoBandeira { get; set; }

        /// <summary>Descrição da bandeira</summary>
        [DataMember]
        public String TipoBandeira { get; set; }

        /// <summary>Valor líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<Credito> FromDTO(List<DTO.Credito> dto)
        {
            Mapper.CreateMap<DTO.Credito, Credito>();
            return Mapper.Map<List<Credito>>(dto);
        }
    }

    #endregion

    #region [ Lançamentos Futuros - Crédito - Detalhe - Totalizadores - WACA1326 / WA1326 / ISHQ ]

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos totalizadores do Relatório de Lançamentos Futuros - Crédito - Detalhe.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1326 / Programa WA1326 / TranID ISHQ
    /// </remarks>
    [DataContract]
    public class CreditoDetalheTotalizador : BasicContract
    {
        /// <summary>Totais por Bandeira</summary>
        [DataMember]
        public List<CreditoDetalheTotalizadorValor> Valores { get; set; }

        /// <summary>Totais no Período</summary>
        [DataMember]
        public CreditoDetalheTotalizadorTotais Totais { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static CreditoDetalheTotalizador FromDTO(DTO.CreditoDetalheTotalizador dto)
        {
            Mapper.CreateMap<DTO.CreditoDetalheTotalizador, CreditoDetalheTotalizador>();
            Mapper.CreateMap<DTO.CreditoDetalheTotalizadorTotais, CreditoDetalheTotalizadorTotais>();
            Mapper.CreateMap<DTO.CreditoDetalheTotalizadorValor, CreditoDetalheTotalizadorValor>();
            return Mapper.Map<CreditoDetalheTotalizador>(dto);
        }
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos totalizadores do Relatório de Lançamentos Futuros - Crédito - Detalhe.<br/>
    /// Representa os Totais por Bandeira.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1326 / Programa WA1326 / TranID ISHQ
    /// </remarks>
    [DataContract]
    public class CreditoDetalheTotalizadorValor : BasicContract
    {        
        /// <summary>Descrição da bandeira</summary>
        [DataMember]
        public String TipoBandeira { get; set; }

        /// <summary>Valor bruto</summary>
        [DataMember]
        public Decimal ValorBruto { get; set; }

        /// <summary>Valor líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }

        /// <summary>Código da bandeira</summary>
        [DataMember]
        public Int32 CodigoBandeira { get; set; }

        /// <summary>Valor descontado</summary>
        [DataMember]
        public Decimal ValorDescontado { get; set; }
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos totalizadores do Relatório de Lançamentos Futuros - Crédito - Detalhe.<br/>
    /// Representa os Totais no Período.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1326 / Programa WA1326 / TranID ISHQ
    /// </remarks>
    [DataContract]
    public class CreditoDetalheTotalizadorTotais : BasicContract
    {        
        /// <summary>Valor bruto total</summary>
        [DataMember]
        public Decimal TotalValorBrutoVenda { get; set; }

        /// <summary>Valor líquido total</summary>
        [DataMember]
        public Decimal TotalValorLiquido { get; set; }

        /// <summary>Valor descontado total</summary>
        [DataMember]
        public Decimal TotalValorDescontado { get; set; }
    }

    #endregion

    #region [ Lançamentos Futuros - Crédito - Detalhe - WACA1327 / WA1327 / ISHR ]

    /// <summary>
    /// Enumeração auxiliar utilizada para customizar o retorno dos registros dos serviços
    /// de consulta do relatório de Lançamentos Futuros - Crédito - Detalhe.
    /// </summary>
    [DataContract]
    public enum CreditoDetalheTipoRegistro
    {
        /// <summary>Todos os registros</summary>
        [EnumMember]
        Todos = 0,
        /// <summary>Detalhe - DT</summary>
        [EnumMember, Description("DT")]
        Detalhe = 1,
        /// <summary>Ajuste com Valor - A1</summary>
        [EnumMember, Description("A1")]
        AjusteComValor = 2,
        /// <summary>Ajuste sem Valor - A2</summary>
        [EnumMember, Description("A2")]
        AjusteSemValor = 3
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Lançamentos Futuros - Crédito - Detalhe.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1327 / Programa WA1327 / TranID ISHR
    /// </remarks>
    [DataContract]
    public class CreditoDetalhe : BasicContract
    {        
        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<CreditoDetalhe> FromDTO(List<DTO.CreditoDetalhe> dto)
        {
            Mapper.CreateMap<DTO.CreditoDetalhe, CreditoDetalhe>()
                .Include<DTO.CreditoDetalheA1, CreditoDetalheA1>()
                .Include<DTO.CreditoDetalheA2, CreditoDetalheA2>()
                .Include<DTO.CreditoDetalheDT, CreditoDetalheDT>();
            Mapper.CreateMap<DTO.CreditoDetalheA1, CreditoDetalheA1>();
            Mapper.CreateMap<DTO.CreditoDetalheA2, CreditoDetalheA2>();
            Mapper.CreateMap<DTO.CreditoDetalheDT, CreditoDetalheDT>();
            return Mapper.Map<List<CreditoDetalhe>>(dto);
        }
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Lançamentos Futuros - Crédito - Detalhe.<br/>
    /// Representa os Registros de Detalhamento.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1327 / Programa WA1327 / TranID ISHR
    /// </remarks>
    [DataContract]
    public class CreditoDetalheDT : CreditoDetalhe
    {
        /// <summary>Data da venda</summary>
        [DataMember]
        public DateTime DataVenda { get; set; }

        /// <summary>Data de vencimento</summary>
        [DataMember]
        public DateTime DataVencimento { get; set; }

        /// <summary>Prazo de recebimento</summary>
        [DataMember]
        public Int32 PrazoRecebimento { get; set; }

        /// <summary>Número do estabelecimento</summary>
        [DataMember]
        public Int32 NumeroPV { get; set; }

        /// <summary>Número do resumo de vendas</summary>
        [DataMember]
        public Int32 NumeroResumo { get; set; }

        /// <summary>Status da OC</summary>
        [DataMember]
        public String StatusOc { get; set; }

        /// <summary>Quantidade de transações no Resumo de Vendas</summary>
        [DataMember]
        public Int32 QuantidadeTransacoesRV { get; set; }

        /// <summary>Descrição da bandeira</summary>
        [DataMember]
        public String Bandeira { get; set; }

        /// <summary>Descrição do resumo de vendas</summary>
        [DataMember]
        public String DescricaoResumo { get; set; }

        /// <summary>Valor bruto apresentação</summary>
        [DataMember]
        public Decimal ValorApresentacaoBruto { get; set; }

        /// <summary>Valor desconto</summary>
        [DataMember]
        public Decimal ValorDesconto { get; set; }

        /// <summary>Valor líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }

        /// <summary>Indicador Débito/Crédito (D/C)</summary>
        [DataMember]
        public String IndicadorSinalValor { get; set; }

        /// <summary>Código da bandeira</summary>
        [DataMember]
        public Int32 CodigoBandeira { get; set; }
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Lançamentos Futuros - Crédito - Detalhe.<br/>
    /// Representa os Registros de Ajuste com Valor.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1327 / Programa WA1327 / TranID ISHR
    /// </remarks>
    [DataContract]
    public class CreditoDetalheA1 : CreditoDetalhe
    {
        /// <summary>Chave linha detalhe</summary>
        [DataMember]
        public String ChaveLinhaDetalhe { get; set; }

        /// <summary>Descrição da bandeira</summary>
        [DataMember]
        public String Bandeira { get; set; }

        /// <summary>Descrição da compensação</summary>
        [DataMember]
        public String DescricaoCompensacao { get; set; }

        /// <summary>Valor total da apresentação</summary>
        [DataMember]
        public Decimal TotalValorApresentacao { get; set; }

        /// <summary>Valor total do desconto</summary>
        [DataMember]
        public Decimal TotalValorDesconto { get; set; }

        /// <summary>Valor líquido total</summary>
        [DataMember]
        public Decimal TotalValorLiquido { get; set; }

        /// <summary>Indicador Débito/Crédito - (D/C)</summary>
        [DataMember]
        public String IndicadorSinalValor { get; set; }

        /// <summary>Código da bandeira</summary>
        [DataMember]
        public Int32 CodigoBandeira { get; set; }
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Lançamentos Futuros - Crédito - Detalhe.<br/>
    /// Representa os registros de Ajuste sem Valor.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1327 / Programa WA1327 / TranID ISHR
    /// </remarks>
    [DataContract]
    public class CreditoDetalheA2 : CreditoDetalhe
    {
        /// <summary>Chave linha detalhe</summary>
        [DataMember]
        public String ChaveLinhaDetalhe { get; set; }

        /// <summary>Descrição da bandeira</summary>
        [DataMember]
        public String Bandeira { get; set; }

        /// <summary>Descrição da compensação</summary>
        [DataMember]
        public String DescricaoCompensacao { get; set; }

        /// <summary>Valor total da apresentação</summary>
        [DataMember]
        public Decimal TotalValorApresentacao { get; set; }

        /// <summary>Valor total do desconto</summary>
        [DataMember]
        public Decimal TotalValorDesconto { get; set; }

        /// <summary>Valor líquido total</summary>
        [DataMember]
        public Decimal TotalValorLiquido { get; set; }

        /// <summary>Indicador Débito/Crédito - (D/C)</summary>
        [DataMember]
        public String IndicadorSinalValor { get; set; }

        /// <summary>Código da bandeira</summary>
        [DataMember]
        public Int32 CodigoBandeira { get; set; }
    }

    #endregion

    #region [ Lançamentos Futuros - Débito - Totalizadores - WACA1328 / WA1328 / ISHS ]

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos totalizadores do Relatório de Lançamentos Futuros - Débito.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1328 / Programa WA1328 / TranID ISHS
    /// </remarks>
    [DataContract]
    public class DebitoTotalizador : BasicContract
    {
        /// <summary>Total por Bandeira</summary>
        [DataMember]
        public List<DebitoTotalizadorValor> Valores { get; set; }

        /// <summary>Total no Período</summary>
        [DataMember]
        public DebitoTotalizadorTotais Totais { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static DebitoTotalizador FromDTO(DTO.DebitoTotalizador dto)
        {
            Mapper.CreateMap<DTO.DebitoTotalizador, DebitoTotalizador>();
            Mapper.CreateMap<DTO.DebitoTotalizadorTotais, DebitoTotalizadorTotais>();
            Mapper.CreateMap<DTO.DebitoTotalizadorValor, DebitoTotalizadorValor>();
            return Mapper.Map<DebitoTotalizador>(dto);
        }
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos totalizadores do Relatório de Lançamentos Futuros - Débito.<br/>
    /// Representa os Totais por Bandeira.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1328 / Programa WA1328 / TranID ISHS
    /// </remarks>
    [DataContract]
    public class DebitoTotalizadorValor : BasicContract
    {        
        /// <summary>Descrição da bandeira</summary>
        [DataMember]
        public String TipoBandeira { get; set; }

        /// <summary>Valor da Venda</summary>
        [DataMember]
        public Decimal ValorVenda { get; set; }

        /// <summary>Valor Líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }

        /// <summary>Valor descontado</summary>
        [DataMember]
        public Decimal ValorDescontado { get; set; }
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos totalizadores do Relatório de Lançamentos Futuros - Débito.<br/>
    /// Representa os Totais no Período.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1328 / Programa WA1328 / TranID ISHS
    /// </remarks>
    [DataContract]
    public class DebitoTotalizadorTotais : BasicContract
    {        
        /// <summary>Valor total da venda</summary>
        [DataMember]
        public Decimal TotalValorVenda { get; set; }

        /// <summary>Valor líquido total</summary>
        [DataMember]
        public Decimal TotalValorLiquido { get; set; }

        /// <summary>Valor descontado total</summary>
        [DataMember]
        public Decimal TotalValorDescontado { get; set; }
    }

    #endregion

    #region [ Lançamentos Futuros - Débito - WACA1329 / WA1329 / ISHT ]
    
    /// <summary>
    /// Enumeração auxiliar utilizada para customizar o retorno dos registros dos serviços
    /// de consulta do relatório de Lançamentos Futuros - Débito.
    /// </summary>
    [DataContract]
    public enum DebitoTipoRegistro
    {
        /// <summary>Todos</summary>
        [EnumMember]
        Todos = 0,
        /// <summary>Detalhe - DT</summary>
        [EnumMember, Description("DT")]
        Detalhe = 1,
        /// <summary>Ajuste Com Valor - A1</summary>
        [EnumMember, Description("A1")]
        AjusteComValor = 2,
        /// <summary>Ajuste Sem Valor - A2</summary>
        [EnumMember, Description("A2")]
        AjusteSemValor = 3
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Lançamentos Futuros - Débito.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1329 / Programa WA1329 / TranID ISHT
    /// </remarks>
    [DataContract]
    public class Debito : BasicContract
    {        
        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<Debito> FromDTO(List<DTO.Debito> dto)
        {
            Mapper.CreateMap<DTO.Debito, Debito>()
                .Include<DTO.DebitoA1, DebitoA1>()
                .Include<DTO.DebitoA2, DebitoA2>()
                .Include<DTO.DebitoDT, DebitoDT>();
            Mapper.CreateMap<DTO.DebitoA1, DebitoA1>();
            Mapper.CreateMap<DTO.DebitoA2, DebitoA2>();
            Mapper.CreateMap<DTO.DebitoDT, DebitoDT>();
            return Mapper.Map<List<Debito>>(dto);
        }
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Lançamentos Futuros - Débito.<br/>
    /// Representa os registros de Detalhamento.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1329 / Programa WA1329 / TranID ISHT
    /// </remarks>
    [DataContract]
    public class DebitoDT : Debito
    {
        /// <summary>Data de vencimento</summary>
        [DataMember]
        public DateTime DataVencimento { get; set; }

        /// <summary>Número do estabelecimento</summary>
        [DataMember]
        public Int32 NumeroPV { get; set; }

        /// <summary>Data do Resumo de Venda</summary>
        [DataMember]
        public DateTime DataResumo { get; set; }

        /// <summary>Número do resumo de venda</summary>
        [DataMember]
        public Int32 NumeroResumo { get; set; }

        /// <summary>Quantidade de transações do Resumo de Venda</summary>
        [DataMember]
        public Int32 QuantidadeTransacoesRV { get; set; }

        /// <summary>Descrição da bandeira</summary>
        [DataMember]
        public String Bandeira { get; set; }

        /// <summary>Descrição do Resumo de Venda</summary>
        [DataMember]
        public String DescricaoResumo { get; set; }

        /// <summary>Valor da apresentação</summary>
        [DataMember]
        public Decimal ValorApresentacao { get; set; }

        /// <summary>Valor do desconto</summary>
        [DataMember]
        public Decimal ValorDesconto { get; set; }

        /// <summary>Valor líquido antecipado</summary>
        [DataMember]
        public Decimal ValorLiquidoAntecipado { get; set; }

        /// <summary>Indicador de Crédito/Débito - (C/D)</summary>
        [DataMember]
        public String IndicadorSinalValor { get; set; }

        /// <summary>Banco do Crédito</summary>
        [DataMember]
        public Int32 BancoCredito { get; set; }

        /// <summary>Agência do crédito</summary>
        [DataMember]
        public Int32 AgenciaCredito { get; set; }

        /// <summary>Conta do crédito</summary>
        [DataMember]
        public String ContaCredito { get; set; }
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Lançamentos Futuros - Débito.<br/>
    /// Representa os registros de Ajuste Com Valor.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1329 / Programa WA1329 / TranID ISHT
    /// </remarks>
    [DataContract]
    public class DebitoA1 : Debito
    {
        /// <summary>Chave detalhe</summary>
        [DataMember]
        public String ChaveDetalhe { get; set; }

        /// <summary>Descrição da bandeira</summary>
        [DataMember]
        public String Bandeira { get; set; }
        
        /// <summary>Descrição total diário bandeira</summary>
        [DataMember]
        public String DescricaoTotalDiarioBandeira { get; set; }

        /// <summary>Total do valor total do resumo</summary>
        [DataMember]
        public Decimal TotalValorTotalResumo { get; set; }

        /// <summary>Valor total do desconto</summary>
        [DataMember]
        public Decimal TotalValorDesconto { get; set; }

        /// <summary>Valor líquido antecipado total</summary>
        [DataMember]
        public Decimal TotalValorLiquidoAntecipado { get; set; }

        /// <summary>Indicador de Crédito/Débito (C/D)</summary>
        [DataMember]
        public String IndicadorSinalValor { get; set; }

        /// <summary>Banco do crédito</summary>
        [DataMember]
        public Int32 BancoCredito { get; set; }

        /// <summary>Agência do crédito</summary>
        [DataMember]
        public Int32 AgenciaCredito { get; set; }

        /// <summary>Conta do crédito</summary>
        [DataMember]
        public String ContaCredito { get; set; }
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para consulta dos registros do Relatório de Lançamentos Futuros - Débito.<br/>
    /// Representa os registros de Ajuste Sem Valor.
    /// </summary>
    /// <remarks>
    /// Encapsula os registros retornados pelo Serviço HIS do Book:<br/>
    /// - Book WACA1329 / Programa WA1329 / TranID ISHT
    /// </remarks>
    [DataContract]
    public class DebitoA2 : Debito
    {
        /// <summary>Chave detalhe</summary>
        [DataMember]
        public String ChaveDetalhe { get; set; }

        /// <summary>Descrição da bandeira</summary>
        [DataMember]
        public String Bandeira { get; set; }

        /// <summary>Descrição total diário bandeira</summary>
        [DataMember]
        public String DescricaoTotalDiarioBandeira { get; set; }

        /// <summary>Valor total do resumo</summary>
        [DataMember]
        public Decimal TotalValorTotalResumo { get; set; }

        /// <summary>Valor total do desconto</summary>
        [DataMember]
        public Decimal TotalValorDesconto { get; set; }

        /// <summary>Valor líquido antecipado total</summary>
        [DataMember]
        public Decimal TotalValorLiquidoAntecipado { get; set; }

        /// <summary>Indicador de Crédito/Débito (C/D)</summary>
        [DataMember]
        public String IndicadorSinalValor { get; set; }

        /// <summary>Banco do crédito</summary>
        [DataMember]
        public Int32 BancoCredito { get; set; }

        /// <summary>Agência do crédito</summary>
        [DataMember]
        public Int32 AgenciaCredito { get; set; }

        /// <summary>Conta do crédito</summary>
        [DataMember]
        public String ContaCredito { get; set; }
    }

    #endregion
}