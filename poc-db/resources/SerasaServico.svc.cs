using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Negocio;
using AutoMapper;

namespace Redecard.PN.Credenciamento.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SerasaServico" in code, svc and config file together.
    public class SerasaServico : ServicoBase, ISerasaServico
    {
        /// <summary>
        /// Consulta dados Serasa para pessoa Jurídica
        /// </summary>
        /// <param name="cnpj"></param>
        /// <returns></returns>
        public PJ ConsultaSerasaPJ(String cnpj)
        {
            using (Logger Log = Logger.IniciarLog("Serasa - Consulta PJ - WF050"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { cnpj });

                try
                {
                    SerasaBLL serasaBLL = new SerasaBLL();
                    Modelo.PJ retorno = serasaBLL.ConsultaSerasaPJ(cnpj);

                    Mapper.CreateMap<Modelo.PJ, PJ>();
                    Mapper.CreateMap<Modelo.Socio, Socio>();
                    Mapper.CreateMap<Modelo.CodigoCNAE, CodigoCNAE>();

                    return Mapper.Map<PJ>(retorno);
                }
                catch (PortalRedecardException ex)
                {
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

        /// <summary>
        /// Consulta dados Serasa para pessoa Física
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns></returns>
        public PF ConsultaSerasaPF(String cpf)
        {
            using (Logger Log = Logger.IniciarLog("Serasa - Consulta PF - WF050"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { cpf });

                try
                {
                    SerasaBLL serasaBLL = new SerasaBLL();
                    Modelo.PF retorno = serasaBLL.ConsultaSerasaPF(cpf);

                    Mapper.CreateMap<Modelo.PF, PF>();

                    return Mapper.Map<PF>(retorno);
                }
                catch (PortalRedecardException ex)
                {
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
