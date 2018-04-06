namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Depoimentos".
    /// </summary>
    public sealed class DTODepoimento : DTOItem
    {
        /// <summary>
        ///   ID do depoimento.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Dados do autor do depoimento.
        /// </summary>
        public string DadosDoAutor
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Depoimento do usuário.
        /// </summary>
        public string Depoimento { get; set; }

        /// <summary>
        ///   Indica o ramo do qual este depoimento faz parte.
        /// </summary>
        public string Ramo { get; set; }

        /// <summary>
        ///   Indica o produto ou serviço deste depoimento.
        /// </summary>
        public string ProdutoServico { get; set; }

        /// <summary>
        ///   Indica o estado deste depoimento.
        /// </summary>
        public string Estado { get; set; }
    }
}