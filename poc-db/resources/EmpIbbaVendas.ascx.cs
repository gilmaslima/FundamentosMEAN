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
    /// HomePage Segmentada - EMP/IBBA - Box Consulte Suas Vendas
    /// </summary>
    public partial class EmpIbbaVendas : BaseUserControl
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
        /// Verifica se usuário possui permissão para Acesso à Consulta por Transação
        /// </summary>
        private Boolean AcessoConsultaTransacao
        {
            get
            {
                //Código do serviço de Consulta por Transação
                //Int32 codigoServico = 10007;
                //Boolean possuiAcesso = base.ValidarServico(codigoServico);
                //return possuiAcesso;
                return Sessao.Contem() && 
                    this.ValidarPagina("/sites/fechado/extrato/Paginas/pn_ConsultaTransacao.aspx");
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
            txtDataInicial.Attributes["maxdate"] = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");
            txtDataFinal.Attributes["maxdate"] = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");

            //Valores iniciais dos campos de período
            txtDataInicial.Text = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
            txtDataFinal.Text = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");

            //Configuração de visualização do conteúdo do box conforme permissões de Relatório de Vendas do usuário
            mvwConteudo.SetActiveView(this.AcessoConsultaTransacao ? pnlConteudo : pnlAcessoNegado);
        }

        #endregion

        #region [ Eventos dos Controles ]

        /// <summary>
        /// Clique de Veja Mais para consulta de transação.
        /// Redireciona para a tela de Consulta por Transação.
        /// </summary>        
        protected void btnVejaMais_Click(object sender, EventArgs e)
        {
            String tipoVenda = rdnTipo.SelectedValue;
            DateTime? dataInicial = txtDataInicial.Text.ToDateTimeNull("dd/MM/yyyy");
            DateTime? dataFinal = txtDataFinal.Text.ToDateTimeNull("dd/MM/yyyy");
            Boolean consultaCartao = rdnTipoVendaCartao.Checked;
            Boolean consultaNsuCv = rdnTipoVendaNsuCv.Checked;
            Boolean consultaTid = rdnTipoVendaTid.Checked;
            Int32? numero = txtTipoVenda.Text.ToInt32Null();

            var qs = new QueryStringSegura();
            qs["TipoVenda"] = tipoVenda; //"C" (crédito) ou "D" (débito)
            qs["DataInicial"] = dataInicial.Value.ToString("dd/MM/yyyy");
            qs["DataFinal"] = dataFinal.Value.ToString("dd/MM/yyyy");
            qs["Numero"] = numero.Value.ToString();
            qs["NumeroPv"] = SessaoAtual.ToString();
            if (consultaCartao)
                qs["TipoConsulta"] = "Cartao";
            else if (consultaNsuCv)
                qs["TipoConsulta"] = "NsuCv";
            else if (consultaTid)
                qs["TipoConsulta"] = "TID";

            //Redireciona para tela Extrato - Consulta por Transação
            RedirecionarPaginaPN("extrato", "pn_ConsultaTransacao.aspx", qs);
        }

        #endregion
    }
}
