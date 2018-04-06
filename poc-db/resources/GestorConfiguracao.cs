using Rede.PN.AtendimentoDigital.Core.Padroes.Singleton;
using System;
using System.Configuration;

namespace Rede.PN.AtendimentoDigital.Core.Config
{
	/// <summary>
	/// Classe que gerencia o serviçode de configuração.
	/// </summary>
	public sealed class GestorConfiguracao : Singleton<GestorConfiguracao>
	{
		#region Metodos Estaticos

		/// <summary>
		/// Obtém a System.Configuration.ConnectionStringSettings da aplicação.
		/// </summary>
		/// <param name="nome">Nome da string de conexao</param>
		/// <returns>Retorna o objeto System.Configuration.ConnectionStringSettings que contém o conteúdo de 
		///  configuração da connection string.</returns>
		internal static ConnectionStringSettings ObterStringConexao(String nome)
		{
			return GestorConfiguracao.Instancia.ObterStringConexaoInterno(nome);
		}

		/// <summary>
		/// Recupera uma seção de configuração especificada da aplicação.
		/// </summary>
		/// <param name="nomeSecao">Caminho e nome da seção de configuração.</param>
		/// <returns>Retorna um objeto System.Configuration.ConfigurationSection especificado ou null se a seção não existir.</returns>
		public static T ObterSecaoConfiguracao<T>(String nomeSecao)
		{
			return GestorConfiguracao.Instancia.ObterSecaoConfiguracaoInterno<T>(nomeSecao);
		}

		/// <summary>
		/// Recupera o valor referente a chave passada por parâmetro.
		/// </summary>
		/// <param name="chave">Chave do ítem do AppSettings.</param>
		/// <returns>Retorna o conteúdo do valor referente a chave passada por parâmetro.</returns>
		public static String ObterValorAppSettings(String chave)
		{
			return GestorConfiguracao.Instancia.ObterValorAppSettingsInterno(chave);
		}

		/// <summary>
		/// Método responsável por criptografar uma seção do arquivo de configuração.
		/// </summary>
		/// <param name="config">Arquivo de configuração.</param>
		/// <param name="nomeSecao">Nome da Secao a ser criptografada.</param>
		/// <param name="provider">Nome do provider no arquivo de configuração utilizado para criptografar.</param>
		/// <returns>Retorna se a secao foi criptografada ou não.</returns>
		public static Boolean CriptografarSecao(Configuration config, String nomeSecao, String provider)
		{
			return GestorConfiguracao.Instancia.CriptografarSecaoInterno(config, nomeSecao, provider);
		}

		/// <summary>
		/// Método responsável por descriptografar uma seção do arquivo de configuração
		/// </summary>
		/// <param name="config">Arquivo de configuração.</param>
		/// <param name="nomeSecao">Nome da Secao a ser criptografada.</param>
		/// <returns>Retorna se a secao foi descriptografada ou não.</returns>
		public static Boolean DescriptografarSecao(Configuration config, String nomeSecao)
		{
			return GestorConfiguracao.Instancia.DescriptografarSecaoInterno(config, nomeSecao);
		}

		#endregion

		#region Gestor Configuracao

		/// <summary>
		/// Define o serviço de configuração que será utilizado pelo gestor de configuração.
		/// </summary>
		private IServicoConfiguracao configuracao;

		/// <summary>
		/// Inicializa uma instância do componente de gestão de configurações.
		/// </summary>
		private GestorConfiguracao()
		{
			this.configuracao = Core.LocalizadorServico.ObterServico<IServicoConfiguracao>();
		}

		/// <summary>
		/// Obtém a System.Configuration.ConnectionStringSettings da aplicação.
		/// </summary>
		/// <param name="nome">Nome da string de conexao</param>
		/// <returns>Retorna o objeto System.Configuration.ConnectionStringSettings que contém o conteúdo de 
		///  configuração da connection string.</returns>
		private ConnectionStringSettings ObterStringConexaoInterno(String nome)
		{
			return configuracao.ObterStringConexaoBanco(nome);
		}

		/// <summary>
		/// Recupera uma seção de configuração especificada da aplicação.
		/// </summary>
		/// <param name="nomeSecao">Caminho e nome da seção de configuração.</param>
		/// <returns>Retorna um objeto System.Configuration.ConfigurationSection especificado ou null se a seção não existir.</returns>
		private T ObterSecaoConfiguracaoInterno<T>(String nomeSecao)
		{
			return this.configuracao.ObterSecaoConfiguracao<T>(nomeSecao);
		}

		/// <summary>
		/// Recupera o valor referente a chave passada por parâmetro.
		/// </summary>
		/// <param name="chave">Chave do ítem do AppSettings.</param>
		/// <returns>Retorna o conteúdo do valor referente a chave passada por parâmetro.</returns>
		private String ObterValorAppSettingsInterno(String chave)
		{
			return this.configuracao.ObterValorAppSettings(chave);
		}

		/// <summary>
		/// Método responsável por criptografar uma seção do arquivo de configuração.
		/// </summary>
		/// <param name="config">Arquivo de configuração.</param>
		/// <param name="nomeSecao">Nome da Secao a ser criptografada.</param>
		/// <param name="provider">Nome do provider no arquivo de configuração utilizado para criptografar.</param>
		/// <returns>Retorna se a secao foi criptografada ou não.</returns>
		private Boolean CriptografarSecaoInterno(Configuration config, String nomeSecao, String provider)
		{
			Boolean criptografado = false;
			if (config != null)
			{
				ConfigurationSection secao = config.GetSection(nomeSecao);
				if (secao != null && !secao.SectionInformation.IsProtected)
				{
					secao.SectionInformation.ForceSave = true;
					secao.SectionInformation.ProtectSection(provider);
					config.Save(ConfigurationSaveMode.Modified);

					criptografado = true;
				}
			}
			return criptografado;
		}

		/// <summary>
		/// Método responsável por descriptografar uma seção do arquivo de configuração
		/// </summary>
		/// <param name="config">Arquivo de configuração.</param>
		/// <param name="nomeSecao">Nome da Secao a ser criptografada.</param>
		/// <returns>Retorna se a secao foi descriptografada ou não.</returns>
		private Boolean DescriptografarSecaoInterno(Configuration config, String nomeSecao)
		{
			Boolean descriptografado = false;
			if (config != null)
			{
				ConfigurationSection secao = config.GetSection(nomeSecao);
				if (secao != null && secao.SectionInformation.IsProtected)
				{
					secao.SectionInformation.UnprotectSection();
					config.Save();

					descriptografado = true;
				}
			}
			return descriptografado;
		}

		#endregion
	}
}
