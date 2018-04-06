using Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico;
using Redecard.PN.Comum;
using System;
using System.ServiceModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Rede.PN.Eadquirencia.Sharepoint.Webparts.CadastrarDba
{
    public partial class CadastrarDbaUserControl : UserControlBase
    {
        #region [Eventos]

        /// <summary>
        /// OnInit
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarPagina();
            }
        }

        /// <summary>
        /// Evento do botão Cadastrar Nome
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCadastrarNome_Click(object sender, EventArgs e)
        {
            divAlteracaoNome.Visible = false;
            CadastrarNome();
        }

        /// <summary>
        /// Evento do botão Cadastrar Nome
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnValidarNome_Click(object sender, EventArgs e)
        {
            ValidarNome();
        }

        /// <summary>
        /// Cancelar o cadastro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNao_Click(object sender, EventArgs e)
        {
            CarregarPagina();
        }

        #endregion [Fim - Eventos]

        #region [Métodos]

        protected void ValidarNome()
        {
            String msg = String.Empty;
            if (!String.IsNullOrEmpty(litNomeAnterior.Text))
                msg = String.Format("Deseja alterar o nome de identificação na fatura de '{0}' para '{1}'?", litNomeAnterior.Text.ToUpper(), txtNomeDba.Text.ToUpper());
            else
                msg = String.Format("Deseja cadastrar o nome '{0}' como identificação na fatura para este cliente?", txtNomeDba.Text.ToUpper());

            ExibirConfirmacao(msg);
        }

        /// <summary>
        /// Exibir mensagem de confirmação
        /// </summary>
        protected void ExibirConfirmacao(String msg)
        {
            divAlteracaoNome.Visible = true;
            litMensagem.Text = msg;
            StringBuilder sb = new StringBuilder();
            this.ExibirPainelHtml(sb.ToString());
        }

        /// <summary>
        /// Exibir mensagem de sucesso
        /// </summary>
        protected void ExibirCadastroSucesso()
        {
            base.ExibirPainelMensagem(String.Format("Cadastro realizado com sucesso! As próximas faturas emitidas terão o nome '{0}' em até 24hs.", txtNomeDba.Text.ToUpper()));
        }

        /// <summary>
        /// Carregar páginas
        /// </summary>
        protected void CarregarPagina()
        {
            ConsultarNomeDba();
            divAlteracaoNome.Visible = false;
        }

        /// <summary>
        /// Buscar nome Dba
        /// </summary>
        /// <returns></returns>
        protected String ConsultarNomeDba()
        {
            String nomeDba = String.Empty;

            using (Logger log = Logger.IniciarLog("Consulta nome DBA."))
            {
                try
                {
                    if (!Sessao.Contem())
                        throw new Exception("Falha ao obter sessão.");

                    Int32 numeroPv = SessaoAtual.CodigoEntidade;

                    using (var ctx = new ContextoWCF<ServicoEAdquirenciaClient>())
                        nomeDba = ctx.Cliente.ConsultarNomeDba(numeroPv);

                    txtNomeDba.Text = nomeDba;
                    litNomeAnterior.Text = nomeDba;
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    ExibirMensagemErro(ex.Message);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    ExibirMensagemErro("Não foi possível realizar a consulta do nome na fatura. Por favor entre em contato com a Central de atendimento Rede.");
                }
            }

            return nomeDba;
        }

        /// <summary>
        /// Método para cadastrar Nome
        /// </summary>
        protected void CadastrarNome()
        {
            String nomeDba = txtNomeDba.Text;
            divAlteracaoNome.Visible = false;

            if (CadastrarNome(nomeDba))
            {
                litNomeAnterior.Text = nomeDba;
                ConsultarNomeDba();
                ExibirCadastroSucesso();
            }
        }

        /// <summary>
        /// Cadastrar novo nome
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        protected Boolean CadastrarNome(String nome)
        {
            using (Logger log = Logger.IniciarLog("Cadastrar nome DBA."))
            {
                try
                {
                    if (!Sessao.Contem())
                        throw new Exception("Falha ao obter sessão.");

                    Int32 numeroPv = SessaoAtual.CodigoEntidade;

                    using (var ctx = new ContextoWCF<ServicoEAdquirenciaClient>())
                        ctx.Cliente.DefineNomeDba(numeroPv, nome);

                    return true;
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    ExibirMensagemErro(ex.Message);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    ExibirMensagemErro("Não foi possível realizar o cadastro do nome na fatura. Por favor entre em contato com a Central de atendimento Rede.");
                }
            }

            return false;
        }

        /// <summary>
        /// Exibir mensagem de erro padrão
        /// </summary>
        protected void ExibirMensagemErro(String msg)
        {
            divAlteracaoNome.Visible = false;
            base.ExibirPainelMensagem(msg);
        }


        #endregion [Fim - Métodos]
    }
}
