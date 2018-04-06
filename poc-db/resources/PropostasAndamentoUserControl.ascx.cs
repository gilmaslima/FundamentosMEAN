using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Sharepoint.WFProposta;
using Redecard.PN.Credenciamento.Sharepoint.WFEnderecos;
using Redecard.PN.Credenciamento.Sharepoint.WFTecnologia;
using Redecard.PN.Credenciamento.Sharepoint.WFProdutos;
using Redecard.PN.Credenciamento.Sharepoint.WFDomBancario;
using Redecard.PN.Credenciamento.Sharepoint.WFServicos;
using Redecard.PN.Credenciamento.Sharepoint.WFProprietarios;
using Redecard.PN.Credenciamento.Sharepoint.GEPontoVen;
using Redecard.PN.Credenciamento.Sharepoint.GEProdutos;
using Redecard.PN.Credenciamento.Sharepoint.GERegimes;
using Redecard.PN.Credenciamento.Sharepoint.Modelo;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using Redecard.PN.Credenciamento.Sharepoint.GEProprietarios;
using Redecard.PN.Credenciamento.Sharepoint.WFSerasa;
using Redecard.PN.Credenciamento.Sharepoint.GEDomBancario;

namespace Redecard.PN.Credenciamento.Sharepoint.WebParts.PropostasAndamento
{
    public partial class PropostasAndamentoUserControl : UserControlCredenciamentoBase
    {
        #region [ Propriedades ]

        /// <summary>
        /// Propostas
        /// </summary>
        public List<PropostaPendente> Propostas = new List<PropostaPendente>();

        /// <summary>
        /// Lista de Dados dos Produtos Crédito
        /// </summary>
        public List<ProdutosListaDadosFeaturesPorCCA> DadosProdutosCredito
        {
            get
            {
                if (ViewState["DadosProdutosCredito"] == null)
                    ViewState["DadosProdutosCredito"] = new List<ProdutosListaDadosFeaturesPorCCA>();

                return (List<ProdutosListaDadosFeaturesPorCCA>)ViewState["DadosProdutosCredito"];
            }
            set
            {
                ViewState["DadosProdutosCredito"] = value;
            }
        }

        /// <summary>
        /// Lista de Dados dos Produtos Débito
        /// </summary>
        public List<ProdutosListaDadosFeaturesPorCCA> DadosProdutosDebito
        {
            get
            {
                if (ViewState["DadosProdutosDebito"] == null)
                    ViewState["DadosProdutosDebito"] = new List<ProdutosListaDadosFeaturesPorCCA>();

                return (List<ProdutosListaDadosFeaturesPorCCA>)ViewState["DadosProdutosDebito"];
            }
            set
            {
                ViewState["DadosProdutosDebito"] = value;
            }
        }

        /// <summary>
        /// Lista de Dados dos Produtos Construcard
        /// </summary>
        public List<ProdutosListaDadosFeaturesPorCCA> DadosProdutosConstrucard
        {
            get
            {
                if (ViewState["DadosProdutosConstrucard"] == null)
                    ViewState["DadosProdutosConstrucard"] = new List<ProdutosListaDadosFeaturesPorCCA>();

                return (List<ProdutosListaDadosFeaturesPorCCA>)ViewState["DadosProdutosConstrucard"];
            }
            set
            {
                ViewState["DadosProdutosConstrucard"] = value;
            }
        }

        #endregion

        #region [ Eventos da Página ]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Page.MaintainScrollPositionOnPostBack = true;

