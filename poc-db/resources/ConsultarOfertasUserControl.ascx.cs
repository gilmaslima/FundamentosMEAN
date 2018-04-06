/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.ServiceModel;
using System.Web;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.OutrosServicos.SharePoint.ControlTemplates.OutrosServicos.Ofertas;
using Redecard.PN.OutrosServicos.SharePoint.ZPPlanoContasServico;

namespace Redecard.PN.OutrosServicos.SharePoint.WebParts.ConsultarOfertas
{
    /// <summary>
    /// UserControl da WebPart de Consulta de Ofertas do módulo Serviços do Portal PN.
    /// </summary>
    public partial class ConsultarOfertasUserControl : UserControlBase
    {
        #region [ Propriedades / Atributos / Variáveis / Controles ]

        /// <summary>Nome da página Conta Certa</summary>
        private const String PaginaContaCerta = "ConsultarOfertasContaCerta.aspx";
        /// <summary>Nome da página Bônus Rede</summary>
        private const String PaginaBonusRede = "ConsultarOfertasBonus.aspx";
        /// <summary>Nome da página principal de ofertas</summary>
        private const String PaginaPrincipal = "pn_ConsultarOfertas.aspx";
        /// <summary>Nome da página Preço Único</summary>
        private const String paginaPrecoUnico = "ConsultarOfertasPrecoUnico.aspx";

        /// <summary>
        /// ucContaCerta control.
        /// </summary>
        protected ContaCerta UcContaCerta
        {
            get { return (ContaCerta)ucContaCerta; }
        }

        /// <summary>
        /// ucJapao control.
        /// </summary>
        protected JapaoBonusRede UcJapao
        {
            get { return (JapaoBonusRede)ucJapao; }
        }

        /// <summary>
        /// ucSemOfertas control.
        /// </summary>
        protected QuadroAviso UcSemOfertas
        {
            get { return (QuadroAviso)ucSemOfertas; }
        }

        /// <summary>
        /// ucTurquia control.
        /// </summary>
        protected TurquiaPrecoUnico UcTurquia
        {
            get { return (TurquiaPrecoUnico)ucTurquia; }
        }

        /// <summary>
        /// Modo de funcionamento da WebPart - Tipo da Oferta
        /// </summary>
        public String TipoOferta { get; set; }

        /// <summary>
        /// Tipo da Oferta na QueryString (para validação)
        /// </summary>
        private TipoOferta TipoOfertaQueryString
        {
            get
            {
                TipoOferta tipoOferta = ZPPlanoContasServico.TipoOferta.SemOferta;

                String dados = Request.QueryString["dados"];
                if (!String.IsNullOrEmpty(dados))
                {
                    var qs = new QueryStringSegura(dados);
                    tipoOferta = (TipoOferta)qs["TipoOferta"].ToInt32();
                }

                return tipoOferta;
            }
        }

        #endregion

        #region [ Eventos Páginas / Botões ]

