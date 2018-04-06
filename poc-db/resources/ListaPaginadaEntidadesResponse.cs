using Redecard.PN.DadosCadastrais.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Lista de Entidades Paginada
    /// </summary>
    public class ListaPaginadaEntidadesResponse : ResponseBaseList<EntidadeServicoModel>
    {
        /// <summary>
        /// Total de linhas das busca toda
        /// </summary>
        public Int32 TotalRows { get; set; }

        /// <summary>
        /// Quantidade de E-mails Por Cpf
        /// </summary>
        public Int32 QuantidadeEmailsPorCpf { get; set; } 

        /// <summary>
        /// Indica se usuários do e-mail consultado possuem senhas iguais
        /// </summary>
        public Boolean? PVsSenhasIguais { get; set; }
    }
}