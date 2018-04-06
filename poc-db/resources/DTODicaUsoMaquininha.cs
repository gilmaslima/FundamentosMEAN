namespace Redecard.Portal.Fechado.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Dicas de Uso da Maquininhas".
    /// </summary>
    public sealed class DTODicaUsoMaquininha : DTOItem
    {
        /// <summary>
        ///   ID do Dica de Uso da Maquininha.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título do Dica de Uso da Maquininha.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Hyperlink do Dica de Uso da Maquininha.
        /// </summary>
        public string Hiperlink { get; set; }
    }
}
