using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DataCash.SharePoint.WebParts
{
    public interface IEdicaoSenhaConnector
    {
        /// <summary>
        /// Sucesso na troca de senha
        /// </summary>
        void CarregarHeader(Boolean sucessoTrocaSenha);
    }
}
