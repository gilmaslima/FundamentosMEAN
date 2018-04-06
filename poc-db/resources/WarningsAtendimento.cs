/*
© Copyright 2017 Rede S.A.
Autor : Lucas Akira Uehara
Empresa : Iteris Consultoria e Software
*/
using Rede.PN.AtendimentoDigital.SharePoint.Core.Warnings;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Warnings.Model;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.AtendimentoDigital.SharePoint.Handlers
{
    /// <summary>
    /// Define a classe Handler WarningsAtendimento.
    /// </summary>
    public class WarningsAtendimento : HandlerBase
    {
        [HttpGet]
        public HandlerResponse ConsultarWarnings()
        {
            try 
            {
                //Obtém a data atual do servidor.
                var dataAtual = DateTime.Now;
                //Inicializa WarningsAtendimentoPortal
                var warningsAtendimento = new WarningsAtendimentoPortal(base.CurrentSPContext.Web, base.Sessao);
                //Executa a consulta
                var result = warningsAtendimento.ListarWarnings(dataAtual);
                //retorna a lista de warnings
                return new HandlerResponse(result);
            }
            catch (ArgumentException ex)
            {
                SharePointUlsLog.LogErro(ex);
                Logger.GravarErro("Erro genérico 1 durante pesquisa", ex);
                return new HandlerResponse(301, "Erro genérico 1 durante consulta");
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                Logger.GravarErro("Erro genérico 2 durante pesquisa", ex);
                return new HandlerResponse(302, "Erro genérico 2 durante consulta");
            }
        }

    }
}
