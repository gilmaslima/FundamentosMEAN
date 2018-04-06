using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Redecard.Portal.Fechado.Model.Repository.DTOs;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Fechado.Model.Repository;
using Redecard.Portal.Helper;
using Redecard.Portal.Fechado.Model;
using Microsoft.SharePoint;


namespace Redecard.Portal.Fechado.WebParts.RedecardEnquetes {
    public partial class RedecardEnquetesUserControl : UserControl {

        #region Variáveis_____________________

        private List<DTOEnqueteResultado> listResultados = null;

        private object _usuarioHasVoted = null;

        #endregion

        #region Propriedades__________________

        private RedecardEnquetes WebPart {
            get {
                return (RedecardEnquetes)this.Parent;
            }
        }

        private int Contador = 0;

        #endregion

        #region Constantes____________________

        #endregion

        #region Métodos_______________________

        protected void CarregarEnquete() {
            DTOEnquetePergunta pergunta = null;
            IList<DTOEnqueteResposta> resposta = null;

            try {
                using (var repositoryPergunta = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOEnquetePergunta, EnquetePerguntasItem>>())
                using (var repositoryResposta = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOEnqueteResposta, EnqueteRespostasItem>>()) {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate {
                        pergunta = repositoryPergunta.GetItems(i => i.Ativo == true && i.DataDeInício <= DateTime.Now && i.DataDeFim >= DateTime.Now).FirstOrDefault();

                        if (pergunta != null) {
                            resposta = repositoryResposta.GetItems(i => i.Pergunta.Id == pergunta.ID && i.Ativo == true).ToList();

                            this.pnlEnquete.Visible = true;
                            this.pnlResultado.Visible = false;
                            this.ltlPergunta.Text = pergunta.Pergunta;
                            this.rptEnquete.DataSource = resposta;
                            this.rptEnquete.DataBind();
                        }
                        else {
                            this.msgErro.Text = RedecardHelper.ObterResourceFechado("_RedecardEnquetes_NenhumItem");
                            this.msgErro.Visible = true;
                        }
                    });
                }
            }
            catch (Exception) {
                //TODO:Redirecionar para uma página padrão de erro
            }
        }

