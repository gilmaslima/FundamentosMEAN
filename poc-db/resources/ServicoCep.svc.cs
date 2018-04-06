/*
© Copyright 2017 Rede S.A.
Autor : Mário neto
Empresa : Iteris Consultoria e Software
*/

using AutoMapper;
using System;
using Rede.PN.AtendimentoDigital.Modelo.Entidades;
using Rede.PN.AtendimentoDigital.Negocio;
using Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoDados.Response;
using Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoDados.Response.EnderecoCepResponse;
using Rede.PN.AtendimentoDigital.Servicos.Contrato.ContratoServico;

namespace Rede.PN.AtendimentoDigital.Servicos.Servicos
{
    /// <summary>
    /// Classe de serviço do Favorito.
    /// </summary>
    public class ServicoCep : ServicoBase, IServicoCep
    {
        #region [Inicialização]
        /// <summary>
        /// Define se o mapeamento foi realizado.
        /// </summary>
        private static readonly Boolean mapeamentoRealizado;

        /// <summary>
        /// Construtor.
        /// </summary>
        static ServicoCep()
        {
            if (!mapeamentoRealizado)
            {
                Mapper.CreateMap<EntidadeEnderecoCep, EnderecoCepResponse>();
//                Mapper.CreateMap<EntidadeEnderecoCep, EnderecoCepResponseItem>();
                mapeamentoRealizado = true;
            }
        }

        #endregion [Inicialização]

        /// <summary>
        /// Obtem endereco completo por CEP informado.
        /// </summary>
        /// <param name="cep">cep</param>
        public EnderecoCepResponseItem BuscarEnderecoPorCep(String cep)
        {
            return this.ExecucaoTratada<EnderecoCepResponseItem>(retorno =>
            {
                NegocioEnderecoCep negocio = new NegocioEnderecoCep();

                EntidadeEnderecoCep item = negocio.BuscarEnderecoPorCep(cep);

                retorno.Item = item.CodigoRetorno == 0 ? Mapper.Map<EnderecoCepResponse>(item) : null;
                retorno.Mensagem = item.Mensagem;
                retorno.StatusRetorno = item.CodigoRetorno == 0 ? StatusRetorno.OK : StatusRetorno.ErroNegocio;

            });
        }
    }
}