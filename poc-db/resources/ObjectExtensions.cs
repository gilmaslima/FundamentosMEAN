using System;

namespace Redecard.PN.RAV
{
	/// <summary>
	/// Classe genérica contendo extensões para o tipo Object.
	/// </summary>
	internal static class ObjectExtensions
	{
		#region [ Int32 ]

		/// <summary>
		/// Converte Object para Int32.
		/// </summary>
		/// <param name="target">O valor a ser convertido.</param>
		/// <param name="defaultValue">Número padrão de retorno (se não for número).</param>
		/// <remarks>
		/// Em caso de o valor ser uma <see cref="string"/> é chamada a extensão específica.
		/// </remarks>
		internal static int? ToInt32Null(this Object target, int? defaultValue)
		{
			if (target == null)
				return defaultValue;

			// Conversão customizadas para valores do tipo String
			if (target is String)
				return StringExtensions.ToInt32Null(target.ToString(), defaultValue);

			// Realizando a conversão
			int returnValue;
			if (int.TryParse(target.ToString(), out returnValue))
				return (int?)returnValue;
			else
				return defaultValue;
		}

		/// <summary>
		/// Converte Object para Int32.
		/// </summary>
		/// <param name="target">O valor a ser convertido.</param>
		/// <remarks>
		/// Em caso de o valor ser uma <see cref="string"/> é chamada a extensão específica.
		/// </remarks>
		public static int? ToInt32Null(this Object target)
		{
			return ObjectExtensions.ToInt32Null(target, null);
		}

		/// <summary>
		/// Converte Object para Int32.
		/// </summary>
		/// <param name="target">O valor a ser convertido.</param>
		/// <param name="defaultValue">Número padrão de retorno (se não for número).</param>
		/// <remarks>
		/// Em caso de o valor ser uma <see cref="string"/> é chamada a extensão específica.
		/// </remarks>
		public static int ToInt32(this Object target, int defaultValue)
		{
			int? returnValue = ObjectExtensions.ToInt32Null(target, defaultValue);
			return returnValue.HasValue ? returnValue.Value : defaultValue;
		}

		/// <summary>
		/// Converte Object para Int32.
		/// </summary>
		/// <param name="target">O valor a ser convertido.</param>
		/// <remarks>
		/// Em caso de o valor ser uma <see cref="string"/> é chamada a extensão específica.
		/// </remarks>
		public static int ToInt32(this Object target)
		{
			return ObjectExtensions.ToInt32(target, 0);
		}

		#endregion
	}
}
