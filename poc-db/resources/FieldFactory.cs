/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe FieldFactory
    /// </summary>
	public class FieldFactory
	{
        /// <summary>
        /// Cria um novo objeto que herda de Field de acordo com o TypeMemberInfo.
        /// </summary>
        /// <param name="memberInfo">Objeto TypeMemberInfo.</param>
        /// <returns>Objeto que deriva de Field criado.</returns>
		public static Field CreateAssociateField(TypeMemberInfo memberInfo)
		{
             Field associateField = null;

            switch (memberInfo.TypeProperty)
            {
                case TypeProperty.Enum:
                    associateField = new EnumField(memberInfo);
                    break;
                case TypeProperty.Numeric:
                    associateField = new NumericField(memberInfo);
                    break;                    
                case TypeProperty.Boolean:
                    associateField = new BooleanField(memberInfo);
                    break;
                
                case TypeProperty.Char:
                    associateField = new CharField(memberInfo);
                    break;  
            
                case TypeProperty.Guid:
                    associateField = new GuidField(memberInfo);
                    break;

                case TypeProperty.String:
                    associateField = new StringField(memberInfo);
                    break;

                case TypeProperty.DateTime:
                    associateField = new DateTimeField(memberInfo);
                    break;

                case TypeProperty.DateTimeOffSet:
                    associateField = new DateTimeOffsetField(memberInfo);
                    break;

                case TypeProperty.TimeSpan:
                    associateField = new TimeSpanField(memberInfo);
                    break;

                case TypeProperty.SystemUri:
                    associateField = new UriField(memberInfo);
                    break;

                case TypeProperty.Array:
                    associateField = new ArrayField(memberInfo);
                    break;

                case TypeProperty.Collection:
                    associateField = new CollectionField(memberInfo);
                    break;

                case TypeProperty.Dictionary:
                    associateField = new DictionaryField(memberInfo);
                    break;

                case TypeProperty.Nullable:
                    associateField = new NullableField(memberInfo);
                    break;

                case TypeProperty.KeyValuePair:
                    associateField = new KeyValuePairField(memberInfo);
                    break;

                case TypeProperty.DataSet:
                    associateField = new DataSetField(memberInfo);
                    break;

                default:
                    associateField = new CompositeField(memberInfo);
                    break; 
            }
           
            return associateField;         

		}

        /// <summary>
        /// Cria um novo DataSet, DataField ou Field baseado no TypeMemberInfo e na instância do objeto informada como parâmetro.
        /// </summary>
        /// <param name="memberInfo">Objeto TyperMemeberInfo.</param>
        /// <param name="obj">Instância do objeto do tipo correspondente ao TyperMemberInfo.</param>
        /// <returns>Novo objeto que deriva de Field.</returns>
		public static Field CreateAssociateField(TypeMemberInfo memberInfo, Object obj)
		{
			if (memberInfo.TypeProperty == TypeProperty.DataSet)
			{
				return new DataSetField(memberInfo, obj);
			}

            if (memberInfo.TypeProperty == TypeProperty.DataTable)
            {
                return new DataTableField(memberInfo, obj);
            }

			return new Field(memberInfo, obj);
		}

        /// <summary>
        /// Cria um novo objeto que herda de Field utilizando o nome e o TypeMemberInfo.
        /// </summary>
        /// <param name="name">Nome do campo.</param>
        /// <param name="memberInfo">Objeto que define o tipo do campo.</param>
        /// <returns>Novo objeto que deriva de Field.</returns>
        public static Field CreateAssociateField(String name, TypeMemberInfo memberInfo)
		{
			Field field = FieldFactory.CreateAssociateField(memberInfo);
			field.Name = name;
			return field;
		}

	}
}
