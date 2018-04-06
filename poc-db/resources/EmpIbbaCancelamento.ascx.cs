/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Linq;
using System.Web.UI;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.HomePage
{
    /// <summary>
    /// HomePage Segmentada - EMP/IBBA - Box de Cancelamento de Vendas
    /// </summary>
    public partial class EmpIbbaCancelamento : BaseUserControl
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

        #endregion

        /// <summary>
        /// Verifica se usuário possui permissão para Acesso ao Cancelamento de Vendas
        /// </summary>
        private Boolean AcessoCancelamento
        {
            get
            {
                //Código do Serviço de Cancelamento de Vendas
                //Int32 codigoServico = 10037;
                //Boolean possuiAcesso = base.ValidarServico(codigoServico);
                //return possuiAcesso;
                if (this.NovoCancelamento)
                    return Sessao.Contem() && 
                        this.ValidarPagina("/sites/fechado/servicos/Paginas/SolicitarCancelamento.aspx");
                else
                    return Sessao.Contem() && 
                        this.ValidarPagina("/sites/fechado/servicos/Paginas/pn_cancelamentovendas.aspx");
            }
        }

        /// <summary>
        /// Boolean indicando se existe o Novo Cancelamento (Rede.PN.Cancelamento)
        /// Cancelamento antigo: Redecard.PN.Cancelamento
        /// </summary>
        private Boolean NovoCancelamento
        {
            get
            {
                String url = ProcurarUrlPaginaPN(SessaoAtual, "servicos", "SolicitarCancelamento.aspx").FirstOrDefault();
                if (!String.IsNullOrEmpty(url))
                    return true;
                else
                    return false;
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

            //Configuração de limite de datas no calendário
            txtDataVenda.Attributes["maxdate"] = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");
            txtDataVenda.Attributes["mindate"] = DateTime.Today.AddDays(-366).ToString("yyyyMMdd");

            //Se tiver o Novo Cancelamento (PN.CancelamentoDebito), exibe opção Débito, senão, apenas Crédito
            ltrTipoVendaCredito.Visible = !this.NovoCancelamento;
            rbnTipoVenda.Visible = this.NovoCancelamento;

            //Configuração de visualização do conteúdo do box conforme permissões do usuário            
            mvwConteudo.SetActiveView(this.AcessoCancelamento ? pnlConteudo : pnlAcessoNegado);
        }

        #endregion

        #region [ Eventos de Controles ]

        /// <summary>
        /// Evento do botão Cancelar Vendas. 
        /// Redireciona para tela de Cancelamento de Vendas
        /// </summary>        
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            String numeroPv = txtEstabelecimento.Text;
            String tipoVenda = rbnTipoVenda.SelectedValue;
            String numeroCartaoCvNsu = txtNsuCvCartao.Text;
            String dataVenda = txtDataVenda.Text;
            String valorVenda = txtValorVenda.Text;

            var qs = new QueryStringSegura();
            qs["Origem"] = "HomePageCancelamento";
            qs["NumeroPV"] = numeroPv;
            qs["TipoVenda"] = tipoVenda; //"Credito" | "Debito"
            qs["Numero"] = numeroCartaoCvNsu;
            qs["DataVenda"] = dataVenda; // "dd/MM/yyyy"
            qs["ValorVenda"] = valorVenda; // N2

            //Redireciona para Cancelamento de Vendas
            //Se já existir o serviço da nova página de Cancelamento (PN.CancelamentoDebito)
            if (this.NovoCancelamento)
                RedirecionarPaginaPN("servicos", "SolicitarCancelamento.aspx", qs);
            //Caso contrário, redireciona para a primeira versão do Cancelamento (PN.Cancelamento)
            else
                RedirecionarPaginaPN("servicos", "pn_CancelamentoVendas.aspx", qs);
        }

        #endregion
    }
}