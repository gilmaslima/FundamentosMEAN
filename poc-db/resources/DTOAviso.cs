namespace Redecard.Portal.Fechado.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Avisos".
    /// </summary>
    public sealed class DTOAviso : DTOItem
    {
        /// <summary>
        ///   ID do Aviso.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título do Aviso.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Descrição do Aviso.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Audiência do Aviso
        /// </summary>
        public string Audiencia { get; set; }
    }
}
