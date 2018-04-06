namespace Redecard.Portal.Fechado.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Links Favoritos".
    /// </summary>
    public sealed class DTOLinkFavorito : DTOItem
    {
        /// <summary>
        ///   ID do Link Favorito.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título do Link Favorito.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Hyperlink do Link Favorito.
        /// </summary>
        public string Hiperlink { get; set; }
    }
}
