using AutoMapper;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Serviço oferta maquininha conta certa
    /// </summary>
    public class MaquininhaContaCerta : ServicoBase, IMaquininhaContaCerta
    {
        /// <summary>
        /// Consulta o contrato da oferta maquininha conta certa
        /// </summary>
        /// <param name="numEstabelecimento">Código do estabelecimento</param>
        /// <param name="dataFimVigencia">Data fim de vigência</param>
        /// <param name="codigoStatusContrato">Código da situação do contrato</param>
        /// <returns>Sumário do contrato maquininha conta certa</returns>
        public MaquininhaContrato ConsultaContrato(Int32 numEstabelecimento, DateTime dataFimVigencia, Int16? codigoStatusContrato)
        {
            using (Logger log = Logger.IniciarLog("Maquininha Conta Certa - ConsultaContrato"))
            {
                try
                {
                    // papeamentos do Modelo de Negocio e de Serviço
                    Mapper.CreateMap<Modelo.MaquininhaContrato, MaquininhaContrato>();

                    Modelo.MaquininhaContrato modelo = new Negocio.Maquininha().ConsultaContrato(
                        numEstabelecimento,
                        dataFimVigencia,
                        codigoStatusContrato);

                    MaquininhaContrato contrato = Mapper.Map<MaquininhaContrato>(modelo);
                    return contrato;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta as faixas de faturamento das metas do contrato maquininha conta certa
        /// </summary>
        /// <param name="numPdv">Código do estabelecimento</param>
        /// <returns>Faixa de faturamento das metas</returns>
        public List<MaquininhaMetas> ConsultaMetas(int numPdv)
        {
            using (Logger log = Logger.IniciarLog("Maquininha Conta Certa - ConsultaMetas"))
            {
                try
                {
                    // papeamentos do Modelo de Negocio e de Serviço
                    Mapper.CreateMap<Modelo.MaquininhaMetas, MaquininhaMetas>();

                    List<Modelo.MaquininhaMetas> modelo = new Negocio.Maquininha().ConsultaMetas(numPdv);

                    List<MaquininhaMetas> metas = Mapper.Map<List<MaquininhaMetas>>(modelo);
                    return metas;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta o histórico de apuração
        /// </summary>
        /// <param name="numPdv">Código do estabelecimento</param>
        /// <returns>Lista com o histórico de apuração</returns>
        public List<MaquininhaHistoricoApuracao> ConsultaHistoricoApuracao(int numPdv)
        {
            using (Logger log = Logger.IniciarLog("Maquininha Conta Certa - ConsultaHistoricoApuracao"))
            {
                try
                {
                    // papeamentos do Modelo de Negocio e de Serviço
                    Mapper.CreateMap<Modelo.MaquininhaHistoricoApuracao, MaquininhaHistoricoApuracao>();

                    List<Modelo.MaquininhaHistoricoApuracao> modelo = new Negocio.Maquininha().ConsultaHistoricoApuracao(numPdv);

                    List<MaquininhaHistoricoApuracao> historico = Mapper.Map<List<MaquininhaHistoricoApuracao>>(modelo);
                    return historico;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                            new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }
    }
}
