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
    /// Classe CharField
    /// </summary>
    [Serializable]
    [DataContract]
	public class CharField : Field
	{
        /// <summary>
        /// Construtor CharField
        /// </summary>
        public CharField()
        { 
        }

        /// <summary>
        /// Construtor CharField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public CharField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
		}

        /// <summary>
        /// Cria um objeto Char baseado no atributo value.
        /// </summary>
        /// <returns></returns>
        public override Object CreateObject()
		{
			return this.value[0];
		}

        /// <summary>
        /// Valida se a string pode ser convertida para o tipo Char.
        /// </summary>
        /// <param name="input">String que será validada.</param>
        public override void ValidateAndCanonicalize(String input)
		{
			if (input.Length == 1)
			{
				this.value = input;
				return;
			}

			this.value = null;
		}
	}
}
