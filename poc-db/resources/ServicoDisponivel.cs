/*
 © Copyright 2017 Rede S.A.
   Autor : Rodrigo Coelho - rodrigo.oliveira@iteris.com.br
   Empresa : Iteris
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Rede.PN.ZeroDolar.Modelo {

    /// <summary>
    /// Classe de modelo para cada serviço disponível
    /// </summary>
    public class ServicoDisponivel {

        /// <summary>
        /// Nome do serviço
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Id do serviço
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Situação do serviço
        /// </summary>
        public char Situacao { get; set; }

        /// <summary>
        /// Código do serviço utilizado para contratação e cancelamento
        /// </summary>
        public int CodigoServico { get; set; }


        /// <summary>
        /// Data de contratação do serviço
        /// </summary>
        public DateTime? DataContratacao { get; set; }

        /// <summary>
        /// Obtem a descrição do status do serviço
        /// </summary>
        /// <param name="codigoSituacao"></param>
        /// <returns></returns>
        public string ObterDescricaoSituacao() {
            if (Situacao == 'A') return "Ativado";
            if (Situacao == 'R') return "Ativado";
            if (Situacao == 'C') return "Desativado";
            if (Situacao == 'D') return "Disponível";
            return string.Empty;
        }

    }
}
