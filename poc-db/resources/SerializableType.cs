/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe SerializableType
    /// </summary>
    [Serializable]
    [DataContract]
	public class SerializableType
	{
        #region [Attributes and Properties]
        /// <summary>
        /// numericTypes
        /// </summary>
        [DataMember]
        private static List<String> numericTypes = new List<String>(new String[]
		{
			typeof(Int16).FullName,
			typeof(Int32).FullName,
			typeof(Int64).FullName,
			typeof(UInt16).FullName,
			typeof(UInt32).FullName,
			typeof(UInt64).FullName,
			typeof(Byte).FullName,
			typeof(SByte).FullName,
			typeof(float).FullName,
			typeof(Double).FullName,
			typeof(Decimal).FullName
		});

        /// <summary>
        /// dataSetSchema
        /// </summary>
        [DataMember]
        private String dataSetSchema;

        /// <summary>
        /// editorType
        /// </summary>
        [DataMember]
        private EditorType editorType;

        /// <summary>
        /// enumChoices
        /// </summary>
        [DataMember]
        private String[] enumChoices;

        /// <summary>
        /// friendlyName
        /// </summary>
        [DataMember]
        private String friendlyName;

        /// <summary>
        /// isInvalid
        /// </summary>
        [DataMember]
        private Boolean isInvalid;

        /// <summary>
        /// members
        /// </summary>
        [DataMember]
        private ICollection<TypeMemberInfo> members = new List<TypeMemberInfo>();

        /// <summary>
        /// subTypes
        /// </summary>
        [DataMember]
        private ICollection<SerializableType> subTypes = new List<SerializableType>();

        /// <summary>
        /// typeName
        /// </summary>
        [DataMember]
        private String typeName;

        /// <summary>
        /// typeProperty
        /// </summary>
        [DataMember]
        private TypeProperty typeProperty = new TypeProperty();
        
        /// <summary>
        /// DataSetSchema
        /// </summary>
        public String DataSetSchema
        {
            get
            {
                return this.dataSetSchema;
            }
        }

        /// <summary>
        /// EditorType
        /// </summary>
        public EditorType EditorType
        {
            get
            {
                return this.editorType;
            }
        }

        /// <summary>
        /// FriendlyName
        /// </summary>
        public String FriendlyName
        {
            get
            {
                if (this.friendlyName == null)
                {
                    this.ComposeFriendlyName();
                }
                return this.friendlyName;
            }
        }

        /// <summary>
        /// IsInvalid
        /// </summary>
        public Boolean IsInvalid
        {
            get
            {
                return this.isInvalid;
            }
        }

        /// <summary>
        /// Members
        /// </summary>
        public ICollection<TypeMemberInfo> Members
        {
            get
            {
                return this.members;
            }
        }

        /// <summary>
        /// SubTypes
        /// </summary>
        public ICollection<SerializableType> SubTypes
        {
            get
            {
                return this.subTypes;
            }
        }

        /// <summary>
        /// TypeName
        /// </summary>
        public String TypeName
        {
            get
            {
                return this.typeName;
            }
        }

        /// <summary>
        /// TypeProperty
        /// </summary>
        public TypeProperty TypeProperty
        {
            get
            {
                return this.typeProperty;
            }
        }
        #endregion

        #region [Constructors]
        /// <summary>
        /// Construtor SerializableType
        /// </summary>
        /// <param name="type">Type do objeto.</param>
        public SerializableType(Type type)
        {
            this.typeName = type.FullName;

            if (String.Compare(this.typeName, "System.Guid") == 0)
            {
                this.typeProperty = TypeProperty.Guid;
            }
            else if (type.IsEnum)
            {
                this.enumChoices = Enum.GetNames(type);
                this.typeProperty = TypeProperty.Enum;
            }
            else if (String.Compare(this.typeName, "System.Char") == 0)
            {
                this.typeProperty = TypeProperty.Char;
            }
            else if (String.Compare(this.typeName, "System.DateTime") == 0)
            {
                this.typeProperty = TypeProperty.DateTime;
            }
            else if (String.Compare(this.typeName, "System.DateTimeOffset") == 0)
            {
                this.typeProperty = TypeProperty.DateTimeOffSet;
            }
            else if (String.Compare(this.typeName, "System.TimeSpan") == 0)
            {
                this.typeProperty = TypeProperty.TimeSpan;
            }
            else if (String.Compare(this.typeName, "System.String") == 0)
            {
                this.typeProperty = TypeProperty.String;
            }
            else if (String.Compare(this.typeName, "System.Uri") == 0)
            {
                this.typeProperty = TypeProperty.SystemUri;
            }
            else if (String.Compare(this.typeName, "System.Boolean") == 0)
            {
                this.typeProperty = TypeProperty.Boolean;
            }
            else if (SerializableType.numericTypes.Contains(this.typeName))
            {
                this.typeProperty = TypeProperty.Numeric;
            }
            else if (DataContractAnalyzer.IsDataSet(type))
            {
                this.typeProperty = TypeProperty.DataSet;
                DataSet dataSet = Activator.CreateInstance(type) as DataSet;
                this.dataSetSchema = dataSet.GetXmlSchema();
            }
            else if (DataContractAnalyzer.IsDataTable(type))
            {
                this.typeProperty = TypeProperty.DataTable;
            }
            else if (type.IsArray)
            {
                this.typeProperty = TypeProperty.Array;
            }
            else if (DataContractAnalyzer.IsNullableType(type))
            {
                this.typeProperty = TypeProperty.Nullable;
            }
            else if (DataContractAnalyzer.IsCollectionType(type))
            {
                this.typeProperty = TypeProperty.Collection;
            }
            else if (DataContractAnalyzer.IsDictionaryType(type))
            {
                this.typeProperty = TypeProperty.Dictionary;
            }
            else if (DataContractAnalyzer.IsKeyValuePairType(type))
            {
                this.typeProperty = TypeProperty.KeyValuePair;
            }
            else if (DataContractAnalyzer.IsSupportedType(type))
            {
                this.typeProperty = TypeProperty.Composite;
                if (type.IsValueType)
                {
                    this.typeProperty = TypeProperty.Struct;
                }
            }
            else if (this.typeName.Contains("NullField"))
            {
                this.typeProperty = TypeProperty.NullField;
                this.typeName = "NullField";
            }           

            if (SerializableType.numericTypes.Contains(this.typeName) || this.typeProperty == TypeProperty.Char || 
                this.typeProperty == TypeProperty.Guid || this.typeProperty == TypeProperty.DateTime || 
                this.typeProperty == TypeProperty.DateTimeOffSet || this.typeProperty == TypeProperty.TimeSpan || 
                this.typeProperty == TypeProperty.String || this.typeProperty == TypeProperty.SystemUri ||
                this.typeProperty == TypeProperty.NullField)
            {
                this.editorType = EditorType.TextBox;
                return;
            }

            if (this.IsContainer())
            {
                this.editorType = EditorType.TextBoxArray;
                return;
            }

            if (this.typeProperty == TypeProperty.Boolean || this.HasMembers() || this.enumChoices != null 
                || this.TypeProperty == TypeProperty.DataSet)
            {
                this.editorType = EditorType.DropDownBox;
                return;
            }

        }
        #endregion

        #region [Methods]
        /// <summary>
        /// Retorna o valor padrão de acordo com o tipo definido no atributo typeName.
        /// </summary>
        /// <returns>Valor padrão para o tipo.</returns>
        public String GetDefaultValue()
        {
            if (this.enumChoices != null)
                return this.enumChoices[0];

            if (SerializableType.numericTypes.Contains(this.typeName))
                return "0";

            if (this.typeProperty == TypeProperty.Boolean)
                return Boolean.FalseString;

            if (this.typeProperty == TypeProperty.Char)
                return "A";

            if (this.typeProperty == TypeProperty.Guid)
                return Guid.NewGuid().ToString();

            if (this.typeProperty == TypeProperty.DateTime)
            {
                StringBuilder sbDateTime = new StringBuilder();
                sbDateTime.Append(DateTime.Now.ToShortDateString()).Append(" ").Append(DateTime.Now.ToShortTimeString());

                return sbDateTime.ToString();
            }

            if (this.typeProperty == TypeProperty.DateTimeOffSet)
                return DateTimeOffset.Now.ToString();

            if (this.typeProperty == TypeProperty.TimeSpan)
                return TimeSpan.Zero.ToString();

            if (this.typeProperty == TypeProperty.SystemUri)
                return "http://localhost";

            if (this.IsContainer())
                return "length=0";

            if (this.typeProperty == TypeProperty.KeyValuePair || this.typeProperty == TypeProperty.Struct)
                return this.typeName;

            return "(null)";
        }

        /// <summary>
        /// Retorna a lista de itens selecionados, utilizado somente para dropdown com checkbox.
        /// </summary>
        /// <returns>Lista de valores selecionados no combo.</returns>
        public String[] GetSelectionList()
        {
            String[] arraySelectionList;

            if (this.editorType != EditorType.DropDownBox)
            {
                return null;
            }
            if (this.typeProperty == TypeProperty.Boolean)
            {
                arraySelectionList = new String[]
				{
					"True",
					"False"
				};
            }
            else
            {
                if (this.enumChoices != null)
                {
                    return this.enumChoices;
                }
                if (this.typeProperty == TypeProperty.KeyValuePair || this.typeProperty == TypeProperty.Struct)
                {
                    arraySelectionList = new String[]
					{
						this.typeName
					};
                }
                else
                {
                    if (this.typeProperty == TypeProperty.DataSet)
                    {
                        arraySelectionList = new String[]
						{
							"(null)",
							""
						};
                    }
                    else
                    {
                        arraySelectionList = new String[0];
                    }
                }
            }

            if (arraySelectionList != null && arraySelectionList.Length == 0)
            {
                var list = new List<String>();
                list.Add("(null)");
                list.Add(this.typeName);
                foreach (SerializableType current in this.subTypes)
                {
                    if (!current.IsInvalid)
                    {
                        list.Add(current.TypeName);
                    }
                }
                arraySelectionList = new String[list.Count];
                list.CopyTo(arraySelectionList);
            }
            return arraySelectionList;
        }

        /// <summary>
        /// Retorna a strig de representação do campo de acordo com o tipo.
        /// </summary>
        /// <param name="obj">Objeto.</param>
        /// <returns>String com a representação do campo.</returns>
        public String GetStringRepresentation(Object obj)
        {
            if (obj == null)
            {
                return "(null)";
            }
            if (obj.GetType().Equals(typeof(NullField)))
            {
                return "(null)";
            }
            if (this.typeProperty == TypeProperty.DataSet)
            {
                return String.Empty;
            }
            if (this.editorType == EditorType.DropDownBox)
            {
                if (obj.GetType().Equals(typeof(Boolean)) || this.enumChoices != null)
                {
                    return obj.ToString();
                }
                return String.Empty;
            }
            else
            {
                if (obj.GetType().IsArray)
                {
                    return String.Format("length={0}",((Array)obj).Length);
                }
                if (DataContractAnalyzer.IsDictionaryType(obj.GetType()) || DataContractAnalyzer.IsCollectionType(obj.GetType()))
                {
                    return String.Format("length={0}", ((ICollection)obj).Count);
                }
                if (obj is String)
                {
                    return StringFormatter.ToEscapeCode(obj.ToString());
                }
                return obj.ToString();
            }
        }

        /// <summary>
        /// Analisa o atributo "typeProperty" e verifica se ele possui membros.
        /// </summary>
        /// <returns>Retorno da validação.</returns>
        public Boolean HasMembers()
        {
            return this.typeProperty == TypeProperty.Composite || this.typeProperty == TypeProperty.Nullable || this.typeProperty == TypeProperty.KeyValuePair || 
                this.typeProperty == TypeProperty.Struct;
        }

        /// <summary>
        /// Analisa o atributo "typeProperty" e verifica o tipo em questão é um array ou collection.
        /// </summary>
        /// <returns>Retorno da comparação.</returns>
        public Boolean IsContainer()
        {
            return this.typeProperty == TypeProperty.Array || this.typeProperty == TypeProperty.Dictionary || this.typeProperty == TypeProperty.Collection;
        }

        /// <summary>
        /// Seta o atributo isValid para true.
        /// </summary>
        public void MarkAsInvalid()
        {
            this.isInvalid = true;
        }

        /// <summary>
        /// Formata o nome do tipo de uma forma amigável para exibição no FrontEnd.
        /// </summary>
        private void ComposeFriendlyName()
        {
            Int32 num = this.TypeName.IndexOf('`');
            if (num > -1)
            {
                StringBuilder stringBuilder = new StringBuilder(this.TypeName.Substring(0, num));
                stringBuilder.Append("<");
                ICollection<TypeMemberInfo> collectionMembers = this.members;

                if (this.typeProperty == TypeProperty.Dictionary)
                {
                    collectionMembers = ((List<TypeMemberInfo>)this.members)[0].Members;
                }

                Int32 count = 0;
                foreach (TypeMemberInfo current in collectionMembers)
                {
                    if (count++ > 0)
                    {
                        stringBuilder.Append(",");
                    }
                    stringBuilder.Append(current.FriendlyTypeName);
                }

                stringBuilder.Append(">");
                this.friendlyName = stringBuilder.ToString();
                return;
            }
            this.friendlyName = this.TypeName;
        }
        #endregion       
	}
}
