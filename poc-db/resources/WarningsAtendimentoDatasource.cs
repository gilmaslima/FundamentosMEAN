using Microsoft.SharePoint;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Enums;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Datasource;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Warnings.Model.Content;
using Rede.PN.AtendimentoDigital.SharePoint.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Warnings.Datasource
{
    /// <summary>
    /// Define o datasource.
    /// </summary>
    public class WarningsAtendimentoDatasource : IDatasource
    {
        /// <summary>
        /// Define cacheLockGetItems.
        /// </summary>
        private static readonly Object cacheLockGetItems;
        /// <summary>
        /// Instância do contexto SPWeb
        /// </summary>
        public SPWeb Web { get; set; }
        /// <summary>
        /// Construtor estático.
        /// </summary>
        static WarningsAtendimentoDatasource()
        {
            cacheLockGetItems = new Object();
        }
        /// <summary>
        /// Construtor padrão
        /// </summary>
        /// <param name="web">Contexto SPWeb</param>
        public WarningsAtendimentoDatasource(SPWeb web)
        {
            this.Web = web;
        }
        /// <summary>
        /// Obtém os itens.
        /// </summary>
        /// <param name="useCache">Determina o uso do cache.</param>
        /// <returns>Lista de conteúdo.</returns>
        public List<WarningsAtendimentoItem> GetItems(Boolean useCache = true)
        {
            String cacheKey = "warningsAtendimento";
            var itens = new List<WarningsAtendimentoItem>();

            lock (cacheLockGetItems)
            {
                if (useCache)
                {
                    try
                    {
                        //tenta inicialmente recuperar do cache do AppFabric
                        //em caso de erro, utiliza cache em memória como fallback
                        itens = CacheAdmin.Recuperar<List<WarningsAtendimentoItem>>(Cache.Warnings, cacheKey);
                    }
                    catch (NullReferenceException ex)
                    {
                        itens = CacheAtendimento.GetItem<List<WarningsAtendimentoItem>>(cacheKey);
                        Logger.GravarErro("Erro 1 na consulta do cache AppFabric do RedePNWarnings, recuperando do cache em memória", ex);
                    }
                    catch (Exception ex)
                    {
                        itens = CacheAtendimento.GetItem<List<WarningsAtendimentoItem>>(cacheKey);
                        Logger.GravarErro("Erro 2 na consulta do cache AppFabric do RedePNWarnings, recuperando do cache em memória", ex);
                    }
                    
                    if (itens != null)
                        return itens;

                    itens = new List<WarningsAtendimentoItem>();
                }

                if (this.Web != null)
                {
                    //Obtém toda a lista de warnings
                    List<WarningsAtendimentoEntity> warnings = new WarningsAtendimentoRepository().GetAll(this.Web);

                    if (warnings != null && warnings.Count > 0)
                    {
                        foreach (WarningsAtendimentoEntity warning in warnings)
                        {
                            itens.Add(new WarningsAtendimentoItem()
                            {
                                Segmentos = warning.Segmentos,
                                NumerosPV = warning.NumerosPV,
                                Texto = warning.Texto,
                                URLExibicao = warning.URLExibicao,
                                URLDestino = warning.URLDestino,
                                Tipo = warning.Tipo,
                                TextoBotao = warning.TextoBotao,
                                DataInicioExibicao = warning.DataInicioExibicao,
                                DataFimExibicao = warning.DataFimExibicao,
                            });
                        }

                        if (useCache)
                        {                            
                            try
                            {
                                //tenta inicialmente adicionar no cache do AppFabric (4h / 240min)
                                //em caso de erro, utiliza cache em memória como fallback
                                CacheAdmin.Adicionar(Cache.Warnings, cacheKey, itens);
                            }
                            catch (NullReferenceException ex)
                            {
                                CacheAtendimento.AddItem(cacheKey, itens, 240);
                                Logger.GravarErro("Erro 1 na gravação no cache AppFabric do RedePNWarnings, utilizando cache em memória", ex);
                            }
                            catch (Exception ex)
                            {                                
                                CacheAtendimento.AddItem(cacheKey, itens, 240);
                                Logger.GravarErro("Erro 2 na gravação no cache AppFabric do RedePNWarnings, utilizando cache em memória", ex);
                            }
                        }
                    }
                }
                return itens;
            }
        }
    }
}
