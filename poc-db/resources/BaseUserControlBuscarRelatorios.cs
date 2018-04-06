using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.SharePoint.ControlTemplates;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Microsoft.SharePoint.Utilities;
using System.ServiceModel;

namespace Redecard.PN.Extrato.SharePoint.WebParts
{
    public abstract class BaseUserControlBuscarRelatorios : BaseUserControl
    {
        #region Atributos e Propriedades
        private SelecionarPVs objSelecionarPVs;
        private BuscarRelatorioVendas objBuscar;
        private Redecard.PN.Extrato.SharePoint.ControlTemplates.Paginacao objPaginacao;
        #endregion Atributos e Propriedades

        #region Metodos de auxilio
        protected void InicializaPageLoad(String relatorioValorSelecionado, int tipoVendaIndex, System.Web.UI.WebControls.DropDownList ddlRegistroPorPagina)
        {
            /*Esconde os dados dos relatorios*/
            MostrarResultadoRelatorio(false);

            //recupera o controle de Selecionar PVs e seta seus eventos
            objSelecionarPVs = this.FindControl("SelecionarPVs1") as SelecionarPVs;

            //recupera o controle de Busca e seta seus eventos
            objBuscar = this.FindControl("BuscarRelatorioVendas1") as BuscarRelatorioVendas;
            objBuscar.onBuscar += new BuscarRelatorioVendas.Buscar(objBuscar_onBuscar);
            objBuscar.onDownloadExcel += new BuscarRelatorioVendas.DownloadExcel(objBuscar_onDownloadExcel);
            objBuscar.SelecionarPVs = objSelecionarPVs;
            objBuscar.RelatorioValorSelecionado = relatorioValorSelecionado;
            objBuscar.TipoVendaIndex = tipoVendaIndex;

            //recupera o controle de Paginação e seta seus eventos e variaveis
            objPaginacao = this.FindControl("Paginacao1") as Redecard.PN.Extrato.SharePoint.ControlTemplates.Paginacao;
            objPaginacao.onPaginacaoChanged += new Redecard.PN.Extrato.SharePoint.ControlTemplates.Paginacao.PaginacaoChanged(objPaginacao_onPaginacaoChanged);
            objPaginacao.RegistrosPorPagina = Convert.ToInt32(ddlRegistroPorPagina.SelectedValue);
        }

        protected void ConfigurarDadosPainelBusca(BuscarDados objBuscarDados, Boolean ativarControles) {
            objBuscar.SetValoresBusca(objBuscarDados.DataInicial.ToString("dd/MM/yyyy"), objBuscarDados.DataFinal.ToString("dd/MM/yyyy"), objBuscarDados.Estabelecimentos);
            objBuscar.AtivarControles(ativarControles);
        }


        protected void ConfigurarPaginacao(int qtdTotalDeRegistros, int paginaAtual)
        {
            objPaginacao.QuantidadeTotalRegistros = qtdTotalDeRegistros;
            objPaginacao.PaginaAtual = paginaAtual;
        }

        protected void MostrarPaginacao(bool FlagMostrar)
        {
            if (objPaginacao != null)
            {
                objPaginacao.Visible = FlagMostrar;
            }
        }

        protected void ChamarBuscar() {
            BuscarDados dados = objBuscar.GetBuscarDadosFiltro();
            ChamarBuscar(dados);
        }

        protected void ChamarBuscar(BuscarDados buscarDados)
        {
            ViewState["Dados"] = buscarDados;
            InternalChamarConsultar(buscarDados, 1);
        }

        private void InternalChamarConsultar(BuscarDados buscarDados, int pagina) {
            try
            {
                Consultar(buscarDados, pagina);
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                if (Request.QueryString["mostrarErro"] != null)
                {
                    throw ex;
                }
                base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString());
            }
            catch (Exception ex)
            {
                if (Request.QueryString["mostrarErro"] != null)
                {
                    throw ex;
                }
                Redecard.PN.Comum.SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            
        }

        #endregion

        #region Eventos
        protected void objPaginacao_onPaginacaoChanged(int pagina, EventArgs e)
        {
            InternalChamarConsultar((BuscarDados)ViewState["Dados"], pagina);
        }

        protected void objBuscar_onBuscar(BuscarDados buscarDados, EventArgs e)
        {
            ChamarBuscar(buscarDados);
        }


        protected void objBuscar_onDownloadExcel(BuscarDados Dados, EventArgs e)
        {
            // TODO Implementar Donwload Excel
        }

        protected void ddlRegistroPorPagina_SelectedIndexChanged(object sender, EventArgs e)
        {
            //esconde os resultas
            MostrarResultadoRelatorio(false);

            ChamarBuscar();
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            //GetPaginaVisitada()
            PaginaVisitada objPagina = base.GetPaginaVisitadaRedirecionar();
            if (objPagina.Nome == string.Empty)
            {
                return;
            }
            SPUtility.Redirect(objPagina.Nome, SPRedirectFlags.CheckUrl, this.Context);
        }
        #endregion

        #region Metodos abstratos

        protected abstract void Consultar(BuscarDados buscarDados, int pagina);

        /// <summary>
        /// Mostra os resultados do relatorio
        /// </summary>
        /// <param name="FlagMostrar">True = Mostrar, False = Não Mostrar</param>
        protected abstract void MostrarResultadoRelatorio(bool FlagMostrar);

        #endregion
    }
}
