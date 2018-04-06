/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Text;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe StringFormatter
    /// </summary>
	public class StringFormatter
	{
        /// <summary>
        /// FromEscapeCode
        /// </summary>
        /// <param name="input">String para inclusão dos escapes.</param>
        /// <returns></returns>
        public static String FromEscapeCode(String input)
		{
			Char[] array = input.ToCharArray();
			StringBuilder stringBuilder = new StringBuilder();
			Int32 i = 0;
			Int32 num = 0;
			while (i < array.Length)
			{
                Char c = array[i];
				if (num == 0)
				{
					if (Char.Equals(array[i],'\\'))
					{
						num = 1;
					}
					else
					{
						num = 0;
						stringBuilder.Append(c);
					}
				}
				else
				{
					if (num != 1)
					{
						return null;
					}
					if (Char.Equals(c, 'r'))
					{
						num = 0;
						stringBuilder.Append('\r');
					}
					else
					{
						if (Char.Equals(c, 'n'))
						{
							num = 0;
							stringBuilder.Append('\n');
						}
						else
						{
							if (Char.Equals(c, 't'))
							{
								num = 0;
								stringBuilder.Append('\t');
							}
							else
							{
								if (!Char.Equals(c, '\\'))
								{
									return null;
								}
								num = 0;
								stringBuilder.Append('\\');
							}
						}
					}
				}
				i++;
			}
			if (num == 0)
			{
				return stringBuilder.ToString();
			}
			return null;
		}

        /// <summary>
        /// ToEscapeCode
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
		public static String ToEscapeCode(String input)
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.CurrentCulture);
			CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
			cSharpCodeProvider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), stringWriter, new CodeGeneratorOptions());
			return stringWriter.ToString();
		}

	}
}
