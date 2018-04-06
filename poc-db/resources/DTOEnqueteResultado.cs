namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Enquetes - Resultados".
    /// </summary>
    public sealed class DTOEnqueteResultado : DTOItem
    {
        /// <summary>
        ///   ID da Enquete - Resultado.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título da Enquete - Resultado.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Pergunta da Enquete - Resultado.
        /// </summary>
        public DTOEnquetePergunta Pergunta { get; set; }

        /// <summary>
        ///   Resposta da Enquete - Resultado.
        /// </summary>
        public DTOEnqueteResposta Resposta { get; set; }

        /// <summary>
        ///   Usuário da Enquete - Resultado.
        /// </summary>
        public string Usuario { get; set; }
    }
}
