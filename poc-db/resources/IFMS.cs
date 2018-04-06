using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Redecard.PN.FMS.Servico.Modelo;
using Redecard.PN.FMS.Servico.Modelo.Transacoes;
using Redecard.PN.FMS.Servico.Modelo.Merchant;
using Redecard.PN.FMS.Servico.Modelo.CriterioSelecao;
using Redecard.PN.FMS.Servico.Modelo.CadastroIPs;

namespace Redecard.PN.FMS.Servico
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    /// <summary>
    /// Interface dos serviços FMS.
    /// </summary>
    [ServiceContract]
    public interface IFMS
    {
        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        PesquisarTransacoesPorNumeroCartaoEEstabelecimentoRetorno PesquisarTransacoesPorNumeroCartaoEEstabelecimento(PesquisarTransacoesPorNumeroCartaoEEstabelecimentoEnvio Envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginRetorno PesquisarTransacoesSuspeitasPorEmissorEUsuarioLogin(PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginEnvio Envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginRetorno ExportarTransacoesSuspeitasPorEmissorEUsuarioLogin(PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        PesquisaTipoRespostaRetorno PesquisarTiposResposta(PesquisaTipoRespostaEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        int AtualizarResultadoAnalise(AtualizarResultadoAnaliseEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        void DesbloquearCartao(DesbloquearCartaoEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        void BloquearCartao(BloquearCartaoEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        PesquisarCriteriosSelecaoPorUsuarioLoginRetorno PesquisarCriteriosSelecaoPorUsuarioLogin(PesquisarCriteriosSelecaoPorUsuarioLoginEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        PesquisarMCCRetorno PesquisarMerchantPorCodigoCategoria(PesquisarMCCenvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        PesquisarRangeBinPorEmissorRetorno PesquisarRangeBinPorEmissor(PesquisarRangeBinPorEmissorEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        string[] PesquisarUsuariosPorEmissor(int numeroEmissor, int grupoEntidade, string usuarioLogin);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        void AtualizarCriteriosSelecao(int numeroEmissor, string usuarioLogin, int grupoEntidade, PesquisarCriteriosSelecaoPorUsuarioLoginRetorno retornoAtualizado);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        TransacoesEmissorRetornoComQuantidade PesquisarTransacoesPorSituacaoEPeriodo(PesquisarTransacoesPorSituacaoEPeriodoEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        System.Collections.Generic.List<Redecard.PN.FMS.Servico.Modelo.TransacaoEmissor> ExportarTransacoesPorSituacaoEPeriodo(Redecard.PN.FMS.Servico.Modelo.Transacoes.PesquisarTransacoesPorSituacaoEPeriodoEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        TransacoesEmissorRetornoComQuantidade PesquisarTransacoesAnalisadasPorUsuarioEPeriodo(PesquisarTransacoesAnalisadasPorUsuarioEPeriodoEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        System.Collections.Generic.List<Redecard.PN.FMS.Servico.Modelo.TransacaoEmissor> ExportarTransacoesAnalisadasPorUsuarioEPeriodo(Redecard.PN.FMS.Servico.Modelo.PesquisarTransacoesAnalisadasPorUsuarioEPeriodoEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        Redecard.PN.FMS.Servico.Modelo.RelatorioProdutividadePorData PesquisarRelatorioProdutividadePorData(PesquisarRelatorioProdutividadeEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        Redecard.PN.FMS.Servico.Modelo.RelatorioProdutividadePorAnalista RelatorioProdutividadePorAnalista(PesquisarRelatorioProdutividadeEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesEstabelecimentoPorCartao PesquisarTransacoesEstabelecimentoAgrupadasPorCartao(TransacaoEstabelecimentoPorCartaoEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesEstabelecimentoPorCartao ExportarTransacoesEstabelecimentoAgrupadasPorCartao(Redecard.PN.FMS.Servico.Modelo.Transacoes.TransacaoEstabelecimentoPorCartaoEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesEstabelecimento PesquisarTransacoesAgrupadasPorEstabelecimento(TransacaoEstabelecimentoEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesPorCartao PesquisarTransacoesPorCartao(TransacaoPorCartaoEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        Redecard.PN.FMS.Servico.Modelo.Transacoes.RespostaTransacoesPorCartao ExportarTransacoesAgrupadasPorCartao(Redecard.PN.FMS.Servico.Modelo.Transacoes.TransacaoPorCartaoEnvio envio);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        Redecard.PN.FMS.Servico.Modelo.ParametrosSistema ListaParametrosSistema(int numeroEmissor, int grupoEntidade, string usuarioLogin);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        void DescartarSessaoPesquisaTransacoesSuspeitasPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        void DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimentoPorCartao(int numeroEmissor, int grupoEntidade, string usuarioLogin);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        void DescartarSessaoPesquisaTransacoesSuspeitasEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        void DescartarSessaoPesquisaTransacoesSuspeitas(int numeroEmissor, int grupoEntidade, string usuarioLogin);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        void BloquearEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin, long numeroEstabelecimento, int tempoLimite);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        void DesbloquearEstabelecimento(int numeroEmissor, int grupoEntidade, string usuarioLogin, long numeroEstabelecimento);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        void IncluirListaIps(List<IPsAutorizados> listaIPs, bool flagIpsAutorizados, int numeroEmissor, int grupoEntidade);

        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        List<IPsAutorizados> BuscarListaIPs(int numeroEmissor, int grupoEntidade, string usuarioLogin);

        #region Pesquisar transações por transação associada (findTransactionListByAssociatedTransaction).
        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        List<TransacaoEmissor> PesquisarTransacoesPorTransacaoAssociada(PesquisarTransacoesPorTransacaoAssociadaEnvio envio);
        #endregion

        #region PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociada (findAnalysedAndNotAnalysedTransactionListByAssociatedTransaction).
        [OperationContract]
        [FaultContract(typeof(BusinessFault))]
        [FaultContract(typeof(GeneralFault))]
        TransacoesEmissorRetornoComQuantidade PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociada(PesquisarTransacoesAnalisadasENaoAnalisadasPorTransacaoAssociadaEnvio envio);
        #endregion
    }
}
