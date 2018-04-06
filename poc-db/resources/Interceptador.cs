using System;
using System.Configuration;

namespace Rede.PN.AtendimentoDigital.Core.Config.GeradorObjeto
{
	/// <summary>
	/// Representa o elemento de Interceptação associado a um objeto.
	/// </summary>
	public class Interceptador : ConfigurationElement
	{
		#region Propriedades

		/// <summary>
		/// Obtém o nome da expressão.
		/// </summary>
		[ConfigurationProperty("expressoes", IsKey = true, IsRequired = true)]
		public String Expressoes
		{
			get { return base["expressoes"].ToString(); }
			set { base["expressoes"] = value; }
		}

		/// <summary>
		/// Obtém o interceptador do objeto.
		/// </summary>
		[ConfigurationProperty("interceptador", IsRequired = true)]
		public String TipoInterceptador
		{
			get { return base["interceptador"].ToString(); }
			set { base["interceptador"] = value; }
		}

		/// <summary>
		/// Obtém uma coleção de <see cref="Propriedade"/>.
		/// </summary>
		[ConfigurationProperty("propriedades", IsRequired = false)]
		public ColecaoPropriedade Propriedades
		{
			get { return (ColecaoPropriedade)base["propriedades"]; }
			set { base["propriedades"] = value; }
		}
		#endregion
	}
}
