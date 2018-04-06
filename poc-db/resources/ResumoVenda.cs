using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using AutoMapper;
using DTO = Redecard.PN.Extrato.Modelo.ResumoVendas;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos.ResumoVenda
{
    #region [ Resumo de Vendas - Débito - CVs Aceitos - WACA668 / WA668 / IS10 ]

    /// <summary>
    /// Classe modelo para Relatório de Resumo de Vendas - Débito - CVs Aceitos
    /// </summary>
    [DataContract]
    public class DebitoCVsAceitos : BasicContract
    {
        /// <summary>Número do CV</summary>
        [DataMember]
        public Decimal NumeroCV { get; set; }

        /// <summary>Número do Cartão</summary>
        [DataMember]
        public String NumeroCartao { get; set; }

        /// <summary>Data do CV</summary>
        [DataMember]
        public DateTime DataCV { get; set; }

        /// <summary>Data de Vencimento</summary>
        [DataMember]
        public DateTime DataVencimento { get; set; }

        /// <summary>Valor do CV</summary>
        [DataMember]
        public Decimal ValorCV { get; set; }

        /// <summary>Valor Líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }

        /// <summary>Valor do Saque</summary>
        [DataMember]
        public Decimal ValorSaque { get; set; }

        /// <summary>Valor da Compra</summary>
        [DataMember]
        public Decimal ValorCompra { get; set; }

        /// <summary>Quantidade de Parcelas</summary>
        [DataMember]
        public Int16 Plano { get; set; }

        /// <summary>Horas do CV</summary>
        [DataMember]
        public DateTime HorasCV { get; set; }

        /// <summary>Descrição</summary>
        [DataMember]
        public String Descricao { get; set; }

        /// <summary>Venda Cancelada</summary>
        [DataMember]
        public String VendaCancelada { get; set; }

        /// <summary>Número TID</summary>
        [DataMember]
        public String TID { get; set; }

        /// <summary>Indicador de Tokenizacao</summary>
        [DataMember]
        public String IndicadorTokenizacao { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<DebitoCVsAceitos> FromDTO(List<DTO.DebitoCVsAceitos> dto)
        {
            Mapper.CreateMap<DTO.DebitoCVsAceitos, DebitoCVsAceitos>();
            return Mapper.Map<List<DebitoCVsAceitos>>(dto);
        }
    }

    #endregion

    #region [ Resumo de Vendas - Débito/Construcard - Ajustes - WACA748 / WA748 / ISD4 ]

    /// <summary>
    /// Classe modelo para Relatório de Resumo de Vendas - Débito/Construcard - Ajustes
    /// </summary>
    [DataContract]
    public class DebitoCDCAjuste : BasicContract
    {
        /// <summary>Código de Ajuste</summary>
        [DataMember]
        public Int16 CodigoAjuste { get; set; }

        /// <summary>Descrição do Ajuste</summary>
        [DataMember]
        public String DescricaoAjuste { get; set; }

        /// <summary>Referência do Ajuste</summary>
        [DataMember]
        public String ReferenciaAjuste { get; set; }

        /// <summary>PV do Ajuste</summary>
        [DataMember]
        public Int32 PVAjuste { get; set; }

        /// <summary>Valor do Débito</summary>
        [DataMember]
        public Decimal ValorDebito { get; set; }

        /// <summary>Valor do Ajuste</summary>
        [DataMember]
        public Decimal ValorAjuste { get; set; }

        /// <summary>Data de Referência</summary>
        [DataMember]
        public DateTime DataReferencia { get; set; }

        /// <summary>Débito / Desagendamento</summary>
        [DataMember]
        public String IndicadorDebitoDesagendamento { get; set; }

        /// <summary>Data Referência 2</summary>
        [DataMember]
        public String DataReferencia2 { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<DebitoCDCAjuste> FromDTO(List<DTO.DebitoCDCAjuste> dto)
        {
            Mapper.CreateMap<DTO.DebitoCDCAjuste, DebitoCDCAjuste>();
            return Mapper.Map<List<DebitoCDCAjuste>>(dto);
        }
    }

    #endregion

    #region [ Resumo de Vendas - Crédito - CVs Aceitos - WACA706 / WA706 / IS16 ]

    /// <summary>
    /// Classe modelo para Relatório de Resumo de Vendas - Crédito - CVs Aceitos
    /// </summary>
    [DataContract]
    public class CreditoCVsAceitos : BasicContract
    {
        /// <summary>Número CV</summary>
        [DataMember]
        public Int32 NumCV { get; set; }

        /// <summary>Número NSU</summary>
        [DataMember]
        public String NSU { get; set; }

        /// <summary>Número do Cartão</summary>
        [DataMember]
        public String Cartao { get; set; }

        /// <summary>País</summary>
        [DataMember]
        public String Pais { get; set; }

        /// <summary>Número do Ponto de Venda</summary>
        [DataMember]
        public Int32 NumeroPV { get; set; }

        /// <summary>Data do CV</summary>
        [DataMember]
        public DateTime DataCV { get; set; }

        /// <summary>Valor</summary>
        [DataMember]
        public Decimal Valor { get; set; }

        /// <summary>Quantidade de Parcelas</summary>
        [DataMember]
        public Int16 QuantidadeParcelas { get; set; }

        /// <summary>Hora</summary>
        [DataMember]
        public DateTime Hora { get; set; }

        /// <summary>Tipo de Captura</summary>
        [DataMember]
        public String TipoCaptura { get; set; }

        /// <summary>Cancelamento de Venda</summary>
        [DataMember]
        public String CancelamentoVenda { get; set; }

        /// <summary>Número TID</summary>
        [DataMember]
        public String TID { get; set; }

        /// <summary>Indicador de Tokenizacao</summary>
        [DataMember]
        public String IndicadorTokenizacao { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<CreditoCVsAceitos> FromDTO(List<DTO.CreditoCVsAceitos> dto)
        {
            Mapper.CreateMap<DTO.CreditoCVsAceitos, CreditoCVsAceitos>();
            return Mapper.Map<List<CreditoCVsAceitos>>(dto);
        }
    }

    #endregion

    #region [ Resumo de Vendas - Crédito - Ajustes - WACA704 / WA704 / IS14 ]

    /// <summary>
    /// Classe modelo para Relatório de Resumo de Vendas - Crédito - Ajustes
    /// </summary>
    [DataContract]
    public class CreditoAjustes : BasicContract
    {
        /// <summary>Código do Ajuste</summary>
        [DataMember]
        public Int32 CodigoAjuste { get; set; }

        /// <summary>Descrição do Ajuste</summary>
        [DataMember]
        public String DescricaoAjuste { get; set; }

        /// <summary>Referência do Ajuste</summary>
        [DataMember]
        public String ReferenciaAjuste { get; set; }

        /// <summary>PV Ajuste</summary>
        [DataMember]
        public Int32 PVAjuste { get; set; }

        /// <summary>Valor do Débito</summary>
        [DataMember]
        public Decimal ValorDebito { get; set; }

        /// <summary>Valor do Ajuste</summary>
       [DataMember]
        public Decimal ValorAjuste { get; set; }

        /// <summary>Data do Vencimento</summary>
        [DataMember]
        public DateTime DataVencimento { get; set; }

        /// <summary>Código da Bandeira</summary>
        [DataMember]
        public String CodigoBandeira { get; set; }

        /// <summary>Indicador DEB - Débito ou  DES - Agendamento</summary>
        [DataMember]
        public String IndicadorDebitoDesagendamento { get; set; }

        /// <summary>Processo de Retenção que faz parte do ajuste</summary>
        [DataMember]
        public String ProcessoRetencao { get; set; }

        /// <summary>Data de Referência</summary>
        [DataMember]
        public String DataReferencia { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<CreditoAjustes> FromDTO(List<DTO.CreditoAjustes> dto)
        {
            Mapper.CreateMap<DTO.CreditoAjustes, CreditoAjustes>();
            return Mapper.Map<List<CreditoAjustes>>(dto);
        }
    }

    #endregion

    #region [ Resumo de Vendas - Construcard - CVs Aceitos - WACA797 / WA797 / IS35 ]

    /// <summary>
    /// Classe modelo para Relatório de Resumo de Vendas - Construcard - CVs Aceitos
    /// </summary>
    [DataContract]
    public class ConstrucardCVsAceitos : BasicContract
    {
        /// <summary>Número do Cartão</summary>
        [DataMember]
        public String NumeroCartao { get; set; }

        /// <summary>Data do CV</summary>
        [DataMember]
        public DateTime DataCV { get; set; }

        /// <summary>Valor do CV</summary>
        [DataMember]
        public Decimal ValorCV { get; set; }

        /// <summary>Quantidade de Parcelas</summary>
        [DataMember]
        public Int16 Plano { get; set; }

        /// <summary>Horas do CV</summary>
        [DataMember]
        public DateTime HorasCV { get; set; }

        /// <summary>Tipo da Transação</summary>
        [DataMember]
        public Int16 TipoTransacao { get; set; }

        /// <summary>Sub Tipo da Transação</summary>
        [DataMember]
        public Int16 SubTipoTransacao { get; set; }

        /// <summary>Descrição</summary>
        [DataMember]
        public String Descricao { get; set; }

        /// <summary>Valor do Saque</summary>
        [DataMember]
        public Decimal ValorSaque { get; set; }

        /// <summary>Valor da Compra</summary>
        [DataMember]
        public Decimal ValorCompra { get; set; }

        /// <summary>Número TID</summary>
        [DataMember]
        public String TID { get; set; }

        /// <summary>Número CV</summary>
        [DataMember]
        public Decimal NumeroCV { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<ConstrucardCVsAceitos> FromDTO(List<DTO.ConstrucardCVsAceitos> dto)
        {
            Mapper.CreateMap<DTO.ConstrucardCVsAceitos, ConstrucardCVsAceitos>();
            return Mapper.Map<List<ConstrucardCVsAceitos>>(dto);
        }
    }

    #endregion

    #region [ Resumo de Vendas - Recarga de Celular - Resumo - BKWA2430 / WA243 / ISIC ]

    /// <summary>
    /// Classe modelo para Resumo de Vendas - Recarga Celular - Resumo
    /// </summary>
    [DataContract]
    public class RecargaCelularResumo
    {
        /// <summary>Data de Referência</summary>
        [DataMember]
        public DateTime? DataReferencia { get; set; }

        /// <summary>Data do Processamento</summary>
        [DataMember]
        public DateTime? DataProcessamento { get; set; }

        /// <summary>Quantidade de Transação</summary>
        [DataMember]
        public Int32 QuantidadeTransacao { get; set; }

        /// <summary>Valor Total da Transação</summary>
        [DataMember]
        public Decimal ValorTotalTransacao { get; set; }

        /// <summary>Valor Total de Desconto</summary>
        [DataMember]
        public Decimal ValorTotalDesconto { get; set; }

        /// <summary>Valor Total da Comissão</summary>
        [DataMember]
        public Decimal ValorTotalComissao { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static RecargaCelularResumo FromDTO(DTO.RecargaCelularResumo dto)
        {
            Mapper.CreateMap<DTO.RecargaCelularResumo, RecargaCelularResumo>();
            return Mapper.Map<RecargaCelularResumo>(dto);
        }
    }

    #endregion

    #region [ Resumo de Vendas - Recarga de Celular - Vencimentos - BKWA2440 / WA244 / ISID ]

    /// <summary>
    /// Classe modelo para Resumo de Vendas - Recarga Celular - Vencimentos
    /// </summary>
    [DataContract]
    public class RecargaCelularVencimento
    {
        /// <summary>Data de Pagamento</summary>
        [DataMember]
        public DateTime? DataPagamento { get; set; }

        /// <summary>Data de Antecipação</summary>
        [DataMember]
        public DateTime? DataAntecipacao { get; set; }

        /// <summary>Número da OC</summary>
        [DataMember]
        public Decimal NumeroOc { get; set; }

        /// <summary>Status da Comissão</summary>
        [DataMember]
        public String StatusComissao { get; set; }

        /// <summary>Valor Líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static RecargaCelularVencimento FromDTO(DTO.RecargaCelularVencimento dto)
        {
            Mapper.CreateMap<DTO.RecargaCelularVencimento, RecargaCelularVencimento>();
            return Mapper.Map<RecargaCelularVencimento>(dto);
        }
    }

    #endregion

    #region [ Resumo de Vendas - Recarga de Celular - Ajustes - BKWA2450 / WA245 / ISIE ]

    /// <summary>
    /// Classe modelo para Resumo de Vendas - Recarga Celular - Ajustes
    /// </summary>
    [DataContract]
    public class RecargaCelularAjuste
    {
        /// <summary>Data de Recebimento</summary>
        [DataMember]
        public DateTime? DataRecebimento { get; set; }

        /// <summary>Descrição da origem do ajuste</summary>
        [DataMember]
        public String DescricaoOrigemAjuste { get; set; }

        /// <summary>Código do ajuste</summary>
        [DataMember]
        public Int32 CodigoAjustes { get; set; }

        /// <summary>Descrição do serviço</summary>
        [DataMember]
        public String DescricaoAjuste { get; set; }

        /// <summary>Número do PV Ajuste</summary>
        [DataMember]
        public Int32 NumeroPvAjuste { get; set; }

        /// <summary>Valor da Venda</summary>
        [DataMember]
        public Decimal ValorVenda { get; set; }

        /// <summary>Valor do Ajuste</summary>
        [DataMember]
        public Decimal ValorAjuste { get; set; }

        /// <summary>Data da Referência</summary>
        [DataMember]
        public DateTime? DataReferencia { get; set; }
    }

    /// <summary>
    /// Classe modelo para Resumo de Vendas - Recarga Celular - Ajustes Crédito
    /// </summary>
    [DataContract]
    public class RecargaCelularAjusteCredito : RecargaCelularAjuste 
    {
        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<RecargaCelularAjusteCredito> FromDTO(List<DTO.RecargaCelularAjusteCredito> dto)
        {
            Mapper.CreateMap<DTO.RecargaCelularAjusteCredito, RecargaCelularAjusteCredito>();
            return Mapper.Map<List<RecargaCelularAjusteCredito>>(dto);
        }
    }

    /// <summary>
    /// Classe modelo para Resumo de Vendas - Recarga Celular - Ajustes Débito
    /// </summary>
    [DataContract]
    public class RecargaCelularAjusteDebito : RecargaCelularAjuste 
    {
        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<RecargaCelularAjusteDebito> FromDTO(List<DTO.RecargaCelularAjusteDebito> dto)
        {
            Mapper.CreateMap<DTO.RecargaCelularAjusteDebito, RecargaCelularAjusteDebito>();
            return Mapper.Map<List<RecargaCelularAjusteDebito>>(dto);
        }
    }

    #endregion

    #region [ Resumo de Vendas - Recarga de Celular - Comprovantes Realizados - BKWA2460 / WA246 / ISIF ]

    /// <summary>
    /// Classe modelo para Resumo de Vendas - Recarga Celular - Comprovantes Realizados
    /// </summary>
    [DataContract]
    public class RecargaCelularComprovante
    {
        /// <summary>Número da Transação</summary>
        [DataMember]
        public Decimal NumeroTransacao { get; set; }

        /// <summary>Data e Hora da Transação</summary>
        [DataMember]
        public DateTime? DataHoraTransacao { get; set; }

        /// <summary>Nome Fantasia da Operadora</summary>
        [DataMember]
        public String NomeOperadora { get; set; }

        /// <summary>Número do Celular</summary>
        [DataMember]
        public String NumeroCelular { get; set; }

        /// <summary>Valor da Transação</summary>
        [DataMember]
        public Decimal ValorTransacao { get; set; }

        /// <summary>Valor da Comissão</summary>
        [DataMember]
        public Decimal ValorComissao { get; set; }

        /// <summary>Status da Transação</summary>
        [DataMember]
        public String StatusTransacao { get; set; }

        /// <summary>Converte classe modelo DTO para classe modelo da camada de serviços</summary>
        /// <param name="dto">Instância DTO</param>
        /// <returns>Classe modelo da camada de serviços</returns>
        public static List<RecargaCelularComprovante> FromDTO(List<DTO.RecargaCelularComprovante> dto)
        {
            Mapper.CreateMap<DTO.RecargaCelularComprovante, RecargaCelularComprovante>();
            return Mapper.Map<List<RecargaCelularComprovante>>(dto);
        }
    }
    #endregion
}