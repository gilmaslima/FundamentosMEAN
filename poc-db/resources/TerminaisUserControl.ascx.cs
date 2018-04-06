using Microsoft.SharePoint;
using Rede.PN.CondicaoComercial.SharePoint.MaximoServico;
using Rede.PN.CondicaoComercial.SharePoint.ZPDadosCadastraisServico;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rede.PN.CondicaoComercial.SharePoint.ControlTemplates.Terminais
{
    public partial class TerminaisUserControl : UserControlBase
    {
        #region [ Variáveis/Controles ]

        /// <summary>
        /// Terminais bancários
        /// </summary>
        private TerminalBancario[] Terminais
        {
            get
            {
                if (ViewState["Terminais"] == null)
                    ViewState["Terminais"] = new TerminalBancario[0];
                return (TerminalBancario[])ViewState["Terminais"];
            }
            set { ViewState["Terminais"] = value; }
        }

        /// <summary>
        /// Terminais do Sistema Máximo
        /// </summary>
        private List<TerminalDetalhado> TerminaisMaximo
        {
            get
            {
                if (ViewState["TerminaisMaximo"] == null)
                    ViewState["TerminaisMaximo"] = new List<TerminalDetalhado>();
                return (List<TerminalDetalhado>)ViewState["TerminaisMaximo"];
            }
            set { ViewState["TerminaisMaximo"] = value; }
        }

        #endregion

        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.CarregarTerminais();
                this.CarregarTerminaisMaximo();
            }
        }

        /// <summary>
        /// Carrega os terminais na tela
        /// </summary>
        private void CarregarTerminais()
        {
            using (Logger Log = Logger.IniciarLog("Terminais - Carregando terminais"))
            {
                try
                {
                    //Parâmetros para chamada e retorno
                    Int16 codRetorno = 0;
                    Int32 dataPesquisa = Request["data"].ToInt32(DateTime.Now.AddMonths(-1).ToString("yyyyMM").ToInt32());

                    this.Terminais = null;

                    using (var client = new HISServicoZP_DadosCadastraisClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico, new { SessaoAtual.CodigoEntidade, dataPesquisa });
                        this.Terminais = client.ObterTecnologia(out codRetorno, SessaoAtual.CodigoEntidade, dataPesquisa);
                        Log.GravarLog(EventoLog.RetornoServico, new { this.Terminais, codRetorno });

                        if (codRetorno > 0)
                            base.ExibirPainelExcecao("ZPDadosCadastraisServico.ObterTecnologia", codRetorno);
                    }
                }
                catch (FaultException<ZPDadosCadastraisServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
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
        /// Consulta terminais através do Sistema Máximo
	/// --------------------------------------------------
	/// Alterações
	/// ===========
	/// 19/04/2017: Remoção do filtro de tecnologias
	///		Em conferência com a área de Canais, ficou estabelecido que o filtro de tecnologias não é mais necessário
	/// --------------------------------------------------
        /// </summary>
        private void CarregarTerminaisMaximo()
        {
            using (Logger Log = Logger.IniciarLog("Sistema Máximo - Carregando terminais"))
            {
                try
                {
                    //Prepara parâmetros para consulta no Sistema Máximo
                    FiltroTerminal filtroTerminal = new FiltroTerminal();
                    filtroTerminal.PontoVenda = SessaoAtual.CodigoEntidade.ToString();
                    filtroTerminal.Situacao = TipoTerminalStatus.EMPRODUCAO;

                    //Limpa terminais já carregados
                    this.TerminaisMaximo.Clear();

                    //Consulta Sistema Máximo
                    using (var contexto = new ContextoWCF<MaximoServicoClient>())
                        this.TerminaisMaximo = contexto.Cliente.ConsultarTerminalDetalhado(filtroTerminal);

                    //Configura exibição de dados ou quadro de aviso
                    if (this.TerminaisMaximo == null || this.TerminaisMaximo.Count == 0)
                    {
                        mvTerminais.SetActiveView(vwSemDados);
                        qdAvisoSemProduto.Mensagem = "Não há tecnologia cadastrada.";
                    }
                    else
                    {
                        CarregarRepeaterTerminais();
                    }
                }
                catch (FaultException<MaximoServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
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
        /// Método para carregar os terminais com de-para no conta certa
        /// </summary>
        private void CarregarRepeaterTerminais()
        {
            using (Logger Log = Logger.IniciarLog("Sistema Máximo - Carregando terminais"))
            {
                try
                {
                    ContaCertaServico.FilialTerminais filial = new ContaCertaServico.FilialTerminais();
                    filial.Filial = new ContaCertaServico.Filial
                    {
                        PontoVenda = SessaoAtual.CodigoEntidade
                    };

                    filial.Terminais = new List<ContaCertaServico.TerminalContaCerta>();

                    this.TerminaisMaximo.ForEach(t =>
                    {
                        filial.Terminais.Add(new ContaCertaServico.TerminalContaCerta
                        {
                            TerminalBancario = new ContaCertaServico.TerminalBancario
                            {
                                NumeroTerminal = t.NumeroLogico,
                                TipoEquipamento = t.TipoEquipamento
                            },
                            IndicadorContaCerta = false,
                            IndicadorFlex = t.Flex
                        });
                    });

                    List<ContaCertaServico.FilialTerminais> filiais = new List<ContaCertaServico.FilialTerminais>();
                    filiais.Add(filial);

                    List<ContaCertaServico.FilialTerminais> terminais = new List<ContaCertaServico.FilialTerminais>();

                    using (ContaCertaServico.ContaCertaServicoClient client = new ContaCertaServico.ContaCertaServicoClient())
                        terminais = client.VerificaTerminalContaCerta(filiais);

                    if (terminais.Count > 0 && terminais.FirstOrDefault().Terminais.Count > 0)
                    {
                        var terminaisListagem = terminais.FirstOrDefault().Terminais;

                        // troca os nomes dos equipamentos conforme lista do Sharepoint
                        Dictionary<String, String> dicEquipamentos = this.GetDicionarioEquipamentos();
                        if (dicEquipamentos != null && dicEquipamentos.Count > 0)
                        {
                            foreach (var item in terminaisListagem)
                            {
                                String descricaoEquipamento = String.Empty;
                                dicEquipamentos.TryGetValue(item.TerminalBancario.TipoEquipamento, out descricaoEquipamento);
                                if (!String.IsNullOrEmpty(descricaoEquipamento))
                                {
                                    item.TerminalBancario.TipoEquipamento =
                                        String.Format("{0} ({1})", descricaoEquipamento, item.TerminalBancario.TipoEquipamento);
                                }
                            }
                        }
                        rptTecnologia.DataSource = terminaisListagem;
                        rptTecnologia.DataBind();
                        hdnContemRegistrosTerminais.Value = "1";
                    }
                    else
                    {
                        mvTerminais.SetActiveView(vwSemDados);
                        qdAvisoSemProduto.Mensagem = "Não há tecnologia cadastrada.";
                    }
                }
                catch (FaultException<ContaCertaServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (FaultException<MaximoServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
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
        /// Preenche a linha da tabela com os dados de um terminal
        /// </summary>
        protected void rptTecnologia_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //var rdoTecnologia = e.Item.FindControl("rdoTecnologia") as RadioButton;
                var lblTipoEquipamento = e.Item.FindControl("lblTipoEquipamento") as Label;
                var lblNumeroTerminal = e.Item.FindControl("lblNumeroTerminal") as Label;
                var lblValorCobrado = e.Item.FindControl("lblValorCobrado") as Label;
                //var lblEquipamentoFlex = e.Item.FindControl("lblEquipamentoFlex") as Label;
                var lblNumeroLogico = e.Item.FindControl("lblNumeroLogico") as Label;
                var terminal = e.Item.DataItem as ContaCertaServico.TerminalContaCerta;

                //Busca o terminal equivalente no Sistema Máximo
                var terminalZP = this.Terminais.FirstOrDefault(t => (t.NumeroTerminal ?? String.Empty)
                    .Equals(terminal.TerminalBancario.NumeroTerminal, StringComparison.InvariantCultureIgnoreCase));
                Decimal valorCobrado = terminalZP != null ? terminalZP.ValorCobrado : 0;

                if (terminal.IndicadorContaCerta)
                {
                    valorCobrado = 0;
                    (e.Item.FindControl("imgInterrogacao")).Visible = true;
                }

                lblTipoEquipamento.Text = terminal.TerminalBancario.TipoEquipamento;
                lblNumeroTerminal.Text = terminal.TerminalBancario.NumeroTerminal;
                //lblEquipamentoFlex.Text = terminal.IndicadorFlex ? "sim" : "não";
                lblValorCobrado.Text = valorCobrado.ToString("N2", new CultureInfo("pt-BR"));

                lblNumeroLogico.Text = string.Empty;
                if (string.Compare(terminal.TerminalBancario.TipoEquipamento, "PDV", true) == 0)
                {
                    /* A composição do número lógico deve conter 15 dígitos: 
                     * - Número do terminal + 0 + número do PV. 
                     *
                     * Caso a junção do número do terminal TEF + o número do PV não atinjam 15 dígitos, 
                     * deve-se acrescentar "0" entre o terminal e o PV. 
                     * 
                     * A quantidade de "0" deve ser suficiente para que a composição do número lógico alcance 15 dígitos
                     * 
                     * Ex: PVXXXXXX000NNNNNN
                     */

                    string numeroLogico = Regex.Replace(terminal.TerminalBancario.NumeroTerminal, @"[^0-9]+", "");
                    string numeroPv = SessaoAtual.CodigoEntidade.ToString();
                    int len = numeroLogico.Length + numeroPv.Length + 1;
                    int zerosPad = numeroPv.Length;

                    if (len < 15)
                        zerosPad += 15 - len;

                    lblNumeroLogico.Text = string.Concat(numeroLogico, "0", numeroPv.PadLeft(zerosPad, '0'));
                }
            }
        }

        /// <summary>
        /// Lista de documentos por motivo de chargeback
        /// </summary>
        private Dictionary<String, String> GetDicionarioEquipamentos()
        {
            // verifica se a lista já foi consultada
            if (Session["DicionarioEquipamentos"] == null)
            {
                try
                {
                    SPList lista = null;
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (SPSite spSite = SPContext.Current.Site.WebApplication.Sites["sites/fechado"])
                        using (SPWeb spWeb = spSite.RootWeb)
                            lista = spWeb.Lists.TryGetList("Dicionario");
                    });

                    if (lista == null)
                    {
                        Logger.GravarErro("Lista 'Dicionario' encontrada");
                        SharePointUlsLog.LogErro("Lista 'Dicionario' encontrada");
                        return null;
                    }

                    SPQuery query = new SPQuery
                    {
                        Query = @"
<Where>
    <Eq>
        <FieldRef Name=""Categoria"" />
        <Value Type=""Text"">TERMINAIS_DE_PARA</Value>
    </Eq>
</Where>"
                    };

                    SPListItemCollection spList = lista.GetItems(query);
                    if (spList.Count == 0)
                    {
                        String logErro = String.Format("Nenhum registro retornado na busca; query executada: {0}", query.Query);
                        Logger.GravarErro(logErro);
                        SharePointUlsLog.LogErro(logErro);
                        return null;
                    }

                    Dictionary<String, String> dicEquipamentos = new Dictionary<String, String>();
                    foreach (var item in spList.Cast<SPListItem>())
                    {
                        String
                            key = Convert.ToString(item["Chave"]),
                            value = Convert.ToString(item["Valor"]);
                        
                        if (!dicEquipamentos.ContainsKey(key))
                            dicEquipamentos.Add(key, value);
                    }

                    Session["DicionarioEquipamentos"] = dicEquipamentos;
                }
                catch (WebException ex)
                {
                    Logger.GravarErro("Erro ao consultar a API de dicionário", ex);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro ao consultar a API de dicionário", ex);
                    SharePointUlsLog.LogErro(ex);
                }
            }

            return (Dictionary<String, String>)Session["DicionarioEquipamentos"];
        }

        #region [ Botões Inferioes de Ação ]

        /// <summary>Voltar para a tela de quadros de menu de Informações Cadastrais</summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            String url = String.Format("{0}/Paginas/pn_InformacoesCadastrais.aspx", base.web.ServerRelativeUrl);
            Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        #endregion

    }
}
