using System;

namespace Redecard.Portal.Aberto.WebParts.ParaSeuComputador
{
    /// <summary>
    /// Autor: Cristiano M. Dias
    /// Data criação: meados de 20/10/2010
    /// Classe que representa uma exceção que pode ser disparada para interromper o fluxo normal de um programa quando
    /// uma alguma biblioteca em consulta não é encontrada
    /// </summary>
    public sealed class BibliotecaInexistenteException : ApplicationException
    {
        public BibliotecaInexistenteException() : base() { }
        public BibliotecaInexistenteException(string mensagem) : base(mensagem) { }
        public BibliotecaInexistenteException(string mensagem, Exception excecao) : base(mensagem, excecao) { }
    }
}