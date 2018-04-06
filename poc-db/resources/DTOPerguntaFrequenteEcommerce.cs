namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Perguntas Frequentes".
    /// </summary>
    public sealed class DTOPerguntaFrequenteEcommerce : DTOItem
    {
        /// <summary>
        ///   ID da pergunta.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Pergunta.
        /// </summary>
        public string Pergunta
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Resposta da pergunta.
        /// </summary>
        public string Resposta { get; set; }

        /// <summary>
        ///   Indica a área da pergunta.
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        ///   Indica o assunto da pergunta.
        /// </summary>
        public string Assunto { get; set; }

        /// <summary>
        ///   Indica número de ordem da pergunta.
        /// </summary>
        public double? Ordem { get; set; }
    }
}
