using System.Configuration;

namespace Rede.PN.AtendimentoDigital.Core.Config.Dados
{
	/// <summary>
	/// Fornece suporte ao sistema de configuração para a seção de configuração 
	/// "infraestrutura/conexoesBancoDados".
	/// </summary>
	public sealed class ConfiguracoesConexoesBancoDados : ConfigurationSection
	{
		#region Propriedades

		/// <summary>
		/// Define os valores definidos na seção de configurações.
		/// </summary>
		private static ConfiguracoesConexoesBancoDados configuracoes = GestorConfiguracao.ObterSecaoConfiguracao<ConfiguracoesConexoesBancoDados>("infraestrutura/conexoesBancoDados");

		/// <summary>
		/// Obtém as configurações.
		/// </summary>
		public static ConfiguracoesConexoesBancoDados Configuracoes
		{
			get
			{
				if (ConfiguracoesConexoesBancoDados.configuracoes == null)
					ConfiguracoesConexoesBancoDados.configuracoes = new ConfiguracoesConexoesBancoDados();

				return ConfiguracoesConexoesBancoDados.configuracoes;
			}
		}

		/// <summary>
		/// Obtém uma coleção de <see cref="ConexaoBancoDados"/>.
		/// </summary>
		[ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public ColecaoConexaoBancoDados Conexoes
		{
			get { return ((ColecaoConexaoBancoDados)(base[""])); }
			set { base[""] = value; }
		}

		#endregion
	}
}
