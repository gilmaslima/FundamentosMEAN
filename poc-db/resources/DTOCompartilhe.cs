namespace Redecard.Portal.Aberto.Model.Repository.DTOs
{
    /// <summary>
    ///   
    /// </summary>
    public sealed class DTOCompartilhe : DTOItem
    {
        /// <summary>
        ///   ID do item.
        /// </summary>
        public new int? ID
        {
            get { return base.ID; }
            internal set { base.ID = value; }
        }

        /// <summary>
        ///   Título do item.
        /// </summary>
        public string Titulo
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        ///   
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///   
        /// </summary>
        public bool Visivel { get; set; }

        /// <summary>
        ///   
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Retorna o link formatado de compartilhamento do provider
        /// </summary>
        /// <param name="sCurrentUrl"></param>
        /// <returns></returns>
        public string GetLink(string sCurrentUrl, string sTitle) {
            return this.Url.Replace("[title]", sTitle).Replace("[url]", sCurrentUrl).Trim();
        }
    }
}