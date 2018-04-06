using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento
{
   public partial class ItemCancelamentoEntrada
    {

        #region Propriedades
        public string DataCancelamentoFormatada
        {
            get
            {
                return this.DtTransf != null && this.DtTransf.CompareTo(DateTime.MinValue) != 0 ? this.DtTransf.ToString("dd/MM/yyyy"): string.Empty;
            }
        }

        public string NumAviso { get; set; }

        public String FormaVenda { get; set; }

        public string NumeroAutorizacao { get; set; }

        public string DataHoraTransacao { get; set; }

        public List<ModConsultaDuplicado> ListaDuplicados { get; set; }

        public String TpVendaFormatado {
            get {
                return this.TpVenda.CompareTo("RO") == 0 ? "À Vista" : this.TpVenda.CompareTo("PC") == 0 ? "Parcelado Emissor" : "Parcelado Estabelecimento";
            } 
            
        }

        public String NSUFormatado
        {
            get
            {
                if (this.NSU.Length == 16)
                {
                    return this.NSU.Substring(0, 6) + "******" + this.NSU.Substring(12, 4);
                }
                else {
                    return NSU;
                }
            }

        }

        public string VlTransFormatado {
            get
            {
                return this.VlTrans.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

        public string VlCancFormatado
        {
            get
            {
                return this.VlCanc.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR"));
            }
        }

        public string mensagemErro { get; set; }
        #endregion

    }
}
