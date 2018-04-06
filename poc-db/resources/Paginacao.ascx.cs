/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 27/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.ComponentModel;

namespace Redecard.PN.FMS.Sharepoint.ControlTemplates
{
    /// <summary>
    /// Este componente publica a classe Paginacao, estendidade de BaseUserControl, que expõe métodos para exibir a paginação
    /// </summary>
    public partial class Paginacao : BaseUserControl
    {
        #region Atributos e Propriedades
        public int RegistrosPorPagina { get; set; }
        public long QuantidadeTotalRegistros
        {
            get { return Convert.ToInt64(ViewState["QuantidadeTotalRegistros"] == null ? 0 : ViewState["QuantidadeTotalRegistros"]); }
            set {
                ViewState["QuantidadeTotalRegistros"] = value <= 0 ? 0 : value;
                if (RegistroInicialPaginaAtual > value)
                {
                    PaginaAtual = TotalPagina;
                }
            }
        }
        public int PaginaAtual
        {
            get { return Convert.ToInt32(ViewState["PaginaAtual"] == null ? 1 : ViewState["PaginaAtual"]); }
            set { ViewState["PaginaAtual"] = value < 1 ? 1 : value; }
        }
        public int TotalPagina { get { return ObterTotalDePaginas(); } }

        public int RegistroInicialPaginaAtual
        {
            get { 
                return ((PaginaAtual -1) * RegistrosPorPagina) + 1;
            }
        }


        #endregion Atributos e Propriedades

        #region Eventos
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuantidadeTotalRegistros = 0;
            PaginaAtual = 0;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            MontaPaginacao();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);


            if (QuantidadeTotalRegistros != 0)
            {
                MontaPaginacao();
            }
        }

        protected void lnkAnterior_Click(object sender, EventArgs e)
        {
            int iPaginaAnterior = PaginaAtual - 1;

            //verifica se jã não é a primeira pagina
            if (iPaginaAnterior < 1)
            {
                iPaginaAnterior = 1;
            }

            PaginaAtual = iPaginaAnterior;
            PaginacaoChangedTrigger(e);
        }

        protected void lnkProxima_Click(object sender, EventArgs e)
        {
            int iPaginaProxima = PaginaAtual + 1;

            //verifica se não é a ultima pagina
            if (iPaginaProxima > this.TotalPagina)
            {
                iPaginaProxima = this.TotalPagina;
            }

            PaginaAtual = iPaginaProxima;
            PaginacaoChangedTrigger(e);
        }

        private void PaginacaoChangedTrigger(EventArgs e)
        {
            if (onPaginacaoChanged != null)
            {
                onPaginacaoChanged(PaginaAtual, e);
            }
        }

        protected void lnkAnteriorUltima_Click(object sender, EventArgs e)
        {
            int iPaginaAnterior = 1;
            PaginaAtual = iPaginaAnterior;
            PaginacaoChangedTrigger(e);
        }

        protected void lnkProximaUltima_Click(object sender, EventArgs e)
        {
            int iPaginaProxima = TotalPagina;
            PaginaAtual = iPaginaProxima;
            PaginacaoChangedTrigger(e);
        }

        protected void SelectedPage_Selected(object sender, EventArgs e)
        {
            int iPagina = Convert.ToInt32(((Button)sender).Text.Trim());
            PaginaAtual = iPagina;
            PaginacaoChangedTrigger(e);
        }
        #endregion Eventos

        #region Metodos
        /// <summary>
        /// Este método é utilizado para  obter o total de paginas.
        /// </summary>
        /// <returns></returns>
        private Int32 ObterTotalDePaginas()
        {
            if (RegistrosPorPagina == 0 || QuantidadeTotalRegistros == 0)
            {
                return 1;
            }
            int iTotalPagina = (int)Math.Ceiling((double)QuantidadeTotalRegistros / RegistrosPorPagina);
            return iTotalPagina <= 0?0:iTotalPagina;

        }

        /// <summary>
        /// Monta o numero de paginas necessarias para a navegação
        /// </summary>
        /// <param name="TotalRegistros">Numero total de registros</param>
        private void MontaPaginacao()
        {
            spanRollOverPagination.Controls.Clear();


            Button objLinkButton;

            Int32 iTotalPagina = this.TotalPagina;

            int iMaxPaginasTela = 15;
            int iInicio = 1;
            int iFim = iTotalPagina;
            int iPaginaAtual = PaginaAtual;

            if (iTotalPagina > iMaxPaginasTela)
            {
                int iMaxPaginasDiv2 = (int)(iMaxPaginasTela / 2);
                iInicio = iPaginaAtual - iMaxPaginasDiv2;
                if (iInicio < 1)
                {
                    iInicio = 1;
                }
                iFim = iInicio + iMaxPaginasTela;
                if (iFim > iTotalPagina)
                {
                    iFim = iTotalPagina;
                    iInicio = iFim - iMaxPaginasTela;
                }
            }
            for (int i = iInicio; i <= iFim; i++)
            {
                objLinkButton = new Button();
                objLinkButton.Text = " " + (i).ToString() + " ";
                objLinkButton.Click += new EventHandler(SelectedPage_Selected);
                objLinkButton.ID = "lnkPaginacao" + (i).ToString();
                objLinkButton.EnableViewState = true;
                objLinkButton.CssClass = "botaoPaginacao";

                if (iPaginaAtual == i)
                {
                    objLinkButton.Enabled = false;
                    objLinkButton.CssClass = "botaoPaginacaoPaginaAtual";
                }
                spanRollOverPagination.Controls.Add(objLinkButton);
            }

            if (iTotalPagina <= 1)
            {
                tblPaginacao.Visible = false;
            }
            else
            {
                tblPaginacao.Visible = true;
            }

            if (iPaginaAtual > 0 && iTotalPagina > 0)
            {
                divPaginas.InnerText = String.Format("Página {0:N0} de {1:N0}", iPaginaAtual, iTotalPagina);
                divPaginas.Visible = true;
            }
            else
            {
                divPaginas.Visible = false;
            }
        }
        #endregion Metodos

        #region Delegates e Eventos
        public delegate void PaginacaoChanged(int pagina, EventArgs e);
        [Browsable(true)]
        public event PaginacaoChanged onPaginacaoChanged;
        #endregion Delegates e Eventos
        /// <summary>
        /// Este método é utilizado para indicar se os parâmetros de sistema foram carregados
        /// </summary>
        /// <returns></returns>
        protected override bool CarregarParametrosSistema()
        {
            return false;
        }
    }
}
