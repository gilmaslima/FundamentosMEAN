using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Comum.TrataErroServico;
using System.ServiceModel;

namespace Redecard.PN.Comum.SharePoint.LAYOUTS.Redecard.Comum
{
    /// <summary>
    /// Tela de atualização de mensagens de erro
    /// </summary>
    public partial class CadastroErros : ApplicationPageBaseAutenticadaWindows
    {
        /// <summary>
        /// Inicialização da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {   
                if (!Page.IsPostBack)
                {
                    this.ChecarUsuarioAdministrador();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                dvPainel.Controls.Add(base.RetornarPainelExcecao(ex.Message));
                pnlBusca.Visible = false;
            }

            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                dvPainel.Controls.Add(base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void ChecarUsuarioAdministrador()
        {
            if (!SPContext.Current.Web.UserIsWebAdmin)
            {
                throw new UnauthorizedAccessException("Somente usuários administradores podem alterar as mensagens de erro.");
                
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                Int32 codigoErro = 0;
                using (var servico = new TrataErroServico.TrataErroServicoClient())
                {
                    if (Int32.TryParse(txtCodigoErro.Text, out codigoErro))
                    {
                        var mensageErro = servico.ConsultarPorCodigo(codigoErro);
                        if (mensageErro.Codigo > 0)
                        {
                            lblCodigoErro.Text = mensageErro.Codigo.ToString();
                            txtMensagem.Text = mensageErro.Fonte;
                            ckbAtivo.Checked = mensageErro.Ativo;
                            pnlResultado.Visible = true;
                            pnlAviso.Visible = false;
                        }
                        else
                        {
                            Alerta("Código da mensagem de erro não localizado.");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Alerta(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSalvar_OnClick(object sender, EventArgs e)
        {
            try
            {
                Int32 codigoErro = 0;
                using (var servico = new TrataErroServico.TrataErroServicoClient())
                {
                    if (Int32.TryParse(lblCodigoErro.Text, out codigoErro))
                    {

                        if (codigoErro > 0)
                        {
                            TrataErro mensagemErro = new TrataErro();
                            mensagemErro.Codigo = codigoErro;
                            mensagemErro.Fonte = txtMensagem.Text;
                            mensagemErro.Ativo = ckbAtivo.Checked;

                            servico.AtualizarMensagem(mensagemErro);
                            lblErro.Text = "Mensagem alterada com sucesso!";
                            pnlAviso.Visible = true;
                        }
                        else
                        {
                            Alerta("Não é possível alterar a mensagem de erro.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Alerta(ex.Message);
            }
        }
        void Alerta(string mensagem)
        {
            lblErro.Text = mensagem;
            pnlAviso.Visible = true;
            pnlResultado.Visible = false;
        }
    }
}
