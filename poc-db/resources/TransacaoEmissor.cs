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
using System.Text;
using System.Runtime.Serialization;

namespace Redecard.PN.FMS.Servico.Modelo
{
    /// <summary>
    /// Este componente publica a classe TransacaoEmissor, que expõe propriedades para manipular dados de transação por emissor. 
    /// </summary>
    [DataContract]
    public class TransacaoEmissor
    {
        [DataMember]
        public TipoAlarme TipoAlarme { get; set; }
        [DataMember]
        public DateTime DataEnvioAnalise { get; set; }
        [DataMember]
        public ResultadoAutorizacao ResultadoAutorizacao { get; set; }
        [DataMember]
        public string NumeroCartao { get; set; }
        [DataMember]
        public string Bandeira { get; set; }
        [DataMember]
        public TipoCartao TipoCartao { get; set; }
        [DataMember]
        public string EntryMode { get; set; }
        [DataMember]
        public string DescricaoEntryMode { get; set; }
        [DataMember]
        public SituacaoFraude SituacaoTransacao { get; set; }
        [DataMember]
        public string ComentarioAnalise { get; set; }
        [DataMember]
        public DateTime DataAnalise { get; set; }
        [DataMember]
        public string UsuarioAnalise { get; set; }
        [DataMember]
        public SituacaoBloqueio SituacaoBloqueio { get; set; }
        [DataMember]
        public long CodigoMCC { get; set; }
        [DataMember]
        public string DescricaoMCC { get; set; }
        [DataMember]
        public long CodigoEstabelecimento { get; set; }
        [DataMember]
        public string NomeEstabelecimento { get; set; }
        [DataMember]
        public int Score { get; set; }
        [DataMember]
        public string UnidadeFederacao { get; set; }
        [DataMember]
        public TipoResposta TipoResposta { get; set; }
        [DataMember]
        public decimal Valor { get; set; }
        [DataMember]
        public long IdentificadorTransacao { get; set; }
        [DataMember]
        public DateTime DataTransacao { get; set; }
    }
}
