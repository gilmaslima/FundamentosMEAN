#region Used Namespaces
using System;
#endregion

namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Releases".
    /// </summary>
    public sealed class DTORelease : DTOItem
    {
        /// <summary>
        ///   ID do release.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título do release.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Conteúdo descritivo do release
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        ///   Data do release.
        /// </summary>
        public DateTime? Data { get; set; }

        /// <summary>
        ///   Hiperlink que a notícia redireciona.
        /// </summary>
        public string Hiperlink { get; set; }
    }
}