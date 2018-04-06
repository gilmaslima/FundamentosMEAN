using Rede.PN.AtendimentoDigital.Core.Config;
using Rede.PN.AtendimentoDigital.Core.Config.Dados;
using Rede.PN.AtendimentoDigital.Core.EntLib.Dados;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Rede.PN.AtendimentoDigital.Core.EntLib.Config
{
	/// <summary>
	/// Classe concreta que implementa a interface <see cref="IServicoConfiguracao"/> para tratar as configurações utilizando o EnterpriseLibrary.
	/// </summary>
	public sealed class ServicoConfiguracaoEnterpriseLibrary : IServicoConfiguracao
	{
		/// <summary>
		/// Instância do objeto de configuração
		/// </summary>
		private IConfigurationSource configuracao;

		/// <summary>
		/// Construtor responsável por criar as fontes de configuração
		/// </summary>
		public ServicoConfiguracaoEnterpriseLibrary()
		{
			configuracao = ConfigurationSourceFactory.Create();
		}

		#region Métodos

		/// <summary>
		/// Obtém a System.Configuration.ConnectionStringSettings da aplicação.
		/// </summary>
		/// <param name="nome">Nome da string de conexao</param>
		/// <returns>Retorna o objeto System.Configuration.ConnectionStringSettings que contém o conteúdo de 
		///  configuração da connection string.</returns>
		public ConnectionStringSettings ObterStringConexaoBanco(String nome)
		{
			ConexaoBancoDados conexao = ConfiguracoesConexoesBancoDados.Configuracoes.Conexoes[nome];
			if (conexao != null)
			{
				// Inicializando as variáveis locais
				String parametros = conexao.Parametros;
				Regex regEx;
				Match ocorrencia;

				// Validando a existência do parâmetro Persist Security Info = false
				// que garante que as credenciais da conexão de banco não sejam exibidas
				regEx = new Regex("Persist Security Info=([^;]*)");
				ocorrencia = regEx.Match(parametros);
				if (!String.IsNullOrWhiteSpace(ocorrencia.Value))
					parametros = parametros.Replace(ocorrencia.Value, "Persist Security Info=false");
				else
					parametros = String.Format("{0}{1}{2};",
						parametros,
						!parametros.EndsWith(";") ? ";" : String.Empty,
						"Persist Security Info=false"
					);

				// Recuperando senha de banco utilizando o SyncP2
				if (!String.IsNullOrWhiteSpace(conexao.ArquivoSyncP2))
				{
					IServicoSyncPass syncPass = GeradorObjeto.Obter<IServicoSyncPass>();
					String senha = syncPass.ObterSenha(conexao.ArquivoSyncP2);
					if (!String.IsNullOrWhiteSpace(senha))
					{
						regEx = new Regex("Password=([^;]*)");
						ocorrencia = regEx.Match(parametros);
						if (!String.IsNullOrWhiteSpace(ocorrencia.Value))
							parametros = parametros.Replace(
								ocorrencia.Value,
								String.Format("Password={0}", senha)
							);
						else
							parametros = String.Format("{0}{1}{2};",
								parametros,
								!parametros.EndsWith(";") ? ";" : String.Empty,
								String.Format("Password={0}", senha)
							);
					}
				}

				// Criando o objeto de configuração da conection string
				return new ConnectionStringSettings(
					conexao.Nome,
					parametros,
					conexao.NomeProvider
				);
			}

			return null;
		}

		/// <summary>
		/// Recupera uma seção de configuração especificada da aplicação.
		/// </summary>
		/// <param name="nomeSecao">Caminho e nome da seção de configuração.</param>
		/// <returns>Retorna um objeto System.Configuration.ConfigurationSection especificado ou null se a seção não existir.</returns>
		public T ObterSecaoConfiguracao<T>(String nomeSecao)
		{
			Object secao = ConfigurationManager.GetSection(nomeSecao);
			if (secao == null)
				secao = configuracao.GetSection(nomeSecao);

			return (T)Convert.ChangeType(secao, typeof(T));
		}

		/// <summary>
		/// Recupera o valor referente a chave passada por parâmetro.
		/// </summary>
		/// <param name="chave">Chave do ítem do AppSettings.</param>
		/// <returns>Retorna o conteúdo do valor referente a chave passada por parâmetro.</returns>
		public String ObterValorAppSettings(String chave)
		{
			String valor = ConfigurationManager.AppSettings[chave];

			if (String.IsNullOrWhiteSpace(valor))
			{
				AppSettingsSection app = configuracao.GetSection("appSettings") as AppSettingsSection;
				if (app != null && app.Settings[chave] != null)
					valor = app.Settings[chave].Value;
			}

			return valor;
		}

		#endregion
	}
}
