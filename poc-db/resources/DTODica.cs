using System;
namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Dicas".
    /// </summary>
    public sealed class DTODica : DTOItem
    {
        /// <summary>
        ///   ID da dica.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título da dica.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Curto resumo do conteúdo da dica.
        /// </summary>
        public string Resumo { get; set; }

        /// <summary>
        ///   Conteúdo da dica.
        /// </summary>
        public string Dica { get; set; }

        /// <summary>
        ///   Indica o tipo da dica.
        /// </summary>
        public TipoDaDica? TipoDaDica { get; set; }

        /// <summary>
        ///   Indica a categoria da dica.
        /// </summary>
        public string Categoria { get; set; }

        /// <summary>
        ///   Quantidade de exibições da dica.
        /// </summary>
        public double? NumeroDeExibicoes { get; set; }

        /// <summary>
        /// Data de publicação da Dica
        /// </summary>
        public string DataPublicacao { get; set; }

    }
}