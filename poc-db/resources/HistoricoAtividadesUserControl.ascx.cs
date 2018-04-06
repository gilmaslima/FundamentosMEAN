/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.HistoricoAtividadeServico;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.UI.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.HistoricoAtividades
{
    /// <summary>
    /// Administração de Usuários - Histórico de Atividades / Detalhamento de Atividade
    /// </summary>
    public partial class HistoricoAtividadesUserControl : UserControlBase
    {

        #region [ Propriedades ]

        /// <summary>
        /// View que estará habilitada na página (0 - listagem, ou 1 - detalhamento de atividade) - Fica configurado no elements.xml
        /// </summary>
        public Int32 ActiveViewIndex { get; set; }

        #endregion

        #region [ Variáveis ViewState ]

        /// <summary>
        /// Tipos de Atividades existentes
        /// </summary>
        private Dictionary<Int32, TipoAtividade> TiposAtividade
        {
            get
            {
                if (ViewState["TiposAtividade"] == null)
                    ViewState["TiposAtividade"] = ConsultarTiposAtividade()
                        .ToDictionary(atividade => atividade.Codigo, atividade => atividade);
                return (Dictionary<Int32, TipoAtividade>)ViewState["TiposAtividade"];
            }
            set
            {
                ViewState["TiposAtividade"] = value;
            }
        }

        /// <summary>
        /// Ordenacao da tabela de usuarios coluna
        /// </summary>
        private String OrdenacaoColuna
        {
            get
            {
                if (ViewState["OrdenacaoColuna"] == null)
                    ViewState["OrdenacaoColuna"] = String.Empty;
                return (String)ViewState["OrdenacaoColuna"];
            }
            set { ViewState["OrdenacaoColuna"] = value; }
        }

        /// <summary>
        /// Ordenacao da tabela de usuarios direcao
        /// </summary>
        private String OrdenacaoDirecao
        {
            get
            {
                if (ViewState["OrdenacaoDirecao"] == null)
                    ViewState["OrdenacaoDirecao"] = String.Empty;
                return (String)ViewState["OrdenacaoDirecao"];
            }
            set { ViewState["OrdenacaoDirecao"] = value; }
        }

        /// <summary>
        /// Lista das Atividades
        /// </summary>
        private List<HistoricoAtividade> HistoricoAtividades
        {
            get
            {
                if (ViewState["HistoricoAtividades"] == null)
                    ViewState["HistoricoAtividades"] = new List<HistoricoAtividade>();
                return (List<HistoricoAtividade>)ViewState["HistoricoAtividades"];
            }
            set { ViewState["HistoricoAtividades"] = value; }
        }
        #endregion

        #region [ Eventos da Página ]

        /// <summary>
        /// Verificação inicial
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //Se for usuário não migrado, força redirecionamento para tela de Migração
            if (Sessao.Contem() && this.SessaoAtual.Legado)
            {
                //Nome da página de cadastro da migração do usuário
                String paginaCadastroUsuarioMigracao = "CadastroUsuarioMigracao.aspx";

                //Redireciona para página de cadastro de migração do usuário
                Response.Redirect(paginaCadastroUsuarioMigracao);
            }
        }

        /// <summary>
        /// Page_Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Historico de atividades - Page_Load"))
            {
                try
                {
                    //Verifica permissão (apenas Master e Central de Atendimento podem acessar)
                    Boolean usuarioMaster = SessaoAtual != null && SessaoAtual.UsuarioMaster();
                    Boolean usuarioAtendimento = SessaoAtual != null && SessaoAtual.UsuarioAtendimento;
                    if (!usuarioMaster && !usuarioAtendimento)
                    {
                        mvwHistoricoAtividades.SetActiveView(pnlAcessoNegado);
                        return;
                    }

                    //Botão default da Página
                    Page.Form.DefaultButton = btnBuscar.UniqueID;

                    if (!IsPostBack)
                    {
                        //Histórico de Atividades
                        if (this.ActiveViewIndex == 0)
                        {
                            log.GravarMensagem("Listagem");
                            mvwHistoricoAtividades.SetActiveView(pnlHistoricoAtividades);
                            CarregarTiposAtividades();

                            txtDataInicio.Text = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
                            txtDataFim.Text = DateTime.Now.ToString("dd/MM/yyyy");
                        }
                        //Detalhamento de Atividade
                        else
                        {
                            log.GravarMensagem("Detalhamento");
                            mvwHistoricoAtividades.SetActiveView(pnlDetalhesAtividade);
                            CarregarDetalhesAtividade();
                        }
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #endregion

        #region [ View - Histórico de Atividades - Listagem ]

        #region [ Eventos dos Controles ]

        /// <summary>
        /// btnDownload_Click
        /// </summary>
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Historico de atividades - btnDownload_Click"))
            {
                try
                {
                    String nomeArquivo = String.Format("Administracao de Usuarios - Historico de Atividades - {0}.xls",
                        DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
                    String conteudoCsv = GerarConteudoExportacao();

                    Response.Clear();
                    Response.ClearHeaders();
                    Response.Buffer = true;
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + nomeArquivo);
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding(1252);
                    Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                    Response.ContentType = "application/ms-excel";
                    Response.Write(conteudoCsv);
                    Response.Flush();
                    Response.End();
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// btnBuscar_Click
        /// </summary>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Historico de atividades - btnBuscar_Click"))
            {
                try
                {
                    divAtividades.Visible = true;

                    this.OrdenacaoColuna = String.Empty;
                    this.OrdenacaoDirecao = String.Empty;

                    this.HistoricoAtividades.Clear();

                    this.CarregarAtividades();
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// btnDetalhar_Click
        /// </summary>
        protected void btnDetalhar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Historico de atividades - btnDetalhar_Click"))
            {
                try
                {
                    var btnDetalhes = (LinkButton)sender;

                    String codigoIdHistorico = btnDetalhes.CommandArgument;
                    var qs = new QueryStringSegura();
                    qs["CodigoIdHistorico"] = codigoIdHistorico;

                    String url = String.Format("HistoricoAtividadesDetalhes.aspx?dados={0}", qs.ToString());
                    Response.Redirect(url, false);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// grvAtividades_PageIndexChanging
        /// </summary>
        protected void grvAtividades_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Historico de atividades - grvAtividades_PageIndexChanging"))
            {
                try
                {
                    grvAtividades.PageIndex = e.NewPageIndex;
                    CarregarAtividades();
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// grvAtividades_RowDataBound
        /// </summary>
        protected void grvAtividades_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var atividade = (HistoricoAtividade)e.Row.DataItem;

                var lnkBtnDetalhar = (LinkButton)e.Row.FindControl("lnkBtnDetalhar");

                lnkBtnDetalhar.CommandArgument = atividade.Codigo.ToString();
            }
        }

        protected void grvAtividades_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Historico de atividades - grvAtividades_RowCommand"))
            {
                try
                {
                    if (e.CommandSource.GetType() == typeof(ImageButton))
                    {
                        ImageButton acionador = (ImageButton)e.CommandSource;
                        if (String.Compare(acionador.ID, "imbUsuarioOrdenacao", true) == 0 ||
                            String.Compare(acionador.ID, "imbEmailOrdenacao", true) == 0 ||
                            String.Compare(acionador.ID, "imbAtividadeOrdenacao", true) == 0 ||
                            String.Compare(acionador.ID, "imbDataOrdenacao", true) == 0 ||
                            String.Compare(acionador.ID, "imbPerfilOrdenacao", true) == 0
                            )
                        {
                            if (String.Compare(e.CommandName, this.OrdenacaoColuna, true) == 0)
                            {
                                if (String.Compare(this.OrdenacaoDirecao, "ASC", true) == 0)
                                    this.OrdenacaoDirecao = "DESC";
                                else
                                    this.OrdenacaoDirecao = "ASC";
                            }
                            else
                            {
                                this.OrdenacaoColuna = e.CommandName;
                                this.OrdenacaoDirecao = "ASC";
                            }

                            this.CarregarAtividades();
                        }
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #endregion

        #region [ Métodos Privados ]

        /// <summary>
        /// Carrega os tipos de atividades na combo
        /// </summary>
        private void CarregarTiposAtividades()
        {
            ddlTipoAtividade.DataSource = this.TiposAtividade.Select(tipo => tipo.Value);
            ddlTipoAtividade.DataBind();
        }

        /// <summary>
        /// Carrega as atividades de acordo com o filtro e página
        /// </summary>
        private void CarregarAtividades()
        {
            if (this.HistoricoAtividades == null || this.HistoricoAtividades.Count == 0)
            {
                //Recupera dados informados nos filtros
                Int32? tipoAtividade = ddlTipoAtividade.SelectedValue.ToInt32();
                if (tipoAtividade == 0) tipoAtividade = null;
                String nomeUsuario = txtNomeUsuario.Text;
                DateTime? dataInicio = txtDataInicio.Text.ToDateTimeNull("dd/MM/yyyy");
                DateTime? dataFim = txtDataFim.Text.ToDateTimeNull("dd/MM/yyyy");

                //Consulta as atividades
                this.HistoricoAtividades = ConsultarAtividades(null, SessaoAtual.CodigoEntidade, tipoAtividade, nomeUsuario, dataInicio, dataFim);
            }

            this.HistoricoAtividades = OrdernarAtividades(this.HistoricoAtividades);

            grvAtividades.DataSource = this.HistoricoAtividades;
            grvAtividades.DataBind();

            //Atualiza mensagem de quantidade de atividades encontradas/exibidas
            Int32 totalRegistros = this.HistoricoAtividades.Count;
            Int32 registroInicial = (grvAtividades.PageIndex) * grvAtividades.PageSize + Math.Min(1, totalRegistros);
            Int32 registroFinal = (1 + grvAtividades.PageIndex) * grvAtividades.PageSize;
            if (totalRegistros < registroFinal)
                registroFinal = totalRegistros;

            if (totalRegistros > 0)
            {
                ltrAtividadesEncontradas.Visible = true;
                ltrAtividadesEncontradas.Text = String.Format("Exibindo atividades {0} a {1} de {2} encontrada{3}",
                    registroInicial, registroFinal, totalRegistros, totalRegistros > 1 ? "s" : String.Empty);

                grvAtividades.UseAccessibleHeader = true;
                grvAtividades.HeaderRow.TableSection = TableRowSection.TableHeader;
                grvAtividades.FooterRow.TableSection = TableRowSection.TableFooter;

                if (grvAtividades.TopPagerRow != null)
                {
                    grvAtividades.TopPagerRow.TableSection = TableRowSection.TableHeader;
                }
                if (grvAtividades.BottomPagerRow != null)
                {
                    grvAtividades.BottomPagerRow.TableSection = TableRowSection.TableFooter;
                }
                mnuAcoes.Visible = true;
                divRegistroNaoEncontrado.Visible = false;
            }
            else
            {
                ltrAtividadesEncontradas.Visible = false;
                mnuAcoes.Visible = false;
                divRegistroNaoEncontrado.Visible = true;
            }
        }

        /// <summary>
        /// Gera conteúdo CSV para Exportação/Download
        /// </summary>
        public String GerarConteudoExportacao()
        {
            //Lista de colunas do CSV
            List<String> colunas = new[] { "Nome do Usuário", "Email", "Atividade", "Data", "Perfil" }.ToList();

            //Função para conversão de um registro de Atividade em List<String>
            //Deve aplicar o mesmo padrão de formatação utilizado na montagem da grid
            Func<HistoricoAtividade, List<String>> dadosLinha = (atividade) =>
            {
                return new[] {
                    atividade.NomeUsuario,
                    atividade.EmailUsuario,
                    atividade.Detalhes,
                    atividade.Timestamp.ToString("dd/MM/yyyy HH:mm"),
                    atividade.PerfilUsuario
                }.ToList();
            };

            //Gera CSV, com TAB como delimitador
            return CSVExporter.GerarCSV(this.HistoricoAtividades, colunas, dadosLinha, "\t");
        }

        /// <summary>
        /// Obtém o título do tipo da atividade, dado seu código
        /// </summary>
        private String ObterTituloTipoAtividade(Int32 codigoTipoAtividade)
        {
            if (this.TiposAtividade.ContainsKey(codigoTipoAtividade))
                return this.TiposAtividade[codigoTipoAtividade].Titulo;
            else
                return codigoTipoAtividade.ToString();
        }

        /// <summary>
        /// Obtém a descrição do tipo da atividade, dado seu código
        /// </summary>
        private String ObterDescricaoTipoAtividade(Int32 codigoTipoAtividade)
        {
            if (this.TiposAtividade.ContainsKey(codigoTipoAtividade))
                return this.TiposAtividade[codigoTipoAtividade].Descricao;
            else
                return codigoTipoAtividade.ToString();
        }

        private List<HistoricoAtividade> OrdernarAtividades(List<HistoricoAtividade> atividades)
        {
            List<HistoricoAtividade> retorno = null;
            if (!String.IsNullOrWhiteSpace(this.OrdenacaoColuna))
            {
                if (String.Compare(this.OrdenacaoDirecao, "ASC", true) == 0)
                    retorno = atividades.OrderBy(this.OrdenacaoColuna).ToList();
                else
                    retorno = atividades.OrderByDescending(this.OrdenacaoColuna).ToList();
            }
            else
            {
                retorno = atividades;
            }

            return retorno;
        }

        #endregion

        #region [ Consultas ]

        /// <summary>
        /// Consulta as atividades no PV, dado os parâmetros de filtro
        /// </summary>
        /// <param name="codigoHistorico">Código ID do histórico da atividade</param>
        /// <param name="codigoEntidade">Código do Estabelecimento</param>
        /// <param name="tipoAtividade">Tipo da atividade</param>
        /// <param name="nomeUsuario">Nome do usuário que realizou a atividade</param>
        /// <param name="dataInicio">Data de início</param>
        /// <param name="dataFim">Date de término</param>
        /// <returns>Lista de atividades encontradas</returns>
        private List<HistoricoAtividade> ConsultarAtividades(
            Int64? codigoHistorico, Int32? codigoEntidade, Int32? tipoAtividade, String nomeUsuario, DateTime? dataInicio, DateTime? dataFim)
        {
            var historico = default(HistoricoAtividade[]);

            using (var ctx = new ContextoWCF<HistoricoAtividadeServicoClient>())
                historico = ctx.Cliente.ConsultarHistorico(codigoHistorico, codigoEntidade,
                    tipoAtividade, dataInicio, dataFim, nomeUsuario, null, null, true);

            if (historico == null)
                return new List<HistoricoAtividade>();

            var atividades = historico.ToList();

            atividades.ForEach(atv =>
            {
                atv.EmailUsuario = atv.EmailUsuario.ToLower();
                atv.PerfilUsuario = atv.PerfilUsuario.ToLower();
                atv.Detalhes = ObterTituloTipoAtividade(atv.CodigoTipoAtividade);
                atv.Descricao = atv.Descricao.ToLower();
            });

            return atividades;
        }

        /// <summary>
        /// Consulta os tipos de atividades existentes
        /// </summary>
        /// <returns>Tipos de Atividades</returns>
        private List<TipoAtividade> ConsultarTiposAtividade()
        {
            var tiposAtividade = default(TipoAtividade[]);

            using (var ctx = new ContextoWCF<HistoricoAtividadeServicoClient>())
                tiposAtividade = ctx.Cliente.ConsultarTiposAtividades(true);

            if (tiposAtividade == null)
                return new List<TipoAtividade>();

            List<TipoAtividade> tipoAtividades = tiposAtividade.ToList();

            tipoAtividades.ForEach(tp =>
            {
                tp.Descricao = tp.Descricao.ToLower();
                tp.Titulo = tp.Titulo.ToLower();
            });

            return tipoAtividades;
        }

        /// <summary>
        /// Consulta um usuário por ID
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        private Usuario ConsultarUsuario(Int32 codigoIdUsuario)
        {
            var usuario = default(Usuario);

            using (Logger log = Logger.IniciarLog("Consultando usuário por ID"))
            {
                var codigoRetorno = default(Int32);

                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                        usuario = ctx.Cliente.ConsultarPorID(out codigoRetorno, codigoIdUsuario);
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }
            }

            return usuario;
        }

        #endregion

        #endregion

        #region [ View - Detalhes da Atividade ]

        /// <summary>
        /// Carrega os detalhes da atividade
        /// </summary>
        private void CarregarDetalhesAtividade()
        {
            String dados = Request.QueryString["dados"];
            if (!String.IsNullOrEmpty(dados))
            {
                var qs = new QueryStringSegura(dados);
                Int64 codigoIdHistorico = qs["CodigoIdHistorico"].ToInt64();

                HistoricoAtividade historico = ConsultarAtividades(codigoIdHistorico, null, null, null, null, null).FirstOrDefault();
                if (historico != null)
                {
                    ltrTipoAtividade.Text = ObterTituloTipoAtividade(historico.CodigoTipoAtividade);
                    ltrResponsavel.Text = historico.NomeUsuario;
                    ltrData.Text = historico.Timestamp.ToString("dd/MM/yyyy HH'h'mm");
                    ltrPerfil.Text = historico.PerfilUsuario;
                    ltrDescricao.Text = historico.Descricao;

                    if (historico.CodigoIdUsuario.HasValue)
                    {
                        Usuario usuario = ConsultarUsuario(historico.CodigoIdUsuario.Value);
                        if (usuario != null && usuario.Status != null && usuario.Status.Codigo.HasValue)
                        {
                            var status = (Comum.Enumerador.Status)usuario.Status.Codigo.Value;
                            Boolean usuarioProprio = usuario.CodigoIdUsuario == SessaoAtual.CodigoIdUsuario;
                            Boolean usuarioEditavel = !usuarioProprio && //Próprio
                                (status == Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoAlteracaoEmail ||
                                status == Comum.Enumerador.Status.UsuarioAtivo ||
                                status == Comum.Enumerador.Status.UsuarioAtivoAguardandoConfirmacaoRecuperacaoSenha ||
                                status == Comum.Enumerador.Status.UsuarioAtivoLiberacaoAcessoCompletoBloqueada ||
                                status == Comum.Enumerador.Status.RespostaIdPosPendente);
                            pnlEdicaoResponsavel.Visible = usuarioEditavel;
                            btnEditarPermissoes.CommandArgument = usuario.CodigoIdUsuario.ToString();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// btnEditarPermissoes_Click
        /// </summary>
        protected void btnEditarPermissoes_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Detalhamento Historico de atividades - btnEditarPermissoes_Click"))
            {
                try
                {
                    LinkButton btnEditarPermissoes = (LinkButton)sender;
                    String codigoIdUsuario = btnEditarPermissoes.CommandArgument;

                    var qs = new QueryStringSegura();
                    qs["CodigoIdUsuario"] = codigoIdUsuario;
                    qs["Acao"] = "AlterarPermissoes";

                    String url = String.Format("Usuarios.aspx?dados={0}", qs.ToString());

                    Response.Redirect(url, false);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #endregion
    }
}