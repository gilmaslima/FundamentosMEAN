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
    /// Classe ArrayField
    /// </summary>
    [Serializable]
    [DataContract]
	public class ArrayField : ContainerField
	{
        /// <summary>
        /// Construtor ArrayField
        /// </summary>
        public ArrayField()
        { 

        }

        /// <summary>
        /// Construtor ArrayField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public ArrayField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
        }

        /// <summary>
        /// Cria um objeto array com todos os seus sub-tipos baseado no atributo value.
        /// </summary>
        /// <returns>Objeto ArrayField</returns>
        public override Object CreateObject()
		{
			if (String.IsNullOrWhiteSpace(this.value))
			{
				return null;
			}

			Type type = ClientSettings.GetType(this.CurrentMember.TypeName.Substring(0, this.CurrentMember.TypeName.Length - 2));
			Array array = Array.CreateInstance(type, int.Parse(this.value.Substring("length=".Length), CultureInfo.CurrentCulture));

			Int32 num = 0;
			base.GetChildFields();

			if (this.ChildFields != null)
			{
				Field[] childFields = this.ChildFields;
				for (Int32 i = 0; i < childFields.Length; i++)
				{
					Field field = childFields[i];
					array.SetValue(field.CreateObject(), num++);
				}
			}

			return array;
		}
	}
}
