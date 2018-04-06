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
using Redecard.PN.Comum;
using Redecard.PN.RAV.Sharepoint.ModuloRAV;
using System.ServiceModel;

namespace Redecard.PN.RAV.Sharepoint.WebParts.ConfirmacaoRavAvulso
{
    public partial class ConfirmacaoRavAvulsoUserControl : UserControlBase
    {
        #region Constantes
        public const string FONTE = "ConfirmacaoRavAvulsoUserControl.ascx";
        public const int CODIGO_ERRO_LOAD = 3002;
        public const int CODIGO_ERRO_CONFIRMAR = 3002;
        #endregion

        #region Atributos
        private string validaSenha = bool.FalseString;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Confirmação RAV Avulso - Page Load"))
            {
                try
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

                        validaSenha = queryString["AcessoSenha"];

                        lblValorAntecipadoPermitido.Text = queryString["ValorAntecipadoPermitido"];

                        if (Session["DadosRavAvulso"] == null)
                        {
                            queryString = new QueryStringSegura();
                            queryString["AcessoSenha"] = validaSenha;
                            Response.Redirect(string.Format("pn_Principal.aspx?dados={0}", queryString.ToString()), false);
                            return;
                        }

                        if (!Page.IsPostBack)
                        {
                            // O usuario do tipo atendimento tem permissao apenas para visualizar a pagina
                            if (SessaoAtual != null && SessaoAtual.UsuarioAtendimento)
                            {
                                btnConfirmar.Visible = false;
                            }

                            using (ServicoPortalRAVClient cliente = new ServicoPortalRAVClient())
                            {
                                ModRAVAvulsoEntrada entradaRAV = new ModRAVAvulsoEntrada();

                                if (Session["DadosRavAvulso"] != null)
                                {
                                    ModRAVAvulsoSaida saidaRAV = new ModRAVAvulsoSaida();// Session["DadosRavAvulso"] as ModRAVAvulsoSaida;
                                    saidaRAV = (ModRAVAvulsoSaida)Session["DadosRAVAvulso"];
                                    
                                    if (saidaRAV != null)
                                    {
                                        if (saidaRAV.DadosParaCredito.Count > 0)
                                        {
                                            if (saidaRAV.DadosParaCredito[0].ValorLiquido > 0)
                                            {
                                                entradaRAV.DiasCredito = 0;
                                            }
                                            else
                                            {
                                                entradaRAV.DiasCredito = 1;
                                            }
                                        }
                                        else
                                        {
                                            entradaRAV.DiasCredito = 1;
                                        }
                                    }
                                    else
                                    {
                                        entradaRAV.DiasCredito = 1;
                                    }
                                }
                                else
                                {
                                    entradaRAV.DiasCredito = 1;
                                }

                                entradaRAV.NumeroPDV = SessaoAtual.CodigoEntidade;
                                entradaRAV.ValorAntecipado = retornaValorDisponivel();
                                entradaRAV.DadosAntecipacao = new ModRAVAntecipa();

                                if (String.Compare(queryString["TipoRavAvulsoAlteracao"], "P", true) == 0)
                                {
                                    entradaRAV.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Parcelado;
                                }
                                else if (String.Compare(queryString["TipoRavAvulsoAlteracao"], "V", true) == 0)
                                {
                                    entradaRAV.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Rotativo;
                                }
                                else
                                {
                                    entradaRAV.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Ambos;
                                }

                                decimal valorAntecipar = 0;
                                if (queryString["ValorRavAvulsoEfetivacao"] != null)
                                {
                                    valorAntecipar = queryString["ValorRavAvulsoEfetivacao"].ToString().ToDecimal();
                                }

                                

                                if (queryString["RavAvulsoTipoAntecipacao"] != null)
                                {
                                    Session["RavAvulsoTipoAntecipacaoTEXTO"] = queryString["RavAvulsoTipoAntecipacao"].ToString();
                                    ltrTipoAntecipacao.Text = RAVComum.RetornaAliasProdutoAntecipacao(queryString["RavAvulsoTipoAntecipacao"].ToString());
                                }

                                if (queryString["RavAutomaticoTipoAntecipacao"] != null)
                                    ltrTipoAntecipacao.Text = RAVComum.RetornaAliasProdutoAntecipacao(queryString["RavAutomaticoTipoAntecipacao"].ToString());

                                ModRAVAvulsoSaida saidaConsulta = cliente.ConsultarRAVAvulso(entradaRAV, SessaoAtual.CodigoEntidade, entradaRAV.DiasCredito, valorAntecipar);

                                decimal valorSolicitado = 0;
                                valorAntecipar = 0;
                                if (queryString["ValorRavAvulsoSolicitado"] != null && !string.IsNullOrEmpty(queryString["ValorRavAvulsoSolicitado"].ToString()))
                                {
                                    valorSolicitado = queryString["ValorRavAvulsoSolicitado"].ToString().ToDecimal();
                                }

                                if (saidaConsulta != null && saidaConsulta.Retorno > 70000)
                                {
                                    base.ExibirPainelExcecao(FONTE, saidaConsulta.Retorno);
                                    return;
                                }

                                //Session["DadosRAVAvulsoComprovante"] = saida;

                                //if (!string.IsNullOrEmpty(queryString["RavAvulsoTipoVenda"].ToString()))
                                if (!String.IsNullOrEmpty(queryString["TipoRavAvulsoAlteracao"]))
                                {
                                    switch (queryString["TipoRavAvulsoAlteracao"])
                                    {
                                        case "A":
                                            ltrTipoVenda.Text = "ambos (à vista e parcelado)";
                                            break;
                                        case "V":
                                            ltrTipoVenda.Text = "à vista";
                                            break;
                                        case "P":
                                            ltrTipoVenda.Text = "parcelado";
                                            break;
                                        default:
                                            ltrTipoVenda.Text = "não especificado";
                                            break;
                                    }
                                }
                                //if (!string.IsNullOrEmpty(queryString["ValorRavAvulsoSolicitado"].ToString()))                        

                                if (saidaConsulta.DadosParaCredito.Count > 0)
                                {
                                    /*14/06/2013: Foi verificado que na Consulta de RAV Avulso (Função 01)
                                     * os dados para crédito NUNCA são preenchidos na posição 1 dos DadosParaCrédito,
                                     * por isso ocorre o erro na verificação de D0 e D1 na efetivação do RAV (70016, erro 16 MA30)
                                     */
                                    //if (saida.DadosParaCredito[0].ValorLiquido > 0)
                                    //{
                                    //    if (queryString["RavAvulsoTipoVenda"].CompareTo("Parcelado") == 0)
                                    //    {
                                    //        ltrValorSolicitado.Text = saida.DadosParaCredito[0].ValorParcelado.ToString("0,0.00");
                                    //    }
                                    //    else if (queryString["RavAvulsoTipoVenda"].ToString().Replace('?', 'À').CompareTo("À vista") == 0)
                                    //    {
                                    //        ltrValorSolicitado.Text = saida.DadosParaCredito[0].ValorRotativo.ToString("0,0.00");
                                    //    }
                                    //    else
                                    //    {
                                    //        ltrValorSolicitado.Text = saida.ValorDisponivel.ToString("0,0.00");
                                    //    }

                                    //    ltrValorLiquidoReceber.Text = saida.DadosParaCredito[0].ValorLiquido.ToString("0,0.00");
                                    //    ltrDataCredito.Text = saida.DadosParaCredito[0].DataCredito.ToShortDateString();
                                    //    ltrTaxaEfetiva.Text = saida.Desconto.ToString("0.00"); //.DadosParaCredito[0].TaxaEfetiva.ToString("0.00");
                                    //    ltrTaxaMes.Text = saida.DadosParaCredito[0].TaxaPeriodo.ToString("0.00");
                                    //}
                                    //else
                                    //{

                                    //Dados para crédito sempre são preenchidos na posição 1 do array
                                    if (String.Compare(queryString["TipoRavAvulsoAlteracao"], "P", true) == 0)
                                    {
                                        ltrValorSolicitado.Text = valorSolicitado > 0 ? valorSolicitado.ToString("0,0.00") : saidaConsulta.DadosParaCredito[1].ValorParcelado.ToString("0,0.00");
                                    }
                                    else if (String.Compare(queryString["TipoRavAvulsoAlteracao"], "V", true) == 0)
                                    {
                                        ltrValorSolicitado.Text = valorSolicitado > 0 ? valorSolicitado.ToString("0,0.00") : saidaConsulta.DadosParaCredito[1].ValorRotativo.ToString("0,0.00");
                                    }
                                    else
                                    {
                                        ltrValorSolicitado.Text = valorSolicitado > 0 ? valorSolicitado.ToString("0,0.00") : saidaConsulta.ValorDisponivel.ToString("0,0.00");
                                    }

                                    ltrValorLiquidoReceber.Text = saidaConsulta.DadosParaCredito[1].ValorLiquido.ToString("0,0.00");
                                    ltrDataCredito.Text = saidaConsulta.DadosParaCredito[1].DataCredito.ToShortDateString();
                                    ltrTaxaEfetiva.Text = saidaConsulta.Desconto.ToString("0.00"); //DadosParaCredito[1].TaxaEfetiva.ToString("0.00");
                                    ltrTaxaMes.Text = saidaConsulta.DadosParaCredito[1].TaxaPeriodo.ToString("0.00");
                                    //}
                                }
                            }

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
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO_LOAD);
                }
            }
        }
        
        /// <summary>
        /// Retorna o tipo do valor disponível.
        /// </summary>
        /// <returns></returns>
        protected decimal retornaValorDisponivel()
        {
            QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
            decimal valorDisponivel = 0;

            if (decimal.TryParse(ltrValorLiquidoReceber.Text, out valorDisponivel))
            {
                return valorDisponivel;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Redireciona o usuário para a tela de alteração de RAV Avulso.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Alterar(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Confirmação RAV Avulso - Alterar"))
            {
                try
                {
                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                    queryString["AcessoSenha"] = validaSenha;
                    queryString["ValorRavAvulsoSolicitado"] = ltrValorSolicitado.Text;
                    queryString["RavAvulsoTipoVenda"] = ltrTipoVenda.Text;
                    Response.Redirect(string.Format("pn_AlteracaoRavAvulso.aspx?dados={0}", queryString.ToString()), false);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO_LOAD);
                }
            }
        }

        /// <summary>
        /// Confirma o RAV Avulso.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Confirmar(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Confirmação RAV Avulso - Confirmar"))
            {
                try
                {
                    using (ServicoPortalRAVClient cliente = new ServicoPortalRAVClient())
                    {
                        QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                        ModRAVAvulsoEntrada entrada = new ModRAVAvulsoEntrada();

                        Log.GravarMensagem("Dados Antecipação Avulsa nulo?", Session["DadosRavAvulso"] == null ? "Nulo" : "Preenchido");

                        if (Session["DadosRavAvulso"] != null)
                        {
                            ModRAVAvulsoSaida ent = Session["DadosRavAvulso"] as ModRAVAvulsoSaida;
                            if (ent != null)
                            {
                                if (ent.DadosParaCredito.Count > 0)
                                {
                                    if (ent.DadosParaCredito[0].ValorLiquido > 0)
                                    {
                                        entrada.DiasCredito = 0;
                                    }
                                    else
                                    {
                                        entrada.DiasCredito = 1;
                                    }
                                }
                                else
                                    entrada.DiasCredito = 1;
                            }
                            else
                            {
                                entrada.DiasCredito = 1;
                            }
                        }
                        else
                        {
                            entrada.DiasCredito = 1;
                        }
                        Log.GravarMensagem("Tipo Crédito da Efetivação", entrada.DiasCredito.ToString());

                        //ALTERAR O MÉTODO DE CONSULTA POIS NÃO PRECISAMOS DO PARÂMETRO DE ENTRADA

                        entrada.NumeroPDV = SessaoAtual.CodigoEntidade;
                        entrada.ValorAntecipado = retornaValorDisponivel();
                        entrada.DadosAntecipacao = new ModRAVAntecipa();

                        if (String.Compare(queryString["TipoRavAvulsoAlteracao"], "P", true) == 0)
                        {
                            entrada.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Parcelado;
                        }
                        else if (String.Compare(queryString["TipoRavAvulsoAlteracao"], "V", true) == 0)
                        {
                            entrada.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Rotativo;
                        }
                        else
                        {
                            entrada.DadosAntecipacao.IndicadorProduto = ElndProdutoAntecipa.Ambos;
                        }

                        decimal valorAntecipar = 0;
                        if (queryString["ValorRavAvulsoEfetivacao"] != null && !string.IsNullOrEmpty(queryString["ValorRavAvulsoEfetivacao"].ToString()))
                        {
                            valorAntecipar = queryString["ValorRavAvulsoEfetivacao"].ToString().ToDecimal();
                        }

                        //int retorno = cliente.EfetuarRAVAvulso(entrada, SessaoAtual.CodigoEntidade, queryString["RavAvulsoTipoCred"].ToInt32(), retornaValorDisponivel());
                        int retorno = cliente.EfetuarRAVAvulso(entrada, SessaoAtual.CodigoEntidade, entrada.DiasCredito, valorAntecipar);

                        if (retorno == 0)
                        {
                            queryString = new QueryStringSegura();
                            queryString["AcessoSenha"] = validaSenha;
                            queryString["RavAvulsoTipoVenda"] = ltrTipoVenda.Text;
                            queryString["ValorRavAvulsoSolicitado"] = ltrValorSolicitado.Text;
                            queryString["TaxaEfetiva"] = ltrTaxaEfetiva.Text;
                            queryString["TaxaMes"] = ltrTaxaMes.Text;
                            queryString["ValorLiquidoRavAvulso"] = ltrValorLiquidoReceber.Text;
                            queryString["DataCreditoRavAvulso"] = ltrDataCredito.Text;
                            queryString["RavAvulsoTipoAntecipacao"] = Session["RavAvulsoTipoAntecipacaoTEXTO"] == null ? "" : Session["RavAvulsoTipoAntecipacaoTEXTO"].ToString();

                            //Registro no histórico/log de atividades
                            Historico.RealizacaoServico(SessaoAtual, "Solicitação Antecipação Avulsa");

                            Response.Redirect(string.Format("pn_ComprovanteRavAvulso.aspx?dados={0}", queryString.ToString()), false);
                        }
                        else
                        {
                            base.ExibirPainelExcecao(FONTE, retorno);
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
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }
    }
}
