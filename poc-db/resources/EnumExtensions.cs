using System;
using System.ComponentModel;
using System.Reflection;

namespace Redecard.PN.RAV
{
	/// <summary>
	/// Classe genérica contendo extensões para o tipo Enum.
	/// </summary>
	internal static class EnumExtensions
	{
		/// <summary>
		/// Obtém o valor da tag de descrição do enumerador.
		/// </summary>
		/// <param name="value">Valor do enumerador.</param>
		/// <returns>Descrição do enumerador.</returns>
		public static string GetDescription(this Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());
			DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
			if (attributes != null && attributes.Length > 0)
				return attributes[0].Description;
			else
				return value.ToString();
		}
	}
}
