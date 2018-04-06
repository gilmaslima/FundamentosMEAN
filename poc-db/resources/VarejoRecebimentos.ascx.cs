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
    /// HomePage Segmentada - Varejo - Box de Recebimentos
    /// </summary>
    public partial class VarejoRecebimentos : BaseUserControl
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
        /// Verifica se usuário possui permissão para Acesso ao Relatório de Lançamentos Futuros
        /// </summary>
        private Boolean AcessoLancamentosFuturos
        {
            get
            {
                //Código do serviço do Relatório de Lançamentos Futuros
                //Int32 codigoServico = 10071;
                //Boolean possuiAcesso = base.ValidarServico(codigoServico);
                //return possuiAcesso;
                return Sessao.Contem() &&
                    this.ValidarPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=4");
            }
        }

        /// <summary>
        /// Verifica se usuário possui permissão para Acesso ao Relatório de Valores Pagos
        /// </summary>
        private Boolean AcessoValoresPagos
        {
            get
            {
                //Código do serviço do Relatório de Valores Pagos
                //Int32 codigoServico = 10070;
                //Boolean possuiAcesso = base.ValidarServico(codigoServico);
                //return possuiAcesso;
                return Sessao.Contem() && this.ValidarPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=1");
            }
        }

        #region [ Eventos da Página ]

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
            txtDataFinalLancamentosFuturos.Attributes["readonly"] = "readonly";
            txtDataFinalValoresPagos.Attributes["readonly"] = "readonly";

            //Configuração de limite de datas no calendário
            txtDataInicialLancamentosFuturos.Attributes["mindate"] = DateTime.Today.AddDays(1).ToString("yyyyMMdd");
            txtDataInicialLancamentosFuturos.Attributes["maxdate"] = DateTime.Today.AddDays(366).ToString("yyyyMMdd");
            txtDataInicialValoresPagos.Attributes["mindate"] = DateTime.Today.AddDays(-366).ToString("yyyyMMdd");
            txtDataInicialValoresPagos.Attributes["maxdate"] = DateTime.Today.ToString("yyyyMMdd");

            //Valores iniciais dos campos de período
            if (txtDataInicialLancamentosFuturos.Text.EmptyToNull() == null)
                txtDataInicialLancamentosFuturos.Text = DateTime.Today.AddDays(1).ToString("dd/MM/yyyy");
            if (txtDataFinalLancamentosFuturos.Text.EmptyToNull() == null)
                txtDataFinalLancamentosFuturos.Text = DateTime.Today.AddDays(5).ToString("dd/MM/yyyy");
            if (txtDataInicialValoresPagos.Text.EmptyToNull() == null)
                txtDataInicialValoresPagos.Text = DateTime.Today.AddDays(-5).ToString("dd/MM/yyyy");
            if (txtDataFinalValoresPagos.Text.EmptyToNull() == null)
                txtDataFinalValoresPagos.Text = DateTime.Today.ToString("dd/MM/yyyy");

            //Configuração de visualização do conteúdo da aba 'Lançamentos Futuros' conforme permissões do usuário
            mvwLancamentosFuturos.SetActiveView(this.AcessoLancamentosFuturos ?
                pnlConteudoLancamentosFuturos : pnlAcessoNegadoLancamentosFuturos);

            //Configuração de visualização do conteúdo da aba 'Valores Pagos' conforme permissões do usuário
            mvwValoresPagos.SetActiveView(this.AcessoValoresPagos ?
                pnlConteudoValoresPagos : pnlAcessoNegadoValoresPagos);

            //Carrega os atalhos na Home
            fieldSetAtalhos.Visible = CarregarAtalhosHome(rptAtalhos, this.ConfiguracaoAtalhos);
        }

        #endregion

        #region [ Eventos dos Controles ]

        /// <summary>
        /// Clique para redirecionamento para o Relatório de Lançamentos
        /// Futuros - Crédito.
        /// </summary>        
        protected void btnContinuarLancamentosFuturos_Click(object sender, EventArgs e)
        {
            //Redireciona para Relatório de Lançamentos Futuros - Crédito
            this.RedirecionarRelatorio(TipoRelatorio.LancamentosFuturos, TipoVenda.Credito,
               txtDataInicialLancamentosFuturos.Text.ToDate("dd/MM/yyyy"),
               txtDataFinalLancamentosFuturos.Text.ToDate("dd/MM/yyyy"), true);
        }

        /// <summary>
        /// Clique para redirecionamento para o Relatório de Valores Pagos - Crédito
        /// </summary>        
        protected void btnContinuarValoresPagos_Click(object sender, EventArgs e)
        {
            //Redireciona para Relatório de Valores Pagos - Crédito
            this.RedirecionarRelatorio(TipoRelatorio.ValoresPagos, TipoVenda.Credito,
               txtDataInicialValoresPagos.Text.ToDate("dd/MM/yyyy"),
               txtDataFinalValoresPagos.Text.ToDate("dd/MM/yyyy"), true);
        }

        #endregion
    }
}