/*
(c) Copyright [ANO] Redecard S.A.
Autor : [Daniel]
Empresa : [BRQ IT Services]
Histórico:
    - [01/08/2012] – [Daniel] – [Etapa inicial]
    - [29/03/2017] - [Jacques - Iteris] - [Facelift da página de RAV Avulso]
*/

using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using Redecard.PN.RAV.Core.Web.Controles.Portal;
using Redecard.PN.RAV.Sharepoint.ModuloRAV;
using System;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.RAV.Sharepoint.WebParts.RavAvulsoAlteracao
{
    public partial class RavAvulsoAlteracaoUserControl : UserControlBase
    {
        #region [ Constantes ]

        private const string FONTE = "RavAvulsoAlteracaoUserControl.ascx";
        private const int COGIDO_ERRO_PAGELOAD = 3019;
        private const int CODIGO_ERRO_CADASTRO = 3020;
        private const int CODIGO_ERRO_ANTECIPACAO_MINIMA = 3021;
        private const int CODIGO_ERRO_ANTECIPACAO_MAXIMA = 3022;
        private const int CODIGO_ERRO_CADASTRAR = 3023;

        #endregion

        #region [ Atributos ]

        /// <summary>
        /// Identifica se deve validar a senha
        /// </summary>
        private string validaSenha = bool.FalseString;

        /// <summary>
        /// Identifica se deve exibir erro ao usuário
        /// </summary>
        private Boolean exibeErro = true;

        /// <summary>
        /// Identifica o tipo de venda em função das checkboxes de tipo de venda selecionadas
        /// </summary>
        private TipoVendaEnum TipoVenda
        {
            get
            {
                if (chkTipoVendaAvista.Checked && chkTipoVendaParcelado.Checked)
                    return TipoVendaEnum.Ambos;

                if (chkTipoVendaAvista.Checked)
                    return TipoVendaEnum.Avista;

                if (chkTipoVendaParcelado.Checked)
                    return TipoVendaEnum.Parcelado;

                return TipoVendaEnum.Nenhum;
            }
        }

        #endregion

        #region [ Eventos ]

        /// <summary>
        /// Evento de load da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.MaintainScrollPositionOnPostBack = true;

            using (Logger Log = Logger.IniciarLog("Alteração RAV Avulso - Page Load"))
            {
                try
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

                        validaSenha = queryString["AcessoSenha"];

                        if (!Page.IsPostBack)
                        {
                            // O usuario do tipo atendimento tem permissao apenas para visualizar a pagina
                            if (SessaoAtual != null && SessaoAtual.UsuarioAtendimento)
                                btnCadastrar.Visible = false;

                            //Tratativa para quando o usuario vem de uma pagina Angular
                            if (queryString["Origem"] != null && queryString["Origem"].ToLower().CompareTo("angular") == 0)
                            {
                                VerificarRAVAvulso();
                            }
                            else
                            {
                                if (Session["DadosRAVAvulso"] == null)
                                {
                                    queryString = new QueryStringSegura();
                                    queryString["AcessoSenha"] = validaSenha;
                                    Response.Redirect(string.Format("pn_Principal.aspx?dados={0}", queryString.ToString()), false);
                                    return;
                                }
                            }

                            //using (ServicoPortalRAVClient cliente = new ServicoPortalRAVClient())
                            //{
                            //ModRAVAvulsoEntrada entrada = new ModRAVAvulsoEntrada();

                            AtualizaValores();

                            //if (!String.IsNullOrEmpty(queryString["ValorRavAvulsoSolicitado"]))
                            //{
                            //    txtValorRAV.Text = queryString["ValorRavAvulsoSolicitado"];
                            //}
                            if (Session["ValorRavAvulsoSolicitado"] != null)
                            {
                                txtValorRAV.Text = Session["ValorRavAvulsoSolicitado"].ToString();
                                lblValorAntecipadoPermitido.Text = Session["LabelValorRavAvulsoAlteracao"].ToString();
                            }

                            if (queryString["TipoRavAvulsoAlteracao"] != null)
                            {
                                chkTipoVendaAvista.Checked = false;
                                chkTipoVendaParcelado.Checked = false;

                                switch (queryString["TipoRavAvulsoAlteracao"])
                                {
                                    case "A":
                                        chkTipoVendaAvista.Checked = true;
                                        chkTipoVendaParcelado.Checked = true;
                                        break;
                                    case "V":
                                        chkTipoVendaAvista.Checked = true;
                                        break;
                                    case "P":
                                        chkTipoVendaParcelado.Checked = true;
                                        break;
                                    default:
                                        chkTipoVendaAvista.Checked = false;
                                        chkTipoVendaParcelado.Checked = false;
                                        break;
                                }
                            }
                            //}
                        }
                    }
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirPainelExcecao(FONTE, COGIDO_ERRO_PAGELOAD);
                    SharePointUlsLog.LogErro(ex);
                }
            }
        }

        /// <summary>
        /// Valida e redireciona o usuário para a página de confirmação de RAV Avulso.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Cadastrar(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            using (Logger Log = Logger.IniciarLog("Alteração RAV Avulso - Cadastrar"))
            {
                QueryStringSegura queryString = new QueryStringSegura();
                decimal valorAntecipacao = -1;
                try
                {
                    valorAntecipacao = Convert.ToDecimal(txtValorRAV.Text.Replace(".", ""));
                    queryString["ValorRavAvulsoSolicitado"] = valorAntecipacao.ToString();
                    queryString["ValorRavAvulsoEfetivacao"] = valorAntecipacao.ToString();
                }
                catch (FormatException ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirPainelExcecao(FONTE, CODIGO_ERRO_CADASTRO);
                    return;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirPainelExcecao(FONTE, CODIGO_ERRO_CADASTRO);
                    return;
                }

                try
                {
                    using (ServicoPortalRAVClient cliente = new ServicoPortalRAVClient())
                    {
                        ModRAVAvulsoEntrada entrada = new ModRAVAvulsoEntrada();
                        ModRAVAvulsoSaida saida = (ModRAVAvulsoSaida)Session["DadosRAVAvulso"];

                        if (!ReferenceEquals(saida.DadosAntecipacao, null))
                            queryString["RavAvulsoTipoAntecipacao"] = saida.DadosAntecipacao.NomeProdutoAntecipacao;

                        switch (this.TipoVenda)
                        {
                            case TipoVendaEnum.Ambos:
                                if (saida.ValorDisponivel < valorAntecipacao)
                                {
                                    this.ExibirMensagem("Validação", "Não existe saldo suficiente");
                                    txtValorRAV.Text = saida.ValorDisponivel.ToString("0,0.00");
                                    return;
                                }
                                break;
                            case TipoVendaEnum.Avista:
                                if (saida.DadosParaCredito.Count > 0)
                                {
                                    if (saida.DadosParaCredito[0].ValorRotativo > 0)
                                    {
                                        if (saida.DadosParaCredito[0].ValorRotativo < valorAntecipacao)
                                        {

                                            this.ExibirMensagem("Validação", "Não existe saldo suficiente");
                                            txtValorRAV.Text = saida.DadosParaCredito[0].ValorRotativo.ToString("0,0.00");
                                            return;
                                        }
                                    }
                                    else if (saida.DadosParaCredito[1].ValorRotativo > 0)
                                    {
                                        if (saida.DadosParaCredito[1].ValorRotativo < valorAntecipacao)
                                        {
                                            this.ExibirMensagem("Validação", "Não existe saldo suficiente");
                                            txtValorRAV.Text = saida.DadosParaCredito[1].ValorRotativo.ToString("0,0.00");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        this.ExibirMensagem("Validação", "Não existe saldo suficiente");
                                        return;
                                    }
                                }
                                else
                                {
                                    this.ExibirMensagem("Validação", "Não existe saldo suficiente");
                                    return;
                                }
                                break;
                            case TipoVendaEnum.Parcelado:
                                if (saida.DadosParaCredito.Count > 0)
                                {
                                    if (saida.DadosParaCredito[0].ValorParcelado > 0)
                                    {
                                        if (saida.DadosParaCredito[0].ValorParcelado < valorAntecipacao)
                                        {
                                            this.ExibirMensagem("Validação", "Não existe saldo suficiente");
                                            txtValorRAV.Text = saida.DadosParaCredito[0].ValorParcelado.ToString("0,0.00");
                                            return;
                                        }
                                    }
                                    else if (saida.DadosParaCredito[1].ValorParcelado > 0)
                                    {
                                        if (saida.DadosParaCredito[1].ValorParcelado < valorAntecipacao)
                                        {
                                            this.ExibirMensagem("Validação", "Não existe saldo suficiente");
                                            txtValorRAV.Text = saida.DadosParaCredito[1].ValorParcelado.ToString("0,0.00");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        this.ExibirMensagem("Validação", "Não existe saldo suficiente");
                                        return;
                                    }
                                }
                                else
                                {
                                    this.ExibirMensagem("Validação", "Não existe saldo suficiente");
                                    return;
                                }
                                break;
                            case TipoVendaEnum.Nenhum:
                            default:
                                this.ExibirMensagem("Validação", "Tipo de venda não selecionado");
                                return;
                        }

                        if (valorAntecipacao > Convert.ToDecimal(lblValorAntecipadoPermitido.Text))
                        {
                            this.ExibirMensagem("Validação", "Não existe saldo suficiente");
                            AtualizaValores();
                            return;
                        }

                        queryString["TipoRavAvulsoAlteracao"] = this.GetTipoVendaQueryString();

                        // validação para alterar o VALOR PARA LIQUIDO
                        if (saida.DadosParaCredito[0].ValorLiquido > 0 &&
                            valorAntecipacao > saida.DadosParaCredito[0].ValorLiquido)
                        {
                            valorAntecipacao = saida.DadosParaCredito[0].ValorLiquido;
                        }
                        else if (
                            saida.DadosParaCredito[1].ValorLiquido > 0 &&
                            valorAntecipacao > saida.DadosParaCredito[1].ValorLiquido)
                        {
                            valorAntecipacao = saida.DadosParaCredito[1].ValorLiquido;
                        }

                        queryString["ValorRavAvulsoEfetivacao"] = valorAntecipacao.ToString();

                        // repassa o valor antecipado permitido para apresentação na tela de confirmação
                        queryString["ValorAntecipadoPermitido"] = hdnValorAntecipadoPermitido.Value;

                        queryString["AcessoSenha"] = validaSenha;
                        Response.Clear();
                        Response.Redirect(string.Format("pn_ConfirmacaoRavAvulso.aspx?dados={0}", queryString.ToString()), false);
                    }
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirPainelExcecao(FONTE, CODIGO_ERRO_CADASTRO);
                    SharePointUlsLog.LogErro(ex);
                }
            }
        }

        /// <summary>
        /// Redireciona o usuário para a página principal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Voltar(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Alteração RAV Avulso - Voltar"))
            {
                try
                {
                    QueryStringSegura queryString = new QueryStringSegura();

                    queryString["TipoRavAvulsoAlteracao"] = this.GetTipoVendaQueryString();
                    queryString["AcessoSenha"] = bool.TrueString;

                    if (!String.IsNullOrEmpty(txtValorRAV.Text))
                        queryString["ValorRavAvulsoSolicitado"] = txtValorRAV.Text;

                    Response.Redirect(string.Format("pn_Principal.aspx?dados={0}", queryString.ToString()), false);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    SharePointUlsLog.LogErro(ex);
                }
            }
        }

        /// <summary>
        /// Validação em servidor para o controle de tipo de venda
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void TipoVendaCustomServerValidate(Object source, CustomServerValidateEventArgs args)
        {
            args.IsValid = chkTipoVendaAvista.Checked || chkTipoVendaParcelado.Checked;
            args.ErrorMessage = crvTipoVenda.ErrorMessageRequired;
        }

        #endregion

        #region [ Métodos auxiliares ]

        /// <summary>
        /// Atualiza os valores da tela pela opção selecionada.
        /// </summary>
        private void AtualizaValores()
        {
            ModRAVAvulsoSaida saida = (ModRAVAvulsoSaida)Session["DadosRAVAvulso"];

            switch (this.TipoVenda)
            {
                case TipoVendaEnum.Ambos:
                    txtValorRAV.Text = saida.ValorDisponivel.ToString("0,0.00");
                    lblValorAntecipadoPermitido.Text = saida.ValorDisponivel.ToString("0,0.00");
                    Session["LabelValorRavAvulsoAlteracao"] = lblValorAntecipadoPermitido.Text;
                    break;
                case TipoVendaEnum.Avista:
                    if (saida.DadosParaCredito.Count > 0)
                    {
                        if (saida.DadosParaCredito[0].ValorRotativo > 0)
                        {
                            txtValorRAV.Text = saida.DadosParaCredito[0].ValorRotativo.ToString("0,0.00");
                            lblValorAntecipadoPermitido.Text = saida.DadosParaCredito[0].ValorRotativo.ToString("0,0.00");
                            Session["LabelValorRavAvulsoAlteracao"] = lblValorAntecipadoPermitido.Text;
                        }
                        else
                        {
                            txtValorRAV.Text = saida.DadosParaCredito[1].ValorRotativo.ToString("0,0.00");
                            lblValorAntecipadoPermitido.Text = saida.DadosParaCredito[1].ValorRotativo.ToString("0,0.00");
                            Session["LabelValorRavAvulsoAlteracao"] = lblValorAntecipadoPermitido.Text;
                        }
                    }
                    break;
                case TipoVendaEnum.Parcelado:
                    if (saida.DadosParaCredito.Count > 0)
                    {
                        if (saida.DadosParaCredito[0].ValorParcelado > 0)
                        {
                            txtValorRAV.Text = saida.DadosParaCredito[0].ValorParcelado.ToString("0,0.00");
                            lblValorAntecipadoPermitido.Text = saida.DadosParaCredito[0].ValorParcelado.ToString("0,0.00");
                            Session["LabelValorRavAvulsoAlteracao"] = lblValorAntecipadoPermitido.Text;
                        }
                        else
                        {
                            txtValorRAV.Text = saida.DadosParaCredito[1].ValorParcelado.ToString("0,0.00");
                            lblValorAntecipadoPermitido.Text = saida.DadosParaCredito[1].ValorParcelado.ToString("0,0.00");
                            Session["LabelValorRavAvulsoAlteracao"] = lblValorAntecipadoPermitido.Text;
                        }
                    }
                    break;
                case TipoVendaEnum.Nenhum:
                default:
                    txtValorRAV.Text = 0.ToString("0,0.00");
                    lblValorAntecipadoPermitido.Text = 0.ToString("0,0.00");
                    Session["LabelValorRavAvulsoAlteracao"] = lblValorAntecipadoPermitido.Text;
                    break;
            }

            // replica o valor para o campo oculto
            hdnValorAntecipadoPermitido.Value = lblValorAntecipadoPermitido.Text;

            if (!ReferenceEquals(saida.DadosAntecipacao, null))
                ltrTipoAntecipacao.Text = RAVComum.RetornaAliasProdutoAntecipacao(saida.DadosAntecipacao.NomeProdutoAntecipacao);

            hldValorDisponivel.Value = saida.ValorDisponivel.ToString("0,0.00");

            hldValorAVista.Value = String.Format("0,0.00", "0");

            if (saida.DadosParaCredito.Count > 0)
            {
                if (saida.DadosParaCredito[0].ValorRotativo > 0)
                {
                    hldValorAVista.Value = saida.DadosParaCredito[0].ValorRotativo.ToString("0,0.00");
                }
                else
                {
                    hldValorAVista.Value = saida.DadosParaCredito[1].ValorRotativo.ToString("0,0.00");
                }
            }

            hldValorParcelado.Value = String.Format("0,0.00", "0");

            if (saida.DadosParaCredito.Count > 0)
            {
                if (saida.DadosParaCredito[0].ValorParcelado > 0)
                {
                    hldValorParcelado.Value = saida.DadosParaCredito[0].ValorParcelado.ToString("0,0.00");
                }
                else
                {
                    hldValorParcelado.Value = saida.DadosParaCredito[1].ValorParcelado.ToString("0,0.00");
                }
            }
        }

        /// <summary>
        /// Método para exibir painel de informações
        /// </summary>
        /// <param name="titulo">Título da mensagem</param>
        /// <param name="mensagem">Conteúdo da mensagem</param>
        private void ExibirMensagem(string titulo, string mensagem)
        {
            String scriptKey = "RavAvulsoAlteracaoUserControl.ExibirMensagem";
            String script = String.Format("BusinessValidator.ShowPopup('{0}', '{1}');", titulo, mensagem);

            Page.ClientScript.RegisterStartupScript(Page.GetType(), scriptKey, script, true);
        }

        /// <summary>
        /// Exibe o painel de exceção
        /// </summary>
        /// <param name="fonteErro"></param>
        /// <param name="codigoErro"></param>
        private void ExibirPainelExcecao(String fonteErro, Int32 codigoErro)
        {
            if (this.exibeErro)
            {
                this.exibeErro = false;
                this.ExibirPainelExcecao(fonteErro, codigoErro);
            }
        }

        /// <summary>
        /// Obtém a letra correspondente ao tipo de venda definido a ser usada na QueryString
        /// </summary>
        /// <returns>Letra para o tipo de venda</returns>
        private String GetTipoVendaQueryString()
        {
            switch (this.TipoVenda)
            {
                case TipoVendaEnum.Ambos:
                    return "A";
                case TipoVendaEnum.Avista:
                    return "V";
                case TipoVendaEnum.Parcelado:
                    return "P";
                case TipoVendaEnum.Nenhum:
                default:
                    return String.Empty;
            }
        }

        #endregion

        /// <summary>
        /// Enumerator para identificação do tipo de venda
        /// </summary>
        private enum TipoVendaEnum
        {
            Nenhum,
            Avista,
            Parcelado,
            Ambos
        }



        /// <summary>
        /// Verifica o RAV Avulso e preenche na tela
        /// </summary>
        private void VerificarRAVAvulso()
        {
            #region RAV Avulso
            using (Logger log = Logger.IniciarLog("Verificando RAV Avulso - Front"))
            {
                using (ServicoPortalRAVClient servicoPortal = new ServicoPortalRAVClient())
                {
                    ModRAVAvulsoSaida ravAvulso = servicoPortal.VerificarRAVDisponivel(SessaoAtual.CodigoEntidade);

                    if (ravAvulso != null && ravAvulso.Retorno == -1)
                    {
                        ExibirMensagem("Acesso não permitido.", ravAvulso.MsgErro);
                        return;
                    }

                    if (ravAvulso != null & ravAvulso.Retorno > 70000)
                    {
                        this.ExibirPainelExcecao(FONTE, ravAvulso.Retorno);
                        return;
                    }

                    Session["DadosRAVAvulso"] = ravAvulso;

                    if (ravAvulso.ValorDisponivel >= 50)
                    {
                        string valorRavAvulso = ravAvulso.ValorDisponivel.ToString("0,0.00");
                        Session["ValorRavAvulsoSolicitado"] = valorRavAvulso;
                    }
                }
            }
            #endregion
        }
    }
}
