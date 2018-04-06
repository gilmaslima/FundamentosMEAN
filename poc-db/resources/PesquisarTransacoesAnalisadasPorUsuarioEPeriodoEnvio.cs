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

namespace Redecard.PN.FMS.Servico.Modelo
{
    /// <summary>
    /// Este componente publica a classe PesquisarTransacoesAnalisadasPorUsuarioEPeriodoEnvio, que expõe propriedades para manipular dados de envio da pesquisa de transações analisadas por usuário e período.
    /// </summary>
    [DataContract]
    public class PesquisarTransacoesAnalisadasPorUsuarioEPeriodoEnvio
    {
        [DataMember]
        public int NumeroEmissor {get;set;}
        [DataMember]
        public int GrupoEntidade { get; set; }
        [DataMember]
        public string UsuarioLogin { get; set; }
        [DataMember]
        public string Usuario { get; set; }
        [DataMember]
        public DateTime DataInicial { get; set; }
        [DataMember]
        public DateTime DataFinal { get; set; }
        [DataMember]
        public int PosicaoPrimeiroRegistro { get; set; }
        [DataMember]
        public int QuantidadeRegistros { get; set; }
        [DataMember]
        public bool RenovarContador { get; set; }
        [DataMember]
        public CriterioOrdemTransacoesAnalisadasPorUsuarioEPeriodo CriterioOrdenacao { get; set; }
        [DataMember]
        public OrdemClassificacao Ordem { get; set; }
        [DataMember]
        public IndicadorTipoCartao TipoCartao { get; set; }
    }
}