                if (!Page.IsPostBack)
                {
                    CarregarPropostas();
                    CarregarPVs();

                    DadosProdutosCredito = ListaDadosFeaturePorCca(1);
                    DadosProdutosDebito = ListaDadosFeaturePorCca(5);
                    DadosProdutosDebito.AddRange(ListaDadosFeaturePorCca(14));
                    DadosProdutosConstrucard = ListaDadosFeaturePorCca(22);

                    if (Propostas.Count == 0)
                    {
                        ConsultaProximaSequencia();
                        //Credenciamento.ProdutosCredito = ListaDadosProdutosPorPontoDeVenda(new PropostaPendente { NroEstabelecimento = (Int32)Credenciamento.NumPdvMatriz }, 'C');
                        //Credenciamento.ProdutosDebito = ListaDadosProdutosPorPontoDeVenda(new PropostaPendente { NroEstabelecimento = (Int32)Credenciamento.NumPdvMatriz }, 'D').FindAll(p => p.CodCCA != 22);
                        //Credenciamento.ProdutosConstrucard = ListaDadosProdutosPorPontoDeVenda(new PropostaPendente { NroEstabelecimento = (Int32)Credenciamento.NumPdvMatriz }, 'D').FindAll(p => p.CodCCA == 22);
                        CarregarDadosDomicilioBancarioCreditoPorPontoVenda(new PropostaPendente { NroEstabelecimento = (Int32)Credenciamento.NumPdvMatriz });

                        CarregarDadosPorPontoVenda(new PropostaPendente { NroEstabelecimento = (Int32)Credenciamento.NumPdvMatriz });
                        ListaProprietariosPontoVenda(new PropostaPendente { NroEstabelecimento = (Int32)Credenciamento.NumPdvMatriz });
                        Credenciamento.RecuperadaGE = true;

                        Response.Redirect("pn_dadoscliente.aspx", false);
                    }

                    gvPropostas.DataSource = Propostas;
                    gvPropostas.DataBind();
                }
            }
            catch (FaultException<WFProdutos.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Propostas em Andamento", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, fe.Detail.CodigoErro.ToString());
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Propostas em Andamento", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Propostas em Andamento", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Propostas em Andamento", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        #endregion

        #region [ Eventos de Controles ]

        /// <summary>
        /// Evento do botão Continuar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            try
            {
                Boolean existeSelecao;
                PropostaPendente propostaSelecionada = GetSelectedRow(out existeSelecao);

                if (existeSelecao)
                {
                    if (propostaSelecionada.StatusProposta == 1)
                    {
                        Credenciamento.CodTipoMovimento = 'A';
                        CarregarPropostaEmAndamento(propostaSelecionada);
                        CarregarDadosEnderecos(propostaSelecionada);
                        CarregarDadosTecnologia(propostaSelecionada);
                        //Credenciamento.ProdutosVan = CarregarProdutosVan(propostaSelecionada);
                        CarregarDadosDomicilioBancario(propostaSelecionada, 1);
                        CarregarDadosDomicilioBancario(propostaSelecionada, 3);
                        CarregarDadosDomicilioBancario(propostaSelecionada, 4);
                        CarregarDadosProprietarios(propostaSelecionada);
                        CarregarDadosServicos(propostaSelecionada);
                        CarregarSerasa();

                        //if (Credenciamento.CodTipoEstabelecimento == 1)
                        //{
                        //    Credenciamento.ProdutosCredito = CarregarDadosProdutos(propostaSelecionada, 'C', null, null);
                        //    Credenciamento.ProdutosDebito = CarregarDadosProdutos(propostaSelecionada, 'D', null, null).FindAll(p => p.CodCCA != 22);
                        //    Credenciamento.ProdutosConstrucard = CarregarDadosProdutos(propostaSelecionada, 'D', 22, 21);
                        //}
                    }
                    else
                    {
                        ConsultaProximaSequencia();
                        ListaProprietariosPontoVenda(propostaSelecionada);

                        if (propostaSelecionada.StatusProposta == 2)
                        {
                            Credenciamento.CodTipoMovimento = 'D';
                            Credenciamento.Recredenciamento = true;
                            CarregarDadosRecredenciamento(propostaSelecionada);
                            CarregarDadosDomicilioBancarioCreditoPorPontoVenda(propostaSelecionada);
                            CarregarSerasa();
                        }
                        else if (propostaSelecionada.StatusProposta == 3)
                        {
                            //Credenciamento.ProdutosCredito = ListaDadosProdutosPorPontoDeVenda(propostaSelecionada, 'C');
                            //Credenciamento.ProdutosDebito = ListaDadosProdutosPorPontoDeVenda(propostaSelecionada, 'D').FindAll(p => p.CodCCA != 22);
                            //Credenciamento.ProdutosConstrucard = ListaDadosProdutosPorPontoDeVenda(propostaSelecionada, 'D').FindAll(p => p.CodCCA == 22);
                            CarregarDadosDomicilioBancarioCreditoPorPontoVenda(propostaSelecionada);
                            Credenciamento.CodTipoMovimento = 'U';
                            GravaInfoTipoEstabCredenciamento();
                            CarregarDadosPorPontoVenda(propostaSelecionada);
                            Credenciamento.RecuperadaGE = true;
                            Credenciamento.Duplicacao = true;
                        }
                    }

                    Response.Redirect("pn_dadoscliente.aspx", false);

                    //switch (Credenciamento.Fase)
                    //{
                    //    case 1: Response.Redirect("pn_dadoscliente.aspx", false);
                    //        break;
                    //    case 2: Response.Redirect("pn_dadosnegocio.aspx", false);
                    //        break;
                    //    case 3: Response.Redirect("pn_dadosendereco.aspx", false);
                    //        break;
                    //    case 4: Response.Redirect("pn_dadosoperacionais.aspx", false);
                    //        break;
                    //    case 5: Response.Redirect("pn_dadosbancarios.aspx", false);
                    //        break;
                    //    case 6: Response.Redirect("pn_escolhatecnologia.aspx", false);
                    //        break;
                    //    case 7: Response.Redirect("pn_contracaoservicos.aspx", false);
                    //        break;
                    //    default: Response.Redirect("pn_dadoscliente.aspx", false);
                    //        break;
                    //}

                    //if (Credenciamento.Fase != null && Credenciamento.Fase >= 2 && String.Compare(Credenciamento.EnderecoComercial.CEP, Credenciamento.CEP) != 0)
                    //    Response.Redirect("pn_dadosnegocio.aspx", false);
                }
                else
                    base.ExibirPainelExcecao("Selecione uma proposta pendente", "300");

            }
            catch (FaultException<GEProprietarios.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Propostas em Andamento", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, fe.Detail.CodErro.ToString());
            }
            catch (FaultException<GEPontoVen.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Propostas em Andamento", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, fe.Detail.CodErro.ToString());
            }
            catch (FaultException<WFProprietarios.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Propostas em Andamento", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, fe.Detail.CodigoErro.ToString());
            }
            catch (FaultException<WFTecnologia.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Propostas em Andamento", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, fe.Detail.CodigoErro.ToString());
            }
            catch (FaultException<WFEnderecos.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Propostas em Andamento", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, fe.Detail.CodigoErro.ToString());
            }
            catch (FaultException<WFProposta.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Propostas em Andamento", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, fe.Detail.CodigoErro.ToString());
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Propostas em Andamento", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Propostas em Andamento", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Propostas em Andamento", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Data Bound das linhas da grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPropostas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ((Label)e.Row.FindControl("lblEstabelecimento")).Text = ((PropostaPendente)e.Row.DataItem).NroEstabelecimento.ToString();
                    Label cpf_cnpj = ((Label)e.Row.FindControl("lblCNPJ"));
                    if (((PropostaPendente)e.Row.DataItem).TipoPessoa == 'J')
                        cpf_cnpj.Text = String.Format(@"{0:00\.000\.000\/0000\-00}", ((PropostaPendente)e.Row.DataItem).CNPJ);
                    else
                        cpf_cnpj.Text = String.Format(@"{0:000\.000\.000\-00}", ((PropostaPendente)e.Row.DataItem).CNPJ);

                    ((Label)e.Row.FindControl("lblRazaoSocial")).Text = ((PropostaPendente)e.Row.DataItem).RazaoSocial;
                    ((Label)e.Row.FindControl("lblRamo")).Text = ((PropostaPendente)e.Row.DataItem).Ramo;
                    Label tipoEstabelecimento = (Label)e.Row.FindControl("lblTipoEstabelecimento");
                    if (((PropostaPendente)e.Row.DataItem).TipoEstabelecimento == 0)
                        tipoEstabelecimento.Text = "Autônomo";
                    else if (((PropostaPendente)e.Row.DataItem).TipoEstabelecimento == 1)
                        tipoEstabelecimento.Text = "Filial";
                    else
                        tipoEstabelecimento.Text = "Matriz";

                    ((Label)e.Row.FindControl("lblCategoria")).Text = ((PropostaPendente)e.Row.DataItem).Categoria.ToString();
                    ((Label)e.Row.FindControl("lblEndereco")).Text = ((PropostaPendente)e.Row.DataItem).EnderecoComercial;

                    Image status = (Image)e.Row.FindControl("imgSituacao");
                    if (((PropostaPendente)e.Row.DataItem).StatusProposta == 1)
                        status.ImageUrl = "../../../_layouts/Redecard.Comum/Images/sinal_amarelo.png";
                    else if (((PropostaPendente)e.Row.DataItem).Categoria == 'X' || ((PropostaPendente)e.Row.DataItem).Categoria == 'E')
                        status.ImageUrl = "../../../_layouts/Redecard.Comum/Images/sinal_vermelho.png";
                    else
                        status.ImageUrl = "../../../_layouts/Redecard.Comum/Images/sinal_verde.png";

                    ((HiddenField)e.Row.FindControl("hiddenNumSeq")).Value = ((PropostaPendente)e.Row.DataItem).NumSequencia.ToString();
                    ((HiddenField)e.Row.FindControl("hiddenStatus")).Value = ((PropostaPendente)e.Row.DataItem).StatusProposta.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Propostas em Andamento", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }

        }

        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Carrega lista de propostas
        /// </summary>
        private void CarregarPropostas()
        {
            ServicoPortalWFPropostaClient client = new ServicoPortalWFPropostaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega dados das propostas"))
                {
                    Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                    Int64 numCNPJ = 0;
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                    else
                        Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                    Int32? numSequencia = null;

                    PropostasCNPJ[] propostas = client.ConsPropCredenciamentoPendente(codTipoPessoa, numCNPJ, numSequencia);
                    client.Close();

                    foreach (PropostasCNPJ proposta in propostas)
                    {
                        Propostas.Add(new PropostaPendente
                        {
                            NroEstabelecimento = proposta.NumPontoVenda,
                            CNPJ = proposta.NumCNPJCPF,
                            TipoPessoa = proposta.CodTipoPessoa,
                            RazaoSocial = proposta.RazaoSocial,
                            Ramo = proposta.CodigoGrupoRamo + String.Format(@"{0:0000}", proposta.CodRamoAtividade),
                            TipoEstabelecimento = proposta.CodTipoEstabelecimento,
                            Categoria = ' ',
                            EnderecoComercial = String.Format("{0}, {1}", proposta.Logradouro, proposta.NumEndereco),
                            StatusProposta = 1,
                            NumSequencia = proposta.NumSequencia
                        });
                    }
                }
            }
            catch (FaultException<WFProposta.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Propostas em Andamento", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, (Int32)fe.Detail.CodigoErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Propostas em Andamento", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Propostas em Andamento", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Propostas em Andamento", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega lista de PVs
        /// </summary>
        private void CarregarPVs()
        {
            ServicoPortalGEPontoVendaClient client = new ServicoPortalGEPontoVendaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega lista de PVs"))
                {
                    Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                    Int64 numCNPJ = 0;
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                    else
                        Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                    PontoVendaListaCadastroReduzidoPorCNPJ[] pontosVenda = client.ListaCadastroReduzidoPorCNPJ(codTipoPessoa, numCNPJ);
                    client.Close();

                    foreach (PontoVendaListaCadastroReduzidoPorCNPJ pontoVenda in pontosVenda)
                    {
                        Propostas.Add(new PropostaPendente
                        {
                            NroEstabelecimento = pontoVenda.NumPdv,
                            TipoPessoa = pontoVenda.CodTipoPessoa,
                            CNPJ = pontoVenda.NumCNPJ,
                            RazaoSocial = pontoVenda.RazaoSocial,
                            Ramo = String.Format(@"{0}{1:0000}", pontoVenda.CodGrupoRamo, pontoVenda.CodRamoAtivididade),
                            TipoEstabelecimento = pontoVenda.CodTipoEstabelecimento,
                            Categoria = pontoVenda.CodCategoria,
                            EnderecoComercial = String.Format("{0}, {1}", pontoVenda.NomeLogradouro, pontoVenda.NumLogradouro),
                            StatusProposta = pontoVenda.CodCategoria == 'X' || pontoVenda.CodCategoria == 'E' ? 2 : 3
                        });
                    }
                }
            }
            catch (FaultException<GEPontoVen.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Propostas em Andamento", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Propostas em Andamento", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Propostas em Andamento", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Propostas em Andamento", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega dados de uma proposta na sessão
        /// </summary>
        private void CarregarPropostaEmAndamento(PropostaPendente proposta)
        {
            ServicoPortalWFPropostaClient client = new ServicoPortalWFPropostaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega dados da proposta selecionada"))
                {
                    if (proposta.TipoPessoa != null && proposta.CNPJ != null && proposta.NumSequencia != null)
                    {
                        Char tipoPessoa = (Char)proposta.TipoPessoa;
                        Int64 cpf_cnpj = (Int64)proposta.CNPJ;
                        Int32 numSeq = (Int32)proposta.NumSequencia;

                        PropostaPorCNPJCPF[] dados = client.ConsultaPropostaPorCNPJCPF(tipoPessoa, cpf_cnpj, numSeq);
                        client.Close();

                        if (dados.Length > 0)
                        {
                            Credenciamento.TipoPessoa = dados[0].CodTipoPessoa.ToString();
                            if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                            {
                                Credenciamento.CNPJ = String.Format(@"{0:00\.000\.000\/0000\-00}", dados[0].NumCnpjCpf);
                                Credenciamento.DataFundacao = (DateTime)dados[0].DataFundacao;
                                Credenciamento.RazaoSocial = dados[0].RazaoSocial;
                                Credenciamento.GrupoRamo = (Int32)dados[0].CodGrupoRamo;
                                Credenciamento.RamoAtividade = (Int32)dados[0].CodRamoAtividade;
                            }
                            else
                            {
                                Credenciamento.CPF = String.Format(@"{0:000\.000\.000\-00}", dados[0].NumCnpjCpf);
                                Credenciamento.DataNascimento = (DateTime)dados[0].DataFundacao;
                                Credenciamento.NomeCompleto = dados[0].RazaoSocial;
                            }

                            Credenciamento.NumSequencia = dados[0].IndSeqProp;
                            Credenciamento.NumPdv = dados[0].NumPdv;
                            Credenciamento.EndInstIgualComer = (Char)dados[0].IndEnderecoIgualCom;
                            Credenciamento.PessoaContato = dados[0].PessoaContato;
                            Credenciamento.NomeEmail = dados[0].NomeEmail;
                            Credenciamento.NomeHomePage = dados[0].NomeHomePage;
                            Credenciamento.NumDDD1 = dados[0].NumDDD1;
                            Credenciamento.NumTelefone1 = dados[0].NumTelefone1;
                            Credenciamento.Ramal1 = dados[0].Ramal1;
                            Credenciamento.NumDDD2 = dados[0].NumDDD2;
                            Credenciamento.NumTelefone2 = dados[0].NumTelefone2;
                            Credenciamento.Ramal2 = dados[0].Ramal2;
                            Credenciamento.NumDDDFax = dados[0].NumDDDFax;
                            Credenciamento.NumTelefoneFax = dados[0].NumTelefoneFax;
                            Credenciamento.CodFilial = dados[0].CodFilial;
                            Credenciamento.CodGerencia = dados[0].CodGerencia;
                            Credenciamento.CodCarteira = dados[0].CodCarteira;
                            Credenciamento.CodZonaVenda = dados[0].CodZona;
                            Credenciamento.HorarioFuncionamento = dados[0].CodHoraFuncionamentoPV;
                            Credenciamento.CodTipoEstabelecimento = dados[0].CodTipoEstabelecimento;
                            Credenciamento.NumPdvMatriz = dados[0].NumeroMatriz;
                            Credenciamento.NomeFatura = dados[0].NomeFatura;
                            Credenciamento.GrupoComercial = dados[0].NumGrupoComercial;
                            Credenciamento.GrupoGerencial = dados[0].NumGrupoGerencial;
                            Credenciamento.LocalPagamento = dados[0].CodLocalPagamento;
                            Credenciamento.Centralizadora = dados[0].NumCentralizadora;

                            if (Credenciamento.Canal != (Int32)dados[0].CodCanal || Credenciamento.Celula != (Int32)dados[0].CodCelula)
                                Credenciamento.RefazerNegociacao = true;

                            Credenciamento.DataCadastroProposta = (DateTime)dados[0].DataCadastroProposta;
                            Credenciamento.CPFVendedor = dados[0].NumCPFVendedor;
                            //Credenciamento.TaxaAdesao = (Double)dados[0].ValorTaxaAdesao;
                            Credenciamento.EDV = dados[0].CodigoEVD;
                            Credenciamento.CNAE = dados[0].CodigoCNAE;
                            Credenciamento.IndExtratoEmail = dados[0].IndEnvioExtratoEmail;
                            Credenciamento.CodCampanha = dados[0].CodigoCampanha;
                            Credenciamento.Fase = dados[0].CodFaseFiliacao;
                            Credenciamento.IndRegiaoLoja = dados[0].IndRegiaoLoja != null ? (Char)dados[0].IndRegiaoLoja : 'R';
                            Credenciamento.NumSolicitacao = dados[0].NumOcorrencia;
                        }
                    }
                }
            }
            catch (FaultException<WFProposta.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Carregar dados de endereço
        /// </summary>
        /// <returns></returns>
        private void CarregarDadosEnderecos(PropostaPendente proposta)
        {
            ServicoPortalWFEnderecosClient client = new ServicoPortalWFEnderecosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega dados dos endereços da proposta"))
                {
                    Char tipoPessoa = (Char)proposta.TipoPessoa;
                    Int64 cpf_cnpj = (Int64)proposta.CNPJ;
                    Int32 numSeq = (Int32)proposta.NumSequencia;

                    ConsultaEnderecos[] enderecos = client.ConsultaEnderecos(tipoPessoa, cpf_cnpj, numSeq, null);
                    client.Close();

                    if (enderecos.Length > 0)
                    {
                        foreach (ConsultaEnderecos endereco in enderecos)
                        {
                            if (endereco.IndTipoEndereco == '1')
                            {
                                Credenciamento.EnderecoComercial = new Endereco();
                                Credenciamento.EnderecoComercial.CEP = String.Format("{0}-{1}", endereco.CodigoCep, endereco.CodComplementoCep);
                                Credenciamento.EnderecoComercial.Logradouro = endereco.Logradouro.Trim();
                                Credenciamento.EnderecoComercial.Complemento = endereco.ComplementoEndereco.Trim();
                                Credenciamento.EnderecoComercial.Estado = endereco.Estado.Trim();
                                Credenciamento.EnderecoComercial.Cidade = endereco.Cidade.Trim();
                                Credenciamento.EnderecoComercial.Bairro = endereco.Bairro.Trim();
                                Credenciamento.EnderecoComercial.Numero = endereco.NumeroEndereco.Trim();
                            }

                            if (endereco.IndTipoEndereco == '2')
                            {
                                Credenciamento.EnderecoCorrespondencia = new Endereco();
                                Credenciamento.EnderecoCorrespondencia.CEP = String.Format("{0}-{1}", endereco.CodigoCep, endereco.CodComplementoCep);
                                Credenciamento.EnderecoCorrespondencia.Logradouro = endereco.Logradouro.Trim();
                                Credenciamento.EnderecoCorrespondencia.Complemento = endereco.ComplementoEndereco.Trim();
                                Credenciamento.EnderecoCorrespondencia.Estado = endereco.Estado.Trim();
                                Credenciamento.EnderecoCorrespondencia.Cidade = endereco.Cidade.Trim();
                                Credenciamento.EnderecoCorrespondencia.Bairro = endereco.Bairro.Trim();
                                Credenciamento.EnderecoCorrespondencia.Numero = endereco.NumeroEndereco.Trim();
                            }

                            if (endereco.IndTipoEndereco == '4')
                            {
                                Credenciamento.EnderecoInstalacao = new Endereco();
                                Credenciamento.EnderecoInstalacao.CEP = String.Format("{0}-{1}", endereco.CodigoCep, endereco.CodComplementoCep);
                                Credenciamento.EnderecoInstalacao.Logradouro = endereco.Logradouro.Trim();
                                Credenciamento.EnderecoInstalacao.Complemento = endereco.ComplementoEndereco.Trim();
                                Credenciamento.EnderecoInstalacao.Estado = endereco.Estado.Trim();
                                Credenciamento.EnderecoInstalacao.Cidade = endereco.Cidade.Trim();
                                Credenciamento.EnderecoInstalacao.Bairro = endereco.Bairro.Trim();
                                Credenciamento.EnderecoInstalacao.Numero = endereco.NumeroEndereco.Trim();
                            }

                            if (Credenciamento.CEP != Credenciamento.EnderecoComercial.CEP)
                                Credenciamento.RefazerNegociacao = true;
                        }
                    }
                }
            }
            catch (FaultException<WFEnderecos.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Carrega dados da tecnologia
        /// </summary>
        /// <param name="proposta"></param>
        private void CarregarDadosTecnologia(PropostaPendente proposta)
        {
            ServicoPortalWFTecnologiaClient client = new ServicoPortalWFTecnologiaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega dados da tecnologia"))
                {
                    Char tipoPessoa = (Char)proposta.TipoPessoa;
                    Int64 cpf_cnpj = (Int64)proposta.CNPJ;
                    Int32 numSeq = (Int32)proposta.NumSequencia;

                    ConsultaTecnologia[] tecnologias = client.ConsultaTecnologia(tipoPessoa, cpf_cnpj, numSeq);
                    client.Close();

                    if (tecnologias.Length > 0)
                    {
                        //Credenciamento.CodTipoEquipamento = tecnologias[0].CodTipoEquipamento;
                        //Credenciamento.TipoEquipamento = tecnologias[0].CodTipoEquipamento;
                        //Credenciamento.QtdeTerminaisSolicitados = (Int32)tecnologias[0].QtdeTerminalSolicitado;
                        //Credenciamento.IndVendaDigitada = tecnologias[0].IndHabilitaVendaDigitada;
                        //Credenciamento.EndInstIgualComer = (Char)tecnologias[0].IndEnderecoIgualComercial;
                        //Credenciamento.NomeContatoInstalacao = tecnologias[0].NomeContato.Trim();
                        //Credenciamento.NumDDDInstalacao = tecnologias[0].NumDDD.Trim();
                        //Credenciamento.NumTelefoneInstalacao = (Int32)tecnologias[0].NumTelefone;
                        //Credenciamento.RamalInstalacao = (Int32)tecnologias[0].NumRamal;
                        //Credenciamento.CodSoftwareTEF = tecnologias[0].CodFornecedorSoftware;
                        //Credenciamento.NomeSoftwareTEF = tecnologias[0].NomeFornecedorSoftware;
                        //Credenciamento.CodMarcaPDV = tecnologias[0].CodFabricanteHardware;
                        //Credenciamento.NomeMarcaPDV = tecnologias[0].NomeFabricanteHardware;
                        //Credenciamento.NroRenpac = (Int32)tecnologias[0].NumeroRenpac;
                        //Credenciamento.DiaInicioFuncionamento = (Int32)tecnologias[0].DiaInicioFuncionamento;
                        //Credenciamento.DiaFimFuncionamento = (Int32)tecnologias[0].DiaFimFuncionamento;
                        //Credenciamento.HoraInicioFuncionamento = (Int32)tecnologias[0].HoraInicioFuncionamento;
                        //Credenciamento.HoraFimFuncionamento = (Int32)tecnologias[0].HoraFimFuncionamento;
                        //Credenciamento.ValorAluguel = (Double)tecnologias[0].ValorEquipamento;
                        //Credenciamento.CodCenario = tecnologias[0].CodigoCenario;
                        //Credenciamento.CodEvento = tecnologias[0].CodigoEventoEspecial;
                        Credenciamento.Observacao = tecnologias[0].Observacao;
                    }
                }
            }
            catch (FaultException<WFTecnologia.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Carregar dados produtos
        /// </summary>
        private List<ProdutosListaDadosProdutosPorRamoCanal> CarregarDadosProdutos(PropostaPendente proposta, Char tipoProduto, Int32? codCca, Int32? codFeature)
        {
            List<ProdutosListaDadosProdutosPorRamoCanal> retorno = new List<ProdutosListaDadosProdutosPorRamoCanal>();
            ServicoPortalWFProdutosClient client = new ServicoPortalWFProdutosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega dados de Produtos"))
                {
                    Char tipoPessoa = (Char)proposta.TipoPessoa;
                    Int64 cpf_cnpj = (Int64)proposta.CNPJ;
                    Int32 numSeq = (Int32)proposta.NumSequencia;

                    ConsultaProdutos[] produtos = client.ConsultaProdutos(tipoPessoa, cpf_cnpj, numSeq, tipoProduto, codCca, codFeature);
                    client.Close();

                    foreach (ConsultaProdutos produto in produtos)
                    {
                        if (produto.CodFeature == 3)
                        {
                            if (Credenciamento.Patamares == null)
                                Credenciamento.Patamares = new List<Patamar>();

                            Credenciamento.Patamares.Add(new Patamar
                            {
                                CodCca = (Int32)produto.CodCca,
                                CodFeature = (Int32)produto.CodFeature,
                                PatamarInicial = produto.ValInicioPatamar,
                                PatamarFinal = produto.ValFimPatamar,
                                SequenciaPatamar = produto.IndSeqPatamar != null ? (Int32)produto.IndSeqPatamar : 1,
                                TaxaPatamar = produto.ValTaxaPatamar,
                                Prazo = (Int32)produto.PrazoPadrao
                            });
                        }

                        ProdutosListaDadosProdutosPorRamoCanal p = new ProdutosListaDadosProdutosPorRamoCanal();
                        p.CodCCA = produto.CodCca;
                        p.CodFeature = produto.CodFeature;
                        p.IndFormaPagamento = produto.IndFormaPagamento;
                        p.ValorPrazoDefault = produto.PrazoPadrao;
                        p.ValorTaxaDefault = produto.ValorPadrao;
                        p.CodTipoNegocio = produto.IndTipoOperacaoProd;
                        p.QtdeDefaultParcela = produto.ValorLimiteParcela;

                        if (produto.CodCca == 1 || produto.CodCca == 2 || produto.CodCca == 3 || produto.CodCca == 4 || produto.CodCca == 20)
                        {
                            ProdutosListaDadosFeaturesPorCCA dados = DadosProdutosCredito.FirstOrDefault(d => d.CodFeature == produto.CodFeature);
                            if (dados != null)
                            {
                                p.NomeFeature = dados.NomeFeature;
                                p.QtdeMaximaPatamar = dados.QtdeMaximaPatamar;
                            }
                        }

                        if (produto.CodCca == 5 || produto.CodCca == 14)
                        {
                            ProdutosListaDadosFeaturesPorCCA dados = DadosProdutosDebito.FirstOrDefault(d => d.CodFeature == produto.CodFeature);
                            if (dados != null)
                            {
                                p.NomeFeature = dados.NomeFeature;
                                p.QtdeMaximaPatamar = dados.QtdeMaximaPatamar;
                            }
                        }

                        if (produto.CodCca == 22)
                        {
                            ProdutosListaDadosFeaturesPorCCA dados = DadosProdutosConstrucard.FirstOrDefault(d => d.CodFeature == produto.CodFeature);
                            if (dados != null)
                            {
                                p.NomeFeature = dados.NomeFeature;
                                p.QtdeMaximaPatamar = dados.QtdeMaximaPatamar;
                            }
                        }

                        retorno.Add(p);
                    }
                }
                return retorno;
            }
            catch (FaultException<WFProdutos.ModelosErroServicos> fe)
            {
                client.Close();
                throw fe;
            }
            catch (Exception ex)
            {
                client.Close();
                throw ex;
            }
        }

        /// <summary>
        /// Carregar dados de Domicílio Bancário
        /// </summary>
        private List<DomicilioBancarioInclusaoAlteracaoDomicilioBancario> CarregarDadosDomicilioBancario(PropostaPendente proposta, Int32 tipoOperacao)
        {
            List<DomicilioBancarioInclusaoAlteracaoDomicilioBancario> retorno = new List<DomicilioBancarioInclusaoAlteracaoDomicilioBancario>();
            ServicoPortalWFDomicilioBancarioClient client = new ServicoPortalWFDomicilioBancarioClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega dados do domicilio bancário"))
                {
                    Char tipoPessoa = (Char)proposta.TipoPessoa;
                    Int64 cpf_cnpj = (Int64)proposta.CNPJ;
                    Int32 numSeq = (Int32)proposta.NumSequencia;

                    ConsultaDomicilioBancario[] domiciliosBancarios = client.ConsultaDomicilioBancario(tipoPessoa, cpf_cnpj, numSeq, tipoOperacao);
                    client.Close();

                    if (domiciliosBancarios.Length > 0)
                    {
                        if (tipoOperacao == 1)
                        {
                            Credenciamento.CodBancoCredito = (Int32)domiciliosBancarios[0].CodBancoCompensacao;
                            Credenciamento.NomeBancoCredito = domiciliosBancarios[0].NomeBanco;
                            Credenciamento.AgenciaCredito = (Int32)domiciliosBancarios[0].CodigoAgencia;
                            Credenciamento.ContaCredito = domiciliosBancarios[0].NumContaCorrente;
                        }
                        else if (tipoOperacao == 3)
                        {
                            Credenciamento.CodBancoDebito = (Int32)domiciliosBancarios[0].CodBancoCompensacao;
                            Credenciamento.NomeBancoDebito = domiciliosBancarios[0].NomeBanco;
                            Credenciamento.AgenciaDebito = (Int32)domiciliosBancarios[0].CodigoAgencia;
                            Credenciamento.ContaDebito = domiciliosBancarios[0].NumContaCorrente;
                        }
                        else if (tipoOperacao == 4)
                        {
                            Credenciamento.CodBancoConstrucard = (Int32)domiciliosBancarios[0].CodBancoCompensacao;
                            Credenciamento.NomeBancoConstrucard = domiciliosBancarios[0].NomeBanco;
                            Credenciamento.AgenciaConstrucard = (Int32)domiciliosBancarios[0].CodigoAgencia;
                            Credenciamento.ContaConstrucard = domiciliosBancarios[0].NumContaCorrente;
                        }
                    }
                }

                return retorno.ToList();
            }
            catch (FaultException<WFDomBancario.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Retorna os dados das features de um Cca
        /// </summary>
        /// <param name="codCca"></param>
        /// <returns></returns>
        private List<ProdutosListaDadosFeaturesPorCCA> ListaDadosFeaturePorCca(Int32 codCca)
        {
            ServicoPortalGEProdutosClient client = new ServicoPortalGEProdutosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Lista Dados Feature por Cca"))
                {
                    ProdutosListaDadosFeaturesPorCCA[] retorno = client.ListaDadosFeaturesPorCCA(codCca);
                    client.Close();

                    return retorno.ToList();
                }
            }
            catch (FaultException<GEProdutos.ModelosErroServicos> ex)
            {
                client.Abort();
                throw ex;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Carrega lista de proprietários
        /// </summary>
        /// <returns></returns>
        private void CarregarDadosProprietarios(PropostaPendente proposta)
        {
            ServicoPortalWFProprietariosClient client = new ServicoPortalWFProprietariosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega lista de proprietários"))
                {
                    Char tipoPessoa = (Char)proposta.TipoPessoa;
                    Int64 cpf_cnpj = (Int64)proposta.CNPJ;
                    Int32 numSeq = (Int32)proposta.NumSequencia;

                    ConsultaProprietarios[] proprietarios = client.ConsultaProprietarios(tipoPessoa, cpf_cnpj, numSeq, null, null, null);
                    client.Close();

                    if (Credenciamento.Proprietarios != null)
                        Credenciamento.Proprietarios.Clear();
                    else
                        Credenciamento.Proprietarios = new List<Proprietario>();
                    foreach (ConsultaProprietarios proprietario in proprietarios)
                    {
                        Credenciamento.Proprietarios.Add(new Proprietario
                        {
                            CPF_CNPJ = proprietario.NumCNPJCPFProprietario.ToString(),
                            Nome = proprietario.NomeProprietario,
                            Participacao = proprietario.ParticipacaoAcionaria.ToString(),
                            TipoPessoa = proprietario.CodTipoPesProprietario.ToString(),
                            Relato = proprietario.IndDadosRelatoProprietario
                        });
                    }
                }
            }
            catch (FaultException<WFProprietarios.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Carrega lista de serviços
        /// </summary>
        private void CarregarDadosServicos(PropostaPendente proposta)
        {
            ServicoPortalWFServicosClient client = new ServicoPortalWFServicosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega lista de serviços"))
                {
                    Char tipoPessoa = (Char)proposta.TipoPessoa;
                    Int64 cpf_cnpj = (Int64)proposta.CNPJ;
                    Int32 numSeq = (Int32)proposta.NumSequencia;

                    ConsultaServico[] servicos = client.ConsultaServicos(tipoPessoa, cpf_cnpj, numSeq, null);
                    client.Close();

                    if (servicos.Length > 0)
                    {
                        foreach (ConsultaServico servico in servicos)
                        {
                            Credenciamento.Servicos.Add(new Modelo.Servico
                            {
                                CodServico = servico.CodServico,
                                CodRegimeServico = servico.CodRegimeServico,
                                QtdeMinima = (Int32)servico.QtdeMinimaConsulta,
                                ValorFranquia = servico.ValorFranquia
                            });
                        }
                    }
                }
            }
            catch (FaultException<WFServicos.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Carrega dados para um recredenciamento
        /// </summary>
        /// <param name="propostaSelecionada"></param>
        private void CarregarDadosRecredenciamento(PropostaPendente proposta)
        {
            ServicoPortalGEPontoVendaClient client = new ServicoPortalGEPontoVendaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega lista de serviços"))
                {
                    Char tipoPessoa = (Char)proposta.TipoPessoa;
                    Int64 cpf_cnpj = (Int64)proposta.CNPJ;

                    ListaCadastroRecredenciamento[] dados = client.ListaCadastroRecredenciamento(cpf_cnpj, tipoPessoa);
                    client.Close();

                    if (dados.Length > 0)
                    {
                        Credenciamento.NumPdv = dados[0].NumPdv;
                        
                        if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        {
                            Credenciamento.GrupoRamo = (Int32)dados[0].CodGrupoRamo;
                            Credenciamento.RamoAtividade = (Int32)dados[0].CodRamoAtivididade;
                            Credenciamento.RazaoSocial = dados[0].RazaoSocial;
                            Credenciamento.DataFundacao = (DateTime)dados[0].DataFundacao;
                        }
                        else
                        {
                            Credenciamento.NomeCompleto = dados[0].RazaoSocial;
                            Credenciamento.DataNascimento = (DateTime)dados[0].DataFundacao;
                        }
                        Credenciamento.PessoaContato = dados[0].PessoaContato;
                        //Credenciamento.CEP = String.Format("{0}-{1}", dados[0].CodCEP, dados[0].CodCompCEP);
                        Credenciamento.EnderecoComercial.CEP = String.Format("{0}-{1}", dados[0].CodCEP, dados[0].CodCompCEP);
                        Credenciamento.EnderecoComercial.Logradouro = dados[0].NomeLogradouro;
                        Credenciamento.EnderecoComercial.Numero = dados[0].NumLogradouro;
                        Credenciamento.EnderecoComercial.Complemento = dados[0].CompEndereco;
                        Credenciamento.EnderecoComercial.Bairro = dados[0].NomeBairro;
                        Credenciamento.EnderecoComercial.Cidade = dados[0].NomeCidade;
                        Credenciamento.EnderecoComercial.Estado = dados[0].NomeUF;
                        Credenciamento.NomeFatura = dados[0].NomeFatura;
                        Credenciamento.EnderecoCorrespondencia.CEP = String.Format("{0}-{1}", dados[0].CodCEPCorrespondencia, dados[0].CodCompCEPCorrespondencia);
                        Credenciamento.EnderecoCorrespondencia.Cidade = dados[0].NomeCidadeCorrespondencia;
                        Credenciamento.EnderecoCorrespondencia.Bairro = dados[0].NomeBairroCorrespondencia;
                        Credenciamento.EnderecoCorrespondencia.Logradouro = dados[0].NomeLogradouroCorrespondencia;
                        Credenciamento.EnderecoCorrespondencia.Complemento = dados[0].CompEnderecoCorrespondencia;
                        Credenciamento.EnderecoCorrespondencia.Estado = dados[0].NomeUFCorrespondencia;
                        Credenciamento.EnderecoCorrespondencia.Numero = dados[0].NumLogradouroCorrespondencia;
                        Credenciamento.NumDDD1 = dados[0].NumDDD1;
                        Credenciamento.NumTelefone1 = dados[0].NumTelefone1;
                        Credenciamento.Ramal1 = dados[0].NumRamal1;
                        Credenciamento.NumDDD2 = dados[0].NumDDD2;
                        Credenciamento.NumTelefone2 = dados[0].NumTelefone2;
                        Credenciamento.Ramal2 = dados[0].NumRamal2;
                        Credenciamento.NumDDDFax = dados[0].NumDDDFax;
                        Credenciamento.NumTelefoneFax = dados[0].NumTelefoneFax;
                        Credenciamento.NomeEmail = dados[0].Email;
                        Credenciamento.CodTipoEstabelecimento = dados[0].CodTipoEstabelecimento;
                        Credenciamento.NomeHomePage = dados[0].HomePage;
                        Credenciamento.CodCarteira = dados[0].CodCarteira;
                        Credenciamento.CodGerencia = dados[0].CodGerencia;
                        Credenciamento.LocalPagamento = dados[0].CodLocalPagamento;
                        Credenciamento.PermIATA = dados[0].IndicadorIATA;
                        Credenciamento.GrupoComercial = dados[0].NumGrupoComercial;
                        Credenciamento.GrupoGerencial = dados[0].NumGrupoGerencial;
                    }
                }
            }
            catch (FaultException<GEPontoVen.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Carrega dados da proposta por ponto de venda
        /// </summary>
        /// <param name="proposta"></param>
        private void CarregarDadosPorPontoVenda(PropostaPendente proposta)
        {
            ServicoPortalGEPontoVendaClient client = new ServicoPortalGEPontoVendaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega dados por ponto de venda"))
                {
                    Int32 numPdv = (Int32)proposta.NroEstabelecimento;

                    ListaCadastroPorPontoVenda[] dados = client.ListaCadastroPorPontoVenda(numPdv);
                    client.Close();

                    if (dados.Length > 0)
                    {
                        //Credenciamento.CEP = String.Format("{0}-{1}", dados[0].CodCEP, dados[0].CodCompCEP);
                        //Credenciamento.EnderecoComercial.CEP = String.Format("{0}-{1}", dados[0].CodCEP, dados[0].CodCompCEP);
                        Credenciamento.EnderecoCorrespondencia.CEP = String.Format("{0}-{1}", dados[0].CodCEPCorrespondencia, dados[0].CodCompCEPCorrespondencia);
                        //Credenciamento.EnderecoInstalacao.CEP = String.Format("{0}-{1}", dados[0].CodCEPTecnologia, dados[0].CodCompCEPTecnologia);
                        //if (dados[0].CodCanal != null)
                        //    Credenciamento.Canal = (Int32)dados[0].CodCanal;
                        //if (dados[0].CodCelula != null)
                        //    Credenciamento.Celula = (Int32)dados[0].CodCelula;
                        Credenciamento.CodFilial = dados[0].CodFilial;
                        if (dados[0].CodGrupoRamo != null)
                            Credenciamento.GrupoRamo = (Int32)dados[0].CodGrupoRamo;
                        Credenciamento.HorarioFuncionamento = dados[0].CodHorarioFuncionamento;
                        if (dados[0].CodRamoAtivididade != null)
                            Credenciamento.RamoAtividade = (Int32)dados[0].CodRamoAtivididade;
                        //Credenciamento.CodTipoEstabelecimento = dados[0].CodTipoEstabelecimento;
                        //Credenciamento.TipoPessoa = dados[0].CodTipoPessoa.ToString();
                        Credenciamento.CodZonaVenda = dados[0].CodZona;
                        //Credenciamento.EnderecoComercial.Complemento = dados[0].CompEndereco;
                        Credenciamento.EnderecoCorrespondencia.Complemento = dados[0].CompEnderecoCorrespondencia;
                        //Credenciamento.EnderecoInstalacao.Complemento = dados[0].CompEnderecoTecnologia;
                        //if (dados[0].DataAssinaturaProposta != null)
                        //    Credenciamento.DataCadastroProposta = (DateTime)dados[0].DataAssinaturaProposta;

                        if (dados[0].DataFundacao != null)
                            if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                                Credenciamento.DataFundacao = (DateTime)dados[0].DataFundacao;
                            else
                                Credenciamento.DataNascimento = (DateTime)dados[0].DataFundacao;

                        //Credenciamento.DescricaoCanal = dados[0].DescCanal;
                        //Credenciamento.DescricaoCelula = dados[0].DescCelula;
                        Credenciamento.DescricaoRamoAtividade = dados[0].DescRamoAtividade;
                        Credenciamento.NomeEmail = dados[0].Email;
                        Credenciamento.NomeHomePage = dados[0].HomePage;
                        //Credenciamento.IndExtratoEmail = dados[0].IndEnvioExtratoEmail;
                        //Credenciamento.EnderecoComercial.Bairro = dados[0].NomeBairro;
                        Credenciamento.EnderecoCorrespondencia.IndTipoEndereco = '2';
                        Credenciamento.EnderecoCorrespondencia.Bairro = dados[0].NomeBairroCorrespondencia;
                        //Credenciamento.EnderecoInstalacao.Bairro = dados[0].NomeBairroTecnologia;
                        //Credenciamento.EnderecoComercial.Cidade = dados[0].NomeCidade;
                        Credenciamento.EnderecoCorrespondencia.Cidade = dados[0].NomeCidadeCorrespondencia;
                        //Credenciamento.EnderecoInstalacao.Cidade = dados[0].NomeCidadeTecnologia;
                        Credenciamento.NomeFatura = dados[0].NomeFatura;
                        Credenciamento.NomeFilial = dados[0].NomeFilial;
                        //Credenciamento.EnderecoComercial.Logradouro = dados[0].NomeLogradouro;
                        Credenciamento.EnderecoCorrespondencia.Logradouro = dados[0].NomeLogradouroCorrespondencia;
                        //Credenciamento.EnderecoInstalacao.Logradouro = dados[0].NomeLogradouroTecnologia;
                        //Credenciamento.EnderecoComercial.Estado = dados[0].NomeUF;
                        Credenciamento.EnderecoCorrespondencia.Estado = dados[0].NomeUFCorrespondencia;
                        //Credenciamento.EnderecoInstalacao.Estado = dados[0].NomeUFTecnologia;
                        Credenciamento.NomeZonaVenda = dados[0].NomeZona;

                        //Credenciamento.CPFVendedor = dados[0].NumCPFVendedor;
                        Credenciamento.Centralizadora = dados[0].NumCentralizadora;
                        //Credenciamento.NumDDD1 = dados[0].NumDDD1;
                        Credenciamento.NumDDD2 = dados[0].NumDDD2;
                        Credenciamento.NumDDDFax = dados[0].NumDDDFax;
                        //Credenciamento.NumDDDInstalacao = dados[0].NumDDDCV;
                        Credenciamento.GrupoComercial = dados[0].NumGrupoComercial;
                        Credenciamento.GrupoGerencial = dados[0].NumGrupoGerencial;
                        //Credenciamento.EnderecoComercial.Numero = dados[0].NumLogradouro;
                        Credenciamento.EnderecoCorrespondencia.Numero = dados[0].NumLogradouroCorrespondencia;
                        //Credenciamento.EnderecoInstalacao.Numero = dados[0].NumLogradouroTecnologia;
                        //Credenciamento.NumPdv = dados[0].NumPdv;
                        //Credenciamento.Ramal1 = dados[0].NumRamal1;
                        Credenciamento.Ramal2 = dados[0].NumRamal2;
                        //if (dados[0].NumRamalCV != null)
                        //    Credenciamento.RamalInstalacao = (Int32)dados[0].NumRamalCV;
                        //Credenciamento.NumTelefone1 = dados[0].NumTelefone1;
                        Credenciamento.NumTelefone2 = dados[0].NumTelefone2;
                        //if (dados[0].NumTelefoneCV != null)
                        //    Credenciamento.NumTelefoneInstalacao = (Int32)dados[0].NumTelefoneCV;
                        Credenciamento.NumTelefoneFax = dados[0].NumTelefoneFax;
                        Credenciamento.NumPdvMatriz = dados[0].NumeroMatriz;
                        Credenciamento.PessoaContato = dados[0].PessoaContato;
                        if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                            Credenciamento.RazaoSocial = dados[0].RazaoSocial;
                        else
                            Credenciamento.NomeCompleto = dados[0].RazaoSocial;

                        //if (dados[0].ValorTaxaAdesao != null)
                        //    Credenciamento.TaxaAdesao = (Double)dados[0].ValorTaxaAdesao;
                    }
                }
            }
            catch (FaultException<GEPontoVen.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Recupera a linha selecionada da grid
        /// </summary>
        private PropostaPendente GetSelectedRow(out Boolean existeSelecao)
        {
            existeSelecao = false;
            PropostaPendente proposta = new PropostaPendente();

            GridViewRowCollection rows = gvPropostas.Rows;

            for (int i = 0; i < rows.Count; i++)
            {
                RadioButton rb = (RadioButton)rows[i].FindControl("rbSelect");
                if (rb.Checked)
                {
                    existeSelecao = true;
                    Int64 cpf_cnpj = 0;
                    Int64.TryParse(((Label)rows[i].FindControl("lblCNPJ")).Text.Replace(".", "").Replace("/", "").Replace("-", ""), out cpf_cnpj);

                    proposta.TipoPessoa = ((Label)rows[i].FindControl("lblCNPJ")).Text.Contains("/") ? 'J' : 'F';
                    proposta.CNPJ = cpf_cnpj;
                    proposta.NroEstabelecimento = ((Label)rows[i].FindControl("lblEstabelecimento")).Text.ToInt32();
                    proposta.NumSequencia = ((HiddenField)rows[i].FindControl("hiddenNumSeq")).Value.ToInt32Null();
                    proposta.StatusProposta = ((HiddenField)rows[i].FindControl("hiddenStatus")).Value.ToInt32Null();
                }
            }

            return proposta;
        }

        /// <summary>
        /// Consulta o número de sequência
        /// </summary>
        private void ConsultaProximaSequencia()
        {
            ServicoPortalWFPropostaClient client = new ServicoPortalWFPropostaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Consulta Próxima Sequência"))
                {
                    Char tipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                    Int64 numCNPJCPF = 0;
                    if (tipoPessoa == 'J')
                        Int64.TryParse(Credenciamento.CNPJ.Replace(".", "").Replace("-", "").Replace("/", ""), out numCNPJCPF);
                    else
                        Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJCPF);

                    Logger.GravarLog("Chamada ao Serviço ConsultaProximaSequencia");
                    Logger.GravarLog(String.Format("numCNPJCPF: {0}", numCNPJCPF));
                    ConsultaProximaSequencia[] proximaSequencia = client.ConsultaProximaSequencia(tipoPessoa, numCNPJCPF);
                    client.Close();

                    Logger.GravarLog(String.Format("Lenght: {0}", proximaSequencia.Length));
                    if (proximaSequencia.Length > 0)
                    {
                        Logger.GravarLog(String.Format("NumSequencia: {0}", proximaSequencia[0].NumSequencia));
                        Credenciamento.NumSequencia = proximaSequencia[0].NumSequencia;
                    }
                }
            }
            catch (FaultException<WFProposta.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Lista proprietários de um ponto de venda
        /// </summary>
        /// <param name="proposta"></param>
        private void ListaProprietariosPontoVenda(PropostaPendente proposta)
        {
            ServicoPortalGEProprietariosClient client = new ServicoPortalGEProprietariosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Lista Proprietários por ponto de venda"))
                {
                    Int32 numPdv = (Int32)proposta.NroEstabelecimento;

                    ProprietarioListaDadosPorPontoVenda[] proprietarios = client.ListaDadosPorPontoVenda(numPdv);
                    client.Close();

                    Credenciamento.Proprietarios = new List<Proprietario>();

                    foreach (ProprietarioListaDadosPorPontoVenda proprietario in proprietarios)
                    {
                        Credenciamento.Proprietarios.Add(new Modelo.Proprietario
                        {
                            CPF_CNPJ = proprietario.NumCNPJCPFProprietario.ToString(),
                            Nome = proprietario.NomeProprietario,
                            Participacao = proprietario.ValorParticipacaoSocietaria.ToString(),
                            TipoPessoa = proprietario.CodTipoPessoaProprietario.ToString()
                        });
                    }
                }
            }
            catch (FaultException<GEProprietarios.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Lista dados produtos por ponto de venda
        /// </summary>
        private List<ProdutosListaDadosProdutosPorRamoCanal> ListaDadosProdutosPorPontoDeVenda(PropostaPendente proposta, Char? tipoOperacao)
        {
            ServicoPortalGEProdutosClient client = new ServicoPortalGEProdutosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Lista Produtos por ponto de venda"))
                {
                    List<ProdutosListaDadosProdutosPorRamoCanal> retorno = new List<ProdutosListaDadosProdutosPorRamoCanal>();
                    Int32 numPdv = (Int32)proposta.NroEstabelecimento;

                    ProdutosListaDadosProdutosPorPontoVenda[] produtos = client.ListaDadosProdutoscomRegimePorPontoVenda(numPdv, tipoOperacao);
                    client.Close();

                    Logger.GravarLog(String.Format("ListaDadosProdutosPorPontoVenda - Lenght: {0}", produtos.Length));
                    foreach (ProdutosListaDadosProdutosPorPontoVenda produto in produtos)
                    {
                        if (produto.CodCCA == 1 && produto.CodFeature == 1)
                        {
                            Credenciamento.AgenciaCredito = (Int32)produto.CodAgenciaDomicilio;
                            Credenciamento.CodBancoCredito = (Int32)produto.CodBancoDomicilio;
                            Credenciamento.ContaCredito = produto.NumContaCorrenteDomicilio;
                            Credenciamento.NomeBancoCredito = String.Empty;
                        }

                        if (produto.CodCCA == 5)
                        {
                            Credenciamento.AgenciaDebito = (Int32)produto.CodAgenciaDomicilio;
                            Credenciamento.CodBancoDebito = (Int32)produto.CodBancoDomicilio;
                            Credenciamento.ContaDebito = produto.NumContaCorrenteDomicilio;
                        }

                        if (produto.CodCCA == 22)
                        {
                            Credenciamento.AgenciaConstrucard = (Int32)produto.CodAgenciaDomicilio;
                            Credenciamento.CodBancoConstrucard = (Int32)produto.CodBancoDomicilio;
                            Credenciamento.ContaConstrucard = produto.NumContaCorrenteDomicilio;
                        }

                        if (produto.CodFeature == 3)
                        {
                            if (Credenciamento.Patamares == null)
                                Credenciamento.Patamares = new List<Patamar>();

                            Credenciamento.Patamares.Add(new Patamar
                            {
                                CodCca = (Int32)produto.CodCCA,
                                CodFeature = (Int32)produto.CodFeature,
                                PatamarInicial = produto.PatamarInicioNovo != null ? produto.PatamarInicioNovo : 0,
                                PatamarFinal = produto.PatamarFimNovo != null ? produto.PatamarFimNovo : 0,
                                SequenciaPatamar = produto.NumSequenciaPatamar != null ? (Int32)produto.NumSequenciaPatamar : 1,
                                TaxaPatamar = produto.ValorTaxaRegime,
                                Prazo = (Int32)produto.PrazoRegime
                            });
                        }

                        ProdutosListaDadosProdutosPorRamoCanal p = new ProdutosListaDadosProdutosPorRamoCanal();
                        p.CodCCA = produto.CodCCA;
                        p.CodFeature = produto.CodFeature;
                        p.IndFormaPagamento = produto.IndUtilizacaoTarifa == 'N' ? 'X' : 'T';
                        p.ValorPrazoDefault = produto.PrazoRegime;
                        p.ValorTaxaDefault = produto.ValorTaxaRegime;
                        p.CodTipoNegocio = produto.CodTipoTransacaoOperacao;
                        p.QtdeMaximaParcela = produto.QtdeLimiteParcelaNovo;
                        p.QtdeDefaultParcela = produto.QtdeLimiteParcelaNovo;
                        p.NomeFeature = produto.NomeFeature;
                        p.NomeCCA = produto.NomeCCA;

                        retorno.Add(p);
                    }

                    Logger.GravarLog(String.Format("Retorno - Count: {0}", retorno.Count));
                    return retorno;
                }
            }
            catch (FaultException<GEProdutos.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Busca Informações do Tipo de Estabelecimento e grava na sessão
        /// </summary>
        private void GravaInfoTipoEstabCredenciamento()
        {
            ServicoPortalGEPontoVendaClient client = new ServicoPortalGEPontoVendaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Consulta Tipo de Estabelecimento Credenciamento"))
                {
                    Int64 numCNPJ = 0;
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                    else
                        Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                    PontoVendaConsultaTipoEstabCredenciamento[] pontoVenda = client.ConsultaTipoEstabCredenciamento(numCNPJ, null);
                    client.Close();

                    if (pontoVenda.Length > 0)
                    {
                        Credenciamento.CodTipoEstabelecimento = pontoVenda[0].CodTipoEstabelecimento;
                        Credenciamento.CodTipoEstabMatriz = pontoVenda[0].CodTipoEstabMatriz;
                        Credenciamento.NumPdv = 0;
                        Credenciamento.NumPdvMatriz = pontoVenda[0].NumPdvMatriz;
                    }
                }
            }
            catch (FaultException<GEPontoVen.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Consulta dados do Serasa para pessoas físca ou jurídica
        /// </summary>
        private void CarregarSerasa()
        {
            SerasaServicoClient client = new SerasaServicoClient();

            try
            {
                if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                {
                    using (Logger log = Logger.IniciarLog("Carregar Dados do Serasa PJ"))
                    {
                        String cnpj = Credenciamento.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "");
                        PJ dadosPJ = client.ConsultaSerasaPJ(cnpj);
                        client.Close();

                        Credenciamento.RetornoSerasa = dadosPJ.CodRetorno;

                        if (String.Compare(dadosPJ.CodRetorno, "00") == 0)
                        {
                            if (!String.IsNullOrEmpty(dadosPJ.ComplGrafia))
                            {
                                Credenciamento.RazaoSocial = dadosPJ.ComplGrafia;
                            }
                            if (!String.IsNullOrEmpty(dadosPJ.DataFundacao) && String.Compare(dadosPJ.DataFundacao, "00000000") != 0)
                            {
                                Credenciamento.DataFundacao = dadosPJ.DataFundacao.ToDate("yyyyMMdd");
                            }

                            if (!String.IsNullOrEmpty(dadosPJ.CNAEs[0].CodCNAE))
                            {
                                Credenciamento.CNAE = dadosPJ.CNAEs[0].CodCNAE;
                                Credenciamento.RamoAtividade = 0;
                                Credenciamento.GrupoRamo = 0;
                            }

                            if (dadosPJ.Socios.Length > 0)
                            {
                                foreach (Modelo.Proprietario propietario in Credenciamento.Proprietarios)
                                {
                                    ExcluirProprietario(propietario);
                                }

                                Credenciamento.Proprietarios = new List<Modelo.Proprietario>();
                                foreach (Socio socio in dadosPJ.Socios)
                                {
                                    Credenciamento.Proprietarios.Add(new Modelo.Proprietario()
                                    {
                                        CPF_CNPJ = socio.CPF_CNPJ,
                                        Nome = socio.Nome,
                                        Participacao = socio.Participacao,
                                        TipoPessoa = socio.TipoPessoa,
                                        Relato = String.Empty
                                    });
                                }
                            }
                        }
                    }
                }
                //else
                //{
                //    using (Logger log = Logger.IniciarLog("Carregar Dados do Serasa PF"))
                //    {
                //        String cpf = Credenciamento.CPF.Replace(".", "").Replace("-", "");
                //        PF dadosPF = client.ConsultaSerasaPF(cpf);
                //        client.Close();

                //        if (String.Compare(dadosPF.CodRetorno, "00") == 0)
                //        {
                            
                //        }
                //        else
                //        {
                //            base.ExibirPainelExcecao("Não foi retornado dados do SERASA para esse CPF", "300");
                //        }
                //    }
                //}
            }
            catch (FaultException<WFSerasa.GeneralFault> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
            }
            catch (FaultException<WFProprietarios.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodigoErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Excluí um proprietário
        /// </summary>
        private WFProprietarios.RetornoErro ExcluirProprietario(Modelo.Proprietario proprietario)
        {
            ServicoPortalWFProprietariosClient client = new ServicoPortalWFProprietariosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Excluí dados do proprietário"))
                {
                    Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                    Int64 numCNPJ = 0;
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                    else
                        Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                    Int32 numSeqProp = (Int32)Credenciamento.NumSequencia;
                    Char codTipoPesProprietario = proprietario.TipoPessoa.ToCharArray()[0];
                    Int64 numCNPJCPFProprietario = 0;
                    if (codTipoPesProprietario == 'J')
                        Int64.TryParse(proprietario.CPF_CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJCPFProprietario);
                    else
                        Int64.TryParse(proprietario.CPF_CNPJ.Replace(".", "").Replace("-", ""), out numCNPJCPFProprietario);

                    String nomeProprietario = proprietario.Nome;
                    DateTime? dataNascProprietario = null;
                    Double? participacaoAcionaria = null;
                    String usuarioUltimaAtualizacao = String.Empty;

                    WFProprietarios.RetornoErro[] retorno = client.ExclusaoProprietarios(codTipoPessoa, numCNPJ, numSeqProp, codTipoPesProprietario, numCNPJCPFProprietario, nomeProprietario, dataNascProprietario, participacaoAcionaria, usuarioUltimaAtualizacao);
                    client.Close();

                    return retorno[0];
                }
            }
            catch (FaultException<PNTransicoesServico.GeneralFault>)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Carrega lista de produtos Van da proposta
        /// </summary>
        private List<ProdutosListaDadosProdutosVanPorRamo> CarregarProdutosVan(PropostaPendente proposta)
        {
            ServicoPortalWFProdutosClient client = new ServicoPortalWFProdutosClient();

            try
            {
                Char codTipoPessoa = (Char)proposta.TipoPessoa;
                Int64 numCNPJ = (Int64)proposta.CNPJ;
                Int32 numSeqProp = (Int32)proposta.NumSequencia;
                Int32? codCca = null;

                ConsultaProdutosVan[] produtos = client.ConsultaProdutosVan(codTipoPessoa, numCNPJ, numSeqProp, codCca);
                client.Close();

                return produtos.Select(p =>  new ProdutosListaDadosProdutosVanPorRamo{
                    CodCCA = p.CodCca,
                }).ToList();
            }
            catch (FaultException<WFProdutos.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Carrega dados domicílio bancário crédito da matriz
        /// </summary>
        /// <param name="proposta"></param>
        private void CarregarDadosDomicilioBancarioCreditoPorPontoVenda(PropostaPendente proposta)
        {
            ServicoPortalGEDomicilioBancarioClient client = new ServicoPortalGEDomicilioBancarioClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega dados domicílio bancário crédito do ponto de venda"))
                {
                    Int32 numPontoVenda = (Int32)proposta.NroEstabelecimento;
                    Char? codTipoOperacao = 'C';
                    String siglaProduto = "CR";

                    DomBancariosPorPVOperProd[] dados = client.ConsultaDomBancariosPorPVOperProd(numPontoVenda, codTipoOperacao, siglaProduto);
                    client.Close();

                    if (dados[0].CodigoErro == 0)
                    {
                        Credenciamento.CodBancoCredito = (Int32)dados[0].CodBancoAtual;
                        Credenciamento.AgenciaCredito = (Int32)dados[0].CodAgenciaAtual;
                        Credenciamento.ContaCredito = dados[0].NumeroContaAtual;
                        Credenciamento.NomeBancoCredito = dados[0].NomeBancoAtual;
                    }
                }
            }
            catch (FaultException<GEDomBancario.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        #endregion
    }
}
