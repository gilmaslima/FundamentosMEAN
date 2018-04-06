using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Redecard.PN.Comum.BlacklistValidations
{
    public class BlacklistPVs
    {
        /// <summary>
        /// Retorna a lista dos PVs bloqueados cadastrados no SharePoint
        /// </summary>
        public SPList ListaPvsBloqueados
        {
            get
            {
                SPList lista = null;
                // SPUtility.ValidateFormDigest();

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite spSite = SPContext.Current.Site.WebApplication.Sites["sites/fechado"])
                    using (SPWeb spWeb = spSite.AllWebs["minhaconta"])
                    {
                        lista = spWeb.Lists.TryGetList("PVs Bloqueados para Criação de Acesso");
                    }
                });

                return lista;
            }
        }

        /// <summary>
        /// Retorna o filtro dos PVs bloqueados aplicado à lista do SharePoint
        /// </summary>
        public List<int> PVsBloqueados
        {
            get
            {
                try
                {
                    var pvsBloqueados = ListaPvsBloqueados;
                    if (pvsBloqueados != null)
                    {
                        var query = new SPQuery();
                        query.Query = String.Concat(
                            "<Where>",
                                "<Eq>",
                                    "<FieldRef Name=\"Ativo\" />",
                                    "<Value Type=\"Boolean\">1</Value>",
                                "</Eq>",
                            "</Where>");

                        return pvsBloqueados.GetItems(query).Cast<SPListItem>().Select(x =>
                            {
                                int pv = 0;
                                int.TryParse(Convert.ToString(x["PV"]), out pv);
                                return pv;
                            }).ToList();
                    }
                }
                catch (SPException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                }

                return new List<int>();
            }
        }

        /// <summary>
        /// Valida se PV é válido, segundo a lista de PVs bloqueados
        /// </summary>
        /// <param name="pv"></param>
        /// <returns></returns>
        public bool ValidarPv(int pv)
        {
            List<int> pvReprovado;
            return this.ValidarPvs(new List<int> { pv }, out pvReprovado);
        }

        /// <summary>
        /// Valida se PVs informados não se encontram na Blacklist de PVs
        /// </summary>
        /// <param name="pvsValidar">Listagem dos PVs a serem validados</param>
        /// <returns>
        ///     TRUE: PVs validados
        ///     FALSE: Algum PV inválido
        /// </returns>
        public bool ValidarPvs(List<int> pvsValidar, out List<int> pvsReprovados)
        {
            pvsReprovados = new List<int>();
            foreach (int pv in pvsValidar)
            {
                if (this.PVsBloqueados.Contains(pv))
                {
                    pvsReprovados.Add(pv);
                }
            }

            // se houver algum PV reprovado, retorna como false
            return pvsReprovados.Count == 0;
        }
    }
}
