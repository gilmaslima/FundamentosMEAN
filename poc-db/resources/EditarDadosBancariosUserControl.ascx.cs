using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.SharePoint;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Core = Redecard.PNCadastrais.Core.Web.Controles.Portal;
using webControls = System.Web.UI.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.EditarDadosBancarios
{
    /// <summary>
    /// Classe para página de Alteração de Dados de Domicílio Bancário
    /// </summary>
    public partial class EditarDadosBancariosUserControl : UserControlBase
    {
        /// <summary>
        /// Indica se o PV possui trava de domicílio
        /// </summary>
        private Boolean PermiteAlterar
        {
            get
            {
                if (ViewState["PermiteAlterar"] != null)
                    return (Boolean)ViewState["PermiteAlterar"];
                else
                    return false;
            }
            set
            {
                ViewState["PermiteAlterar"] = value;
            }
        }

        /// <summary>
        /// Lista de Todos os Bancos
        /// </summary>
        private List<EntidadeServico.Banco> ListaBancos
        {
            get
            {
                if (ViewState["ListaBancos"] != null)
                    return (List<EntidadeServico.Banco>)ViewState["ListaBancos"];
                else
                    return new List<EntidadeServico.Banco>();
            }

            set
            {
                ViewState["ListaBancos"] = value;
            }
        }

        /// <summary>
        /// Lista de Bancos com Confirmação Eletrônica
        /// </summary>
        private List<EntidadeServico.Banco> ListaBancosConfirmacaoEletronica
        {
            get
            {
                if (ViewState["ListaBancosConfirmacaoEletronica"] != null)
                    return (List<EntidadeServico.Banco>)ViewState["ListaBancosConfirmacaoEletronica"];
                else
                    return new List<EntidadeServico.Banco>();
            }

            set
            {
                ViewState["ListaBancosConfirmacaoEletronica"] = value;
            }
        }

        /// <summary>
        /// Dados de Domicílios Bancários da Entidade
        /// </summary>
        private EntidadeServico.DadosDomiciolioBancario[] DomiciliosBancarios
        {
            get
            {
                if (ViewState["DomicilioBancario"] != null)
                    return (EntidadeServico.DadosDomiciolioBancario[])ViewState["DomicilioBancario"];
                else
                    return null;
            }
            set
            {
                ViewState["DomicilioBancario"] = value;
            }
        }

        /// <summary>
        /// Indica se a página esta sendo editada ou nao
        /// </summary>
        protected Boolean ModoEdicao
        {
            get
            {
                if (Session["informacoescadastraismodoedicao"] != null)
                {
                    return Convert.ToBoolean(Session["informacoescadastraismodoedicao"]);
                }
                return false;
            }
            set
            {
                Session["informacoescadastraismodoedicao"] = value;
            }
        }

        /// <summary>
        /// Dados de Domicílios Bancários que foram alterados para gerar pdf
        /// </summary>
        private String DomiciliosBancariosAlterados
        {
            get
            {
                if (ViewState["DomiciliosBancariosAlterados"] != null)
                    return (String)ViewState["DomiciliosBancariosAlterados"];
                else
                    return null;
            }
            set
            {
                ViewState["DomiciliosBancariosAlterados"] = value;
            }
        }

        /// <summary>
        /// Inicialização da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Dados Bancários - Carregando página"))
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        if (base.VerificarConfirmacaoPositia())
                        {
                            RedirecionarConfirmacaoPositiva();
                            return;
                        }

                        HabilitarQuadroBancario(false);
                        if (CarregarDomiciliosBancarios() && !Object.ReferenceEquals(this.DomiciliosBancarios, null) && this.DomiciliosBancarios.Length > 0)
                        {
                            CarregarBancosConfirmacaoEletronica();
                            CarregarBancos();
                            ExibirUltimaSolicitacao();

                            ExibirFormularioDomicilios();
                        }
                        else
                        {
                            this.PermiteAlterar = false;
                            qdAvisoDadosBancarios.Visible = true;
                            qdAvisoDadosBancarios.TipoQuadro = Core.TipoQuadroAviso.Aviso;
                            qdAvisoDadosBancarios.Mensagem = "Nenhum domicílio bancário foi encontrado para a Entidade.";
                        }
                        // O usuario do tipo atendimento tem permissao apenas para visualizar a pagina
                        if (SessaoAtual != null && SessaoAtual.UsuarioAtendimento)
                        {
                            // no modo edicao nao aparece nenhum botao para edicao 
                            this.ModoEdicao = true;
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
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
        /// Pre renderizacao
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Dados Bancários - Carregando página pre render"))
            {
                try
                {
                    AtualizarEdicaoPagina();
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
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
        /// databound das bandeiras
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptBandeiraSolicitacao_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Bandeira bandeira = (Bandeira)e.Item.DataItem;
                Label lblBandeira = (Label)e.Item.FindControl("lblBandeira");
                lblBandeira.Text = DescricaoBandeira(bandeira.SiglaProduto);
            }
        }

        /// <summary>
        /// Clique para editar os dados bancarios
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkBtnEditarBancos_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Dados Bancários - Edição dos dados"))
            {
                try
                {
                    this.DomiciliosBancariosAlterados = String.Empty;
                    mnuAcoes.Visible = false;
                    HabilitarQuadroBancario(true);
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
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
        /// Cancelando a edicao
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDadosBancariosCancelarEdicao_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Dados Bancários - cancelando edição dos dados"))
            {
                try
                {
                    HabilitarQuadroBancario(false);
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
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
        /// Concluindo a edicao
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDadosBancariosConfirmar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Dados Bancários - Confirmando edição dos dados"))
            {
                try
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    List<BancoFront> bancosModificados = new List<BancoFront>();

                    if (!String.IsNullOrWhiteSpace(hdfDadosBancariosModificado.Value))
                    {
                        List<BancoFront> bancosOriginais = serializer.Deserialize<List<BancoFront>>(hdfDadosBancariosOriginal.Value);
                        List<BancoFront> bancosPost = serializer.Deserialize<List<BancoFront>>(hdfDadosBancariosModificado.Value);
                        if (bancosPost.Count > 0)
                        {
                            foreach (var banco in bancosPost)
                            {
                                var bancoOriginal = bancosOriginais.FirstOrDefault(b =>
                                                        b.CodigoBanco == banco.CodigoBanco &&
                                                        String.Compare(b.CodigoAgencia, banco.CodigoAgencia, true) == 0 &&
                                                        String.Compare(b.CodigoAgencia, banco.CodigoAgencia, true) == 0);

                                // verifica se o banco existe na lista dos bancos originais
                                if (bancoOriginal == null && (banco.BandeirasCredito.Count > 0 || banco.BandeirasDebito.Count > 0))
                                {
                                    bancosModificados.Add(banco);
                                }
                                else
                                {
                                    // obtem as bandeiras que nao existiam no banco original
                                    var bandeirasCredito = banco.BandeirasCredito.Where(bc =>
                                                            !bancoOriginal.BandeirasCredito.Exists(bco =>
                                                                String.Compare(bco.SiglaProduto, bc.SiglaProduto) == 0 &&
                                                                bco.TipoTransacao == bc.TipoTransacao)).ToList();

                                    var bandeirasDebito = banco.BandeirasDebito.Where(bd =>
                                                            !bancoOriginal.BandeirasDebito.Exists(bdo =>
                                                                String.Compare(bdo.SiglaProduto, bd.SiglaProduto) == 0 &&
                                                                bdo.TipoTransacao == bd.TipoTransacao)).ToList();

                                    if (bandeirasCredito.Count > 0 || bandeirasDebito.Count > 0)
                                    {
                                        bancosModificados.Add(new BancoFront
                                        {
                                            BandeirasCredito = bandeirasCredito,
                                            BandeirasDebito = bandeirasDebito,
                                            CodigoAgencia = banco.CodigoAgencia,
                                            CodigoBanco = banco.CodigoBanco,
                                            NomeBanco = banco.NomeBanco,
                                            NumeroConta = banco.NumeroConta
                                        });
                                    }

                                }
                            }
                        }
                    }

                    // se possui alguma bandeira que mudou de banco
                    if (bancosModificados.Count > 0)
                    {
                        // indica se alguns dos bancos alterados tem confirmacao eletronica
                        Boolean confirmacaoEletronicaGeral = false;

                        using (var entidadeCliente = new EntidadeServico.EntidadeServicoClient())
                        {
                            foreach (BancoFront banco in bancosModificados)
                            {
                                Boolean confirmacaoEletronica = this.ListaBancosConfirmacaoEletronica.Exists(bo => String.Compare(bo.Codigo, banco.CodigoBanco.ToString(), true) == 0);
                                if (confirmacaoEletronica && !confirmacaoEletronicaGeral)
                                    confirmacaoEletronicaGeral = true;

                                //Grupo de Solicitação - CNC: Solicitação SEM confirmação eletrônica
                                //                       S04: Solicitação COM confirmação eletrônica
                                String grupoSolicitacao = confirmacaoEletronica ? "S04" : "CNC";
                                String confEletronica = confirmacaoEletronica ? "N" : "S";

                                String codigoBanco = banco.CodigoBanco.ToString();
                                String codigoAgencia = banco.CodigoAgencia;
                                String numeroConta = banco.NumeroConta;

                                //BBF19 - substitui qlqr caracter alfa por 0 no numero da conta
                                numeroConta = Regex.Replace(numeroConta, @"[a-zA-Z]+", "0").Replace("-", String.Empty);

                                String aguardaDocumento = chkTodasFiliais.Checked ? "S" : "N";

                                //Bandeiras de Crédito que serão alteradas
                                foreach (BandeiraFront bnd in banco.BandeirasCredito)
                                {
                                    //Variável para guardar o número da REQUISAÇÃO de Alteração
                                    Int32 numeroRequicao = 0;
                                    //Variável para guardar o número da SOLICITAÇÃO de Alteração
                                    Int32 numeroSolicitacao = 0;

                                    Int32 codigoRetorno = entidadeCliente.InserirSolicitacaoAlteracaoDomicilioBancario(
                                        out numeroRequicao,
                                        out numeroSolicitacao,
                                        SessaoAtual.CodigoEntidade,
                                        grupoSolicitacao,
                                        SessaoAtual.CNPJEntidade);

                                    if (codigoRetorno > 0)
                                    {
                                        base.ExibirPainelExcecao("EntidadeServico.InserirSolicitacaoAlteracaoDomicilioBancario", codigoRetorno);
                                        return;
                                    }
                                    else
                                    {
                                        String tipoTransacao = bnd.TipoTransacao.ToString();
                                        String siglaBandeira = bnd.SiglaProduto;

                                        codigoRetorno = 0;
                                        codigoRetorno = entidadeCliente.InserirAlteracaoDomicilioBancario(
                                                            numeroRequicao,
                                                            numeroSolicitacao,
                                                            siglaBandeira,
                                                            codigoBanco,
                                                            codigoAgencia,
                                                            numeroConta,
                                                            aguardaDocumento,
                                                            confEletronica,
                                                            "99",
                                                            "355",
                                                            tipoTransacao);

                                        if (codigoRetorno > 0)
                                        {
                                            base.ExibirPainelExcecao("EntidadeServico.InserirAlteracaoDomicilioBancario", codigoRetorno);
                                            return;
                                        }
                                    }
                                }

                                //Bandeiras de Débito que serão alteradas
                                foreach (BandeiraFront bnd in banco.BandeirasDebito)
                                {
                                    //Variável para guardar o número da REQUISAÇÃO de Alteração
                                    Int32 numeroRequicao = 0;
                                    //Variável para guardar o número da SOLICITAÇÃO de Alteração
                                    Int32 numeroSolicitacao = 0;

                                    Int32 codigoRetorno = entidadeCliente.InserirSolicitacaoAlteracaoDomicilioBancario(
                                                            out numeroRequicao,
                                                            out numeroSolicitacao,
                                                            SessaoAtual.CodigoEntidade,
                                                            grupoSolicitacao,
                                                            SessaoAtual.CNPJEntidade);

                                    if (codigoRetorno > 0)
                                    {
                                        base.ExibirPainelExcecao("EntidadeServico.InserirSolicitacaoAlteracaoDomicilioBancario", codigoRetorno);
                                        return;
                                    }
                                    else
                                    {
                                        String tipoTransacao = bnd.TipoTransacao.ToString();
                                        String siglaBandeira = bnd.SiglaProduto;

                                        codigoRetorno = 0;
                                        codigoRetorno = entidadeCliente.InserirAlteracaoDomicilioBancario(
                                                            numeroRequicao,
                                                            numeroSolicitacao,
                                                            siglaBandeira,
                                                            codigoBanco,
                                                            codigoAgencia,
                                                            numeroConta,
                                                            aguardaDocumento,
                                                            confEletronica,
                                                            "99",
                                                            "355",
                                                            tipoTransacao);

                                        if (codigoRetorno > 0)
                                        {
                                            base.ExibirPainelExcecao("EntidadeServico.InserirAlteracaoDomicilioBancario", codigoRetorno);
                                            return;
                                        }
                                    }
                                }
                            }

                            ExibirComprovante(confirmacaoEletronicaGeral);

                            this.DomiciliosBancariosAlterados = hdfDadosBancariosModificado.Value;
#if DEBUG
                            hdfDadosBancariosOriginal.Value = hdfDadosBancariosModificado.Value;
#else
                            if (CarregarDomiciliosBancarios())
                            {
                                ExibirUltimaSolicitacao();
                                ExibirFormularioDomicilios();
                            }
#endif
                            hdfDadosBancariosModificado.Value = String.Empty;
                        }
                    }

                    HabilitarQuadroBancario(false);
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
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
        /// Habilita/desabilita os botoes para que a pagina possa ser editada
        /// </summary>
        private void AtualizarEdicaoPagina()
        {
            lnkBtnEditarBancos.Visible = !this.ModoEdicao && this.PermiteAlterar;
        }

        /// <summary>
        /// habilita/desabilita a edicao do formulario
        /// </summary>
        /// <param name="habilitado"></param>
        private void HabilitarQuadroBancario(bool habilitado)
        {
            this.ModoEdicao = habilitado;
            AtualizarEdicaoPagina();

            divDadosBancarios.Attributes["data-edit-mode"] = habilitado.ToString();
            divAcoesDadosBancarios.Visible = habilitado;
        }

        /// <summary>
        /// Carrega a lista de Bancos Financeiros para o grid de Alteração
        /// </summary>
        private void CarregarBancos()
        {
            using (Logger log = Logger.IniciarLog("Carregando lista de bancos financeiros"))
            {
                using (var entidadeServico = new EntidadeServico.EntidadeServicoClient())
                {
                    var bancos = entidadeServico.ConsultarBancos();
                    this.ListaBancos = new List<Banco>();
                    foreach (EntidadeServico.Banco banco in bancos)
                    {
                        this.ListaBancos.Add(banco);
                    }
                }

                if (this.ListaBancos.Count > 0)
                {
                    ddlBancos.DataSource = this.ListaBancos.Select(bc =>
                    {
                        Boolean confirmacaoEletronica = this.ListaBancosConfirmacaoEletronica.Exists(bo => String.Compare(bo.Codigo, bc.Codigo, true) == 0);
                        return new
                        {
                            Descricao = String.Format("{0} - {1}", bc.Codigo.PadLeft(3, '0'), bc.Descricao),
                            Codigo = String.Format("{0}|{1}", bc.Codigo, confirmacaoEletronica)
                        };
                    });
                    ddlBancos.DataTextField = "Descricao";
                    ddlBancos.DataValueField = "Codigo";
                    ddlBancos.DataBind();
                }
            }
        }

        /// <summary>
        /// Carregar a lista de bancos com confirmação eletrônica
        /// </summary>
        private void CarregarBancosConfirmacaoEletronica()
        {
            using (Logger log = Logger.IniciarLog("Carregando bancos com confirmação eletrônica"))
            {
                using (var entidadeServico = new EntidadeServico.EntidadeServicoClient())
                {
                    this.ListaBancosConfirmacaoEletronica = new List<Banco>();
                    var bancos = entidadeServico.ConsultarBancosConfirmacaoEletronica();
                    foreach (Banco bc in bancos)
                    {
                        this.ListaBancosConfirmacaoEletronica.Add(bc);
                    }
                }
            }
        }

        /// <summary>
        /// Verifica se há Domicílios Bancários para a Entidade
        /// </summary>
        /// <returns></returns>
        private bool CarregarDomiciliosBancarios()
        {
            using (Logger log = Logger.IniciarLog("Carregando domicílios bancários"))
            {
                Int32 codigoRetorno;
                Boolean permissaoAlteracao;
                using (var entidadeServico = new EntidadeServico.EntidadeServicoClient())
                {
                    this.DomiciliosBancarios = entidadeServico.ConsultarDadosDomiciliosBancarios(out permissaoAlteracao, out codigoRetorno, SessaoAtual.CodigoEntidade);
                    this.PermiteAlterar = permissaoAlteracao;

                    if (codigoRetorno > 0)
                    {
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarDomiciliosBancario", codigoRetorno);
                        return false;
                    }
                    else
                        return true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ExibirUltimaSolicitacao()
        {
            using (Logger log = Logger.IniciarLog("Exibindo a última solicitação de alteração de domicílio"))
            {
                using (var entidadeCliente = new EntidadeServico.EntidadeServicoClient())
                {
                    var ultimasSolicitacoes = entidadeCliente.ConsultarDomiciliosAlterados(SessaoAtual.CodigoEntidade);

                    if (ultimasSolicitacoes.Length > 0)
                    {
                        DadosDomiciolioBancario ultimaSolicitacao = ultimasSolicitacoes[0];
                        var bandeirasCreditoUltimaSolicitacao = new List<Bandeira>();
                        var bandeirasDebitoUltimaSolicitacao = new List<Bandeira>();

                        SPSecurity.RunWithElevatedPrivileges(delegate ()
                        {
                            using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                            {
                                using (SPWeb web = site.OpenWeb(SPContext.Current.Web.ID))
                                {
                                    String nomeLista = "Produtos Bandeira";

                                    SPList lista = web.Lists.TryGetList(nomeLista);
                                    if (!object.ReferenceEquals(lista, null))
                                    {
                                        String tipoOperacao = "";
                                        SPQuery query = new SPQuery();
                                        SPListItemCollection collection;

                                        foreach (DadosDomiciolioBancario domicilio in ultimasSolicitacoes)
                                        {
                                            query = new SPQuery();
                                            query.Query = String.Format("<Where><Eq><FieldRef Name=\"GE\" /><Value Type=\"Text\">{0}</Value></Eq></Where>", domicilio.BandeirasCredito[0].SiglaProduto);
                                            //collection = new SPListItemCollection();
                                            collection = lista.GetItems(query);

                                            if (collection.Count > 0)
                                            {
                                                SPListItem infoBandeira = collection[0];
                                                //infoBandeira = collection[0];
                                                tipoOperacao = infoBandeira["Produto"].ToString();

                                                if (tipoOperacao[0].Equals('C'))
                                                {
                                                    bandeirasCreditoUltimaSolicitacao.Add(domicilio.BandeirasCredito[0]);
                                                }
                                                else
                                                {
                                                    bandeirasDebitoUltimaSolicitacao.Add(domicilio.BandeirasCredito[0]);
                                                }
                                            }
                                            else
                                            {
                                                // se nao encontra a bandeira na lista SP add a bandeira na lista de credito com a sigla
                                                bandeirasCreditoUltimaSolicitacao.Add(domicilio.BandeirasCredito[0]);
                                            }
                                        }
                                    }
                                }
                            }
                        });

                        ultimaSolicitacao.NomeBanco = String.Format("Banco não identif. ({0})", ultimaSolicitacao.CodigoBanco);
                        Banco bancoSolicitacao = this.ListaBancos.FirstOrDefault(b => String.Compare(b.Codigo, ultimaSolicitacao.CodigoBanco.ToString(), true) == 0);
                        if (!object.ReferenceEquals(bancoSolicitacao, null))
                            ultimaSolicitacao.NomeBanco = bancoSolicitacao.Descricao;

                        //lblDataPedido.Text = ultimaSolicitacao.DataSolicitacao;
                        lblBancoPedido.Text = ultimaSolicitacao.NomeBanco;
                        lblDomicilioPedido.Text = String.Format("{0} / {1}", ultimaSolicitacao.CodigoAgencia, ultimaSolicitacao.NumeroConta);

                        rptBandeiraSolicitacaoCredito.DataSource = bandeirasCreditoUltimaSolicitacao;
                        rptBandeiraSolicitacaoCredito.DataBind();

                        rptBandeiraSolicitacaoDebito.DataSource = bandeirasDebitoUltimaSolicitacao;
                        rptBandeiraSolicitacaoDebito.DataBind();

                        pnlPedidosAlteracao.Visible = true;

                        Core.QuadroAviso qdAvisoGeral = (Core.QuadroAviso)this.Parent.FindControl("qdAvisoGeral");
                        qdAvisoGeral.TipoQuadro = Core.TipoQuadroAviso.Aviso;
                        qdAvisoGeral.Mensagem = String.Format("pendência: pedido de alteração dos dados bancários solicitado. {0} - {1} - Ag. {2} C/C: {3}", ultimaSolicitacao.DataSolicitacao, ultimaSolicitacao.NomeBanco, ultimaSolicitacao.CodigoAgencia, ultimaSolicitacao.NumeroConta);
                        qdAvisoGeral.Visible = true;
                    }
                }
            }
        }

        /// <summary>
        /// Carrega as informações da tela de Domicílios
        /// </summary>
        private void ExibirFormularioDomicilios()
        {
            using (Logger log = Logger.IniciarLog("Exibindo de listagem de Domicílios"))
            {
                Boolean AcessoMatriz = true;

                using (var entidadeCliente = new EntidadeServicoClient())
                {
                    Int32 codigoRetorno = 0;
                    Entidade entidade = entidadeCliente.ConsultarDadosCompletos(out codigoRetorno, SessaoAtual.CodigoEntidade, false);
                    if (codigoRetorno == 0)
                    {
                        AcessoMatriz = entidade.TipoEstabelecimento == 2;
                    }
                    else
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarDadosCompletos", codigoRetorno);
                }

                chkTodasFiliais.Visible = AcessoMatriz && PossuiFiliais();

                List<BancoFront> bancosPv = new List<BancoFront>();
                foreach (DadosDomiciolioBancario banco in this.DomiciliosBancarios)
                {
                    List<BandeiraFront> bandeirasCredito = new List<BandeiraFront>();
                    List<BandeiraFront> bandeirasDebito = new List<BandeiraFront>();
                    foreach (Bandeira bandeira in banco.BandeirasCredito)
                    {
                        // BBF Sprint 8 - antigamente exibia uma linha com ochk desabilitado e um '-', agora nao exibe essa bandeira com a sigla 0
                        if (bandeira.SiglaProduto != "0")
                        {
                            BandeiraFront bandeiraCredito = new BandeiraFront
                            {
                                DescricaoBandeira = DescricaoBandeira(bandeira.SiglaProduto),
                                SiglaProduto = bandeira.SiglaProduto,
                                TipoTransacao = bandeira.TipoTransacao,
                                Trava = bandeira.Trava,
                            };
                            bandeirasCredito.Add(bandeiraCredito);
                        }
                    }
                    foreach (Bandeira bandeira in banco.BandeirasDebito)
                    {
                        // BBF Sprint 8 - antigamente exibia uma linha com ochk desabilitado e um '-', agora nao exibe essa bandeira com a sigla 0
                        if (bandeira.SiglaProduto != "0")
                        {
                            BandeiraFront bandeiraDebito = new BandeiraFront
                            {
                                DescricaoBandeira = DescricaoBandeira(bandeira.SiglaProduto),
                                SiglaProduto = bandeira.SiglaProduto,
                                TipoTransacao = bandeira.TipoTransacao,
                                Trava = bandeira.Trava,
                            };
                            bandeirasDebito.Add(bandeiraDebito);
                        }
                    }

                    BancoFront bancoFront = new BancoFront
                    {
                        CodigoBanco = banco.CodigoBanco,
                        NomeBanco = banco.NomeBanco,
                        CodigoAgencia = banco.CodigoAgencia,
                        NumeroConta = banco.NumeroConta,
                        ConfirmacaoEletronica = this.ListaBancosConfirmacaoEletronica.Exists(bo => String.Compare(bo.Codigo, banco.CodigoBanco.ToString(), true) == 0),
                        BandeirasCredito = bandeirasCredito,
                        BandeirasDebito = bandeirasDebito,
                    };

                    bancosPv.Add(bancoFront);
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                hdfDadosBancariosOriginal.Value = serializer.Serialize(bancosPv);

                //se nao permite alterar exibe quadro de aviso
                if (!this.PermiteAlterar)
                {
                    qdAvisoDadosBancarios.Visible = true;
                    qdAvisoDadosBancarios.TipoQuadro = Core.TipoQuadroAviso.Aviso;
                    qdAvisoDadosBancarios.Mensagem = "Não é possível realizar alteração de domicílio bancário";
                }
            }
        }

        /// <summary>
        /// Verifica se o PV possui filiais para exibir checkbox de filiais
        /// </summary>
        /// <returns></returns>
        private Boolean PossuiFiliais()
        {
            using (Logger log = Logger.IniciarLog("Consultando filiais"))
            {
                Int32 codigoRetorno = 0;
                Boolean retorno = false;

                // Chama o serviço que carrega as filiais para o Estabelecimento atual
                using (EntidadeServico.EntidadeServicoClient client = new EntidadeServico.EntidadeServicoClient())
                {
                    EntidadeServico.Filial[] filiais = client.ConsultarFiliais(out codigoRetorno, SessaoAtual.CodigoEntidade, 2);

                    if (codigoRetorno > 0)
                    {
                        retorno = false;
                    }
                    else
                    {
                        retorno = (filiais.Length > 0);
                    }
                }

                return retorno;
            }
        }

        /// <summary>
        /// Retorna o nome da bandeira de acordo com a lista de Bandeiras
        /// </summary>
        /// <param name="siglaBandeira">Sigla da bandeira que será buscada</param>
        /// <returns>Descrição da bandeira. Caso não encontre a bandeira, retorna "..."</returns>
        private String DescricaoBandeira(String siglaBandeira)
        {
            String descricaoBandeira = siglaBandeira;

            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb(SPContext.Current.Web.ID))
                    {
                        String nomeLista = "Produtos Bandeira";

                        SPList lista = web.Lists.TryGetList(nomeLista);
                        if (!object.ReferenceEquals(lista, null))
                        {
                            SPQuery query = new SPQuery();
                            query.Query = String.Format("<Where><Eq><FieldRef Name=\"GE\" /><Value Type=\"Text\">{0}</Value></Eq></Where>", siglaBandeira);

                            SPListItemCollection collection = lista.GetItems(query);

                            if (collection.Count > 0)
                            {
                                SPListItem infoBandeira = collection[0];
                                if (infoBandeira["Bandeira"] != null)
                                    descricaoBandeira = infoBandeira["Bandeira"].ToString();
                            }
                        }
                    }
                }
            });

            return descricaoBandeira;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="confirmacaoEletronica"></param>
        private void ExibirComprovante(Boolean confirmacaoEletronica)
        {
            using (Logger log = Logger.IniciarLog("Exibir comprovante da alteração"))
            {
                webControls.Image imgModalConfirmacao = (webControls.Image)this.Parent.FindControl("imgModalConfirmacao");
                webControls.Literal ltrTituloModalConfirmacao = (webControls.Literal)this.Parent.FindControl("ltrTituloModalConfirmacao");
                Core.AreaTexto artModalConfirmacao = (Core.AreaTexto)this.Parent.FindControl("artModalConfirmacao");

                if (imgModalConfirmacao != null && imgModalConfirmacao != null && artModalConfirmacao != null)
                {
                    StringBuilder mensagem = new StringBuilder();
                    ltrTituloModalConfirmacao.Text = "o pedido de alteração foi realizado com sucesso!";
                    if (!confirmacaoEletronica)
                    {
                        String emailRede = "dadosbancarios@userede.com.br";
                        imgModalConfirmacao.ImageUrl = "/_layouts/DadosCadastrais/Images/warning.png";

                        mensagem.Append("<p>É necessário o envio de documentos para comprovação do domicílio bancário para o e-mail: ");
                        mensagem.AppendFormat("<a href='mailto:{0}?subject=Confirmação de domicílio bancário'><strong>{0}.</strong></a></p>", emailRede);
                        artModalConfirmacao.Controls.Add(new LiteralControl(mensagem.ToString()));
                    }
                    else
                    {
                        mnuAcoes.Visible = true;

                        imgModalConfirmacao.ImageUrl = "/_layouts/DadosCadastrais/Images/smile.png";

                        mensagem.Append("<p>importante saber que a alteração não é imediata, demora alguns dias<br />");
                        mensagem.Append("para alterar essas informações.<br /><br />");
                        mensagem.Append("Caso queira fazer o download do PDF desta solicitação, utilize o menu lateral.");
                        artModalConfirmacao.Controls.Add(new LiteralControl(mensagem.ToString()));
                    }

                    String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { dadosGeraisOpenModal('[id$=lbxModalConfirmacao]', true); }, 'SP.UI.Dialog.js');";
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AbrirModalDialog", javaScript, true);
                }
            }
        }

        /// <summary>
        /// Gera o excel do Comprovante
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void linkExcel_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Início impressão Excel"))
            {
                try
                {
                    if (!String.IsNullOrWhiteSpace(hdnConteudoExportacao.Value))
                    {
                        String csvContent = hdnConteudoExportacao.Value;
                        String nomeArquivo = "comprovantedomicilio" + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                        log.GravarMensagem("Nome do arquivo Excel", nomeArquivo);

                        //Define o Enconding para Ansi
                        HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding(1252);

                        //Retorna
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.ContentType = "application/ms-excel";
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);//Define para não ter cache
                        HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + nomeArquivo);    //Define o nome do arquivo
                        using (StringWriter sw = new StringWriter())
                        {
                            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                            {
                                HttpContext.Current.Response.Write(csvContent);
                            }
                        }
                    }
                }
                catch (ArgumentException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
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
        /// Gera o PDF do Comprovante
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkPDF_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Início imprimessão Excel"))
            {
                try
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    List<BancoFront> bancos = serializer.Deserialize<List<BancoFront>>(this.DomiciliosBancariosAlterados);

                    DateTime horaImpressao = DateTime.Now;
                    String nomeArquivo = "comprovantedomicilio" + "_" + horaImpressao.ToString("yyyyMMdd") + ".pdf";

                    PdfPTable tabelaComprovante = new PdfPTable(2);
                    tabelaComprovante.TotalWidth = 250;
                    Document doc = new Document(PageSize.A2);

                    PdfPCell celulaCabecalho = new PdfPCell((new PdfPCell(new Phrase(new Phrase("Data da Alteração: " + horaImpressao.ToShortDateString() + " às " + horaImpressao.ToShortTimeString() + "", new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(25, 60, 145))))) { Padding = 5, Border = 0, Colspan = 9, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 2 }));
                    PdfPCell celulaTitulo = new PdfPCell((new PdfPCell(new Phrase(new Phrase("COMPROVANTE DE ALTERAÇÃO DE DOMICÍLIO", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.WHITE)))) { Padding = 5, Top = 5, Colspan = 2, BackgroundColor = new BaseColor(25, 60, 145), HorizontalAlignment = 0 }));

                    celulaTitulo.Colspan = 2;
                    celulaTitulo.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right            

                    celulaCabecalho.Colspan = 2;

                    tabelaComprovante.AddCell(celulaCabecalho);
                    tabelaComprovante.AddCell(celulaTitulo);

                    PdfPCell celulaInfo = new PdfPCell();
                    //new Phrase(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, BaseColor.BLACK)))) { Padding = 5, Border = 0, Colspan = 9, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 0 })
                    Font fonteTabela = new Font(Font.FontFamily.HELVETICA, 8, 1, BaseColor.BLACK);
                    BaseColor corLinha = BaseColor.WHITE;
                    BaseColor corLinhaAlt = new BaseColor(228, 228, 228);

                    celulaInfo.Phrase = new Phrase("", fonteTabela);
                    celulaInfo.Padding = 5;
                    celulaInfo.Colspan = 2;
                    celulaInfo.Border = 0;
                    celulaInfo.BackgroundColor = BaseColor.WHITE;
                    celulaInfo.HorizontalAlignment = 0;
                    tabelaComprovante.AddCell(celulaInfo);

                    foreach (BancoFront banco in bancos)
                    {
                        if (banco.BandeirasCredito.Count == 0 && banco.BandeirasDebito.Count == 0)
                            continue;

                        //Domicílio Alteração
                        celulaInfo.Phrase = new Phrase(String.Format("{0} - Ag. {1} C/C {2}", banco.NomeBanco, banco.CodigoAgencia, banco.NumeroConta), fonteTabela);
                        celulaInfo.Padding = 5;
                        celulaInfo.Colspan = 2;
                        celulaInfo.Border = 1;
                        celulaInfo.HorizontalAlignment = 0;
                        celulaInfo.BackgroundColor = new BaseColor(129, 129, 129);
                        tabelaComprovante.AddCell(celulaInfo);

                        celulaInfo.Phrase = new Phrase("Crédito", fonteTabela);
                        celulaInfo.Padding = 5;
                        celulaInfo.Colspan = 1;
                        celulaInfo.Border = 1;
                        celulaInfo.HorizontalAlignment = 1;
                        celulaInfo.BackgroundColor = new BaseColor(129, 129, 129);
                        tabelaComprovante.AddCell(celulaInfo);

                        celulaInfo.Phrase = new Phrase("Débito", fonteTabela);
                        celulaInfo.Padding = 5;
                        celulaInfo.Colspan = 1;
                        celulaInfo.Border = 1;
                        celulaInfo.HorizontalAlignment = 1;
                        celulaInfo.BackgroundColor = new BaseColor(129, 129, 129);
                        tabelaComprovante.AddCell(celulaInfo);

                        //Verifica em qual Operação há mais bandeiras
                        Int32 maisBandeiras = banco.BandeirasCredito.Count > banco.BandeirasDebito.Count ? banco.BandeirasCredito.Count : banco.BandeirasDebito.Count;

                        for (Int32 i = 0; i <= (maisBandeiras - 1); i++)
                        {
                            String bandeiraCredito = String.Empty;
                            String bandeiraDebito = String.Empty;

                            if (banco.BandeirasCredito.Count > i)
                                bandeiraCredito = banco.BandeirasCredito[i].DescricaoBandeira;
                            if (banco.BandeirasDebito.Count > i)
                                bandeiraDebito = banco.BandeirasDebito[i].DescricaoBandeira;

                            celulaInfo.Phrase = new Phrase(bandeiraCredito, fonteTabela);
                            celulaInfo.Padding = 5;
                            celulaInfo.Colspan = 1;
                            celulaInfo.Border = 1;
                            celulaInfo.HorizontalAlignment = 1;
                            celulaInfo.BackgroundColor = i % 2 > 0 ? corLinhaAlt : corLinha;
                            tabelaComprovante.AddCell(celulaInfo);

                            celulaInfo.Phrase = new Phrase(bandeiraDebito, fonteTabela);
                            celulaInfo.Padding = 5;
                            celulaInfo.Colspan = 1;
                            celulaInfo.Border = 1;
                            celulaInfo.HorizontalAlignment = 1;
                            celulaInfo.BackgroundColor = i % 2 > 0 ? corLinhaAlt : corLinha;
                            tabelaComprovante.AddCell(celulaInfo);
                        }
                    }
                    MemoryStream ms = new MemoryStream();
                    PdfWriter.GetInstance(doc, ms);
                    doc.Open();

                    //string img = "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";
                    //byte[] array = Convert.FromBase64String(img);
                    //iTextSharp.text.Image headerImage = iTextSharp.text.Image.GetInstance(array);
                    //headerImage.ScaleToFit(800, 300);

                    //doc.Add(headerImage);

                    doc.Add(tabelaComprovante);
                    doc.Close();

                    byte[] file = ms.GetBuffer();
                    //byte[] buffer = new byte[4096];

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", String.Format("attachment; filename={0}", nomeArquivo));
                    Response.BinaryWrite(file);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();

                }
                catch (ArgumentException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }
    }
}