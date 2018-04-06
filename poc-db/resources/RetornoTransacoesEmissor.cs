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

namespace Redecard.PN.FMS.Servico.Modelo.Transacoes
{
    /// <summary>
    /// Este componente publica a classe RetornoTransacoesEmissor, que expõe propriedades para manipular dados de retorno de transações por emissor. 
    /// </summary>
    [DataContract]
    public class RetornoTransacoesEmissor
    {
        [DataMember]
        public List<TransacaoEmissor> ListaTransacoesEmissor { get; set; }
        [DataMember]
        public long QuantidadeTotalRegistros { get; set; }
        [DataMember]
        public int QuantidadeHorasRecuperadas { get; set; }
        [DataMember]
        public int QuantidadeHorasTotalPeriodo { get; set; }
        [DataMember]
        public CriterioOrdemTransacoesAgrupadasPorEstabelecimento Criterio { get; set; }
    }
}
