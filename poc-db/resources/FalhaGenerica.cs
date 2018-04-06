using System;
using System.Runtime.Serialization;

namespace Rede.PN.AtendimentoDigital.Servicos.Excecao
{
	/// <summary>
	/// Define a falha padrão.
	/// </summary>
	[DataContract]
	public class FalhaGenerica
	{
		/// <summary>
		/// Inicializa uma instância da classe de falha.
		/// </summary>
		/// <param name="codigo">O código da falha.</param>
		/// <param name="fonte">A fonte da falha.</param>
		public FalhaGenerica(Int32 codigo, String fonte)
		{
			this.Codigo = codigo;
			this.Fonte = fonte;
		}

		/// <summary>
		/// Define o código da falha.
		/// </summary>
		[DataMember]
		public Int32 Codigo { get; set; }

		/// <summary>
		/// Define a fonte da falha.
		/// </summary>
		[DataMember]
		public String Fonte { get; set; }
	}
}
