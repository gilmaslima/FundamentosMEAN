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
    /// Este componente publica a classe PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginRetorno, que expõe propriedades para manipular dados de retorno da pesquisa de transações suspeitas por emissor e usuário logado. 
    /// </summary>
    [DataContract]
    public class PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginRetorno
    {
        [DataMember]
        public List<TransacaoEmissor> ListaTransacoesEmissor { get; set; }
        [DataMember]
        public long QuantidadeTotalRegistros { get; set; }
        [DataMember]
        public int QuantidadeHorasRecuperadas { get; set; }
        [DataMember]
        public int QuantidadeHorasTotalPeriodo { get; set; }
    }
}