/*
© Copyright 2017 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Rede.PN.AtendimentoDigital.Modelo.Entidades
{
    /// <summary>
    /// Responsável pelo tratamento de informações de Comunicação com outros sistemas
    /// </summary>
    public class EntidadeBase
    {
        /// <summary>
        /// Codigo de Retorno da Comunicação
        /// </summary>
        public Int32 CodigoRetorno { get; set; }

        /// <summary>
        /// Mensagem de Retorno da Comunicação
        /// </summary>
        public String Mensagem { get; set; }

        /// <summary>
        /// Fonte da Comunicação
        /// </summary>
        public String Fonte { get; set; }
    }
}
