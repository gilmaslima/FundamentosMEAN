/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe DateTimeField
    /// </summary>
    [Serializable]
    [DataContract]
    public class DateTimeField : Field
	{

        /// <summary>
        /// Construtor DateTimeField
        /// </summary>
        public DateTimeField()
        { 
        }

        /// <summary>
        /// Construtor DateTimeField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public DateTimeField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
		}

        /// <summary>
        /// Cria um objeto DataTime baseado no atributo value.
        /// </summary>
        /// <returns>Instância de um objeto DataTime.</returns>
        public override Object CreateObject()
		{
			return new DateTimeConverter().ConvertFrom(this.value);
		}

        /// <summary>
        /// Valida se o valor de input pode ser convertido para um DataTime válido.
        /// </summary>
        /// <param name="input">String com o valor para validação.</param>
        public override void ValidateAndCanonicalize(String input)
        {
            this.value = new DateTimeConverter().ConvertFrom(input).ToString();
        }
	}
}
