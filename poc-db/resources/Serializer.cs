/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 27/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Reflection;
using System.Collections;

using System.Text;
using System.Xml;
using System.IO;

namespace Redecard.PN.FMS.Comum.Log
{
    /// <summary>
    /// Este componente publica a classe Serializer, que expõe métodos para manipular serialização do log.
    /// </summary>
    class Serializer
    {
        readonly IReflectionProvider _reflectionProvider;
        protected XmlDocument _xmlDoc;
        /// <summary>
        /// Construtor que utiliza reflection.
        /// </summary>
        /// <param name="reflectionProvider"></param>
        public Serializer(IReflectionProvider reflectionProvider)
        {
            this._reflectionProvider = reflectionProvider;
        }
        /// <summary>
        /// Construtor que utiliza instância.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public string Serialize(object instance)
        {
            MemoryStream resultadoSerializacao = new MemoryStream();

            _xmlDoc = new XmlDocument();
            _xmlDoc.AppendChild(SerializeInternal(instance.GetType().Name, instance, typeof(object)));
            _xmlDoc.Save(resultadoSerializacao);

            StreamReader reader = new StreamReader(resultadoSerializacao);
            resultadoSerializacao.Position= 0;
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

            // Valores Atomicos
            if (instance == null || IsAtomic(type))
            {
                string itemName = name??"Valor";
                string itemValue = instance == null ? "null" : instance.ToString();
                xmlItem = _xmlDoc.CreateElement(XmlConvert.EncodeName(itemName));
                xmlItem.InnerText = itemValue;
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
                        xmlValor.InnerText = itemValue;
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
                        xmlValor.InnerText = itemValue;
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
                foreach (object item in collection)
                {
                    xmlItem.AppendChild(SerializeInternal(null, item, declaredItemType));
                }
            }


            // Demais classes - Serializadas as propriedades ate o nivel de valores atomicos
            else
            {
                
                xmlItem = _xmlDoc.CreateElement(XmlConvert.EncodeName(name??"Item"));

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
            return type.IsPrimitive || type.IsEnum || type == typeof(string) || type.IsNullable() || type == typeof(DateTime) || type == typeof(decimal) || type == typeof(double) || type == typeof(Single);
        }
    }
}