        protected void CarregarResultado() {
            DTOEnquetePergunta pergunta = null;
            IList<DTOEnqueteResposta> resposta = null;

            try {
                this.Contador = 0;
                using (var repositoryPergunta = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOEnquetePergunta, EnquetePerguntasItem>>())
                using (var repositoryResposta = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOEnqueteResposta, EnqueteRespostasItem>>()) {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate {
                        pergunta = repositoryPergunta.GetItems(i => i.Ativo == true && i.DataDeInício <= DateTime.Now && i.DataDeFim >= DateTime.Now).FirstOrDefault();

                        if (pergunta != null) {
                            resposta = repositoryResposta.GetItems(i => i.Pergunta.Id == pergunta.ID && i.Ativo == true).ToList();

                            this.pnlResultado.Visible = true;
                            this.pnlEnquete.Visible = false;
                            this.ltlPergunta.Text = pergunta.Pergunta;
                            this.ltlContador.Text = this.Contador.ToString();

                            this.ltlContadorS.Visible = this.Contador <= 1;
                            this.ltlContadorM.Visible = !this.ltlContadorS.Visible;

                            this.rptEnqueteResultado.DataSource = resposta;
                            this.rptEnqueteResultado.DataBind();
                        }
                        else {
                            this.pnlResultado.Visible = false;
                            this.msgErro.Text = RedecardHelper.ObterResourceFechado("_RedecardEnquetes_NenhumItem");
                            this.msgErro.Visible = true;
                        }
                    });
                }
            }
            catch (Exception) {
                //TODO:Redirecionar para uma página padrão de erro
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool UsuarioJaVotou() {
            if (object.ReferenceEquals(_usuarioHasVoted, null)) {
                DTOEnquetePergunta pergunta = null;
                bool _usuarioJaVotou = false;
                using (var repositoryResultado = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOEnqueteResultado, EnqueteResultadosItem>>())
                using (var repositoryPergunta = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOEnquetePergunta, EnquetePerguntasItem>>()) {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                        delegate {
                            pergunta = repositoryPergunta.GetItems(i => i.Ativo == true && i.DataDeInício <= DateTime.Now && i.DataDeFim >= DateTime.Now).FirstOrDefault();
                            _usuarioJaVotou = repositoryResultado.GetItems(p => p.Pergunta.Id == pergunta.ID && p.UsuárioId == SPContext.Current.Web.CurrentUser.ID).Count > 0 ? true : false;
                        });
                }
                _usuarioHasVoted = _usuarioJaVotou;
                return _usuarioJaVotou;
            }
            return (bool)_usuarioHasVoted;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void VotarEnquete() {
            DTOEnqueteResposta resposta = null;
            DTOEnquetePergunta pergunta = null;

            if (this.Page.Request.Form["optEnquete"] != "") {
                using (var repositoryResultado = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOEnqueteResultado, EnqueteResultadosItem>>())
                using (var repositoryResposta = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOEnqueteResposta, EnqueteRespostasItem>>())
                using (var repositoryPergunta = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOEnquetePergunta, EnquetePerguntasItem>>()) {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate {
                        resposta = repositoryResposta.GetItems(i => i.Id == int.Parse(this.Page.Request.Form["optEnquete"])).First();
                        pergunta = repositoryPergunta.GetItems(i => i.Ativo == true && i.DataDeInício <= DateTime.Now && i.DataDeFim >= DateTime.Now).FirstOrDefault();

                        //Verifica se o usuário já votou nesta enquete
                        bool UsuarioJaVotou = this.UsuarioJaVotou();

                        //DTOEnqueteResultado resultado = new DTOEnqueteResultado();
                        //resultado.Pergunta = pergunta;
                        //resultado.Resposta = resposta;

                        //resultado.Titulo = resultado.Pergunta.Titulo;
                        //resultado.Usuario = SPContext.Current.Web.CurrentUser.Name ;
                        //resultado.UsuarioId = SPContext.Current.Web.CurrentUser.ID;

                        //repositoryResultado.Persist(resultado);

                        //Se o usuário não votou computa o voto
                        if (!UsuarioJaVotou) {
                            using (SPWeb oWeb = SPContext.Current.Web.Impersonate()) {
                                SPList oResultados = oWeb.Lists["Enquete - Resultados"];
                                SPListItem oItem = oResultados.AddItem();
                                oItem["Title"] = pergunta.Titulo;
                                oItem[oItem.Fields.GetFieldByInternalName("Usu_x00e1_rio").Id] = SPContext.Current.Web.CurrentUser.ID;
                                oItem["Pergunta"] = (int)pergunta.ID;
                                oItem["Resposta"] = (int)resposta.ID;
                                oItem.Update();
                            }

                            this.CarregarResultado();
                        }
                        else {
                            this.pnlResultado.Visible = false;
                            this.msgErro.Text = RedecardHelper.ObterResourceFechado("_RedecardEnquetes_UsuarioJaVotou");
                            this.msgErro.Visible = true;
                            this.pnlEnquete.Visible = true;
                        }
                    });
                }
            }
        }

        #endregion

        #region Eventos_______________________

        protected void Page_Load(object sender, EventArgs e) {
            this.pnlEnquete.Visible = true;
            this.pnlResultado.Visible = false;

            if (!this.Page.IsPostBack) {
                if (UsuarioJaVotou())
                    this.CarregarResultado();
                else
                    this.CarregarEnquete();
            }
        }

        protected void rptEnqueteResultado_ItemDataBound(object sender, RepeaterItemEventArgs e) {
            int quantidade = 0;
            double percentual = 0;

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {
                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOEnqueteResultado, EnqueteResultadosItem>>()) {
                    AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                    delegate {
                        if (listResultados == null)
                            listResultados = repository.GetItems(p => p.Pergunta.Id == ((DTOEnqueteResposta)e.Item.DataItem).RespostaEntity.Pergunta.Id);

                        quantidade = listResultados.Where(p => p.Resposta.ID == ((DTOEnqueteResposta)e.Item.DataItem).ID).Count();

                        this.Contador += quantidade;
                        if (listResultados.Count > 0 && quantidade > 0) {
                            percentual = (((double)quantidade / (double)listResultados.Count) * 100.0);
                            ((Literal)e.Item.FindControl("ltlVoto")).Text = string.Format("{0:00}", percentual);
                        }
                        else {
                            ((Literal)e.Item.FindControl("ltlVoto")).Text = "0";
                        }
                    });
                }

                ltlContador.Text = this.Contador.ToString();
                this.ltlContadorS.Visible = this.Contador <= 1;
                this.ltlContadorM.Visible = !this.ltlContadorS.Visible;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void hlkResponder_Click(object sender, EventArgs e) {
            this.VotarEnquete();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void hlkResultado_Click(object sender, EventArgs e) {
            this.CarregarResultado();
        }

    }
}
