using Redecard.PN.Extrato.Servicos.Contrato.ContratoDados.Response;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo.Vendas;
using Redecard.PN.Extrato.Servicos.Vendas;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Vendas.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book WACA1310 / Programa WA1310 / TranID ISHA / Método ConsultarCreditoTotalizadores<br/>
    /// - Book WACA1311 / Programa WA1311 / TranID ISHB / Método ConsultarCredito<br/>
    /// - Book WACA1312 / Programa WA1312 / TranID ISHC / Método ConsultarDebitoTotalizadores<br/>
    /// - Book WACA1313 / Programa WA1313 / TranID ISHD / Método ConsultarDebito<br/>
    /// - Book WACA1314 / Programa WA1314 / TranID ISHE / Método ConsultarConstrucardTotalizadores<br/>
    /// - Book WACA1315 / Programa WA1315 / TranID ISHF / Método ConsultarConstrucard
    /// - Book BKWA2610	/ Programa WAC261 / TranID WAAF / Método ConsultarRecargaCelularPvFisicoTotalizadores<br/>
    /// - Book BKWA2620	/ Programa WAC262 / TranID WAAG / Método ConsultarRecargaCelularPvFisico<br/>
    /// - Book BKWA2630	/ Programa WAC263 / TranID WAAH / Método ConsultarRecargaCelularPvLogicoTotalizadores<br/>
    /// - Book BKWA2640	/ Programa WAC264 / TranID WAAI / Método ConsultarRecargaCelularPvLogico
    /// </remarks>
    [ServiceContract,
    ServiceKnownType(typeof(CreditoTotalizador)),
    ServiceKnownType(typeof(Credito)),
    ServiceKnownType(typeof(CreditoD)),
    ServiceKnownType(typeof(DebitoTotalizador)),
    ServiceKnownType(typeof(Debito)),
    ServiceKnownType(typeof(DebitoDT)),
    ServiceKnownType(typeof(DebitoA1)),
    ServiceKnownType(typeof(DebitoA2)),
    ServiceKnownType(typeof(ConstrucardTotalizador)),
    ServiceKnownType(typeof(Construcard)),
    ServiceKnownType(typeof(ConstrucardDT)),
    ServiceKnownType(typeof(ConstrucardA1)),
    ServiceKnownType(typeof(ConstrucardA2)),
    ServiceKnownType(typeof(RecargaCelularTotalizador)),
    ServiceKnownType(typeof(RecargaCelular)),
    ServiceKnownType(typeof(RecargaCelularPvFisico)),
    ServiceKnownType(typeof(RecargaCelularPvLogico))]
    public interface IHISServicoWAExtratoVendasRest
    {
        /// <summary>
        /// Consulta de Relatório de Vendas - Crédito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1310 / Programa WA1310 / TranID ISHA<br/>
        /// - Book WACA1311 / Programa WA1311 / TranID ISHB
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1310 / Programa WA1310 / TranID ISHA<br/>
        /// - Book WACA1311 / Programa WA1311 / TranID ISHB
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "RelatorioCredito")]
        RelatorioVendasCreditoResponse ConsultarRelatorioCredito(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros);

        /// <summary>
        /// Consulta de Relatório de Vendas - Débito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1312 / Programa WA1312 / TranID ISHC<br/>
        /// - Book WACA1313 / Programa WA1313 / TranID ISHD
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1312 / Programa WA1312 / TranID ISHC<br/>
        /// - Book WACA1313 / Programa WA1313 / TranID ISHD
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="modalidade">Modalidade</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "RelatorioDebito")]
        RelatorioVendasDebitoResponse ConsultarRelatorioDebito(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            Modalidade modalidade,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            DebitoTipoRegistro tipoRegistro,
            Int32 quantidadeTotalRegistros);

        /// <summary>
        /// Consulta de Relatório de Vendas - Construcard.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1314 / Programa WA1314 / TranID ISHE<br/>
        /// - Book WACA1315 / Programa WA1315 / TranID ISHF
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1314 / Programa WA1314 / TranID ISHE<br/>
        /// - Book WACA1315 / Programa WA1315 / TranID ISHF
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "RelatorioConstrucard")]
        RelatorioVendasConstrucardResponse ConsultarRelatorioConstrucard(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ConstrucardTipoRegistro tipoRegistro,
            Int32 quantidadeTotalRegistros);

        /// <summary>
        /// Consulta de Relatório de Vendas - Recarga de Celular - PV Físico.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
        /// </remarks>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "RelatorioRecargaFisico")]
        RelatorioVendasRecargaCelularResponse ConsultarRelatorioRecargaCelularPvFisico(
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros);

        /// <summary>
        /// Consulta de Relatório de Vendas - Recarga de Celular - PV Lógico.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2610 / Programa WAC263 / TranID WAAH<br/>
        /// - Book BKWA2620 / Programa WAC264 / TranID WAAI
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2610 / Programa WAC263 / TranID WAAH<br/>
        /// - Book BKWA2620 / Programa WAC264 / TranID WAAI
        /// </remarks>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "RelatorioRecargaLogico")]
        RelatorioVendasRecargaCelularResponse ConsultarRelatorioRecargaCelularPvLogico(
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros);

        /// <summary>
        /// Consulta de Relatório de Vendas - Recarga de Celular - PV Lógico.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2610 / Programa WAC263 / TranID WAAH<br/>
        /// - Book BKWA2620 / Programa WAC264 / TranID WAAI
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2610 / Programa WAC263 / TranID WAAH<br/>
        /// - Book BKWA2620 / Programa WAC264 / TranID WAAI
        /// </remarks>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="tipoPv">Tipo do PV da pesquisa</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "RelatorioRecarga")]
        RelatorioVendasRecargaCelularResponse ConsultarRelatorioRecargaCelular(
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            RecargaCelularTipoPv tipoPv,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Vendas - Crédito.<br/>
        /// - Book WACA1310 / Programa WA1310 / TranID ISHA
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1310 / Programa WA1310 / TranID ISHA
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Vendas - Crédito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "TotalizadoresCredito")]
        ResponseBaseItem<CreditoTotalizador> ConsultarCreditoTotalizadores(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de registros do Relatório de Vendas - Crédito.<br/>
        /// - Book WACA1311 / Programa WA1311 / TranID ISHB
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1311 / Programa WA1311 / TranID ISHB
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Vendas - Crédito.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "Credito")]
        ResponseBaseList<Credito> ConsultarCredito(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Vendas - Débito.<br/>
        /// - Book WACA1312 / Programa WA1312 / TranID ISHC
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1312 / Programa WA1312 / TranID ISHC
        /// </remarks>      
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="modalidade">Modalidade</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Vendas - Débito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "TotalizadoresDebito")]
        ResponseBaseItem<DebitoTotalizador> ConsultarDebitoTotalizadores(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            Modalidade modalidade,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de registros do Relatório de Vendas - Débito.<br/>
        /// - Book WACA1313 / Programa WA1313 / TranID ISHD
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1313 / Programa WA1313 / TranID ISHD
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="modalidade">Modalidade</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Vendas - Débito.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "Debito")]
        ResponseBaseList<Debito> ConsultarDebito(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            DebitoTipoRegistro tipoRegistro,
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            Modalidade modalidade,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Vendas - Construcard.<br/>
        /// - Book WACA1314 / Programa WA1314 / TranID ISHE
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1314 / Programa WA1314 / TranID ISHE
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Vendas - Construcard</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "TotalizadoresConstrucard")]
        ResponseBaseItem<ConstrucardTotalizador> ConsultarConstrucardTotalizadores(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de registros do Relatório de Vendas - Construcard.<br/>
        /// - Book WACA1315 / Programa WA1315 / TranID ISHF
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1315 / Programa WA1315 / TranID ISHF
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>        
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Vendas - Construcard.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "Construcard")]
        ResponseBaseList<Construcard> ConsultarConstrucard(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            ConstrucardTipoRegistro tipoRegistro,
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Vendas - Recarga de Celular de PV Físico.<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF
        /// </remarks>     
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Vendas - Recarga de Celular de PV Físico</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "TotalizadoresRecargaFisico")]
        ResponseBaseItem<RecargaCelularTotalizador> ConsultarRecargaCelularPvFisicoTotalizadores(
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de registros do Relatório de Vendas - Recarga de Celular de PV Físico.<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>        
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Vendas - Recarga de Celular de PV Físico.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "RecargaFisico")]
        ResponseBaseList<RecargaCelularPvFisico> ConsultarRecargaCelularPvFisico(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Vendas - Recarga de Celular de PV Lógico.<br/>
        /// - Book BKWA2630 / Programa WAC263 / TranID WAAH
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2630 / Programa WAC263 / TranID WAAH
        /// </remarks>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Vendas - Recarga de Celular de PV Lógico</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "TotalizadoresRecargaLogico")]
        ResponseBaseItem<RecargaCelularTotalizador> ConsultarRecargaCelularPvLogicoTotalizadores(
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de registros do Relatório de Vendas - Recarga de Celular de PV Lógico.<br/>
        /// - Book BKWA2640 / Programa WAC264 / TranID WAAI
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2640 / Programa WAC264 / TranID WAAI
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>        
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Vendas - Recarga de Celular de PV Lógico.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "RecargaLogico")]
        ResponseBaseList<RecargaCelularPvLogico> ConsultarRecargaCelularPvLogico(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);
    }
}
