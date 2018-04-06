using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    /// <summary>
    /// Classe Modelo de Distribuidor Fornecedor 
    /// </summary>
    public class DistribuidorFornecedor
    {
        /// <summary>
        /// Entidade Fornecedor do Distribuidor
        /// </summary>
        public Entidade EntidadeFornecedor { get; set; }

        /// <summary>
        /// Entidade do Distribuidor
        /// </summary>
        public Entidade EntidadeDistribuidor { get; set; }

        /// <summary>
        /// Código do Grupo do Fornecedor
        /// </summary>
        public Int32 CodigoGrupoFornecedor { get; set; }

        /// <summary>
        /// Pacote de Taxa de Honorário do Distribuidor Fornecedor 
        /// </summary>
        public Double PacoteTaxaHonorario { get; set; }

        /// <summary>
        /// Descrição do Distribuidor
        /// </summary>
        public String DescricaoDistribuidor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DataUltimaAtualizacao { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String ResponsavelAtualizacao { get; set; }
    }
}
