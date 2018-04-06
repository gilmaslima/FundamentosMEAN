/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe DataSetField
    /// </summary>
    [Serializable]
    [DataContract]
    public class DataSetField : Field
    {
        #region [Attributes]
        /// <summary>
        /// Data Set Value.
        /// </summary>
        [DataMember]
        private DataSet dataSetValue;

        /// <summary>
        /// Is General Data Set
        /// </summary>
        [DataMember]
        private Boolean IsGeneralDataSet
        {
            get
            {
                return String.Compare(this.CurrentMember.TypeName, "System.Data.DataSet", StringComparison.Ordinal) == 0;
            }
        }        
        #endregion

        #region [Constructors]

        /// <summary>
        /// Construtor DataSetField
        /// </summary>
        public DataSetField()
        { 
        }

        /// <summary>
        /// Construtor DataSetField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        public DataSetField(TypeMemberInfo declaredMember)
            : base(declaredMember)
        {
            this.CreateEmptyDataSet(declaredMember.DataSetSchema);
        }

        /// <summary>
        /// Construtor DataSetField
        /// </summary>
        /// <param name="declaredMember">Objeto que define o tipo do campo.</param>
        /// <param name="obj">Objeto do tipo DataSet.</param>
        public DataSetField(TypeMemberInfo declaredMember, Object obj)
            : base(declaredMember, obj)
        {
            this.CreateEmptyDataSet(((DataSet)obj).GetXmlSchema());
            using (StringReader stringReader = new StringReader(((DataSet)obj).GetXml()))
            {
                this.dataSetValue.ReadXml(stringReader);
            }
        }        
        #endregion

        #region [Methods]
        /// <summary>
        /// Faz um clone do campo atual e retorna como um Field.
        /// </summary>
        /// <returns></returns>
        public override Field Clone()
        {
            DataSetField dataSetField = new DataSetField(this.CurrentMember);
            if (dataSetField.CopyFrom(this))
            {
                return dataSetField;
            }
            return null;
        }

        /// <summary>
        /// Copia o parâmetro para o atributo dataSetValue.
        /// </summary>
        /// <param name="field">Objeto do tipo DataSetField.</param>
        /// <returns>Retorna se a cópia foi realizada com sucesso ou não.</returns>
        public override Boolean CopyFrom(Field field)
        {
            if (field == null || Object.ReferenceEquals(this, field))
            {
                return false;
            }
            this.dataSetValue.Dispose();
            DataSetField dataSetField = field as DataSetField;
            this.dataSetValue = dataSetField.dataSetValue.Copy();
            return true;
        }

        /// <summary>
        /// Cria um objeto do tipo DataSet baseado no atributo value.
        /// </summary>
        /// <returns>Objeto do tipo DataSet</returns>
        public override Object CreateObject()
        {
            if (String.Compare(this.value, "(null)") == 0)
            {
                return null;
            }

            if (this.IsGeneralDataSet)
            {
                return this.dataSetValue;
            }

            Type type = ClientSettings.GetType(this.CurrentMember.TypeName);
            Object obj = Activator.CreateInstance(type);
            using (StringReader stringReader = new StringReader(this.dataSetValue.GetXml()))
            {
                ((DataSet)obj).ReadXml(stringReader);
            }
            return obj;
        }

        /// <summary>
        /// Retorna o atributo dataSetValue.
        /// </summary>
        /// <returns>Atributo dataSetValue.</returns>
        public Object GetDataSetValue()
        {
            return this.dataSetValue;
        }

        /// <summary>
        /// Rertorna um XML com o schema do atributo dataSetValue.
        /// </summary>
        /// <returns>String XML com o schema do dataSetValue.</returns>
        public String GetXmlSchema()
        {
            return this.dataSetValue.GetXmlSchema();
        }

        /// <summary>
        /// Valida se o dataSetValue possui um schema padrão. 
        /// </summary>
        /// <returns>Resultado da validação.</returns>
        public Boolean IsDefaultDataSet()
        {
            DataSet obj = new DataSet(this.dataSetValue.DataSetName);
            DataSetField field = new DataSetField(this.CurrentMember, obj);
            return this.SchemaEquals(field);
        }

        /// <summary>
        /// Compara o schema do DataSetField informado com o schema do atributo dataSetValue.
        /// </summary>
        /// <param name="field">Objeto do tipo DataSetField.</param>
        /// <returns>Resultado da comparação.</returns>
        public Boolean SchemaEquals(Field field)
        {
            DataSetField dataSetField = field as DataSetField;
            return string.Equals(this.GetXmlSchema(), dataSetField.GetXmlSchema(), StringComparison.Ordinal);
        }

        /// <summary>
        /// Cria um DataSet em branco utilizando o schema informado.
        /// </summary>
        /// <param name="schema">XML com o schema desejado.</param>
        private void CreateEmptyDataSet(String schema)
        {
            this.dataSetValue = new DataSet();
            using (StringReader stringReader = new StringReader(schema))
            {
                this.dataSetValue.ReadXmlSchema(stringReader);
            }
            this.dataSetValue.Locale = this.dataSetValue.Locale;
        }
        #endregion
        
	}
}
