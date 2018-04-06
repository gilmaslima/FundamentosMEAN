using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Aberto.Model;
using Redecard.Portal.Aberto.Model.Repository;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Redecard.Portal.Aberto.WebParts.ControlTemplates;
using Redecard.Portal.Helper;
using Redecard.Portal.Helper.Paginacao;
using System.Reflection;
using System.Collections.Specialized;
using System.Collections;
using System.Web.UI.WebControls;


namespace Redecard.Portal.Aberto.WebParts.RedecardNoticias {
    public partial class RedecardNoticiasUserControl : UserControl {

        /// <summary>
        /// 
        /// </summary>
        public DateTime _currentMonth = DateTime.Now;

        /// <summary>
        /// Nome da âncora utilizada para posicionamento do usuário na página
        /// </summary>
        public string AncoraListagem {
            get {
                return "wptListagemNoticias";
            }
        }

        /// <summary>
        /// Referência tipada ao user control de paginação
        /// </summary>
        private UserControlPaginador Paginador {
            get {
                return this.ucPaginadorHibrido as UserControlPaginador;
            }
        }

        #region Propriedades__________________

        private RedecardNoticias WebPart {
            get {
                return (RedecardNoticias)this.Parent;
            }
        }

        #endregion

        #region Constantes____________________

        #endregion

        #region Métodos_______________________

        private void ListarResumo() {
            IList<DTONoticia> itens = new List<DTONoticia>();
            this.rptResumo.Visible = true;
            using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTONoticia, NotíciasItem>>()) {
                AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                delegate {
                    itens = repository.GetItems(item => item.DataDeExpiração.Value.CompareTo(DateTime.Now) > 0).OrderByDescending(item => item.Data).Take(this.WebPart.quantidadeItens).ToList();
                });
                if (itens.Count > 0) {
                    this.rptResumo.DataSource = itens;
                    this.rptResumo.DataBind();
                }
                else {
                    ltEmptyResumo.Visible = true;
                }
            }
        }

        #endregion

        #region Métodos_______________________

        private void ListarTodos() {
            this.rptResumo.Visible = true;
            using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTONoticia, NotíciasItem>>()) {
                ListaPaginada<DTONoticia> itens = null;
                AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                delegate {
                    itens = new ListaPaginada<DTONoticia>(repository.GetItems(item => item.Data.Value.Month == _currentMonth.Month && item.DataDeExpiração.Value.CompareTo(DateTime.Now) > 0).OrderByDescending(item => item.Data), this.Paginador.Pagina, this.WebPart.quantidadeItens);
                });
                if (itens.Count > 0) {
                    this.rptTodos.DataSource = itens;
                    this.rptTodos.DataBind();

                    this.Paginador.MontarPaginador(itens.TotalItens, this.WebPart.quantidadeItens, this.AncoraListagem);
                }
                else {
                    ltEmptyTodos.Visible = true;
                }
            }
        }

        #endregion

        #region Métodos_______________________

        private void ListarDetalhes()
        {
            this.rptDetalhe.Visible = true;

            using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTONoticia, NotíciasItem>>())
            {
                List<DTONoticia> itens = null;
                AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                delegate
                {
                    itens = new List<DTONoticia>(repository.GetItems(item => item.Data.Value.Month == _currentMonth.Month && item.DataDeExpiração.Value.CompareTo(DateTime.Now) > 0).Take(this.WebPart.quantidadeItens).OrderByDescending(item => item.Data));
                });
                if (itens.Count > 0)
                {
                    this.rptDetalhe.DataSource = itens;
                    this.rptDetalhe.DataBind();                    
                }
                else
                {
                    ltEmptyDetalhe.Visible = true;
                }
            }
        }

        #endregion

        #region Eventos_______________________

        protected void Page_Load(object sender, EventArgs e) {
            switch (this.WebPart.tipoVisao) {
                case RedecardNoticias.Visao.Detalhe:                    
                    this.ListarDetalhes();
                    break;
                case RedecardNoticias.Visao.Todos:
                    this.WebPart.quantidadeItens = 8;
                    this.SetMonth();
                    this.ListarTodos();
                    this.LoadOptions();
                    break;
                default:
                    this.ListarResumo();
                    break;
            }

            // modificar modo de exibição
            this.HandlerShow();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected void LoadOptions() {
            Dictionary<string, DateTime> meses = this.ObterMeses();
            foreach (KeyValuePair<string, DateTime> mes in meses) {
                ListItem li = new ListItem();
                li.Text = mes.Key;
                li.Value = mes.Value.ToString();
                slcMonths.Items.Add(li);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, DateTime> ObterMeses() {
            IList<DTONoticia> galerias = null;
            try {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTONoticia, NotíciasItem>>()) {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate {
                        galerias = repository.GetItems(item => item.Data.Value.ToString("MMyyyy") != _currentMonth.ToString("MMyyyy")).OrderByDescending(g => g.Data.Value).Distinct().ToList();
                    });
                }
            }
            catch (Exception) {
                //TODO:Redirecionar para uma página padrão de erro
            }
            Dictionary<string, DateTime> mesesDisp = new Dictionary<string, DateTime>();
            if (!object.ReferenceEquals(galerias, null)) {
                foreach (DTONoticia noticia in galerias) {
                    string sFormat = String.Format("{0} / {1}", this.GetMonthName(noticia.Data.Value.Month).ToUpper(), noticia.Data.Value.Year);
                    if (!mesesDisp.Keys.Contains(sFormat)) {
                        mesesDisp.Add(sFormat, noticia.Data.Value);
                    }
                }
                return mesesDisp;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SetMonth() {
            if (!String.IsNullOrEmpty(Request["period"] as string)) {
                _currentMonth = DateTime.Parse(Request["period"] as string);
            }
            // setar mês atual de pesquisa
            string sAno = _currentMonth.Year.ToString();
            string sMes = this.GetMonthName(_currentMonth.Month);
            ltMonth.Text = String.Format("{0} / {1}", sMes, sAno);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonthNumber"></param>
        protected string GetMonthName(int MonthNumber) {
            return System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.GetMonthName(MonthNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void HandlerShow() {
            if (this.WebPart.tipoVisao == RedecardNoticias.Visao.Resumo) {
                pnlResumo.Visible = true;
                pnlTodos.Visible = false;
                pnlDetalhe.Visible = false;
            }
            else if (this.WebPart.tipoVisao == RedecardNoticias.Visao.Todos) {
                pnlResumo.Visible = false;
                pnlTodos.Visible = true;
                pnlDetalhe.Visible = false;
            }
            else if (this.WebPart.tipoVisao == RedecardNoticias.Visao.Detalhe)            {
                pnlResumo.Visible = false;
                pnlTodos.Visible = false;
                pnlDetalhe.Visible = true;
            }
        }
    }
}
