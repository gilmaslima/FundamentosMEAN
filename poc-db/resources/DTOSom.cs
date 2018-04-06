#region Used Namespaces
using System;
#endregion

namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Sons".
    /// </summary>
    public sealed class DTOSom : DTOItem
    {
        /// <summary>
        ///   ID do som.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título do som.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Url do som armazenado no PodBr.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///   Descrição das características do som.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        ///   Data do som.
        /// </summary>
        public DateTime? Data { get; set; }

        /// <summary>
        ///   Indica o tipo deste som.
        /// </summary>
        public string TipoDoSom { get; set; }

        /// <summary>
        ///   Quantidade de execuções deste som.
        /// </summary>
        public double? NumeroDeExecucoes { get; set; }

        /// <summary>
        ///   Url dos anexos do download.
        /// </summary>
        public string Anexos { get; internal set; }
    }
}