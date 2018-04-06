using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Redecard.PN.Extrato.SharePoint.Helper
{

    
    public class FormatarMoedaHelper
    {

        private static Dictionary<string, CultureInfo> TiposMoeda
        {
                get {
                    Dictionary<string, CultureInfo> tiposMoeda = new Dictionary<string, CultureInfo>();
                    tiposMoeda.Add(Constantes.Codigo_Tipo_Moeda_Dolar,Constantes.Desc_Tipo_Moeda_Dolar );
                    tiposMoeda.Add(Constantes.Codigo_Tipo_Moeda_real, Constantes.Desc_Tipo_Moeda_Real);
                    return tiposMoeda;
                }
                set {TiposMoeda = value;}

            }

        public static string FormataNumeroPorTipoMoeda(string tipoMoeda, decimal valor)
        {
            if (TiposMoeda.ContainsKey(tipoMoeda))
            {
                return valor.ToString("C", TiposMoeda[tipoMoeda]);
            }
            else
            {
                throw new Exception(string.Format("Tipo de moeda informado não é válido! Tipo de moeda[{0}]", tipoMoeda));
            }
        }
    }
}
