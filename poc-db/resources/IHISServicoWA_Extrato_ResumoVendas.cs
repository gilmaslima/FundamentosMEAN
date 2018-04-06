using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.ResumoVenda;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Resumo de Vendas.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// WACA668	WA668	IS10	ConsultarDebitoCVsAceitos
    /// - Book WACA704 / Programa WA704 / TranID IS14 / Método ConsultarCreditoAjustes<br/>
    /// - Book WACA706 / Programa WA706 / TranID IS16 / Método ConsultarCreditoCVsAceitos<br/>
    /// - Book WACA748 / Programa WA748 / TranID ISD4 / Método ConsultarDebitoConstrucardAjustes<br/>
    /// - Book WACA797 / Programa WA797 / TranID IS35 / Método ConsultarConstrucardCVsAceitos<br/>
    /// - Book BKWA2430 / Programa WA243 / TranID ISIC / Método ConsultarRecargaCelularResumo<br/>
    /// - Book BKWA2440 / Programa WA244 / TranID ISID / Método ConsultarRecargaCelularVencimentos<br/>
    /// - Book BKWA2450 / Programa WA245 / TranID ISIE / Método ConsultarRecargaCelularAjustesCredito<br/>
    /// - Book BKWA2450 / Programa WA245 / TranID ISIE / Método ConsultarRecargaCelularAjustesDebito<br/>
    /// - Book BKWA2460 / Programa WA246 / TranID ISIF / Método ConsultarRecargaCelularComprovantes
    /// </remarks>
    [ServiceContract]
    public interface IHISServicoWA_Extrato_ResumoVendas
    {
        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Débito.<br/>
        /// Efetua a consulta dos CVs Aceitos.<br/>
        /// - Book WACA668 / Programa WA668 / TranID IS10
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA668 / Programa WA668 / TranID IS10
        /// </remarks>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroRV">Número do Resumo de Vendas</param>
        /// <param name="dataApresentacao">Data de apresentação do Resumo de Vendas</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>CVs de Débito aceitos</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<DebitoCVsAceitos> ConsultarDebitoCVsAceitos(
            Int32 pv,
            Int32 numeroRV,
            DateTime dataApresentacao,
            out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Débito/Construcard.<br/>
        /// Efetua a consulta dos Ajustes.<br/>
        /// - Book WACA748 / Programa WA748 / TranID ISD4
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA748 / Programa WA748 / TranID ISD4
        /// </remarks>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="dataApresentacao">Data de apresentação do Resumo de Vendas</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Ajustes Débito/Construcard</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<DebitoCDCAjuste> ConsultarDebitoConstrucardAjustes(
            Int32 pv,
            Int32 resumo,
            DateTime dataApresentacao,
            String tipoResumo,
            out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Crédito.<br/>
        /// Efetua a consulta dos CVs Aceitos.<br/>
        /// - Book WACA706 / Programa WA706 / TranID IS16
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA706 / Programa WA706 / TranID IS16
        /// </remarks>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroRV">Número do Resumo de Vendas</param>
        /// <param name="numeroMes">Número do mês</param>
        /// <param name="dataApresentacao">Data de apresentação do Resumo de Vendas</param>
        /// <param name="timestamp">Timestamps do Resumo de Vendas</param>
        /// <param name="tipoResumoVenda">Tipo do Resumo de Vendas</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>CVs de Crédito aceitos</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<CreditoCVsAceitos> ConsultarCreditoCVsAceitos(
            Int32 pv,
            Int32 numeroRV,
            Int16 numeroMes,
            DateTime dataApresentacao,
            String timestamp,
            String tipoResumoVenda,
            out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Crédito.<br/>
        /// Efetua a consulta dos Ajustes<br/>
        /// - Book WACA704 / Programa WA704 / TranID IS14
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA704 / Programa WA704 / TranID IS14
        /// </remarks>
        /// <param name="timestamp">Timestamps do Resumo de Vendas</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroRV">Número do Resumo de Vendas</param>
        /// <param name="dataApresentacao">Data de apresentação do Resumo de Vendas</param>        
        /// <param name="tipoRV">Tipo do Resumo de Vendas</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Ajustes Crédito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<CreditoAjustes> ConsultarCreditoAjustes(
            String timestamp,
            Int32 pv,
            Int16 tipoRV,
            Int32 numeroRV,
            DateTime dataApresentacao,
            out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Construcard.<br/>
        /// Efetua a consulta dos CVs Aceitos.<br/>
        /// - Book WACA797 / Programa WA797 / TranID IS35
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA797 / Programa WA797 / TranID IS35
        /// </remarks>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroRV">Número do Resumo de Vendas</param>        
        /// <param name="dataApresentacao">Data de apresentação do Resumo de Vendas</param>        
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>CVs de Construcard aceitos</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ConstrucardCVsAceitos> ConsultarConstrucardCVsAceitos(
            Int32 pv,
            Int32 numeroRV,
            DateTime dataApresentacao,
            out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Recarga de Celular.<br/>
        /// Efetua a consulta do Resumo.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do Book:<br/>
        /// - Book BKWA2430 / Programa WA243 / TranID ISIC
        /// </remarks>
        /// <param name="numeroPv">Número do estabelecimento</param>
        /// <param name="numeroRv">Número do resumo de vendas</param>
        /// <param name="dataPagamento">Data do pagamento</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <param name="origemPesquisa">Origem/Tipo da pesquisa</param>
        /// <returns>Resumo do Resumo de Vendas</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        RecargaCelularResumo ConsultarRecargaCelularResumo(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            RecargaCelularResumoOrigem origemPesquisa,
            out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Recarga de Celular.<br/>
        /// Efetua a consulta do Vencimento.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do Book:<br/>
        /// - Book BKWA2440 / Programa WA244 / TranID ISID
        /// </remarks>
        /// <param name="numeroPv">Número do estabelecimento</param>
        /// <param name="numeroRv">Número do resumo de vendas</param>
        /// <param name="dataPagamento">Data do pagamento</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Vencimento</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        RecargaCelularVencimento ConsultarRecargaCelularVencimentos(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Recarga de Celular.<br/>
        /// Efetua a consulta dos Ajustes Crédito.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do Book:<br/>
        /// - Book BKWA2450 / Programa WA245 / TranID ISIE
        /// </remarks>
        /// <param name="numeroPv">Número do estabelecimento</param>
        /// <param name="numeroRv">Número do resumo de vendas</param>
        /// <param name="dataPagamento">Data do pagamento</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Ajustes Crédito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<RecargaCelularAjusteCredito> ConsultarRecargaCelularAjustesCredito(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Recarga de Celular.<br/>
        /// Efetua a consulta dos Ajustes Débito.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do Book:<br/>
        /// - Book BKWA2450 / Programa WA245 / TranID ISIE
        /// </remarks>
        /// <param name="numeroPv">Número do estabelecimento</param>
        /// <param name="numeroRv">Número do resumo de vendas</param>
        /// <param name="dataPagamento">Data do pagamento</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Ajustes Débito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<RecargaCelularAjusteDebito> ConsultarRecargaCelularAjustesDebito(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            out StatusRetorno status);

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Recarga de Celular.<br/>
        /// Efetua a consulta dos Comprovantes Realizados.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do Book:<br/>
        /// - Book BKWA2460 / Programa WA246 / TranID ISIF
        /// </remarks>
        /// <param name="numeroPv">Número do estabelecimento</param>
        /// <param name="numeroRv">Número do resumo de vendas</param>
        /// <param name="dataPagamento">Data do pagamento</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Comprovantes Realizados</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<RecargaCelularComprovante> ConsultarRecargaCelularComprovantes(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            out StatusRetorno status);
    }
}
