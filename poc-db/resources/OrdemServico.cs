/*
© Copyright 2014 Rede S.A.
Autor : Lucas Uehara
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.OrdemServico
{
    /// <summary>
    /// Ordem de serviço
    /// </summary>
    public class OrdemServico
    {
        /// <summary>
        /// Endereço de atendimento
        /// </summary>
        public EnderecoOs EnderecoAtendimento { get; set; }

        /// <summary>
        /// Contato
        /// </summary>
        public ContatoOs Contato { get; set; }

        /// <summary>
        /// Horário de atendimento
        /// </summary>
        public List<HorarioOs> HorarioAtendimento { get; set; }

        /// <summary>
        /// Número lógico
        /// </summary>
        public String NumeroLogico { get; set; }

        /// <summary>
        /// Tipo de equipamento
        /// </summary>
        public String TipoEquipamento { get; set; }

        /// <summary>
        /// Problema encontrado
        /// </summary>
        public String ProblemaEncontrado { get; set; }
    }
}
