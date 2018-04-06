using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.MultivanAlelo.Sharepoint.ControlTemplates.Voucher
{
    /// <summary>
    /// Classe de Modelo para Solicitações de Voucher
    /// </summary>
    public class SolicitacaoVoucher
    {
        /// <summary>
        /// Data e Hora da Solicitação
        /// </summary>
        public DateTime DataHora { get; set; }

        /// <summary>
        /// CPF/CNPJ do Credenciamento
        /// </summary>
        public Int64 CpfCnpj { get; set; }

        /// <summary>
        /// Tipo do Credenciamento do Voucher
        /// </summary>
        public String TipoCredenciamento { get; set; }

        /// <summary>
        /// Bandeira do Credenciamento do Voucher
        /// </summary>
        public String Bandeira { get; set; }

        /// <summary>
        /// Status da Solicitação
        /// </summary>
        public String Status { get; set; }

        /// <summary>
        /// Número Estabelecimento
        /// </summary>
        public Int64 NumeroEstabelecimento { get; set; }
    }
}
