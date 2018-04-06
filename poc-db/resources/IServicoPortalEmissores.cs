/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/09/25 - Tiago Barbosa dos Santos - Versão Inicial
*/
using System;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.Comum;
using Redecard.PN.Emissores.Modelos;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Redecard.PN.Emissores.Servicos.Erro;
namespace Redecard.PN.Emissores.Servicos
{
    [ServiceContract]
    public interface IServicoPortalEmissores
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<DownloadAnual> ListarDownload();

        /// <summary>
        /// Obtem as Informações do PV
        /// </summary>
        /// <param name="numPV">Número PV</param>
        /// <returns>PV</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        PontoVenda ObtemPV(int numPV, out int codigoRetorno);

        /// <summary>
        /// Lista os dados de Solicitação de Tecnologia a partir do número do PV
        /// </summary>
        /// <param name="numPV"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<DadosSolicitacaoTecnologia> ConsultarTecnologia(int numPV);

        /// <summary>
        /// Lista os Integradores
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Integrador> ConsultarIntegrador(string codIntegrador, string situacao);

        /// <summary>
        /// Lista os Equipamentos (Maquinetas)
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Equipamento> ConsultarEquipamento(out int codigoRetorno, out string mensagemRetorno);

        /// <summary>
        /// Consulta de Limite do Emissor
        /// </summary>
        /// <param name="numEmissor">Numero do Emissor</param>
        /// <returns>Valor Limite</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        decimal ConsultaLimite(int numEmissor, out int codigoRetorno, out string mensagemRetorno);

        /// <summary>
        /// Consulta PV Travado
        /// </summary>
        /// <param name="numEmissor">Emissor</param>
        /// <param name="cnpj">CNPJ</param>
        /// <returns>PVs</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<DadosPV> ConsultaPVTravado(int numEmissor, int cnpj);

        /// <summary>
        /// Consulta PV Não Travado
        /// </summary>
        /// <param name="numEmissor">Emissor</param>
        /// <param name="cnpj">CNPJ</param>
        /// <returns>PVs</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<DadosPV> ConsultaPVNaoTravado(int numEmissor, int cnpj);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        TotaisPV ConsultaTotaisPV(int numEmissor, int cnpj);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        byte[] DownloadArquivo(string codEmissor, string mesArquivo, string anoArquivo);

        #region [Pré-Pagamentos]

        /// <summary>
        /// Pré-Pagamento Detalhado
        /// </summary>
        /// <param name="numEmissor">Emissor</param>
        /// <param name="dataInicial">Data Inicial</param>
        /// <param name="dataFinal">Data Final</param>
        /// <returns></returns>        
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<SaldoPrePagamento> SaldoDetalhadoPrePagamento(int numEmissor, DateTime dataInicial, DateTime dataFinal);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numEmissor"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        SaldoPrePagamento SaldoConsolidadoPrePagamento(int numEmissor, DateTime dataInicial, DateTime dataFinal);

        /// <summary>
        /// Executa a procedure que ajusta a carga de confirmados
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean AjustarCargaConfirmados();

        /// <summary>
        /// Consulta os Pré-Pagamentos carregados do KN de acordo com os parâmetros filtrados
        /// </summary>
        /// <param name="codigoBancen">Código do Emissor do Pré-Pagamento. 
        /// Passar como 0 para retornar dos os dados.</param>
        /// <param name="dataInicial">Período de Vencimento inicial dos Pré-Pagamentos</param>
        /// <param name="dataFinal">Período de Vencimento final dos Pré-Pagamentos</param>
        /// <param name="bandeiras">Listagem de bandeiras e código EmissorBandeira a serem filtradas</param>
        /// <returns>Listagem de Pré-Pagamentos retornados</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<PrePagamento> ConsultarPrePagamento(Int32 codigoBacen, DateTime dataInicial, DateTime dataFinal, List<Bandeira> bandeiras);

        /// <summary>
        /// Consulta todos os Bancos(Emissores) com Pré-Pagamentos carregados
        /// </summary>
        /// <returns>List of Banco Listagem Bancos(Emissores) com Pré-Pagamentos</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Emissor> ConsultarEmissores();

        /// <summary>
        /// Consulta todos os Códigos Emissor-Bandeira existente para os Pré-Pagamentos
        /// </summary>
        /// <param name="codigoBacen">Código do Emissor a filtrar as bandeiras</param>
        /// <returns>List of Bandeira Listagem de Emissor-Bandeira com Pré-Pagamentos</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Bandeira> ConsultarEmissoresBandeiras(Int32 codigoBacen);

        /// <summary>
        /// Consulta todas as Bandeiras cadastradas no Oracle DR
        /// </summary>
        /// <returns>List of Bandeira </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Bandeira> ConsultarBandeiras();

        /// <summary>
        /// Consulta a listagem de Bancos reconhecidos pelo Bacen na base Sybase DR
        /// </summary>
        /// <returns>List of Banco </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Emissor> ConsultarBancos();

        /// <summary>
        /// Exclui todos os pré-pagamentos confirmados na base afim de realizar uma nova.
        /// </summary>
        /// <returns>Retorna se a execuçãou foi feita com sucesso</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean ExcluirPrePagamentosConfirmados();

        /// <summary>
        /// Exclui todos os pré-pagamentos temporários na base afim de realizar uma nova.
        /// </summary>
        /// <returns>Retorna se a execuçãou foi feita com sucesso</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean ExcluirPrePagamentosTemporarios();

        /// <summary>
        /// Exclui todos os pré-pagamentos parcelados na base afim de realizar uma nova.
        /// </summary>
        /// <returns>Retorna se a execuçãou foi feita com sucesso</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean ExcluirPrePagamentosParcelados();

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipoPessoa"></param>
        /// <param name="CpfCnpj"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        DadosVendedor ConsultaVendedor(string tipoPessoa, string CpfCnpj);

        /// <summary>
        /// Realiza a carga de Pré-Pagamentos na base do RQ
        /// </summary>
        /// <param name="prePagamentos">Listagem de Pré-Pagamentos a carregar</param>
        /// <param name="confirmados">Indica se os Pré-Pagamentos são do tipo Confirmados ou Agendados/Parcelados.
        /// True - Grava na tabela TBRQ0006; False - Grava na tabela TBRQ0008 </param>
        /// <returns>List of PrePagamento - Listagem de pré-pagamentos que retornaram erro</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<PrePagamento> CarregarPrePagamentos(List<PrePagamento> prePagamentos, Boolean confirmados);
    }
}
