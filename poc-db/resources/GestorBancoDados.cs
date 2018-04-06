using Rede.PN.AtendimentoDigital.Core.Config;
using Rede.PN.AtendimentoDigital.Core.Config.Dados;
using Rede.PN.AtendimentoDigital.Core.Config.GeradorObjeto;
using Rede.PN.AtendimentoDigital.Core.Padroes.Singleton;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;

namespace Rede.PN.AtendimentoDigital.Core.EntLib.Dados
{
	/// <summary>
	/// Esta classe provê um gerador de objetos para conexão com banco de dados.
	/// </summary>
	public sealed class GestorBancoDados : Singleton<GestorBancoDados>
	{
		#region Metodos Estaticos

		/// <summary>
		/// Recupera um objeto de conexão com o banco de dados
		/// </summary>
		/// <param name="nomeConexao">O nome da string de conexão configurada em infraestrutura/conexoesBancoDados</param>
		public static Database ObterBancoDados(String nomeConexao)
		{
			return GestorBancoDados.ObterBancoDados(nomeConexao, null);
		}

		/// <summary>
		/// Recupera um objeto de conexão com o banco de dados
		/// </summary>
		/// <param name="nomeConexao">O nome da string de conexão configurada em infraestrutura/conexoesBancoDados</param>
		/// <param name="providerFactory">A instância da factory a ser utilizada.</param>
		public static Database ObterBancoDados(String nomeConexao, DbProviderFactory providerFactory)
		{
			return GestorBancoDados.Instancia.ObterBancoDadosInterno(nomeConexao, providerFactory);
		}

		/// <summary>
		/// Recupera um objeto de conexão com o banco de dados
		/// </summary>
		/// <param name="tipoRepositorio">O tipo do repositório.</param>
		public static Database ObterBancoDados(Type tipoRepositorio)
		{
			return GestorBancoDados.ObterBancoDados(tipoRepositorio, null);
		}

		/// <summary>
		/// Recupera um objeto de conexão com o banco de dados
		/// </summary>
		/// <param name="tipoRepositorio">O tipo do repositório.</param>
		/// <param name="providerFactory">A instância da factory a ser utilizada.</param>
		public static Database ObterBancoDados(Type tipoRepositorio, DbProviderFactory providerFactory)
		{
			return GestorBancoDados.Instancia.ObterBancoDadosInterno(
				GestorBancoDados.Instancia.ObterNomeConexaoRepositorioInterno(tipoRepositorio),
				providerFactory
			);
		}

		/// <summary>
		/// Recupera o nome da conexão configurado para o repositório
		/// </summary>
		/// <param name="tipoRepositorio">O tipo do repositório.</param>
		public static String ObterNomeConexaoRepositorio(Type tipoRepositorio)
		{
			return GestorBancoDados.Instancia.ObterNomeConexaoRepositorioInterno(tipoRepositorio);
		}

		#endregion

		#region Gerador Banco Dados

		/// <summary>
		/// Define a coleção de cache dos provedores de banco de dados utilizados pelo projeto.
		/// </summary>
		private Dictionary<String, DbProviderFactory> provedoresBancoDados;

		/// <summary>
		/// Inicializa uma instância do componente de gestão de banco de dados.
		/// </summary>
		private GestorBancoDados()
		{
			this.provedoresBancoDados = new Dictionary<string, DbProviderFactory>();
		}

		/// <summary>
		/// Recupera um objeto de conexão com o banco de dados
		/// </summary>
		/// <param name="nomeConexao">O nome da string de conexão configurada em infraestrutura/conexoesBancoDados</param>
		/// <param name="providerFactory">A instância da factory a ser utilizada.</param>
		private Database ObterBancoDadosInterno(String nomeConexao, DbProviderFactory providerFactory)
		{
			// Recuperando as configurações da string de conexão
			ConnectionStringSettings conexao = GestorConfiguracao.ObterStringConexao(nomeConexao);
			if (conexao != null)
			{
				// Recuperando o provider do banco de dados
				DbProviderFactory provedor;
				if (this.provedoresBancoDados.ContainsKey(conexao.ProviderName))
					provedor = this.provedoresBancoDados[conexao.ProviderName];
				else
				{
					provedor = providerFactory ?? DbProviderFactories.GetFactory(conexao.ProviderName);
					this.provedoresBancoDados.Add(conexao.ProviderName, provedor);
				}

				// Criando o objeto do banco de dados
				return new GenericDatabase(conexao.ConnectionString, provedor);
			}

			return null;
		}

		/// <summary>
		/// Recupera o nome da conexão configurado para o repositório
		/// </summary>
		/// <typeparam name="TRepositorio">O tipo do repositório.</typeparam>
		private String ObterNomeConexaoRepositorioInterno(Type tipoRepositorio)
		{
			// Recuperando a conexão padrão
			String nomeConexao = ConfiguracoesDados.Configuracoes.NomeConexaoPadrao;

			// Verificando se foi definida uma conexão específica para o repositório
			Objeto objeto = ConfiguracoesGeradorObjeto.Configuracoes.Objetos.Cast<Objeto>().FirstOrDefault(f => Type.GetType(f.Interface).IsAssignableFrom(tipoRepositorio));
			if (objeto != null)
			{
				Repositorio repositorio = ConfiguracoesDados.Configuracoes.Repositorios.Cast<Repositorio>().FirstOrDefault(f => f.Nome.Equals(objeto.Nome, StringComparison.OrdinalIgnoreCase));
				if (repositorio != null
					&& !String.IsNullOrWhiteSpace(repositorio.NomeConexao))
				{
					nomeConexao = repositorio.NomeConexao;
				}
			}

			return nomeConexao;
		}

		#endregion
	}
}
