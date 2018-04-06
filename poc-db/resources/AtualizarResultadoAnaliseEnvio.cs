/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 27/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.FMS.Servico.Modelo.Transacoes
{
    /// <summary>
    /// Este componente publica a classe AtualizarResultadoAnaliseEnvio, que expõe propriedades para manipular dados de  envio da atualização de resultado da análise.
    /// </summary>
    [DataContract]
    public class AtualizarResultadoAnaliseEnvio
    {
        [DataMember]
        public List<RespostaAnaliseItem> ListaRespostaAnalise { get; set; }
        [DataMember]
        public Redecard.PN.FMS.Servico.Modelo.IndicadorTipoCartao TipoCartao { get; set; }
    }
}