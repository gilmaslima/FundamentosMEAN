/*
© Copyright 2017 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Software e Consultoria.
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.ContratoDados
{
	/// <summary>
	/// Define os valores de response padrão.
	/// </summary>
	[DataContract]
	public class ResponseBase
	{
		/// <summary>
		/// Define o status de retorno.
		/// </summary>
        [DataMember(Name = "statusRetorno")]
		public StatusRetorno StatusRetorno { get; set; }

		/// <summary>
		/// Define a mensagem de retorno.
		/// </summary>
        [DataMember(Name = "mensagem")]
		public String Mensagem { get; set; }
	}
}