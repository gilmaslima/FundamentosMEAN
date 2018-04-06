using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;

namespace Rede.PN.CondicaoComercial.SharePoint.Business
{
    /// <summary>
    /// Listagem das mandeiras
    /// </summary>
    public class ListaImagensBandeira
    {
        /// <summary>
        /// Obté as imagens salvas na lista do Sharepoint - /sites/fechado/ImagensBandeira
        /// </summary>
        /// <returns></returns>
        public static List<BandeiraImagem> ObterImagensBandeiras()
        {
            List<BandeiraImagem> retorno = new List<BandeiraImagem>();
            String nomeLista = "ImagensBandeira";

            using (SPSite sites = new SPSite(SPUtility.GetFullUrl(SPContext.Current.Site, "/sites/fechado/")))
            using (SPWeb web = sites.OpenWeb("minhaconta"))
            {
                var lista = web.Lists.TryGetList(nomeLista);

                if (lista != null)
                {
                    foreach (SPListItem item in lista.Items)
                    {
                        retorno.Add(new BandeiraImagem
                        {
                            Codigo = item["Codigo"].ToString().ToInt32(),
                            Descricao = item["Title"].ToString(),
                            Url = item["ows_EncodedAbsUrl"].ToString()
                        });
                    }
                }
            }
            return retorno;
        }
    }

}
