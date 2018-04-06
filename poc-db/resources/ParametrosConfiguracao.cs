/*
© Copyright 2016 Rede S.A.
Autor : EMA
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    /// <summary>
    /// A class ParametrosConfiguracao é uma entidade da lista parâmetros de configurações.
    /// </summary>
    public class ParametrosConfiguracao : BaseEntity
    {
        /// <summary>
        /// Gets or Sets a Descricao da ParametrosConfiguracao.
        /// </summary>
        public String Descricao { get; set; }

        /// <summary>
        /// Gets or Sets a Ativo da ParametrosConfiguracao.
        /// </summary>
        public Boolean Ativo { get; set; }

        /// <summary>
        /// Gets or Sets a Name da ParametrosConfiguracao.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or Sets Valor da ParametrosConfiguracao.
        /// </summary>
        public String Valor { get; set; }

        /// <summary>
        /// Gets or Sets CanaisExibicao da ParametrosConfiguracao.
        /// </summary>
        public String CanaisExibicao { get; set; }

        /// <summary>
        /// Gets or Sets Categoria da ParametrosConfiguracao.
        /// </summary>
        public String Categoria { get; set; }
        
    }
}
