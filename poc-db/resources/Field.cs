/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Linq;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe Field
    /// </summary>    
	[Serializable]
    [KnownType("GetKnownTypes")]    
    [DataContract(IsReference=true)]    
	public class Field
    {
        #region [Attributes and Properties]
        
        /// <summary>
        /// ChildFields
        /// </summary>
        [DataMember]
        protected Field[] ChildFields { get; set; }

        /// <summary>
        /// CurrentMember
        /// </summary>
        [DataMember]
        protected TypeMemberInfo CurrentMember { get; set; }

        /// <summary>
        /// DeclaredMember
        /// </summary>  
        [DataMember]
        public TypeMemberInfo DeclaredMember { get; set; }

        /// <summary>
        /// ServiceMethodInfo
        /// </summary>
        [NonSerialized]
        protected ServiceMethodInfo ServiceMethodInfo;

        /// <summary>
        /// name
        /// </summary>
        protected String name;

        /// <summary>
        /// value
        /// </summary>
        protected String value;

        /// <summary>
        /// empty
        /// </summary>
        private static readonly Field[] empty = new Field[0];

        /// <summary>
        /// poolSize
        /// </summary>
        private static Int32 poolSize = 1;

        /// <summary>
        /// poolSize
        /// </summary>
        private static IList<Field> fieldPool = new List<Field>();

        /// <summary>
        /// isKey
        /// </summary>
        [DataMember]
        private Boolean isKey;

        /// <summary>
        /// modifiable
        /// </summary>
        [DataMember]
        private Boolean modifiable = true;

        /// <summary>
        /// isCollapsed
        /// </summary>
        private Boolean isCollapsed = true;

        /// <summary>
        /// Parent
        /// </summary>
        [DataMember]
        public Field Parent { get; set; }

        /// <summary>
        /// TextIdentity
        /// </summary>
        [DataMember]
        public Int32 TextIdentity { get; set; }

        /// <summary>
        /// EditorType
        /// </summary>
        public EditorType EditorType
        {
            get
            {
                return this.DeclaredMember.EditorType;
            }
        }

        /// <summary>
        /// FriendlyTypeName
        /// </summary>        
        public String FriendlyTypeName
        {
            get
            {
                return this.DeclaredMember.FriendlyTypeName;
            }
        }

        /// <summary>
        /// IsKey
        /// </summary>
        public Boolean IsKey
        {
            set
            {
                this.isKey = value;
                if (this.isKey && String.Compare(this.value, "(null)", StringComparison.Ordinal) == 0)
                {
                    if (this.DeclaredMember.HasMembers())
                    {
                        this.value = this.TypeName;
                    }
                    if (String.Compare(this.TypeName, "System.String", StringComparison.Ordinal) == 0)
                    {
                        this.value = String.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public virtual String Name
        {
            get
            {
                if (this.name == null)
                {
                    return this.DeclaredMember.FieldName;
                }
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// TypeName
        /// </summary>
        public String TypeName
        {
            get
            {
                return this.DeclaredMember.TypeName;
            }
        }

        /// <summary>
        /// IsCollapsed
        /// </summary>
        [DataMember]
        public Boolean IsCollapsed
        {
            get { return this.isCollapsed; }
            set { this.isCollapsed = value; }
        }

        /// <summary>
        /// Value
        /// </summary>
        [DataMember]
        public String Value
        {
            get
            {
                if (String.Compare(this.value, this.TypeName, StringComparison.Ordinal) == 0 && this.CurrentMember.HasMembers())
                {
                    return this.FriendlyTypeName;
                }
                return this.value;
            }

            set { this.value = value; }
        }

        #endregion

        #region [Constructors]

        /// <summary>
        /// Construtor Field
        /// </summary>
        public Field()
        { 

        }

        /// <summary>
        /// Constructor Field
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public Field(TypeMemberInfo declaredMember)
        {
            this.CurrentMember = declaredMember;
            this.DeclaredMember = declaredMember;
            this.value = this.CurrentMember.GetDefaultValue();
        }

        /// <summary>
        /// Constructor Field
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        /// <param name="obj">Instância de objeto do tipo definido pelo "declaredMember"</param>
        public Field(TypeMemberInfo declaredMember, Object obj)
            : this(declaredMember)
        {
            this.value = declaredMember.GetStringRepresentation(obj);
            this.modifiable = false;
        }
        #endregion

        #region [Methods]
        /// <summary>
        /// Método base para fazer um clone do campo.
        /// </summary>
        /// <returns>Uma cópia da instância do objeto.</returns>
        public virtual Field Clone()
        {
            return null;
        }

        /// <summary>
        /// Copia o valeu do parâmetro para o atributo value.
        /// </summary>
        /// <param name="field">Objeto do tipo Field.</param>
        /// <returns>Retorna o resultado da comparação e cópia.</returns>
        public virtual Boolean CopyFrom(Field field)
        {
            if (field == null || Object.ReferenceEquals(this, field))
            {
                return false;
            }
            this.value = field.value;
            return true;
        }

        /// <summary>
        /// Método base para criar um objeto do tipo DeclaredMember.
        /// </summary>
        /// <returns></returns>
        public virtual Object CreateObject()
        {
            return null;
        }

        /// <summary>
        /// Cria todos o campos filhos e seta o atribuito "ChildFields"
        /// </summary>
        /// <returns>Array com todos os campos filhos criados.</returns>
        public Field[] GetChildFields()
        {
            if (String.Compare(this.value, "(null)", StringComparison.Ordinal) == 0)
            {
                return Field.empty;
            }

            if (this.modifiable)
            {
                if (this.DeclaredMember.HasMembers() && (this.ChildFields == null ||
                    String.Compare(this.value, this.CurrentMember.TypeName) != 0))
                {
                    this.CurrentMember = this.DeclaredMember;
                    String fieldName = this.DeclaredMember.FieldName;

                    foreach (SerializableType current in this.DeclaredMember.SubTypes)
                    {
                        if (String.Compare(current.TypeName, this.value) == 0)
                        {
                            this.CurrentMember = new TypeMemberInfo(fieldName, current);
                            break;
                        }
                    }

                    this.ChildFields = new Field[this.CurrentMember.Members.Count];
                    Int32 num = 0;
                    foreach (TypeMemberInfo current in this.CurrentMember.Members)
                    {
                        this.ChildFields[num] = FieldFactory.CreateAssociateField(current);
                        if (this.CurrentMember.TypeProperty == TypeProperty.KeyValuePair
                            && String.Compare(current.FieldName, "Key", StringComparison.Ordinal) == 0)
                        {
                            this.ChildFields[num].IsKey = true;
                        }
                        this.ChildFields[num].SetServiceMethodInfo(this.ServiceMethodInfo);
                        this.ChildFields[num].Parent = this;

                        num++;
                    }
                }

                if (this.DeclaredMember.IsContainer())
                {
                    Int32 arrayLength = Field.GetArrayLength(this.value);
                    Field[] currentChildFields = this.ChildFields;
                    this.ChildFields = new Field[arrayLength];
                    TypeMemberInfo memberInfo = null;
                    using (IEnumerator<TypeMemberInfo> enumeratorMembers = this.DeclaredMember.Members.GetEnumerator())
                    {
                        if (enumeratorMembers.MoveNext())
                        {
                            TypeMemberInfo current = enumeratorMembers.Current;
                            memberInfo = current;
                        }
                    }
                    for (Int32 i = 0; i < arrayLength; i++)
                    {
                        if (currentChildFields != null && i < currentChildFields.Length)
                        {
                            this.ChildFields[i] = currentChildFields[i];
                        }
                        else
                        {
                            this.ChildFields[i] = FieldFactory.CreateAssociateField("[" + i + "]", memberInfo);
                            this.ChildFields[i].SetServiceMethodInfo(this.ServiceMethodInfo);
                            this.ChildFields[i].Parent = this;
                            if (this.DeclaredMember.TypeProperty == TypeProperty.Dictionary)
                            {
                                this.ChildFields[i].GetChildFields();
                            }
                        }
                    }
                }
            }
            return this.ChildFields;
        }
              
        /// <summary>
        /// Verifica se o campo possui campos filhos.
        /// </summary>
        /// <returns>Resultado da verificação.</returns>
        public Boolean IsExpandable()
        {
            if (this.ChildFields != null && this.ChildFields.Length > 0)
            {
                return true;
            }
            if (this.EditorType == EditorType.DropDownBox)
            {
                return !(this.DeclaredMember.TypeProperty == TypeProperty.Boolean) &&
                    !(this.DeclaredMember.TypeProperty == TypeProperty.Enum) &&
                    !(this.DeclaredMember.TypeProperty == TypeProperty.DataSet) &&
                    !String.Equals(this.value, "(null)", StringComparison.Ordinal);
            }
            return this.DeclaredMember.IsContainer() &&
                !(String.Compare(this.value, "(null)") == 0) && Field.GetArrayLength(this.value) > 0;
        }

        /// <summary>
        /// Seta os campos filhos.
        /// </summary>
        /// <param name="value">Array com os novos campos filhos.</param>
        public void SetChildFields(Field[] value)
        {
            this.ChildFields = value;
        }

        /// <summary>
        /// Seta o atributo ServiceMethodInfo.
        /// </summary>
        /// <param name="serviceMethodInfo">Objeto do tipo ServiceMethodInfo.</param>
        public void SetServiceMethodInfo(ServiceMethodInfo serviceMethodInfo)
        {
            this.ServiceMethodInfo = serviceMethodInfo;
        }

        /// <summary>
        /// Valida se o input é null e seta o atributo value.
        /// </summary>
        /// <param name="input">String com o valor que se deseja setar.</param>
        public virtual void ValidateAndCanonicalize(String input)
        {
            if (input == null)
            {
                this.value = null;
                return;
            }
            if (this.isKey && String.Compare(input, "(null)", StringComparison.Ordinal) == 0)
            {
                this.value = null;
                return;
            }

            this.value = input;
        }

        /// <summary>
        /// Retorna o tamanho do array.
        /// </summary>
        /// <param name="fieldArrayValue">Valor do campo tipo array.</param>
        /// <returns>Tamanho do array.</returns>
        private static Int32 GetArrayLength(String fieldArrayValue)
        {
            return Int32.Parse(fieldArrayValue.Substring("length=".Length), CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Retorna todo os tipos derivados do tipo Field.
        /// </summary>
        /// <returns>Enumerable de tipos.</returns>
        public static IEnumerable<Type> GetKnownTypes()
        {
            var knownTypes = default(IEnumerable<Type>);
            Type baseType = typeof(Field);
            var allTypes = default(IEnumerable<Type>);
            Assembly assembly = baseType.Assembly;

            allTypes = assembly.GetTypes().OfType<Type>();

            knownTypes = allTypes.Where(t => t != baseType && t.IsSubclassOf(baseType));
            
            return knownTypes;
        }

        #endregion
	}
}
