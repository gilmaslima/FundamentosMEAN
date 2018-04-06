using System;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    /// <summary>
    /// Estrutura de dados da agência, usado na Confirmação Positiva para recuperar o
    /// nome de uma agência
    /// </summary>
    public class Agencia
    {
        /// <summary>
        /// 
        /// </summary>
        public String NomeAgencia { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String Cidade { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String Estado { get; set; }
    }
}
