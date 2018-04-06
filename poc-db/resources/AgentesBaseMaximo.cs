/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Linq;
using Redecard.PN.Comum;

namespace Redecard.PN.Maximo.Agentes
{
    /// <summary>
    /// Classe base de agentes do máximo
    /// </summary>
    public class AgentesBaseMaximo<Instancia, Classe> : AgentesBase<Instancia, Classe> where Classe : Instancia
    {        
        /// <summary>
        /// Construtor estático da base de Agentes
        /// </summary>
        static AgentesBaseMaximo()
        {
            /// Inicialização dos profiles AutoMapper.
            /// Carrega TODAS as classes Profile do Assembly Agentes (Mapeadores).
            /// Chamado dentro do construtor estático para garantir execução única do setup do AutoMapper.
            Mapeadores.Mapeador.Configurar();
        }

        /// <summary>
        /// Converte um faultcode do máximo para um código inteiro para ser utilizado no portal
        /// </summary>
        /// <param name="faultcode">faultcode</param>
        /// <remarks>
        /// Histórico: 15/08/2013 - Criação do método        
        /// </remarks>
        /// <returns>Retorna um código numérico. São considerados apenas números e letras.
        /// Letras são convertidas para maiúsculas, e utilizado o valor numérico de seu char.
        /// A: 65; Z: 90</returns>
        protected static Int32 ObterCodigo(String faultcode)
        {
            return String.Join(String.Empty, (faultcode ?? String.Empty).ToUpper()
                .Select(c =>
                {
                    if (Char.IsLetter(c))
                        return ((Int32)c).ToString("D2");
                    else if (Char.IsNumber(c))
                        return c.ToString();
                    else
                        return String.Empty;
                }).ToArray()).ToInt32(CODIGO_ERRO);
        }
    }
}