using System;

namespace Rede.PN.HealthCheck.Models
{
    /// <summary>
    /// Classe Modelo com informações de status dos sistemas parceiros
    /// </summary>
    public class SistemaParceiro
    {
        /// <summary>
        /// Sigla Parceira
        /// </summary>
        public String Sigla { get; set; }

        /// <summary>
        /// Indicador se o status do Sistema está ativo
        /// </summary>
        public Boolean StatusOK { get; set; }
    }
}