using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.Emissores.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IHisServicoZpEmissores" in both code and config file together.
    [ServiceContract]
    public interface IHisServicoZpEmissores
    {
        /// <summary>
        /// Consulta Trava de Domicilio por Periodo
        /// </summary>
        /// <param name="numEmissor"></param>
        /// <param name="mes"></param>
        /// <param name="ano"></param>
        /// <param name="PV"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        EntidadeConsultaTrava ConsultaPeriodo(Int16 numEmissor, Int32 codigoProduto, Int16 mes, Int16 ano, out Int16 codRetorno);

        /// <summary>
        /// Consulta Trava de Domicilio por Periodo do establecimento ou cnpj/cpf
        /// </summary>
        /// <param name="numEmissor"></param>
        /// <param name="mes"></param>
        /// <param name="ano"></param>
        /// <param name="PV"></param>
        /// <param name="cnpj"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        EntidadeConsultaTrava ConsultarTotaisCobranca(Int16 funcao,
            Int32 numPv,
            decimal cnpj,
            String dataDe,
            String dataAte,
            Int16 codigoBanco,
            Int32 codigoProduto,
            Int16 anoCompetencia,
            Int16 mesCompetencia,
            decimal precoMedioReferencia,
            out Int16 codigoRetorno,
            out String mensagemRetorno);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<InformacaoCobranca> ConsultaInformacaoCobranca(Int16 numEmissor, Int16 codigoProduto, Int16 mes, Int16 ano, out Int16 codigoErro);


        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<InformacaoPVCobrada> ConsultarInformacoesPVCobranca(Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            Int16 funcao, Int32 numeroPv, Decimal cnpj, String datade,
            String datate, Int16 codBanco, Int32 codigoProduto, Int16 anoCompetencia, Int16 mesCompetencia, Decimal faixaInicialFaturamento,
            Decimal faixaFinalFaturamento, Decimal fatorMultiplicador, out Int16 codigoRetorno, out String mensagemRetorno);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<InformacaoDetalhada> ConsultarInformacoesDetalhadas(Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros, Int16 codBanco, Int32 codigoProduto, Int16 anoCompetencia, Int16 mesCompetencia,
            Decimal faixaInicialFaturamento, Decimal faixaFinalFaturamento, out Int16 codigoRetorno, out String mensagemRetorno);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<DetalheFatura> ConsultarDetalheFatura(Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros, Int16 codBanco, Int32 codigoProduto, Int16 anoCompetencia,
            Int16 mesCompetencia, Decimal faixaInicialFaturamento, Decimal faixaFinalFaturamento, Int32 pvOriginal,
            out Int16 codigoRetorno, out String mensagemRetorno);
    }
}
