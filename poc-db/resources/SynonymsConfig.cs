/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Rede.PN.AtendimentoDigital.SharePoint.Repository;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Config
{
    /// <summary>
    /// Classe representando o arquivo de configuração de sinônimos
    /// </summary>
    public class SynonymsConfig : List<List<String>>
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
        /// Caminho do arquivo de sinônimos contido na pasta de configurações
        /// </summary>
        public static String ConfigFile { get { return "synonyms.txt"; } }

        #endregion

        #region [ Construtores ]

        /// <summary>
        /// Construtor privado, para evitar instanciação fora da classe
        /// </summary>
        private SynonymsConfig() { }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="fileContent">Conteúdo/Linhas que contém os sinônimos das palavras</param>
        public SynonymsConfig(String[] fileContent)
        {
            Initialize(fileContent);
        }

        #endregion

        #region [ Métodos Privados ]

        /// <summary>
        /// Inicialização da instância com base nas linhas de sinônimos
        /// </summary>
        /// <param name="fileContent">Conteúdo do arquivo de sinônimos</param>
        private void Initialize(String[] fileContent)
        {
            if (fileContent == null)
                return;

            this.AddRange(fileContent

                //Ignora as linhas em branco ou iniciadas com '#' (linhas de comentário)
                .Where(linha => !String.IsNullOrWhiteSpace(linha) &&
                    linha.Length > 0 &&
                    linha[0] != '#')

                //Divide cada linha com o separador ';' e remove duplicações na mesma linha
                .Select(linha => linha.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(palavra => palavra.Trim().ToLowerInvariant())
                    .Distinct().ToList())

                //Considera apenas as linhas que realmente possuem um sinônimo (mais de uma palavra por linha)
                .Where(linha => linha.Count > 1)
                .ToList());
        }

        #endregion

        #region [ Métodos estáticos públicos ]

        /// <summary>
        /// Obtém a instância da configuração de sinônimos
        /// </summary>
        /// <param name="web">SPWeb</param>
        /// <param name="useCache">Se utiliza cache ou não</param>
        /// <returns>Configuração de sinônimos</returns>
        public static SynonymsConfig GetInstance(SPWeb web, Boolean useCache)
        {
            String cacheKey = "SynonymsConfig_" + web.ID.ToString();
            SynonymsConfig instance = null;

            lock (bloqueador)
            {
                //Se UseCache, tenta recuperar dicionário do Cache
                if (useCache == true)
                {
                    instance = CacheAtendimento.GetItem<SynonymsConfig>(cacheKey);
                }

                //se não está mais no cache, realiza leitura do arquivo de configuração direto da
                //biblioteca do SharePoint
                if (instance == null)
                {
                    //Lê arquivos de configuração da biblioteca AtendimentoDigital/search
                    var repository = new AtendimentoDigitalRepository();

                    String[] lines = repository.GetFileLines(web, ConfigFolder, ConfigFile);

                    instance = new SynonymsConfig(lines);

                    if (useCache == true)
                        CacheAtendimento.AddItem(cacheKey, instance);
                }
            }

            return instance;
        }

        #endregion
    }
}
