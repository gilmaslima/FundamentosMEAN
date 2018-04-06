namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   Entidade que representa um item da lista "Downloads".
    /// </summary>
    public sealed class DTODownload : DTOItem
    {
        /// <summary>
        ///   ID do download.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título do download.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   Url dos anexos do download.
        /// </summary>
        public string Anexos { get; internal set; }

        /// <summary>
        ///   Descrição do download.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        ///   Indica a área na qual este arquivo faz parte.
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        ///   Indica o assunto deste arquivo.
        /// </summary>
        public string Assunto { get; set; }

        /// <summary>
        ///   Quantidade de downloads deste arquivo.
        /// </summary>
        public double? NumeroDeDownloads { get; set; }

        /// <summary>
        ///   Quantidade de downloads deste arquivo.
        /// </summary>
        public TipoDeDownload? TipoDeDownload { get; set; }

    }
}