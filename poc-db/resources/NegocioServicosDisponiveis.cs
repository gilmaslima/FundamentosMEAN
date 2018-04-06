/*
 © Copyright 2017 Rede S.A.
   Autor : Rodrigo Coelho - rodrigo.oliveira@iteris.com.br
   Empresa : Iteris
 */


using Rede.PN.ZeroDolar.Agentes;
using Rede.PN.ZeroDolar.Agentes.GEServicos;
using Rede.PN.ZeroDolar.Modelo;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Rede.PN.ZeroDolar.Negocio {

    /// <summary>
    /// Camada de negócio para contratação e cancelamento de serviço
    /// </summary>
    public class NegocioServicosDisponiveis : RegraDeNegocioBase {


        /// <summary>
        /// Instância para a camada de agentes
        /// </summary>
        private AgenteServicosDisponiveis objAgente = new AgenteServicosDisponiveis();

        /// <summary>
        /// Lista serviços disponíveis para contratação e cancelamento
        /// </summary>
        /// <param name="numeroPV">Número do PV</param>
        /// <returns></returns>
        public List<ServicoDisponivel> ListarServicosDisponiveis(int numeroPV) {
            List<ServicoDisponivel> lstServicosDisponiveis = new List<ServicoDisponivel>();
            using (Logger Log = Logger.IniciarLog("Listagem de serviços disponíveis")) {
                try {

                    var objServicos = objAgente.ListarServicosDisponiveis(numeroPV);
                    foreach (ListaServicosPV objServico in objServicos) {
                        lstServicosDisponiveis.Add(new Rede.PN.ZeroDolar.Modelo.ServicoDisponivel() {
                            CodigoServico = objServico.CodServico,
                            Nome = objServico.DescricaoServico,
                            Situacao = objServico.CodigoSituacaoServico,
                            DataContratacao = objServico.DataContratacao
                        });
                    }

                } catch (PortalRedecardException ex) {
                    Log.GravarErro(ex);
                    throw ex;
                } catch (Exception ex) {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
            return lstServicosDisponiveis;
        }


        /// <summary>
        /// Realiza a contratação de serviço
        /// </summary>
        /// <param name="numeroPV">Número do PV</param>
        /// <param name="codigoServico">Código do Serviço</param>
        /// <param name="codigoCanal">Código do Canal</param>
        /// <param name="codigoCelula">Código da Célula</param>
        /// <param name="usuario">Nome do usuário</param>
        public void ContratarServico(int numeroPV, int codigoServico, int codigoCanal, int codigoCelula, char situacaoAtual, string usuario) {
            using (Logger Log = Logger.IniciarLog("Listagem de serviços disponíveis")) {
                try {

                    ResponseBase objRetornoContratacao = null;
                    CodErroDescricaoErro objRetornoReativacao = null;

                    if (situacaoAtual == 'D') {
                        objRetornoContratacao = objAgente.ContratarServico(numeroPV, codigoServico, codigoCanal, codigoCelula, usuario);
                        if (objRetornoContratacao.CodigoRetorno > 0)
                            throw new Exception(objRetornoContratacao.DescricaoRetorno);
                        else
                            GravarTogTermoAceite(numeroPV, codigoServico, usuario);

                    } else if (situacaoAtual == 'C')
                        objRetornoReativacao = objAgente.ReativarServico(numeroPV, codigoServico, usuario);

                } catch (PortalRedecardException ex) {
                    Log.GravarErro(ex);
                    throw ex;
                } catch (Exception ex) {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Grava dados em arquivo referente ao aceite dos termos do serviço
        /// </summary>
        private void GravarTogTermoAceite(int numeroPV, int codigoServico, string nomeUsuario) {
            using (Logger Log = Logger.IniciarLog("Listagem de serviços disponíveis")) {
                try {
                    string strCaminho = @"D:\Log\PN\ZeroDollar";
                    if (ConfigurationManager.AppSettings["CaminhoLogTermoAceiteServicos"] != null)
                        strCaminho = ConfigurationManager.AppSettings["CaminhoLogTermoAceiteServicos"].ToString();

                    AsseguraCaminhoLog(strCaminho);

                    StringBuilder logTermoAceite = new StringBuilder();
                    logTermoAceite.AppendLine("==================================================");
                    logTermoAceite.AppendLine(string.Format("Nome: {0}", nomeUsuario));
                    logTermoAceite.AppendLine(string.Format("Data: {0}", DateTime.Now));
                    logTermoAceite.AppendLine(string.Format("Número PV: {0}", numeroPV));
                    logTermoAceite.AppendLine(string.Format("Código Serviço: {0}", codigoServico));
                    logTermoAceite.AppendLine("==================================================");

                    using (StreamWriter fileLogTermoAceite = new StreamWriter(strCaminho + @"\LogTermoAceiteServicos.txt", true)) {
                        fileLogTermoAceite.WriteLine(logTermoAceite.ToString());
                    }

                } catch (IOException ex) {
                    Log.GravarErro(ex);
                    throw ex;
                } catch (PortalRedecardException ex) {
                    Log.GravarErro(ex);
                    throw ex;
                } catch (Exception ex) {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Assegura que o caminho do log esteja criado, caso não exista ele cria
        /// </summary>
        /// <param name="caminhoLog"></param>
        private static void AsseguraCaminhoLog(string caminhoLog) {
            string[] arrPartesCaminho = caminhoLog.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

            string strUnidade = arrPartesCaminho[0];
            foreach (var strParte in arrPartesCaminho) {
                if (strParte != strUnidade) {
                    if (!Directory.Exists(strUnidade + "\\" + strParte))
                        Directory.CreateDirectory(strUnidade + "\\" + strParte);
                    strUnidade = strUnidade + "\\" + strParte;
                }
            }
        }

        /// <summary>
        /// Cancela serviços contratados
        /// </summary>
        /// <param name="numeroPV">Número do PV</param>
        /// <param name="codigoServico">Código do Serviço</param>
        /// <param name="usuario">Nome do usuário</param>
        public void CancelarServico(int numeroPV, int codigoServico, string usuario) {
            using (Logger Log = Logger.IniciarLog("Listagem de serviços disponíveis")) {
                try {

                    var objRetorno = objAgente.CancelarServico(numeroPV, codigoServico, usuario);
                    if (objRetorno.CodErro > 0)
                        throw new Exception(objRetorno.DescricaoErro);

                } catch (PortalRedecardException ex) {
                    Log.GravarErro(ex);
                    throw ex;
                } catch (Exception ex) {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }


        /// <summary>
        /// Reativar um serviço cancelado
        /// </summary>
        /// <param name="numeroPV">Número do PV</param>
        /// <param name="codigoServico">Código do Serviço</param>
        /// <param name="usuario">Nome do usuário</param>
        /// <returns></returns>
        public void ReativarServico(int numeroPV, int codigoServico, string usuario) {
            using (Logger Log = Logger.IniciarLog("Listagem de serviços disponíveis")) {
                try {

                    var objRetorno = objAgente.ReativarServico(numeroPV, codigoServico, usuario);
                    if (objRetorno.CodErro > 0)
                        throw new Exception(objRetorno.DescricaoErro);

                } catch (PortalRedecardException ex) {
                    Log.GravarErro(ex);
                    throw ex;
                } catch (Exception ex) {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

    }
}
