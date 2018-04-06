using System;
using System.Data;

namespace Rede.PN.AtendimentoDigital.Core.EntLib.Dados.Generico
{
	/// <summary>
	/// Define um item de parâmetro a ser utilizado na execução de comandos de banco de dados.
	/// </summary>
	public class ParametroComando
	{
		public ParametroComando(DirecaoParametro direcao, String nome, DbType tipoDadoBanco, Object valor)
		{
			this.Direcao = direcao;
			this.Nome = nome;
			this.TipoDadoBanco = tipoDadoBanco;
			this.Valor = valor;
		}

		/// <summary>
		/// Direção do parâmetro.
		/// </summary>
		public DirecaoParametro Direcao { get; set; }

		/// <summary>
		/// Nome do parâmetro.
		/// </summary>
		public String Nome { get; set; }

		/// <summary>
		/// Tipo de dados no banco de dados.
		/// </summary>
		public DbType TipoDadoBanco { get; set; }

		/// <summary>
		/// O valor a ser definido para o parâmetro (necessário apenas para parâmetros de entrada).
		/// </summary>
		public Object Valor { get; set; }

		/// <summary>
		/// O tamanho máximo dos dados (necessário apenas para parâmetros de saída).
		/// </summary>
		public Int32 Tamanho { get; set; }

	}
}
