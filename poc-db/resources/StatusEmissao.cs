using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.GerencieExtrato.Modelo
{
    public class StatusEmissao
    {
        /// <summary>
        /// Numero do pv
        /// </summary>
        public String PontoVenda { get; set; }
        /// <summary>
        /// Status da emissao do extrato papel
        /// 'E' - EMITIR,  'I' - INIBIR
        /// </summary>
        public String Status { get; set; }
        /// <summary>
        /// Indicador de situaç o de cobrança extrato
        /// 'E' - EMITIR,  'I' - INIBIR
        /// </summary>
        public String SituacaoCobranca { get; set; }
        /// <summary>
        /// CODIGO RETORNO DA PESQUISA DO PV
        /// 00 - OK
        /// 10 - NUMERO DO PV INVALIDO
        /// 23 - NAO ENCONTRADO
        /// </summary>
        public Int32 CodigoRetornoPV { get; set; }


    }
}
