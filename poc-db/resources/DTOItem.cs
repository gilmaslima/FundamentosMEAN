namespace Redecard.Portal.Fechado.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade base para todos os repositórios de listas.
    /// </summary>
    public abstract class DTOItem
    {
        /// <summary>
        ///   ID do Item.
        /// </summary>
        protected int? ID { get; set; }

        /// <summary>
        ///   Título do Item.
        /// </summary>
        protected string Title { get; set; }
    }
}