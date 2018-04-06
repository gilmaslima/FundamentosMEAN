using Microsoft.SharePoint.Utilities;
using Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico;
using Rede.PN.Eadquirencia.Sharepoint.Helper;
using Rede.PN.Eadquirencia.Sharepoint.SvcHomolProAtiv;
using Redecard.PN.Comum;
using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Rede.PN.Eadquirencia.Sharepoint.Webparts.HomologarPV
{
    public partial class HomologarPVUserControl : WebpartBase
    {
        #region [Eventos]

        /// <summary>
        /// Evento Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            //Esconder popup de confirmação
            divConfirmacao.Visible = false;
        }

        /// <summary>
        /// Evento do botão confirmar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConfirmar_Click(object sender, CommandEventArgs e)
        {
            switch (e.CommandArgument.ToString())
            {
                case "Reativar":
                    ReativarHomologacaoPV();
                    return;
                default:
                    break;
            }

            RedirecionarGerenciamentoVendas();
        }

        /// <summary>
        /// Evento do botão nova homologação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNovaHomologacao_Click(object sender, EventArgs e)
        {
            // Consultar ponto de venda
            Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico.PontoDeVenda pontoVenda = ConsultarPV();
            CarregarTela(pontoVenda);
        }
        #endregion [Fim - Eventos]

        #region [Métodos]

        /// <summary>
        /// Redireciona para Gerenciamento de Vendas
        /// </summary>
        private void RedirecionarGerenciamentoVendas()
        {
            SPUtility.Redirect("/sites/fechado/erede/paginas/pn_configuracoes.aspx", SPRedirectFlags.Default, HttpContext.Current);
        }

        /// <summary>
        /// Carregar a tela conforme status do PV
        /// </summary>
        /// <param name="pontoVenda"></param>
        private void CarregarTela(Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico.PontoDeVenda pontoVenda)
        {
            // escondendo a div de confirmação
            divConfirmacao.Visible = false;

            // Verificar se foi possível obter o pv
            if (pontoVenda == null)
            {
                CarregarPopupAlerta("Atenção", "Falha ao obter os dados do ponto de venda. Por favor entre em contato com a Central de atendimento Rede.", "OK", "Redirecionar");
                return;
            }

            switch (pontoVenda.StatusPv)
            {
                case Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico.TipoStatusPv.Desenvolvimento:
                    CarregarPVDesenvolvimento();
                    break;
                case Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico.TipoStatusPv.Reativado:
                case Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico.TipoStatusPv.Homologado:
                    CarregarPVHomologado();
                    break;
                case Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico.TipoStatusPv.PendenteHomologacao:
                case Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico.TipoStatusPv.EmHomologacao:
                    CarregarProcessoEmHomologacao();
                    break;
                case Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico.TipoStatusPv.Cancelado:
                case Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico.TipoStatusPv.Suspenso:
                    ConfirmarProcessoHomologacao();
                    break;
                default:
                    CarregarPopupAlerta("Atenção", "Falha ao obter o status do ponto de venda. Por favor entre em contato com a Central de atendimento Rede.", "OK", "Redirecionar");
                    break;
            }
        }

        /// <summary>
        /// Carregar confirmação para iniciar processo de homologação
        /// </summary>
        private void ConfirmarProcessoHomologacao()
        {
            litTitulo.Text = "Atenção";
            litMensagem.Text = "Você irá reativar o processo de homologação do certificado de segurança SSL de sua loja virtual. Verifique se seu certificado está válido para que possa transacionar com a Rede. Deseja confirmar?";
            divConfirmacao.Visible = true;
            btnSim.Visible = true;
            btnSim.Text = "Sim";
            btnSim.CommandArgument = "Reativar";
            spanBtnNao.Visible = true;
            btnNao.CommandArgument = "NaoReativar";
            StringBuilder sb = new StringBuilder();
            this.ExibirPainelHtml(sb.ToString());
        }

        /// <summary>
        /// Carregar a tela para os pv's já estão em homologação
        /// </summary>
        private void CarregarProcessoEmHomologacao()
        {
            CarregarPopupAlerta("Atenção", "Sua loja virtual está em processo de homologação. Verifique se seu certificado de segurança SSL está válido para continuar transacionando conosco.", "OK", "Redirecionar");
        }

        /// <summary>
        /// Carregar a tela para os pv's já homologado
        /// </summary>
        private void CarregarPVHomologado()
        {
            CarregarPopupAlerta("Atenção", "Sua loja virtual já foi homologado e está de acordo com as normas de segurança solicitadas pela Rede. Mantenha seu certificado de segurança SSL válido, para continuar transacionando conosco.", "OK", "Redirecionar");
        }

        /// <summary>
        /// Carregar a tela para os pv's em desenvolvimento
        /// </summary>
        private void CarregarPVDesenvolvimento()
        {
            CarregarPopupAlerta("Atenção", "PV não pode ser homologado. Nenhuma transação encontrada.\n\n\n\n\n", "OK", "Redirecionar");
        }

        /// <summary>
        /// Carregar POPUP em formato alerta
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="msg"></param>
        /// <param name="textoBotao"></param>
        /// <param name="argumentoEvento"></param>
        private void CarregarPopupAlerta(String titulo, String msg, String textoBotao, String argumentoEvento)
        {
            litTitulo.Text = titulo;
            litMensagem.Text = msg;
            divConfirmacao.Visible = true;
            btnSim.Visible = true;
            btnSim.Text = textoBotao;
            btnSim.CommandArgument = argumentoEvento;
            spanBtnNao.Visible = false;
            btnNao.CommandArgument = "";
            StringBuilder sb = new StringBuilder();
            this.ExibirPainelHtml(sb.ToString());
        }

        /// <summary>
        /// Consulta o serviço do adquirência para verificar se o PV já está Homologado
        /// </summary>
        /// <returns></returns>
        private Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico.PontoDeVenda ConsultarPV()
        {
            Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico.PontoDeVenda pontoVenda = null;

            ExecucaoTratada("Consultar o PV - ConsultarPV", () =>
            {
                
                if (!Sessao.Contem())
                    throw new Exception("Falha ao obter sessão.");

                Int32 numeroPv = SessaoAtual.CodigoEntidade;
                using (var ctx = new ContextoWCF<ServicoEAdquirenciaClient>())
                    pontoVenda = ctx.Cliente.RecuperarDadosEC(numeroPv.ToString());
            });

            return pontoVenda;
        }

        /// <summary>
        /// Reativer homologacao do PV
        /// </summary>
        private void ReativarHomologacaoPV()
        {
            Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico.PontoDeVenda pontoVenda = ConsultarPV();
            divConfirmacao.Visible = false;
            if (pontoVenda.StatusPv != Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico.TipoStatusPv.Suspenso && pontoVenda.StatusPv != Rede.PN.Eadquirencia.Sharepoint.EadquirenciaServico.TipoStatusPv.Cancelado)
            {
                CarregarTela(pontoVenda);
                return;
            }

            ExecucaoTratada("Reativar homologação do PV - ReativarHomologacaoPV", () =>
            {

                if (!Sessao.Contem())
                    throw new Exception("Falha ao obter sessão.");

                Int32 numeroPv = SessaoAtual.CodigoEntidade;

                //Reativar homologação do PV
                using (var ctx = new ContextoWCF<ServicoHomologacaoProAtivaClient>())
                { 
                    ctx.Cliente.ReativarHomologacaoPV(numeroPv, Rede.PN.Eadquirencia.Sharepoint.SvcHomolProAtiv.PrioridadeHomologacao.Urgente, (Int32) SolicitanteHomologacao.Estabelecimento);
                    CarregarPopupAlerta("Atenção", "Sua homologação foi reativada com sucesso, a Rede validará seu certificado SSL novamente.", "OK", "Redirecionar");
                }
            });
        }

        #endregion [Fim - Métodos]
    }
}
