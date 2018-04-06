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
    /// Enum EditorType
    /// </summary>
    [Serializable]
    [DataContract]    
	public enum EditorType
	{
        [EnumMember]
		TextBox = 0,
		[EnumMember]
        DropDownBox = 1,
        [EnumMember]
        TextBoxArray = 2
	}
}
