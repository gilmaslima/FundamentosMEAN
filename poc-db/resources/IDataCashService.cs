using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.DataCash.Servicos
{
    [ServiceContract]
    public interface IDataCashService
    {
        #region [ Usuários E-Commerce ]

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean TrocarSenha(out Modelos.MensagemErro mensagemErro, Int32 codigoEntidade, String senha);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        String ConsultarSenha(out Modelos.MensagemErro mensagemErro, Int32 codigoEntidade);

        #endregion [ FIM: Usuários E-Commerce ]

        #region [ Gerenciamento IP e Urlback ]

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean ManutencaoGerenciamentoPV(out Modelos.MensagemErro mensagemErro, Modelos.GerenciamentoPV gerenciamentoPv);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelos.GerenciamentoPV ConsultaGerencimentoPV(Int32 pv);

        #endregion [ FIM: Gerenciamento IP e Urlback ]

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelos.TotalTransacoes GetTotalTransacoes(DateTime dataInicio, DateTime dataFim, Int32 pv);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelos.RetornoDistribuidor ConsultarDistribuidores(Int32 pv, Int32 numeroPagina);
       
        #region Gerenciamento AVS

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<GrupoAVS> ConsultarOpcoesAVS(Int32 pv);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        String ConsultarRegraAVS(Int32 pv, out Modelos.MensagemErro mensagemErro);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean GerenciarRegraAVS(Int32 pv, Char avs, out Modelos.MensagemErro mensagemErro);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<GrupoAVS> AtualizarGrupoAVS(Int32 pv, List<GrupoAVS> lstGrupos);

        #endregion

        #region Configurações Boleto

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelos.Boleto ConsultarBoleto(Int32 pv, out Modelos.MensagemErro mensagemErro);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean GerenciarBoleto(Int32 pv, Modelos.Boleto boleto, out Modelos.MensagemErro mensagemErro);

        #endregion

        #region [ Perfil ]

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ConsultaPerfilPV(Int32 pv, out Modelos.MensagemErro mensagemErro);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean PerfilPVFornecedor(Int32 pv, out Modelos.MensagemErro mensagemErro);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean PerfilPVDistribuidor(Int32 pv, out Modelos.MensagemErro mensagemErro);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean PerfilPVComum(Int32 pv, out Modelos.MensagemErro mensagemErro);

        #endregion

        #region [Entidade]

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Int32 ConsultarNumeroParcelas(Int32 codigoEntidade);
        
        #endregion

        #region [ Bandeiras Adicionais ]

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Boolean GravarAtualizarBandeirasAdicionais(Int32 pv, out Modelos.MensagemErro mensagemErro, String numeroAfiliacaoPdv, String chaveConfiguracaoPdv);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelos.RetornoBandeirasAdicionais ConsultaBandeirasAdicionais(Int32 pv, out Modelos.MensagemErro mensagemErro);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelos.RetornoServicoPV ListaServicoPV(Int32 pv);
        
        #endregion
    }
}
