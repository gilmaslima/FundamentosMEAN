using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Redecard.Portal.Aberto.WebParts.FormularioContato;
using Redecard.Portal.Helper.Conversores;
using Redecard.Portal.Helper.DTO;
using Redecard.Portal.Helper;

namespace Redecard.Portal.Aberto.WebParts.ControlTemplates
{
    public abstract class WebPartFormularioContatoBase : Microsoft.SharePoint.WebPartPages.WebPart
    {
        #region Variáveis privadas
        /// <summary>
        /// Implementação para tradução de string para lista de motivos de contato
        /// </summary>
        private ITraducao<string, IList<MotivoContato>> tradutor = new TradutorDeStringParaMotivoContato();

        /// <summary>
        /// Itens de motivo de contato editados pelo usuário
        /// </summary>
        private string motivosContato;
        #endregion

        #region Constantes de valores e mensagens
        protected static string valorPadraoSelecaoOpcao = "-1";
        protected static string textoPadraoSelecaoOpcao = RedecardHelper.ObterResource("formContato_SelecioneUmaOpcao");

        protected static string textoPadraoSucessoEnvioEmail = RedecardHelper.ObterResource("formContato_MensagemEnviadaComSucesso");
        protected static string textoPadraoCabecalhoErros = RedecardHelper.ObterResource("formContato_CorrijaSeguintesErrosTenteNovamente");
        protected static string textoPadraoCabecalhoEmail = RedecardHelper.ObterResource("formContato_CabecalhoEmail");
        #endregion

        #region Métodos
        /// <summary>
        /// Considerando que entra string vazia no campo Email com cópia do objeto MotivoContato, avalia se o item selecionado da lista contém email atrelado
        /// Útil para aplicação da regra de envio com cópia ou sem cópia.
        /// </summary>
        protected bool MotivoSelecionadoContemEmailDestinatario(ListControl lista)
        {
            return !string.IsNullOrEmpty(lista.SelectedValue);
        }

        /// <summary>
        /// Popula os itens de motivos de contato na lista indicada por parâmetro
        /// </summary>
        /// <param name="controleLista"></param>
        protected void PopularListaMotivosContato(ref DropDownList controleLista)
        {
            if (object.ReferenceEquals(controleLista, null)){
                controleLista = new DropDownList();
                controleLista.Width = Unit.Pixel(150);
            }
            foreach (MotivoContato mc in this.ListaMotivosContato)
                controleLista.Items.Add(new ListItem(mc.Descricao,mc.EmailDestinatario));

            controleLista.Items.Insert(0, new ListItem(WebPartFormularioContatoBase.textoPadraoSelecaoOpcao, WebPartFormularioContatoBase.valorPadraoSelecaoOpcao));
            controleLista.SelectedIndex = 0;
        }
        #endregion

        #region Propriedades Customizadas da WebPart
        /// <summary>
        /// Leitura/escrita de itens de Motivo de Contato gerenciáveis através das propriedades da WebPart
        /// Itens serializados em string
        /// </summary>
        public string MotivosContato
        {
            get
            {
                return this.motivosContato;
            }
            set
            {
                this.motivosContato = value;
            }
        }

        /// <summary>
        /// Retorna uma lista de Motivos de Contato.
        /// Esta lista é resultante do processo de conversão dos Motivos de Contato em string para List&lt;MotivoContato&gt;
        /// </summary>
        protected IList<MotivoContato> ListaMotivosContato
        {
            get
            {
                if (string.IsNullOrEmpty(this.MotivosContato))
                    return new List<MotivoContato>();

                return this.tradutor.Traduzir(this.MotivosContato);
            }
        }
        #endregion
    }
}