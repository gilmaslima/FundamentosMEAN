
using System;
using System.Web;

/// <summary>
/// 
/// </summary>
public class Helper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static String FormatarLink(String url)
    {
        Boolean ambienteSimulacao = AmbienteSimulacao;
        if (ambienteSimulacao)
        {
            return String.Format("/Intranet{0}", url);
        }
        return url;
    }

    /// <summary>
    /// 
    /// </summary>
    public static Boolean AmbienteSimulacao
    {
        get
        {
            return (HttpContext.Current.Request.Url.AbsoluteUri.ToLowerInvariant().Contains("redecard.simul") ? true : false);
        }
    }
}