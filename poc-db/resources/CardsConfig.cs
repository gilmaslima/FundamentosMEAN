/*
© Copyright 2017 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.SharePoint;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content;
using Rede.PN.AtendimentoDigital.SharePoint.Repository;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Config
{
    /// <summary>
    /// Classe representando o arquivo de configuração dos cards
    /// </summary>
    public class CardsConfig : List<CardConfig>
    {
        /// <summary>
        /// Bloqueador utilizado para garantir a sincronia entre threads concorrentes.
        /// </summary>
        private static readonly Object bloqueador = new Object();

        #region [ Constantes ]

        /// <summary>
        /// Caminho da pasta dos arquivos de configuração da busca
        /// </summary>
        public static String ConfigFolder { get { return "AtendimentoDigital/search"; } }

        /// <summary>
        /// Caminho do arquivo de configuração dos cards
        /// </summary>
        public static String ConfigFile { get { return "cards-config.txt"; } }

        #endregion

        #region [ Construtores ]

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="fileContent">Conteúdo/Linhas que contém o mapeamento dos cards vs. termos</param>
        public CardsConfig(String[] fileContent) : base()
        {
            Initialize(fileContent);
        }

        #endregion

        #region [ Métodos Privados ]

        /// <summary>
        /// Inicialização da instância com base nas linhas contendo o mapeamento dos cards vs. termos
        /// </summary>
        /// <param name="fileContent">Conteúdo do arquivo de mapeamento dos cards vs. termos</param>
        private void Initialize(String[] fileContent)
        {
            if (fileContent == null)
                return;

            //Ignora as linhas em branco ou iniciadas com '#' (linhas de comentário)
            fileContent = fileContent
                .Where(linha => !String.IsNullOrWhiteSpace(linha) &&
                    linha.Length > 0 &&
                    linha[0] != '#').ToArray();

            try
            {
                //deserializa json em objeto de configuração de cards e inclui na instância atual

                String jsonCardsConfig = String.Join(Environment.NewLine, fileContent);
                var serializer = new DataContractJsonSerializer(typeof(CardConfig[]));

                if (!String.IsNullOrWhiteSpace(jsonCardsConfig))
                {
                    var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonCardsConfig));
                    var cardsConfig = serializer.ReadObject(memoryStream) as CardConfig[];
                    if (cardsConfig != null && cardsConfig.Length > 0)
                    {
                        this.AddRange(cardsConfig);
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                Logger.GravarErro("Erro durante leitura de configuração de cards", ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante leitura de configuração de cards", ex);
            }
        }

        #endregion

        #region [ Métodos estáticos públicos ]

        /// <summary>
        /// Obtém a instância da configuração de cards
        /// </summary>
        /// <param name="web">SPWeb</param>
        /// <param name="useCache">Se utiliza cache ou não</param>
        /// <returns>Configuração de cards</returns>
        public static CardsConfig GetInstance(SPWeb web, Boolean useCache)
        {
            String cacheKey = "CardsConfig_" + web.ID.ToString();
            CardsConfig instance = null;

            lock (bloqueador)
            {
                //Se UseCache, tenta recuperar dicionário do Cache
                if (useCache == true)
                {
                    instance = CacheAtendimento.GetItem<CardsConfig>(cacheKey);
                }

                //se não está mais no cache, realiza leitura do arquivo de configuração direto da
                //biblioteca do SharePoint
                if (instance == null)
                {
                    //Lê arquivos de configuração da biblioteca AtendimentoDigital/search
                    var repository = new AtendimentoDigitalRepository();

                    String[] lines = repository.GetFileLines(web, ConfigFolder, ConfigFile);

                    instance = new CardsConfig(lines);

                    if (useCache == true)
                        CacheAtendimento.AddItem(cacheKey, instance);
                }
            }

            return CacheAtendimento.GetItem<CardsConfig>(cacheKey);
        }

        #endregion
    }
}
