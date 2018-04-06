using System;
using System.Configuration;

namespace Rede.PN.AtendimentoDigital.Core.Config.Dados
{
	/// <summary>
	/// Representa o elemento de configuração de dados associado a um repositorio.
	/// </summary>
	public class Repositorio : ConfigurationElement
	{
		#region Propriedades

		/// <summary>
		/// Obtém o nome do repositório.
		/// </summary>
		[ConfigurationProperty("nome", IsKey = true, IsRequired = true)]
		public String Nome
		{
			get { return base["nome"].ToString(); }
			set { base["nome"] = value; }
		}

		/// <summary>
		/// Obtém o nome da conexão do repositório.
		/// </summary>
		[ConfigurationProperty("nomeConexao", IsRequired = false)]
		public String NomeConexao
		{
			get { return base["nomeConexao"].ToString(); }
			set { base["nomeConexao"] = value; }
		}

		#endregion
	}
}
