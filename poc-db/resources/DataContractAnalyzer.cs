/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe DataContractAnalyzer
    /// </summary>
    [Serializable]
	public class DataContractAnalyzer : MarshalByRefObject
    {
        #region [Attributes and Properties]
        /// <summary>
        /// TypesCache, Dictionary que mantêm em memória os tipos utilizados no domain.
        /// </summary>
        public static IDictionary<String, Type> TypesCache = new Dictionary<String, Type>();

        /// <summary>
        /// ObjectsCache, Dictionary que mantêm em memória os objetos utilizados no domain.
        /// </summary>
        public static IDictionary<Object, Field> ObjectsCache = new Dictionary<Object, Field>(new EqualityComparer());

        /// <summary>
        /// serviceTypeInfoDictionary, Dictionary que mantêm em memória os objetos utilizados no domain.
        /// </summary>
        private static IDictionary<String, SerializableType> serviceTypeInfoDictionary = new Dictionary<String, SerializableType>();

        /// <summary>
        /// memberAttributes, define um array com o possíveis MemberAttributes no contrato
        /// </summary>
        private static Type[] memberAttributes = new Type[]
		{
			typeof(DataMemberAttribute),
			typeof(MessageBodyMemberAttribute),
			typeof(MessageHeaderAttribute),
			typeof(MessageHeaderArrayAttribute),
			typeof(XmlAttributeAttribute),
			typeof(XmlElementAttribute),
			typeof(XmlArrayAttribute),
			typeof(XmlTextAttribute)
		};

        /// <summary>
        /// typeAttributes, define um array com o possíveis TypeAttributes no contrato
        /// </summary>
        private static Type[] typeAttributes = new Type[]
		{
			typeof(DataContractAttribute),
			typeof(XmlTypeAttribute),
			typeof(MessageContractAttribute)
		};        

        #endregion

        #region [Methods]
        /// <summary>
        /// Varifica se o objeto deve ser armazenado no "cache" em memória.
        /// </summary>
        /// <param name="value">Objeto para a verificação.</param>
        /// <returns>Resultado da verificação.</returns>
        private static Boolean ShouldCache(Object value)
        {
            return !(value is String);
        }

        /// <summary>
        /// Intancia o objeto recebido como parâmetro, bem como todos os seus sub-tipos.
        /// </summary>
        /// <param name="name">Nome do campo.</param>
        /// <param name="value">Objeto a ser instânciado.</param>
        /// <returns>Objetod Field instânciado.</returns>
        public static Field BuildFieldInfo(String name, Object value)
        {
            if (value == null)
            {
                value = new NullField();
            }

            if (DataContractAnalyzer.ObjectsCache.ContainsKey(value))
            {
                return DataContractAnalyzer.ObjectsCache[value];
            }

            Type type = value.GetType();
            String fullName = type.FullName;
            SerializableType serviceTypeInfo;

            if (!DataContractAnalyzer.serviceTypeInfoDictionary.TryGetValue(fullName, out serviceTypeInfo))
            {
                serviceTypeInfo = DataContractAnalyzer.CreateServiceTypeInfo(type);
            }

            TypeMemberInfo typeMemberInfo = new TypeMemberInfo(name, serviceTypeInfo);
            Field field = FieldFactory.CreateAssociateField(typeMemberInfo, value);

            if (DataContractAnalyzer.ShouldCache(value))
            {
                DataContractAnalyzer.ObjectsCache.Add(value, field);
            }

            if (typeMemberInfo.Members != null)
            {
                Field[] fieldMembers;
                if (type.IsArray)
                {
                    Array array = (Array)value;
                    fieldMembers = new Field[array.Length];
                    for (Int32 indexArray = 0; indexArray < array.Length; indexArray++)
                    {
                        Object valueArrayItem = array.GetValue(indexArray);
                        fieldMembers[indexArray] = DataContractAnalyzer.BuildFieldInfo(String.Format("[{0}]", indexArray), valueArrayItem);
                    }
                }
                else if (DataContractAnalyzer.IsCollectionType(type))
                {
                    ICollection collection = (ICollection)value;
                    fieldMembers = new Field[collection.Count];
                    Int32 indexCollection = 0;
                    IEnumerator enumerator = collection.GetEnumerator();

                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            Object current = enumerator.Current;
                            fieldMembers[indexCollection++] = DataContractAnalyzer.BuildFieldInfo(String.Format("[{0}]", indexCollection), current);
                        }
                    }                    
                    finally
                    {
                        IDisposable disposableCollection = enumerator as IDisposable;
                        if (disposableCollection != null)
                            disposableCollection.Dispose();
                    }

                }
                else if (DataContractAnalyzer.IsDictionaryType(type))
                {
                    IDictionary dictionary = (IDictionary)value;
                    fieldMembers = new Field[dictionary.Count];
                    Int32 indexDictionary = 0;
                    IDictionaryEnumerator enumeratorDictionary = dictionary.GetEnumerator();

                    try
                    {
                        while (enumeratorDictionary.MoveNext())
                        {
                            DictionaryEntry dictionaryEntry = (DictionaryEntry)enumeratorDictionary.Current;
                            fieldMembers[indexDictionary++] = DataContractAnalyzer.BuildFieldInfo(String.Format("[{0}]", indexDictionary), dictionaryEntry);
                        }
                    }                    
                    finally
                    {
                        IDisposable disposableDictionary = enumeratorDictionary as IDisposable;
                        if (disposableDictionary != null)
                            disposableDictionary.Dispose();
                    }
                }
                else
                {
                    fieldMembers = new Field[typeMemberInfo.Members.Count];
                    Int32 index = 0;
                    PropertyInfo[] properties = type.GetProperties();
                    for (Int32 indexProperty = 0; indexProperty < properties.Length; indexProperty++)
                    {
                        PropertyInfo propertyInfo = properties[indexProperty];
                        if (DataContractAnalyzer.IsSupportedMember(propertyInfo) || value is DictionaryEntry || DataContractAnalyzer.IsKeyValuePairType(type))
                        {
                            Object valueProperty = propertyInfo.GetValue(value, null);
                            fieldMembers[index++] = DataContractAnalyzer.BuildFieldInfo(propertyInfo.Name, valueProperty);
                        }
                    }

                    FieldInfo[] fields = type.GetFields();
                    for (Int32 indexField = 0; indexField < fields.Length; indexField++)
                    {
                        FieldInfo fieldInfo = fields[indexField];
                        if (DataContractAnalyzer.IsSupportedMember(fieldInfo))
                        {
                            Object valueField = fieldInfo.GetValue(value);
                            fieldMembers[index++] = DataContractAnalyzer.BuildFieldInfo(fieldInfo.Name, valueField);
                        }
                    }
                }

                field.SetChildFields(fieldMembers);
            }

            return field;
        }

        /// <summary>
        /// Intancia o objeto recebido como parâmetro, bem como todos os seus sub-tipos e retorna um array.
        /// </summary>
        /// <param name="result">Objeto retornado pelo serviço.</param>
        /// <param name="outValues">Array que será preenchido com os parâmetros de saida.</param>
        /// <returns>Array com os campos instânciados.</returns>
        public static Field[] BuildFieldInfos(Object result, IDictionary<String, Object> outValues)
        {
            Field[] fields = new Field[outValues.Count + 1];
            DataContractAnalyzer.ObjectsCache.Clear();

            fields[0] = DataContractAnalyzer.BuildFieldInfo("(return)", result);
            Int32 index = 1;

            foreach (KeyValuePair<String, Object> current in outValues)
            {
                fields[index++] = DataContractAnalyzer.BuildFieldInfo(current.Key, current.Value);
            }

            return fields;
        }

        /// <summary>
        /// Valida se o tipo atual possui a marcação com o atributo: CollectionDataContractAttribute
        /// </summary>
        /// <param name="currentType">Tipo a ser validado.</param>
        /// <returns>Resultado da validação.</returns>
        public static Boolean IsCollectionType(Type currentType)
        {
            return currentType.GetCustomAttributes(typeof(CollectionDataContractAttribute), true).Length > 0;
        }

        /// <summary>
        /// Valida se tipo informado é um DataSet.
        /// </summary>
        /// <param name="type">Tipo que será validado.</param>
        /// <returns>Resultado da validação.</returns>
        public static Boolean IsDataSet(Type type)
        {
            Type typeFromHandle = typeof(DataSet);
            return type.Equals(typeFromHandle) || typeFromHandle.IsAssignableFrom(type);
        }

        /// <summary>
        /// Valida se tipo informado é um Dictionary.
        /// </summary>
        /// <param name="type">Tipo que será validado.</param>
        /// <returns>Resultado da validação.</returns>
        public static Boolean IsDictionaryType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        /// <summary>
        /// Valida se tipo informado é um KeyValuePair.
        /// </summary>
        /// <param name="type">Tipo que será validado.</param>
        /// <returns>Resultado da validação.</returns>
        public static Boolean IsKeyValuePairType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
        }

        /// <summary>
        /// Valida se tipo informado é um Nullable.
        /// </summary>
        /// <param name="type">Tipo que será validado.</param>
        /// <returns>Resultado da validação.</returns>
        public static Boolean IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Valida se tipo informado é um DictionaryEntry.
        /// </summary>
        /// <param name="type">Tipo que será validado.</param>
        /// <returns>Resultado da validação.</returns>
        public static Boolean IsSupportedType(Type currentType)
        {
            if (currentType == typeof(DictionaryEntry))
            {
                return true;
            }

            Type[] typeAttributes = DataContractAnalyzer.typeAttributes;

            for (Int32 i = 0; i < typeAttributes.Length; i++)
            {
                Type type = typeAttributes[i];
                if (DataContractAnalyzer.HasAttribute(currentType, type))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Valida se tipo informado é um DataTable.
        /// </summary>
        /// <param name="type">Tipo que será validado.</param>
        /// <returns>Resultado da validação.</returns>
        public static Boolean IsDataTable(Type type)
        {
            Type typeFromHandle = typeof(DataTable);
            return type.Equals(typeFromHandle) || typeFromHandle.IsAssignableFrom(type);
        }


        /// <summary>
        /// Analisa o contrato de dados e configura os métodos e parâmetros no endpoint.
        /// </summary>
        /// <param name="endpoint">EndPoint a ser analisado.</param>
        /// <param name="assemblyFullPath">Caminho completo da DLL de  proxy do endpoint.</param>
        /// <returns>ClienEndpoint com os métodos e parâmetros instânciados.</returns>
        public ClientEndpointInfo AnalyzeDataContract(ClientEndpointInfo endpoint, String assemblyFullPath)
        {
            Assembly clientAssembly = ClientSettings.ClientAssembly;
            Type type = clientAssembly.GetType(endpoint.OperationContractTypeName);

            if (type == null)
            {
                endpoint.Valid = false;
                return endpoint;
            }

            Object[] customAttributes = type.GetCustomAttributes(typeof(ServiceContractAttribute), true);

            if (customAttributes != null && customAttributes.Length == 1 && ((ServiceContractAttribute)customAttributes[0]).CallbackContract != null)
                endpoint.Valid = false;
            else
                endpoint.Valid = true;

            endpoint.ClientTypeName = DataContractAnalyzer.GetContractTypeName(type);
            MethodInfo[] methods = type.GetMethods();

            for (Int32 i = 0; i < methods.Length; i++)
            {
                MethodInfo methodInfo = methods[i];
                Boolean isOneWay = false;
                Object[] customAttributesMethod = methodInfo.GetCustomAttributes(typeof(OperationContractAttribute), false);

                if (customAttributesMethod.Length == 1 && ((OperationContractAttribute)customAttributesMethod[0]).IsOneWay)
                {
                    isOneWay = true;
                }

                ServiceMethodInfo serviceMethodInfo = new ServiceMethodInfo(endpoint, methodInfo.Name, isOneWay);
                endpoint.Methods.Add(serviceMethodInfo);
                ParameterInfo[] parameters = methodInfo.GetParameters();

                for (Int32 j = 0; j < parameters.Length; j++)
                {
                    ParameterInfo parameterInfo = parameters[j];
                    String name = parameterInfo.Name;
                    TypeMemberInfo parameter;

                    if (parameterInfo.ParameterType.IsByRef)
                        parameter = new TypeMemberInfo(name, DataContractAnalyzer.CreateServiceTypeInfo(parameterInfo.ParameterType.GetElementType()));
                    else
                        parameter = new TypeMemberInfo(name, DataContractAnalyzer.CreateServiceTypeInfo(parameterInfo.ParameterType));

                    if (parameterInfo.IsIn || !parameterInfo.IsOut)
                        serviceMethodInfo.InputParameters.Add(parameter);
                    else
                        serviceMethodInfo.OtherParameters.Add(parameter);
                }

                if (methodInfo.ReturnType != null && !methodInfo.ReturnType.Equals(typeof(void)))
                {
                    TypeMemberInfo parameterReturn = new TypeMemberInfo("(return)", DataContractAnalyzer.CreateServiceTypeInfo(methodInfo.ReturnParameter.ParameterType));
                    serviceMethodInfo.OtherParameters.Add(parameterReturn);
                }
            }

            return endpoint;
        }

        /// <summary>
        /// Cria um objeto SerializableType de acordo com o tipo informado.
        /// </summary>
        /// <param name="type">Tipo</param>
        /// <returns>Objeto SerializableType.</returns>
        private static SerializableType CreateServiceTypeInfo(Type type)
        {
            String fullName = type.FullName;
            if (DataContractAnalyzer.serviceTypeInfoDictionary.ContainsKey(type.FullName))
            {
                return DataContractAnalyzer.serviceTypeInfoDictionary[fullName];
            }

            Boolean isInvalid = false;
            SerializableType serviceTypeInfo = new SerializableType(type);
            DataContractAnalyzer.serviceTypeInfoDictionary.Add(fullName, serviceTypeInfo);

            if (type.IsArray)
            {
                SerializableType arrayType = DataContractAnalyzer.CreateServiceTypeInfo(type.GetElementType());
                isInvalid = arrayType.IsInvalid;
                serviceTypeInfo.Members.Add(new TypeMemberInfo("[0]", arrayType));
            }
            else if (DataContractAnalyzer.IsNullableType(type))
            {
                Type[] genericArguments = type.GetGenericArguments();
                SerializableType nullableType = DataContractAnalyzer.CreateServiceTypeInfo(genericArguments[0]);
                isInvalid = nullableType.IsInvalid;
                serviceTypeInfo.Members.Add(new TypeMemberInfo("Value", nullableType));
            }
            else if (DataContractAnalyzer.IsCollectionType(type))
            {
                Type baseType = type.BaseType;
                if (baseType.IsGenericType)
                {
                    Type[] genericArgumentsCollectionType = baseType.GetGenericArguments();
                    SerializableType collectionType = DataContractAnalyzer.CreateServiceTypeInfo(genericArgumentsCollectionType[0]);
                    isInvalid = collectionType.IsInvalid;
                    serviceTypeInfo.Members.Add(new TypeMemberInfo("[0]", collectionType));
                }
            }
            else if (DataContractAnalyzer.IsDictionaryType(type))
            {
                Type[] genericArgumentsDictionaryType = type.GetGenericArguments();
                Type typeFromHandle = typeof(KeyValuePair<,>);

                Type genericType = typeFromHandle.MakeGenericType(new Type[]
			    {
				    genericArgumentsDictionaryType[0],
				    genericArgumentsDictionaryType[1]
			    });

                SerializableType dictionaryType = DataContractAnalyzer.CreateServiceTypeInfo(genericType);
                isInvalid = dictionaryType.IsInvalid;
                serviceTypeInfo.Members.Add(new TypeMemberInfo("[0]", dictionaryType));

                if (!DataContractAnalyzer.TypesCache.ContainsKey(fullName))
                {
                    DataContractAnalyzer.TypesCache.Add(fullName, type);
                }
            }
            else if (DataContractAnalyzer.IsKeyValuePairType(type))
            {
                Type[] genericArgumentsKeyValuePairType = type.GetGenericArguments();
                SerializableType serializableTypeKey = DataContractAnalyzer.CreateServiceTypeInfo(genericArgumentsKeyValuePairType[0]);
                SerializableType serializableTypeValue = DataContractAnalyzer.CreateServiceTypeInfo(genericArgumentsKeyValuePairType[1]);

                isInvalid = (serializableTypeKey.IsInvalid || serializableTypeValue.IsInvalid);
                serviceTypeInfo.Members.Add(new TypeMemberInfo("Key", serializableTypeKey));
                serviceTypeInfo.Members.Add(new TypeMemberInfo("Value", serializableTypeValue));

                if (!DataContractAnalyzer.TypesCache.ContainsKey(fullName))
                {
                    DataContractAnalyzer.TypesCache.Add(fullName, type);
                }
            }
            else if (DataContractAnalyzer.IsSupportedType(type))
            {
                PropertyInfo[] properties = type.GetProperties();
                for (Int32 i = 0; i < properties.Length; i++)
                {
                    PropertyInfo propertyInfo = properties[i];
                    if (DataContractAnalyzer.IsSupportedMember(propertyInfo) || type == typeof(DictionaryEntry))
                    {
                        SerializableType serializableTypeProperty = DataContractAnalyzer.CreateServiceTypeInfo(propertyInfo.PropertyType);
                        if (serializableTypeProperty.IsInvalid)
                        {
                            isInvalid = true;
                        }
                        serviceTypeInfo.Members.Add(new TypeMemberInfo(propertyInfo.Name, serializableTypeProperty));
                    }
                }

                FieldInfo[] fields = type.GetFields();
                for (Int32 j = 0; j < fields.Length; j++)
                {
                    FieldInfo fieldInfo = fields[j];
                    if (DataContractAnalyzer.IsSupportedMember(fieldInfo))
                    {
                        SerializableType serializableTypeField = DataContractAnalyzer.CreateServiceTypeInfo(fieldInfo.FieldType);
                        if (serializableTypeField.IsInvalid)
                        {
                            isInvalid = true;
                        }
                        serviceTypeInfo.Members.Add(new TypeMemberInfo(fieldInfo.Name, serializableTypeField));
                    }
                }
            }

            if (isInvalid)
            {
                serviceTypeInfo.MarkAsInvalid();
            }

            Object[] customKnownTypeAttribure = type.GetCustomAttributes(typeof(KnownTypeAttribute), false);
            for (Int32 k = 0; k < customKnownTypeAttribure.Length; k++)
            {
                Object attr = customKnownTypeAttribure[k];
                var knownTypeAttribute = (KnownTypeAttribute)attr;
                serviceTypeInfo.SubTypes.Add(DataContractAnalyzer.CreateServiceTypeInfo(knownTypeAttribute.Type));
            }

            Object[] customXmlIncludeAttribute = type.GetCustomAttributes(typeof(XmlIncludeAttribute), false);
            for (Int32 l = 0; l < customXmlIncludeAttribute.Length; l++)
            {
                Object attr = customXmlIncludeAttribute[l];
                var xmlIncludeAttribute = (XmlIncludeAttribute)attr;
                serviceTypeInfo.SubTypes.Add(DataContractAnalyzer.CreateServiceTypeInfo(xmlIncludeAttribute.Type));
            }

            return serviceTypeInfo;
        }

        /// <summary>
        /// Obtêm o nome do tipo de contrato utilizado pelo serviço.
        /// </summary>
        /// <param name="contractType">Tipo do contrato.</param>
        /// <returns>Nome do tipo.</returns>
        private static String GetContractTypeName(Type contractType)
        {
            Type[] types = ClientSettings.ClientAssembly.GetTypes();

            for (Int32 i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                if (contractType.IsAssignableFrom(type) && !type.IsInterface)
                {
                    return type.FullName;
                }
            }

            return null;
        }

        /// <summary>
        /// Verifica se o member informado possui um custom attribute do tipo type.
        /// </summary>
        /// <param name="member">Objeto MemberInfo.</param>
        /// <param name="type">Objeto Type.</param>
        /// <returns>Resultado da verificação.</returns>
        private static Boolean HasAttribute(MemberInfo member, Type type)
        {
            return member.GetCustomAttributes(type, true).Length > 0;
        }

        /// <summary>
        /// Verifica se o member possui um tipo suportado de attributo.
        /// </summary>
        /// <param name="member">Objeto MemberInfo.</param>
        /// <returns>Resultado da validação.</returns>
        private static Boolean IsSupportedMember(MemberInfo member)
        {
            Type[] array = DataContractAnalyzer.memberAttributes;

            for (Int32 i = 0; i < array.Length; i++)
            {
                Type type = array[i];
                if (DataContractAnalyzer.HasAttribute(member, type))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
	}
}
