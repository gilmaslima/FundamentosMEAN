using System;
using Winnovative.WnvHtmlConvert;

namespace Redecard.PN.DadosCadastrais.SharePoint.Business
{
    /// <summary>
    /// Classe para geracao de PDF copiado do projeto Extrato
    /// </summary>
    public class Pdf
    {
        private PdfConverter pdfConverter;

        /// <summary>
        /// Construtor da classe
        /// </summary>
        public Pdf()
        {
            pdfConverter = new PdfConverter();
            pdfConverter.LicenseKey = "rIedjJ2Mnp+YjJmCnIyfnYKdnoKVlZWV";
            pdfConverter.PdfDocumentOptions.FitWidth = true;
            pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            pdfConverter.PdfDocumentOptions.StretchToFit = true;
            pdfConverter.PdfDocumentOptions.TopMargin = 29;
            pdfConverter.PdfDocumentOptions.RightMargin = 29;
            pdfConverter.PdfDocumentOptions.BottomMargin = 29;
            pdfConverter.PdfDocumentOptions.LeftMargin = 29;
            pdfConverter.AvoidImageBreak = true;
            pdfConverter.AvoidTextBreak = true;
            pdfConverter.NavigationTimeout = 5000;
        }

        /// <summary>
        /// Gera um pdf com base em uma URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public byte[] GerarPdfUrl(String url)
        {
            return pdfConverter.GetPdfBytesFromUrl(url);
        }
    }
}