        /// <summary>Page Load</summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("ConsultarOfertasUserControl.ascx - Page_Load"))
            {
                try
                {
                    if (Sessao.Contem())
                    {
                        if (!IsPostBack)
                        {
                            //Se TipoOferta está em branco, é a tela inicial de ofertas,
                            //que irá verificar qual a oferta ativa e redirecionar para a tela
                            //específica (utiliza esta mesma webpart)
                            switch (this.TipoOferta)
                            {
                                case "Oferta Bônus Rede":
                                    //Verifica navegação (usuário não pode digitar a URL diretamente)
                                    if (this.TipoOfertaQueryString == ZPPlanoContasServico.TipoOferta.OfertaJapao)
                                    {
                                        mvwOfertas.SetActiveView(pnlJapao);
                                        UcJapao.CarregarControle();
                                    }
                                    else
                                        Redirecionar(PaginaPrincipal);
                                    break;
                                case "Maquininha Rede Itaú":
                                    //Verifica navegação (usuário não pode digitar a URL diretamente)
                                    if (this.TipoOfertaQueryString == ZPPlanoContasServico.TipoOferta.OutrasOfertas)
                                    {
                                        mvwOfertas.SetActiveView(pnlContaCerta);
                                        UcContaCerta.CarregarControle();
                                    }
                                    else
                                        Redirecionar(PaginaPrincipal);
                                    break;

                                case "Preço Único":
                                    //Verifica navegação (usuário não pode digitar a URL diretamente)
                                    if (this.TipoOfertaQueryString == ZPPlanoContasServico.TipoOferta.OfertaTurquia)
                                    {
                                        mvwOfertas.SetActiveView(pnlTurquia);
                                        UcTurquia.CarregarControle();
                                    }
                                    else
                                        Redirecionar(PaginaPrincipal);
                                    break;

                                default:
                                    //Redireciona para oferta ativa (se existir) do estabelecimento
                                    Boolean existeOferta = RedirecionarParaOfertaAtiva();

                                    //Não existe oferta, exibe quadro de aviso
                                    if (!existeOferta)
                                    {
                                        mvwOfertas.SetActiveView(pnlSemOfertas);
                                        UcSemOfertas.CarregarMensagem();
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #endregion

        #region [ Carregamento de Controles ]

        /// <summary>
        /// Verifica qual a oferta ativa cadastrada para o estabelecimento
        /// Japão/Bônus Rede, Turquia, Plano de Contas/Conta Certa, Sem Ofertas, ...
        /// </summary>
        /// <returns>Flag indicando se existe uma oferta ou não (se houve redirecionamento)</returns>
        private Boolean RedirecionarParaOfertaAtiva()
        {
            TipoOferta tipoOferta = ConsultarTipoOfertaAtiva();
            String pagina = String.Empty;
            switch (tipoOferta)
            {
                case ZPPlanoContasServico.TipoOferta.OfertaJapao:
                    pagina = PaginaBonusRede;
                    break;
                case ZPPlanoContasServico.TipoOferta.OutrasOfertas:
                    pagina = PaginaContaCerta;
                    break;
                case ZPPlanoContasServico.TipoOferta.OfertaTurquia:
                    pagina = paginaPrecoUnico;
                    break;
                case ZPPlanoContasServico.TipoOferta.SemOferta:
                default:
                    return false;
            }

            //Monta URL de redirecionamento. Repassa o TipoOferta para validação via 
            //querystring (para evitar que o usuário entre diretamente na página de uma oferta específica)
            var qs = new QueryStringSegura();
            qs["TipoOferta"] = ((Int32)tipoOferta).ToString();
            String url = String.Format("{0}?dados={1}", pagina, qs.ToString());

            //Redireciona            
            Redirecionar(url);

            return true;
        }

        /// <summary>
        /// Redireciona para determinada URL
        /// </summary>
        /// <param name="url">URL da página</param>
        private void Redirecionar(String url)
        {
            Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        #endregion

        #region [ Consultas ]

        /// <summary>
        /// Consulta o tipo de oferta Ativa que deve ser exibida para o usuário
        /// Japão, Plano de Contas, Turquia, Sem Oferta, ...
        /// </summary>
        /// <returns>Tipo de oferta ativa</returns>
        private TipoOferta ConsultarTipoOfertaAtiva()
        {
            var codigoRetorno = default(Int16);
            var tipoOferta = default(TipoOferta);
            var numeroPv = SessaoAtual.CodigoEntidade;

            using (Logger Log = Logger.IniciarLog("Consulta tipo de oferta ativa"))
            {
                try
                {
                    using (var ctx = new ContextoWCF<HISServicoZP_PlanoContasClient>())
                        codigoRetorno = ctx.Cliente.ConsultarTipoOfertaAtiva(out tipoOferta, numeroPv);
                }
                catch (FaultException<ZPPlanoContasServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return codigoRetorno == 0 ? tipoOferta : ZPPlanoContasServico.TipoOferta.SemOferta;
        }

        #endregion
    }
}