using Rede.PN.AtendimentoDigital.Core.Padroes.Repositorio;
using System;
using System.Collections.Generic;

namespace Rede.PN.AtendimentoDigital.Core.EntLib.Dados.Generico
{
	public sealed class EspecificacaoComando<TEntidade, TChave>
	where TEntidade : class, IEntidade<TChave>, new()
	{
		/// <summary>
		/// Define o modo a ser utilizado na execução do comando.
		/// </summary>
		public ModoExecucao ModoExecucao { get; set; }

		/// <summary>
		/// Define o tipo de comando a ser executado.
		/// </summary>
		public TipoComando TipoComando { get; set; }

		/// <summary>
		/// Define o comando.
		/// Utilize esta propriedade para informar a stored procedure ou consulta SQL.
		/// </summary>
		public String Comando { get; set; }

		/// <summary>
		/// Define a lista de parâmetros a serem uilizados na execução do comando.
		/// </summary>
		public Func<TEntidade, List<ParametroComando>> DefinirParametrosComando { get; set; }

		/// <summary>
		/// Define a ação a ser executada após a execução do comando.
		/// </summary>
		public Func<Object, Object> TratarRetornoComando { get; set; }

		/// <summary>
		/// Define a ação a ser executada para tratamento dos parâmetros carregados após a execução.
		/// </summary>
		public Action<IEnumerable<ParametroComando>> TratarParametrosComando { get; set; }
	}
}
