using System;
using System.Collections.Generic;

namespace Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario
{
    /// <summary>
    /// Model para comunicação usando Ajax
    /// </summary>
    [Serializable]
    public class CriacaoUsuarioHandlerResponse : ConsultaPvsHandlerResponse
    {
        /// <summary>
        /// Define se deve ocultar o campo de CPF/CNPJ do sócio na tela
        /// </summary>
        public Boolean OcultarCpfSocio { get; set; }

        /// <summary>
        /// Define se é o primeiro usuário do PV
        /// </summary>
        public Boolean PrimeiroUsuarioPv { get; set; }
    }
}
