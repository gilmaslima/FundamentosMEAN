using System;
using Redecard.PN.Comum;
using System.Linq;

namespace Redecard.PN.Extrato.SharePoint.Modelo
{
    [Serializable]
    public class BuscarDados : IBuscarDados
    {
        /// <summary>
        /// Código da bandeira
        /// </summary>
        public Int16 CodigoBandeira { get; set; }

        /// <summary>
        /// Data inicial da pesquisa
        /// </summary>
        public DateTime DataInicial { get; set; }

        /// <summary>
        /// Data final da pesquisa
        /// </summary>
        public DateTime DataFinal { get; set; }

        /// <summary>
        /// Identificador do Tipo de Estabelecimento
        /// ----------------------------------------
        /// Matriz              0
        /// Filiais             2
        /// Centralizados       1
        /// Consignados         3
        /// Mesmo CNPJ          4
        /// </summary>
        public Int32 TipoEstabelecimento { get; set; }

        /// <summary>
        /// Número dos PV's em que a pesquisa deve ser 
        /// realizada
        /// </summary>
        public Int32[] Estabelecimentos { get; set; }

        /// <summary>
        /// Pode ser um dentre os valores
        /// ---------------------------------
        /// Vendas                          0
        /// Valores Pagos                   1
        /// Ordens de Crédito               2
        /// Pagamentos Ajustados            3
        /// Lançamentos Futuros             4
        /// RAV (Antecipações)              5
        /// Débitos e Desagendamentos       6
        /// Débitos e Serviços              7
        /// Suspensos Penhorados Retidos    8
        /// </summary>
        public Int32 IDRelatorio { get; set; }

        /// <summary>
        /// Pode ser um dentre os valores
        /// ---------------------------------
        /// Crédito                         0
        /// Débito                          1
        /// Construcard                     2
        /// Todos                           3
        /// </summary>
        public Int32 IDTipoVenda { get; set; }

        /// <summary>
        /// Código da solicitaçaõ para pesquisa saldos Em Aberto Vsam
        /// </summary>
        public String CodigoSolicitacao { get; set; }

        /// <summary>
        /// Todos                0
        /// À Vista              1
        /// Pré-Datado           2
        /// Trishop              3
        /// Compre e Saque         4
        /// Parcele Mais         5
        /// Pagamento de Fatura  6
        /// </summary>
        public Int32 IDModalidade { get; set; }
    }
}
