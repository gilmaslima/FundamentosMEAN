using Redecard.PN.Comum;
using System;
using System.Runtime.CompilerServices;

/// <summary>
/// Define a classe base de implementação dos serviços REST.
/// </summary>
namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Define a classe base de implementação dos serviços.
    /// </summary>
    public class ServicoRestBase : ServicoBase
    {
        /// <summary>
        /// Método que encapsula os catchs.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public TResult ExecucaoTratada<TResult>(String operationDescription, Action<TResult> action)
            where TResult : BaseResponse, new()
        {
            TResult retorno = new TResult();
            using (Logger Log = Logger.IniciarLog(operationDescription))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    action.Invoke(retorno);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                Log.GravarLog(EventoLog.FimServico, new { retorno });
            }
            return retorno;
        }
    }
}
