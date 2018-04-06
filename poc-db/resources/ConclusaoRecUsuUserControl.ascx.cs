using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais;
using System.Linq;
using System.Collections.Generic;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using System.ServiceModel;
using System.Web;
using Microsoft.SharePoint.Administration;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.ConclusaoRecuperacaoUsuario
{
    /// <summary>
    /// Webpart do passo de conclusão de recuperação de usuário
    /// </summary>
    public partial class ConclusaoRecuperacaoUsuarioUserControl : UserControlBase
    {
        #region [Controles da WebPart]

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

        /// <summary>
        /// qdAviso control.
        /// </summary>
        protected QuadroAviso QdAviso { get { return (QuadroAviso)qdAviso; } }

        /// <summary>
        /// txtEmailUsuario control.
        /// </summary>
        protected CampoNovoAcesso TxtEmailUsuario { get { return (CampoNovoAcesso)txtEmailUsuario; } }

        #endregion

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

                                if (_infoUsuario.PvsSelecionados != null && _infoUsuario.PvsSelecionados.Count() == 1)
                                {
                                    var email = _infoUsuario.EstabelecimentosRelacinados.FirstOrDefault(x => _infoUsuario.PvsSelecionados.Contains(x.NumeroPV));

                                    if (email != null)
                                    {
                                        TxtEmailUsuario.Text = email.Email;
                                    }
                                    else
                                    {
                                        TxtEmailUsuario.Text = _infoUsuario.EmailUsuario;
                                    }
                                }
                                else
                                {
                                    TxtEmailUsuario.Text = _infoUsuario.EmailUsuario;
                                }

                                if (!String.IsNullOrWhiteSpace(TxtEmailUsuario.Text))
                                {
                                    if (TxtEmailUsuario.Text.IndexOf('@') >= 0)
                                        lblEmailUsuario.Text = this.TruncarEmail(TxtEmailUsuario.Text);
                                    else
                                        lblEmailUsuario.Text = TxtEmailUsuario.Text;
                                }

                                lnkSim.NavigateUrl = String.Format("{0}/Paginas/RecuperacaoSenhaIdentificacao.aspx", base.web.ServerRelativeUrl);
                                lnkNao.NavigateUrl = base.RecuperarEnderecoPortal();

                                CarregarPvsRelacionadosAoUsu();
                                //Registra no histórico/log de atividades
                                HistoricoRecuperacaoUsuario(_infoUsuario.IdUsuario,
                                                            _infoUsuario.NomeCompleto,
                                                            _infoUsuario.EmailUsuario,
                                                            _infoUsuario.TipoUsuario,
                                                            _infoUsuario.PvsSelecionados);

                                AtualizarStatus();
                            }
                            else
                            {
                                this.ExibirErro("SharePoint.PodeRecuperarAcessoSenha", 1154, "Atenção", this.RecuperarEnderecoPortal());
                            }
                        }
                        else
                        {
                            Response.Redirect(this.RecuperarEnderecoPortal(), false);
                        }
                    }
                    catch (FaultException<EntidadeServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", this.RecuperarEnderecoPortal());

                        return;
                    }
                    catch (HttpException ex)
                    {
                        log.GravarErro(ex);
                        this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", this.RecuperarEnderecoPortal());

                        return;
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
        /// Grava um histórico dos pvs 
        /// </summary>
        /// <param name="idUsuario">Id do usuário</param>
        /// <param name="nomeCompleto">Nome completo</param>
        /// <param name="emailUsuario">Email usuário</param>
        /// <param name="tipoUsuario">Tipo usuário</param>
        /// <param name="pvsSelecionados">Pvs selecionados</param>
        private void HistoricoRecuperacaoUsuario(int idUsuario, string nomeCompleto, string emailUsuario, string tipoUsuario, int[] pvsSelecionados)
        {
            foreach (var numPv in pvsSelecionados)
            {
                Historico.RecuperacaoUsuario(idUsuario,
                                             nomeCompleto,
                                             emailUsuario,
                                             tipoUsuario,
                                             numPv);
            }
        }

        /// <summary>
        /// Atualização do Status do usuário
        /// </summary>
        private void AtualizarStatus()
        {
            if (!InformacaoUsuario.Existe())
            {
                this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", this.RecuperarEnderecoPortal());
            }

            if (_infoUsuario.PvsSelecionados == null || !_infoUsuario.PvsSelecionados.Any())
                _infoUsuario.PvsSelecionados = new Int32[] { _infoUsuario.NumeroPV };


            using (Logger log = Logger.IniciarLog("Atualizando status usuário"))
            {
                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                    {
                        log.GravarMensagem("Preparando para atualizar status");

                        ctx.Cliente.AtualizarStatusPorPvs(_infoUsuario.PvsSelecionados,
                                                            _infoUsuario.CpfUsuario,
                                                            null,
                                                            UsuarioServico.Status1.RespostaIdPosPendente);

                        log.GravarMensagem("Staus atualizado com sucesso", new { status = UsuarioServico.Status1.RespostaIdPosPendente });
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", this.RecuperarEnderecoPortal());

                    return;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", this.RecuperarEnderecoPortal());

                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", this.RecuperarEnderecoPortal());
                }

            }
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

        /// <summary>
        /// Altera a flag que informa que os Pvs foram filtrados
        /// </summary>
        private void ChangeFlagPvsFiltrados()
        {
            if (!string.IsNullOrEmpty(txtBusca.Text))
            {
                hdnPvsFiltrados.Value = "1";
                return;
            }

            hdnPvsFiltrados.Value = "0";

        }
        #endregion


        #region Seleção PVs

        /// <summary>
        /// Verifica se existem pvs relacionados ao usuário, caso exista chama o método para carregar a grid
        /// </summary>
        /// <param name="maisDeUmPvMesmoUsu">Informa que exinstem mais de um pv para o mesmo usuário (CPF ou E-mail)</param>
        private void CarregarPvsRelacionadosAoUsu()
        {
            if (!IsVisibleListaPvs())
            {
                EntidadeServico.EntidadeServicoModel[] arrayPvs = GetPVsSemPaginacao();
                if (arrayPvs != null && arrayPvs.Count() > 1)
                {
                    //Verifica se é o mesmo e-mail para mais de um estabelecimento.
                    var arrDistinct = arrayPvs.GroupBy(p => p.Email).Select(p => p.First());

                    if (arrDistinct != null && arrDistinct.Count() == 1)
                    {
                        this.litRecUsuConclusao.Text = "O usuário cadastrado para os estabelecimentos selecionados é:";
                        lblEmailUsuario.Text = this.TruncarEmail(arrDistinct.First().Email);
                        pnlUsuarios.Visible = false;
                        pnUmUsuario.Visible = true;
                        return;
                    }
                    pnlUsuarios.Visible = true;
                    pnUmUsuario.Visible = false;
                    VisibleListaPvs(true);
                    this.paginacaoSelecaoPvs.Carregar();
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
                                lblEmailUsuario.Text = this.TruncarEmail(model.Email);
                            }
                        }
                    }
                    pnlUsuarios.Visible = false;
                    pnUmUsuario.Visible = true;
                }
            }
        }

        private string TruncarEmail(string email)
        {
            TxtEmailUsuario.Text = email;
            CampoEmail cpEmail = (TxtEmailUsuario.Campo as CampoEmail);

            String contaEmail = cpEmail.Conta;
            String dominioEmail = cpEmail.Dominios;

            return String.Format("{0}***{1}@{2}", contaEmail.Left(1), contaEmail.Right(1), dominioEmail);
        }

        /// <summary>
        /// Obtem os Pvs sem paginação
        /// </summary>
        /// <returns></returns>
        public EntidadeServico.EntidadeServicoModel[] GetPVsSemPaginacao()
        {
            int totalRows;

            return GetPVs(null, null, out totalRows);
        }

        /// <summary>
        /// Seleciona os Pvs com Paginação
        /// </summary>
        /// <param name="pagina">Página</param>
        /// <param name="qtdRegistros">Quantidade de registros por página</param>
        /// <param name="totalRows">Total de registros</param>
        /// <returns></returns>
        public EntidadeServico.EntidadeServicoModel[] GetPVsComPaginacao(int pagina, int qtdRegistros, out int totalRows)
        {
            return GetPVs(pagina, qtdRegistros, out totalRows);
        }

        /// <summary>
        /// Retorna PVs selecionados
        /// </summary>
        /// <returns></returns>
        private EntidadeServico.EntidadeServicoModel[] GetPVs(int? pagina, int? qtdRegistros, out int totalRows)
        {
            int codigoRetorno = 0;
            Int64 cpf = 0;
            totalRows = 0;
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
                            arrayPvs = contextoEntidade.Cliente.ConsultarPvPorCpfComPaginacao(out codigoRetorno, out totalRows, out quantidadeEmaislDiferentes, cpf, pagina.HasValue ? pagina.Value : 1, qtdRegistros.HasValue ? qtdRegistros.Value : 9999, true, GetPvsSelecionados(), txtBusca.Text);
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
        /// Filtra para apenas retornar ps Pvs selecionados no passo 1
        /// </summary>
        /// <param name="arrayPvs"></param>
        /// <returns></returns>
        private EntidadeServicoModel[] Filtrar(EntidadeServicoModel[] arrayPvs)
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
        /// Pega os pvs que estão no cache, caso não existe retorna null
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="cpf">Cpf</param>
        /// <returns></returns>
        private EntidadeServicoModel[] GetPvsCache(string email, Int64? cpf)
        {
            if (_infoUsuario != null)
            {
                return _infoUsuario.EstabelecimentosRelacinados;
            }

            return null;
        }

        /// <summary>
        /// Evento que irá preenxer os dados na grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptSelecaoPvs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger log = Logger.IniciarLog("rptSelecaoPvs_ItemDataBound"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        var entidade = (EntidadeServico.EntidadeServicoModel)e.Item.DataItem;

                        Label lblCodigo = (Label)e.Item.FindControl("lblCodigo");
                        Label lblDescricao = (Label)e.Item.FindControl("lblDescricao");
                        Label lblRazaoSocial = (Label)e.Item.FindControl("lblRazaoSocial");

                        lblRazaoSocial.Text = entidade.RazaoSocial;
                        lblCodigo.Text = Util.TruncarNumeroPV(entidade.NumeroPV.ToString());
                        lblDescricao.Text = Util.TruncarEmailUsuario(entidade.Email.Trim());
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                
                    return;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                
                    return;
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Obtem os dados para paginação
        /// </summary>
        /// <param name="IdPesquisa">Identificador da pesquisa</param>
        /// <param name="registroInicial">Registro inicial</param>
        /// <param name="quantidadeRegistros">Quantidade de registros</param>
        /// <param name="quantidadeRegistroBuffer">Quantidade registros no buffer</param>
        /// <param name="quantidadeTotalRegistrosEmCache">Quantidade total de registros</param>
        /// <param name="parametros">Parametros para consulta</param>
        /// <returns></returns>
        protected IEnumerable<Object> Paginacao_ObterDados(Guid IdPesquisa, int registroInicial, int quantidadeRegistros, int quantidadeRegistroBuffer, out int quantidadeTotalRegistrosEmCache, params object[] parametros)
        {
            int pagina;

            if (registroInicial == 0)
            {
                pagina = 1;
            }
            else
            {
                pagina = (registroInicial + quantidadeRegistros) / quantidadeRegistros;
            }

            return GetPVsComPaginacao(pagina, quantidadeRegistros, out quantidadeTotalRegistrosEmCache);
        }

        /// <summary>
        /// Retorna o CPF preenxido na tela
        /// </summary>
        /// <returns></returns>
        public Int64 GetCPF()
        {
            long retorno = _infoUsuario.CpfUsuario;

            if (retorno == 0)
            {
                return 0;
            }
            else
            {
                return retorno;
            }

        }


        /// <summary>
        /// Informa se a lista de Pvs estão visiveis
        /// </summary>
        /// <returns></returns>
        private bool IsVisibleListaPvs()
        {
            return pnlUsuarios.Visible;
        }

        /// <summary>
        /// Altera a visibilidade da lista de Pvs de acordo com parâmetro
        /// </summary>
        /// <param name="visible">Informa se ficará visivel a lista</param>
        private void VisibleListaPvs(bool visible)
        {
            pnlUsuarios.Visible = visible;
        }

        /// <summary>
        /// Verifica se o Pv está selecionado
        /// </summary>
        /// <param name="numPdv"></param>
        /// <returns></returns>
        public Boolean VerificarPvSelecionado(int numPdv)
        {
            if (InformacaoUsuario.Existe())
            {
                return _infoUsuario.PvsSelecionados.Contains(numPdv);
            }
            return false;
        }

        /// <summary>
        /// Retorna os Pvs selecionados
        /// </summary>
        /// <returns></returns>
        public string GetPvsSelecionados()
        {
            if (InformacaoUsuario.Existe())
            {
                return _infoUsuario.GetPvsSelecionados();
            }

            return null;
        }

        /// <summary>
        /// Evento de filtro de Pvs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBusca_Click(object sender, EventArgs e)
        {
            ChangeFlagPvsFiltrados();

            using (Logger log = Logger.IniciarLog("Busca pvs na tela"))
            {
                try
                {
                    paginacaoSelecaoPvs.Carregar();
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", this.RetornarHome());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", this.RetornarHome());
                }
            }
        }

        /// <summary>
        /// Retorna a URL da HomePage do site
        /// </summary>
        /// <returns></returns>
        private String RetornarHome()
        {
            String url = String.Empty;
            url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default);
            return url;
        }


        #endregion

    }
}
