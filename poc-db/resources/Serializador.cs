using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Redecard.PN.Comum
{
    abstract class BaseReflector : IReflectionProvider
    {
        public virtual T GetSingleAttributeOrDefault<T>(MemberInfo memberInfo) where T : Attribute, new()
        {
            object[] attributes = memberInfo.GetCustomAttributes(typeof(T), false);
            return attributes.Length == 0 ? new T() : attributes[0] as T;
        }

        public virtual IEnumerable<MemberInfo> GetSerializableMembers(Type type)
        {
            return type.GetProperties(ReflectionHelper.PublicInstanceMembers | BindingFlags.FlattenHierarchy)
                .Where(p => p.GetGetMethod() != null && p.GetGetMethod().GetParameters().Length == 0)
                .Cast<MemberInfo>()
                .Union(type.GetFields(ReflectionHelper.PublicInstanceMembers | BindingFlags.FlattenHierarchy).Cast<MemberInfo>());
        }

        public abstract object GetValue(MemberInfo member, object instance);
        public abstract MethodHandler GetDelegate(MethodBase method);
    }

    class DirectReflector : BaseReflector
    {

        public override object GetValue(MemberInfo member, object instance)
        {
            if (member is PropertyInfo)
                return (member as PropertyInfo).GetGetMethod().Invoke(instance, null);
            if (member is FieldInfo)
                return (member as FieldInfo).GetValue(instance);
            throw new NotImplementedException();
        }

        public override MethodHandler GetDelegate(MethodBase method)
        {
            return (instance, args) => method.Invoke(instance, args);
        }
    }

    delegate object MethodHandler(object target, params object[] args);

    interface IReflectionProvider
    {
        T GetSingleAttributeOrDefault<T>(MemberInfo memberInfo) where T : Attribute, new();
        IEnumerable<MemberInfo> GetSerializableMembers(Type type);
        object GetValue(MemberInfo member, object instance);
        MethodHandler GetDelegate(MethodBase method);
    }

    static class ReflectionHelper
    {
        public static readonly BindingFlags PublicInstanceMembers = BindingFlags.Public | BindingFlags.Instance;

        public static Type GetMemberType(this MemberInfo member)
        {
            if (member is PropertyInfo)
                return (member as PropertyInfo).PropertyType;
            if (member is FieldInfo)
                return (member as FieldInfo).FieldType;
            throw new NotImplementedException();
        }

        public static bool IsHashSet(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>);
        }

        public static bool IsGenericList(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static bool IsGenericCollection(this Type type)
        {
            return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        public static bool IsGenericDictionary(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsHashTable(Type type)
        {

            return type == typeof(Hashtable);
        }
    }

    /// <summary>
    /// Qualifies the serialization of a public property or field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    class SerializationAttribute : Attribute
    {
        /// <summary>
        /// Override the serialized name of this value.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ignore this value when serializing.
        /// </summary>
        public bool Ignore { get; set; }

        public bool Required { get; set; }
    }

    class Serializer
    {
        readonly IReflectionProvider _reflectionProvider;
        protected XmlDocument _xmlDoc;

        public Serializer(IReflectionProvider reflectionProvider)
        {
            this._reflectionProvider = reflectionProvider;
        }

        public string Serialize(object instance)
        {
            MemoryStream resultadoSerializacao = new MemoryStream();

            _xmlDoc = new XmlDocument();
            _xmlDoc.AppendChild(SerializeInternal(instance.GetType().Name, instance, typeof(object)));            
            _xmlDoc.Save(resultadoSerializacao);

            StreamReader reader = new StreamReader(resultadoSerializacao);
            resultadoSerializacao.Position = 0;
            return reader.ReadToEnd();

        }

        public string Serialize(object instance, string rootName)
        {
            MemoryStream resultadoSerializacao = new MemoryStream();

            _xmlDoc = new XmlDocument();
            _xmlDoc.AppendChild(SerializeInternal(rootName, instance, typeof(object)));
            _xmlDoc.Save(resultadoSerializacao);

            StreamReader reader = new StreamReader(resultadoSerializacao);
            resultadoSerializacao.Position = 0;
            return reader.ReadToEnd();

        }
      
        /// <summary>
        /// Método para efetuar a serialização de um determinado objeto, retornando uma string com estes dados.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="instance"></param>
        /// <param name="declaredType"></param>
        /// <returns></returns>
        private XmlNode SerializeInternal(string name, object instance, Type declaredType)
        {

            Type type = instance == null ? declaredType : instance.GetType();
            XmlElement xmlItem;

            // Valores Atômicos
            if (instance == null || IsAtomic(type))
            {
                string itemName = name ?? "Valor";
                string itemValue = instance == null ? "null" : instance.ToString();
                xmlItem = _xmlDoc.CreateElement(XmlConvert.EncodeName(itemName));
                xmlItem.InnerText = RemoveInvalidXmlChars(itemValue);
                if (instance != null)
                {
                    var attr = _xmlDoc.CreateAttribute("Tipo");
                    attr.Value = instance.GetType().Name;
                    xmlItem.Attributes.Append(attr);
                }
            }

            else if (instance is DbParameterCollection)            
            {                
                string itemName = name ?? "Valor";
                xmlItem = _xmlDoc.CreateElement(XmlConvert.EncodeName(itemName));
                if (instance != null)
                {
                    DbParameterCollection parameters = instance as DbParameterCollection;
                    foreach (DbParameter parameter in parameters)
                    {
                        Type parameterType = typeof(object);
                        if (parameter.Value != null) parameterType = parameter.Value.GetType();
                        xmlItem.AppendChild(SerializeInternal(parameter.ParameterName, parameter.Value, parameterType));
                    }
                }
            }

            //Coleção de Parâmetros
            else if (instance is IParameterCollection)
            {
                var collection = instance as IParameterCollection;
                xmlItem = _xmlDoc.CreateElement(XmlConvert.EncodeName(name ?? "Valor"));

                for (Int32 iParametro = 0; iParametro < collection.Count; iParametro++)
                {
                    ParameterInfo info = collection.GetParameterInfo(iParametro);
                    xmlItem.AppendChild(SerializeInternal(info.Name, collection[iParametro], typeof(Object)));
                }
            }
     
            // Dictionaries
            else if (type.IsGenericDictionary())
            {
                IDictionary dictionary = instance as IDictionary;
                Type[] genericArguments = dictionary.GetType().GetGenericArguments();
                Type keyDeclaredType = genericArguments[0];
                Type valueDeclaredType = genericArguments[1];

                // Inicia Tag referente a coleção
                xmlItem = _xmlDoc.CreateElement(XmlConvert.EncodeName(name));
                xmlItem.SetAttribute("TipoChave", keyDeclaredType.Name);
                xmlItem.SetAttribute("TipoValor", valueDeclaredType.Name);
                foreach (object key in dictionary.Keys)
                {
                    XmlElement xmlSubItem = _xmlDoc.CreateElement("Item");
                    XmlElement xmlChave = _xmlDoc.CreateElement("Chave");
                    XmlElement xmlValor = _xmlDoc.CreateElement("Valor");

                    string itemName = "";
                    string itemValue = "";

                    if (!IsAtomic(keyDeclaredType))
                        xmlChave.AppendChild(SerializeInternal(null, key, keyDeclaredType));
                    else
                    {
                        itemName = key == null ? "null" : key.ToString();
                        xmlChave.InnerText = itemName;
                    }

                    if (!IsAtomic(valueDeclaredType))
                        xmlValor.AppendChild(SerializeInternal(null, dictionary[key], valueDeclaredType));
                    else
                    {
                        itemValue = dictionary[key] == null ? "null" : dictionary[key].ToString();
                        xmlValor.InnerText = RemoveInvalidXmlChars(itemValue);
                    }
                    xmlSubItem.AppendChild(xmlChave);
                    xmlSubItem.AppendChild(xmlValor);
                    xmlItem.AppendChild(xmlSubItem);
                }
            }

            // Hashtable
            else if (ReflectionHelper.IsHashTable(type))
            {
                xmlItem = _xmlDoc.CreateElement(XmlConvert.EncodeName(name ?? "Valor"));
                Hashtable hashTable = instance as Hashtable;

                // Inicia Tag referente a coleção
                foreach (object key in hashTable.Keys)
                {
                    XmlElement xmlSubItem = _xmlDoc.CreateElement("Item");
                    XmlElement xmlChave = _xmlDoc.CreateElement("Chave");
                    xmlChave.SetAttribute("Tipo", key.GetType().ToString());
                    XmlElement xmlValor = _xmlDoc.CreateElement("Valor");
                    xmlValor.SetAttribute("Tipo", hashTable[key].GetType().ToString());

                    string itemName = "";
                    string itemValue = "";

                    if (!IsAtomic(key.GetType()))
                        xmlChave.AppendChild(SerializeInternal(null, key, key.GetType()));
                    else
                    {
                        itemName = key == null ? "null" : key.ToString();
                        xmlChave.InnerText = itemName;
                    }

                    if (!IsAtomic(hashTable[key].GetType()))
                        xmlValor.AppendChild(SerializeInternal(null, hashTable[key], hashTable[key].GetType()));
                    else
                    {
                        itemValue = hashTable[key] == null ? "null" : hashTable[key].ToString();
                        xmlValor.InnerText = RemoveInvalidXmlChars(itemValue);
                    }

                    xmlSubItem.AppendChild(xmlChave);
                    xmlSubItem.AppendChild(xmlValor);
                    xmlItem.AppendChild(xmlSubItem);

                }
            }

            // Arrays, lists e sets (qualquer colecao exceto Dictiorary e Hashtable)
            else if (type.IsGenericCollection())
            {
                IEnumerable collection = instance as IEnumerable;
                Type declaredItemType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];

                // Inicia Tag referente a coleção
                xmlItem = _xmlDoc.CreateElement(XmlConvert.EncodeName(name ?? "Valor"));
                xmlItem.SetAttribute("Tipo", declaredItemType.Name);
                if (declaredItemType == typeof(Byte))
                {
                    xmlItem.SetAttribute("Tamanho", collection.Cast<Byte>().Count().ToString());
                }
                else
                {
                    foreach (object item in collection)               
                        xmlItem.AppendChild(SerializeInternal(null, item, declaredItemType));
                }
            }
           
            // Demais classes - Serializadas as propriedades ate o nivel de valores atomicos
            else
            {
                xmlItem = _xmlDoc.CreateElement(XmlConvert.EncodeName(name ?? "Item"));

                foreach (MemberInfo memberInfo in _reflectionProvider.GetSerializableMembers(type))
                {
                    SerializationAttribute memberAttr = _reflectionProvider.GetSingleAttributeOrDefault<SerializationAttribute>(memberInfo);

                    if (memberAttr.Ignore)
                        continue;

                    Type memberType = memberInfo.GetMemberType();
                    object value = _reflectionProvider.GetValue(memberInfo, instance);

                    // Se a propriedade nao possuir nome definido pelo atributo, 
                    // utiliza o nome do item
                    string memberName = memberAttr.Name ?? memberInfo.Name;

                    xmlItem.AppendChild(SerializeInternal(memberName, value, memberType));
                }

            }

            return xmlItem;
        }

        /// <summary>
        /// Verifica se o type corresponde a um valor atomico, sem possibilitar sua decomposição
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsAtomic(Type type)
        {
            return type.IsPrimitive || type.IsEnum || type == typeof(string) 
                || type.IsNullable() || type == typeof(DateTime) || type == typeof(decimal) 
                || type == typeof(double) || type == typeof(Single) || type == typeof(Guid);
        }

        /// <summary>
        /// Remove os caracteres inválidos em um XML de uma String
        /// </summary>
        /// <param name="text">String representando o texto a ser analisado</param>
        /// <returns>String sem caracteres XML inválidos</returns>
        public static String RemoveInvalidXmlChars(String text)
        {
            StringBuilder textOut = new StringBuilder();
            Char current;

            if (text == null || text == String.Empty) return String.Empty;
            for (Int32 i = 0; i < text.Length; i++)
            {
                current = text[i];
                if ((current == 0x9 || current == 0xA || current == 0xD) ||
                    ((current >= 0x20) && (current <= 0xD7FF)) || ((current >= 0xE000) && (current <= 0xFFFD)))
                    textOut.Append(current);
            }

            return textOut.ToString();
        }
    }   
}
