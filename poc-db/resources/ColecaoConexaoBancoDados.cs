using System;
using System.Configuration;

namespace Rede.PN.AtendimentoDigital.Core.Config.Dados
{
	/// <summary>
	/// Contém uma coleção de <see cref="ConexaoBancoDados"/>.
	/// </summary>
	[ConfigurationCollection(typeof(ConexaoBancoDados))]
	public sealed class ColecaoConexaoBancoDados : ConfigurationElementCollection
	{
		#region Propriedades

		/// <summary>
		/// Obtém o elemento de conexão especificado.
		/// </summary>
		/// <param name="idx">O índice do elemento a ser acessado.</param>
		/// <returns>O elemento especificado pelo índice.</returns>
		public ConexaoBancoDados this[Int32 idx]
		{
			get
			{
				return (ConexaoBancoDados)BaseGet(idx);
			}
		}

		/// <summary>
		/// Obtém o elemento de conexão especificado.
		/// </summary>
		/// <param name="nome">O nome do elemento a ser acessado.</param>
		/// <returns>O elemento especificado pelo nome.</returns>
		public ConexaoBancoDados this[String nome]
		{
			get
			{
				return (ConexaoBancoDados)BaseGet(nome);
			}
		}

		#endregion

		#region Métodos

		/// <summary>
		/// Obtém um valor informando se a coleção é somente leitura.
		/// </summary>
		/// <returns>true se a coleção é somente leitura.</returns>
		public override bool IsReadOnly()
		{
			return false;
		}

		/// <summary>
		/// Cria um novo elemento de configuração.
		/// </summary>
		/// <returns>O novo elemento criado.</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new ConexaoBancoDados();
		}

		/// <summary>
		/// Obtém a chave do elemento de configuração especificado.
		/// </summary>
		/// <param name="elemento">O elemento de configuração.</param>
		/// <returns>Um objeto que representa a chave do item.</returns>
		protected override Object GetElementKey(ConfigurationElement elemento)
		{
			return ((ConexaoBancoDados)(elemento)).Nome;
		}

		#endregion
	}
}
