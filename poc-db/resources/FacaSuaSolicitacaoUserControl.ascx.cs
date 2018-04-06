#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [27/07/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using System.ServiceModel;
using Redecard.PN.Comum;
using System.Collections.Generic;
using Microsoft.SharePoint.Utilities;
using System.Web;
using Microsoft.SharePoint.WebControls;

using System.Linq;

namespace Redecard.PN.OutrosServicos.SharePoint.WebParts.FacaSuaSolicitacao
{
    /// <summary>
    /// Web part de faça sua solicitação
    /// </summary>
    public partial class FacaSuaSolicitacaoUserControl : UserControlBase
    {
        /// <summary>
        /// Carregamrnto da Web Part, carregar dados das ocorrências
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CarregarOcorrencias();
            }
        }

        private Int32 _alternate = 0;

        /// <summary>
        /// 
        /// </summary>
        public String AlternateClass
        {
            get
            {
                if (_alternate == 0)
                {
                    _alternate = 1;
                    return "";
                }
                else
                {
                    _alternate = 0;
                    return "alttr";
                }
            }
        }

        /// <summary>
        /// Carregar lista de ocorrências da base do WM
        /// </summary>
        private void CarregarOcorrencias()
        {
            using (Logger Log = Logger.IniciarLog("Carregando ocorrências"))
            {
                try
                {
                    using (var _client = new SolicitacaoServico.SolicitacaoServicoClient())
                    {
                        SolicitacaoServico.Ocorrencia[] ocorrencias = _client.ConsultarOcorrencias();
                        if (ocorrencias.Length > 0)
                        {
                            ddlOcorrencias.DataSource = ocorrencias;
                            ddlOcorrencias.DataValueField = "CodigoTipoOcorrencia";
                            ddlOcorrencias.DataTextField = "NomeOcorrencia";
                            ddlOcorrencias.DataBind();
                        }
                    }
                }
                catch (FaultException<SolicitacaoServico.GeneralFault> ex)
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
        /// Carregar os motivos e as propriedades customizadas de acordo com a
        /// ocorrência selecionada
        /// </summary>
        protected void CarregarMotivosEPropriedades(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Carregando motivos e propriedades de acordo com ocorrência"))
            {
                using (var _client = new SolicitacaoServico.SolicitacaoServicoClient())
                {
                    String ocorrencia = ((DropDownList)sender).SelectedValue;
                    // Consultar Motivos
                    this.CarregarMotivos(ocorrencia, _client);
                    // Consultar Propriedades
                    this.CarregarPropriedades(ocorrencia, _client);
                    // Consultar Modo de Envio
                    CarregarModoEnvio(_client, ocorrencia);
                }
            }
        }

        /// <summary>
        /// Carregamento das propriedades
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PropriedadeDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                TextBox txtValor = e.Item.FindControl("txtValor") as TextBox;
                SolicitacaoServico.Propriedade propriedade = e.Item.DataItem as SolicitacaoServico.Propriedade;

                if (!object.ReferenceEquals(txtValor, null)
                    && !object.ReferenceEquals(propriedade, null))
                {
                    if (propriedade.TipoCampo.ToLowerInvariant().Equals("num"))
                    {
                        if (propriedade.CasasDecimais > 0)
                            txtValor.Attributes.Add("alt", "decimal");
                        else
                            txtValor.Attributes.Add("alt", "number");
                    }
                    else if (propriedade.TipoCampo.ToLowerInvariant().Equals("dat"))
                    {
                        txtValor.CssClass = "caixaTexto dtPicker";
                    }
                }
            }
        }

        /// <summary>
        /// Consultar se deve exibir o painel de modo de envio para a ocorrência
        /// </summary>
        private void CarregarModoEnvio(SolicitacaoServico.SolicitacaoServicoClient _client, String ocorrencia)
        {
            using (Logger Log = Logger.IniciarLog("Carregando modo de envio"))
            {
                try
                {
                    String _modoEnvio = _client.ConsultarModoEnvio(ocorrencia);
                    if (_modoEnvio.ToLowerInvariant() == "se")
                        pnlModoEnvio.Visible = true;
                    else
                        pnlModoEnvio.Visible = false;
                }
                catch (FaultException<SolicitacaoServico.GeneralFault> ex)
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
        /// Carregar as propriedades customizadas de acordo com a ocorrência
        /// selecionada
        /// </summary>
        private void CarregarPropriedades(String ocorrencia, SolicitacaoServico.SolicitacaoServicoClient _client)
        {
            using (Logger Log = Logger.IniciarLog("Carregando propriedades"))
            {
                try
                {
                    SolicitacaoServico.Propriedade[] propriedades = _client.ConsultarPropriedades(ocorrencia);
                    if (propriedades.Length > 0)
                    {
                        // ordernar propriedades
                        List<SolicitacaoServico.Propriedade> _propriedades = new List<SolicitacaoServico.Propriedade>();
                        foreach (SolicitacaoServico.Propriedade propriedade in propriedades)
                        {
                            String campo = propriedade.CodigoCampo.ToLowerInvariant();
                            if (campo != "emal_antg" && campo != "indicativo")
                                _propriedades.Add(propriedade);
                        }
                        _propriedades.Sort(new PropriedadesComparer());

                        rptPropriedades.DataSource = _propriedades;
                        rptPropriedades.DataBind();
                    }
                }
                catch (FaultException<SolicitacaoServico.GeneralFault> ex)
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
        /// Carregar todos os motivos de acordo com a ocorrência selecionada
        /// </summary>
        /// <param name="ocorrencia"></param>
        private void CarregarMotivos(String ocorrencia, SolicitacaoServico.SolicitacaoServicoClient _client)
        {
            using (Logger Log = Logger.IniciarLog("Carregando motivos"))
            {
                try
                {
                    SolicitacaoServico.Motivo[] motivos = _client.ConsultarMotivos(ocorrencia);
                    if (motivos.Length > 0)
                    {
                        ddlMotivo.Items.Clear();
                        ddlMotivo.DataSource = motivos;
                        ddlMotivo.DataValueField = "IdentificadorMotivo";
                        ddlMotivo.DataTextField = "DescricaoMotivo";
                        ddlMotivo.DataBind();
                    }
                }
                catch (FaultException<SolicitacaoServico.GeneralFault> ex)
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
        /// Classe privada para ordenação das propriedades customizadas
        /// </summary>
        private class PropriedadesComparer : IComparer<SolicitacaoServico.Propriedade>
        {
            public int Compare(SolicitacaoServico.Propriedade x, SolicitacaoServico.Propriedade y)
            {
                return x.OrdemApresentacao.CompareTo(y.OrdemApresentacao);
            }
        }

        /// <summary>
        /// Criar uma nova solicitação de acordo com os paramêtros informados
        /// </summary>
        protected void EfetivarSolicitacao(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Efetuando solicitações"))
            {
                try
                {
                    String _ocorrencia = ddlOcorrencias.SelectedValue;
                    String _motivo = ddlMotivo.SelectedValue;
                    Int32 _numeroPV = this.SessaoAtual.CodigoEntidade;
                    String _descricaoCaso = txtDetalhes.Text;

                    // Formas de envio da resposta a solicitação
                    //Solicitação Usuário para Gravar Sempre 3 Forma de Envio Correio
                    List<KeyValuePair<String, String>> formasEnvio = new List<KeyValuePair<String, String>>();
                    formasEnvio.Add(new KeyValuePair<String, String>("3", String.Empty));
                    if (pnlModoEnvio.Visible)
                    {
                        if (chkFax.Checked)
                            formasEnvio.Add(new KeyValuePair<String, String>("2", txtFax.Text));
                        if (chkEmail.Checked)
                            formasEnvio.Add(new KeyValuePair<String, String>("3", txtEmail.Text));
                        if (chkCorreio.Checked)
                            formasEnvio.Add(new KeyValuePair<String, String>("4", "Enviado por correio"));
                    }

                    // Pré-requisitos preenchidos
                    List<KeyValuePair<String, String>> _propriedades = new List<KeyValuePair<String, String>>();
                    foreach (RepeaterItem item in rptPropriedades.Items)
                    {
                        HiddenField hdfCodigo = item.FindControl("hdfCodigo") as HiddenField;
                        HiddenField hdfTipoCampo = item.FindControl("hdfTipoCampo") as HiddenField;
                        TextBox txtValor = item.FindControl("txtValor") as TextBox;

                        if (!object.ReferenceEquals(hdfCodigo, null)
                            && !object.ReferenceEquals(txtValor, null)
                            && !object.ReferenceEquals(hdfTipoCampo, null))
                        {
                            if (hdfTipoCampo.Value.ToLowerInvariant().Equals("dat"))
                            {
                                DateTime dt = txtValor.Text.ToDate();
                                String valor = dt.ToString("ddMMyyyy");
                                _propriedades.Add(new KeyValuePair<String, String>(hdfCodigo.Value, valor));
                            }
                            else
                                _propriedades.Add(new KeyValuePair<String, String>(hdfCodigo.Value, txtValor.Text));
                        }
                    }

                    System.Diagnostics.Trace.WriteLine("TRC: " + _ocorrencia);
                    System.Diagnostics.Trace.WriteLine("TRC: " + _numeroPV.ToString());
                    System.Diagnostics.Trace.WriteLine("TRC: " + _motivo);
                    System.Diagnostics.Trace.WriteLine("TRC: " + _descricaoCaso);
                    foreach (KeyValuePair<String, String> pair in _propriedades)
                        System.Diagnostics.Trace.WriteLine("TRC: KEY=" + pair.Key + " VALUE=" + pair.Value);
                    foreach (KeyValuePair<String, String> pair in formasEnvio)
                        System.Diagnostics.Trace.WriteLine("TRC: KEY=" + pair.Key + " VALUE=" + pair.Value);

                    using (var _client = new SolicitacaoServico.SolicitacaoServicoClient())
                    {
                        Int32 numeroSolicitacao = _client.IncluirSolicitacao(_ocorrencia, _numeroPV, _motivo, _descricaoCaso, _propriedades.ToArray(), formasEnvio.ToArray());

                        if (numeroSolicitacao > 0)
                        {
                            //Registro no histórico/log de atividades
                            Historico.RealizacaoServico(SessaoAtual, "Solicitação");

                            this.ExibirPainelSucesso(numeroSolicitacao);
                        }
                    }
                }
                catch (FaultException<SolicitacaoServico.GeneralFault> ex)
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
        /// 
        /// </summary>
        /// <param name="numeroSolicitacao"></param>
        private void ExibirPainelSucesso(Int32 numeroSolicitacao)
        {
            //pnlDados.Visible = false;
            //pnlSucesso.Visible = true;
            //txtNumeroSolicitacao.Text = numeroSolicitacao.ToString();

            Panel[] paineis = new Panel[1]
            {
                pnlDados
            };

            String mensagem = String.Format("Solicitação de número <b>{0}</b> incluída com sucesso.<br /><br />O prazo para análise desta solicitação é de até 2 dias úteis", numeroSolicitacao);

            base.ExibirPainelConfirmacaoAcao("Concluído com Sucesso", mensagem, SPUtility.GetPageUrlPath(HttpContext.Current), paineis);
        }
    }
}