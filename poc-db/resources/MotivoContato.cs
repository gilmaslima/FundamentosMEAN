using System;

namespace Redecard.Portal.Helper.DTO
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Data criação: meados de 20/09/2010
    /// Descrição: Classe que representa um Motivo de Contato.
    /// Útil para operações de binding com controles de listas através de lista de MotivoContato
    /// </summary>
    public class MotivoContato
    {
        /// <summary>
        /// Item que descreve um Motivo de Contato
        /// </summary>
        public string Descricao { get; private set; }

        /// <summary>
        /// Endereço para um e-mail é enviado quando o motivo atual é selecionado.
        /// Opcional.
        /// </summary>
        public string EmailDestinatario { get; private set; }

        /// <summary>
        /// Construtor padrão do DTO.
        /// </summary>
        /// <param name="descricao"></param>
        /// <param name="emailDestinatario"></param>
        public MotivoContato(string descricao, string emailDestinatario)
        {
            this.Descricao = descricao;
            this.EmailDestinatario = emailDestinatario;
        }
    }
}