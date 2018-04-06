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
    /// HomePage Segmentada - EMP/IBBA - Box do Cancelamento de Vendas
    /// </summary>
    public partial class EmpIbbaCancelamentoLote : BaseUserControl
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

            //Configuração de visualização do conteúdo do box conforme permissões do usuário            
            mvwConteudo.SetActiveView(this.AcessoCancelamento ? pnlConteudo : pnlAcessoNegado);
        }

        #endregion

        #region [ Eventos dos Controles ]

        /// <summary>
        /// Evento de clique para envio do arquivo de Cancelamento em lote.
        /// Redireciona para tela de Cancelamento de Vendas em Lote.
        /// </summary>        
        protected void btnEnviarArquivo_Click(object sender, EventArgs e)
        {
            if (fupCancelamentoLote.HasFile)
            {
                //Verifica tamanho do arquivo (máx 1MB)
                if (fupCancelamentoLote.PostedFile.ContentLength / 1024 > 1024)
                {
                    mensagem.InnerHtml = "O arquivo deve ter no máximo 1 MB.";
                    return;
                }

                //Guid para armazenamento de arquivo em Sessão
                String guidLoteHome = Guid.NewGuid().ToString("N");
                
                //Montagem de querystring para redirecionamento para tela de Cancelamento em Lote
                var qs = new QueryStringSegura();
                qs["Origem"] = "HomePageCancelamentoLote";
                qs["GuidLote"] = guidLoteHome;
                qs["NomeArquivo"] = fupCancelamentoLote.PostedFile.FileName;

                //Armazena arquivo na sessão do usuário
                Session[guidLoteHome] = fupCancelamentoLote.FileBytes;

                //Redireciona para Cancelamento de Vendas
                //Se já existir o serviço da nova página de Cancelamento (PN.CancelamentoDebito)
                if (this.NovoCancelamento)
                    RedirecionarPaginaPN("servicos", "SolicitarCancelamento.aspx", qs);
                //Caso contrário, redireciona para a primeira versão do Cancelamento (PN.Cancelamento)
                else
                    RedirecionarPaginaPN("servicos", "pn_CancelamentoVendas.aspx", qs);
            }
        }

        #endregion
    }
}