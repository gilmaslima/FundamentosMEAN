/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.WAExtratoServicosServico;
using System;
using System.Web;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.Servicos
{
    /// <summary>
    /// Relatório de Serviços
    /// </summary>
    public partial class RelatorioServicos : BaseUserControl, IRelatorioHandler
    {
        #region [ Controles ]

        /// <summary>boxSerasa control.</summary>
        protected Serasa BoxSerasa { get { return (Serasa)boxSerasa; } }

        /// <summary>boxAvs control.</summary>
        protected Avs BoxAvs { get { return (Avs)boxAvs; } }

        /// <summary>boxGateway control.</summary>
        protected Gateway BoxGateway { get { return (Gateway)boxGateway; } }

        /// <summary>boxAnaliseRisco control.</summary>
        protected AnaliseRisco BoxAnaliseRisco { get { return (AnaliseRisco)boxAnaliseRisco; } }

        /// <summary>boxBoleto control.</summary>
        protected Boleto BoxBoleto { get { return (Boleto)boxBoleto; } }

        /// <summary>boxManualReview control.</summary>
        protected ManualReview BoxManualReview { get { return (ManualReview)boxManualReview; } }

        /// <summary>boxNovoPacote control.</summary>
        protected NovoPacote BoxNovoPacote { get { return (NovoPacote)boxNovoPacote; } }

        /// <summary>boxRecargaCelular control.</summary>
        protected RecargaCelular BoxRecargaCelular { get { return (RecargaCelular)boxRecargaCelular; } }

        #endregion

        #region [ Propriedades ViewState ]

        /// <summary>Guid de Pesquisa do box Serasa/AVS</summary>
        private Guid GuidSerasaAvs
        {
            get
            {
                if (ViewState["GuidSerasaAvs"] == null)
                    ViewState["GuidSerasaAvs"] = Guid.NewGuid();
                return (Guid)ViewState["GuidSerasaAvs"];
            }
            set { ViewState["GuidSerasaAvs"] = value; }
        }
        /// <summary>Guid de Pesquisa do box Gateway</summary>
        private Guid GuidGateway
        {
            get
            {
                if (ViewState["GuidGateway"] == null)
                    ViewState["GuidGateway"] = Guid.NewGuid();
                return (Guid)ViewState["GuidGateway"];
            }
            set { ViewState["GuidGateway"] = value; }
        }
        /// <summary>Guid de Pesquisa do box Análise Risco</summary>
        private Guid GuidAnaliseRisco
        {
            get
            {
                if (ViewState["GuidAnaliseRisco"] == null)
                    ViewState["GuidAnaliseRisco"] = Guid.NewGuid();
                return (Guid)ViewState["GuidAnaliseRisco"];
            }
            set { ViewState["GuidAnaliseRisco"] = value; }
        }
        /// <summary>Guid de Pesquisa do box Boleto</summary>
        private Guid GuidBoleto
        {
            get
            {
                if (ViewState["GuidBoleto"] == null)
                    ViewState["GuidBoleto"] = Guid.NewGuid();
                return (Guid)ViewState["GuidBoleto"];
            }
            set { ViewState["GuidBoleto"] = value; }
        }
        /// <summary>Guid de Pesquisa do box Manual Review</summary>
        private Guid GuidManualReview
        {
            get
            {
                if (ViewState["GuidManualReview"] == null)
                    ViewState["GuidManualReview"] = Guid.NewGuid();
                return (Guid)ViewState["GuidManualReview"];
            }
            set { ViewState["GuidManualReview"] = value; }
        }
        /// <summary>Guid de Pesquisa do box Novo Pacote</summary>
        private Guid GuidNovoPacote
        {
            get
            {
                if (ViewState["GuidNovoPacote"] == null)
                    ViewState["GuidNovoPacote"] = Guid.NewGuid();
                return (Guid)ViewState["GuidNovoPacote"];
            }
            set { ViewState["GuidNovoPacote"] = value; }
        }
        /// <summary>Guid de Pesquisa do box Recarga de Celular</summary>
        private Guid GuidRecargaCelular
        {
            get
            {
                if (ViewState["GuidRecargaCelular"] == null)
                    ViewState["GuidRecargaCelular"] = Guid.NewGuid();
                return (Guid)ViewState["GuidRecargaCelular"];
            }
            set { ViewState["GuidRecargaCelular"] = value; }
        }

        #endregion

        /// <summary>
        /// Efetua a consulta do relatório
        /// </summary>
        /// <param name="dados">Parâmetros da consulta</param>
        public void Pesquisar(BuscarDados dados)
        {
            GravarBuscarDados(dados);

            //Carrega todos os boxes
            Int32 qtdSerasa = BoxSerasa.Carregar(GuidSerasaAvs, dados);
            Int32 qtdAvs = BoxAvs.Carregar(GuidSerasaAvs, dados);
            Int32 qtdGateway = BoxGateway.Carregar(GuidGateway, dados);
            Int32 qtdAnaliseRisco = BoxAnaliseRisco.Carregar(GuidAnaliseRisco, dados);
            Int32 qtdBoleto = BoxBoleto.Carregar(GuidBoleto, dados);
            Int32 qtdManualReview = BoxManualReview.Carregar(GuidManualReview, dados);
            Int32 qtdNovoPacote = BoxNovoPacote.Carregar(GuidNovoPacote, dados);
            Int32 qtdRecargaCelular = BoxRecargaCelular.Carregar(GuidRecargaCelular, dados);

            //Verifica os controles que devem estar visíveis
            base.VerificaControlesVisiveis(qtdSerasa + qtdAvs + qtdGateway + qtdAnaliseRisco + 
                qtdBoleto + qtdManualReview + qtdNovoPacote + qtdRecargaCelular, null, null);
        }

        /// <summary>
        /// Evento do botão Voltar
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            //Redirecionamento para a página "Home" dos relatórios
            Response.Redirect("pn_default.aspx", false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        #region [ Implementação Interfaces ]

        /// <summary>
        /// Gera conteúdo do relatório para exportação
        /// </summary>
        /// <param name="dados">Parâmetros da busca</param>
        /// <param name="quantidadeRegistros">Quantidade máxima de registros por relatório</param>
        /// <param name="incluirTotalizadores">Incluir totalizadores?</param>
        /// <returns>Conteúdo do relatório para exportação</returns>
        public string ObterTabelaExcel(BuscarDados dados, Int32 quantidadeRegistros, Boolean incluirTotalizadores)
        {
            //Carrega os boxes, e prepara para exportação
            BoxSerasa.PrepararParaExportacao(GuidSerasaAvs, dados, quantidadeRegistros);
            BoxAvs.PrepararParaExportacao(GuidSerasaAvs, dados, quantidadeRegistros);
            BoxGateway.PrepararParaExportacao(GuidGateway, dados, quantidadeRegistros);
            BoxAnaliseRisco.PrepararParaExportacao(GuidAnaliseRisco, dados, quantidadeRegistros);
            BoxBoleto.PrepararParaExportacao(GuidBoleto, dados, quantidadeRegistros);
            BoxManualReview.PrepararParaExportacao(GuidManualReview, dados, quantidadeRegistros);
            BoxNovoPacote.PrepararParaExportacao(GuidNovoPacote, dados, quantidadeRegistros);
            BoxRecargaCelular.PrepararParaExportacao(GuidRecargaCelular, dados, quantidadeRegistros);

            //Renderiza conteúdo para exportação
            return base.RenderizarControles(true, boxSerasa, boxAvs, boxGateway, 
                boxAnaliseRisco, boxBoleto, boxManualReview, boxNovoPacote, boxRecargaCelular);
        }

        /// <summary>Identificação do relatório</summary>
        public string IdControl { get { return "RelatorioServicos"; } }

        #endregion

        #region [ Métodos Estáticos Auxiliares ]

        /// <summary>
        /// Valida retorno das consultas
        /// </summary>
        /// <param name="status">Objeto de status retornado pelas consultas dos boxes</param>
        /// <returns>Booleano indicando se o código de retorno corresponde à consutla efetuada com sucesso.</returns>
        internal static Boolean ValidaRetorno(StatusRetorno status)
        {
            return status.CodigoRetorno == 0 || status.CodigoRetorno == 10 || status.CodigoRetorno == 60;
        }

        #endregion
    }
}