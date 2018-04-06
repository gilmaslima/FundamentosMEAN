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
    /// Classe GuidField
    /// </summary>
    [Serializable]
    [DataContract]
    public class GuidField : Field
	{
        /// <summary>
        /// Construtor GuidField
        /// </summary>
        public GuidField()
        { 
        }

        /// <summary>
        /// Construtor GuidField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public GuidField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
		}

        /// <summary>
        /// Cria um novo objeto do tipo Guid baseado no atributo value.
        /// </summary>
        /// <returns>Objeto do tipo Guid.</returns>
        public override Object CreateObject()
		{
			return new Guid(this.value);
		}

        /// <summary>
        /// Valida se o valor "input" pode ser convertido para um GUID.
        /// </summary>
        /// <param name="input">Valor para o teste.</param>
        public override void ValidateAndCanonicalize(String input)
        {
            this.value = new Guid(input).ToString();
            base.ValidateAndCanonicalize(this.value);
        }
	}
}
