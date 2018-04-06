using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.Comum;
using Microsoft.SharePoint.Administration;
using System.Net.Mail;
using System.IO;
using System.Text;
using Microsoft.SharePoint;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.DadosCadastrais.SharePoint.MaximoServico;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    public partial class SuporteTerminalEmail : UserControlBase
    {
        #region [ Controles ]

        /// <summary>
        /// qdrAviso control.
        /// </summary>        
        protected QuadroAviso qdrAviso;

        /// <summary>
        /// rcHeader control.
        /// </summary>        
        protected SuporteTerminalHeader rcHeader;

        /// <summary>
        /// rcEmailHeader control.
        /// </summary>        
        protected SuporteTerminalHeader rcEmailHeader;

        #endregion

        #region [ Listas ]

        private static SPList ListaEmails { get { return SPContext.Current.Web.Lists.TryGetList("Suporte à Terminais - E-mails"); } }

        /// <summary>
        /// Recupera a lista de e-mails que receberão as solicitações de Atendimento por E-mail
        /// </summary>
        public List<String> ObterEmails()
        {
            SPQuery query = new SPQuery();
            query.Query = "<Where><Eq><FieldRef Name=\"Ativo\" /><Value Type=\"Boolean\">1</Value></Eq></Where>";

            List<String> emails = new List<String>();

            foreach (SPListItem item in ListaEmails.GetItems(query))
                emails.Add((String)item["Email"]);

            return emails;
        }

        #endregion

        /// <summary>
        /// Carrega os dados de identificação do terminal
        /// </summary>
        /// <param name="terminal">Terminal</param>
        /// <param name="descricaoMotivo">Descrição do motivo selecionado</param>
        internal void Carregar(TerminalDetalhado terminal, String descricaoMotivo)
        {
            qdrAviso.CarregarMensagem();
            rcHeader.CarregarDadosTerminal(terminal);
            rcEmailHeader.CarregarDadosTerminal(terminal);

            this.lblMotivo.Text = descricaoMotivo;
            this.lblEmailMotivo.Text = descricaoMotivo;
        }

        /// <summary>
        /// Enviar Solicitação de Atendimento por E-mail
        /// </summary>
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Envio de Solicitação de Atendimento por E-mail"))
            {
                try
                {
                    String assuntoEmail = String.Concat("Suporte à Terminais - Terminal #", this.rcHeader.Terminal.NumeroLogico);
                    String[] destinatarioEmail = ObterEmails().ToArray();

                    //Preenche controles do html que será enviado por e-mail,
                    //replicando os dados do formulário para o html
                    lblNome.Text = txtNome.Text;
                    lblEmail.Text = txtEmail.Text;
                    lblDDD1.Text = txtDDD1.Text;
                    lblTelefone1.Text = txtTelefone1.Text;
                    lblRamal1.Text = txtRamal1.Text;
                    lblDDD2.Text = txtDDD2.Text;
                    lblTelefone2.Text = txtTelefone2.Text;
                    lblRamal2.Text = txtRamal2.Text;
                    lblMensagem.Text = txtMensagem.Text;

                    //Renderização e preparação do HTML
                    StringBuilder corpoEmail = new StringBuilder();
                    corpoEmail.Append("<html><body>");
                    using (StringWriter writer = new StringWriter(corpoEmail))
                    using (HtmlTextWriter hwriter = new HtmlTextWriter(writer))
                        vwEmail.RenderControl(hwriter);
                    corpoEmail.Append("</body></html>");

                    //Envio do e-mail
                    EnviarEmail("rede@userede.com.br", destinatarioEmail, assuntoEmail, corpoEmail.ToString());

                    mvwAtendimentoEmail.SetActiveView(vwAvisoSucesso);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #region [ Envio de E-mail ]

        /// <summary>
        /// Envia e-mail utilizando a configuração SMTP do SharePoint
        /// </summary>
        /// <param name="remetente">E-mail do remetente</param>
        /// <param name="destinatarios">E-mails dos destinatários</param>
        /// <param name="assunto">Assunto do e-mail</param>
        /// <param name="corpoHtml">Mensagem do e-mail (html)</param>
        private static void EnviarEmail(String remetente, String[] destinatarios, String assunto, String corpoHtml)
        {
            //Cria o objeto para envio de e-mail (Buscando da configuração do Sharepoint)
            string smtpServer = SPAdministrationWebApplication.Local.OutboundMailServiceInstance != null ?
                                SPAdministrationWebApplication.Local.OutboundMailServiceInstance.Server.Address
                                : "";

#if !DEBUG
                //Verifica se retornou o servidor para envio de e-mail
                if (string.IsNullOrEmpty(smtpServer))
                    throw new Exception("SMTP para envio de e-mail não configurado no servidor do Sharepoint.");

#else
            smtpServer = "smptp@iteris.com.br";
#endif

            //Cria o objeto para envio do e-mail
            SmtpClient smtpClient = new SmtpClient(smtpServer);

            //Prepara o HTML para envio por e-mail, aplicando estilos inline nos elementos
            CSSInlinerConfig cssInlinerConfig = 
                new CSSInlinerConfig { ConverterURLsRelativas = true, TextoCSS = ObterCSS(), SubstituirControlesPorTexto = true };
            CSSInliner cssInliner = new CSSInliner(cssInlinerConfig);
            corpoHtml = cssInliner.Processar(corpoHtml);
            
            //Cria a mensagem //MailMessage mensagemEmail = new MailMessage(remetente, destino, assunto, corpoHtml);
            var mensagemEmail = new MailMessage();
            mensagemEmail.From = new MailAddress(remetente);
            mensagemEmail.Sender = new MailAddress(remetente);
            mensagemEmail.To.Add(String.Join(",", destinatarios));
            mensagemEmail.IsBodyHtml = true;
            mensagemEmail.Body = corpoHtml;
            mensagemEmail.Subject = assunto;
            
            //Substitui as URLs de imagens de LAYOUTS do e-mail como recurso embutido
            CSSInliner.EmbutirImagens(mensagemEmail);

            //Envia o e-mail
            smtpClient.Send(mensagemEmail);
        }

        /// <summary>
        /// Conteúdo do CSS que será aplicado aos elementos do e-mail
        /// </summary>
        private static String _CSS;
        /// <summary>
        /// Obtém o conteúdo do CSS
        /// </summary>        
        private static String ObterCSS()
        {
#if DEBUG
            _CSS = null;
#endif
            if (_CSS == null)
            {
                String cssSuporteTerminal = @"DadosCadastrais\CSS\suporteterminal.css";

                try
                {
                    Byte[] cssData = CSSInliner.ObterRecursoLayouts(cssSuporteTerminal);
                    _CSS = new StreamReader(new MemoryStream(cssData)).ReadToEnd();
                }
                catch (Exception exc)
                {
                    Logger.GravarErro("Erro durante leitura de CSS para envio de E-mail: " + cssSuporteTerminal, exc);
                }                
            }
            return _CSS ?? String.Empty;
        }
     
        #endregion
    }
}
