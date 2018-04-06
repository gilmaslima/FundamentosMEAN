namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Sub Menus dos Mini Sites".
    /// </summary>
    public sealed class DTOSubMenuMiniSite : DTOItem
    {
        /// <summary>
        ///   ID do SubMenu do Mini Site.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título do SubMenu do Mini Site.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Hiperlink do SubMenu do Mini Site.
        /// </summary>
        public string Hiperlink { get; set; }
    }
}
