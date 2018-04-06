using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using DTO = Redecard.PN.Extrato.Modelo.ConsultaTransacao;
using AutoMapper;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.ConsultaTransacao
{
    #region [ Consulta de Transação de Débito por Cartão/NSU - MEC084CO - MEC084 - IS89 ]

    /// <summary>
    /// Enumeração auxiliar utilizada pela classe Debito do serviço de Consulta de Transação de Débito por Cartão/NSU.
    /// </summary>
    /// <seealso cref="Redecard.PN.Extrato.Servicos.ConsultaTransacao.Debito">Classe Debito.</seealso>
    [DataContract]
    public enum DebitoTipoCancelamento
    {
        /// <summary>Não identificado</summary>
        [EnumMember]
        NaoIdentificado = -1,
        /// <summary>Não desagendado</summary>
        [EnumMember]
        NaoDesagendado = 0,
        /// <summary>Desagendado total</summary>
        [EnumMember]
        DesagendadoTotal = 1,
        /// <summary>Desagendado parcial</summary>
        [EnumMember]
        DesagendadoParcial = 2
    }

    /// <summary>
    /// Enumeração auxiliar utilizada pela classe Debito do serviço de Consulta de Transação de Débito por Cartão/NSU.
    /// </summary>
    /// <seealso cref="Redecard.PN.Extrato.Servicos.ConsultaTransacao.Debito">Classe Debito.</seealso>
    [DataContract]
    public enum DebitoCanal
    {
        /// <summary>Não identificado</summary>
        [EnumMember]
        NaoIdentificado = -1,
        /// <summary>Estabelecimento</summary>
        [EnumMember]
        Estabelecimento = 0,
        /// <summary>Emissor</summary>
        [EnumMember]
        Emissor = 1
    }

    /// <summary>
    /// Enumeração auxiliar utilizada pela classe Debito do serviço de Consulta de Transação de Débito por Cartão/NSU.
    /// </summary>
    /// <seealso cref="Redecard.PN.Extrato.Servicos.ConsultaTransacao.Debito">Classe Debito.</seealso>
    [DataContract]
    public enum DebitoOrigemDesagendamento
    {
        /// <summary>Não identificado</summary>
        [EnumMember]
        NaoIdentificado = -1,
        /// <summary>KC</summary>
        [EnumMember]
        KC = 0,
        /// <summary>XD</summary>
        [EnumMember]
        XD = 1
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para Consulta de Transação de Débito por Cartão/NSU.
    /// </summary>
    /// <remarks>
    /// Encapsula as informações retornadas pelo Serviço HIS do Book:<br/>
    /// - Book MEC084CO / Programa MEC084 / TranID IS89
    /// </remarks>
    [DataContract]
    public class Debito : BasicContract
    {
        /// <summary>Número Resumo Vendas</summary>
        [DataMember]
        public Int32 ResumoVenda { get; set; }

        /// <summary>Número ID Datacash</summary>
        [DataMember]
        public String IdDatacash { get; set; }

        /// <summary>Número do Cartão</summary>
        [DataMember]
        public String NumeroCartao { get; set; }

        /// <summary>Número NSU</summary>
        [DataMember]
        public Decimal NSU { get; set; }

        /// <summary>Número do Estabelecimento</summary>
        [DataMember]
        public Int32 NumeroPV { get; set; }

        /// <summary>Data da Transação</summary>
        [DataMember]
        public DateTime DataTransacao { get; set; }

        /// <summary>Data do Resumo</summary>
        [DataMember]
        public DateTime DataResumo { get; set; }

        /// <summary>Número de Parcelas</summary>
        [DataMember]
        public Int16 NumeroParcelas { get; set; }

        /// <summary>Valor da Transação</summary>
        [DataMember]
        public Decimal ValorTransacao { get; set; }

        /// <summary>Número da Autorização</summary>
        [DataMember]
        public String NumeroAutorizacaoBanco { get; set; }

        /// <summary>Código do Produto de Venda</summary>
        [DataMember]
        public String CodigoProdutoVenda { get; set; }

        /// <summary>Descrição do Produto de Venda</summary>
        [DataMember]
        public String DescricaoProdutoVenda { get; set; }

        /// <summary>ID do Cancelamento</summary>
        [DataMember]
        public Int16 IdCancelamento { get; set; }

        /// <summary>Descrição do Cancelamento</summary>
        [DataMember]
        public DebitoTipoCancelamento TipoCancelamento { get; set; }

        /// <summary>TimeStamp da Transação</summary>
        [DataMember]
        public String TimeStampTransacao { get; set; }

        /// <summary>Descrição da Bandeira</summary>
        [DataMember]
        public String DescricaoBandeira { get; set; }

        /// <summary>Quantidade de Cancelamentos</summary>
        [DataMember]
        public Int16 QuantidadeCancelamento { get; set; }

        /// <summary>Lista de Cancelamentos</summary>
        [DataMember]
        public List<CancelamentoDebito> Cancelamentos { get; set; }

        /// <summary>Indicador de Tokenizacao</summary>
        [DataMember]
        public String IndicadorTokenizacao { get; set; }


        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<Debito> FromDTO(List<DTO.Debito> dto)
        {
            Mapper.CreateMap<DTO.Debito, Debito>();
            Mapper.CreateMap<DTO.CancelamentoDebito, CancelamentoDebito>();
            Mapper.CreateMap<DTO.DebitoCanal, DebitoCanal>();
            Mapper.CreateMap<DTO.DebitoOrigemDesagendamento, DebitoOrigemDesagendamento>();
            Mapper.CreateMap<DTO.DebitoTipoCancelamento, DebitoTipoCancelamento>();            
            return Mapper.Map<List<Debito>>(dto);
        }
    }

    /// <summary>
    /// Classe auxiliar utilizada pela classe Debito do serviço de Consulta de Transação de Débito por Cartão/NSU.
    /// </summary>
    /// <seealso cref="Redecard.PN.Extrato.Servicos.ConsultaTransacao.Debito">Classe Debito.</seealso>
    [DataContract]
    public class CancelamentoDebito : BasicContract
    {
        /// <summary>Número do Aviso/Processo</summary>
        [DataMember]
        public Decimal NumeroAvisoProcesso { get; set; }

        /// <summary>Data do Cancelamento</summary>
        [DataMember]
        public DateTime DataCancelamento { get; set; }

        /// <summary>Código Canal</summary>
        [DataMember]
        public String CodigoCanal { get; set; }

        /// <summary>Canal</summary>
        [DataMember]
        public DebitoCanal Canal { get; set; }

        /// <summary>ID Origem do Desagendamento</summary>
        [DataMember]
        public String IdOrigemDesagendamento { get; set; }

        /// <summary>Origem do Desagendamento</summary>
        [DataMember]
        public DebitoOrigemDesagendamento OrigemDesagendamento { get; set; }
    }

    #endregion

    #region [ Consulta de Transação de Crédito por Cartão/NSU - MEC119CO / MEC119 / IS96 ]

    /// <summary>
    /// Enumeração auxiliar utilizada pela classe Credito do serviço de Consulta de Transação de Crédito por Cartão/NSU.
    /// </summary>
    /// <seealso cref="Redecard.PN.Extrato.Servicos.ConsultaTransacao.Credito">Classe Credito.</seealso>
    [DataContract]
    public enum CreditoTipoCancelamento
    {
        /// <summary>Não identificado</summary>
        [EnumMember]
        NaoIdentificado = -1,
        /// <summary>Saldo disponível</summary>
        [EnumMember]
        SaldoDisponivel = 0,
        /// <summary>Cancelada total</summary>
        [EnumMember]
        CanceladaTotal = 1,
        /// <summary>Cancelada total no dia</summary>
        [EnumMember]
        CanceladaTotalNoDia = 2
    }

    /// <summary>
    /// Enumeração auxiliar utilizada pela classe Credito do serviço de Consulta de Transação de Crédito por Cartão/NSU.
    /// </summary>
    /// <seealso cref="Redecard.PN.Extrato.Servicos.ConsultaTransacao.Credito">Classe Credito.</seealso>
    [DataContract]
    public enum CreditoMotivoCancelamento
    {
        /// <summary>Não identificado</summary>
        [EnumMember]
        NaoIdentificado = -1,
        /// <summary>Cancelamento</summary>
        [EnumMember]
        Cancelamento = 18,
        /// <summary>Chageback</summary>
        [EnumMember]
        ChargeBack = 23
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para Consulta de Transação de Crédito por Cartão/NSU.
    /// </summary>
    /// <remarks>
    /// Encapsula as informações retornadas pelo Serviço HIS do Book:<br/>
    /// - Book MEC119CO / Programa MEC119 / TranID IS96
    /// </remarks>
    [DataContract]
    public class Credito : BasicContract
    {
        /// <summary>Número ID Datacash</summary>
        [DataMember]
        public String NumeroIdDatacash { get; set; }

        /// <summary>Número do Cartão</summary>
        [DataMember]
        public String NumeroCartao { get; set; }

        /// <summary>Data da Transação</summary>
        [DataMember]
        public DateTime DataTransacao { get; set; }

        /// <summary>Valor da Transação</summary>
        [DataMember]
        public Decimal ValorTransacao { get; set; }

        /// <summary>Código Autorização da Venda</summary>
        [DataMember]
        public String AutorizacaoVenda { get; set; }

        /// <summary>Número Resumo</summary>
        [DataMember]
        public Int32 NumeroResumoVendas { get; set; }

        /// <summary>Quantidade de Parcelas</summary>
        [DataMember]
        public Int16 QuantidadeParcelas { get; set; }

        /// <summary>Código do Produto</summary>
        [DataMember]
        public String CodigoProdutoVenda { get; set; }

        /// <summary>Descrição do produto de venda</summary>
        [DataMember]
        public String DescricaoProdutoVenda { get; set; }

        /// <summary>TimeStamp da Transação</summary>
        [DataMember]
        public String TimeStampTransacao { get; set; }

        /// <summary>Descrição da Bandeira</summary>
        [DataMember]
        public String DescricaoBandeira { get; set; }

        /// <summary>Quantidade de Cancelamentos</summary>
        [DataMember]
        public Int16 QuantidadeCancelamentos { get; set; }

        /// <summary>Data do Resumo</summary>
        [DataMember]
        public DateTime DataResumo { get; set; }

        /// <summary>Id do Cancelamento</summary>
        [DataMember]
        public Int32 IdCancelamento { get; set; }

        /// <summary>Tipo do Cancelamento</summary>
        [DataMember]
        public CreditoTipoCancelamento TipoCancelamento { get; set; }

        /// <summary>Lista de Cancelamentos</summary>
        [DataMember]
        public List<CancelamentoCredito> Cancelamentos { get; set; }

        /// <summary>Indicador de Tokenizacao</summary>
        [DataMember]
        public String IndicadorTokenizacao { get; set; }


        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<Credito> FromDTO(List<DTO.Credito> dto)
        {
            Mapper.CreateMap<DTO.Credito, Credito>();
            Mapper.CreateMap<DTO.CancelamentoCredito, CancelamentoCredito>();
            Mapper.CreateMap<DTO.CreditoMotivoCancelamento, CreditoMotivoCancelamento>();
            Mapper.CreateMap<DTO.CreditoTipoCancelamento, CreditoTipoCancelamento>();
            return Mapper.Map<List<Credito>>(dto);
        }
    }

    /// <summary>
    /// Classe auxiliar utilizada pela classe Credito do serviço de Consulta de Transação de Crédito por Cartão/NSU.
    /// </summary>
    /// <seealso cref="Redecard.PN.Extrato.Servicos.ConsultaTransacao.Credito">Classe Credito.</seealso>
    [DataContract]
    public class CancelamentoCredito : BasicContract
    {
        /// <summary>Data do Cancelamento</summary>
        [DataMember]
        public DateTime DataCancelamento { get; set; }

        /// <summary>Número do Aviso/Processo</summary>
        [DataMember]
        public Decimal NumeroAvisoProcesso { get; set; }

        /// <summary>Código do Motivo do Cancelamento</summary>
        [DataMember]
        public Int16 CodigoMotivoCancelamento { get; set; }

        /// <summary>Motivo do cancelamento</summary>
        [DataMember]
        public CreditoMotivoCancelamento MotivoCancelamento { get; set; }
    }

    #endregion

    #region [ Consulta de Transação de Débito por TID - MEC324CO / MEC324 / IS88 ]

    /// <summary>
    /// Enumeração auxiliar utilizada pela classe DebitoTID do serviço de Consulta de Transação de Débito por TID.
    /// </summary>
    /// <seealso cref="Redecard.PN.Extrato.Servicos.ConsultaTransacao.DebitoTID">Classe DebitoTID.</seealso>
    [DataContract]
    public enum DebitoTidTipoCancelamento
    {
        /// <summary>Não identificado</summary>
        [EnumMember]
        NaoIdentificado = -1,
        /// <summary>Não desagendado</summary>
        [EnumMember]
        NaoDesagendado = 0,
        /// <summary>Desagendado total</summary>
        [EnumMember]
        DesagendadoTotal = 1,
        /// <summary>Desagendado parcial</summary>
        [EnumMember]
        DesagendadoParcial = 2
    }

    /// <summary>
    /// Enumeração auxiliar utilizada pela classe DebitoTID do serviço de Consulta de Transação de Débito por TID.
    /// </summary>
    /// <seealso cref="Redecard.PN.Extrato.Servicos.ConsultaTransacao.DebitoTID">Classe DebitoTID.</seealso>
    [DataContract]
    public enum DebitoTidCanal
    {
        /// <summary>Não identificado</summary>
        [EnumMember]
        NaoIdentificado = -1,
        /// <summary>Estabelecimento</summary>
        [EnumMember]
        Estabelecimento = 0,
        /// <summary>Emissor</summary>
        [EnumMember]
        Emissor = 1
    }

    /// <summary>
    /// Enumeração auxiliar utilizada pela classe DebitoTID do serviço de Consulta de Transação de Débito por TID.
    /// </summary>
    /// <seealso cref="Redecard.PN.Extrato.Servicos.ConsultaTransacao.DebitoTID">Classe DebitoTID.</seealso>
    [DataContract]
    public enum DebitoTidOrigemDesagendamento
    {
        /// <summary>Não identificado</summary>
        [EnumMember]
        NaoIdentificado = -1,
        /// <summary>KC</summary>
        [EnumMember]
        KC = 0,
        /// <summary>XD</summary>
        [EnumMember]
        XD = 1
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para Consulta de Transação de Débito por TID.
    /// </summary>
    /// <remarks>
    /// Encapsula as informações retornadas pelo Serviço HIS do Book:<br/>
    /// - Book MEC324CO / Programa MEC324 / TranID IS88
    /// </remarks>
    [DataContract]
    public class DebitoTID : BasicContract
    {
        /// <summary>Número do Resumo de Vendas</summary>
        [DataMember]
        public Int32 NumeroResumoVendas { get; set; }

        /// <summary>Número TID</summary>
        [DataMember]
        public String NumeroIdDataCash { get; set; }

        /// <summary>Número do Cartão</summary>
        [DataMember]
        public String NumeroCartao { get; set; }

        /// <summary>Número NSU</summary>
        [DataMember]
        public Decimal NsuAquirer { get; set; }

        /// <summary>Número do Estabelecimentos</summary>
        [DataMember]
        public Int32 NumeroPV { get; set; }

        /// <summary>Data da Transação</summary>
        [DataMember]
        public DateTime DataTransacao { get; set; }

        /// <summary>Data do Resumo</summary>
        [DataMember]
        public DateTime DataResumo { get; set; }

        /// <summary>Quantidade de Parcelas</summary>
        [DataMember]
        public Int16 NumeroParcelas { get; set; }

        /// <summary>Valor da Transação</summary>
        [DataMember]
        public Decimal ValorTransacao { get; set; }

        /// <summary>Número da Autorização do Banco</summary>
        [DataMember]
        public String NumeroAutorizacaoBanco { get; set; }

        /// <summary>Código do Tipo de Produto</summary>
        [DataMember]
        public String CodigoProdutoVenda { get; set; }

        /// <summary>Descrição do Produto de Venda</summary>
        [DataMember]
        public String DescricaoProdutoVenda { get; set; }

        /// <summary>ID de Cancelamento</summary>
        [DataMember]
        public Int16 IdCancelamento { get; set; }

        /// <summary>Tipo do Cancelamento</summary>
        [DataMember]
        public DebitoTidTipoCancelamento TipoCancelamento { get; set; }

        /// <summary>Timestamp da Transação</summary>
        [DataMember]
        public String TimestampTransacao { get; set; }

        /// <summary>Bandeira</summary>
        [DataMember]
        public String Bandeira { get; set; }

        /// <summary>Indicador de Tokenizacao</summary>
        [DataMember]
        public String IndicadorTokenizacao { get; set; }

        /// <summary>Quantidade de Cancelamentos</summary>
        [DataMember]
        public Int16 QuantidadeCancelamento { get; set; }

        /// <summary>Lista de Cancelamentos</summary>
        [DataMember]
        public List<CancelamentoDebitoTID> Cancelamentos { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static DebitoTID FromDTO(DTO.DebitoTID dto)
        {
            Mapper.CreateMap<DTO.DebitoTID, DebitoTID>();
            Mapper.CreateMap<DTO.DebitoTidCanal, DebitoTidCanal>();
            Mapper.CreateMap<DTO.DebitoTidOrigemDesagendamento, DebitoTidOrigemDesagendamento>();
            Mapper.CreateMap<DTO.DebitoTidTipoCancelamento, DebitoTidTipoCancelamento>();
            Mapper.CreateMap<DTO.CancelamentoDebitoTID, CancelamentoDebitoTID>();
            return Mapper.Map<DebitoTID>(dto);
        }
    }

    /// <summary>
    /// Classe auxiliar utilizada pela classe Debito do serviço de Consulta de Transação de Débito por TID.
    /// </summary>
    /// <seealso cref="Redecard.PN.Extrato.Servicos.ConsultaTransacao.DebitoTID">Classe DebitoTID.</seealso>
    [DataContract]
    public class CancelamentoDebitoTID : BasicContract
    {
        /// <summary>Número do Aviso/Processo</summary>
        [DataMember]
        public Decimal NumeroAvisoProcesso { get; set; }

        /// <summary>Data do Cancelamento</summary>
        [DataMember]
        public DateTime DataCancelamento { get; set; }

        /// <summary>Código Canal</summary>
        [DataMember]
        public String CodigoCanal { get; set; }

        /// <summary>Canal</summary>
        [DataMember]
        public DebitoTidCanal Canal { get; set; }

        /// <summary>ID Origem do Desagendamento</summary>
        [DataMember]
        public String IdOrigemDesagendamento { get; set; }

        /// <summary>Origem do Desagendamento</summary>
        [DataMember]
        public DebitoTidOrigemDesagendamento OrigemDesagendamento { get; set; }
    }

    #endregion

    #region [ Consulta de Transação de Crédito por TID - MEC323CO / MEC323 / IS99 ]

    /// <summary>
    /// Enumeração auxiliar utilizada pela classe CreditoTID do serviço de Consulta de Transação de Crédito por TID.
    /// </summary>
    /// <seealso cref="Redecard.PN.Extrato.Servicos.ConsultaTransacao.CreditoTID">Classe CreditoTID.</seealso>
    [DataContract]
    public enum CreditoTidTipoCancelamento
    {
        /// <summary>Não identificado</summary>
        [EnumMember]
        NaoIdentificado = -1,
        /// <summary>Saldo disponível</summary>
        [EnumMember]
        SaldoDisponivel = 0,
        /// <summary>Cancelada total</summary>
        [EnumMember]
        CanceladaTotal = 1,
        /// <summary>Cancelada total no dia</summary>
        [EnumMember]
        CanceladaTotalNoDia = 2
    }

    /// <summary>
    /// Enumeração auxiliar utilizada pela classe CreditoTID do serviço de Consulta de Transação de Crédito por TID.
    /// </summary>
    /// <seealso cref="Redecard.PN.Extrato.Servicos.ConsultaTransacao.CreditoTID">Classe CreditoTID.</seealso>
    [DataContract]
    public enum CreditoTidMotivoCancelamento
    {
        /// <summary>Não identificado</summary>
        [EnumMember]
        NaoIdentificado = -1,
        /// <summary>Cancelamento</summary>
        [EnumMember]
        Cancelamento = 18,
        /// <summary>Chargeback</summary>
        [EnumMember]
        ChargeBack = 23
    }

    /// <summary>
    /// Classe utilizada no módulo Extrato para Consulta de Transação de Crédito por TID.
    /// </summary>
    /// <remarks>
    /// Encapsula as informações retornadas pelo Serviço HIS do Book:<br/>
    /// - Book MEC323CO / Programa MEC323 / TranID IS99
    /// </remarks>
    [DataContract]
    public class CreditoTID : BasicContract
    {
        /// <summary>Número TID</summary>
        [DataMember]
        public String NumeroIdDataCash { get; set; }
       
        /// <summary>Código do Estabelecimento / Número do PV</summary>
        [DataMember]
        public Int32 NumeroPV { get; set; }

        /// <summary>Número do cartão</summary>
        [DataMember]
        public String NumeroCartao { get; set; }

        /// <summary>Data da Transação</summary>
        [DataMember]
        public DateTime DataTransacao { get; set; }

        /// <summary>Valor da Transação</summary>
        [DataMember]
        public Decimal ValorTransacao { get; set; }

        /// <summary>Autorização de Vendas</summary>
        [DataMember]
        public String AutorizacaoVenda { get; set; }

        /// <summary>Número do Resumo de Vendas</summary>
        [DataMember]
        public Int32 NumeroResumoVendas { get; set; }

        /// <summary>Quantidade de Parcelas</summary>
        [DataMember]
        public Int16 QuantidadeParcelas { get; set; }

        /// <summary>Código do Produto de Venda</summary>
        [DataMember]
        public String CodigoProdutoVenda { get; set; }

        /// <summary>Descrição do Produto de Venda</summary>
        [DataMember]
        public String DescricaoProdutoVenda { get; set; }

        /// <summary>ID do Cancelamento</summary>
        [DataMember]
        public Int16 IDCancelamento { get; set; }

        /// <summary>Tipo do Cancelamento</summary>
        [DataMember]
        public CreditoTidTipoCancelamento TipoCancelamento { get; set; }

        /// <summary>Data do Resumo</summary>
        [DataMember]
        public DateTime DataResumo { get; set; }

        /// <summary>Timestamp Transacao</summary>
        [DataMember]
        public String TimestampTransacao { get; set; }

        /// <summary>Bandeira</summary>
        [DataMember]
        public String Bandeira { get; set; }

        /// <summary>Quantidade de Cancelamentos</summary>
        [DataMember]
        public Int16 QuantidadeCancelamento { get; set; }

        /// <summary>Lista de Cancelamentos</summary>
        [DataMember]
        public List<CancelamentoCreditoTID> Cancelamentos { get; set; }

        /// <summary>Indicador de Tokenizacao</summary>
        [DataMember]
        public String IndicadorTokenizacao { get; set; }


        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static CreditoTID FromDTO(DTO.CreditoTID dto)
        {
            Mapper.CreateMap<DTO.CreditoTID, CreditoTID>();
            Mapper.CreateMap<DTO.CreditoTidMotivoCancelamento, CreditoTidMotivoCancelamento>();
            Mapper.CreateMap<DTO.CreditoTidTipoCancelamento, CreditoTidTipoCancelamento>();
            Mapper.CreateMap<DTO.CancelamentoCreditoTID, CancelamentoCreditoTID>();
            return Mapper.Map<CreditoTID>(dto);
        }
    }

    /// <summary>
    /// Classe auxiliar utilizada pela classe CreditoTID do serviço de Consulta de Transação de Crédito por TID.
    /// </summary>
    /// <seealso cref="Redecard.PN.Extrato.Servicos.ConsultaTransacao.CreditoTID">Classe CreditoTID.</seealso>
    [DataContract]
    public class CancelamentoCreditoTID : BasicContract
    {
        /// <summary>Data do Cancelamento</summary>
        [DataMember]
        public DateTime DataCancelamento { get; set; }

        /// <summary>Número do Aviso</summary>
        [DataMember]
        public Decimal NumeroAvisoProcesso { get; set; }

        /// <summary>Código do Motivo do Cancelamento</summary>
        [DataMember]
        public Int16 CodigoMotivoCancelamento { get; set; }

        /// <summary>Motivo do cancelamento</summary>
        [DataMember]
        public CreditoTidMotivoCancelamento MotivoCancelamento { get; set; }
    }

    #endregion    
}