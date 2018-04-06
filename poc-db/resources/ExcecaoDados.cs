using System;

namespace Rede.PN.AtendimentoDigital.Modelo.Excecao
{
	/// <summary>
	/// Define a exceção retornada quando regras de dados não são atendidas.
	/// </summary>
	public class ExcecaoDados : Exception
	{
		/// <summary>
		/// Recupera o código que descreve a exceção corrente.
		/// </summary>
		public Int32 Codigo { get; private set; }

		/// <summary>
		/// Inicializa uma instância da classe de exceção de dados.
		/// Define a mensagem como <see cref="String.Empty"/>.
		/// </summary>
		public ExcecaoDados() :
			this(String.Empty) { }

		/// <summary>
		/// Inicializa uma instância da classe de exceção de dados.
		/// </summary>
		/// <param name="mensagem">A mensagem da exceção.</param>
		public ExcecaoDados(String mensagem) :
			this(mensagem, new Object[] { }) { }

		/// <summary>
		/// Inicializa uma instância da classe de exceção de dados.
		/// </summary>
		/// <param name="formatoMensagem">O formato da mensagem, conforme utilizado em <see cref="String.Format"/>.</param>
		/// <param name="valores">Os valores para preenchimento do formato da mensagem, conforme utilizado em <see cref="String.Format"/>.</param>
		public ExcecaoDados(String formatoMensagem, params Object[] valores) :
			this(default(Int32), String.Format(formatoMensagem, valores)) { }

		/// <summary>
		/// Inicializa uma instância da classe de exceção de dados.
		/// </summary>
		/// <param name="codigo">Código da execeção.</param>
		/// <param name="mensagem">A mensagem da exceção.</param>
		public ExcecaoDados(Int32 codigo, String mensagem) :
			this(codigo, mensagem, new Object[] { }) { }

		/// <summary>
		/// Inicializa uma instância da classe de exceção de dados.
		/// </summary>
		/// <param name="codigo">Código da execeção.</param>
		/// <param name="formatoMensagem">O formato da mensagem, conforme utilizado em <see cref="String.Format"/>.</param>
		/// <param name="valores">Os valores para preenchimento do formato da mensagem, conforme utilizado em <see cref="String.Format"/>.</param>
		public ExcecaoDados(Int32 codigo, String formatoMensagem, params Object[] valores) :
			base(String.Format(formatoMensagem, valores))
		{
			this.Codigo = codigo;
		}
	}
}
