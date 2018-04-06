/*
© Copyright 2017 Rede S.A.
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
    public sealed class AccentuationConfig : List<List<Char>>
    {
        /// <summary>
        /// Bloqueador utilizado para garantir a sincronia entre threads concorrentes.
        /// </summary>
        private static readonly Object bloqueador = new Object();

        #region [ Constantes ]

        /// <summary>
        /// Nome da pasta
        /// </summary>
        private static String ConfigFolder { get { return "AtendimentoDigital/search"; } }

        /// <summary>
        /// Nome do arquivo de configuração
        /// </summary>
        private static String ConfigFile { get { return "accentuation.txt"; } }

        #endregion        

        #region [ Construtor ]

        /// <summary>
        /// Construtor privado, para evitar instanciação fora da classe
        /// </summary>
        private AccentuationConfig() { }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="fileContent">Conteúdo/Linhas que contém as acentuações e que serão processadas</param>
        private AccentuationConfig(String[] fileContent)
        {
            Initialize(fileContent);
        }

        #endregion

        #region [ Método Privados ]

        /// <summary>
        /// Inicialização da instância com base nas linhas de acentuação
        /// </summary>
        /// <param name="fileContent">Conteúdo do arquivo de acentuação</param>
        private void Initialize(String[] fileContent)
        {
            //tratamento caso conteúdo seja inválido
            if (fileContent == null)
                return;

            //processa as linhas recebidas
            this.AddRange(fileContent

                //Ignora as linhas em branco ou iniciadas com '#' (linhas de comentário)
                .Where(linha => !String.IsNullOrWhiteSpace(linha) &&
                    linha.Length > 0 &&
                    linha[0] != '#')

                //Considera apenas as linhas que realmente possuem equivalência de caracteres (mais de um caractere por linha)
                .Where(linha => linha.Length > 1)
                .Select(linha => linha.ToLowerInvariant().ToList())
                .ToList());
        }

        #endregion

        #region [ Métodos Públicos ]

        /// <summary>
        /// Geração de regular expression com base na 'palavra' informada, 
        /// adicionando caracteres equivalentes de acentuação.
        /// </summary>
        /// <param name="palavra">Palavra que será tratada</param>
        /// <returns>Regex da palavra, com expressão regular de caracteres equivalentes</returns>
        public String GerarRegex(String palavra)
        {
            foreach (List<Char> equivalentes in this)
                palavra = String.Join("(" + String.Join("|", equivalentes) + ")", palavra.Split(equivalentes.ToArray()));
            return palavra;
        }

        /// <summary>
        /// Geração de lista de regular expression com base nas 'palavras' recebidas.
        /// Adiciona caracteres equivalentes de acentuação em cada regex gerada.
        /// </summary>
        /// <param name="palavras">Lista de palavras</param>
        /// <returns>Lista de expressões regulares</returns>
        public List<String> GerarRegex(IEnumerable<String> palavras)
        {
            if (palavras == null)
                palavras = new String[0];

            return palavras.Select(palavra => GerarRegex(palavra)).ToList();
        }

        #endregion

        #region [ Métodos estáticos públicos ]

        /// <summary>
        /// Obtém a instância da configuração de acentuação
        /// </summary>
        /// <param name="web">SPWeb</param>
        /// <param name="useCache">Se utiliza cache ou não</param>
        /// <returns>Configuração de acentuação</returns>
        public static AccentuationConfig GetInstance(SPWeb web, Boolean useCache)
        {
            String cacheKey = "AcentuacaoConfig_" + web.ID.ToString();
            AccentuationConfig instance = null;

            lock (bloqueador)
            {
                //Se UseCache, tenta recuperar dicionário do Cache
                if (useCache == true)
                {
                    instance = CacheAtendimento.GetItem<AccentuationConfig>(cacheKey);
                }

                //se não está mais no cache, realiza leitura do arquivo de configuração direto da
                //biblioteca do SharePoint
                if (instance == null)
                {
                    //Lê arquivos de configuração da biblioteca AtendimentoDigital/search
                    var repository = new AtendimentoDigitalRepository();

                    String[] lines = repository.GetFileLines(web, ConfigFolder, ConfigFile);

                    instance = new AccentuationConfig(lines);

                    if (useCache == true)
                        CacheAtendimento.AddItem(cacheKey, instance);
                }
            }

            return instance;
        }

        #endregion
    }
}