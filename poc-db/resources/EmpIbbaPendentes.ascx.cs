/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Web.UI;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.HomePage
{
    /// <summary>
    /// HomePage Segmentada - EMP/IBBA - Box de Comprovantes Pendentes
    /// </summary>
    public partial class EmpIbbaPendentes : BaseUserControl
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
        /// Verifica se usuário possui permissão para Acesso aos Comprovantes Pendentes
        /// </summary>
        private Boolean AcessoComprovantesPendentes
        {
            get
            {
                //Código do serviço de Comprovantes Pendentes
                //Int32 codigoServico = 10040;
                //Boolean possuiAcesso = base.ValidarServico(codigoServico);
                //return possuiAcesso;
                return Sessao.Contem() && 
                    this.ValidarPagina("/sites/fechado/servicos/Paginas/pn_ComprovantesPendentes.aspx");
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

            //Configuração de visualização do conteúdo do box conforme permissões de Relatório de Vendas do usuário
            mvwConteudo.SetActiveView(this.AcessoComprovantesPendentes ? pnlConteudo : pnlAcessoNegado);
        }

        #endregion

        #region [ Eventos dos Controles ]

        /// <summary>
        /// Clique no botão continua: redireciona para tela de Comprovantes
        /// Pendentes, conforme seleção do tipo de Venda (crédito ou débito)
        /// </summary>        
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            QueryStringSegura qs = new QueryStringSegura();
            qs["TipoVenda"] = rdnTipoVenda.SelectedValue; //"Credito" ou "Debito"
            
            //Redireciona para tela de Comprovação de Vendas - Comprovantes Pendentes
            RedirecionarPaginaPN("servicos", "pn_ComprovantesPendentes.aspx", qs);
        }

        #endregion
    }
}