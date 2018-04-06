using System;
using System.Collections.Generic;

namespace Redecard.Portal.Helper.Internacionalizacao
{
    /// <summary>
    /// Objeto com propriedades úteis principais de idioma
    /// </summary>
    public struct Idioma
    {
        public string Titulo;
        public uint Codigo;
        public string Rotulo;
    }

    /// <summary>
    /// Listagem de idiomas utilizados no site (Variations)
    /// </summary>
    public enum Idiomas : uint
    {
        Portugues = 1046,
        Ingles_EUA = 1033
    }

    /// <summary>
    /// Classe utilitária para obtenção de dados de internacionalização
    /// </summary>
    public static class InternacionalUtils
    {
        private static IDictionary<Idiomas, Idioma> idiomas;

        /// <summary>
        /// Construtor
        /// Carregamento do dicionário de idiomas utilizados no site
        /// </summary>
        static InternacionalUtils()
        {
            InternacionalUtils.idiomas = new Dictionary<Idiomas, Idioma>();
            InternacionalUtils.idiomas[Idiomas.Portugues] = new Idioma() { Codigo = 1046, Rotulo = "pt-BR", Titulo = "Português" };
            InternacionalUtils.idiomas[Idiomas.Ingles_EUA] = new Idioma() { Codigo = 1033, Rotulo = "en-US", Titulo = "English" };
        }

        /// <summary>
        /// Obtém, com base num item de lista ad enum Idiomas, um objeto idioma
        /// </summary>
        /// <param name="idioma"></param>
        /// <returns></returns>
        public static Idioma ObterDadosIdioma(Idiomas idioma)
        {
            return InternacionalUtils.idiomas[idioma];
        }
    }
}
