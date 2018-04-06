using System;

namespace Redecard.PN.Request.SharePoint.Model
{
    /// <summary>
    /// Modelo para a lista de documentos por motivo de chargeback
    /// </summary>
    [Serializable]
    public class DocMotivoChargeback
    {
        public Int16 CodMotivo { get; set; }
        public String Documentos { get; set; }
    }
}
