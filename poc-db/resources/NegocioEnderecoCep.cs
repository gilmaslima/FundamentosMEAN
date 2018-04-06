/*
© Copyright 2017 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Text;
using Rede.PN.AtendimentoDigital.Core;
using Rede.PN.AtendimentoDigital.Modelo.Agente;
using Rede.PN.AtendimentoDigital.Modelo.Excecao;
using Rede.PN.AtendimentoDigital.Modelo.Entidades;

namespace Rede.PN.AtendimentoDigital.Negocio
{
    /// <summary>
    /// Negocio para Rotina de Endereco.
    /// </summary>
    public class NegocioEnderecoCep
    {
        //Agente Responsável pela comunicação com WSDR (Servico para endereço)
        private readonly IAgenteEnderecoCep _agenteCEP;

        /// <summary>
        /// Construtor Injeção de Depend~encia.
        /// </summary>
        public NegocioEnderecoCep() : this(GeradorObjeto.Obter<IAgenteEnderecoCep>()) { }

        /// <summary>
        /// Construtor.
        /// </summary>
        public NegocioEnderecoCep(IAgenteEnderecoCep agenteCEP)
        {
            _agenteCEP = agenteCEP;
        }

        /// <summary>
        /// Busca o Endereco (DRWS) para o cep informado.
        /// </summary>
        /// <param name="cep"> CEP de possível endereço.</param>
        public EntidadeEnderecoCep BuscarEnderecoPorCep(String cep)
        {
            StringBuilder erros = new StringBuilder();

            if (String.IsNullOrEmpty(cep) || String.IsNullOrWhiteSpace(cep))
                erros.AppendLine("Deve Ser informado um cep.");

            if (erros.Length > 0)
                throw new ExcecaoNegocio(erros.ToString());

            return _agenteCEP.BuscarEnderecoPorCep(cep);
        }
    }
}