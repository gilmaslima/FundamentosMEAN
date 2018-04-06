/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe CompositeField
    /// </summary>
    [Serializable]
    [DataContract]
    public class CompositeField : Field
	{
        /// <summary>
        /// Construtor CompositeField
        /// </summary>
        public CompositeField()
        { 
        }

        /// <summary>
        /// Construtor CompositeField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public CompositeField(TypeMemberInfo declaredMember)
            : base(declaredMember)
		{
		}

        /// <summary>
        /// Cria um objeto do tipo definido pelo Declared Member baseado no atributo value.
        /// </summary>
        /// <returns>Instância do objeto definido pelo Declared Member.</returns>
        public override Object CreateObject()
		{
			if (String.Compare(this.value, "(null)") == 0)
			{
				return null;
			}

			base.GetChildFields();

			Type type = ClientSettings.GetType(this.CurrentMember.TypeName);
            Object obj = Activator.CreateInstance(type);
            Field[] childFields = this.ChildFields;

			for (Int32 i = 0; i < childFields.Length; i++)
			{
                Field field = childFields[i];
				PropertyInfo property = type.GetProperty(field.Name);
				if (property != null)
				{
					property.SetValue(obj, field.CreateObject(), null);
				}
				else
				{
					FieldInfo fieldInfo  = type.GetField(field.Name);
                    fieldInfo.SetValue(obj, field.CreateObject());
				}
			}

			return obj;
		}
	}
}
