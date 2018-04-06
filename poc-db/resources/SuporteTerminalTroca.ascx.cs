using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.Comum;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.SharePoint.Utilities;
using System.Net.Mail;
using Microsoft.SharePoint.Administration;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using Microsoft.SharePoint;
using Redecard.PN.DadosCadastrais.SharePoint.MaximoServico;
using System.Collections.Generic;
using System.ServiceModel;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    public partial class SuporteTerminalTroca : UserControlBase
    {
        #region [ Controles ]

        /// <summary>
        /// qdrConfirmacao control.
        /// </summary>        
        protected QuadroAviso qdrConfirmacao;

        /// <summary>
        /// rcHeader control.
        /// </summary>        
        protected SuporteTerminalHeader rcHeader;

        #endregion

        #region [ Propriedades / Variáveis ]

        /// <summary>
        /// Terminal bancário
        /// </summary>
        internal TerminalDetalhado Terminal 
        {
            get { return rcHeader.Terminal; }
        }

        /// <summary>
        /// ID do motivo
        /// </summary>
        private Int32 IdMotivo
        {
            get { return Convert.ToString(ViewState["IdMotivo"]).ToInt32(0); }
            set { ViewState["IdMotivo"] = value; }
        }
        
        /// <summary>
        /// Dias da semana, começando com Segunda = 0 (zero-based index)
        /// </summary>
        private List<String> DiasSemana
        {
            get { return new List<String>(new[] { "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado", "Domingo" }); }
        }

        #endregion

        /// <summary>
        /// Carrega os dados de um terminal
        /// </summary>
        /// <param name="terminal">Terminal</param>
        /// <param name="descricaoMotivo">Descrição do motivo</param>
        /// <param name="idMotivo">ID do motivo na lista de motivos</param>
        internal void Carregar(TerminalDetalhado terminal, Int32 idMotivo, String descricaoMotivo)
        {
            rcHeader.CarregarDadosTerminal(terminal);
            this.IdMotivo = idMotivo;
            this.lblMotivo.Text = descricaoMotivo;
            this.CarregarComboUF();
            this.CarregarCombosHorarioFuncionamento();
        }

        /// <summary>Carrega combo UF</summary>
        private void CarregarComboUF()
        {
            ddlUF.DataSource = Enum.GetValues(typeof(TipoUf)).Cast<TipoUf>()
                .Select(uf => uf.ToString()).OrderBy(uf => uf).ToArray();
            ddlUF.DataBind();
        }

        /// <summary>Carrega combos de Horário de Funcionamento</summary>
        private void CarregarCombosHorarioFuncionamento()
        {            
            rptHorarioFuncionamento.DataSource = this.DiasSemana;
            rptHorarioFuncionamento.DataBind();
        }

        /// <summary>Bind dos horários nas combos de Horário de Funcionamento</summary>
        protected void rptHorarioFuncionamento_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {            
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal lblDiaSemana = e.Item.FindControl("lblDiaSemana") as Literal;
                DropDownList ddlInicio1 = e.Item.FindControl("ddlInicio1") as DropDownList;
                DropDownList ddlFim1 = e.Item.FindControl("ddlFim1") as DropDownList;
                DropDownList ddlInicio2 = e.Item.FindControl("ddlInicio2") as DropDownList;
                DropDownList ddlFim2 = e.Item.FindControl("ddlFim2") as DropDownList;

                lblDiaSemana.Text = e.Item.DataItem as String;

                this.CarregarHorariosCombo(ddlInicio1);
                this.CarregarHorariosCombo(ddlFim1);
                this.CarregarHorariosCombo(ddlInicio2);
                this.CarregarHorariosCombo(ddlFim2);
            }
        }

        /// <summary>Carrega horários na combo (0:00 à 23:00)</summary>
        /// <param name="ddl"></param>
        private void CarregarHorariosCombo(DropDownList ddl)
        {
            var horarios = new Dictionary<String, String>();
            horarios.Add(String.Empty, "-");

            for (Int32 iHora = 0; iHora < 24; iHora++)
                horarios.Add(iHora.ToString(), String.Format("{0:D2}:00", iHora));
            
            ddl.DataTextField = "Value";
            ddl.DataValueField = "Key";
            ddl.DataSource = horarios;
            ddl.DataBind();
        }

        /// <summary>Evento Click de Envio de Solicitação de Troca de Terminal</summary>
        protected void btnEnviar_Click(object sender, EventArgs e)
        {                                   
            String protocoloOS;
            DateTime? dataProgramada;
            DateTime dataSolicitacao = DateTime.Now;
            Boolean sucessoCriacao = this.CriarOrdemServico(out protocoloOS, out dataProgramada);

            if (sucessoCriacao)
            {
                //Muda para view de confirmação de criação de OS
                mvwTrocaTerminal.SetActiveView(vwConfirmacao);

                //Preenche o quadro de mensagem de confirmação do protocolo de atendimento
                qdrConfirmacao.Mensagem = qdrConfirmacao.Mensagem
                    .Replace("{PREVISAO_ATENDIMENTO}", dataProgramada.HasValue ? dataProgramada.Value.ToString("dd/MM/yyyy") : "-")
                    .Replace("{NUMERO_PROTOCOLO}", protocoloOS);
                qdrConfirmacao.CarregarMensagem();

                //Preenche os dados para envio de e-mail, caso solicitado pelo usuário
                CultureInfo ptBR = new CultureInfo("pt-BR");
                lblDataEmail.Text = String.Format("{0:D2} de {1} de {2:D4}", dataSolicitacao.Day,
                    ptBR.DateTimeFormat.GetMonthName(dataSolicitacao.Month), dataSolicitacao.Year);
                lblDataSolicitacao.Text = dataSolicitacao.ToString("dd/MM/yyyy");
                lblNumeroProtocolo.Text = protocoloOS;
                lblPrevisaoAtendimento.Text = dataProgramada.HasValue ? dataProgramada.Value.ToString("dd/MM/yyyy") : "-";
                lblNumeroTerminal.Text = this.Terminal.NumeroLogico;
                lblTipoTerminal.Text = this.Terminal.TipoEquipamento;
                lblNumeroEstabelecimento.Text = base.SessaoAtual.CodigoEntidade.ToString();
                lblEndereco.Text = String.Format("{0}, {1} {2}", txtEndereco.Text, txtNumero.Text, txtComplemento.Text).Trim();
                lblNomeContato.Text = txtNome.Text;
                lblNomeCliente.Text = txtNome.Text;
                lblTelefoneContato.Text = String.Format("{0} {1} {2}", txtDDD.Text, txtTelefone.Text, txtRamal.Text).Trim();

                //Contabiliza solicitação no relatório de troca de terminal
                ContabilizarTrocaTerminal();
            }
        }
        
        /// <summary>
        /// Evento de Clique para recebimento do Comprovante de Solicitação de Troca por E-mail
        /// </summary>
        protected void btnSim_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Envio de Comprovante de Solicitação de Troca por E-mail"))
            {
                String destinatario = txtEmail.Text;

                try
                {
                    EnviarEmail(destinatario);
                    Response.Redirect(Request.RawUrl);
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
        /// Envia e-mail com protocolo de solicitação de troca de terminal
        /// </summary>
        /// <param name="destinatarioEmail">E-mail do destinatário</param>
        private void EnviarEmail(String destinatarioEmail)
        {
            String assuntoEmail = 
                String.Format("Protocolo de Solicitação de Troca de Terminal #{0}", lblNumeroProtocolo.Text);
            String remetenteEmail = "rede@userede.com.br";            

            //Renderização e preparação do HTML
            StringBuilder corpoEmail = new StringBuilder();
            corpoEmail.Append("<html><body>");
            using (StringWriter writer = new StringWriter(corpoEmail))
            using (HtmlTextWriter hwriter = new HtmlTextWriter(writer))
                vwEmailConfirmacao.RenderControl(hwriter);
            corpoEmail.Append("</html></body>");               
            
            //Envio do e-mail
            EnviarEmail(remetenteEmail, destinatarioEmail, assuntoEmail, corpoEmail.ToString());
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

        /// <summary>
        /// Envia e-mail utilizando a configuração SMTP do SharePoint
        /// </summary>
        /// <param name="origem">E-mail do remetente</param>
        /// <param name="destino">E-mail do destinatário</param>
        /// <param name="assunto">Assunto do e-mail</param>
        /// <param name="corpoHtml">Mensagem do e-mail (html)</param>
        private static void EnviarEmail(String origem, String destino, String assunto, String corpoHtml)
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
                new CSSInlinerConfig { ConverterURLsRelativas = true, TextoCSS = ObterCSS() };
            CSSInliner cssInliner = new CSSInliner(cssInlinerConfig);
            corpoHtml = cssInliner.Processar(corpoHtml);

            //Cria a mensagem
            MailMessage mensagemEmail = new MailMessage(origem, destino, assunto, corpoHtml);
            mensagemEmail.IsBodyHtml = true;
            
            //Substitui as URLs de imagens de LAYOUTS do e-mail como recurso embutido
            CSSInliner.EmbutirImagens(mensagemEmail);

            //Envia o e-mail
            smtpClient.Send(mensagemEmail);
        }
     
        #endregion

        #region [ Listas - Relatório de Trocas ]
   
        /// <summary>
        /// Contabiliza abertura de OS no relatório de trocas de terminais
        /// </summary>
        private void ContabilizarTrocaTerminal()
        {            
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb(SPContext.Current.Web.ID))
                    {
                        String nomeLista = "Suporte à Terminais - Relatório de Trocas de Terminal";
                        SPList listaRelatorioTrocas = web.Lists.TryGetList(nomeLista);

                        web.AllowUnsafeUpdates = true;

                        SPListItem item = listaRelatorioTrocas.Items.Add();
                        item["Procedimento"] = new SPFieldLookupValue(this.IdMotivo, "Motivo");
                        item["PV"] = SessaoAtual.CodigoEntidade;
                        item.Update();
                    }
                }
            });                        
        }

        #endregion

        #region [ Sistema Máximo ]

        /// <summary>
        /// Criação de uma ordem de serviço no Sistema Máximo
        /// </summary>
        /// <param name="protocoloOS">Número do Protocolo da OS</param>
        /// <param name="dataProgramada">Previsão de atendimento</param>
        /// <returns>Boolean indicando sucesso da criação de OS</returns>
        private Boolean CriarOrdemServico(out String protocoloOS, out DateTime? dataProgramada)
        {
            using (Logger Log = Logger.IniciarLog("Criação da OS"))
            {
                try
                {
                    using (var contexto = new ContextoWCF<MaximoServicoClient>())
                    {
                        //Monta objetos para criação de OS no Sistema Máximo
                        OSCriacao os = this.RecuperarDadosCriacaoOS();
                        
                        //Efetua a criação da OS
                        protocoloOS = contexto.Cliente.CriarOS(out dataProgramada, os);

                        return true;
                    }
                }
                catch (FaultException<MaximoServico.GeneralFault> ex)
                {
                    protocoloOS = String.Empty;
                    dataProgramada = DateTime.MinValue;
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    return false;
                }
                catch (Exception ex)
                {
                    protocoloOS = String.Empty;
                    dataProgramada = DateTime.MinValue;
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }
            }
        }

        /// <summary>Monta objeto contendo os dados da Ordem de Serviço para criação de OS.</summary>
        private OSCriacao RecuperarDadosCriacaoOS()
        {
            OSCriacao os = new OSCriacao();
                                       
            //Preenche dados do endereço
            os.EnderecoAtendimento = new Endereco();            
            os.EnderecoAtendimento.Cep = new String(txtCEP.Text.Where(c => Char.IsNumber(c)).ToArray());
            os.EnderecoAtendimento.Logradouro = txtEndereco.Text.Trim();
            os.EnderecoAtendimento.Numero = new String(txtNumero.Text.Where(c => Char.IsNumber(c)).ToArray());
            os.EnderecoAtendimento.Complemento = txtComplemento.Text.Trim();
            os.EnderecoAtendimento.Bairro = txtBairro.Text.Trim();
            os.EnderecoAtendimento.Cidade = txtCidade.Text.Trim();
            os.EnderecoAtendimento.Estado = (TipoUf)Enum.Parse(typeof(TipoUf), ddlUF.SelectedValue);
            os.EnderecoAtendimento.PontoReferencia = txtPontoReferencia.Text.Trim();
            
            //Preenche dados de contato
            os.Contato = new Contato();
            os.Contato.Nome = txtNome.Text.Trim();
            os.Contato.Email = txtEmail.Text.Trim();
            os.Contato.Telefone = String.Format("{0} {1} {2}", txtDDD.Text.Trim(), txtTelefone.Text.Trim(), txtRamal.Text.Trim());
            os.Contato.Celular = String.Format("{0} {1}", txtDDDCelular.Text.Trim(), txtTelefoneCelular.Text.Trim());

            //Preenche dados do terminal e do PV
            os.NumeroLogico = this.Terminal.NumeroLogico;
            os.TipoEquipamento = this.Terminal.TipoEquipamento;
            os.PontoVenda = new PontoVenda();
            os.PontoVenda.Numero = this.SessaoAtual.CodigoEntidade.ToString();

            //Preenchimento do horário de funcionamento do estabelecimento - workday)
            os.HorarioAtendimento = new List<Horario>();

            foreach (RepeaterItem item in rptHorarioFuncionamento.Items)
            {
                Literal lblDiaSemana = item.FindControl("lblDiaSemana") as Literal;
                DropDownList ddlInicio1 = item.FindControl("ddlInicio1") as DropDownList;
                DropDownList ddlFim1 = item.FindControl("ddlFim1") as DropDownList;
                DropDownList ddlInicio2 = item.FindControl("ddlInicio2") as DropDownList;
                DropDownList ddlFim2 = item.FindControl("ddlFim2") as DropDownList;

                TipoDia diaSemana;
                switch (this.DiasSemana.IndexOf(lblDiaSemana.Text))
                {
                    case 0: diaSemana = TipoDia.SEGUNDA; break;
                    case 1: diaSemana = TipoDia.TERCA; break;
                    case 2: diaSemana = TipoDia.QUARTA; break;
                    case 3: diaSemana = TipoDia.QUINTA; break;
                    case 4: diaSemana = TipoDia.SEXTA; break;
                    case 5: diaSemana = TipoDia.SABADO; break;
                    default: diaSemana = TipoDia.DOMINGO; break;
                }

                if (!String.IsNullOrEmpty(ddlInicio1.SelectedValue) && !String.IsNullOrEmpty(ddlFim1.SelectedValue))
                {
                    Horario horario = new Horario();
                    horario.Dia = diaSemana;
                    horario.Inicio = new DateTime().AddHours(ddlInicio1.SelectedValue.ToInt32(0));
                    horario.Termino = new DateTime().AddHours(ddlFim1.SelectedValue.ToInt32(0));
                    os.HorarioAtendimento.Add(horario);
                }

                if (!String.IsNullOrEmpty(ddlInicio2.SelectedValue) && !String.IsNullOrEmpty(ddlFim2.SelectedValue))
                {
                    Horario horario = new Horario();
                    horario.Dia = diaSemana;
                    horario.Inicio = new DateTime().AddHours(ddlInicio2.SelectedValue.ToInt32(0));
                    horario.Termino = new DateTime().AddHours(ddlFim2.SelectedValue.ToInt32(0));
                    os.HorarioAtendimento.Add(horario);
                }
            }

            //Se FamiliaEquipamento == PIN PAD
            String familiaEquipamento = (rcHeader.Terminal.FamiliaEquipamento ?? String.Empty).ToUpper();
            if(familiaEquipamento.Contains("PIN") && familiaEquipamento.Contains("PAD"))
                os.Classificacao = TipoClassificacao.SERVIÇOSDECAMPOTROCADEPINPAD;
            else
                os.Classificacao = TipoClassificacao.SERVIÇOSDECAMPOTROCADEEQUIPAMENTO;

            //Prioridade normal
            os.Prioridade = TipoPrioridade.NORMAL;

            return os;
        }

        #endregion
    }
}