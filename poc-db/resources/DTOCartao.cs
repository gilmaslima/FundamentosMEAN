namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Cartões".
    /// </summary>
    public sealed class DTOCartao : DTOItem
    {
        /// <summary>
        ///   ID do cartão.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Nome do cartão.
        /// </summary>
        public string Nome
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Imagem do cartão.
        /// </summary>
        public string Imagem { get; set; }

        /// <summary>
        ///   Descrição do cartão.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        ///   Tipo do cartão.
        /// </summary>
        public TipoDoCartão? TipoDoCartao { get; set; }

        /// <summary>
        ///   Campo necessário apenas se o cartão for de benefício.
        /// </summary>
        public string TipoDoCartaoDeBeneficio { get; set; }

        /// <summary>
        /// Ordenação dos cartões
        /// </summary>
        public double Ordem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Beneficios { get; set; }
    }
}