using System;
using System.ComponentModel;

namespace Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario
{
    /// <summary>
    /// Model para o quadro de aviso
    /// </summary>
    public class QuadroAvisoModel
    {
        /// <summary>
        /// Mensagem de exiboção
        /// </summary>
        [DefaultValue("")]
        public String Mensagem { get; set; }

        /// <summary>
        /// Título da mensagem (se houver)
        /// </summary>
        [DefaultValue("")]
        public String Titulo { get; set; }

        /// <summary>
        /// Tipo do quadro
        /// </summary>
        [DefaultValue(TipoModal.None)]
        public TipoModal Tipo { get; set; }

        /// <summary>
        /// Construtor da classe
        /// </summary>
        /// <param name="tipo">Tipo da mensagem</param>
        /// <param name="titulo">Título da mensagem</param>
        /// <param name="mensagem">Mensagem para exibição</param>
        public QuadroAvisoModel(TipoModal tipo, String titulo, String mensagem)
        {
            this.Tipo = tipo;
            this.Titulo = titulo;
            this.Mensagem = mensagem;
        }

        /// <summary>
        /// Construtor da classe
        /// </summary>
        /// <param name="tipo">Tipo da mensagem</param>
        /// <param name="mensagem">Mensagem para exibição</param>
        public QuadroAvisoModel(TipoModal tipo, String mensagem) :
            this(tipo, String.Empty, mensagem) { }
    }
}
