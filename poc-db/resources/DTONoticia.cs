#region Used Namespaces
using System;
#endregion

namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Notícias".
    /// </summary>
    public sealed class DTONoticia : DTOItem
    {
        /// <summary>
        ///   ID da Notícia.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título da notícia.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Conteúdo descritivo da notícia.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        ///   Data da notícia.
        /// </summary>
        public DateTime? Data { get; set; }

        /// <summary>
        ///   Data limite na qual a notícia será exibida aos usuários.
        /// </summary>
        public DateTime? DataDeExpiracao { get; set; }

        /// <summary>
        ///   Hiperlink que a notícia redireciona.
        /// </summary>
        public string Hiperlink { get; set; }
    }
}