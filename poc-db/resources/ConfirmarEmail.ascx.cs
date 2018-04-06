#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [05/06/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.ServiceModel;

using Redecard.PN.Comum;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Utilities;
using System.Web;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Redecard.PN.DadosCadastrais.SharePoint
{
    public partial class ConfirmarEmail : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ConfirmarEmailHandle(object sender, Int32 tipoConfirmacao, string emailConfirmado);

        /// <summary>
        /// 
        /// </summary>
        public event ConfirmarEmailHandle ConfirmarEmailClick;

        /// <summary>
        /// Carregamento da página, buscar e-mail do PV
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                this.BuscarEmailEntidade();
        }

        /// <summary>
        /// 
        /// </summary>
        private void BuscarEmailEntidade()
        {
            using (Logger Log = Logger.IniciarLog("Buscando e-mail entidade"))
            {
                EntidadeServico.EntidadeServicoClient client = new EntidadeServico.EntidadeServicoClient();
                try
                {
                    // cahama o serviço e recupera o e-mail do PV
                    if (InformacaoUsuario.Existe())
                    {
                        InformacaoUsuario _dados = InformacaoUsuario.Recuperar();

                        if (_dados.Confirmado)
                        {
                            Int32 codigoRetornoIS;

                            EntidadeServico.Entidade _entidade = client.ConsultarDadosCompletos(out codigoRetornoIS, _dados.NumeroPV, false); // Somente Estabelecimentos
                            client.Close();
                            if (codigoRetornoIS > 0)
                                base.ExibirPainelExcecao("EntidadeServico.ConsultarDadosCompletos", codigoRetornoIS);
                            else
                            {
                                if (!object.Equals(_entidade, null))
                                {
                                    /* Data Alteração: 10/01/2014
                                     * QuickWins: Removido regra de verificação de alterções nos últimos 30 dias
                                     * 
                                    //verificar data limite para envio de e-mail
                                    DateTime _dataLimite = DateTime.MinValue;

                                    //Manter a data limite sempre como 30 dias anteriores
                                    _dataLimite = DateTime.Now.AddDays(-30);

                                    if (_entidade.DataAlteracao > _dataLimite)
                                    {
                                        // só permitir envio de carta, passar a confirmação
                                        // de e-mail e fazer o envio diretamente por carta
                                        rdoEntidadeCarta.Checked = true;
                                        this.InvocarEventoConfirmacao();
                                    }
                                     */

                                    if (!String.IsNullOrEmpty(_entidade.Email))
                                        ltMail.Text = _entidade.Email;
                                    else
                                    {
                                        // só permitir envio de carta, passar a confirmação
                                        // de e-mail e fazer o envio diretamente por carta
                                        rdoEntidadeCarta.Checked = true;
                                        this.InvocarEventoConfirmacao();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    client.Abort();
                    Log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    client.Abort();
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Cancelar o processo de Confirmação Positiva
        /// </summary>
        protected void Cancelar(object sender, EventArgs e)
        {
            String url = String.Empty;
            url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default);
            Response.Redirect(url, true);
        }

        /// <summary>
        /// Exibe painel customizado
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <returns>Painel customizado</returns>
        private Panel RetornarPainelExcecao(String fonte, Int32 codigo)
        {
            ApplicationBasePage page = new ApplicationBasePage();
            return page.RetornarPainelExcecao(fonte, codigo);
        }

        /// <summary>
        /// Exibe erro no painel
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        private void ExibirErro(String fonte, Int32 codigo)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);
            //((QuadroAviso)quadroAviso).CarregarImagem("/_layouts/DadosCadastrais/IMAGES/CFP/Ico_Alerta.png");
            ((QuadroAviso)quadroAviso).CarregarMensagem("Atenção, dados inválidos.", mensagem);
            pnlErro.Visible = true;
            pnlPagina1.Visible = false;
        }

        /// <summary>
        /// Invoca o evento de validação, esse evento deve estar atachado na classe que hospeda o controle.
        /// </summary>
        protected void InvocarEventoConfirmacao()
        {
            if (ConfirmarEmailClick != null)
                ConfirmarEmailClick(this, (rdoEntidadeCarta.Checked ? 0 : 1), ltMail.Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected void Confirmar(object sender, EventArgs e)
        {
            this.InvocarEventoConfirmacao();
        }
    }
}
