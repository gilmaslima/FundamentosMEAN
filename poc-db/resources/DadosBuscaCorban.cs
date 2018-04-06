using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrosServicos.SharePoint.Modelos
{
    /// <summary>
    /// Classe Serializável para armazenar dados da consulta
    /// </summary>
    [Serializable]
    public class DadosBuscaCorban
    {
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
        /// Código dos Tipos possíveis de Conta
        /// ---------------------------------
        /// 00 - TODOS
        /// 01 - TITULOS
        /// 02 - CONCESSIONARIA
        /// 03 - TRIBUTOS
        /// </summary>
        public Int16 CodigoTipoConta { get; set; }

        /// <summary>
        /// Código dos Tipos possíveis de Conta
        /// ---------------------------------
        /// 00 - TODOS
        /// 02 - DEBITO
        /// 03 - DINHEIRO
        /// </summary>
        public Int16 CodigoFormaPagamento { get; set; }

        /// <summary>
        /// Código dos Tipos possíveis de Conta
        /// ---------------------------------
        /// ' ' - TODOS
        /// 'C' - CONFIRMADA
        /// 'E' - ESTORNADA
        /// </summary>
        public Char CodigoStatusTransacao { get; set; }

        /// <summary>
        /// Guid de Pesquisa do filtro
        /// </summary>
        public Guid IdPesquisa { get; set; }

        /// <summary>
        /// Código do serviço
        /// </summary>
        public Decimal CodigoServico { get; set; }
    }
}
