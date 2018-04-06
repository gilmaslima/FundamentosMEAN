/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe DateTimeOffsetField
    /// </summary>
    [Serializable]
    [DataContract]
    public class DateTimeOffsetField : Field
	{
        /// <summary>
        /// Construtor  DateTimeOffsetField
        /// </summary>
        public DateTimeOffsetField()
        { 
        }

        /// <summary>
        /// Construtor  DateTimeOffsetField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public DateTimeOffsetField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
		}

        /// <summary>
        /// Cria um objeto do tipo DataTimeOffSet baseado no atributo value.
        /// </summary>
        /// <returns></returns>
        public override Object CreateObject()
		{
			return DateTimeOffset.Parse(this.value, CultureInfo.CurrentCulture);
		}

        /// <summary>
        /// Valida se o input pode ser convertido para o tipo DateTimeOffSet.
        /// </summary>
        /// <param name="input">String para a validação.</param>
        public override void ValidateAndCanonicalize(String input)
		{
			base.ValidateAndCanonicalize(input);
			if (this.value == null)
			{
				return;
			}

			DateTimeOffset dateTimeOffset;
			if (DateTimeOffset.TryParse(input, out dateTimeOffset))
			{
				this.value = dateTimeOffset.ToString();
				return;
			}

			this.value = null;
		}
	}
}
