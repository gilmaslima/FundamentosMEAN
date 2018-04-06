using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DataCash.Modelo
{
    /// <summary>
    /// Modelo representando o retorno XML recebido do DataCash ("&lt;Response /&gt;")
    /// </summary>
    [Serializable]    
    public class RetornoTransacaoXML
    {
        /// <summary>Tag "acquirer"</summary>
        public String Acquirer { get; set; }

        /// <summary>Tag "gateway_reference"</summary>
        public String GatewayReference { get; set; }
              
        /// <summary>Tag "extended_response_message"</summary>
        public String ExtendedResponseMessage { get; set; }

        /// <summary>Tag "extended_status"</summary>
        public String ExtendedStatus { get; set; }

        /// <summary>Tag "merchantreference"</summary>
        public String MerchantReference { get; set; }

        /// <summary>Tag "mid"</summary>
        public String Mid { get; set; }

        /// <summary>Tag "mode"</summary>
        public String Mode { get; set; }

        /// <summary>Tag "reason"</summary>
        public String Reason { get; set; }

        /// <summary>Tag "status"</summary>
        public Int32 Status { get; set; }

        /// <summary>Tag "time"</summary>
        public DateTime? Time { get; set; }

        /// <summary>Tag "HistoricTxn\authcode"</summary>
        public String HistoricTxnAuthCode { get; set; }

        /// <summary>Tag "auth_host_reference"</summary>
        public String AuthHostReference { get; set; }

        /// <summary>Tag "information"</summary>
        public String Information { get; set; }

        /// <summary>Tag "lastsuccessfulpayment"</summary>
        public String LastSuccessfulPayment { get; set; }

        /// <summary>Tag "CardTxn\authcode"</summary>
        public String CardTxnAuthCode { get; set; }

        /// <summary>Tag "CardTxn\card_scheme"</summary>
        public String CardTxnCardScheme { get; set; }

        /// <summary>Tag "CardTxn\country"</summary>
        public String CardTxnCountry { get; set; }

        /// <summary>Tag "CardTxn\issuer"</summary>
        public String CardTxnIssuer { get; set; }

        /// <summary>Tag "CardTxn\token"</summary>
        public String CardTxnToken { get; set; }

        /// <summary>Tag "ContAuthTxn\account_status"</summary>
        public String ContAuthTxnAccountStatus { get; set; }
    }
}