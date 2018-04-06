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

namespace Redecard.PN.FMS.Servico.Modelo.Transacoes
{
    /// <summary>
    /// Este componente publica a classe PesquisarRelatorioProdutividadeEnvio, que expõe propriedades para manipular dados de relatório de produtividade.
    /// </summary>
    [DataContract]
    public class PesquisarRelatorioProdutividadeEnvio
    {
        [DataMember]
        public int NumeroEmissor {get; set;}
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
        public CriterioOrdemProdutividade Criterio { get; set; }
        [DataMember]
        public OrdemClassificacao Ordem { get; set; }
    }
}