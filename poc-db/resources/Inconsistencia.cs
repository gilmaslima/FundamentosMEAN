using System;

namespace Redecard.Portal.Helper.Validacao
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Descrição: Classe que representa um erro/inconsistência durante alguma validação qualquer
    /// É utilizada por exemplo pela classe de serviço MontadorMensagemErroUtil para criação de mensagens personalizadas
    /// A classe é imutável: uma vez instanciada, não pode ter seus atributos alterados. Para tal, cria-se um novo objeto Inconsistencia
    /// </summary>
    public class Inconsistencia
    {
        /// <summary>
        /// Nome do campo com a inconsistência
        /// </summary>
        public string Campo { get; private set; }

        /// <summary>
        /// Mensagem relacionada a inconsistência
        /// </summary>
        public string Mensagem { get; private set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        /// <param name="campo"></param>
        /// <param name="mensagem"></param>
        public Inconsistencia(string campo, string mensagem)
        {
            this.Campo = campo;
            this.Mensagem = mensagem;
        }
    }
}
