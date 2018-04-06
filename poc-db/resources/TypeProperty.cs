/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Runtime.Serialization;
namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Enum TypeProperty
    /// </summary>
    [Serializable]
    [DataContract]
    public enum TypeProperty
	{   
        [EnumMember]
		Array = 0,
        [EnumMember]
        Collection = 1,
        [EnumMember]
        Composite = 2,
        [EnumMember]
        DataSet = 3,
        [EnumMember]
        Dictionary = 4,
        [EnumMember]
        Enum = 5,
        [EnumMember]
        KeyValuePair = 6,
        [EnumMember]
        Nullable = 7,
        [EnumMember]
        Numeric = 8,
        [EnumMember]
        Struct = 9,
        [EnumMember]
        Boolean = 10,
        [EnumMember]
        String = 11,
        [EnumMember]
        Char = 12,
        [EnumMember]
        Guid = 13,
        [EnumMember]
        DateTime = 14,
        [EnumMember]
        DateTimeOffSet = 15,
        [EnumMember]
        TimeSpan = 16,
        [EnumMember]
        SystemUri = 17,
        [EnumMember]
        NullField = 18,
        [EnumMember]
        DataTable = 19
	}
}
