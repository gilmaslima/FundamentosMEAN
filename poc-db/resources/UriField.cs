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
    /// Classe UriField
    /// </summary>
    [Serializable]
    [DataContract]
    public class UriField : Field
	{
        /// <summary>
        /// Construtor UriField
        /// </summary>
        public UriField()
        { 
        }

        /// <summary>
        /// Construtor UriField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public UriField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
		}

        /// <summary>
        /// Cria um novo objeto do tipo URI basedo no atributo value.
        /// </summary>
        /// <returns>Novo objeto URI.</returns>
        public override Object CreateObject()
		{
			if (String.Compare(this.value, "(null)") == 0)
			{
				return null;
			}
			return new Uri(this.value);
		}

        /// <summary>
        /// Valida se o input pode ser convertido em um objeto URI.
        /// </summary>
        /// <param name="input">String para a validação.</param>
        public override void ValidateAndCanonicalize(String input)
		{
			if (String.Compare(input, "(null)") == 0)
			{
				base.ValidateAndCanonicalize(input);
				return;
			}

			Uri uri;
			if (Uri.TryCreate(input, UriKind.Absolute, out uri))
			{
				this.value = uri.ToString();
				return;
			}

			this.value = null;
		}
	}
}
