/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Rede.PN.HomePage.SharePoint
{
    /// <summary>
    /// Customização da ToolPart da WebPart para configuração de atalhos da HomePage
    /// </summary>
    public class AtalhosHomeToolPart : Microsoft.SharePoint.WebPartPages.ToolPart
    {
        #region [ Propriedades Privadas ]

        /// <summary>
        /// panel control.
        /// </summary>
        private Panel panel;

        /// <summary>
        /// txtTexto control.
        /// </summary>
        private TextBox[] txtTexto;

        /// <summary>
        /// txtTexto control.
        /// </summary>
        private TextBox[] txtCssClass;

        /// <summary>
        /// txtUrl control.
        /// </summary>
        private TextBox[] txtUrl;

        /// <summary>
        /// Quantidade de itens
        /// </summary>
        private Int32 quantidadeAtalhos;

        /// <summary>
        /// Action para salvar as alterações dos atalhos.
        /// </summary>
        private Action<String> actionApplyChanges;

        /// <summary>
        /// Function para carregar a configuração dos atalhos
        /// </summary>
        private Func<String> actionLoadData;

        #endregion

        #region [ Contrutores ]

        /// <summary>
        /// Construtor padrão
        /// </summary>
        /// <param name="titulo">Título do Toolpart</param>
        /// <param name="quantidadeAtalhos">Quantidade de atalhos</param>
        /// <param name="actionApplyChanges">Action para salvar as configurações</param>
        /// <param name="actionLoadData">Function para carregar as configurações</param>
        public AtalhosHomeToolPart(
            String titulo,
            Int32 quantidadeAtalhos,
            Action<String> actionApplyChanges,
            Func<String> actionLoadData)
        {
            //Inicialização de variáveis e controles
            this.ChromeState = PartChromeState.Minimized;
            this.Title = titulo;
            this.ToolTip = titulo;
            this.panel = new Panel();
            this.txtUrl = new TextBox[quantidadeAtalhos];
            this.txtTexto = new TextBox[quantidadeAtalhos];
            this.txtCssClass = new TextBox[quantidadeAtalhos];

            for (Int32 i = 0; i < quantidadeAtalhos; i++)
            {
                this.txtUrl[i] = new TextBox();
                this.txtTexto[i] = new TextBox();
                this.txtCssClass[i] = new TextBox();
                this.txtUrl[i].Attributes["class"] = "UserInput";
                this.txtTexto[i].Attributes["class"] = "UserInput";
                this.txtCssClass[i].Attributes["class"] = "UserInput";
            }

            this.actionApplyChanges = actionApplyChanges;
            this.actionLoadData = actionLoadData;
            this.quantidadeAtalhos = quantidadeAtalhos;            
        }

        #endregion

        #region [ Métodos sobrescritos ]

        /// <summary>
        /// CreateChildControls
        /// </summary>
        protected override void CreateChildControls()
        {
            //Recupera os atalhos que serão incluídos no box
            List<Pagina> atalhos = RecuperarAtalhos(actionLoadData());

            //Monta HTML contendo os links dos atalhos, seguindo mesmo estilo das
            //toolparts padrões do SharePoint
            Controls.Add(panel);
            {
                for (Int32 i = 0; i < quantidadeAtalhos; i++)
                {
                    var divHead = new HtmlGenericControl("div");
                    divHead.Attributes["class"] = "UserSectionHead";
                    panel.Controls.Add(divHead);
                    {
                        var lblTituloGrupo = new Label { Text = String.Format("Configuração Atalho {0}", i + 1) };
                        divHead.Controls.Add(lblTituloGrupo);
                    }

                    var divBody = new HtmlGenericControl("div");
                    divBody.Attributes["class"] = "UserSectionBody";
                    panel.Controls.Add(divBody);
                    {
                        var divGroupTexto = new HtmlGenericControl("div");
                        divGroupTexto.Attributes["class"] = "UserControlGroup";
                        divBody.Controls.Add(divGroupTexto);
                        {
                            var noBr = new HtmlGenericControl("nobr");
                            divGroupTexto.Controls.Add(noBr);
                            {
                                noBr.Controls.Add(new Label { Text = "Texto: " });
                                if (atalhos.ElementAtOrDefault(i) != null)
                                    this.txtTexto[i].Text = atalhos[i].TextoBotao;
                                noBr.Controls.Add(this.txtTexto[i]);
                            }
                        }

                        var divGroupUrl = new HtmlGenericControl("div");
                        divGroupUrl.Attributes["class"] = "UserControlGroup";
                        divBody.Controls.Add(divGroupUrl);
                        {
                            var noBr = new HtmlGenericControl("nobr");
                            divGroupUrl.Controls.Add(noBr);
                            {
                                noBr.Controls.Add(new Label { Text = "Url: " });

                                if (atalhos.ElementAtOrDefault(i) != null)
                                    this.txtUrl[i].Text = atalhos[i].Url;
                                noBr.Controls.Add(this.txtUrl[i]);
                            }
                        }

                        var divGroupCss = new HtmlGenericControl("div");
                        divGroupCss.Attributes["class"] = "UserControlGroup";
                        divBody.Controls.Add(divGroupCss);
                        {
                            var noBr = new HtmlGenericControl("nobr");
                            divGroupCss.Controls.Add(noBr);
                            {
                                noBr.Controls.Add(new Label { Text = "CSS Class: " });

                                if (atalhos.ElementAtOrDefault(i) != null)
                                    this.txtCssClass[i].Text = atalhos[i].CssClass;
                                noBr.Controls.Add(this.txtCssClass[i]);
                            }
                        }
                    }

                    if (i < this.quantidadeAtalhos - 1)
                    {
                        var divSeparador = new HtmlGenericControl("div");
                        divSeparador.Attributes["class"] = "UserDottedLine";
                        panel.Controls.Add(divSeparador);
                    }
                }
            }

            base.CreateChildControls();
        }

        /// <summary>
        /// Applychanges
        /// </summary>
        public override void ApplyChanges()
        {
            var atalhos = new List<Pagina>();

            //Recupera as configurações dos atalhos para salvar nas propriedades da webpart
            for (Int32 i = 0; i < quantidadeAtalhos; i++)
            {
                var atalho = new Pagina 
                {
                    TextoBotao = this.txtTexto[i].Text,
                    Url = this.txtUrl[i].Text,
                    CssClass = this.txtCssClass[i].Text
                };
                atalhos.Add(atalho);
            }

            actionApplyChanges(MontarStringConfiguracaoAtalhos(atalhos));
        }

        #endregion

        #region [ Métodos Privados ]

        /// <summary>
        /// Método auxiliar para converter uma lista de atalhos em uma String de Configuração.
        /// Realiza o processo inverso do método "RecuperarAtalhos".
        /// Padrão formatação: [Descrição do Link];[URL do Link]\n[Descrição do Link];[URL do Link]...
        /// </summary>
        /// <param name="paginas">Lista de atalhos</param>
        /// <returns>String de configuração dos atalhos</returns>
        private String MontarStringConfiguracaoAtalhos(List<Pagina> paginas)
        {
            return String.Join("\n", paginas.Select(pagina =>
            {
                String texto = (pagina.TextoBotao ?? String.Empty).Trim();
                String url = (pagina.Url ?? String.Empty).Trim();
                String css = (pagina.CssClass ?? String.Empty).Trim();
                return String.Concat(texto, ";", url, ";", css);
            }).ToArray());
        }

        /// <summary>
        /// Método auxiliar para converter uma String de Configuração de Atalhos 
        /// em Modelo de Página. Realiza o processo inverso do método "MontarStringConfiguracaoAtalhos".
        /// Padrão formatação: [Descrição do Link];[URL do Link]\n[Descrição do Link];[URL do Link]...
        /// </summary>
        /// <param name="configuracaoAtalhos">String de configuração dos atalhos</param>
        /// <returns>Lista dos atalhos extraídos da string de configuração</returns>
        private List<Pagina> RecuperarAtalhos(String configuracaoAtalhos)
        {
            //Lista de retorno
            var atalhos = new List<Pagina>();

            if (!String.IsNullOrEmpty(configuracaoAtalhos))
            {
                var reader = new StringReader(configuracaoAtalhos);
                var configuracaoAtalho = default(String);
                //Processa cada linha da string de configuração
                while ((configuracaoAtalho = reader.ReadLine()) != null)
                {
                    //Separa a descrição e a url
                    String[] tokens = configuracaoAtalho.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length > 1)
                    {
                        var atalho = new Pagina();
                        atalho.TextoBotao = (tokens[0] ?? String.Empty).Trim();
                        atalho.Url = (tokens[1] ?? String.Empty).Trim();
                        if (tokens.Length > 2) // possui css configurado
                            atalho.CssClass = (tokens[2] ?? String.Empty).Trim();
                        atalhos.Add(atalho);
                    }
                }
            }

            return atalhos;
        }

        #endregion
    }
}
