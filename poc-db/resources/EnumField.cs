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
    /// Classe EnumField
    /// </summary>
    [Serializable]
    [DataContract]
    public class EnumField : Field
	{
        /// <summary>
        /// Construtor EnumField
        /// </summary>
        public EnumField()
        { 
        }

        /// <summary>
        /// Construtor EnumField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public EnumField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
		}

        /// <summary>
        /// Cria um objeto do tipo Enum baseado no atributo value.
        /// </summary>
        /// <returns>Uma instância de Enum.</returns>
		public override Object CreateObject()
		{
			return Enum.Parse(ClientSettings.GetType(this.CurrentMember.TypeName), this.value);
		}
	}
}
