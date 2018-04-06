using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.RAV.Core.Web.Controles.Portal
{
	/// <summary>
	/// Assistente Passo-a-Passo
	/// </summary>
	[ParseChildren(true, "Passos"),
    DefaultProperty("Passos"),
    ToolboxData(@"  <{0}:AssistentePassos ID="""" runat=""server"">
                        <Passo Descricao="""" />
                    </{0}:AssistentePassos>")]
    public class AssistentePassos : WebControl, INamingContainer, IPostBackEventHandler
    {
        #region [ Propriedades ]

        /// <summary>Lista de passos que compõem o Assistente</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public ArrayList Passos
        {
            get
            {
                if (ViewState["Passos"] == null)
                    ViewState["Passos"] = new ArrayList();
                return (ArrayList)ViewState["Passos"];
            }
            set
            {
                ViewState["Passos"] = value;
            }
        }

        /// <summary>Passo ativo</summary>
        public Int32 PassoAtivo
        {
            get { return ViewState["PassoAtivo"].ToInt32(0); }
            set { ViewState["PassoAtivo"] = value; }
        }

        /// <summary>Passo máximo</summary>
        public Int32 PassoMaximo
        {
            get { return ViewState["PassoMaximo"].ToInt32(0); }
            set { ViewState["PassoMaximo"] = value; }
        }

        /// <summary>
        /// Define a exibição em modo Jade
        /// </summary>
        public Boolean ModoJade
        {
            get
            {
                return ((bool?)ViewState["ModoJade"]).GetValueOrDefault(true);
            }
            set
            {
                ViewState["ModoJade"] = value;
            }
        }

        #endregion

        #region [ Métodos Sobrescritos / Implementações Interfaces ]

        /// <summary>
        /// Renderização do controle
        /// </summary>
        /// <param name="writer">Writer</param>
        protected override void Render(HtmlTextWriter writer)
        {
            List<string> layoutClasses = new List<string>() { "steps-rede" };
            if (this.ModoJade)
                layoutClasses.Add("jade");

            //Renderiza div wrapper do controle
            writer.AddAttribute(HtmlTextWriterAttribute.Class, string.Join(" ", layoutClasses.ToArray()));

            //this.RenderBeginTag(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            //Renderiza os passos dos itens
            foreach (Passo passo in this.Passos)
                this.RenderPasso(writer, passo);

            // Fechando os controles
            this.RenderEndTag(writer);
        }

        /// <summary>
        /// Renderização do HTML de um Passo do Assistente
        /// </summary>
        /// <param name="writer">Writer do controle</param>
        /// <param name="passo">Passo a ser renderizado</param>
        private void RenderPasso(HtmlTextWriter writer, Passo passo)
        {
            //Índice do passo no array de Passos
            Int32 indice = this.Passos.IndexOf(passo);

            //Renderiza div wrapper de todos os elementos do Passo
            //StringBuilder classes = new StringBuilder(String.Format("step passo{0} ", indice + 1));
            StringBuilder classes = new StringBuilder("step ");

            //Atribui classe de acordo com o passo ativo
            if (indice < this.PassoAtivo)
            {
                classes.Append("active ");

                //Se o passo já foi superado e possui evento de clique, renderiza como ativo/habilitado
                if (indice <= this.PassoMaximo && passo.EventoCliqueDefinido)
                    classes.Append("ativado ");
            }
            else if (indice == this.PassoAtivo)
                classes.Append("active current ");
            else
            {
                //classes.Append("proximo ");
                if (indice <= this.PassoMaximo && passo.EventoCliqueDefinido)
                    classes.Append("ativado ");
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Class, classes.ToString());
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "step-bar");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag();
            //<div class="step-bar"></div>

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "step-number");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            //Se não for o passo atual, e se evento foi definido, pode efetuar bind do evento
            if (indice != this.PassoAtivo && passo.EventoCliqueDefinido)
            {
                //Só permite navegar para os passos já superados (passos anteriores ao máximo)
                if (indice <= this.PassoMaximo)
                {
                    StringBuilder jsClick = new StringBuilder();
                    if (passo.ExibirBlockUI)
                        jsClick.Append("blockUI();");
                    jsClick.Append(this.Page.ClientScript.GetPostBackEventReference(this, indice.ToString()));
                    writer.AddAttribute("onclick", jsClick.ToString());
                }
            }

            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(indice + 1);
            writer.RenderEndTag();
            writer.RenderEndTag();
            //<div class="step-number">
            //    <span>1</span>
            //</div>

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "step-title");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write(passo.Descricao);
            writer.RenderEndTag();
            //<div class="step-title">dados de cancelamento</div>

            writer.RenderEndTag();
        }

        /// <summary>
        /// Tratamento dos eventos PostBack do controle
        /// </summary>
        /// <param name="eventArgument">Argumento contendo o índice passo responsável pelo postback</param>
        public void RaisePostBackEvent(String eventArgument)
        {
            Int32? index = eventArgument.ToInt32Null();
            if (index.HasValue)
            {
                //Obtém o passo responsável
                Passo passo = (Passo)this.Passos[index.Value];

                //Se definido, invoca o evento de clique
                if (passo.EventoCliqueDefinido)
                {
                    passo.EfetuarClique(this);

                    //Atualiza o passo ativo
                    this.PassoAtivo = index.Value;
                }
            }
        }

        #endregion

        #region [ Métodos ]

        /// <summary>
        /// Define o passo ativo (zero-based index)
        /// </summary>
        /// <param name="passo">Ordem/Índice do Passo</param>
        public void AtivarPasso(Int32 passo)
        {
            if (passo >= 0 && passo < this.Passos.Count)
            {
                this.PassoAtivo = passo;
                if (passo > this.PassoMaximo)
                    this.PassoMaximo = passo;
            }
        }

        /// <summary>
        /// Avança para o próximo Passo
        /// </summary>
        /// <returns>Novo passo ativo</returns>
        public Int32 Avancar()
        {
            this.AtivarPasso(this.PassoAtivo + 1);
            this.PassoMaximo = this.PassoAtivo;
            return this.PassoAtivo;
        }

        /// <summary>
        /// Retorna ao Passo anterior
        /// </summary>
        /// <returns>Novo passo ativo</returns>
        public Int32 Voltar()
        {
            this.AtivarPasso(this.PassoAtivo - 1);
            return this.PassoAtivo;
        }

        #endregion
    }
}
