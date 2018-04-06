using System;

namespace Redecard.PN.RAV
{
	/// <summary>
	/// Classe genérica contendo extensões para o tipo String.
	/// </summary>
	internal static class StringExtensions
	{
		#region [ Int32 ]

		/// <summary>
		/// Converte String para Int32.
		/// </summary>
		/// <param name="target">O valor a ser convertido.</param>
		/// <param name="defaultValue">Número padrão de retorno (se não for número).</param>
		/// <remarks>
		/// Considera a validação IsEmptyText para definir se o valor é considerado como vazio ou não.
		/// </remarks>
		internal static int? ToInt32Null(this String target, int? defaultValue)
		{
			// Retorna o valor default caso seja um valor vazio
			if (target.IsEmptyText())
				return defaultValue;

			// Realizando a conversão
			int returnValue;
			if (int.TryParse(target.ToString(), out returnValue))
				return (int?)returnValue;
			else
				return defaultValue;
		}

		#endregion

		#region [ PRIVADO ]

		/// <summary>
		/// Verifica se é um conteúdo para considerar como vazio.
		/// </summary>
		/// <param name="target">O texto a ser verificado.</param>
		/// <returns>True se o texto deve ser considerado como vazio.</returns>
		/// <remarks>
		/// Além dos valores Nulo e Vazio, são considerados os valores "-- Selecione --", "-- Todas --" e "-- Todos --".
		/// </remarks>
		private static bool IsEmptyText(this String target)
		{
			return (string.IsNullOrWhiteSpace(target) || target == "-- Selecione --" || target == "-- Todas --" || target == "-- Todos --");
		}

		#endregion
	}
}
