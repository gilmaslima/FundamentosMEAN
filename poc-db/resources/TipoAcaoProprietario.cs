using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{

    [DataContract]
    [Serializable]
    public enum TipoAcaoProprietario
    {
        [EnumMember]
        Desconhecido = 0,
        [EnumMember]
        Alterar = 'A',
        [EnumMember]
        Incluir = 'I',
        [EnumMember]
        Excluir = 'E'
    }

}
