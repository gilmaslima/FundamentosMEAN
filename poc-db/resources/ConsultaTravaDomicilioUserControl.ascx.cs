/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/10/14 - Tiago Barbosa dos Santos - Versão Inicial
*/
using System;

using System.Web.UI.WebControls;
using System.Collections.Generic;
using Redecard.PN.Emissores.Sharepoint.ServicoEmissores;
using System.ServiceModel;
using System.Web.UI;
using Redecard.PN.Comum;
using System.Web.UI.HtmlControls;
using System.Linq;
namespace Redecard.PN.Emissores.Sharepoint.WebParts.ConsultaTravaDomicilio
{
    public partial class ConsultaTravaDomicilioUserControl : UserControlBase
    {
        static readonly DateTime inicioVigencia = new DateTime(2008, 08, 01);

        private static Int16 funcao = 0;
        private static Int32 numeroPV = 0;
        private static decimal cnpj = 0;
        private static Int16 ano = 0;
        private static Int16 mes = 0;

        private enum TipoView
        {
            Principal,
            Filho,
            Detalhe
        }

        protected Redecard.PN.Emissores.Sharepoint.ControlTemplates.TravaDomicilio.ConsultaFilho ucConsultaFilho;
        protected Redecard.PN.Emissores.Sharepoint.ControlTemplates.TravaDomicilio.ConsultaPrincipal ucConsultaPrincipal;
        protected Redecard.PN.Emissores.Sharepoint.ControlTemplates.TravaDomicilio.ConsultaDetalhe ucConsultaDetalhe;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AlterarViewAtual(TipoView.Principal);
                CarregaCampoAno();
                BuscaLimite();

            }
            else
            {
                TipoView viewAtual = ObterViewAtual();
                mvTravaDomicilio.ActiveViewIndex = (Int32)viewAtual;


            }
        }



        protected void ucConsultaFilho_OnItemSelecionado(object[] parametros, EventArgs e)
        {
            if (!object.Equals(parametros, null))
            {
                if (parametros.Length == 8)
                {
                    funcao = parametros[0].ToString().ToInt16();
                    numeroPV = parametros[1].ToString().ToInt32();
                    cnpj = parametros[2].ToString().ToDecimalNull(0).Value;
                    ano = parametros[3].ToString().ToInt16();
                    mes = parametros[4].ToString().ToInt16();

                    decimal faixaInicial = parametros[5].ToString().ToDecimalNull(0).Value;
                    decimal faixaFinal = parametros[6].ToString().ToDecimalNull(0).Value;
                    decimal fator = parametros[7].ToString().ToDecimalNull(0).Value;

                    ucConsultaDetalhe.CarregarDetalhe(GuidPesquisa(), inicioVigencia, funcao, numeroPV, cnpj, string.Empty, string.Empty, ano, mes,
                        faixaInicial, faixaFinal, fator);

                    AlterarViewAtual(TipoView.Detalhe);
                }
            }
        }
        protected void ConsultaPrincipal_OnItemSelecionado(object[] parametros, EventArgs e)
        {
            if (!object.Equals(parametros, null))
            {
                if (parametros.Length == 3)
                {

                    //Int16 funcao = parametros[0].ToString().ToInt16();
                    //Int32 numeroPV = parametros[1].ToString().ToInt32();
                    //decimal cnpj = parametros[2].ToString().ToDecimalNull(0).Value;
                    //Int16 ano = parametros[3].ToString().ToInt16();
                    //Int16 mes = parametros[4].ToString().ToInt16();

                    decimal faixaInicial = parametros[0].ToString().ToDecimalNull(0).Value;
                    decimal faixaFinal = parametros[1].ToString().ToDecimalNull(0).Value;
                    decimal fator = parametros[2].ToString().ToDecimalNull(0).Value;

                    ucConsultaFilho.CarregarDetalheFilho(GuidPesquisa(), inicioVigencia, funcao, ano, mes, numeroPV, cnpj, faixaInicial, faixaFinal, fator);

                    AlterarViewAtual(TipoView.Filho);
                }

            }

        }


        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            TipoView viewAtual = ObterViewAtual();
            TipoView proximoView = TipoView.Principal;
            if (viewAtual == TipoView.Detalhe)
            {
                proximoView = TipoView.Filho;
            }

            AlterarViewAtual(proximoView);

            if (proximoView == TipoView.Principal && ucConsultaPrincipal.QuantidadeRegistros > 0)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Paginacao" + new Guid().ToString(), "pageResultTable('tblDados', 1, 30, 5);", true);

        }

        private void AlterarViewAtual(TipoView view)
        {
            ViewState["ViewAtual"] = view;

            mvTravaDomicilio.ActiveViewIndex = (int)view;

            btnVoltarRight.Visible = view != TipoView.Principal;


        }

        private TipoView ObterViewAtual()
        {
            TipoView view;

            if (ViewState["ViewAtual"] == null)
                view = TipoView.Principal;
            else
                view = (TipoView)ViewState["ViewAtual"];

            return view;
        }

        public Guid GuidPesquisa()
        {
            Guid? guid = ViewState["guidPesquisa"] as Guid?;

            //cria ou recupera os guids de consulta
            if (guid == null)
            {
                guid = Guid.NewGuid();
                ViewState["guidPesquisa"] = guid;
            }
            return (Guid)ViewState["guidPesquisa"];
        }

        private void CarregaCampoAno()
        {
            ddlAno.Items.Clear();
            var anos = new List<ListItem>();
            ddlAno.Items.Add(new ListItem() { Text = "-----------", Value = "0" });
            for (int i = DateTime.Now.Year; i > DateTime.Now.Year - 15; i--)
            {
                ddlAno.Items.Add(i.ToString());
            }
        }

        protected void ClickBuscar(object sender, EventArgs e)
        {
            try
            {
                
                Boolean retorno = false;
                if (ValidaFiltro())
                {
                    if (ddlAno.SelectedIndex > 0 && ddlMes.SelectedIndex > 0)
                    {
                        ano = ddlAno.SelectedValue.ToInt16();
                        mes = ddlMes.SelectedValue.ToInt16();
                        string mensagemRetorno = string.Empty;

                        numeroPV = txtNumEstabelecimento.Text.ToInt32();
                        cnpj = txtNunCNPJCPF.Text.ToDecimal();
                        Int32 codigoProduto = 1;

                        if (numeroPV <= 0 && cnpj <= 0)
                        {
                            funcao = 9;
                        }
                        else if (numeroPV > 0)
                        {
                            funcao = 1;
                        }
                        else if (cnpj > 0)
                        {
                            funcao = 2;
                        }

                        retorno = ucConsultaPrincipal.BuscaPorPeriodo(GuidPesquisa(), funcao, numeroPV, cnpj, ano, mes, codigoProduto);
                        if (retorno)
                            VerificaControlesVisiveis(ucConsultaPrincipal.QuantidadeRegistros, "Aviso", "Consulta Trava de Domicílio <br>Não há estabelecimentos com trava de domicílio para o período informado");
                        else
                            VerificaControlesVisiveis(0, "Aviso", "Erro ao efetuar a consulta");


                    }

                    if (StatusPv.SelectedValue.ToInt16() > 1 && !string.IsNullOrEmpty(txtNunCNPJCPF.Text))
                    {
                        if (StatusPv.SelectedValue.ToInt16() == 2)
                        {
                            retorno = ucConsultaPrincipal.BuscaPvTravado(txtNunCNPJCPF.Text);
                            if (retorno)
                                VerificaControlesVisiveis(ucConsultaPrincipal.QuantidadeRegistros, "Aviso", "Consulta Trava de Domicílio <br>Não há trava de domicílio para o CNPJ / CPF informado");
                            else
                                VerificaControlesVisiveis(0, "Aviso", "Erro ao efetuar a consulta");

                        }
                        else
                        {
                            retorno = ucConsultaPrincipal.BuscaPvNaoTravado(txtNunCNPJCPF.Text);
                            ucConsultaPrincipal.CarregarTotais(txtNunCNPJCPF.Text);

                            if (retorno)
                            {
                                if (ucConsultaPrincipal.QuantidadeRegistros <= 0)
                                    ExibeQuadroAviso("Aviso", "Consulta Trava de Domicílio <br>Não há trava de domicílio para o CNPJ / CPF informado");
                            }
                            else
                                ExibeQuadroAviso("Aviso", "Erro ao efetuar a consulta");
                        }
                        return;
                    }

                    AlterarViewAtual(TipoView.Principal);
                }
                else
                {

                    VerificaControlesVisiveis(0, "Aviso", "Favor informar um filtro válido para efetuar a pesquisa");
                    return;
                }

            }
            catch (FaultException<PortalRedecardException> ex)
            {
                ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                SharePointUlsLog.LogErro(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("ClickBuscar - ", ex);
            }
        }

        private bool ValidaFiltro()
        {
            //Filtro Período
            if (ddlAno.SelectedIndex > 0 && ddlMes.SelectedIndex > 0)
            {
                return true;
            }

            if (StatusPv.SelectedValue.ToInt16() > 1)
            {
                if (!string.IsNullOrEmpty(txtNunCNPJCPF.Text))
                {

                    return true;
                }
            }

            return false;

        }

        #region Busca Limite
        public void BuscaLimite()
        {
            try
            {
                using (var context = new ContextoWCF<ServicoPortalEmissoresClient>())
                {
                    Int32 codigoRetorno = 0;
                    String mensagemRetorno = string.Empty;
                    String limite = context.Cliente.ConsultaLimite(out codigoRetorno, out mensagemRetorno, SessaoAtual.CodigoEntidade).ToString("0,0.00");
                    if (codigoRetorno != 0)
                    {
                        List<Panel> lstPaineis = new List<Panel>();

                        //ExibirPainelConfirmacaoAcao("Trava Domicílio", "Erro ao consultar limite", Request.Url.AbsolutePath, lstPaineis.ToArray(), "icone-aviso");
                        return;
                    }

                    txtLimite.Text = limite;
                }

            }
            catch (FaultException<PortalRedecardException> ex)
            {
                ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                SharePointUlsLog.LogErro(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex.Message);
                ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                return;
            }
        }
        #endregion



        #region quadro aviso
        protected global::System.Web.UI.WebControls.Panel pnlQuadroAviso;
        protected global::Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum.QuadroAviso qdAvisoSemRegistros;

        /// <summary>Verifica os controles que devem estar visíveis</summary>
        /// <param name="qtdRegistros">Quantidade de registros da página</param>
        protected void VerificaControlesVisiveis(int qtdRegistros, string titulo, string mensagem)
        {
            if (qtdRegistros > 0)
            {
                if (pnlQuadroAviso != null)
                    pnlQuadroAviso.Visible = false;
                if (pnlResultado != null)
                    pnlResultado.Visible = true;

            }
            else
            {
                //Define o título da mensagem
                if (titulo == null)
                {
                    titulo = "Aviso";
                }

                if (pnlQuadroAviso != null)
                    pnlQuadroAviso.Visible = true;
                if (pnlResultado != null)
                    pnlResultado.Visible = false;

                if (qdAvisoSemRegistros != null)
                {
                    qdAvisoSemRegistros.ClasseImagem = "icone-aviso";
                    qdAvisoSemRegistros.CarregarMensagem(titulo, mensagem ?? "Não há movimento para o filtro informado!");
                }
            }
        }

        protected void ExibeQuadroAviso(string titulo, string mensagem)
        {
            if (pnlQuadroAviso != null)
                pnlQuadroAviso.Visible = true;
            if (pnlResultado != null)
                pnlResultado.Visible = true;

            qdAvisoSemRegistros.ClasseImagem = "icone-aviso";
            qdAvisoSemRegistros.CarregarMensagem(titulo, mensagem ?? "Não há movimento para o filtro informado!");

        }
        #endregion
    }
}
