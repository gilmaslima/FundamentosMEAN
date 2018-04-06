﻿using System;
using System.Configuration;

namespace Rede.PN.AtendimentoDigital.Core.Config.GeradorObjeto
{
	/// <summary>
	/// Contém uma coleção de <see cref="Interceptador"/>.
	/// </summary>
	[ConfigurationCollection(typeof(Interceptador))]
	public class ColecaoInterceptador : ConfigurationElementCollection
	{
		#region Propriedades

		/// <summary>
		/// Define o nome da propriedade.
		/// </summary>
		private String nomePropriedade = "interceptador";

		/// <summary>
		/// Obtém o tipo da <see cref="ColecaoInterceptador"/>.
		/// </summary>
		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.BasicMap;
			}
		}

		/// <summary>
		/// Obtém o nome utilizado para identificar os itens da coleção de interceptação.
		/// </summary>
		protected override String ElementName
		{
			get
			{
				return nomePropriedade;
			}
		}

		/// <summary>
		/// Obtém o elemento de interceptação especificado.
		/// </summary>
		/// <param name="idx">O índice do elemento a ser acessado.</param>
		/// <returns>O elemento especificado pelo índice.</returns>
		public Interceptador this[Int32 idx]
		{
			get
			{
				return (Interceptador)BaseGet(idx);
			}
		}

		#endregion

		#region Métodos

		/// <summary>
		/// Indica se o elemento especificado existe na coleção de Interceptadores.
		/// </summary>
		/// <param name="nomeElemento">Nome do elemento a ser verificado.</param>
		/// <returns>true se o elemento existir na coleção. Por padrão, retorna false.</returns>
		protected override Boolean IsElementName(String nomeElemento)
		{
			return nomeElemento.Equals(nomePropriedade, StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Obtém um valor informando se a coleção é somente leitura.
		/// </summary>
		/// <returns>true se a coleção é somente leitura.</returns>
		public override bool IsReadOnly()
		{
			return false;
		}

		/// <summary>
		/// Cria um novo elemento de Interceptação.
		/// </summary>
		/// <returns>O novo elemento criado.</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new Interceptador();
		}

		/// <summary>
		/// Obtém a chave do elemento de Interceptador especificado.
		/// </summary>
		/// <param name="elemento">O elemento de Interceptador.</param>
		/// <returns>Um objeto que representa a chave do item.</returns>
		protected override Object GetElementKey(ConfigurationElement elemento)
		{
			return ((Interceptador)(elemento)).Expressoes;
		}

		#endregion
	}
}
