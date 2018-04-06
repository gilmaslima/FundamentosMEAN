using System;
using System.Configuration;

namespace Rede.PN.AtendimentoDigital.Core.Config.Dados
{
	/// <summary>
	/// Representa o elemento de configuração associado a uma conexão.
	/// </summary>
	public sealed class ConexaoBancoDados : ConfigurationElement
	{
		#region Propriedades

		/// <summary>
		/// Obtém o nome da conexão.
		/// </summary>
		[ConfigurationProperty("nome", IsKey = true, IsRequired = true)]
		public String Nome
		{
			get { return base["nome"].ToString(); }
			set { base["nome"] = value; }
		}

		/// <summary>
		/// Obtém os parâmetros da conexão.
		/// </summary>
		[ConfigurationProperty("parametros")]
		public String Parametros
		{
			get { return base["parametros"].ToString(); }
			set { base["parametros"] = value; }
		}

		/// <summary>
		/// Obtém o nome do provider.
		/// </summary>
		[ConfigurationProperty("nomeProvider")]
		public String NomeProvider
		{
			get { return base["nomeProvider"].ToString(); }
			set { base["nomeProvider"] = value; }
		}

		/// <summary>
		/// Obtém o nome do arquivo do SyncP2.
		/// </summary>
		[ConfigurationProperty("arquivoSyncP2")]
		public String ArquivoSyncP2
		{
			get { return base["arquivoSyncP2"].ToString(); }
			set { base["arquivoSyncP2"] = value; }
		}

		#endregion
	}
}
