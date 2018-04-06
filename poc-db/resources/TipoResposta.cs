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
using System.Text;
using System.Runtime.Serialization;

namespace Redecard.PN.FMS.Servico.Modelo
{
    /// <summary>
    /// Este componente publica a classe XXX, que expõe propriedades para manipular dados de tipo de resposta.
    /// </summary>
    [DataContract]
    public class TipoResposta
    {
        [DataMember]
        public Int64 CodigoResposta { get; set; }
        [DataMember]
        public string DescricaoResposta { get; set; }
        [DataMember]
        public string NomeResposta { get; set; }
        [DataMember]
        public SituacaoFraude SituacaoFraude { get; set; }
    }
}
