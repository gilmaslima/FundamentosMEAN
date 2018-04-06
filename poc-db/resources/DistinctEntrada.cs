using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento;

namespace Redecard.PN.Cancelamento.Sharepoint.Modelos
{
    public class DistinctEntrada : IEqualityComparer<ItemCancelamentoEntrada> 
    {
        public bool Equals(ItemCancelamentoEntrada x, ItemCancelamentoEntrada y)
        {
            return x.NSU.Equals(y.NSU) && x.DtTransf.Equals(y.DtTransf) && x.VlTrans.Equals(y.VlTrans);
        }

        public int GetHashCode(ItemCancelamentoEntrada obj)
        {
            return obj.GetHashCode();
        }
    }
}
