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
    /// Este componente publica a classe TransacoesEmissorRetornoComQuantidade, que expõe propriedades para manipular dados de retorno de transações por emissor com quantidade.
    /// </summary>
    [DataContract]
    public class TransacoesEmissorRetornoComQuantidade
    {
        [DataMember]
        public List<TransacaoEmissor> ListaTransacoes { get; set; }
        [DataMember]
        public long QuantidadeRegistros { get; set; }

        public TransacoesEmissorRetornoComQuantidade()
        {
            QuantidadeRegistros = -1;
        }
    }
}
