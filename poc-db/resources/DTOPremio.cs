#region Used Namespaces
using System;
#endregion

namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Prêmios".
    /// </summary>
    public sealed class DTOPremio : DTOItem
    {
        /// <summary>
        ///   ID do prêmio.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título do prêmio.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Url da imagem ilustrativa do prêmio.
        /// </summary>
        public string Imagem { get; set; }

        /// <summary>
        ///   Descrição do prêmio.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        ///   Data do prêmio.
        /// </summary>
        public DateTime? Data { get; set; }

        /// <summary>
        ///   Indica o categoria deste prêmio.
        /// </summary>
        public string Categoria { get; set; }
    }
}