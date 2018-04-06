using System;
using System.ComponentModel;

namespace Redecard.PN.RAV.Core.Web.Controles.Portal
{
	/// <summary>
	/// Classe representando um passo do Assistente
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter)), Serializable]
    public class Passo
    {
        private String descricao;
        private Boolean exibirBlockUI;

        /// <summary>
        /// Evento de clique no passo
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        /// Exibição de BlockUI (Default: false)
        /// </summary>
        [NotifyParentProperty(true)]
        public Boolean ExibirBlockUI
        {
            get { return this.exibirBlockUI; }
            set { this.exibirBlockUI = value; }
        }

        /// <summary>
        /// Descrição do Passo
        /// </summary>
        [NotifyParentProperty(true)]
        public String Descricao
        {
            get { return this.descricao; }
            set { this.descricao = value; }
        }

        /// <summary>
        /// Executa o evento de clique no passo
        /// </summary>
        public void EfetuarClique(Object sender)
        {
            if (this.Click != null)
                this.Click(sender, null);
        }

        /// <summary>
        /// Indica se o evento de clique foi definido
        /// </summary>
        public Boolean EventoCliqueDefinido
        {
            get { return this.Click != null; }
        }
    }
}
