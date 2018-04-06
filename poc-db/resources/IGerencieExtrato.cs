using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.GerencieExtrato.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IGerencieExtrato" in both code and config file together.
    [ServiceContract]
    public interface IGerencieExtrato
    {
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ExtratoEmitido> ListaExtratos(Int32 codigoEstabelecimento,
            Int32 numeroExtrato, ref Int16 totalRegistros, ref Int16 ts_reg, ref String mensagem,
            ref Int16 qtdOcorrencias, ref Int16 codigoRetorno);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Extrato> ConsultarExtrato(ref Int32 codigoEstabelecimento, ref Int32 numeroExtrato, ref String tipoAcesso,
            ref Int16 codigoRetorno, ref String mensagem, ref Int32 sequencia);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void ObterExtratoPapel(ref List<StatusEmissao> solicita, ref Int16 codigoRetorno, ref String mensagemRetorno);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void Extrato_Email(ref String tipo, ref Int32 codigoEstabelecimento, ref Decimal cnpjSolicitante,
           ref String periodicidadeEnvio, ref String diaEnvio, ref String tipoPVSolicitante, ref String tipoSolicitacao,
           ref String nomeUsuario, ref String nomeEmailrecebimento, ref String fraseCriptografada,
           ref List<CadeiaPV> lstCadeia,
           ref List<String> codigoBoxes, 
           ref Int32 codigoErro,
           ref String mensagemRetorno,
           ref String quantidadePvs,
           ref String identificadorContinuacao, ref String acao);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void InibirExtPapel(ref List<DadosPV> dados);

        /// <summary>
        /// Retorna as informações de relatório de preço único, identificação, descrição e período.<br/>
        /// Utilizado no Projeto Turquia / Preço Único<br/>
        /// - Book BKWA2890 / Programa WAC289 / TranID WAGA / Método ConsultarRelatorioPrecoUnico
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2890 / Programa WAC289 / TranID WAGA / Método ConsultarRelatorioPrecoUnico
        /// </remarks>
        /// <param name="numeroPv">Número do Ponto de Venda (Número Estabelecimento) que se deseja consultar.</param>
        /// <param name="codigoRetorno">Cógigo retornado pelo Mainframe indicando o status da execução do comando.</param>
        /// <returns>Retorna identificação, descrição e período do relatório de Preço Único</returns>  
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ExtratoRelatorioPrecoUnico> ConsultarRelatorioPrecoUnico(
           Int32 numeroPv,
           out Int16 codigoRetorno);

        /// <summary>
        /// Retorna o detalhamento do relatório Preço Único. <br/>
        /// Utilizado no Projeto Turquia / Preço Único<br/>
        /// - Book BKWA2900 / Programa WAC290 / TranID WABG / Método DetalharRelatorioPrecoUnico
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2900 / Programa WAC290 / TranID WABG / Método DetalharRelatorioPrecoUnico
        /// </remarks>
        /// <param name="numeroPv">Número do Ponto de Venda (Número Estabelecimento) que se deseja consultar.</param>
        /// <param name="mesAnoRelatorio">Mês e ano que se deseja consultar.</param>
        /// <param name="flagVsam">Flag retornado pela consulta de relatório Preço Único</param>
        /// <param name="codigoRetorno">Cógigo retornado pelo Mainframe indicando o status da execução do comando.</param>
        /// <returns>Relatório detalhado Preço Único </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<RelatorioDetalhadoPrecoUnico> DetalharRelatorioPrecoUnico(
            Int32 numeroPv,
            DateTime mesAnoRelatorio,
            Int16 flagVsam,
            out Int16 codigoRetorno);

        /// <summary>
        /// Solicita a recuperação de Relatório Preço Único baseado em um período.<br/>
        /// Utilizado no Projeto Turquia / Preço Único<br/>
        /// - Book BKWA2910 / Programa WAS291 / TranID WAGC / Método SolicitarRelatorioPrecoUnico
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2910 / Programa WAS291 / TranID WAGC / Método SolicitarRelatorioPrecoUnico
        /// </remarks>
        /// <param name="numeroPv">Número do Ponto de Venda (Número Estabelecimento) que se deseja solicitar o relatório.</param>
        /// <param name="mesAnoDe">Mês inicial que relatório deve contemplar.</param>
        /// <param name="mesAnoAte">Mês final até o qual o relatório deve contemplar.</param>
        /// <param name="codigoRetorno">Cógigo retornado pelo Mainframe indicando o status da execução do comando.</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void SolicitarRelatorioPrecoUnico(
            Int32 numeroPv,
            DateTime mesAnoDe,
            DateTime mesAnoAte,
            out Int16 codigoRetorno);
    }
}