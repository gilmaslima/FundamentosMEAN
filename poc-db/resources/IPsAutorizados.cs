/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – Rodrigo Locoseli – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.FMS.Modelo.CadastroIPs
{
    /// <summary>
    /// DTO para o cadastro de IPs
    /// 
    /// aceito IPv4 e IPv6.
    /// 
    /// IPv4: 192.1168.100.255
    /// IPv6: 2001:0db8:85a3:08d3:1319:8a2e:0370:7344
    /// 
    /// </summary>
    public class IPsAutorizados
    {    
        public long CdIPAssociado { get; set; }
        public string NumeroIP { get; set; }
        public bool EhPassivelValidacaoIP { get; set; }
    }
}