using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.SharePoint.Helper
{
    public static class ConfiguracaoVersao
    {
        private const String keyExtratoGrandesConsultas = "__key_extrato_grandesconsultas__";

        private static Boolean HabilitarExtratoV2 { get { return true; } }

        private static void ConfiguraGrandesConsultas(Int32 versao)
        {
            using (Logger Log = Logger.IniciarLog("Configuração Extrato Grandes Consultas: " + versao))
            {
                HttpContext.Current.Session[keyExtratoGrandesConsultas] = versao;
            }
        }

        /// <summary>
        /// Retorna a versão que será utilizada do Extrato - Grandes Consultas
        /// 1: Versão 1 do Extrato, sem Grandes Consultas <br/>
        /// 2: Versão 2 do Extrato, com Grandes Consultas
        /// </summary>
        public static Int32 VersaoGrandesConsultas()
        {
            return Convert.ToString(HttpContext.Current.Session[keyExtratoGrandesConsultas]).ToInt32(HabilitarExtratoV2 ? 2 : 1);
        }

        /// <summary>
        /// Retorna a versão que será utilizada do Extrato - Grandes Consultas
        /// 1: Versão 1 do Extrato, sem Grandes Consultas <br/>
        /// 2: Versão 2 do Extrato, com Grandes Consultas
        /// </summary>
        public static Int32 VersaoGrandesConsultas(HttpRequest Request)
        {
            if (Request.QueryString["grandesconsultas"] != null)
            {
                Int32 versaoQS = Request.QueryString["grandesconsultas"].ToInt32(VersaoGrandesConsultas());
                if(versaoQS == 1 || versaoQS == 2)
                    ConfiguraGrandesConsultas(versaoQS);
            }
            return VersaoGrandesConsultas();
        }
    }
}