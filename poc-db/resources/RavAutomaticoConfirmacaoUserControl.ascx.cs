/*
(c) Copyright [ANO] Redecard S.A.
Autor : [Tiago]
Empresa : [BRQ IT Services]
Histórico:
- [01/08/2012] – [Tiago] – [Etapa inicial]
*/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.RAV.Sharepoint.ModuloRAV;
using Redecard.PN.Comum;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;

namespace Redecard.PN.RAV.Sharepoint.WebParts.RavAutomaticoConfirmacao
{
    public partial class RavAutomaticoConfirmacaoUserControl : UserControlBase
    {
        #region Constantes
        public const string FONTE = "RavAutomaticoConfirmacaoUserControl.ascx";
        public const int CODIGO_ERRO_LOAD = 3013;
        public const int CODIGO_ERRO_CONTINUAR = 3014;
        public const int CODIGO_ERRO_CONFIRMAR = 3015;
        #endregion

        #region Atributos
        private string _validaSenha = bool.FalseString;
        private string _tipoVenda = "";
        private string _periodoRecebimento = "";
        private string _bandeirasRAVAuto = "";
        private string _diaAntecipacao = "";
        private string _diaSemana = "";

        /// <summary>
        /// Dados RAV Automático
        /// </summary>
        public ModRAVAutomatico DadosRavAutomatico
        {
            get { return (ModRAVAutomatico)ViewState["DadosRavAutomatico"]; }
            set { ViewState["DadosRavAutomatico"] = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Confirmação RAV Automático - Page Load"))
            {
                if (Request.QueryString["dados"] != null)
                {

                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                    if (string.IsNullOrEmpty(queryString["AcessoSenha"]))
                    {
                        Response.Redirect("pn_rav.aspx", false);
                        return;
                    }
                    if (queryString["AcessoSenha"].CompareTo(bool.TrueString) != 0)
                    {
                        Response.Redirect("pn_rav.aspx", false);
                        return;
                    }

                    SharePointUlsLog.LogMensagem(queryString["AcessoSenha"]);
                    Log.GravarMensagem(queryString["AcessoSenha"]);

                    _validaSenha = queryString["AcessoSenha"];
                    _bandeirasRAVAuto = queryString["RavAutomaticoBandeiras"] != null ? queryString["RavAutomaticoBandeiras"] : "";
                    _tipoVenda = queryString["RavAutomaticoTipoVenda"].ToString();
                    _periodoRecebimento = queryString["RavAutomaticoPeriodoRecebimento"] != null ? queryString["RavAutomaticoPeriodoRecebimento"] : "";
                    _diaSemana = queryString["RavAutomaticoDiaSemana"] != null ? queryString["RavAutomaticoDiaSemana"] : "";
                    _diaAntecipacao = queryString["RavAutomaticoDiaAntecipacao"] != null ? queryString["RavAutomaticoDiaAntecipacao"] : "";

                    if (!Page.IsPostBack)
                    {
                        // O usuario do tipo atendimento tem permissao apenas para visualizar a pagina
                        if (SessaoAtual != null && SessaoAtual.UsuarioAtendimento)
                        {
                            btnConfirmar.Visible = false;
                        }

                        try
                        {
                            using (ServicoPortalRAVClient cliente = new ServicoPortalRAVClient())
                            {
                                ModRAVAutomatico automatico = null;
                                if (queryString["RavAutomaticoTipoVenda"] != null && queryString["RavAutomaticoPeriodoRecebimento"] != null)
                                {
                                    if (queryString["RavAutomaticoDataIni"] != null && queryString["RavAutomaticoDataFim"] != null && queryString["RavAutomaticoTipoVenda"] != null && queryString["RavAutomaticoValorMinimo"] != null)
                                    {
                                        String diaSemana = "";
                                        if (!String.IsNullOrEmpty(queryString["RavAutomaticoDiaSemana"]))
                                            diaSemana = queryString["RavAutomaticoDiaSemana"].ToString();

                                        automatico = cliente.ConsultarRAVAutomaticoPersonalizado(SessaoAtual.CodigoEntidade,
                                            Convert.ToChar(queryString["RavAutomaticoTipoVenda"]),
                                            Convert.ToChar(queryString["RavAutomaticoPeriodoRecebimento"]),
                                            queryString["RavAutomaticoValorMinimo"].ToDecimal(),
                                            queryString["RavAutomaticoDataIni"].ToDate(),
                                            queryString["RavAutomaticoDataFim"].ToDate(),
                                            diaSemana,
                                            queryString["RavAutomaticoDiaAntecipacao"].ToString(),
                                            queryString["RavAutomaticoBandeiras"].ToString());
                                    }
                                    else
                                    {
                                        automatico = cliente.SimularRAVAutomatico(SessaoAtual.CodigoEntidade,
                                            Convert.ToChar(_tipoVenda),
                                            Convert.ToChar(_periodoRecebimento));
                                        DadosRavAutomatico = automatico;
                                    }

                                    if (queryString["RavAutomaticoTipoVenda"] != null)
                                    {
                                        if (queryString["RavAutomaticoTipoVenda"].ToString() == "P")
                                        {
                                            lblTipoVenda.Text = "Parcelado";
                                        }
                                        else if (queryString["RavAutomaticoTipoVenda"].ToString() == "R")
                                        {
                                            lblTipoVenda.Text = "À Vista";
                                        }
                                        else
                                        {
                                            lblTipoVenda.Text = "Ambos (À Vista e Parcelado)";
                                        }
                                    }

                                    if (queryString["RavAutomaticoPeriodoRecebimento"] != null)
                                    {
                                        if (queryString["RavAutomaticoPeriodoRecebimento"].ToString() == "Q")
                                        {
                                            lblRecebimento.Text = "Quinzenal";
                                        }
                                        else if (queryString["RavAutomaticoPeriodoRecebimento"].ToString() == "S")
                                        {
                                            lblRecebimento.Text = "Semanal";
                                        }
                                        else if (queryString["RavAutomaticoPeriodoRecebimento"].ToString() == "M")
                                        {
                                            lblRecebimento.Text = "Mensal";
                                        }
                                        else
                                        {
                                            lblRecebimento.Text = "Diário";
                                        }
                                    }
                                }


                                //lblTipoVenda.Text = GetTipoVendaDescricao();
                                if (automatico.DadosRetorno.CodRetorno == 0)
                                {
                                    if (queryString["RavAutomaticoValorMinimo"] != null && !string.IsNullOrEmpty(queryString["RavAutomaticoValorMinimo"]))
                                    {
                                        lblValorMinimo.Text = queryString["RavAutomaticoValorMinimo"].ToString();
                                    }
                                    else
                                    {
                                        if (automatico.ValorMinimo > 30)
                                        {
                                            lblValorMinimo.Text = automatico.ValorMinimo.ToString();
                                        }
                                        else
                                        {
                                            lblValorMinimo.Text = "30,00";
                                        }
                                    }


                                    if (queryString["RavAutomaticoTipoAntecipacao"] != null)
                                    {
                                        Session["RavAutomaticoTipoAntecipacaoTEXTO"] = queryString["RavAutomaticoTipoAntecipacao"].ToString();
                                        lblTipoAntecipacao.Text = RAVComum.RetornaAliasProdutoAntecipacao(queryString["RavAutomaticoTipoAntecipacao"].ToString());
                                    }
                                    else
                                    {
                                        Session["RavAutomaticoTipoAntecipacaoTEXTO"] = automatico.DadosRetorno.NomeProdutoAntecipacao;
                                        lblTipoAntecipacao.Text = RAVComum.RetornaAliasProdutoAntecipacao(automatico.DadosRetorno.NomeProdutoAntecipacao);
                                    }

                                    if (queryString["RavAutomaticoDataIni"] != null)
                                    {
                                        lblPeriodoAntecipacaoInicio.Text = queryString["RavAutomaticoDataIni"].ToString();
                                    }
                                    else
                                    {
                                        lblPeriodoAntecipacaoInicio.Text = automatico.DataVigenciaIni.ToString("dd/MM/yyyy");
                                    }


                                    if (queryString["RavAutomaticoDataFim"] != null)
                                    {
                                        lblPeriodoAntecipacaoFim.Text = queryString["RavAutomaticoDataFim"].ToString();
                                    }
                                    else
                                    {
                                        lblPeriodoAntecipacaoFim.Text = automatico.DataVigenciaFim.ToString("dd/MM/yyyy");
                                    }

                                    lblTaxa.Text = automatico.DadosRetorno.TaxaCategoria.ToString("0.0,0");

                                    if (queryString["RavAutomaticoBandeiras"] != null && !string.IsNullOrEmpty(queryString["RavAutomaticoBandeiras"]))
                                    {
                                        string[] bandeiras = queryString["RavAutomaticoBandeiras"].ToString().Split(';');
                                        if (bandeiras.Length > 0)
                                        {
                                            List<string> bandeirasDS = new List<string>();
                                            foreach (string bandeira in bandeiras)
                                            {
                                                if (bandeira.Split('#')[2] == "S")
                                                    bandeirasDS.Add(bandeira);
                                            }
                                            rptBandeiras.DataSource = bandeirasDS;
                                            rptBandeiras.DataBind();
                                        }
                                    }
                                    else if (automatico.Bandeiras.Count > 0)
                                    {
                                        List<ModRAVAutomaticoBandeira> listaBandeiras = automatico.Bandeiras;

                                        List<string> bandeirasDS = new List<string>();
                                        String bnd = "";
                                        foreach (ModRAVAutomaticoBandeira bandeira in listaBandeiras)
                                        {
                                            bnd = "";
                                            if (bandeira.IndSel == "S")
                                            {
                                                bnd = String.Format("{0}#{1}", bandeira.CodBandeira, bandeira.DscBandeira);
                                                bandeirasDS.Add(bnd);
                                            }
                                        }
                                        rptBandeiras.DataSource = bandeirasDS;
                                        rptBandeiras.DataBind();
                                    }
                                }
                                else
                                    base.ExibirPainelExcecao(FONTE, automatico.DadosRetorno.CodRetorno);

                                //Session["DadosRavAutomaticoComprovante"] = automatico;
                            }
                        }
                        catch (FaultException<ServicoRAVException> ex)
                        {
                            Log.GravarErro(ex);
                            base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                            SharePointUlsLog.LogErro(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Log.GravarErro(ex);
                            SharePointUlsLog.LogErro(ex.Message);
                            base.ExibirPainelExcecao(FONTE, CODIGO_ERRO_LOAD);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Retorna para a tela principal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Voltar(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Confirmação RAV Automático - Voltar"))
            {
                try
                {
                    QueryStringSegura queryString = new QueryStringSegura();
                    queryString["AcessoSenha"] = _validaSenha;
                    Response.Redirect(string.Format("pn_Principal.aspx?dados={0}", queryString.ToString()), false);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex.Message);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO_LOAD);
                }
            }
        }

        /// <summary>
        /// Redireciona o usuário para a tela de personalização do RAV Automático.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Alterar(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Confirmação RAV Automático - Alterar"))
            {
                try
                {
                    QueryStringSegura queryString = new QueryStringSegura();
                    queryString["AcessoSenha"] = _validaSenha;
                    queryString["RavAutomaticoValorMinimo"] = lblValorMinimo.Text;
                    queryString["RavAutomaticoTipoVenda"] = retornaTipoVenda(lblTipoVenda.Text);
                    queryString["RavAutomaticoPeriodoRecebimento"] = retornaPeriodo(lblRecebimento.Text);
                    queryString["RavAutomaticoDataIni"] = lblPeriodoAntecipacaoInicio.Text;
                    queryString["RavAutomaticoDataFim"] = lblPeriodoAntecipacaoFim.Text;
                    queryString["RavAutomaticoBandeiras"] = _bandeirasRAVAuto;
                    queryString["RavAutomaticoDiaAntecipacao"] = _diaAntecipacao;
                    queryString["RavAutomaticoDiaSemana"] = _diaSemana;

                    Response.Redirect(string.Format("pn_PersonalizacaoRavAutomatico.aspx?dados={0}", queryString.ToString()), false);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex.Message);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO_LOAD);
                }
            }
        }


        protected string retornaTipoVenda(string tipo)
        {
            string retorno = "";
            if (tipo == "À Vista")
            {
                retorno = "R";
            }
            else if (tipo == "Parcelado")
            {
                retorno = "P";
            }
            else
            {
                retorno = "A";
            }
            return retorno;
        }


        protected string retornaPeriodo(string periodo)
        {
            string retorno = "";
            if (periodo == "Semanal")
            {
                retorno = "S";
            }
            else if (periodo == "Quinzenal")
            {
                retorno = "Q";
            }
            else if (periodo == "Mensal")
            {
                retorno = "M";
            }
            else
            {
                retorno = "D";
            }
            return retorno;

        }


        /// <summary>
        /// Confirma o cadastro do RAV Automático.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Confirmar(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Confirmação RAV Automático - Confirmar"))
            {
                try
                {
                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);

                    DateTime ravAutomaticoDataIni = queryString["RavAutomaticoDataIni"].ToDate("dd/MM/yyyy");
                    DateTime ravAutomaticoDataFim = queryString["RavAutomaticoDataFim"].ToDate("dd/MM/yyyy");
                    Decimal ravAutomaticoValorMinimo = queryString["RavAutomaticoValorMinimo"] != null ?
                        Convert.ToDecimal(queryString["RavAutomaticoValorMinimo"]) : 30;
                    String bandeiras = queryString["RavAutomaticoBandeiras"];
                    String bandeirasQueryString = String.Empty;
                    if (DadosRavAutomatico != null)
                    {
                        if (ravAutomaticoDataIni == DateTime.MinValue)
                            ravAutomaticoDataIni = DadosRavAutomatico.DataVigenciaIni;
                        if (ravAutomaticoDataFim == DateTime.MinValue)
                            ravAutomaticoDataFim = DadosRavAutomatico.DataVigenciaFim;


                        if (String.IsNullOrWhiteSpace(bandeiras) && DadosRavAutomatico.Bandeiras != null && DadosRavAutomatico.Bandeiras.Count > 0)
                        {
                            bandeiras = String.Join(";", DadosRavAutomatico.Bandeiras.Select(bandeira =>
                                bandeira.CodBandeira + "#" + bandeira.DscBandeira + "#S").ToArray());

                            bandeirasQueryString = String.Join(";", DadosRavAutomatico.Bandeiras.Select(bandeira =>
                                bandeira.DscBandeira).ToArray());
                        }
                    }

                    using (ServicoPortalRAVClient cliente = new ServicoPortalRAVClient())
                    {

                        int verificador = cliente.EfetuarRAVAutomaticoPersonalizado(SessaoAtual.CodigoEntidade,
                            Convert.ToChar(queryString["RavAutomaticoTipoVenda"]),
                            Convert.ToChar(queryString["RavAutomaticoPeriodoRecebimento"]),
                            ravAutomaticoValorMinimo,
                            ravAutomaticoDataIni,
                            ravAutomaticoDataFim,
                            queryString["RavAutomaticoDiaSemana"],
                            queryString["RavAutomaticoDiaAntecipacao"],
                            bandeiras);

                        SharePointUlsLog.LogMensagem("Retorno serviço automatico:" + verificador.ToString());

                        if (verificador == 0)
                        {
                            queryString = new QueryStringSegura();
                            queryString["AcessoSenha"] = _validaSenha;
                            queryString["RavAutomaticoValorMinimo"] = lblValorMinimo.Text;
                            queryString["RavAutomaticoTipoVenda"] = lblTipoVenda.Text;
                            queryString["RavAutomaticoPeriodoRecebimento"] = lblRecebimento.Text;
                            queryString["RavAutomaticoTaxa"] = lblTaxa.Text;
                            queryString["RavAutomaticoDataIni"] = lblPeriodoAntecipacaoInicio.Text;
                            queryString["RavAutomaticoDataFim"] = lblPeriodoAntecipacaoFim.Text;
                            queryString["RavAutomaticoTipoAntecipacao"] = Session["RavAutomaticoTipoAntecipacaoTEXTO"] == null ? "" : Session["RavAutomaticoTipoAntecipacaoTEXTO"].ToString();

                            bandeiras = String.Empty;
                            foreach (RepeaterItem item in rptBandeiras.Items)
                            {
                                bandeiras += ((Label)item.FindControl("lblBandeira")).Text + ';';
                            }
                            queryString["RavAutomaticoBandeiras"] = bandeiras.Length > 0 ? bandeiras.Remove(bandeiras.Length - 1) : bandeirasQueryString;

                            //Registro no histórico/log de atividades
                            Historico.RealizacaoServico(SessaoAtual, "Contratação de Antecipação Automática");

                            Response.Redirect("pn_ComprovanteRavAutomatico.aspx?dados=" + queryString.ToString(), false);
                        }
                        else
                        {
                            Logger.GravarErro("Erro do verificador no Antecipação automática");
                            base.ExibirPainelExcecao(FONTE, verificador);
                            SharePointUlsLog.LogErro("Erro do verificador na Antecipação automática");
                        }
                    }
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex.Message);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO_CONFIRMAR);
                }
            }
        }

        /// <summary>
        /// Retorna a descrição do Tipo de Venda.
        /// </summary>
        /// <returns></returns>
        private string GetTipoVendaDescricao()
        {
            QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);

            switch (queryString["RavAutomaticoTipoVenda"].ToString())
            {
                case "A":
                    { return "Ambos"; }
                case "V":
                    { return "À vista"; }
                case "P":
                    { return "Parcelado"; }
                default:
                    { return null; }
            }
        }

        protected void rptBandeiras_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    Label bandeira = (Label)e.Item.FindControl("lblBandeira");
                    bandeira.Text = e.Item.DataItem.ToString().Split('#')[1];
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante bind de dados na tabela", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao("Redecard.PN.SharePoint", 300);
            }
        }
    }
}
