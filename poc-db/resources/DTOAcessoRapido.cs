namespace Redecard.Portal.Fechado.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Acessos Rápidos".
    /// </summary>
    public sealed class DTOAcessoRapido : DTOItem
    {
        /// <summary>
        ///   ID do Acesso Rápido.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título do Acesso Rápido.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Hyperlink do Acesso Rápido.
        /// </summary>
        public string Hiperlink { get; set; }
    }
}
