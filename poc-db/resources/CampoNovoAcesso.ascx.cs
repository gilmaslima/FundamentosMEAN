/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;

namespace Redecard.PN.OutrasEntidades
{
    [ParseChildren(true)]
    [PersistChildren(false)]
    public partial class CampoNovoAcesso : UserControl, IPostBackEventHandler
    {        
        #region [ Propriedades Públicas ]

        /// <summary>
        /// Controle TextBox interno.
        /// </summary>
        public TextBox TextBox
        {
            get { return this.txtCampoNovoAcesso; }
        }

        /// <summary>
        /// Tipo do Controle
        /// </summary>
        public TipoCampoNovoAcesso? Tipo { get; set; }

        /// <summary>
        /// Validation Group
        /// </summary>
        public String ValidationGroup 
        { 
            get
            {
                return this.TextBox.ValidationGroup;
            }
            set
            {
                this.TextBox.ValidationGroup = value;
                valCustomizado.ValidationGroup = value;
            }
        }
        
        /// <summary>
        /// Habilita/Desabilita a validação de Campo Obrigatório
        /// </summary>
        public Boolean Obrigatorio { get; set; }

        private Control legenda;
        /// <summary>
        /// Conteúdo HTML descritivo abaixo do campo de texto.
        /// </summary>
        public Control Legenda 
        {
            get { return legenda; }
            set { legenda = value; }
        }
        
        /// <summary>
        /// Class CSS aplicada diretamente ao TextBox.
        /// </summary>
        public String CssClass
        {
            get { return this.TextBox.CssClass; }
            set { this.TextBox.CssClass = value; }
        }

        /// <summary>
        /// Quando utilizado o tipo "ConfirmacaoEmail", é o Server ID do controle a ser comparado
        /// </summary>
        public String ControlToCompare { get; set; }

        /// <summary>
        /// Controle associado (ControlToCompare)
        /// </summary>
        public Control ControleAssociado
        {
            get
            {
                if (!String.IsNullOrEmpty(this.ControlToCompare))
                    return FindChildControl<Control>(this.Page, this.ControlToCompare);
                else
                    return null;
            }
        }

        /// <summary>
        /// Conteúdo do TextBox
        /// </summary>
        public String Text
        {
            get { return this.TextBox.Text; }
            set { this.TextBox.Text = value; }
        }

        /// <summary>
        /// Modo do TextBox
        /// </summary>
        public TextBoxMode TextMode
        {
            get { return this.TextBox.TextMode; }
            set { this.TextBox.TextMode = value; }
        }

        /// <summary>
        /// MaxLength do TextBox
        /// </summary>
        public Int32 MaxLength
        {
            get { return this.TextBox.MaxLength; }
            set { this.TextBox.MaxLength = value; }
        }

        /// <summary>
        /// Classe que define as configurações/comportamento/validação do controle
        /// </summary>
        private ICampoNovoAcesso campo;
        /// <summary>
        /// Classe que define as configurações/comportamento/validação do controle
        /// </summary>
        public ICampoNovoAcesso Campo
        {
            get 
            { 
                if(campo == null)
                    campo = ICampoNovoAcesso.Obter(this);
                return campo;
            }
        }

        /// <summary>
        /// SetFocusOnError.
        /// </summary>
        public Boolean SetFocusOnError 
        {
            get { return this.valCustomizado.SetFocusOnError; }
            set { this.valCustomizado.SetFocusOnError = value; }
        }

        /// <summary>
        /// Painel de exibição da Força da Senha e Critérios da Senha
        /// </summary>
        public Panel PainelSenha
        {
            get { return this.pnlSenha; }
        }

        /// <summary>
        /// Indica se o ícone de Ajuda de Nr de Estabelecimento deve ser exibido
        /// </summary>
        public Boolean ExibirIconeAjuda { get; set; }

        /// <summary>
        /// Indica se a mensagem de status do e-mail deve ser exibida
        /// </summary>
        public Boolean ExibirStatusEmail { get; set; }

        /// <summary>
        /// Painel com o ícone de ajuda de Estabelecimento
        /// </summary>
        public Panel PainelAjuda
        {
            get { return this.pnlHelp; }
        }

        /// <summary>
        /// Habilitar/Desabilitar validador
        /// </summary>
        public Boolean HabilitarValidador
        {
            get { return valCustomizado.Enabled; }
            set { valCustomizado.Enabled = value; }
        }

        /// <summary>
        /// Habilitar/Desabilitar textBox
        /// </summary>
        public Boolean Enabled 
        {
            get { return txtCampoNovoAcesso.Enabled; }
            set { txtCampoNovoAcesso.Enabled = value; }
        }

