/*
(c) Copyright [ANO] Redecard S.A.
Autor : [Daniel]
Empresa : [BRQ IT Services]
Histórico:
- [01/08/2012] – [Daniel] – [Etapa inicial]
*/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.ServiceModel;
using Redecard.PN.Comum;
using Redecard.PN.RAV.Sharepoint.ModuloRAV;
using Microsoft.SharePoint.Utilities;
using System.Web;
using Redecard.PN.RAV.Core.Web.Controles.Portal;

//using Redecard.PN.RAV.Sharepoint.ModuloRAV;

namespace Redecard.PN.RAV.Sharepoint.WebParts.RavAutomaticoPersonalizacao
{
    public partial class RavAutomaticoPersonalizacaoUserControl : UserControlBase
    {
        #region Constantes
        private const string FONTE = "RavAutomaticoPersonalizacaoUserControl.ascx";
        private const int CODIGO_ERRO_LOAD = 3016;
        private const int CODIGO_ERRO_CONTINUAR = 3017;
        private const int CODIGO_ERRO_CONTINUAR_STATUS = 3018;
        //private const int COGIDO_ERRO_RAVAUTO = 3005;

        #endregion

        #region Atributos
        private string _validaSenha = bool.FalseString;
        private string _ravAutotipoVenda = bool.FalseString;
        private string _RavAutoperiodicidade = bool.FalseString;
        private string _RavAutovalorMinimo = bool.FalseString;
        private string _tipoVenda = "";
        private string _periodoVenda = "";
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Personalização RAV Automático - Page Load"))
            {
                if (Request.QueryString["dados"] != null)
                {
                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);

                    if (string.IsNullOrEmpty(queryString["AcessoSenha"]) 
                        || queryString["AcessoSenha"].CompareTo(bool.TrueString) != 0)
                    {
                        Response.Redirect(String.Format("pn_rav.aspx?dados={0}", queryString.ToString()), false);
                        return;
                    }

                    SharePointUlsLog.LogMensagem(queryString["AcessoSenha"]);
                    Log.GravarMensagem(queryString["AcessoSenha"]);

                    _validaSenha = queryString["AcessoSenha"];

                    try
                    {
                        using (ServicoPortalRAVClient servicoPortal = new ServicoPortalRAVClient())
                        {
                            if (!Page.IsPostBack)
                            {
                                // O usuario do tipo atendimento tem permissao apenas para visualizar a pagina
                                if (SessaoAtual != null && SessaoAtual.UsuarioAtendimento)
                                {
                                    btnContinuar.Visible = false;
                                }

                                if (queryString["RavAutomaticoTipoVenda"] != null)
                                {
                                    _tipoVenda = queryString["RavAutomaticoTipoVenda"].ToString();

                                    if (_tipoVenda == "A")
                                    {
                                        chkTipoVenda_Parcelado.Checked = true;
                                        chkTipoVenda_Avista.Checked = true;
                                    }
                                    else if (_tipoVenda == "P")
                                    {
                                        chkTipoVenda_Parcelado.Checked = true;
                                    }
                                    else
                                    {
                                        chkTipoVenda_Avista.Checked = true;
                                    }
                                }

                                if (queryString["RavAutomaticoPeriodoRecebimento"] != null)
                                {
                                    _periodoVenda = queryString["RavAutomaticoPeriodoRecebimento"].ToString();

                                    if (_periodoVenda == "M")
                                    {
                                        rbtPeriodoMensal.Checked = true;
                                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "oninit", "showhide('mensal');", true);
                                    }
                                    else if (_periodoVenda == "S")
                                    {
                                        rbtPeriodoSemanal.Checked = true;
                                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "oninit", "showhide('semanal');", true);
                                    }
                                    else if (_periodoVenda == "Q")
                                    {
                                        rbtPeriodoQuinzenal.Checked = true;
                                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "oninit", "showhide('quinzenal');", true);

                                    }
                                    else
                                    {
                                        rbtPeriodoDiario.Checked = true;
                                    }
                                }

                                //ServicoPortalRAVClient cliente = new ServicoPortalRAVClient();
                                ModRAVAutomatico ravAutomatico = servicoPortal.ConsultarRAVAutomatico(SessaoAtual.CodigoEntidade, Convert.ToChar(_tipoVenda), Convert.ToChar(_periodoVenda));

                                Session["RavAutomaticoTipoAntecipacaoTEXTO"] = ravAutomatico.DadosRetorno.NomeProdutoAntecipacao;
                                lblTipoAntecipacao.Text = RAVComum.RetornaAliasProdutoAntecipacao(ravAutomatico.DadosRetorno.NomeProdutoAntecipacao);

                                if (queryString["RavAutomaticoValorMinimo"] != null
                                        && !string.IsNullOrEmpty(queryString["RavAutomaticoValorMinimo"]))
                                {
                                    txtValorMinimo.Text = queryString["RavAutomaticoValorMinimo"];
                                    hdnValorMinimoInicial.Value = queryString["RavAutomaticoValorMinimo"];
                                    //rbtPeriodoSemanal.Checked = true;
                                    //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "oninit", "showhide('semanal');", true);
                                }
                                else
                                {
                                    txtValorMinimo.Text = ravAutomatico.ValorMinimo.ToString();
                                    hdnValorMinimoInicial.Value = ravAutomatico.ValorMinimo.ToString();
                                }

                                if (queryString["RavAutomaticoDataIni"] != null
                                        && !string.IsNullOrEmpty(queryString["RavAutomaticoDataIni"]))
                                {
                                    txtDtIniDatePicker.Text = queryString["RavAutomaticoDataIni"].ToString();
                                }
                                else
                                {
                                    //txtDtIniDatePicker.Text = ravAutomatico.DataVigenciaIni.ToShortDateString();
                                }

                                if (queryString["RavAutomaticoDataFim"] != null
                                        && !string.IsNullOrEmpty(queryString["RavAutomaticoDataFim"]))
                                {
                                    txtDtFimDatePicker.Text = queryString["RavAutomaticoDataFim"].ToString();
                                }
                                else
                                {
                                    //txtDtFimDatePicker.Text = ravAutomatico.DataVigenciaFim.ToShortDateString();
                                }

                                if (!Object.ReferenceEquals(queryString["RavAutomaticoDiaAntecipacao"], null) &&
                                        !String.IsNullOrEmpty(queryString["RavAutomaticoDiaAntecipacao"]))
                                {
                                    String diaAntecipacao = queryString["RavAutomaticoDiaAntecipacao"].ToString();

                                    if (queryString["RavAutomaticoPeriodoRecebimento"].ToString().Equals("M"))
                                    {
                                        Int16 dia = 0;
                                        foreach (Char posicao in diaAntecipacao)
                                        {
                                            dia++;
                                            if (posicao == 'X')
                                                txtDiaMensal.Text = dia.ToString();
                                        }
                                    }
                                    else if (queryString["RavAutomaticoPeriodoRecebimento"].ToString().Equals("Q"))
                                    {
                                        Int16 dia = 0;
                                        foreach (Char posicao in diaAntecipacao)
                                        {
                                            dia++;
                                            if (posicao == 'X')
                                            {
                                                txtDiaQuinzenalInicio.Text = dia.ToString();
                                                txtDiaQuinzenalFinal.Text = (dia + 15).ToString();
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (!Object.ReferenceEquals(queryString["RavAutomaticoDiaSemana"], null)
                                        && !String.IsNullOrEmpty(queryString["RavAutomaticoDiaSemana"]))
                                {
                                    ddlDiasSemana.SelectedValue = queryString["RavAutomaticoDiaSemana"].ToString();
                                }

                                //Verificar se o usuário está cadastrado no Flex
                                //pnlRecebimentoDiferenciado.Visible = true; //Sempre false por enquanto
                                pnlCampos.Visible = true;

                                if (!object.ReferenceEquals(queryString["RavAutomaticoBandeiras"], null)
                                        && !string.IsNullOrEmpty(queryString["RavAutomaticoBandeiras"]))
                                {
                                    String[] bandeiras = queryString["RavAutomaticoBandeiras"].ToString().Split(';');
                                    if (bandeiras.Length > 0)
                                    {
                                        hdnQuantBandeiras.Value = bandeiras.Length.ToString();
                                        foreach (string bandeira in bandeiras)
                                        {
                                            if (bandeira.Split('#')[1] != "0")
                                            {
                                                ListItem listItem = new ListItem(bandeira.Split('#')[1], bandeira.Split('#')[0] + "#" + bandeira.Split('#')[1]);
                                                listItem.Attributes.Add("class", "rede-input");
                                                listItem.Selected = bandeira.Split('#')[2] == "S" ? true : false;
                                                chkListBandeiras.Items.Add(listItem);
                                            }

                                        }
                                    }
                                }
                                else if (ravAutomatico != null)
                                {
                                    //if (ravAutomatico.DadosRetorno.CodRetorno == 0 && ravAutomatico.Bandeiras.Count > 0)
                                    if (ravAutomatico.Bandeiras.Count > 0)
                                    {
                                        hdnQuantBandeiras.Value = ravAutomatico.Bandeiras.Count.ToString();
                                        foreach (ModRAVAutomaticoBandeira bandeira in ravAutomatico.Bandeiras)
                                        {
                                            if (bandeira.CodBandeira != 0)
                                            {
                                                ListItem listItem = new ListItem(bandeira.DscBandeira, bandeira.CodBandeira + "#" + bandeira.DscBandeira);
                                                listItem.Attributes.Add("class", "rede-input");
                                                listItem.Selected = bandeira.IndSel == "S" ? true : false;
                                                chkListBandeiras.Items.Add(listItem);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (FaultException<ModuloRAV.ServicoRAVException> ex)
                    {
                        Log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                        SharePointUlsLog.LogErro(ex);
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
        /// <summary>
        /// Redireciona o usuário para a página Principal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Voltar(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Personalização RAV Automático - Voltar"))
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
        /// Continua com o cadastro do RAV Automático.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Continuar(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Personalização RAV Automático - Continuar"))
            {
                QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                _tipoVenda = queryString["RavAutomaticoTipoVenda"].ToString();
                _periodoVenda = queryString["RavAutomaticoPeriodoRecebimento"].ToString();
                _validaSenha = queryString["AcessoSenha"];

                queryString = new QueryStringSegura();
                bool status = true;
                int quinzenaDiaInicio = 0;

                ModRAVAutomatico automatico = null;

                try
                {
                    using (ServicoPortalRAVClient cliente = new ServicoPortalRAVClient())
                    {
                        //if (queryString["RavAutomaticoTipoVenda"] != null && queryString["RavAutomaticoPeriodoRecebimento"] != null)
                        //{
                        automatico = cliente.ConsultarRAVAutomatico(SessaoAtual.CodigoEntidade, Convert.ToChar(_tipoVenda), Convert.ToChar(_periodoVenda));
                        //}
                        if (automatico != null)
                        {
                            Decimal valorMinimo = 0;
                            Decimal.TryParse(txtValorMinimo.Text, out valorMinimo);
                            if (valorMinimo < 30)
                            {
                                status = false;
                                //base.Alert("Valor mínimo não pode ser menor do que R$ 30,00.", false);
                            }
                            //else if (Convert.ToDecimal(txtValorMinimo.Text) > automatico.ValorMinimo)
                            //{
                            //    status = false;
                            //    base.Alert("Não existe saldo suficiente.", false);

                            //}
                        }

                    }
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    SharePointUlsLog.LogErro(ex);
                    return;
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro em ConsultarRAVAutomatico()", ex);
                    status = false;
                    //base.Alert("Valor inválido.", false);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO_CONTINUAR);
                    return;
                }

                if (Convert.ToDateTime(txtDtIniDatePicker.Text) > Convert.ToDateTime(txtDtFimDatePicker.Text))
                {
                    status = false;
                    crvDataFinal.ErrorMessage = "data de vigência inválida";
                    Logger.GravarErro("Data de vigência inválida.");
                    base.ExibirPainelExcecao(FONTE, 3016);
                    return;
                }

                if (status)
                {
                    try
                    {
                        quinzenaDiaInicio = txtDiaQuinzenalInicio.Text != "" ? Convert.ToInt32(txtDiaQuinzenalInicio.Text) : 0;
                    }
                    catch (Exception ex)
                    {
                        status = false;
                        Logger.GravarErro("Dia escolhido inválido.", ex);
                        //base.Alert("Dia escolhido inválido.", false);
                        SharePointUlsLog.LogErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO_CONTINUAR_STATUS);
                        return;
                    }

                    try
                    {
                        //string dataInicio = txtAnoInicio.Text + "-" + txtMesInicio.Text + "-" + txtDiaInicio.Text;
                        //string dataFinal = txtAnoFinal.Text + "-" + txtMesFinal.Text + "-" + txtDiaFinal.Text;

                        //ravEntrada.DataVigenciaIni = Convert.ToDateTime(dataInicio);
                        //ravEntrada.DataVigenciaFim = Convert.ToDateTime(dataFinal);

                        Convert.ToDateTime(txtDtIniDatePicker.Text);
                        Convert.ToDateTime(txtDtFimDatePicker.Text);
                    }
                    catch (Exception ex)
                    {
                        status = false;
                        Logger.GravarErro("Data selecionada inválida.", ex);
                        //base.Alert("Data selecionada inválida.", false);
                        base.ExibirPainelExcecao(FONTE, 3016);
                        SharePointUlsLog.LogErro(ex);
                        return;
                    }

                    try
                    {
                        char periodoRecebimento = ' ';
                        if (rbtPeriodoDiario.Checked == true)
                        {
                            //ravEntrada.Periodicidade = EPeriodicidade.Diario;
                            periodoRecebimento = 'D';
                        }
                        else if (rbtPeriodoSemanal.Checked == true)
                        {
                            //ravEntrada.Periodicidade = EPeriodicidade.Semanal;
                            periodoRecebimento = 'S';
                        }
                        else if (rbtPeriodoQuinzenal.Checked == true)
                        {
                            //ravEntrada.Periodicidade = EPeriodicidade.Quinzenal;
                            periodoRecebimento = 'Q';
                        }
                        else if (rbtPeriodoMensal.Checked == true)
                        {
                            //ravEntrada.Periodicidade = EPeriodicidade.Mensal;
                            periodoRecebimento = 'M';
                        }
                        queryString["RavAutomaticoPeriodoRecebimento"] = periodoRecebimento.ToString();

                        switch (ddlDiasSemana.SelectedValue)
                        {
                            case "SEG":
                                {
                                    //ravEntrada.DiaSemana = EDiaSemana.Segunda;
                                    if (periodoRecebimento == 'S')
                                        queryString["RavAutomaticoDiaSemana"] = "SEG";
                                    break;
                                }
                            case "TER":
                                {
                                    //ravEntrada.DiaSemana = EDiaSemana.Terca;
                                    if (periodoRecebimento == 'S')
                                        queryString["RavAutomaticoDiaSemana"] = "TER";
                                    break;
                                }
                            case "QUA":
                                {
                                    //ravEntrada.DiaSemana = EDiaSemana.Quarta;
                                    if (periodoRecebimento == 'S')
                                        queryString["RavAutomaticoDiaSemana"] = "QUA";
                                    break;
                                }
                            case "QUI":
                                {
                                    //ravEntrada.DiaSemana = EDiaSemana.Quinta;
                                    if (periodoRecebimento == 'S')
                                        queryString["RavAutomaticoDiaSemana"] = "QUI";
                                    break;
                                }
                            case "SEX":
                                {
                                    //ravEntrada.DiaSemana = EDiaSemana.Sexta;
                                    if (periodoRecebimento == 'S')
                                        queryString["RavAutomaticoDiaSemana"] = "SEX";
                                    break;
                                }
                        }

                        string quinzenaDiaAntecipacao = null;
                        for (int i = 0; i < 30; i++)
                        {
                            quinzenaDiaAntecipacao += (quinzenaDiaInicio == i + 1 || quinzenaDiaInicio + 15 == i + 1) ? "X" : " ";
                        }

                        string mesDiaAntecipacao = String.Empty;
                        for (int i = 0; i < 30; i++)
                        {
                            mesDiaAntecipacao += (txtDiaMensal.Text.ToInt16() == i + 1) ? "X" : " ";
                        }

                        queryString["RavAutomaticoDiaAntecipacao"] = String.Empty;
                        if (periodoRecebimento == 'Q')
                            queryString["RavAutomaticoDiaAntecipacao"] = quinzenaDiaAntecipacao;
                        else if (periodoRecebimento == 'M')
                            queryString["RavAutomaticoDiaAntecipacao"] = mesDiaAntecipacao;

                        string tipoVenda = "";

                        if (chkTipoVenda_Avista.Checked && chkTipoVenda_Parcelado.Checked)
                        { tipoVenda = "A"; }
                        else if (chkTipoVenda_Avista.Checked)
                        { tipoVenda = "R"; }
                        else if (chkTipoVenda_Parcelado.Checked)
                        { tipoVenda = "P"; }

                        queryString["RavAutomaticoTipoVenda"] = tipoVenda.ToString();

                        String bandeiras = String.Empty;
                        foreach (ListItem item in chkListBandeiras.Items)
                        {
                            if (item.Selected)
                                bandeiras += item.Value + "#S;";
                            else
                                bandeiras += item.Value + "#N;";
                        }

                        //Session["DadosRavAutomatico"] = ravEntrada;
                        queryString["RavAutomaticoValorMinimo"] = txtValorMinimo.Text;
                        queryString["RavAutomaticoDataIni"] = txtDtIniDatePicker.Text;
                        queryString["RavAutomaticoDataFim"] = txtDtFimDatePicker.Text;
                        queryString["RavAutomaticoBandeiras"] = bandeiras.Length > 0 ? bandeiras.Remove(bandeiras.Length - 1) : String.Empty;
                        queryString["AcessoSenha"] = _validaSenha;
                        queryString["RavAutomaticoTipoAntecipacao"] = Session["RavAutomaticoTipoAntecipacaoTEXTO"] == null ? "" : Session["RavAutomaticoTipoAntecipacaoTEXTO"].ToString();

                        Response.Redirect(string.Format("pn_ConfirmacaoRavAutomatico.aspx?dados={0}", queryString.ToString()), false);
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex.Message);
                        base.ExibirPainelExcecao("Redecard.PN.SharePoint", CODIGO_ERRO);
                        return;
                    }
                }
                else
                {
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO_CONTINUAR_STATUS);
                }
            }
        }

        /// <summary>
        /// Retorno o valor selecionado no datepicker para o campo de dia inicial.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PreecherValorDia(object sender, EventArgs e)
        {
            Logger.GravarLog("Personalização RAV Automático");
        }

        /// <summary>
        /// Método para exibir painel de informações
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="mensagem"></param>
        private void ExibirMensagem(string titulo, string mensagem)
        {
            Panel[] paineisDados = new Panel[1]{
                            pnlDadosGerais
                    };

            pnlDadosGerais.Visible = false;
            QueryStringSegura queryString = new QueryStringSegura();
            queryString["AcessoSenha"] = bool.TrueString;
            base.ExibirPainelConfirmacaoAcao(titulo, mensagem, string.Format("pn_PersonalizacaoRavAutomatico.aspx?dados={0}", queryString.ToString()), paineisDados);
        }

        protected void TipoVendaCustomServerValidate(Object source, CustomServerValidateEventArgs args)
        {
            args.IsValid = chkTipoVenda_Avista.Checked || chkTipoVenda_Parcelado.Checked;
            args.ErrorMessage = crvTipoVenda.ErrorMessageRequired;
        }


        internal static ServicoPortalRAVClient GetWebServiceInstance()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.SendTimeout = TimeSpan.FromMinutes(1);
            binding.OpenTimeout = TimeSpan.FromMinutes(1);
            binding.CloseTimeout = TimeSpan.FromMinutes(1);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            binding.AllowCookies = false;
            binding.BypassProxyOnLocal = false;
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.MessageEncoding = WSMessageEncoding.Text;
            binding.TextEncoding = System.Text.Encoding.UTF8;
            binding.TransferMode = TransferMode.Buffered;
            binding.UseDefaultWebProxy = true;
            return new ServicoPortalRAVClient(binding, new EndpointAddress("http://localhost:36651/HIServiceMA_RAV.svc"));
        }
    }
}
