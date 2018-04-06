namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Dicas".
    /// </summary>
    public sealed class DTORedecardClique : DTOItem
    {
        /// <summary>
        ///   ID do item.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título do item.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Endereço de acesso ao selecionar um item
        /// </summary>
        public string Url { get; set; }
    }
}