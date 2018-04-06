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
    /// Classe NullableField
    /// </summary>
    [Serializable]
    [DataContract]
    public class NullableField : Field
	{

        /// <summary>
        /// Construtor NullableField
        /// </summary>
        public NullableField()
        { 
        }

        /// <summary>
        /// Construtor NullableField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public NullableField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
		}

        /// <summary>
        /// Cria um objeto do tipo Nullable baseado no atributo value.
        /// </summary>
        /// <returns>Novo objeto Nullable.</returns>
        public override Object CreateObject()
		{
			if (String.Compare(this.value, "(null)") == 0)
			{
				return null;
			}
			base.GetChildFields();
			return this.ChildFields[0].CreateObject();
		}
	}
}
