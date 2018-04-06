using System;
using System.Configuration;

namespace Rede.PN.AtendimentoDigital.Core.Config.Dados
{
    /// <summary>
    /// Fornece suporte ao sistema de configuração para a seção de configuração 
    /// "infraestrutura/dados".
    /// </summary>
    public sealed class ConfiguracoesDados : ConfigurationSection
    {
        #region Propriedades

        /// <summary>
        /// Define os valores definidos na seção de configurações.
        /// </summary>
        private static ConfiguracoesDados configuracoes = GestorConfiguracao.ObterSecaoConfiguracao<ConfiguracoesDados>("infraestrutura/dados");

        /// <summary>
        /// Obtém as configurações do Repositorios de dados.
        /// </summary>
        public static ConfiguracoesDados Configuracoes
        {
            get
            {
                if (ConfiguracoesDados.configuracoes == null)
                    ConfiguracoesDados.configuracoes = new ConfiguracoesDados();

                return ConfiguracoesDados.configuracoes;
            }
        }

        /// <summary>
        /// Obtém uma coleção de <see cref="Repositorio"/>.
        /// </summary>
        [ConfigurationProperty("repositorios", IsRequired = true)]
        public ColecaoRepositorio Repositorios
        {
            get { return ((ColecaoRepositorio)(base["repositorios"])); }
            set { base["repositorios"] = value; }
        }

        /// <summary>
        /// Obtém o nome da conexão padrão para repositório.
        /// </summary>
        [ConfigurationProperty("nomeConexaoPadrao", IsRequired = true)]
        public String NomeConexaoPadrao
        {
            get { return base["nomeConexaoPadrao"].ToString(); }
            set { base["nomeConexaoPadrao"] = value; }
        }

        #endregion
    }
}
