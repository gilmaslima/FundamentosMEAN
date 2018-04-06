using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Agentes
{
    /// <summary>
    /// Consulta do Tipo de Pessoa através de uma chamada de serviço do GE.
    /// </summary>
    public class ObterTipoPessoaGE : AgentesBase
    {
        /// <summary>
        /// Consulta serviço no GE para obter o tipo de pessoa.
        /// </summary>
        /// <param name="codigoEntidade">PV do usuário</param>
        /// <param name="codigoRetorno">Código de retorno do serviço</param>
        /// <returns>Char com o tipo de pessoa</returns>
        /// 
        /*  Códigos de Retorno:
            0  - OK
            1  - Retorno do serviço vazio
            2  - TipoPessoa nulo.
            99 - Exceção genérica
        */
        public Char ObterTipoPessoa(Int32 codigoEntidade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Chamada do Serviço PontoVenda do GE - Obtém TipoPessoa"))
            {
                try
                {
                    Char? tipoPessoa = null;
                    codigoRetorno = 0;
                    ServicoPortalGEPontoVenda.ListaCadastroPorPontoVenda[] pontosVenda = null;

                    Log.GravarLog(EventoLog.InicioAgente);

                    using (ServicoPortalGEPontoVenda.ServicoPortalGEPontoVendaClient client = new ServicoPortalGEPontoVenda.ServicoPortalGEPontoVendaClient())
                    {
                        Log.GravarLog(EventoLog.InicioAgente, new { codigoEntidade });
                        pontosVenda = client.ListaCadastroPorPontoVenda(codigoEntidade);

                        codigoRetorno = pontosVenda == null ? 1 : 0;
                        Log.GravarLog(EventoLog.RetornoAgente, new { codigoRetorno });

                    }

                    if (codigoRetorno == 0)
                    {
                        //Mapeamento de objeto de retorno HIS para modelo de negócio
                        var firstItemTipoVenda = pontosVenda.FirstOrDefault();

                        if (firstItemTipoVenda != null)
                        {
                            tipoPessoa = firstItemTipoVenda.CodTipoPessoa;
                        }

                        if (!tipoPessoa.HasValue)
                        {
                            codigoRetorno = 2;
                            tipoPessoa = Char.MinValue;
                        }
                    }
                    else
                    {
                        tipoPessoa = Char.MinValue;
                    }
                    Log.GravarLog(EventoLog.FimAgente, new { result = tipoPessoa });
                    return (Char)tipoPessoa;
                }
                catch (PortalRedecardException ex)
                {
                    codigoRetorno = 99;
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    codigoRetorno = 99;
                    Log.GravarErro(ex);
                    throw;
                }
            }
        }
    }
}
