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
    /// Classe BooleanField
    /// </summary>
    [Serializable]
    [DataContract]
	public class BooleanField : Field
	{
        /// <summary>
        /// Construtor BooleanField
        /// </summary>
        public BooleanField()
        {
 
        }

        /// <summary>
        /// Construtor BooleanField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public BooleanField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
		}

        /// <summary>
        /// Create a object Boolean baseado no atributo value.
        /// </summary>
        /// <returns></returns>
        public override Object CreateObject()
		{
			return Boolean.Parse(this.value);
		}

        /// <summary>
        /// Valida se a string input pode ser convertida para o tipo Boolean, e altera o valor do atributo value
        /// </summary>
        /// <param name="input">String com um BooleanString</param>
        public override void ValidateAndCanonicalize(String input)
		{
			Boolean flag;
            if (Boolean.TryParse(input, out flag))
			{
				this.value = input;
				return;
			}
			this.value = null;
		}
	}
}
