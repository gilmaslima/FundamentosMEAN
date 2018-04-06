#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [12/07/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;
using AutoMapper;
using Redecard.PN.Comum;
using System.ServiceModel;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Serviço de consultas e solicitações de Material de Vendas
    /// </summary>
    public class EmissorServico : ServicoBase, IEmissorServico
    {
        /// <summary>
        /// Lista as últimas remessas enviadas para o estabelecimento
        /// </summary>
        public List<Cartao> ConsultarBancoEmissor(int codigoBin, out int codigoRetorno)
        {

            using (Logger Log = Logger.IniciarLog("Lista as últimas remessas enviadas para o estabelecimento"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Negocio.Emissor negocioEmissor = new Negocio.Emissor();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoBin});

                    List<Modelo.Cartao> cartoes = negocioEmissor.ConsultarBancoEmissor(codigoBin, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, cartoes });

                    List<Servicos.Cartao> _cartoes = new List<Servicos.Cartao>();

                    if (!object.ReferenceEquals(cartoes, null) && cartoes.Count > 0)
                    {
                        Mapper.CreateMap<Modelo.Cartao, Servicos.Cartao>();
                        cartoes.ForEach(delegate(Modelo.Cartao r)
                        {
                            _cartoes.Add(Mapper.Map<Servicos.Cartao>(r));
                        });
                    }
                    Log.GravarLog(EventoLog.FimServico, new { _cartoes });
                    return _cartoes;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
    }
}