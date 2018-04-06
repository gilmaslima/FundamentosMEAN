using System;
using System.Web.UI;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Extrato.SharePoint.WebParts.Relatorios;
using System.Collections.Generic;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato;
using Redecard.PN.Comum;


namespace Redecard.PN.Extrato.SharePoint.WebParts.RelatorioEmissores
{
    public partial class RelatorioEmissoresUserControl : BaseUserControl, IPostBackDataHandler
    { /// <summary>
        /// 
        /// </summary>
        protected DynamicControlsPlaceholder pnlRelatorioControl;

        /// <summary>
        /// Fonte dos erros de scripts
        /// </summary>
        public String _fonteErro = "Redecard.PN.Extrato";

        /// <summary>
        /// Estrutura que representa um relatório de Extrato
        /// </summary>
        protected struct Relatorio
        {
            public Int32 IDRelatorio;
            public Int32 IDTipoVenda;
            public String ControleRelatorio;
            public String Nome;
        }

        /* 
        * Tabela de Relátorios e Tipos de Venda
        * -----------------------------------------------------
        * Relatórios
        *      Item                            Valor
        * -----------------------------------------------------
        *      Débitos e Desagendamentos       6
        *      Suspensos Penhorados Retidos    8
        * -----------------------------------------------------
        * Tipos de Venda
        *      Item                            Valor
        * -----------------------------------------------------
        *      Crédito                         0
        *      Débito                          1
        *      Construcard                     2
        * -----------------------------------------------------
        */

        /// <summary>
        /// Listagem de relatório disponíveis no extrato
        /// </summary>
        protected List<Relatorio> _relatorios = new List<Relatorio>
        {
            //  Débitos e Desagendamentos
            new Relatorio() { IDRelatorio = 6, IDTipoVenda = 0, ControleRelatorio = "~/_controltemplates/Redecard.PN.Extrato/RelatorioDebitosDesagendamentosUserControl.ascx", Nome = "Débitos e Desagendamentos" },
            //Créditos Suspensos, Retidos e Penhorados
            new Relatorio() { IDRelatorio = 8, IDTipoVenda = 0, ControleRelatorio = "~/_controltemplates/Redecard.PN.Extrato/CredSuspRetPenUserControl.ascx", Nome = "Créditos Suspensos, Retidos e Penhorados" }
        };

        /// <summary>
        /// Converte o relatório de uma tabela HTML para uma tabela do
        /// excel
        /// </summary>
        protected void DownloadExcel(object sender, EventArgs e)
        {
            String html = String.Empty;
            BuscarDados dados = ((FiltroEmissores)filtroControl).RecuperarBuscarDadosDTO();
            if (!object.ReferenceEquals(dados, null))
            {
                Relatorio _relatorio = _relatorios.FirstOrDefault(r => r.IDRelatorio == dados.IDRelatorio && r.IDTipoVenda == dados.IDTipoVenda);

                using (Logger Log = Logger.IniciarLog("Download Excel do Relatório " + _relatorio.Nome))
                {
                    if (!String.IsNullOrEmpty(_relatorio.ControleRelatorio))
                    {
                        try
                        {
                            // Redirecionar 
                            QueryStringSegura queryString = new QueryStringSegura();
                            queryString.Add("MAXLINHAS", MAX_LINHAS_DOWNLOAD.ToString());
                            queryString.Add("SRC", _relatorio.ControleRelatorio);

                            Logger.GravarLog("Redirecionando para página de Download Excel", new { MAX_LINHAS_DOWNLOAD, SRC = _relatorio.ControleRelatorio, dados });

                            Session.Add("DADOS", dados);
                            Response.Redirect("/_layouts/Redecard.PN.Extrato.SharePoint/RelatorioExcel.aspx?dados=" + queryString.ToString(), true);
                        }
                        catch (Exception ex)
                        {
                            Log.GravarErro(ex);
                            SharePointUlsLog.LogErro(ex);
                        }
                    }
                    else
                    {
                        Log.GravarMensagem("Relatório indisponível");
                        // Relatório indisponível
                        this.ExibirErro(_fonteErro, 310);
                    }
                }
            }
            else
            {
                // Relatório indisponível
                this.ExibirErro(_fonteErro, 310);
            }
        }

        /// <summary>
        /// Carregamento da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            String target = Request["__EVENTTARGET"];
            if (IsPostBack && !object.ReferenceEquals(this.SessaoAtual, null))
            {
                // colocar data e hora da consulta na tela de impressão
                DateTime data = DateTime.Now;
                hdnDataHora.Value = String.Format("Data da consulta {0} às {1}", data.ToString("dd/MM/yyyy"), data.ToString("HH:mm"));

                // Exportar para o excel
                if (target.EndsWith("linkExcel"))
                    return;

                //Busca o controle de filtro
                FiltroEmissores objFiltro = ((FiltroEmissores)filtroControl);

                //Verifica se está válido
                if (objFiltro.ValidarFiltro())
                    Buscar(objFiltro.RecuperarBuscarDadosDTO(), target.Equals("filtroControl_btnBuscar"));
                else
                    pnlRelatorioControl.Controls.Clear();

#if DEBUG
                //Se em DEBUG, sempre exibe botão para envio por e-mail
                pnlEnviarPorEmail.Visible = true;
#else
                // Verificar se é central de atendimento, caso positivo, exibir o link de enviar por e-mail
                pnlEnviarPorEmail.Visible = this.SessaoAtual != null && this.SessaoAtual.UsuarioAtendimento;
#endif
            }
        }

        protected void linkExcel_Click(object sender, EventArgs e)
        {
            DownloadExcel(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        protected Control Buscar(BuscarDados Dados, Boolean pesquisar)
        {
            if (!object.ReferenceEquals(Dados, null))
            {
                Relatorio _relatorio = _relatorios.FirstOrDefault(r => r.IDRelatorio == Dados.IDRelatorio && r.IDTipoVenda == Dados.IDTipoVenda);
                using (Logger Log = Logger.IniciarLog("Pesquisa do Relatório " + _relatorio.Nome))
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(_relatorio.ControleRelatorio))
                        {
                            Logger.GravarLog(String.Format("Pesquisa Relatório ID {0} / {1}",
                                _relatorio.IDRelatorio, _relatorio.ControleRelatorio), new { Dados, pesquisar });

                            pnlRelatorioControl.ASCXPath = _relatorio.ControleRelatorio;

                            if (pesquisar)
                            {

                                pnlRelatorioControl.Controls.Clear();
                                UserControl control = (UserControl)Page.LoadControl(_relatorio.ControleRelatorio);
                                control.ID = ((IRelatorioHandler)control).IdControl;

                                pnlRelatorioControl.Controls.Add(control);
                                ((IRelatorioHandler)control).Pesquisar(Dados);

                                ((BaseUserControl)control).OrigemEmissores = "S";

                                Log.GravarMensagem("Relatório pesquisado com sucesso");

                                return control;
                            }
                        }
                        else
                        {
                            Log.GravarMensagem("Relatório indisponível");
                            // Relatório indisponível
                            this.ExibirErro(_fonteErro, 310);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);
                        this.ExibirErro(_fonteErro, 313);
                    }
                }
            }
            else
            {
                // Relatório indisponível
                this.ExibirErro(_fonteErro, 310);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fonte"></param>
        /// <param name="codigo"></param>
        private void ExibirErro(String fonte, Int32 codigo)
        {
            base.ExibirPainelExcecao(fonte, codigo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postDataKey"></param>
        /// <param name="postCollection"></param>
        /// <returns></returns>
        public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RaisePostDataChangedEvent()
        {
        }

    }
}
