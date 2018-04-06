/* © Copyright 2014 Rede S.A. 
 * Autor : Valdinei Ribeiro
 * Empresa : Iteris Consultoria e Software
 * Histórico: 
 * - [19/01/2015] Change Request Turquia - Alexandre Shiroma
 * - [10/04/2071] Facelift preço único - Jacques Sá
*/

using Redecard.PN.Comum;
using Redecard.PN.OutrosServicos.SharePoint.Business;
using Redecard.PN.OutrosServicos.SharePoint.NkPlanoContasServico;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace Redecard.PN.OutrosServicos.SharePoint.ControlTemplates.OutrosServicos.Ofertas
{
    /// <summary>
    /// UserControl da WebPart da Consulta de Preço Único / Projeto Turquia.
    /// </summary>
    public partial class TurquiaPrecoUnico : UserControlBase
    {
        #region [ Propriedades / Variáveis / Atributos ]

        /// <summary>CultureInfo pt-BR</summary>
        private readonly CultureInfo ptBr = CultureInfo.CreateSpecificCulture("pt-BR");

        /// <summary>TextInfo</summary>
        private readonly TextInfo textInfo = new CultureInfo("pt-BR", false).TextInfo;

        /// <summary>
        /// Lista de planos preço único
        /// </summary>
        private List<PlanoPrecoUnico> PlanosPrecoUnico
        {
            get 
            {
                // consulta a view state para não onerar a camada de serviços
                if (ViewState["PlanosPrecoUnico"] == null)
                    ViewState["PlanosPrecoUnico"] = this.ConsultarOfertas();

                return (List<PlanoPrecoUnico>)ViewState["PlanosPrecoUnico"]; 
            }
        }

        #endregion

        #region [ Métodos Públicos/Expostos ]

        /// <summary>
        /// Carregamento inicial do controle
        /// </summary>
        public void CarregarControle()
        {
            if (!IsPostBack)
            {
                // carregamento dos dados de preço único
                this.CarregarOfertas();

                // carregamento do combo do mês de consulta preço único.
                this.CarregarCombosFiltro();
            }

            // dados do usuário no header do bloco de impressão
            if (this.SessaoAtual != null)
                this.ltrHeaderImpressaoUsuario.Text = string.Concat(SessaoAtual.CodigoEntidade, " / ", SessaoAtual.LoginUsuario);
        }

        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Carrega na tela as informações de planos preço único.
        /// </summary>
        private void CarregarOfertas()
        {
            if (this.PlanosPrecoUnico.Count > 0)
            {
                // obtém os dados para visualização
                var planosPrecoUnicoView = 
                    PrecoUnicoBusiness.GetPrecoUnicoViewData(this.PlanosPrecoUnico, SessaoAtual.CodigoEntidade);

                // serializa os dados do preço único para carregamento em client-side
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                this.hdnDadosPrecoUnico.Value = serializer.Serialize(planosPrecoUnicoView);
            }
            else
            {
                mnuAcoes.Visible = false;
                pnlPlanoPrecoUnico.Visible = false;
                pnlSemApuracaoRealizada.Visible = true;
            }
        }

        /// <summary>
        /// Carrega o combo do mês de referência da consulta de preço único.
        /// </summary>
        private void CarregarCombosFiltro()
        {
            if (this.PlanosPrecoUnico != null && this.PlanosPrecoUnico.Count > 0)
            {
                List<DateTime> listaAnoMesReferencia = 
                    this.PlanosPrecoUnico.Select(plano => plano.AnoMesReferencia).ToList();

                // configura a combo de mês
                ddlMesConsulta.DataTextField = "Mes";
                ddlMesConsulta.DataValueField = "MesValor";
                ddlMesConsulta.DataSource =
                    listaAnoMesReferencia.Select(anoMesRef =>
                    {
                        return new
                        {
                            Mes = anoMesRef.ToString("MMMM", ptBr),
                            MesValor = anoMesRef.ToString("MM", ptBr)
                        };

                    }).Distinct().OrderBy(x => x.MesValor);
                ddlMesConsulta.DataBind();

                // configura a combo de ano
                ddlAnoConsulta.DataSource =
                    listaAnoMesReferencia.Select(anoMesRef => anoMesRef.ToString("yyyy", ptBr)).Distinct().OrderByDescending(x => x);
                ddlAnoConsulta.DataBind();
            }

            ddlMesConsulta.Items.Insert(0, new ListItem("todos", ""));
            ddlAnoConsulta.Items.Insert(0, new ListItem("selecione", ""));
        }

        #endregion

        #region [ Consulta WCF ]

        /// <summary>
        /// Consulta os planos Preço Único contratados para o PV (Ponto de Venda) da sessão atual.
        /// </summary>
        /// <returns></returns>
        private List<PlanoPrecoUnico> ConsultarOfertas()
        {
            using (Logger log = Logger.IniciarLog("Consulta planos Preço Único contratados"))
            {
                Int16 codigoRetorno = default(Int16);
                Int32 numeroPv = SessaoAtual.CodigoEntidade;
                List<PlanoPrecoUnico> ofertas = new List<PlanoPrecoUnico>();

                try
                {
                    // efetua a chamada do seviço somente no caso de não existir o objeto na ViewState.
                    using (var ctx = new ContextoWCF<HisServicoNkPlanoContasClient>())
                        ofertas = ctx.Cliente.ConsultarPlanosPrecoUnicoContratados(out codigoRetorno, numeroPv);

                    // se código 60, exibe mensagem customizada de Sem Apuração Realizada (CR Turquia)
                    if (codigoRetorno == 60)
                    {
                        this.mnuAcoes.Visible = false;
                        this.pnlPlanoPrecoUnico.Visible = false;
                        this.pnlSemApuracaoRealizada.Visible = true;
                    }
                    else if (codigoRetorno != 0)
                    {
                        base.ExibirPainelExcecao("NKPlanoContasServico.ConsultaPlanosPrecoUnicoContratados", codigoRetorno);
                    }
                }
                catch (FaultException<NkPlanoContasServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }

                return ofertas;
            }
        }

        #endregion
    }
}
