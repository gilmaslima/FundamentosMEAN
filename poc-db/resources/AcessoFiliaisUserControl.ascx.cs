using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint;
using System.Web;
using System.ServiceModel;

using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Microsoft.SharePoint.Administration;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.Login;
using Microsoft.SharePoint.WebControls;
using ControlesDadosCadastrais = Redecard.PNCadastrais.Core.Web.Controles.Portal;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.AcessoFiliais
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AcessoFiliaisUserControl : UserControlBase
    {
        /// <summary>
        /// Codigo da Filial para acesso
        /// </summary>
        private String CodigoFilial
        {
            get
            {
                if (ViewState["CodigoFilial"] != null)
                    return ViewState["CodigoFilial"].ToString();
                else
                    return "";
            }
            set
            {
                ViewState["CodigoFilial"] = value;
            }
        }

        /// <summary>
        /// Inicialização da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Sessao.Contem() || SPContext.Current.FormContext.FormMode == SPControlMode.Edit)
            {
                return;
            }
            if (!Page.IsPostBack)
            {
                if (base.VerificarConfirmacaoPositia())
                {
                    RedirecionarConfirmacaoPositiva();
                    return;
                }
                else
                {
                    CarregarPagina();
            }
        }
            else
            {
                if (String.Compare(Request.Params.Get("__EVENTTARGET"), "AcessarFilial", true) == 0)
                {
                    AcessarFilial(sender, e);
                }
            }
        }

        /// <summary>
        /// Carregar a página com as filiais
        /// </summary>
        private void CarregarPagina()
        {
            using (Logger Log = Logger.IniciarLog("Carregando página Acesso Filiais"))
            {
                try
                {
                    // verificar se está entrando como uma filial.
                    if (!this.VerificarAcessoFilial())
                    {
                        // carregar lista de filiais do estabelecimento
                        this.CarregarFiliais();
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Verifica o acesso à filial
        /// </summary>
        private Boolean VerificarAcessoFilial()
        {
            if (this.SessaoAtual.AcessoFilial)
            {
                pnlFiliais.Visible = false;
                pnlTituloAcessoFilial.Visible = false;
                pnlNaoPossuiFiliais.Visible = false;

                this.RetornarMatriz(this, null);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Retornar aos dados da matriz caso a tela tenha sido aberta como uma filial
        /// </summary>
        protected void RetornarMatriz(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Retorno de Matriz"))
            {
                try
                {
                    ControlesDadosCadastrais.QuadroAviso quadro = (ControlesDadosCadastrais.QuadroAviso)qdConfirmacaoMatriz;

                    quadro.Mensagem = String.Format("Você deseja retornar para o PV matriz {0} - {1}?", SessaoAtual.CodigoEntidadeMatriz.ToString(), SessaoAtual.NomeEntidadeMatriz.Trim().ToUpper());

                    pnlAcesso.Visible = false;
                    pnlConfirmacaoMatriz.Visible = true;
                    pnlTituloConfirmacaoAcesso.Visible = true;
                    pnlTituloAcessoFilial.Visible = false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Muda o estado da sessão para o número do PV da filial
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AcessarFilial(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Mudando o estado da sessão para o número do PV da filial"))
            {
                try
                {
                    ControlesDadosCadastrais.QuadroAviso quadro = (ControlesDadosCadastrais.QuadroAviso)qdConfirmacao;

                    String codigoFilial = hdfFilial.Value;
                    String nomeFilial = hdfNomeFilial.Value;

                    quadro.Mensagem = String.Format("Você deseja acessar a filial {0} - {1}?", codigoFilial, nomeFilial.ToUpper());

                    //Guarda no ViewState qual o código da filial
                    this.CodigoFilial = codigoFilial;

                    pnlAcesso.Visible = false;
                    pnlConfirmacao.Visible = true;
                    pnlTituloConfirmacaoAcesso.Visible = true;
                    pnlTituloAcessoFilial.Visible = false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CarregarAcessoFilial(object sender, EventArgs args)
        {
            using (Logger Log = Logger.IniciarLog("Acesso Filial"))
            {
                String _redirectUrl = String.Empty;

                if (!this.SessaoAtual.AcessoFilial)
                {

                    // recuperar número da filial
                    String _nomeEntidadeFilial = string.Empty;
                    String _statusPV = string.Empty;
                    String _cnpjEntidade = string.Empty;
                    Int32 _tecnologiaFilial = 0;
                    String _tecnologiaDataCash = String.Empty;
                    DateTime _tecnologiaDataCashDataAtivacao = DateTime.MinValue;
                    String _ufEntidade = string.Empty;
                    //Pega da Sessão qual o código da filial 
                    String numeroFilial = this.CodigoFilial;
                    Int32 _numeroFilial = Int32.Parse(numeroFilial);
                    Char codigoSegmentoFilial = Char.MinValue;
                    Int32 codigoRamoAtividade = 0;
                    Int32 codigoGrupoRamo = 0;
                    Int32 codigoCanal = 0;
                    Int32 codigoCelula = 0;
                    String recarga = String.Empty;
                    Int32 codigoMatriz = 0;
                    Boolean servicoEadquirencia = false;

                    // Quando ocorre o acesso a uma filial, os dados do usuário serão substituidos pelos
                    // números da Matriz original
                    String _nomeUsuario = this.SessaoAtual.NomeEntidade;
                    String _loginUsuario = this.SessaoAtual.CodigoEntidade.ToString();
                    String _emailEntidade = this.SessaoAtual.EmailEntidade;

                    try
                    {
                        Int32 codigoRetorno;

                        using (EntidadeServico.EntidadeServicoClient _client = new EntidadeServico.EntidadeServicoClient())
                        {
                            EntidadeServico.Entidade entidade = _client.ConsultarDadosPV(out codigoRetorno, _numeroFilial);
                            
                            if (codigoRetorno > 0)
                                base.ExibirPainelExcecao("EntidadeServico.Consultar", codigoRetorno);
                            else
                            {
                                // deve retornar somente uma unidade, uma vez que não existem estabelecimentos de mesmo
                                // número
                                _nomeEntidadeFilial = entidade.NomeEntidade;
                                _statusPV = entidade.Status;
                                _cnpjEntidade = entidade.CNPJEntidade;
                                _ufEntidade = entidade.UF;
                                _tecnologiaFilial = _client.ConsultarTecnologiaEstabelecimento(out codigoRetorno, _numeroFilial);
                                _tecnologiaDataCash = entidade.IndicadorDataCash;
                                _tecnologiaDataCashDataAtivacao = entidade.DataAtivacaoDataCash;
                                codigoSegmentoFilial = entidade.CodigoSegmento;
                                codigoGrupoRamo = entidade.CodigoGrupoRamo;
                                codigoRamoAtividade = entidade.CodigoRamoAtividade;
                                codigoCanal = entidade.CodigoCanal;
                                codigoCelula = entidade.CodigoCelula;
                                recarga = entidade.Recarga;
                                codigoMatriz = entidade.CodigoMatriz;
                                servicoEadquirencia = entidade.ServicoEadquirencia;
                                
                                if (codigoRetorno == 0)
                                {
                                    // impersonar filial
                                    this.SessaoAtual.AcessarFilial(
                                        _nomeEntidadeFilial,
                                        _numeroFilial,
                                        _tecnologiaFilial,
                                        _cnpjEntidade,
                                        _nomeUsuario,
                                        _loginUsuario,
                                        _statusPV,
                                        _emailEntidade,
                                        _ufEntidade,
                                        _tecnologiaDataCash,
                                        _tecnologiaDataCashDataAtivacao,
                                        codigoGrupoRamo,
                                        codigoRamoAtividade,
                                        codigoSegmentoFilial,
                                        codigoCanal,
                                        codigoCelula,
                                        recarga,
                                        codigoMatriz,
                                        servicoEadquirencia);

                                    //Remonta o menu e permissões
                                    this.RemontarMenu();

                                    // Redireciona para a home do portal com a flag que o usuário esta logado como filial.
                                    // Com isso será exibido o LightBox de aviso.
                                    //_redirectUrl = SPContext.Current.Site.Url + "/" + SPContext.Current.Site.RootWeb.RootFolder.WelcomePage + "?LogFilial=GyOPKvNNuown4p4rVp3RGPWh0%2bJF32lIkUhWXZHu9xhnrDV9ZQAnthBLypL%2bi6D3";
                                    //_redirectUrl = SPContext.Current.Site.Url + "?LogFilial=GyOPKvNNuown4p4rVp3RGPWh0%2bJF32lIkUhWXZHu9xhnrDV9ZQAnthBLypL%2bi6D3";
                                    Int32 codigoRetornoUsuario = 0;
                                    UsuarioServico.Usuario usuario = null;

                                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                                        usuario = ctx.Cliente.ConsultarDadosUsuario(
                                            out codigoRetornoUsuario,
                                            this.SessaoAtual.CodigoEntidade,
                                            this.SessaoAtual.CodigoIdUsuario,
                                            this.SessaoAtual.NomeEntidade);
                                    
                                    Historico.Login(this.SessaoAtual, usuario != null ? usuario.DataInclusao : (DateTime?)null);

                                    _redirectUrl = Util.BuscarUrlRedirecionamento("/sites/fechado", SPUrlZone.Internet);
                                    _redirectUrl = String.Concat(_redirectUrl, "/HomeSpa/index.html");
                                }
                                else
                                    base.ExibirPainelExcecao("EntidadeServico.ConsultarTecnologiaEstabelecimento", codigoRetorno);
                            }
                        }
                    }
                    catch (FaultException<EntidadeServico.GeneralFault> ex)
                    {
                        Log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                    //CarregarPaginacao();
                }

                // redirecionar caso tudo tenha dado certo
                if (!String.IsNullOrEmpty(_redirectUrl))
                {
                    // redirecionar home page
                    SPUtility.Redirect(_redirectUrl, SPRedirectFlags.DoNotEndResponse, HttpContext.Current);
                }
            }
        }

        /// <summary>
        /// Carrega a Lista de Filiais Associadas
        /// </summary>
        private void CarregarFiliais()
        {
            using (Logger Log = Logger.IniciarLog("Carregando filiais"))
            {
                Int32 codigoRetorno = 0;

                // Chama o serviço que carrega as filiais para o Estabelecimento atual
                using (EntidadeServico.EntidadeServicoClient _client = new EntidadeServico.EntidadeServicoClient())
                {
                    EntidadeServico.Filial[] filiais = _client.ConsultarFiliais(out codigoRetorno, SessaoAtual.CodigoEntidade, 2);

                    if (codigoRetorno > 0)
                    {
                        base.ExibirPainelExcecao("EntidadeServico.Consultar", codigoRetorno);
                    }
                    else
                    {
                        if (filiais.Length > 0)
                            this.ExecutarBindFiliais(filiais);
                        else
                            this.ExibirAvisoNaoPossuiFiliais();
                    }
                }
            }
        }

        /// <summary>
        /// Exibe o aviso de "Este estabelecimento não possui filiais"
        /// </summary>
        private void ExibirAvisoNaoPossuiFiliais()
        {
            try
            {
                pnlNaoPossuiFiliais.Visible = true;
                ControlesDadosCadastrais.QuadroAviso quadro = (ControlesDadosCadastrais.QuadroAviso)qdAviso;
                quadro.Mensagem = "Este estabelecimento não possui filiais.";

                pnlFiliais.Visible = false;
                pnlTituloAcessoFilial.Visible = false;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro na exibição de aviso e não possui filiais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Executa o data bind do controle de listagem das filiais
        /// </summary>
        /// <param name="filiais"></param>
        private void ExecutarBindFiliais(EntidadeServico.Filial[] filiais)
        {
            try
            {
                rptFiliais.DataSource = filiais;
                rptFiliais.DataBind();
                //Adiciona a função de JS de paginação do grid para as funções a serem executadas após renderização
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Paginacao", "pageResultTable('tblFiliais', 1, 10, 5);", true);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro no Bind das Filiais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }


        /// <summary>
        /// Carrega a função para paginação da tabela de filiais
        /// </summary>
        protected void CarregarPaginacao()
        {
            try
            {
                //Adiciona a função de JS de paginação do grid para as funções a serem executadas após renderização
                //pageResultTable(idTabela, idDivPai, pageIndex, pageSize, pagesPerView)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Paginacao", "pageResultTable('tblFiliais', 1, 10, 5);", true);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro carregando paginação", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Continuar Ação Filial
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void Continuar(object sender, EventArgs args)
        {
            using (Logger Log = Logger.IniciarLog("Confirmação de Troca de Filial"))
            {
                try
                {
                    CarregarAcessoFilial(sender, args);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Continuar Voltar Matriz
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void ContinuarMatriz(object sender, EventArgs args)
        {
            using (Logger Log = Logger.IniciarLog("Confirmação de Retorno para Matriz"))
            {
                try
                {
                    if (this.SessaoAtual.AcessoFilial)
                    {
                        this.SessaoAtual.RetornarMatriz();

                        //Remonta o menu e permissões
                        RemontarMenu();

                        // redirecionar para página inicial do Portal de Serviços conforme mensagem
                        // exibida na tela
                        SPUtility.Redirect(SPContext.Current.Site.Url, SPRedirectFlags.DoNotEndResponse, HttpContext.Current);
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Voltar Ação de Acesso Filial
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void Voltar(object sender, EventArgs args)
        {
            using (Logger Log = Logger.IniciarLog("Voltar"))
            {
                try
                {
                    Response.Redirect(SPUtility.GetPageUrlPath(HttpContext.Current), false);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Voltar Ação de Acesso Filial
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void VoltarMatriz(object sender, EventArgs args)
        {
            String url = String.Empty;
            using (Logger Log = Logger.IniciarLog("Click do Voltar ao estar logado como matriz"))
            {
                try
                {
                    url = Util.BuscarUrlRedirecionamento("/sites/fechado/HomeSpa/index.html", SPUrlZone.Default);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            if (!String.IsNullOrWhiteSpace(url))
                Response.Redirect(url);
        }
    
        /// <summary>
        /// Remonta o menu e permissões
        /// </summary>
        private void RemontarMenu()
        {
            using (Logger log = Logger.IniciarLog("Remontagem de Menu"))
            {
                try
                {
                    var usuario = default(UsuarioServico.Usuario);

                    //Código da entidade.
                    //No caso de Central de Atendimento, o usuário "atendimento" é sempre da entidade 1
                    Int32 codigoEntidadeUsuario = SessaoAtual.CodigoEntidadeMatriz;
                    if (SessaoAtual.UsuarioAtendimento)
                        codigoEntidadeUsuario = 1;

                    var codigoRetornoUsuario = default(Int32);
                    Int32 codigoGrupoEntidade = SessaoAtual.GrupoEntidade;
                    String codigoNomeUsuario = SessaoAtual.LoginUsuarioMatriz;

                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                    {
                        usuario = ctx.Cliente.ConsultarDadosUsuario(
                            out codigoRetornoUsuario,
                            codigoGrupoEntidade,
                            codigoEntidadeUsuario,
                            codigoNomeUsuario);
                    }

                    if (usuario != null)
                    {
                        this.SessaoAtual.Servicos.Clear();
                        this.SessaoAtual.Menu.Clear();
                        this.SessaoAtual.Paginas.Clear();

                        LoginClass.CadastrarServicos(this.SessaoAtual, usuario.Menu);
                        LoginClass.CadastrarMenu(this.SessaoAtual, usuario.Menu);
                        LoginClass.CadastrarPermissoes(this.SessaoAtual, usuario.Paginas);
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
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
        /// Evento para popular o grid de estabelecimentos.
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém informações do evento</param>
        protected void rptFiliais_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = (EntidadeServico.Filial)e.Item.DataItem;

                var lblNumeroFilial = (Label)e.Item.FindControl("lblNumeroFilial");
                var lblFilial = (Label)e.Item.FindControl("lblFilial");
                var lnkSelecionar = (LinkButton)e.Item.FindControl("lnkSelecionar");


                lblNumeroFilial.Text = item.PontoVenda.ToString().Trim();
                lblFilial.Text = item.NomeComerc.Trim();
                lblFilial.ToolTip = item.NomeComerc.Trim();
                lnkSelecionar.Attributes.Add("CodigoFilial", item.PontoVenda.ToString().Trim());
                lnkSelecionar.Attributes.Add("NomeFilial", item.NomeComerc.Trim());
                lnkSelecionar.Attributes.Add("href", "javascript:void(0);");
            }
        }

    }
}
