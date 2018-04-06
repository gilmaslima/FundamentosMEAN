/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.ServiceModel;
using System.Web;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using System.Linq;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.MeuUsuario
{
    /// <summary>
    /// Classe base para as telas do item Meu Usuário
    /// </summary>
    public class MeuUsuarioUserControlBase : UserControlBase
    {
        #region [ Propriedades ]

        /// <summary>
        /// Retorna dados do usuário atual logado
        /// </summary>
        protected Usuario UsuarioAtual
        {
            get
            {
                if (ViewState["Usuario"] == null)
                    ViewState["Usuario"] = this.ConsultarUsuario(SessaoAtual.CodigoIdUsuario);
                return (Usuario)ViewState["Usuario"];
            }
        }

        /// <summary>
        /// Retorna os estabelecimentos do usuário atual logado
        /// </summary>
        protected Int32[] EstabelecimentosUsuarioAtual
        {
            get
            {
                if (ViewState["EstabelecimentosUsuarioAtual"] == null)
                {
                    EntidadeServico.Entidade1[] entidades = this.ConsultarEstabelecimentosUsuario(SessaoAtual.CodigoIdUsuario);
                    if (entidades != null && entidades.Length > 0)
                        ViewState["EstabelecimentosUsuarioAtual"] = entidades.Select(e => e.Codigo).ToArray();
                    else
                        ViewState["EstabelecimentosUsuarioAtual"] = new Int32[0];
                }
                return (Int32[])ViewState["EstabelecimentosUsuarioAtual"];
            }
        }

        #endregion

        #region [ Override - Eventos ]

        /// <summary>
        /// Verificação inicial das telas de Usuários
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            //Desabilta ValidarPemissão para Meu Usuário, pois todos DEVEM ter acesso
            this.ValidarPermissao = false;

            base.OnInit(e);

            //Se for usuário não migrado, força redirecionamento para tela de Migração
            if (Sessao.Contem() && this.SessaoAtual.Legado)
            {
                //Nome da página de cadastro da migração do usuário
                String paginaCadastroUsuarioMigracao = "CadastroUsuarioMigracao.aspx";
                //Página atual
                String paginaAspx = System.IO.Path.GetFileName(Request.PhysicalPath);

                //Se for a própria página de cadastro de migração, não pode redirecionar, para não ficar em loop
                Boolean redirecionar = String.Compare(paginaAspx, paginaCadastroUsuarioMigracao, true) != 0;

                //Redireciona para página de cadastro de migração do usuário
                if (redirecionar)
                    Response.Redirect(paginaCadastroUsuarioMigracao);
            }
        }

        #endregion

        #region [ Métodos ]

        /// <summary>
        /// Valida a senha informada com a senha do usuário logado
        /// </summary>
        protected Boolean VerificarSenhaAtual(String senhaInformadaDescriptografada)
        {
            if (this.UsuarioAtual != null)
            {
                String senhaInformada = CampoSenha.Criptografar(senhaInformadaDescriptografada);
                String senhaUsuario = this.UsuarioAtual.Senha;
                return String.Compare(senhaInformada, senhaUsuario, false) == 0;
            }
            else
                return false;
        }

        /// <summary>
        /// Redireciona a tela Meu Usuário para as subtelas.
        /// </summary>
        /// <param name="exibirMensagemSucesso">Exibe mensagem de sucesso ao redirecionar</param>
        protected void RedirecionarParaMeuUsuario(Boolean exibirMensagemSucesso, Boolean exibirAvisoAlteracaoEmail)
        {
            var qs = new QueryStringSegura();
            qs["ExibirMensagemSucesso"] = exibirMensagemSucesso.ToString();
            qs["AlteracaoEmail"] = exibirAvisoAlteracaoEmail.ToString();
            String url = String.Format("{0}?dados={1}", "MeuUsuario.aspx", qs.ToString());

            Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        #endregion

        #region [ Métodos - Consultas ]

        /// <summary>
        /// Consulta um usuário por ID
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        protected Usuario ConsultarUsuario(Int32 codigoIdUsuario)
        {
            var usuario = default(Usuario);

            using (Logger log = Logger.IniciarLog("Consultando usuário por ID"))
            {
                var codigoRetorno = default(Int32);

                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                        usuario = ctx.Cliente.ConsultarPorID(out codigoRetorno, codigoIdUsuario);

                    // Caso o código de retorno seja != de 0, ocorreu um erro
                    if (codigoRetorno > 0)
                        base.ExibirPainelExcecao("UsuarioServico.Consultar", codigoRetorno);
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

            return usuario;
        }

        /// <summary>
        /// Consulta os Estabelecimentos do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código id do usuário</param>
        private EntidadeServico.Entidade1[] ConsultarEstabelecimentosUsuario(Int32 codigoIdUsuario)
        {
            var estabelecimentos = new EntidadeServico.Entidade1[0];

            using (Logger log = Logger.IniciarLog("Consultando permissões do usuário"))
            {
                try
                {
                    Int32 codigoRetorno = default(Int32);
                    using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        estabelecimentos = ctx.Cliente.ConsultarPorUsuario(out codigoRetorno, codigoIdUsuario);

                    // Caso o código de retorno seja != de 0, ocorreu um erro
                    if (codigoRetorno > 0)
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarPorUsuario", codigoRetorno);
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
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
            return estabelecimentos;
        }

        #endregion
    }
}