using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Redecard.PN.Comum;
using System.ServiceModel;
using Redecard.PN.Request.SharePoint.RegimeFranquiaServico;

namespace Redecard.PN.Request.SharePoint.WebParts.RequestCanal
{
    public partial class RequestCanalUserControl : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Definição do botão default da tela
            this.Page.Form.DefaultButton = btnSalvar.UniqueID;

            if (!Page.IsPostBack)
            {
                qdAviso.Visible = false;
                CarregarCanal();
            }
            else { SetarAviso(string.Empty); }
        }

        #region Negócio e Carga de Dados

        /// <summary>Comunica com o serviço e seleciona o radio button correspondente</summary>
        private void CarregarCanal()
        {
            using (Logger Log = Logger.IniciarLog("Como ser avisado"))
            {
                try
                {
                    //variáveis de serviço
                    using (XBChargebackServico.HISServicoXBChargebackClient client = new XBChargebackServico.HISServicoXBChargebackClient())
                    {

                        //parâmetros
                        String descricaoCanal = String.Empty;
                        Int64 codigoOcorrencia = 0;
                        Int32 codigoRetorno = 0;
                        Int32 codigoEstabelecimento = base.SessaoAtual.CodigoEntidade;

                        Log.GravarLog(EventoLog.ChamadaServico, new { codigoEstabelecimento });

                        //chamada do serviço
                        XBChargebackServico.Canal canal = client.ConsultarCanal(codigoEstabelecimento, "IS", ref codigoOcorrencia, out codigoRetorno);

                        Log.GravarLog(EventoLog.RetornoServico, new { codigoOcorrencia, codigoRetorno, canal });

                        //caso de erro
                        //considerar que 10 ou 53 não é erro: DADOS NAO ENCONTRADOS NA TABELA
                        if (codigoRetorno > 0 && codigoRetorno != 10 && codigoRetorno != 53)
                            base.ExibirPainelExcecao("XBChargebackServico.ConsultarCanal", codigoRetorno);
                        else
                        {
                            //seleciona o radio button de acordo com o retornado pelo servico                   
                            rdbCanal.SelectedValue = ((Int32)canal.CanalRecebimento).ToString();
                        }
                    }
                }
                catch (FaultException<XBChargebackServico.GeneralFault> ex)
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

        /// <summary>Incluiu aceite de usuário na base de contratos</summary>        
        private Boolean IncluirAceite()
        {
            using (Logger Log = Logger.IniciarLog("Incluindo aceite"))
            {
                Int32 codGrupoEntidade = SessaoAtual.GrupoEntidade;
                Int32 codEntidade = SessaoAtual.CodigoEntidade;

                //Registra o aceite do usuário na base de contratos
                /* Obs: o método IncluirAceite exposto em RegimeFranquiaServico é o mesmo IncluirAceite utilizado
                 * na atualização do Canal. Para evitar replicação de código, está sendo utilizado diretamente o serviço
                 * criado para o módulo OutrosServicos, apesar do negócio não estar diretamente relacionado ao módulo Request. */
                try
                {
                    //Declaração e preenchimento de valores dos parâmetros da inclusão de aceite
                    String codUsuario = "INTERNET - " + SessaoAtual.LoginUsuario;
                    Int32 codVersao = 1;

                    //Cód. Restrição 3: Termo de Utilização dos Serviços de Aviso de 
                    //Solicitação de Cópia de Documentos e Avisos de Débito
                    Int32 codRestricao = 3;

                    switch (rdbCanal.SelectedValue.ToInt32Null(0).Value)
                    {
                        case 4: codRestricao = 3; break;
                        case 2: codRestricao = 4; break;
                        case 1: codRestricao = 5; break;
                        case 3: codRestricao = 6; break;
                    }

                    //Instanciação do client do serviço RegimeFranquiaServico
                    using (RegimeFranquiaServicoClient client = new RegimeFranquiaServicoClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico, new { codRestricao, codVersao, codGrupoEntidade, codEntidade, codUsuario });

                        //Chama inclusão de aceite                    
                        Int16 codRetorno = 0;

#if RELEASE
                        codRetorno = client.IncluirAceiteUsuario(codRestricao, codVersao, codGrupoEntidade, codEntidade, codUsuario); 
#endif

                        Log.GravarLog(EventoLog.RetornoServico, new { codRetorno });

                        //Em caso de erro, exibe painel de exceção
                        //considerar que 10 ou 53 não é erro: DADOS NAO ENCONTRADOS NA TABELA
                        if (codRetorno > 0 && codRetorno != 10 && codRetorno != 53)
                        {
                            base.ExibirPainelExcecao("RegimeFranquiaServico.AtualizarFranquia", codRetorno);
                            return false;
                        }

                        Historico.RealizacaoServico(SessaoAtual, "Comprovação de Vendas - Alteração de Canal de Recebimento");

                        return true;
                    }
                }
                catch (FaultException<RegimeFranquiaServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                    return false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }
            }
        }

        /// <summary>Atualiza o canal</summary>        
        private Boolean AtualizarCanal()
        {
            using (Logger Log = Logger.IniciarLog("Atualizando canal"))
            {
                Int32 codGrupoEntidade = SessaoAtual.GrupoEntidade;
                Int32 codEntidade = SessaoAtual.CodigoEntidade;

                try
                {
                    //Parâmetros
                    String msgRetorno = String.Empty;
                    Int32 codRetorno = default(Int32);

                    //Client do serviço de atualização de canal
                    using (XBChargebackServico.HISServicoXBChargebackClient client = new XBChargebackServico.HISServicoXBChargebackClient())
                    {

                        //Conversão do canal atual para instância do enumerador
                        XBChargebackServico.CanalRecebimento canalRecebimento = XBChargebackServico.CanalRecebimento.NaoDefinido;
                        if (Enum.IsDefined(typeof(XBChargebackServico.CanalRecebimento), rdbCanal.SelectedValue.ToInt32Null(0).Value))
                        {
                            canalRecebimento = (XBChargebackServico.CanalRecebimento)Enum.Parse(typeof(XBChargebackServico.CanalRecebimento), rdbCanal.SelectedValue);

                            Log.GravarLog(EventoLog.ChamadaServico, new { codEntidade, canalRecebimento });

                            //Atualiza o canal
                            codRetorno = client.AtualizarCanal(out msgRetorno, codEntidade, canalRecebimento, "IS");

                            Log.GravarLog(EventoLog.RetornoServico, new { codRetorno });

                            //Em caso de erro, exibe painel de exceção
                            //considerar que 10 ou 53 não é erro: DADOS NAO ENCONTRADOS NA TABELA
                            if (codRetorno > 0 && codRetorno != 10 && codRetorno != 53)
                            {
                                base.ExibirPainelExcecao("XBChargebackServico.AtualizarCanal", codRetorno);
                                return false;
                            }
                            return true;
                        }
                        else
                            return false;
                    }
                }
                catch (FaultException<XBChargebackServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                    return false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }
            }
        }

        /// <summary>Verifica se existe um canal válido selecionado</summary>        
        private Boolean ValidarSelecao()
        {
            Int32 canalSelecionado = rdbCanal.SelectedValue.ToInt32Null(0).Value;

            if (canalSelecionado != 0)
            {
                if (Enum.IsDefined(typeof(XBChargebackServico.CanalRecebimento), canalSelecionado))
                    return true;
                else return false;
            }
            else return false;
        }

        #endregion

        #region Handlers

        /// <summary>Handler do botão salvar que chama o serviço para alterar o canal de comunicação (caso seja diferente)</summary>        
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Alteração de Canal de Comunicação"))
            {
                //Verifica se existe um canal selecionado
                if (!ValidarSelecao())
                {
                    //base.Alert("É necessário selecionar uma opção.", false);
                    SetarAviso("é necessário selecionar uma opção.");
                    return;
                }

                //Inclui o aceite do usuário na base de contratos. Em caso de erro, o tratamento é feito dentro do método.
                Boolean incluiuAceite = IncluirAceite();

                Log.GravarMensagem("Incluiu aceite = " + incluiuAceite);

                //Se ok, continua com atualização de Canal
                if (incluiuAceite)
                {
                    //Atualiza canal. Em caso de erro, o tratamento é feito dentro do método
                    Boolean atualizouCanal = AtualizarCanal();

                    Log.GravarMensagem("Atualizou canal = " + atualizouCanal);

                    //Ok.
                    if (atualizouCanal)
                    {
                        //Alert("Canal salvo com sucesso.", base.web.ServerRelativeUrl + "/Paginas/pn_ComoSerAvisado.aspx");
                        //Panel[] paineis = new Panel[1];
                        //paineis[0] = pnlPrincipal;
                        ////paineis[1] = pnlVoltar;
                        //base.ExibirPainelConfirmacaoAcao("Confirmação", "Canal salvo com sucesso.", base.web.ServerRelativeUrl + "/Paginas/pn_ComoSerAvisado.aspx", paineis);
                        qdAviso.Visible = true;
                        rdbCanal.Visible = false;
                        btnSalvar.Visible = false;

                    }
                }
            }
        }

        /// <summary>Handler do botão voltar, redireciona para a tela de Comprovação de Vendas</summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect(base.web.ServerRelativeUrl + "/Paginas/pn_ComprovacaoVendas.aspx");
        }
        #endregion

        /// <summary>
        /// Exibe a mensagem de erro
        /// </summary>
        private void SetarAviso(String aviso)
        {
            lblMensagem.Text = aviso;
        }
    }
}
