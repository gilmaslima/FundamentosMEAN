using System.Configuration;

namespace Rede.PN.AtendimentoDigital.Core.Config.LocalizadorServico
{
	/// <summary>
	/// Fornece suporte ao sistema de configuração para a seção de configuração 
	/// "infraestrutura/localizadorServico".
	/// </summary>
	public sealed class ConfiguracoesLocalizadorServico : ConfigurationSection
	{
		#region Propriedades

		/// <summary>
		/// Define os valores definidos na seção de configurações.
		/// </summary>
		private static ConfiguracoesLocalizadorServico configuracoes = ConfigurationManager.GetSection("infraestrutura/localizadorServico") as ConfiguracoesLocalizadorServico;

		/// <summary>
		/// Obtém as configurações do Localizador de Serviços.
		/// </summary>
		public static ConfiguracoesLocalizadorServico Configuracoes
		{
			get
			{
				if (ConfiguracoesLocalizadorServico.configuracoes == null)
					ConfiguracoesLocalizadorServico.configuracoes = new ConfiguracoesLocalizadorServico();

				return ConfiguracoesLocalizadorServico.configuracoes;
			}
		}

		/// <summary>
		/// Obtém uma coleção de <see cref="Servico"/>.
		/// </summary>
		[ConfigurationProperty("servicos", IsRequired = true)]
		public ColecaoServico Servicos
		{
			get { return ((ColecaoServico)(base["servicos"])); }
			set { base["servicos"] = value; }
		}

		#endregion
	}
}
