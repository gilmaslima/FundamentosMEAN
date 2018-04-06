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
    /// Este componente publica a classe PesquisarTransacoesAnalisadasPorUsuarioEPeriodoRetorno, que expõe propriedades para manipular dados de retorno da pesquisa de transações analisadas por usuário e período.
    /// </summary>
    [DataContract]
    public class PesquisarTransacoesAnalisadasPorUsuarioEPeriodoRetorno
    {

        [DataMember]
        public List<Redecard.PN.FMS.Servico.Modelo.TransacaoEmissor> ListaResposta { get; set; }

        [DataMember]
        public int QuantidadeRegistros { get; set; }
    }
}