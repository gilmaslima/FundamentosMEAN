using System;
using System.Collections.Generic;

namespace Redecard.Portal.Helper.Validacao
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Data criação: meados de 20/09/2010
    /// Descrição: Classe que comporta objetos de erros para fins de relatório
    /// </summary>
    public class SumarioValidacao
    {
        private IList<Inconsistencia> inconsistencias = new List<Inconsistencia>();

        /// <summary>
        /// Retorna uma lista de objetos de erro
        /// </summary>
        public IList<Inconsistencia> Inconsistencias
        {
            get
            {
                return this.inconsistencias;
            }
        }

        /// <summary>
        /// Avalia a quantidade de itens na lista de inconsistências e retorna true caso não haja nenhuma
        /// </summary>
        public bool Valido
        {
            get
            {
                return this.inconsistencias.Count == 0;
            }
        }

        /// <summary>
        /// Adiciona um objeto InconsistÊncia à lista de inconsistências
        /// </summary>
        /// <param name="inconsistencia"></param>
        public void AdicionarInconsistencia(Inconsistencia inconsistencia)
        {
            if (inconsistencia == null)
                throw new ArgumentNullException("Objeto Inconsistência não pode ser nulo");

            this.inconsistencias.Add(inconsistencia);
        }
    }
}