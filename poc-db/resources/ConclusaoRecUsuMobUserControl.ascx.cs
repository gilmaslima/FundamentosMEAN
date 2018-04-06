/*
© Copyright 2015 Rede S.A.
Autor : William Santos
Empresa : Rede
*/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Rede.PN.DadosCadastraisMobile.SharePoint.Util;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.ConclusaoRecuperacaoUsuarioMob
{
    public partial class ConclusaoRecuperacaoUsuarioMobUserControl : UserControlBase
    {
        #region [Controles da WebPart]

        /// <summary>
        /// qdAviso control.
        /// </summary>
        protected QuadroAviso QdAviso { get { return (QuadroAviso)qdAviso; } }

        /// <summary>
        /// Webpart de Conclusao Recuperação Usuario
        /// </summary>
        public ConclusaoRecuperacaoUsuarioMob WebPartDadosIniciais { get; set; }

        #endregion

        private InformacaoUsuario _infoUsuario
        {
            
            get
            {
                if (InformacaoUsuario.Existe())
                {
                    return InformacaoUsuario.Recuperar();
                }

                return null;

            } 
        }

        #region [Eventos da WebPart]
        /// <summary>
        /// Inicialização da webpart do passo de conclusão de recuperação de usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ChangeFlagPvsFiltrados();

            if (!IsPostBack)
            {
                using (Logger log = Logger.IniciarLog("Inicialização da webpart do passo de conclusão de recuperação de usuário"))
                {
                    try
                    {
                        if (InformacaoUsuario.Existe())
                        {
                            if (_infoUsuario.PodeRecuperarCriarAcesso)
                            {
                                string textoEmail = string.Empty;
                                if (_infoUsuario.PvsSelecionados != null && _infoUsuario.PvsSelecionados.Count() == 1)
                                {
                                    var email = _infoUsuario.EstabelecimentosRelacinados.FirstOrDefault(x => _infoUsuario.PvsSelecionados.Contains(x.NumeroPV));

                                    if (email != null)
                                    {
                                        textoEmail = email.Email;
                                    }
                                    else
                                    {
                                        textoEmail = _infoUsuario.EmailUsuario;
                                    }
                                }
                                else
                                {
                                    textoEmail = _infoUsuario.EmailUsuario;
                                }

                                if (!String.IsNullOrWhiteSpace(textoEmail))
                                {
                                    if (textoEmail.IndexOf('@') >= 0)
                                        ExibirUsuarioUnico(textoEmail);
                                    else
                                        lblEmailUsuario.Text = textoEmail;
                                }

                                CarregarPvsRelacionadosAoUsu();

                                //Registra no histórico/log de atividades
                                Historico.RecuperacaoUsuario(
                                    _infoUsuario.IdUsuario,
                                    _infoUsuario.NomeCompleto,
                                    _infoUsuario.EmailUsuario,
                                    _infoUsuario.TipoUsuario,
                                    _infoUsuario.NumeroPV);

                                AtualizarStatus();
                            }
                            else
                            {
                                this.ExibirErro("SharePoint.PodeRecuperarAcessoSenha", 1154, "Atenção", this.RecuperarEnderecoPortal());
                            }

                        }
                    }
                    catch (PortalRedecardException ex)
                    {
                        log.GravarErro(ex);
                        this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", this.RecuperarEnderecoPortal());
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", this.RecuperarEnderecoPortal());
                    }
                }
            }
        }

        /// <summary>
        /// Verifica se existem pvs relacionados ao usuário, caso exista chama o método para carregar a grid
        /// </summary>
        /// <param name="maisDeUmPvMesmoUsu">Informa que exinstem mais de um pv para o mesmo usuário (CPF ou E-mail)</param>
        private void CarregarPvsRelacionadosAoUsu()
        {
            if (!IsVisibleListaPvs())
            {
                EntidadeServico.EntidadeServicoModel[] arrayPvs = GetPVsComEmail();
                if (arrayPvs != null && arrayPvs.Count() > 1)
                {
                    var arrDistinct = arrayPvs.GroupBy(p => p.Email).Select(p => p.First());

                    if (arrDistinct != null && arrDistinct.Count() == 1)
                    {
                        this.lblRecUsuConclusao.Text = "O usuário cadastrado para os estabelecimentos selecionados é:";
                        lblEmailUsuario.Text = this.ExibirUsuarioUnico(arrDistinct.First().Email); 
                        pnlEmailsPvs.Visible = false;
                        pnlEmailCadastro.Visible = true;
                        return;
                    }
                    pnlEmailsPvs.Visible = true;
                    pnlEmailCadastro.Visible = false;
                    this.ExibirListaPvs(arrayPvs);
                    return;
                }
                else
                {
                    if (arrayPvs != null)
                    {
                        var model = arrayPvs.FirstOrDefault();
                        if (model != null)
                        {
                            if (!String.IsNullOrWhiteSpace(model.Email))
                            {
                                ExibirUsuarioUnico(model.Email);
                            }
                        }
                    }
                    pnlEmailsPvs.Visible = false;
                    pnlEmailCadastro.Visible = true;
                }
            }
        }

        /// <summary>
        /// Verifica se a lista de Pv está visível
        /// </summary>
        /// <returns></returns>
        private bool IsVisibleListaPvs()
        {
            return pnlEmailsPvs.Visible;
        }


        /// <summary>
        /// Retorna PVs selecionados
        /// </summary>
        /// <returns></returns>
        private EntidadeServico.EntidadeServicoModel[] GetPVsComEmail()
        {
            int codigoRetorno = 0;
            Int64 cpf = 0;
            int totalRows = 0;
            EntidadeServico.EntidadeServicoModel[] arrayPvs = null;
            Int32 quantidadeEmaislDiferentes = 0;
            using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
            {
                cpf = GetCPF();

                using (Logger log = Logger.IniciarLog("Seleciona os Pvs que estão relacionados ao CPF ou E-mail"))
                {
                    try
                    {
                        if (cpf != 0)
                        {
                            arrayPvs = contextoEntidade.Cliente.ConsultarPvPorCpfComPaginacao(out codigoRetorno, out totalRows, out quantidadeEmaislDiferentes, cpf, 1, 9999, true, GetPvsSelecionados(), txtBuscaPv.Text);
                            arrayPvs = Filtrar(arrayPvs);
                        }
                    }
                    catch (FaultException<UsuarioServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                    }
                    catch (HttpException ex)
                    {
                        log.GravarErro(ex);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                    }
                }

                return arrayPvs;
            }
        }

        /// <summary>
        /// Retorna os Pvs selecionados
        /// </summary>
        /// <returns></returns>
        private string GetPvsSelecionados()
        {
            if (InformacaoUsuario.Existe())
            {
                return _infoUsuario.GetPvsSelecionados();
            }

            return null;
        }

        private long GetCPF()
        {
            if(_infoUsuario != null)
                return _infoUsuario.CpfUsuario;
            return 0;
        }

        /// <summary>
        /// Filtra para apenas retornar ps Pvs selecionados no passo 1
        /// </summary>
        /// <param name="arrayPvs"></param>
        /// <returns></returns>
        private EntidadeServico.EntidadeServicoModel[] Filtrar(EntidadeServico.EntidadeServicoModel[] arrayPvs)
        {
            if (_infoUsuario != null)
            {
                if (_infoUsuario.PvsSelecionados != null && _infoUsuario.PvsSelecionados.Any())
                    return arrayPvs.Where(x => _infoUsuario.PvsSelecionados.Contains(x.NumeroPV))
                                   .ToArray();
                else
                {
                    if (_infoUsuario.QuantidadeEmaislDiferentes > 1)
                        return arrayPvs;
                }
            }
            return null;
        }

        /// <summary>
        /// Atualisa o status
        /// </summary>
        private void AtualizarStatus()
        {
            InformacaoUsuario info;

            if (!InformacaoUsuario.Existe())
            {
                this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", this.RecuperarEnderecoPortal());
            }

            if (_infoUsuario.PvsSelecionados == null || !_infoUsuario.PvsSelecionados.Any())
                _infoUsuario.PvsSelecionados = new Int32[] { _infoUsuario.NumeroPV };


            info = InformacaoUsuario.Recuperar();

            using (Logger log = Logger.IniciarLog("Atualizando status usuário"))
            {
                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                    {
                        log.GravarMensagem("Preparando para atualizar status");

                        ctx.Cliente.AtualizarStatusPorPvs(info.PvsSelecionados,
                                                            info.CpfUsuario,
                                                            null,
                                                            UsuarioServico.Status1.RespostaIdPosPendente);

                        log.GravarMensagem("Staus atualizado com sucesso", new { status = UsuarioServico.Status1.RespostaIdPosPendente });
                    }

                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", this.RecuperarEnderecoPortal());
                }

            }
        }

        /// <summary>
        /// Exibe email do usuário truncado.
        /// </summary>
        /// <param name="emailUsuario"></param>
        /// <returns></returns>
        private string ExibirUsuarioUnico(String emailUsuario)
        {
            this.pnlEmailCadastro.Visible = true;
            this.pnlEmailsPvs.Visible = false;
            string emailExibir = Redecard.PN.Comum.Util.TruncarEmailUsuario(emailUsuario);
            lblEmailUsuario.Text = emailExibir;
            return emailExibir;
        }

        /// <summary>
        /// Exibe a lista de PVs
        /// </summary>
        /// <param name="entidades"></param>
        private void ExibirListaPvs(EntidadeServico.EntidadeServicoModel[] entidades)
        {
            this.pnlEmailsPvs.Visible = true;
            this.pnlEmailCadastro.Visible = false;
            this.rptListaPvs.DataSource = entidades;
            this.rptListaPvs.DataBind();
        }

        #endregion

        #region [Métodos privados]

        /// <summary>
        /// Exibe uma mensagem de erro
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="urlVoltar">Url de retorno</param>
        private void ExibirErro(String mensagem, String titulo, String urlVoltar)
        {
            if (String.IsNullOrEmpty(urlVoltar))
                QdAviso.CarregarMensagem(titulo, mensagem, QuadroAviso.IconeMensagem.Erro);
            else
                QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAviso.IconeMensagem.Erro);

            pnlAvisoConclusao.Visible = true;
            pnlConclusao.Visible = false;
        }

        /// <summary>
        /// Exibe a mensagem de erro
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <param name="titulo">Título para o quadro de aviso</param>
        /// <param name="urlVoltar">Url de redirecionamento</param>
        private void ExibirErro(String fonte, Int32 codigo, String titulo, String urlVoltar)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);

            if (String.IsNullOrEmpty(urlVoltar))
                QdAviso.CarregarMensagem(titulo, mensagem, QuadroAviso.IconeMensagem.Erro);
            else
                QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAviso.IconeMensagem.Erro);

            pnlAvisoConclusao.Visible = true;
            pnlConclusao.Visible = false;
        }
        #endregion

        protected void btnNao_Click(object sender, EventArgs e)
        {
            String link = base.RecuperarEnderecoPortal();
            InformacaoUsuario.Limpar();
            Response.Redirect(link);
        }

        protected void btnSim_Click(object sender, EventArgs e)
        {
            String link = String.Format("{0}/Paginas/Mobile/RecuperacaoSenhaIdentificacao.aspx", base.web.ServerRelativeUrl);
            InformacaoUsuario.Limpar();
            Response.Redirect(link);

        }

        protected void btnBuscarPvs_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ChangeFlagPvsFiltrados();

            if (!InformacaoUsuario.Existe())
            {
                return;
            }

            InformacaoUsuario info = InformacaoUsuario.Recuperar();

            // valida se algum estabelecimento foi selecionado
            if (info.EstabelecimentosRelacinados == null)
                return;

            // obtém as entidades com base no filtro realizado no primeiro passo
            var entidades = GetPVsComEmail();
            if (entidades != null)
            {
                this.rptListaPvs.DataSource = entidades
                    .Where(x => string.Concat(x.NumeroPV, x.Email)
                        .ToUpper()
                        .Contains(this.txtBuscaPv.Text.ToUpper()))
                    .ToArray();
                this.rptListaPvs.DataBind();
            }
        }


        private void ChangeFlagPvsFiltrados()
        {
            if (!string.IsNullOrEmpty(txtBuscaPv.Text))
            {
                hdnPvsFiltrados.Value = "1";
                return;
            }

            hdnPvsFiltrados.Value = "0";

        }

    }
}
