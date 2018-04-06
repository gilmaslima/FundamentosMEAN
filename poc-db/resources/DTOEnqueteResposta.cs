namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Enquetes - Respostas".
    /// </summary>
    public sealed class DTOEnqueteResposta : DTOItem
    {
        /// <summary>
        ///   ID da Enquete - Resposta.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título da Enquete - Resposta.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Resposta da Enquete - Resposta.
        /// </summary>
        public string Resposta { get; set; }

        /// <summary>
        ///   Pergunta da Enquete - Resposta.
        /// </summary>
        public DTOEnquetePergunta Pergunta { get; set; }
    }
}
