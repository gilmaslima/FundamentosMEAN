using System;

namespace Redecard.PN.Extrato.SharePoint.Modelo
{
    [Serializable]
    public class SPListaMotivosCreditoDebito
    {
        /// <summary>
        /// ID fornecido pelo WA
        /// </summary>
        public string CodigoID { get; set; }

        /// <summary>
        /// Código textual fornecido pelo WA
        /// </summary>
        public string CodigoOriginal { get; set; }

        /// <summary>
        /// Título customizado para o código textual fornecido pelo WA
        /// </summary>
        public string TituloCustomizado { get; set; }

        /// <summary>
        /// Descrição customizada para o código textual fornecido pelo WA
        /// </summary>
        public string DescritivoCustomizado { get; set; }
    }
}
