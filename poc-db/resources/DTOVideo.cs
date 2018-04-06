#region Used Namespaces
using System;
#endregion

namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Vídeos".
    /// </summary>
    public sealed class DTOVideo : DTOItem
    {
        /// <summary>
        ///   ID do vídeo.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título do vídeo.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Url do vídeo no YouTube.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///   Descrição das características do vídeo.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        ///   Data de criação do vídeo.
        /// </summary>
        public DateTime? DataDeCriacao { get; set; }

        /// <summary>
        ///   Informações do autor do vídeo.
        /// </summary>
        public string Autor { get; set; }

        /// <summary>
        ///   Quantidade de exibições do vídeo.
        /// </summary>
        public double? NumeroDeExibicoes { get; set; }
    }
}