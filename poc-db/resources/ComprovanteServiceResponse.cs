using Redecard.PN.Request.SharePoint.XBChargebackServico;
using System;
using System.Collections.Generic;

namespace Redecard.PN.Request.SharePoint.Model
{
    /// <summary>
    /// Class modelo para o response de comprovantes
    /// </summary>
    public class ComprovanteServiceResponse
    {
        /// <summary>
        /// Lista com os comprovantes
        /// </summary>
        public List<ComprovanteModel> Comprovantes { get; set; }

        /// <summary>
        /// Quantidade total de registros existente em cache do lado servidor
        /// </summary>
        public Int32 QuantidadeTotalRegistrosEmCache { get; set; }

        /// <summary>
        /// Código de retorno (0 = sucesso)
        /// </summary>
        public Int32 CodigoRetorno { get; set; }
    }
}
