#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :   
- [16/05/2012] – [André Garcia] – [Criação]
- [15/04/2016] – [Seygi Kutani] – [Rotina para que essa página possa ser acessada apos o usuario ja estar logado e ja ter selecionado um estabelecimento]
*/
#endregion

using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI;

namespace Redecard.PN.DadosCadastrais.SharePoint
{
    /// <summary>
    /// Controle que exibe as informações de login do usuário no Portal de Serviços
    /// </summary>
    public partial class LoginInfo : UserControlBase
    {


        /// <summary>
        /// Sobrescreve o Onload do Base para nao validar as permissoes
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            this.ValidarPermissao = false;
            base.OnLoad(e);
        }

        protected string HashUsuario
        {
            get
            {
                if (Session["hashacessopn"] != null)
                {
                    return Convert.ToString(Session["hashacessopn"]);
                }
                return null;
            }
            set
            {
                Session["hashacessopn"] = value;
            }
        }

        protected string HashCriptografada
        {
            get
            {
                if (Session["hashacessopnhasencript"] != null)
                {
                    return Convert.ToString(Session["hashacessopnhasencript"]);
                }
                return null;
            }
            set
            {
                Session["hashacessopnhasencript"] = value;
            }
        }
        
        protected Int32 QuantidadeEstabelecimentos
        {
            get
            {
                if (Session["quantidadepvs"] != null)
                {
                    return Convert.ToInt32(Session["quantidadepvs"]);
                }
                return 0;
            }
            set
            {
                Session["quantidadepvs"] = value;
            }
        }

        /// <summary>
        /// Carregamento da página, recuperar 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.ConsultarSessao();
            //if (!Page.IsPostBack)
            //{
            //    likAcessarFilial.Visible = false;
            //    Sessao sessao = null;
            //    if (Sessao.Contem())
            //    {
            //        sessao = Sessao.Obtem();

            //        if (this.QuantidadeEstabelecimentos.Equals(0))
            //        {
            //            List<Entidade1> lstEstabelecimentos = ConsultarEstabelecimentosEmailSenhaHash(sessao.LoginUsuario, this.HashUsuario);
            //            this.QuantidadeEstabelecimentos = lstEstabelecimentos.Count;
            //        }

            //        if (this.QuantidadeEstabelecimentos > 1)
            //        {
            //            QueryStringSegura qs = new QueryStringSegura();
            //            qs["estabelecimento"] = sessao.GrupoEntidade.ToString();
            //            qs["ncadastro"] = sessao.CodigoEntidade.ToString();
            //            qs["usuario"] = sessao.LoginUsuario;
            //            qs["senha"] = this.HashUsuario;
            //            qs["indSenhaCript"] = this.HashCriptografada;
            //            qs["origem"] = "Header";
            //            likAcessarFilial.NavigateUrl = String.Format("/_layouts/DadosCadastrais/LoginSelecionaEstabelecimento.aspx?dados={0}", qs.ToString());
            //            likAcessarFilial.Visible = true;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Consulta a sessão atual e preenche as informações nos literais da tela
        /// </summary>
        private void ConsultarSessao()
        {
            if (Sessao.Contem())
            {
                Sessao sessao = Sessao.Obtem();

                switch (sessao.GrupoEntidade)
                {
                    case 1:
                    case 14: //Central de Atendimento (Perfil 1)
                    case 16: //Central de Atendimento (Perfil 2)
                        ltTextoExtabelecimento.Text = "número do estabelecimento";
                        ltTextoEmpresa.Text = "Empresa";

                        #region Consulta Filiais
                        
                        if (Session["LoginInfoPossuiFiliais"] == null)
                        {
                            using (Logger Log = Logger.IniciarLog("Consultando Filiais"))
                            {
                                int codigoRetorno = 0;

                                // chama o serviço que carrega as filiais para o estabelecimento atual
                                using (EntidadeServico.EntidadeServicoClient _client = new EntidadeServico.EntidadeServicoClient())
                                {
                                    try
                                    {
                                        Session["LoginInfoPossuiFiliais"] = false;

                                        EntidadeServico.Filial[] filiais = _client.ConsultarFiliais(out codigoRetorno, sessao.CodigoEntidade, 2);
                                        if (codigoRetorno == 0 && filiais != null && filiais.Length > 0)
                                        {
                                            Session["LoginInfoPossuiFiliais"] = true;
                                        }
                                    }
                                    catch (FaultException<EntidadeServico.GeneralFault> ex)
                                    {
                                        Log.GravarMensagem("Erro ao consultar filiais");
                                        Log.GravarErro(ex);
                                        Session["LoginInfoPossuiFiliais"] = null;
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.GravarMensagem("Erro ao consultar filiais");
                                        Log.GravarErro(ex);
                                        Session["LoginInfoPossuiFiliais"] = null;
                                    }
                                }
                            }
                        }

                        #endregion

                        if (((bool?)Session["LoginInfoPossuiFiliais"]).GetValueOrDefault(false))
                        {
                            if (sessao.AcessoFilial)
                            {
                                // habilita link para acesso à matriz
                                likAcessarMatriz.Visible = true;
                            }
                            else
                            {
                                // habilita link para acesso às filiais
                                likAcessarFilial.Visible = true;
                            }
                        }

                        break;
                    case 3:
                        ltTextoExtabelecimento.Text = "nº do Emissor";
                        ltTextoEmpresa.Text = "Nome do Emissor";
                        break;
                    case 12:
                        ltTextoExtabelecimento.Text = "nº do Parceiro";
                        ltTextoEmpresa.Text = "Nome do Parceiro";
                        break;
                    default:
                        ltTextoExtabelecimento.Text = "nº da Empresa:";
                        ltTextoEmpresa.Text = "Empresa:";
                        break;
                }

                ltEstabelecimento.Text = sessao.CodigoEntidade.ToString().ToUpperInvariant();
                ltUsuario.Text = sessao.LoginUsuario;
                ltNomeUsuario.Text = sessao.NomeUsuario;
                ltEmpresa.Text = String.IsNullOrEmpty(sessao.NomeEntidade) ?
                    sessao.NomeUsuario.ToUpperInvariant() : sessao.NomeEntidade.ToUpperInvariant();
            }
        }

        /// <summary>
        /// Consulta todos os estabelecimentos associados ao e-mail e senha criptografada Hash
        /// </summary>
        /// <param name="email">E-mail</param>
        /// <param name="senha">Senha Criptografada</param>
        private List<Entidade1> ConsultarEstabelecimentosEmailSenhaHash(String email, String senha)
        {
            var codigoRetorno = default(Int32);
            var pvs = new List<Int32>();
            List<Entidade1> lstEntidades = new List<Entidade1>();

            using (Logger log = Logger.IniciarLog("Consulta estabelecimento por e-mail LogInfor.ascx"))
            {
                try
                {
                    var entidades = new Entidade1[0];

                    using (var ctx = new ContextoWCF<EntidadeServicoClient>())
                        entidades = ctx.Cliente.ConsultarPorEmailSenhaHash(out codigoRetorno, email, senha);

                    if (codigoRetorno != 0)
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarPorEmail", codigoRetorno);

                    if (entidades != null && entidades.Length > 0)
                        lstEntidades = entidades.ToList();
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return lstEntidades;
        }
    }
}