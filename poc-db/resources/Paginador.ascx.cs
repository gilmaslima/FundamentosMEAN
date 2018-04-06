using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.ComponentModel;

namespace Redecard.PN.OutrosServicos.SharePoint.ControlTemplates.OutrosServicos.Corban
{
    public partial class Paginador : UserControlBase
    {
        /// <summary>
        /// Sobrescreve o carregamento do controle 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            this.ValidarPermissao = false;
            base.OnLoad(e);
        }

        #region Atributos e Propriedades
        
        /// <summary>
        /// 
        /// </summary>
        public Int32 RegistrosPorPagina
        {
            get
            {
                if (ViewState["RegistrosPorPagina"] == null)
                    ViewState["RegistrosPorPagina"] = 0;
                return (Int32)ViewState["RegistrosPorPagina"];
            }
            set { ViewState["RegistrosPorPagina"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 QuantidadeTotalRegistros
        {
            get
            {
                if (ViewState["QuantidadeTotalRegistros"] == null)
                    ViewState["QuantidadeTotalRegistros"] = 0;
                return (Int32)ViewState["QuantidadeTotalRegistros"];
            }
            set { ViewState["QuantidadeTotalRegistros"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 PaginaAtual
        {
            get
            {
                if (ViewState["PaginaAtual"] == null)
                    ViewState["PaginaAtual"] = 0;
                return (Int32)ViewState["PaginaAtual"];
            }
            set { ViewState["PaginaAtual"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 TotalPagina { get { return ObterTotalDePaginas(); } }

        /// <summary>
        /// 
        /// </summary>
        public Boolean ExibirBlockUI { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Int32 ObterTotalDePaginas()
        {
            if (RegistrosPorPagina == 0 || QuantidadeTotalRegistros == 0)
                return 0;
            Int32 iTotalPagina = (Int32)Math.Ceiling((double)QuantidadeTotalRegistros / RegistrosPorPagina);
            return iTotalPagina;

        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 QuantidadeTotalRegistrosRetornados
        {
            get { return (Int32)(ViewState["QuantidadeTotalRegistrosRetornados"] ?? 0); }
            set { ViewState["QuantidadeTotalRegistrosRetornados"] = value; }
        }

        #endregion Atributos e Propriedades

        #region Eventos

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuantidadeTotalRegistros = 0;
            PaginaAtual = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            MontaPaginacao(Convert.ToInt32(QuantidadeTotalRegistros));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);


            if (QuantidadeTotalRegistros != 0)
            {
                MontaPaginacao(QuantidadeTotalRegistros);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkAnterior_Click(object sender, EventArgs e)
        {
            int iPaginaAnterior = PaginaAtual - 1;

            //verifica se jã não é a primeira pagina
            if (iPaginaAnterior < 1)
            {
                iPaginaAnterior = 1;
            }

            PaginaAtual = iPaginaAnterior;
            onPaginacaoChanged(iPaginaAnterior, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkProxima_Click(object sender, EventArgs e)
        {
            int iPaginaProxima = PaginaAtual + 1;

            //verifica se não é a ultima pagina
            if (iPaginaProxima > this.TotalPagina)
            {
                iPaginaProxima = this.TotalPagina;
            }

            PaginaAtual = iPaginaProxima;
            onPaginacaoChanged(iPaginaProxima, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkAnteriorUltima_Click(object sender, EventArgs e)
        {
            int iPaginaAnterior = 1;
            PaginaAtual = iPaginaAnterior;
            onPaginacaoChanged(iPaginaAnterior, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkProximaUltima_Click(object sender, EventArgs e)
        {
            int iPaginaProxima = TotalPagina;
            PaginaAtual = iPaginaProxima;

            if (CacheTodosRegistros != null)
            {
                CacheTodosRegistros();
                PaginaAtual = TotalPagina;
            }

            onPaginacaoChanged(PaginaAtual, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SelectedPage_Selected(object sender, EventArgs e)
        {
            int iPagina = Convert.ToInt32(((LinkButton)sender).Text.Trim());
            PaginaAtual = iPagina;
            onPaginacaoChanged(iPagina, e);
        }

        #endregion Eventos

        #region Metodos

        /// <summary>
        /// 
        /// </summary>
        public Int32 PaginasVirtuais
        {
            get
            {
                Int32 maxPaginasTela = 15;
                Int32 inicio = 1;
                Int32 fim;
                Int32 paginaAtual = PaginaAtual;
                Int32 maxPaginasDiv2 = maxPaginasTela / 2;
                inicio = paginaAtual - maxPaginasDiv2;
                if (inicio < 1)
                    inicio = 1;
                fim = inicio + maxPaginasTela;
                return fim;
            }
        }

        /// <summary>
        /// Monta o numero de paginas necessarias para a navegação
        /// </summary>
        /// <param name="TotalRegistros">Numero total de registros</param>
        private void MontaPaginacao(int TotalRegistros)
        {
            phRollOverPagination.Controls.Clear();

            LinkButton objLinkButton;

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
                objLinkButton = new LinkButton();
                objLinkButton.Text = " " + (i).ToString() + " ";
                objLinkButton.Click += new EventHandler(SelectedPage_Selected);
                if (ExibirBlockUI)
                    objLinkButton.OnClientClick = "blockUI();";
                objLinkButton.ID = "lnkPaginacao" + (i).ToString();
                objLinkButton.Style.Add("padding", "0 2px");
                objLinkButton.EnableViewState = true;

                if (iPaginaAtual == i)
                {
                    objLinkButton.Enabled = false;
                    objLinkButton.CssClass = "linkSelecionado";
                    objLinkButton.OnClientClick = String.Empty;
                }

                phRollOverPagination.Controls.Add(objLinkButton);
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
                divPaginas.InnerText = String.Format("Página {0:N0}", iPaginaAtual);
                divPaginas.Visible = true;
            }
            else
            {
                divPaginas.Visible = false;
            }

            lnkAnterior.Enabled = iPaginaAtual != 1;
            lnkAnteriorUltima.Enabled = iInicio != 1;
            lnkProxima.Enabled = iPaginaAtual != iFim;
            lnkProximaUltima.Enabled = iFim != iTotalPagina;

            if (ExibirBlockUI)
            {
                if (lnkAnterior.Enabled)
                    lnkAnterior.OnClientClick = "blockUI();";
                if (lnkAnteriorUltima.Enabled)
                    lnkAnteriorUltima.OnClientClick = "blockUI();";
                if (lnkProxima.Enabled)
                    lnkProxima.OnClientClick = "blockUI();";
                if (lnkProximaUltima.Enabled)
                    lnkProximaUltima.OnClientClick = "blockUI();";
            }
        }
        #endregion Metodos

        #region Delegates e Eventos

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagina"></param>
        /// <param name="e"></param>
        public delegate void PaginacaoChanged(int pagina, EventArgs e);
        
        /// <summary>
        /// 
        /// </summary>
        public delegate void CacheTodosRegistrosEventHandler();
        
        /// <summary>
        /// 
        /// </summary>
        [Browsable(true)]
        public event PaginacaoChanged onPaginacaoChanged;
        
        /// <summary>
        /// 
        /// </summary>
        [Browsable(true)]
        public event CacheTodosRegistrosEventHandler CacheTodosRegistros;
        
        #endregion Delegates e Eventos
    }
}
