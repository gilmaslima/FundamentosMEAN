/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe TypeMemberInfo
    /// </summary>
    [Serializable]
    [DataContract]    
    public class TypeMemberInfo : IComparable
    {
        #region [Attributes and Properties]
        /// <summary>
        /// type
        /// </summary>
        [DataMember]
        private SerializableType type;

        /// <summary>
        /// fieldName
        /// </summary>
        [DataMember]
        private String fieldName;

        /// <summary>
        /// DataSetSchema
        /// </summary>
        public String DataSetSchema
        {
            get
            {
                return this.type.DataSetSchema;
            }
        }

        /// <summary>
        /// EditorType
        /// </summary>
        public EditorType EditorType
        {
            get
            {
                return this.type.EditorType;
            }
        }

        /// <summary>
        /// FriendlyTypeName
        /// </summary>
        public String FriendlyTypeName
        {
            get
            {
                return this.type.FriendlyName;
            }
        }

        /// <summary>
        /// IsInvalid
        /// </summary>
        public Boolean IsInvalid
        {
            get
            {
                return this.type.IsInvalid;
            }
        }

        /// <summary>
        /// Members
        /// </summary>
        public ICollection<TypeMemberInfo> Members
        {
            get
            {
                return this.type.Members;
            }
        }

        /// <summary>
        /// SubTypes
        /// </summary>
        public ICollection<SerializableType> SubTypes
        {
            get
            {
                return this.type.SubTypes;
            }
        }

        /// <summary>
        /// TypeName
        /// </summary>
        public String TypeName
        {
            get
            {
                return this.type.TypeName;
            }
        }

        /// <summary>
        /// TypeProperty
        /// </summary>
        public TypeProperty TypeProperty
        {
            get
            {
                return this.type.TypeProperty;
            }
        }

        /// <summary>
        /// FieldName
        /// </summary>
        public String FieldName
        {
            get
            {
                return this.fieldName;
            }
        }
        #endregion

        #region [Methods]
        /// <summary>
        /// Construtor TypeMemberInfo
        /// </summary>
        /// <param name="fieldName">Nome do campo.</param>
        /// <param name="type">Objeto SerializableType correspondente ao campo.</param>
        public TypeMemberInfo(String fieldName, SerializableType type)
        {
            this.fieldName = fieldName;
            this.type = type;
        }

        /// <summary>
        /// Obtêm o valor padrão do campo baseado no tipo.
        /// </summary>
        /// <returns>Valor padrão do campo.</returns>
        public String GetDefaultValue()
        {
            return this.type.GetDefaultValue();
        }

        /// <summary>
        /// Retorna a lista de itens selecionados, utilizado somente para dropdown com checkbox.
        /// </summary>
        /// <returns>Lista de valores selecionados no combo.</returns>
        public String[] GetSelectionList()
        {
            return this.type.GetSelectionList();
        }

        /// <summary>
        /// Retorna a strig de representação do campo de acordo com o tipo.
        /// </summary>
        /// <param name="obj">Objeto.</param>
        /// <returns>String com a representação do campo.</returns>
        public String GetStringRepresentation(Object obj)
        {
            return this.type.GetStringRepresentation(obj);
        }

        /// <summary>
        /// Analisa o atributo "typeProperty" e verifica se ele possui membros.
        /// </summary>
        /// <returns>Retorno da validação.</returns>
        public Boolean HasMembers()
        {
            return this.type.HasMembers();
        }

        /// <summary>
        /// Analisa o atributo "typeProperty" e verifica o tipo em questão é um array ou collection.
        /// </summary>
        /// <returns>Retorno da comparação.</returns>
        public Boolean IsContainer()
        {
            return this.type.IsContainer();
        }

        /// <summary>
        /// Implementação da interface IComparable
        /// </summary>
        /// <param name="obj">Objeto para comparação.</param>
        /// <returns>Resultado da comparação.</returns>
        public Int32 CompareTo(Object obj)
        {
            var typeMemberInfo = (TypeMemberInfo)obj;
            return String.Compare(this.fieldName, typeMemberInfo.fieldName, StringComparison.Ordinal);
        }

        #endregion

    }
}
