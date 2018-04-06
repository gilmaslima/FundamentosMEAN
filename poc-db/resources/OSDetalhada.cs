/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Redecard.PN.Maximo.Modelo.OrdemServico
{
    /// <summary>
    /// Classe Modelo OSDetalhada
    /// </summary>
    [DataContract]
    public class OSDetalhada : OSEstendida
    {
        /// <summary>
        /// Contato alternativo
        /// </summary>
        [DataMember]
        public Contato ContatoAlternativo { get; set; }

        /// <summary>
        /// Cnpj
        /// </summary>
        [DataMember]
        public String Cnpj { get; set; }

        /// <summary>
        /// Evento
        /// </summary>
        [DataMember]
        public EventoOS Evento { get; set; }

        /// <summary>
        /// Terminal
        /// </summary>
        [DataMember]
        public List<OSTerminal> Terminal { get; set; }

        /// <summary>
        /// Ponto venda
        /// </summary>
        [DataMember]
        public PontoVenda PontoVenda { get; set; }

        /// <summary>
        /// Ação comercial
        /// </summary>
        [DataMember]
        public String AcaoComercial { get; set; }

        /// <summary>
        /// Origem
        /// </summary>
        [DataMember]
        public String Origem { get; set; }

        /// <summary>
        /// Canal
        /// </summary>
        [DataMember]
        public String Canal { get; set; }

        /// <summary>
        /// Célula
        /// </summary>
        [DataMember]
        public String Celula { get; set; }

        /// <summary>
        /// Rede
        /// </summary>
        [DataMember]
        public String Rede { get; set; }

        /// <summary>
        /// Observação
        /// </summary>
        [DataMember]
        public String Observacao { get; set; }

        /// <summary>
        /// Provedor serviço
        /// </summary>
        [DataMember]
        public String ProvedorServico { get; set; }

        /// <summary>
        /// Aluguel
        /// </summary>
        [DataMember]
        public Aluguel Aluguel { get; set; }

        /// <summary>
        /// Cenário
        /// </summary>
        [DataMember]
        public String Cenario { get; set; }
    }
}
