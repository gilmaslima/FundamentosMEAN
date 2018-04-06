#region Histórico do Arquivo
/*
(c) Copyright [2012] BRQ IT Solutions.
Autor       : [- 2012/08/21 - Tiago Barbosa dos Santos]
Empresa     : [BRQ IT Solutions]
Histórico   : Criação da Classe
- [08/06/2012] – [Tiago Barbosa dos Santos] – [Criação]
 *
*/
#endregion

using System;
using System.Collections.Generic;
using Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento;
using Redecard.PN.Comum;
using System.Web.UI;
using System.Web;

namespace Redecard.PN.Cancelamento.Sharepoint.WebParts.HistoricoCancelamento
{
    public partial class HistoricoCancelamentoUserControl : UserControlBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!object.ReferenceEquals(base.SessaoAtual, null))
            {
                using (Logger Log = Logger.IniciarLog("Histórico Cancelamento - Page Load"))
                {
                    this.mensagem_erro.Visible = false;

                    EstabelecimentoCancelamento estabelecimento = null;
                    using (ServicoPortalCancelamentoClient client = new ServicoPortalCancelamentoClient())
                    {
                        try
                        {
                            estabelecimento = client.RetornaDadosEstabelecimentoCancelamento(this.SessaoAtual.CodigoEntidade);
                        }
                        catch (Exception ex)
                        {
                            SharePointUlsLog.LogErro(ex.Message);
                            Log.GravarErro(ex);
                        }
                    }

                    if (estabelecimento == null || !estabelecimento.Centralizadora)
                    {
                        //    ScriptManager.RegisterStartupScript(Page, GetType(), "key", "$('.ocultar').hide();", true);
                        ScriptManager.RegisterStartupScript(Page, GetType(), "key", "$('.ocultar').hide();", true);
                        ScriptManager.RegisterStartupScript(Page, GetType(), "key", "document.getElementsByClassName('ocultar').style.display = 'none';", true);
                    }

                    if (!Page.IsPostBack)
                        ExecutarBindFiliais(new List<ModCancelamentoConsulta>());
                    //rptDados.DataSource = new List<ModCancelamentoConsulta>();
                    //rptDados.DataBind();
                }
            }
        }

        /// <summary>
        /// Metodo executado no clique do botão buscar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Buscar(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Histórico Cancelamento - Buscar"))
            {
                ModConsultaResult lstConsulta = new ModConsultaResult();
                //Validação da Tela
                if (radNumAviso.Checked)
                {
                    if (txtNumCam.Text == "")
                    {
                        mensagem_erro.Text = "Informe o número do Aviso de Cancelamento";
                        //Alert("Informe o número do Aviso de Cancelamento", false);//TODO:(Tiago)Código do Erro
                        return;
                    }
                    decimal x;
                    if (!decimal.TryParse(txtNumCam.Text, out x))
                    {
                        mensagem_erro.Text = "Informe somente números no Aviso de cancelamento";
                        //  ExibirPainelExcecao("Informe somente números no Aviso de cancelamento", 500);//TODO:(Tiago)Código do Erro
                        return;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(data2.Text) && !string.IsNullOrEmpty(data1.Text))
                    {
                        if (data1.Text == null)
                        {
                            mensagem_erro.Text = "Data inicial inválida";
                            //ExibirPainelExcecao("Data inicial inválida", 500);//TODO:(Tiago)Código do Erro
                            return;
                        }
                        else if (data2.Text == null)
                        {
                            mensagem_erro.Text = "Data final inválida";
                            //ExibirPainelExcecao("Data final inválida", 500);//TODO:(Tiago)Código do Erro
                            return;
                        }

                        DateTime t;
                        if (!DateTime.TryParse(data1.Text, out t))
                        {
                            mensagem_erro.Text = "Data inicio inválida.";
                            return;
                        }

                        if (!DateTime.TryParse(data2.Text, out t))
                        {
                            mensagem_erro.Text = "Data final inválida.";
                            return;
                        }


                        else if (Convert.ToDateTime(data2.Text) < Convert.ToDateTime(data1.Text))
                        {
                            mensagem_erro.Text = "Data final maior que data incial.";
                            return;
                        }
                    }
                    else
                    {
                        mensagem_erro.Text = "Data de inválida.";
                        return;
                    }
                }

                using (ServicoPortalCancelamentoClient client = new ServicoPortalCancelamentoClient())
                {

                    try
                    {
                        if (radNumAviso.Checked)
                        {
                            lstConsulta = client.ConsultaPorAvisoCancelamento(SessaoAtual.CodigoEntidade, Convert.ToDecimal(txtNumCam.Text));

                            if (lstConsulta != null && lstConsulta.ListaRetorno != null)
                            {
                                SharePointUlsLog.LogMensagem(string.Format("HISTORICO POR AVISO - COD ERRO: {0} - MSG ERRO: {1} - QTD REGISTROS: {2}", lstConsulta.CodErro, lstConsulta.DescErro, lstConsulta.ListaRetorno.Count));
                                Log.GravarMensagem(string.Format("HISTORICO POR AVISO - COD ERRO: {0} - MSG ERRO: {1} - QTD REGISTROS: {2}", lstConsulta.CodErro, lstConsulta.DescErro, lstConsulta.ListaRetorno.Count));
                            }
                            else
                            {
                                SharePointUlsLog.LogMensagem(string.Format("HISTORICO POR AVISO - RETORNO NULO"));
                                Log.GravarMensagem(string.Format("HISTORICO POR AVISO - RETORNO NULO"));
                            }

                            if (lstConsulta.CodErro != 0)
                            {
                                ExibirPainelExcecao("HistoricoCancelamentoUserControl.ascx", lstConsulta.CodErro);
                            }
                        }
                        else
                        {
                            lstConsulta = client.ConsultaPorPeriodo(Convert.ToString(SessaoAtual.CodigoEntidade), data1.Text, data2.Text);

                            if (lstConsulta.CodErro != 0)
                            {
                                ExibirPainelExcecao("HistoricoCancelamentoUserControl.ascx", lstConsulta.CodErro);

                                if (lstConsulta != null && lstConsulta.ListaRetorno != null)
                                {
                                    SharePointUlsLog.LogMensagem(string.Format("HISTORICO POR PERIODO - COD ERRO: {0} - MSG ERRO: {1} - QTD REGISTROS: {2}", lstConsulta.CodErro, lstConsulta.DescErro, lstConsulta.ListaRetorno.Count));
                                    Log.GravarMensagem(string.Format("HISTORICO POR PERIODO - COD ERRO: {0} - MSG ERRO: {1} - QTD REGISTROS: {2}", lstConsulta.CodErro, lstConsulta.DescErro, lstConsulta.ListaRetorno.Count));
                                }
                                else
                                {
                                    SharePointUlsLog.LogMensagem(string.Format("HISTORICO POR AVISO - RETORNO NULO"));
                                    Log.GravarMensagem(string.Format("HISTORICO POR AVISO - RETORNO NULO"));
                                }
                            }
                        }
                    }
                    catch (PortalRedecardException rex)
                    {
                        SharePointUlsLog.LogErro(rex);
                        ExibirPainelExcecao(rex);
                        Log.GravarErro(rex);
                        return;
                    }
                    catch (Exception ex)
                    {
                        SharePointUlsLog.LogErro(ex);
                        ExibirPainelExcecao("HistoricoCancelamentoUserControl.ascx", 500);//TODO:(Tiago)Código do Erro
                        Log.GravarErro(ex);
                        return;
                    }
                }

                SharePointUlsLog.LogMensagem("HISTORICO - Realizar Bind da Lista");
                Log.GravarMensagem("HISTORICO - Realizar Bind da Lista");
                if (lstConsulta != null && lstConsulta.CodErro == 0)
                {
                    SharePointUlsLog.LogMensagem("HISTORICO - Bind da lista, " + lstConsulta.ListaRetorno.Count + " registros");
                    Log.GravarMensagem("HISTORICO - Bind da lista, " + lstConsulta.ListaRetorno.Count + " registros");
                    ExecutarBindFiliais(lstConsulta.ListaRetorno);
                }
                else
                {
                    ExecutarBindFiliais(new List<ModCancelamentoConsulta>());
                }

                //if (lstConsulta.Count == 0)
                //    ExibirPainelExcecao("A Consulta não retornou registros.", 0);          
            }  
        }

        /// <summary>
        /// Método para realizar a carga dos dados na lista (Grid)
        /// </summary>
        /// <param name="filiais"></param>
        private void ExecutarBindFiliais(List<ModCancelamentoConsulta> filiais)
        {
            while (filiais.Count < 10)
            {

                ModCancelamentoConsulta ObjModCancelamentoConsulta = new ModCancelamentoConsulta();
                filiais.Add(ObjModCancelamentoConsulta);
            }

            rptDados.DataSource = filiais;

            rptDados.DataBind();

            //Adiciona a função de JS de paginação do grid para as funções a serem executadas após renderização
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Paginacao", "pageResultTable('tblDados', 1, 11, 6);", true);
        }

        /// <summary>
        /// Método executado no clique do botão voltar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Cancelamento_Voltar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Histórico Cancelamento - Voltar"))
            {
                Response.Redirect("pn_cancelamento.aspx");
            }
        }

        /// <summary>
        /// Método executado para habilitar a pesquisa por aviso
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void HabilitaAviso(object sender, EventArgs e)
        {
            this.txtNumCam.ReadOnly = false;
            this.data1.ReadOnly = true;
            this.data2.ReadOnly = true;
        }

        /// <summary>
        /// Método executado para habilitar a pesquisa por periodo
        /// </summary>
        /// <param name="sender"></param>
        protected void HabilitaPeriodo(object sender, EventArgs e)
        {
            this.txtNumCam.ReadOnly = true;
            this.data1.ReadOnly = false;
            this.data2.ReadOnly = false;
        }


    }

}
