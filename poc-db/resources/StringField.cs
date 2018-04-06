/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe StringField
    /// </summary>
    [Serializable]
    [DataContract]
	public class StringField : Field
	{
        /// <summary>
        /// Construtor StringField
        /// </summary>
        public StringField()
        { 
        }

        /// <summary>
        /// Construtor StringField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public StringField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
		}

        /// <summary>
        /// Cria um novo objeto do tipo String.
        /// </summary>
        /// <returns>Objeto do tipo String.</returns>
        public override Object CreateObject()
		{
			if (String.Compare(this.value, "(null)") == 0)
			{
				return null;
			}
			return StringFormatter.FromEscapeCode(this.value);
		}

        /// <summary>
        /// Valida se o input é está nula e seta o atributo value.
        /// </summary>
        /// <param name="input">String com o valor para validação.</param>
        public override void ValidateAndCanonicalize(String input)
		{
			base.ValidateAndCanonicalize(input);
		}
	}
}
