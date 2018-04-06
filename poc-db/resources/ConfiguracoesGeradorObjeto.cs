using System.Configuration;

namespace Rede.PN.AtendimentoDigital.Core.Config.GeradorObjeto
{
	/// <summary>
	/// Fornece suporte ao sistema de configuração para a seção de configuração 
	/// "infraestrutura/geradorObjeto".
	/// </summary>
	public sealed class ConfiguracoesGeradorObjeto : ConfigurationSection
	{
		#region Propriedades

		/// <summary>
		/// Define os valores definidos na seção de configurações.
		/// </summary>
		private static ConfiguracoesGeradorObjeto configuracoes = ConfigurationManager.GetSection("infraestrutura/geradorObjeto") as ConfiguracoesGeradorObjeto;

		/// <summary>
		/// Obtém as configurações do Gerador de Obtejos.
		/// </summary>
		public static ConfiguracoesGeradorObjeto Configuracoes
		{
			get
			{
				if (ConfiguracoesGeradorObjeto.configuracoes == null)
					ConfiguracoesGeradorObjeto.configuracoes = new ConfiguracoesGeradorObjeto();

				return ConfiguracoesGeradorObjeto.configuracoes;
			}
		}

		/// <summary>
		/// Obtém uma coleção de <see cref="Objeto"/>.
		/// </summary>
		[ConfigurationProperty("objetos", IsRequired = true)]
		public ColecaoObjeto Objetos
		{
			get { return ((ColecaoObjeto)(base["objetos"])); }
			set { base["objetos"] = value; }
		}

		#endregion
	}
}
