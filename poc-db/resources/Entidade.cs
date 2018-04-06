using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais.Modelo
{
    /// <summary>
    /// Representa uma entidade processada pelo serviço
    /// </summary>
    public class Entidade
    {
        /// <summary>
        /// Identifica se a entidade já foi processada no Credenciamento no dia
        /// </summary>
        [XmlAttribute]
        public Boolean Processada { get; set; }

        /// <summary>
        /// Data e Hora do Processamento
        /// </summary>
        [XmlAttribute]
        public List<DateTime> DataHoraProcessamentos {get; set; }

        /// <summary>
        /// Código do grupo entidade da entidade atual
        /// </summary>
        [XmlAttribute]
        public Int32 CodigoGrupoEntidade { get; set; }

        /// <summary>
        /// Código da entidade atual
        /// </summary>
        [XmlAttribute]
        public Int32 CodigoEntidade { get; set; }

        /// <summary>
        /// Fluxo de processamento para a Entidade
        /// </summary>
        [XmlAttribute]
        public List<Int32> FluxosProcessamento { get; set; }

        /// <summary>
        /// Email de envio de senha
        /// </summary>
        [XmlAttribute]
        public String Email { get; set; }

        /// <summary>
        /// Razão Social da Entidade
        /// </summary>
        [XmlAttribute]
        public String RazaoSocial { get; set; }

        /// <summary>
        /// Identifica se a entidade possui Komerci
        /// </summary>
        [XmlAttribute]
        public Boolean PossuiKomerci { get; set; }

        /// <summary>
        /// Usuário a ser cadastrado para a Entidade
        /// </summary>
        public Usuario Usuario { get; set; }
    }
}
