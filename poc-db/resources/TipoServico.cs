using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public enum TipoServico
    {
        [EnumMember]
        Desconhecido = 0,
        [EnumMember]
        Servico = 'S',
        [EnumMember]
        Pacote = 'P'
    }
}
