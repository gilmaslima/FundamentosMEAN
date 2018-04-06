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
    /// Este componente publica a classe PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginEnvio, que expõe propriedades para manipular dados de envio da pesquisa de transações suspeitas por emissor e usuário logado.
    /// </summary>
    [DataContract]
    public class PesquisarTransacoesSuspeitasPorEmissorEUsuarioLoginEnvio
    {
        [DataMember]
        public int NumeroEmissor { get; set; }

        [DataMember]
        public int GrupoEntidade { get; set; }

        [DataMember]
        public string UsuarioLogin { get; set; }

        [DataMember]
        public int PosicaoPrimeiroRegistro { get; set; }

        [DataMember]
        public int QuantidadeRegistros { get; set; }

        [DataMember]
        public bool RenovarContador { get; set; }

        [DataMember]
        public CriterioOrdemTransacoesSuspeitasPorEmissorEUsuarioLogin Criterio {get;set;}

        [DataMember]
        public OrdemClassificacao Ordem {get;set;}

        [DataMember]
        public IndicadorTipoCartao TipoCartao { get; set; }
    }
}