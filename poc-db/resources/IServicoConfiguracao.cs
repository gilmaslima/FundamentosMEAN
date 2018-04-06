using Rede.PN.AtendimentoDigital.Core.Padroes.ServiceLocator;
using System;
using System.Configuration;

namespace Rede.PN.AtendimentoDigital.Core.Config
{
	/// <summary>
	/// Interface generica de configuração.
	/// Esta interface é utilizada para ser implementada pelos serviços de configuração.
	/// </summary>
	public interface IServicoConfiguracao : IServico
	{

		#region Métodos

		/// <summary>
		/// Obtém a System.Configuration.ConnectionStringSettings da aplicação.
		/// </summary>
		/// <param name="nome">Nome da string de conexao</param>
		/// <returns>Retorna o objeto System.Configuration.ConnectionStringSettings que contém o conteúdo de 
		///  configuração da connection string.</returns>
		ConnectionStringSettings ObterStringConexaoBanco(String nome);

		/// <summary>
		/// Recupera uma seção de configuração especificada da aplicação.
		/// </summary>
		/// <param name="nomeSecao">Caminho e nome da seção de configuração.</param>
		/// <returns>Retorna um objeto System.Configuration.ConfigurationSection especificado ou null se a seção não existir.</returns>
		T ObterSecaoConfiguracao<T>(String nomeSecao);

		/// <summary>
		/// Recupera o valor referente a chave passada por parâmetro.
		/// </summary>
		/// <param name="chave">Chave do ítem do AppSettings.</param>
		/// <returns>Retorna o conteúdo do valor referente a chave passada por parâmetro.</returns>
		String ObterValorAppSettings(String chave);

		#endregion
	}
}
