using System;
using System.Data;

namespace Rede.PN.AtendimentoDigital.Core.EntLib.Dados.Generico
{
	/// <summary>
	/// Define um item de parâmetro a ser utilizado na execução de comandos de banco de dados.
	/// </summary>
	public sealed class ParametroComandoEntrada : ParametroComando
	{
		/// <summary>
		/// Inicializa uma instância.
		/// </summary>
		/// <param name="nome">O nome do parâmetro.</param>
		/// <param name="tipoDadoBanco">O tipo de dados do parâmetro.</param>
		/// <param name="valor">O valor do parâmetro.</param>
		public ParametroComandoEntrada(String nome, DbType tipoDadoBanco, Object valor)
			: base(DirecaoParametro.Entrada, nome, tipoDadoBanco, valor)
		{ }
	}
}