        #endregion

        #region [ Propriedades Privadas ]

        /// <summary>
        /// Configuração do controle, armazenada em um objeto JSON para
        /// acesso via javascript
        /// </summary>
        private ConfiguracaoCampoNovoAcesso Config
        {
            get 
            {
                var config = default(ConfiguracaoCampoNovoAcesso);
                if (!String.IsNullOrEmpty(hddConfig.Value))
                {
                    var deserializer = new DataContractJsonSerializer(typeof(ConfiguracaoCampoNovoAcesso));
                    using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(hddConfig.Value)))
                        config = (ConfiguracaoCampoNovoAcesso)deserializer.ReadObject(stream);
                }

                return config ?? new ConfiguracaoCampoNovoAcesso();
            }
            set 
            {
                var serializer = new DataContractJsonSerializer(typeof(ConfiguracaoCampoNovoAcesso));
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.WriteObject(stream, value);
                    hddConfig.Value = Encoding.UTF8.GetString(stream.ToArray());
                }
            }
        }
       
        #endregion

        #region [ Eventos de Página ]

        /// <summary>
        /// Evento OnInit da página
        /// </summary>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            //Preenche o conteúdo da legenda, com o conteúdo customizado, caso exista
            if (Legenda != null)
            {
                rowLegenda.Visible = true;
                phLegenda.Controls.Add(Legenda);
            }

            //Configura a função javascript de validação para a instância do controle
            valCustomizado.ClientValidationFunction = String.Concat(this.ClientID, ".Validar");

            //Armazena o tipo do controle
            this.tblCampoNovoAcesso.Attributes["tipo"] = this.Tipo.ToString().ToLower();

            //Armazena as configurações do controle em um objeto JSON para acesso via Javascript
            this.ArmazenarConfiguracaoJSON();

            //Inicializa as configurações do controle
            this.Campo.InicializarControle();
        }

        #endregion

        #region [ Métodos Públicos ]

        /// <summary>
        /// Retorna classe que define as configurações/comportamento/validação do controle.
        /// Equivalente à propriedade "Campo".
        /// </summary>
        public T ObterCampo<T>() where T : ICampoNovoAcesso
        {
            return (T)this.Campo;
        }

        /// <summary>
        /// Valida os dados do controle, exibindo a mensagem de validação.<br/>
        //// Retorna true caso válido.
        /// </summary>
        /// <returns>Se o conteúdo é válido</returns>
        public Boolean Validar()
        {
            return this.Campo.Validar(true);
        }

        /// <summary>
        /// Valida os dados do controle, exibindo a mensagem de validação de 
        /// acordo com o parâmetro "exibirMensagem".<br/>
        /// Retorna true caso válido.
        /// </summary>
        /// <param name="exibirMensagem">Flag indicando se deve exibir ou não a mensagem de validação</param>
        /// <returns>Se o conteúdo é válido</returns>
        public Boolean Validar(Boolean exibirMensagem)
        {
            return this.Campo.Validar(exibirMensagem);
        }

        #endregion
        
        #region [ Métodos Públicos - Mensagem ]

        /// <summary>
        /// Exibe mensagem de sucesso na validação
        /// Conteúdos dentro da tag &lt;span&gt;&lt;/span&gt; serão renderizados com a cor padrão cinza.<br/>
        /// Conteúdos fora da tag &lt;span&gt;&lt;/span&gt; serão renderizados em verde.
        /// </summary>
        /// <param name="mensagemHtml">Mensagem HTML</param>
        public void ExibirMensagemSucesso(String mensagemHtml)
        {
            this.ExibirMensagem(TipoMensagemNovoAcesso.Sucesso, mensagemHtml, true);
        }

        /// <summary>
        /// Exibe mensagem de sucesso na validação
        /// Conteúdos dentro da tag &lt;span&gt;&lt;/span&gt; serão renderizados com a cor padrão cinza.<br/>
        /// Conteúdos fora da tag &lt;span&gt;&lt;/span&gt; serão renderizados em verde.
        /// </summary>
        /// <param name="mensagemHtml">Mensagem HTML</param>
        public void ExibirMensagemSucessoSemIcone(String mensagemHtml)
        {
            this.ExibirMensagem(TipoMensagemNovoAcesso.Sucesso, mensagemHtml, false);
        }

        /// <summary>
        /// Exibe mensagem de erro na validação
        /// Conteúdos dentro da tag &lt;span&gt;&lt;/span&gt; serão renderizados com a cor padrão cinza.<br/>
        /// Conteúdos fora da tag &lt;span&gt;&lt;/span&gt; serão renderizados em vermelho.
        /// </summary>
        /// <param name="mensagemHtml">Mensagem HTML</param>
        public void ExibirMensagemErro(String mensagemHtml)
        {
            this.ExibirMensagem(TipoMensagemNovoAcesso.Erro, mensagemHtml, true);
        }

        /// <summary>
        /// Exibe mensagem de erro na validação
        /// Conteúdos dentro da tag &lt;span&gt;&lt;/span&gt; serão renderizados com a cor padrão cinza.<br/>
        /// Conteúdos fora da tag &lt;span&gt;&lt;/span&gt; serão renderizados em vermelho.
        /// </summary>
        /// <param name="mensagemHtml">Mensagem HTML</param>
        public void ExibirMensagemErroSemIcone(String mensagemHtml)
        {
            this.ExibirMensagem(TipoMensagemNovoAcesso.Erro, mensagemHtml, false);
        }

        /// <summary>
        /// Exibe mensagem de erro na validação
        /// Conteúdos dentro da tag &lt;span&gt;&lt;/span&gt; serão renderizados com a cor padrão cinza.<br/>
        /// Conteúdos fora da tag &lt;span&gt;&lt;/span&gt; serão renderizados em laranja.
        /// </summary>
        /// <param name="mensagemHtml">Mensagem HTML</param>        
        public void ExibirMensagemAviso(String mensagemHtml)
        {
            this.ExibirMensagem(TipoMensagemNovoAcesso.Aviso, mensagemHtml, true);
        }

        /// <summary>
        /// Exibe mensagem de erro na validação
        /// Conteúdos dentro da tag &lt;span&gt;&lt;/span&gt; serão renderizados com a cor padrão cinza.<br/>
        /// Conteúdos fora da tag &lt;span&gt;&lt;/span&gt; serão renderizados em laranja.
        /// </summary>
        /// <param name="mensagemHtml">Mensagem HTML</param>        
        public void ExibirMensagemAvisoSemIcone(String mensagemHtml)
        {
            this.ExibirMensagem(TipoMensagemNovoAcesso.Aviso, mensagemHtml, false);
        }
        
        /// <summary>
        /// Oculta a mensagem de validação
        /// </summary>
        public void RemoverMensagem()
        {
            this.ExibirMensagem(null, String.Empty, false);
        }

        /// <summary>
        /// Exibe mensagem de validação, de acordo com o tipo de mensagem.<br/>
        /// Conteúdos dentro da tag &lt;span&gt;&lt;/span&gt; serão renderizados com a cor padrão cinza.<br/>
        /// Conteúdos fora da tag &lt;span&gt;&lt;/span&gt; serão renderizados de acordo com a cor do tipo da mensagem.<br/>
        /// Exemplo: Se tipoMensagem == Sucesso, gera mensagem em vermelho.
        /// </summary>
        /// <param name="tipoMensagem">Tipo da mensagem</param>
        /// <param name="mensagemHtml">Mensagem HTML</param>
        private void ExibirMensagem(TipoMensagemNovoAcesso? tipoMensagem, String mensagemHtml, Boolean exibirIcone)
        {
            StringBuilder cssClass = new StringBuilder("msg");

            if (tipoMensagem.HasValue)
                cssClass.Append(" ").Append(tipoMensagem.Value.GetDescription());

            if (!exibirIcone)
                cssClass.Append(" no-icon");

            pnlMensagem.Attributes["class"] = cssClass.ToString();
            spanMensagem.InnerHtml = mensagemHtml;
        }

        /// <summary>
        /// Exibe mensagem de validação dos itens de segurança de requisitos da senha
        /// </summary>
        /// <param name="forcaSenha">Força da Senha (Inválida, Fraca, Média, Alta)</param>
        /// <param name="itensSeguranca">Itens de segurança (ícone e mensagem)</param>
        public void ExibirMensagensSenhaItemSeguranca(String forcaSenha, List<KeyValuePair<TipoMensagemNovoAcesso, String>> itensSeguranca)
        {
            //Força da Senha
            spanForcaSenha.InnerHtml = forcaSenha;
            
            //Preenchimento da barra
            switch (forcaSenha)
            {
                case "Inválida": pnlBar.CssClass = "bar p0"; break;
                case "Fraca":    pnlBar.CssClass = "bar p25"; break;
                case "Média":    pnlBar.CssClass = "bar p50"; break;
                case "Alta":     pnlBar.CssClass = "bar p75"; break;
                default:         pnlBar.CssClass = "bar p0"; break;
            }

            ulItensSeguranca.Controls.Clear();
            foreach (var itemSeguranca in itensSeguranca)
            {
                String cssClass = itemSeguranca.Key.GetDescription();
                String mensagem = itemSeguranca.Value;

                var listItem = new HtmlGenericControl("li");
                listItem.Attributes.Add("class", cssClass);

                var divItem = new HtmlGenericControl("div");
                divItem.InnerHtml = mensagem;

                listItem.Controls.Add(divItem);
                ulItensSeguranca.Controls.Add(listItem);
            }
        }

        #endregion

        #region [ Métodos Públics - Estáticos ] 

        /// <summary>
        /// Valida os dados de todos os controles, exibindo as mensagens de validação.<br/>
        /// Retorna true caso todos os controles estejam válidos.<br/>
        /// Atenção: não valida métodos customizados de validação (assinaturas diferentes de <code>Validar(String);</code>).
        /// </summary>        
        /// <returns>Se controles são válidos</returns>
        public static Boolean ValidarCampos(Control campoNovoAcesso, params Control[] camposNovoAcesso)
        {
            var campos = new List<Control>();

            if (campoNovoAcesso != null)
                campos.Add(campoNovoAcesso);

            if (camposNovoAcesso != null && camposNovoAcesso.Length > 0)
                campos.AddRange(camposNovoAcesso);

            return CampoNovoAcesso.Validar(true, campos.ToArray());
        }

        /// <summary>
        /// Valida os dados de todos os controles do tipo CampoNovoAcesso da coleção, 
        /// exibindo as mensagens se parâmetro "exibirMensagem=true".<br/>
        /// Retorna true caso todos os controles estejam válidos.<br/>
        /// Ignora possíveis controles repassados que não sejam do tipo CampoNovoAcesso.
        /// </summary>
        /// <param name="exibirMensagem">Flag indicando se deve exibir ou não a mensagem de validação</param>
        /// <returns>Se controles são válidos</returns>
        public static Boolean Validar(Boolean exibirMensagem, params Control[] camposNovoAcesso)
        {
            Boolean valido = true;
            if (camposNovoAcesso != null && camposNovoAcesso.Length > 0)
                foreach (Control campo in camposNovoAcesso)
                    if (campo is CampoNovoAcesso)
                        valido &= ((CampoNovoAcesso)campo).Validar(exibirMensagem);
            return valido;
        }

        #endregion

        #region [ Métodos Privados - Auxiliares ]

        /// <summary>
        /// Armazena as configs do controle no objeto JSON
        /// </summary>
        private void ArmazenarConfiguracaoJSON()
        {
            var config = new ConfiguracaoCampoNovoAcesso();
            config.ControlToCompare = this.ControleAssociado != null ? 
                this.ControleAssociado.ClientID : this.ControlToCompare;
            config.Obrigatorio = this.Obrigatorio;
            this.Config = config;
        }

        /// <summary>
        /// Busca recursiva para recuperar um controle através do seu Server ID.
        /// </summary>
        private static T FindControl<T>(Control startingControl, string id) where T : Control
        {
            T found = startingControl.FindControl(id) as T;

            if (found == null)
                found = FindChildControl<T>(startingControl, id);

            return found;
        }

        /// <summary>     
        /// Busca recursiva para recuperar um controle através do seu Server ID.
        /// Não considera o startingControl como sendo possível controle.
        /// </summary>
        private static T FindChildControl<T>(Control startingControl, string id) where T : Control
        {
            T found = null;

            foreach (Control activeControl in startingControl.Controls)
            {
                found = activeControl as T;

                if (found == null || (string.Compare(id, found.ID, true) != 0))
                    found = FindChildControl<T>(activeControl, id);

                if (found != null)
                    break;
            }

            return found;
        }

        #endregion       

        #region [ Métodos - Implementações IPostBackEventHandler ]

        /// <summary>
        /// Tratamento de RaisePostBackEvent
        /// </summary>
        /// <param name="eventArgument">Argumento do postback</param>
        public void RaisePostBackEvent(String eventArgument)
        {
            //Encaminha evento de postback para o "Campo"
            this.Campo.RaisePostBackEvent(eventArgument);
        }

        /// <summary>
        /// Gera string para postback
        /// </summary>
        public String GerarPostBackEventReference(String postBackArgument)
        {            
            return this.Page.ClientScript.GetPostBackEventReference(this, postBackArgument);
        }

        #endregion
    }  
}
