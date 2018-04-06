using System;
using System.Configuration;

namespace Rede.PN.AtendimentoDigital.Core.Config.GeradorObjeto
{
	/// <summary>
	/// Representa o elemento de configuração associado a um objeto.
	/// </summary>
	public class Objeto : ConfigurationElement
	{
		#region Propriedades

		/// <summary>
		/// Obtém o nome do objeto.
		/// </summary>
		[ConfigurationProperty("nome", IsKey = true, IsRequired = true)]
		public String Nome
		{
			get { return base["nome"].ToString(); }
			set { base["nome"] = value; }
		}

		/// <summary>
		/// Obtém a interface de chamada do objeto.
		/// </summary>
		[ConfigurationProperty("interface", IsRequired = true)]
		public String Interface
		{
			get { return base["interface"].ToString(); }
			set { base["interface"] = value; }
		}

		/// <summary>
		/// Obtém a implementação concreta do objeto.
		/// </summary>
		[ConfigurationProperty("implementacao", IsRequired = true)]
		public String Implementacao
		{
			get { return base["implementacao"].ToString(); }
			set { base["implementacao"] = value; }
		}

		/// <summary>
		/// Obtém a flag se deverá ser criado como singleton.
		/// </summary>
		[ConfigurationProperty("singleton", DefaultValue = "False", IsRequired = false)]
		public Boolean Singleton
		{
			get { return Convert.ToBoolean(base["singleton"]); }
			set { base["singleton"] = value; }
		}

		/// <summary>
		/// Obtém a flag se a classe deverá ser interceptada.
		/// </summary>
		[ConfigurationProperty("interceptado", DefaultValue = "False", IsRequired = false)]
		public Boolean Interceptado
		{
			get { return (base["interceptado"] == null) ? false : Convert.ToBoolean(base["interceptado"]); }
			set { base["interceptado"] = value; }
		}

		/// <summary>
		/// Obtém uma coleção de <see cref="Interceptador"/>.
		/// </summary>
		[ConfigurationProperty("interceptadores", IsRequired = false)]
		public ColecaoInterceptador Interceptadores
		{
			get { return ((ColecaoInterceptador)(base["interceptadores"])); }
			set { base["interceptadores"] = value; }
		}

		/// <summary>
		/// Obtém uma coleção de <see cref="Propriedade"/>.
		/// </summary>
		[ConfigurationProperty("propriedades", IsRequired = false)]
		public ColecaoPropriedade Propriedades
		{
			get { return ((ColecaoPropriedade)(base["propriedades"])); }
			set { base["propriedades"] = value; }
		}
		#endregion
	}
}
