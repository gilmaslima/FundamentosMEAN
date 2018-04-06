using System;
using System.Data;

namespace Rede.PN.AtendimentoDigital.Core.EntLib.Dados.Generico
{
	/// <summary>
	/// Define um item de parâmetro de saída a ser utilizado na execução de comandos de banco de dados.
	/// </summary>
	public sealed class ParametroComandoSaida : ParametroComando
	{
		/// <summary>
		/// Inicializa uma instância.
		/// </summary>
		/// <param name="nome">O nome do parâmetro.</param>
		/// <param name="tipoDadoBanco">O tipo de dados do parâmetro.</param>
		/// <param name="tamanho">O tamanho máximo dos dados (necessário apenas para parâmetros de saída).</param>
		public ParametroComandoSaida(String nome, DbType tipoDadoBanco, Int32 tamanho)
			: base(DirecaoParametro.Saida, nome, tipoDadoBanco, DBNull.Value)
		{
			this.Tamanho = tamanho;
		}
	}
}
