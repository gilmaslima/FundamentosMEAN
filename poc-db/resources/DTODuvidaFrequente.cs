namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Dúvidas Frequentes".
    /// </summary>
    public sealed class DTODuvidaFrequente : DTOItem
    {
        /// <summary>
        ///   ID da Dúvida Frequente.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título da Dúvida Frequente.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Pergunta da Dúvida Frequente.
        /// </summary>
        public string Pergunta { get; set; }

        /// <summary>
        ///   Resposta da Dúvida Frequente.
        /// </summary>
        public string Resposta { get; set; }

        /// <summary>
        ///   Assunto da Dúvida Frequente.
        /// </summary>
        public string Assunto { get; set; }

        /// <summary>
        ///   Ordem da Dúvida Frequente.
        /// </summary>
        public double? Ordem { get; set; }
    }
}
