using System;
namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Enquetes - Perguntas".
    /// </summary>
    public sealed class DTOEnquetePergunta : DTOItem
    {
        /// <summary>
        ///   ID da Enquete - Pergunta.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título da Enquete - Pergunta.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Pergunta da Enquete - Pergunta.
        /// </summary>
        public string Pergunta { get; set; }

        /// <summary>
        ///   Validez da Enquete - Pergunta.
        /// </summary>
        public bool? Ativo { get; set; }

        /// <summary>
        ///   Data de Início da Enquete - Pergunta.
        /// </summary>
        public DateTime? DataDeInicio { get; set; }

        /// <summary>
        ///   Data de Fim da Enquete - Pergunta.
        /// </summary>
        public DateTime? DataDeFim { get; set; }
    }
}
