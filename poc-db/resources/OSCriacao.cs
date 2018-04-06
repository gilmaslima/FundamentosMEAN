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
    /// Classe Modelo OSCriacao
    /// </summary>
    [DataContract]
    public class OSCriacao 
    {
        /// <summary>
        /// Classificação
        /// </summary>
        [DataMember]
        public TipoClassificacao Classificacao { get; set; }

        /// <summary>
        /// Ponto venda
        /// </summary>
        [DataMember]
        public PontoVenda PontoVenda { get; set; }

        /// <summary>
        /// Prioridade
        /// </summary>
        [DataMember]
        public TipoPrioridade Prioridade { get; set; }

        /// <summary>
        /// Data agendada
        /// </summary>
        [DataMember]
        public DateTime? DataAgendada { get; set; }

        /// <summary>
        /// Número lógico
        /// </summary>
        [DataMember]
        public String NumeroLogico { get; set; }

        /// <summary>
        /// Tipo equipamento
        /// </summary>
        [DataMember]
        public String TipoEquipamento { get; set; }

        /// <summary>
        /// Aluguek
        /// </summary>
        [DataMember]
        public Aluguel Aluguel { get; set; }

        /// <summary>
        /// Endereço atendimento
        /// </summary>
        [DataMember]
        public Endereco EnderecoAtendimento { get; set; }

        /// <summary>
        /// Horário atendimento
        /// </summary>
        [DataMember]
        public List<Horario> HorarioAtendimento { get; set; }

        /// <summary>
        /// Contato
        /// </summary>
        [DataMember]
        public Contato Contato { get; set; }

        /// <summary>
        /// Cenário
        /// </summary>
        [DataMember]
        public String Cenario { get; set; }

        /// <summary>
        /// Evento
        /// </summary>
        [DataMember]
        public EventoOS Evento { get; set; }

        /// <summary>
        /// Venda digitada
        /// </summary>
        [DataMember]
        public VendaDigitada VendaDigitada { get; set; }

        /// <summary>
        /// Renpac
        /// </summary>
        [DataMember]
        public String Renpac { get; set; }

        /// <summary>
        /// Software House
        /// </summary>
        [DataMember]
        public String SoftwareHouse { get; set; }

        /// <summary>
        /// Integrador
        /// </summary>
        [DataMember]
        public String Integrador { get; set; }

        /// <summary>
        /// Ação comercial
        /// </summary>
        [DataMember]
        public String AcaoComercial { get; set; }

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
    }
}
