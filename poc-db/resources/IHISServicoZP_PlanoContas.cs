using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.OutrosServicos.Servicos.PlanoContas;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Interface de Serviço para expor os métodos de consulta mainframe do módulo Plano de Contas
    /// </summary>
    [ServiceContract]
    public interface IHISServicoZP_PlanoContas
    {
        /// <summary>
        /// Consulta de ofertas.<br/>        
        /// - Book ZPCA1790	/ Programa ZP1790 / TranID ZPP0 / Método ConsultarOfertas
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPCA1790	/ Programa ZP1790 / TranID ZPP0 / Método ConsultarOfertas
        /// </remarks>
        /// <param name="codigoRetorno">Código de retorno do Serviço</param>
        /// <param name="dataFim">Data de término do período de vigência da pesquisa</param>
        /// <param name="dataInicio">Data de início do período de vigência da pesquisa</param>
        /// <param name="numeroPV">Código do estabelecimento</param>        
        /// <param name="tipoEstabelecimento">Tipo de estabelecimento.</param>
        /// <returns>Ofertas</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Oferta> ConsultarOfertas(
            Int32 numeroPV,
            DateTime dataInicio,
            DateTime dataFim,
            TipoEstabelecimento tipoEstabelecimento,
            out Int16 codigoRetorno);

        /// <summary>
        /// Consulta metas de ofertas.<br/>        
        /// - Book ZPCA1791	/ Programa ZP1791 / TranID ZPP1 / Método ConsultarMetasOfertas
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPCA1791	/ Programa ZP1791 / TranID ZPP1 / Método ConsultarMetasOfertas
        /// </remarks>
        /// <param name="codigoOferta">Código da oferta</param>
        /// <param name="codigoProposta">Código da proposta</param>
        /// <param name="codigoRetorno">Código de retorno do serviço</param>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="tipoEstabelecimento">Tipo de estabelecimento</param>
        /// <returns>Metas da Oferta</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<MetaOferta> ConsultarMetasOferta(
            Int32 codigoOferta,
            Int32 numeroPV,
            TipoEstabelecimento tipoEstabelecimento,
            Decimal codigoProposta,
            out Int16 codigoRetorno);

        /// <summary>
        /// Consulta Apuração - Lista de Ano(s) de Referência(s).<br/>
        /// - Book ZPCA1792	/ Programa ZP1792 / TranID ZPP2 / Método ConsultarAnosReferencia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPCA1792	/ Programa ZP1792 / TranID ZPP2 / Método ConsultarAnosReferencia
        /// </remarks>
        /// <param name="codigoOferta">Código da oferta</param>
        /// <param name="codigoProposta">Código da proposta</param>
        /// <param name="codigoRetorno">Código de retorno do serviço</param>
        /// <param name="numeroPV">Código do estabelecimento</param>        
        /// <param name="tipoEstabelecimento">Tipo de estabelecimento</param>
        /// <returns>Anos Referência da Oferta</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Int16> ConsultarAnosReferencia(
            Int32 codigoOferta,
            Int32 numeroPV,
            TipoEstabelecimento tipoEstabelecimento,
            Decimal codigoProposta,
            out Int16 codigoRetorno);

        /// <summary>
        /// Consulta Apuração - Faturamento Ano/Mês Referência.<br/>
        /// - Book ZPCA1793	/ Programa ZP1793 / TranID ZPP3 / Método ConsultarFaturamentoAnoReferencia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPCA1793	/ Programa ZP1793 / TranID ZPP3 / Método ConsultarFaturamentoAnoReferencia
        /// </remarks>
        /// <param name="anoReferencia">Ano referência</param>
        /// <param name="codigoOferta">Código da oferta</param>
        /// <param name="codigoProposta">Código da proposta</param>
        /// <param name="codigoRetorno">Código de retorno do serviço</param>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="tipoEstabelecimento">Tipo de estabelecimento</param>
        /// <returns>Faturamentos no Ano</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Faturamento> ConsultarFaturamento(
            Int32 codigoOferta,
            Int32 numeroPV,
            TipoEstabelecimento tipoEstabelecimento,
            Decimal codigoProposta,
            Int16 anoReferencia,
            out Int16 codigoRetorno);

        /// <summary>
        /// Consulta Apuração - Faturamento de uma Oferta (para todos os Anos Referência).<br/>
        /// Consulta todos os Anos Referência, e para cada ano, realiza a consulta (multi-thread) dos faturamentos do ano.<br/>
        /// - Book ZPCA1692	/ Programa ZP1692 / TranID ZPP2 / Método ConsultarAnosReferencia<br/>
        /// - Book ZPCA1693	/ Programa ZP1693 / TranID ZPP3 / Método ConsultarFaturamentoAnoReferencia        
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book ZPCA1692	/ Programa ZP1692 / TranID ZPP2 / Método ConsultarAnosReferencia<br/>
        /// - Book ZPCA1693	/ Programa ZP1693 / TranID ZPP3 / Método ConsultarFaturamentoAnoReferencia
        /// </remarks>
        /// <param name="codigoOferta">Código da oferta</param>
        /// <param name="codigoProposta">Código da proposta</param>
        /// <param name="codigoRetorno">Código de retorno do serviço</param>
        /// <param name="numeroPV">Código do estabelecimento</param>        
        /// <param name="tipoEstabelecimento">Tipo de estabelecimento</param>
        /// <returns>Faturamentos do ano</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Faturamento> ConsultarFaturamentos(
            Int32 codigoOferta,
            Int32 numeroPV,
            TipoEstabelecimento tipoEstabelecimento,
            Decimal codigoProposta,
            out Int16 codigoRetorno);
    
         /// <summary>
        /// Consulta o tipo de oferta ativa para o estabelecimento<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKZP7030 / Programa ZPC703 / TranID ZPJA / Método ConsultarTipoOfertaAtiva
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKZP7030 / Programa ZPC703 / TranID ZPJA / Método ConsultarTipoOfertaAtiva
        /// </remarks>
        /// <returns>Consulta tipo de oferta ativa</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int16 ConsultarTipoOfertaAtiva(
            Int32 numeroPv,
            out TipoOferta tipoOferta);

        /// <summary>
        /// Consulta os dados da oferta no aceite<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKZP7040 / Programa ZPC704 / TranID ZPJB / Método ConsultarDadosOfertaAceite
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKZP7040 / Programa ZPC704 / TranID ZPJB / Método ConsultarDadosOfertaAceite
        /// </remarks>
        /// <returns>Consultar dados da oferta aceite</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<FaixaOfertaNoAceite> ConsultarDadosOfertaAceite(
            Int32 numeroPv,
            out Int16 codigoRetorno);
    
        /// <summary>
        /// Consulta dados de celular e bônus.<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKZP7050 / Programa ZPC705 / TranID ZPJC / Método ConsultarDadosCelularBonus
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKZP7050 / Programa ZPC705 / TranID ZPJC / Método ConsultarDadosCelularBonus
        /// </remarks>
        /// <returns>Consultar dados celular bônus</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<OfertaCelular> ConsultarDadosCelularBonus(
            Int32 numeroPv,
            out Int16 codigoRetorno);

        /// <summary>
        /// Consulta dados de apuração.<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKZP7060 / Programa ZPC706 / TranID ZPJD / Método ConsultarDadosApuracao
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKZP7060 / Programa ZPC706 / TranID ZPJD / Método ConsultarDadosApuracao
        /// </remarks>
        /// <returns>Dados de apuração</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<OfertaDadosApuracao> ConsultarDadosApuracao(
            Int32 numeroPv,
            out Int16 codigoRetorno);

        /// <summary>
        /// Consulta detalhes dos dados de apuração.<br/>
        /// Utilizado no Projeto Japão / Bônus Rede<br/>
        /// - Book BKZP7070 / Programa ZPC707 / TranID ZPJE / Método ConsultarDadosApuracaoDetalhes
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKZP7070 / Programa ZPC707 / TranID ZPJE / Método ConsultarDadosApuracaoDetalhes
        /// </remarks>
        /// <returns>Detalhes dos dados de apuração</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<OfertaDadosApuracaoDetalhe> ConsultarDadosApuracaoDetalhes(
            Int32 numeroPv,
            DateTime mesAnoReferencia,
            out Int16 codigoRetorno);
    }
}