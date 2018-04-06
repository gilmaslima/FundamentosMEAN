/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Web.UI;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.Modelo;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.HomePage
{
    /// <summary>
    /// HomePage Segmentada - Varejo - Box de Vendas
    /// </summary>
    public partial class VarejoVendas : BaseUserControl
    {
        #region [ Propriedades Públicas ]

        /// <summary>
        /// Getter/Setter para atributo "class" do controle
        /// </summary>
        [CssClassProperty]
        public String CssClass
        {
            get { return pnlControle.CssClass; }
            set { pnlControle.CssClass = value; }
        }

        /// <summary>
        /// String de configuração dos atalhos do box
        /// </summary>
        public String ConfiguracaoAtalhos { get; set; }

        #endregion

        /// <summary>
        /// Verifica se usuário possui permissão para Acesso ao Relatório de Vendas
        /// </summary>
        private Boolean AcessoVendas
        {
            get
            {
                //Código do Serviço do Relatório de Vendas
                //Int32 codigoServico = 10069;
                //Boolean possuiAcesso = base.ValidarServico(codigoServico);
                //return possuiAcesso;
                return Sessao.Contem() && 
                    this.ValidarPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=0");
            }
        }

        #region [ Eventos das Página ]

        /// <summary>
        /// Load da página
        /// </summary>        
        protected void Page_Load(object sender, EventArgs e)
        {
            //Não precisa validar userControl
            ValidarPermissao = false;

            //Configuração de data final somente leitura
            //Configurado via server-side como atributo HTML pois c.c. não é possível 
            //recuperar o valor setado por JS nos campos
            txtDataFinalCredito.Attributes["readonly"] = "readonly";
            txtDataFinalDebito.Attributes["readonly"] = "readonly";

            //Configuração de limite de datas no calendário
            txtDataInicialCredito.Attributes["mindate"] = DateTime.Today.AddDays(-366).ToString("yyyyMMdd");
            txtDataInicialCredito.Attributes["maxdate"] = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");
            txtDataInicialDebito.Attributes["mindate"] = DateTime.Today.AddDays(-181).ToString("yyyyMMdd");
            txtDataInicialDebito.Attributes["maxdate"] = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");

            //Valores iniciais dos campos de período
            if(txtDataInicialCredito.Text.EmptyToNull() == null)
                txtDataInicialCredito.Text = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
            if (txtDataFinalCredito.Text.EmptyToNull() == null)
                txtDataFinalCredito.Text = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
            if (txtDataInicialDebito.Text.EmptyToNull() == null)
                txtDataInicialDebito.Text = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
            if (txtDataFinalDebito.Text.EmptyToNull() == null)
                txtDataFinalDebito.Text = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");

            //Configuração de visualização do conteúdo do box conforme permissões de Relatório de Vendas do usuário
            mvwConteudo.SetActiveView(this.AcessoVendas ? pnlConteudo : pnlAcessoNegado);

            //Carrega os atalhos na Home
            fieldsetAtalhos.Visible = CarregarAtalhosHome(rptAtalhos, this.ConfiguracaoAtalhos);
        }

        #endregion

        #region [ Eventos dos Controles ]

        /// <summary>
        /// Clique para redirecionamento para o Relatório de Vendas - Crédito.
        /// </summary>        
        protected void btnVejaMaisVendasCredito_Click(object sender, EventArgs e)
        {
            //Redireciona para relatório de Vendas - Crédito
            this.RedirecionarRelatorio(TipoRelatorio.Vendas, TipoVenda.Credito, 
                txtDataInicialCredito.Text.ToDate("dd/MM/yyyy"),
                txtDataFinalCredito.Text.ToDate("dd/MM/yyyy"), true);
        }

        /// <summary>
        /// Clique para redirecionamento para o Relatório de Vendas - Débito
        /// </summary>        
        protected void btnVejaMaisVendasDebito_Click(object sender, EventArgs e)
        {
            //Redireciona para Relatório de Vendas - Débito
            this.RedirecionarRelatorio(TipoRelatorio.Vendas, TipoVenda.Debito,
                txtDataInicialDebito.Text.ToDate("dd/MM/yyyy"),
                txtDataFinalDebito.Text.ToDate("dd/MM/yyyy"), true);
        }

        #endregion
    }
}