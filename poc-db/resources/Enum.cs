using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rede.PN.AtendimentoDigital.Core.EntLib.Dados.Generico
{
	/// <summary>
	/// Define o modo de execução.
	/// </summary>
	public enum ModoExecucao
	{
		/// <summary>
		/// Executa o comando e retorna os dados em um <see cref="System.Data.DataSet"/>.
		/// </summary>
		DataSet = 1,

		/// <summary>
		/// Executa o comando e retorna o número de linhas afetadas.
		/// </summary>
		NonQuery = 2,

		/// <summary>
		/// Executa o comando e retorna um <see cref="System.Data.IDataReader"/> pelo qual
		/// o resultado pode ser lido. O reader será fechado automaticamente após a 
		/// execução do evento pós comando.
		/// </summary>
		Reader = 3,

		/// <summary>
		/// Executa o comando e retorna a primeira coluna da primeira linha do resultado.
		/// Colunas extras são ignoradas.
		/// </summary>
		Scalar = 4,
	}

	/// <summary>
	/// Define o tipo de comando.
	/// </summary>
	public enum TipoComando
	{
		/// <summary>
		/// Cria um <see cref="System.Data.Common.DbCommand"/> para a execução de consulta SQL.
		/// </summary>
		Texto = 1,

		/// <summary>
		/// Cria um <see cref="System.Data.Common.DbCommand"/> para a execução de stored procedure.
		/// </summary>
		StoredProcedure = 4,
	}

	/// <summary>
	/// Define o tipo de um parâmetro.
	/// </summary>
	public enum DirecaoParametro
	{
		/// <summary>
		/// Cria parâmetros para a entrada de valores.
		/// </summary>
		Entrada = 1,

		/// <summary>
		/// Cria parâmetros para a saída de valores.
		/// </summary>
		Saida = 2,
	}
}
