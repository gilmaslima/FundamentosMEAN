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

namespace Redecard.PN.FMS.Servico.Modelo.CriterioSelecao
{
    /// <summary>
    /// Este componente publica a classe PesquisarRangeBinPorEmissorEnvio, que expõe propriedades para manipular dados de envio de pesquisa de range bin por emissor.
    /// </summary>
    [DataContract]
    public class PesquisarRangeBinPorEmissorEnvio
    {
        [DataMember]
        public int numeroEmissor {get;set;}
        [DataMember]
        public int grupoEntidade { get; set; }
        [DataMember]
        public string usuarioLogin { get; set; }
        [DataMember]
        public long ica { get; set; }
        [DataMember]
        public int posicaoPrimeiroRegistro { get; set; }
        [DataMember]
        public int quantidadeMaximaRegistro { get; set; }
        [DataMember]
        public bool renovarContador { get; set; }
    }
}