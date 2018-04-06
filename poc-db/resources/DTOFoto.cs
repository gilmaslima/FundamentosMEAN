#region Used Namespaces
using System;
#endregion

namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Fotos".
    /// </summary>
    public sealed class DTOFoto : DTOItem
    {
        /// <summary>
        ///   ID da foto.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título da foto.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Url da foto armazenada no Flicker.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///   Descrição das características da foto.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        ///   Data da foto.
        /// </summary>
        public DateTime? Data { get; set; }

        /// <summary>
        ///   Indica a galeria da foto.
        /// </summary>
        public string Galeria { get; set; }
    }
}