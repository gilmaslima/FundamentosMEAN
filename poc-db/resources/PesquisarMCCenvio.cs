/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 26/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.FMS.Servico.Modelo.Merchant
{
    /// <summary>
    /// Este componente publica a classe PesquisarMCCenvio, que expõe propriedades para manipular dados de envio de pesquisa de merchant category code.
    /// </summary>
    [DataContract]
    public class PesquisarMCCenvio
    {
        [DataMember]
        public string NumeroEmissor {get; set;}
        [DataMember]
        public int GrupoEntidade { get; set; }
        [DataMember]
        public string UsuarioLogin { get; set; }
        [DataMember]
        public long? CodigoMCC { get; set; }
        [DataMember]
        public string DescricaoMCC { get; set; }
        [DataMember]
        public int PosicaoPrimeiroRegistro { get; set; }
        [DataMember]
        public int QuantidadeMaximaRegistros { get; set; }
        [DataMember]
        public bool RenovarContador { get; set; }
    }
}