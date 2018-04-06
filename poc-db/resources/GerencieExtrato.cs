using System;
using System.Collections.Generic;
using Redecard.PN.Comum;

namespace Redecard.PN.GerencieExtrato.Negocio
{
    public class GerencieExtrato : RegraDeNegocioBase
    {
        public List<Modelo.ExtratoEmitido> ListaExtratos(String programa, Int32 codigoEstabelecimento,
            Int32 numeroExtrato, ref Int16 totalRegistros, ref Int16 ts_reg, ref String mensagem,
            ref Int16 qtdOcorrencias, ref Int16 codigoRetorno)
        {

            try
            {
                List<Modelo.ExtratoEmitido> lstRetorno = new Agentes.GerencieExtrato().ListaExtratos(programa,
            codigoEstabelecimento, numeroExtrato,
            ref totalRegistros,
            ref ts_reg,
            ref mensagem,
            ref qtdOcorrencias,
            ref codigoRetorno);

                return lstRetorno;
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }

        }


        public List<Modelo.Extrato> ConsultarExtrato(ref String programa,
            ref Int32 codigoEstabelecimento,
            ref Int32 numeroExtrato, ref Int32 sequencia, ref String tipoAcesso,
            ref Int16 linhaTela, ref Int16 linhaAtual, ref Int16 linhaInicial, ref Int16 linhaFinal,
            ref Int16 coluna, ref String chaveAnterior, ref Int32 quantidadeLinhas,
            ref Int16 totalRegistros, ref Int16 registro, ref String filler, ref String mensagem,
            ref Int16 codigoRetorno, ref String temMaisRegistros, ref String quantidadeOcorrencias)
        {
            try
            {
                List<Modelo.Extrato> lstRetorno = new Agentes.GerencieExtrato().ConsultarExtrato(ref  programa,
            ref  codigoEstabelecimento,
            ref  numeroExtrato, ref  sequencia, ref  tipoAcesso,
            ref  linhaTela, ref  linhaAtual, ref  linhaInicial, ref  linhaFinal,
            ref  coluna, ref  chaveAnterior, ref  quantidadeLinhas,
            ref  totalRegistros, ref  registro, ref  filler, ref  mensagem,
            ref  codigoRetorno, ref  temMaisRegistros, ref  quantidadeOcorrencias);

                return lstRetorno;
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
        public void ObterExtratoPapel(ref String sistema, ref List<Modelo.StatusEmissao> solicita, ref Int16 codigoRetorno, ref String mensagemRetorno)
        {
            try
            {
                new Agentes.GerencieExtrato().ObterExtratoPapel(ref  sistema, ref solicita, ref  codigoRetorno, ref  mensagemRetorno);

                //return lstRetorno;
            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
        public void Extrato_Email(ref String tipo,
            ref Int32 codigoEstabelecimento,
            ref decimal cnpjSolicitante,
            ref String periodicidadeEnvio,
            ref String diaEnvio,
            ref String tipoPVSolicitante,
            ref String tipoSolicitacao,
            ref String nomeUsuario,
            ref String nomeEmailrecebimento,
            ref String fraseCriptografada,
            ref List<Modelo.CadeiaPV> lstCadeia,
            ref String[] codigoBoxes,
            ref String codigoErro,
            ref String quantidadePvs,
            ref String identificadorContinuacao,
            ref String acao,
            ref String msgErro)
        {
            try
            {
                String abendCics = String.Empty;
                String codigoAcessoDb2 = String.Empty;
                String tableErro = String.Empty;
                String comandoErro = String.Empty;
                String mensagemErro = String.Empty;

                // Verifica o tamanho do campo nomeUsuario o programa Mainframe esta definido com o tamanho 20.
                if (nomeUsuario.Length > 20)
                    nomeUsuario = nomeUsuario.Left(20);

                new Agentes.GerencieExtrato().Extrato_Email(ref tipo, ref codigoEstabelecimento,
                    ref cnpjSolicitante, ref periodicidadeEnvio, ref diaEnvio, ref tipoPVSolicitante, ref tipoSolicitacao, ref nomeUsuario,
                    ref nomeEmailrecebimento, ref fraseCriptografada,
                    ref lstCadeia,
                    ref codigoBoxes, ref codigoErro,
                    out abendCics, out codigoAcessoDb2, ref tableErro, ref comandoErro, out mensagemErro, ref quantidadePvs, ref identificadorContinuacao);

                if (codigoErro.ToInt32() == 22)
                {
                    acao = "I";
                }
                else if (codigoErro.ToInt32() == 0)
                {
                    acao = "A";
                }
                //"WA453CA_ERRO:" & WA453CA_ERRO & " - WA453CA_ABND_CICS:" & WA453CA_ABND_CICS & " - WA453CA_SQLCODE:" & WA453CA_SQLCODE & " - WA453CA_TABELA:" & WA453CA_TABELA & " - WA453CA_COMANDO:" & WA453CA_COMANDO & " - WA453CA_MSG:" & WA453CA_MSG, findUrl , 133
                if ((codigoErro.ToInt32() > 0) && (codigoErro.ToInt32() != 22))
                {
                    msgErro = "WA453CA_ERRO:" + codigoErro + " - WA453CA_ABND_CICS:" + abendCics +
                                " - WA453CA_SQLCODE:" + codigoAcessoDb2 + 
                                " - WA453CA_TABELA:" + tableErro +
                                " - WA453CA_COMANDO:" + comandoErro +
                                " - WA453CA_MSG:" + mensagemErro;
                }

            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
        public void InibirExtPapel(ref String sistema, ref List<Modelo.DadosPV> dados)
        {
            try
            {
                new Agentes.GerencieExtrato().InibirExtPapel(ref sistema, ref dados);

            }
            catch (PortalRedecardException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Retorna as informações de relatório de preço único, identificação, descrição e período.<br/>
        /// Utilizado no Projeto Turquia / Preço Único<br/>
        /// - Book BKWA2890 / Programa WAC289 / TranID WAGA / Método ConsultarRelatorioPrecoUnico
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2890 / Programa WAC289 / TranID WAGA / Método ConsultarRelatorioPrecoUnico
        /// </remarks>  
        /// <param name="numeroPv">Número do Ponto de Venda (Número Estabelecimento) que se deseja consultar.</param>
        /// <param name="possuiRechamada">Flag indicando se ainda existem registros para serem retornados.</param>
        /// <param name="rechamada">Chaves utilizadas para solicitação dos próximos registros.</param>
        /// <param name="codigoRetorno">Cógigo retornado pelo Mainframe indicando o status da execução do comando.</param>
        /// <returns>Retorna identificação, descrição e período do relatório de Preço Único</returns>  
        public List<Modelo.ExtratoRelatorioPrecoUnico> ConsultarRelatorioPrecoUnico(
            Int32 numeroPv,
            ref Dictionary<String, Object> rechamada,
            out Boolean possuiRechamada,
            out Int16 codigoRetorno)
        {
            try
            {
                //Instancia classe de agente
                var agente = new Agentes.GerencieExtrato();

                //Consulta dados
                List<Modelo.ExtratoRelatorioPrecoUnico> retorno = agente.ConsultarRelatorioPrecoUnico(
                    numeroPv, ref rechamada, out possuiRechamada, out codigoRetorno);

                return retorno;
            }
            catch (PortalRedecardException ex)
            {
                throw new PortalRedecardException(ex.Codigo, ex.Fonte, ex);
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Retorna o detalhamento do relatório Preço Único. <br/>
        /// Utilizado no Projeto Turquia / Preço Único<br/>
        /// - Book BKWA2900 / Programa WAC290 / TranID WABG / Método DetalharRelatorioPrecoUnico
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2900 / Programa WAC290 / TranID WABG / Método DetalharRelatorioPrecoUnico
        /// </remarks>
        /// <param name="numeroPv">Número do Ponto de Venda (Número Estabelecimento) que se deseja consultar.</param>
        /// <param name="mesAnoRelatorio">Mês e ano que se deseja consultar.</param>
        /// <param name="flagVsam">Flag retornado pela consulta de relatório Preço Único</param>
        /// <param name="possuiRechamada">Flag indicando se ainda existem registros para serem retornados.</param>
        /// <param name="rechamada">Chaves utilizadas para solicitação dos próximos registros.</param>        
        /// <param name="codigoRetorno">Cógigo retornado pelo Mainframe indicando o status da execução do comando.</param>
        /// <returns>Relatório detalhado Preço Único </returns>
        public List<Modelo.RelatorioDetalhadoPrecoUnico> DetalharRelatorioPrecoUnico(
            Int32 numeroPv,
            DateTime mesAnoRelatorio,
            Int16 flagVsam,
            ref Dictionary<String,Object> rechamada,
            out Boolean possuiRechamada,
            out Int16 codigoRetorno)
        {
            try
            {
                //Instancia classe de agente
                var agente = new Agentes.GerencieExtrato();

                //Consulta dados
                List<Modelo.RelatorioDetalhadoPrecoUnico> retorno = agente.DetalharRelatorioPrecoUnico(
                    numeroPv, mesAnoRelatorio, flagVsam, ref rechamada, out possuiRechamada, out codigoRetorno);

                return retorno;
            }
            catch (PortalRedecardException ex)
            {
                throw new PortalRedecardException(ex.Codigo, ex.Fonte, ex);
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }

        /// <summary>
        /// Solicita a recuperação de Relatório Preço Único baseado em um período.<br/>
        /// Utilizado no Projeto Turquia / Preço Único<br/>
        /// - Book BKWA2910 / Programa WAS291 / TranID WAGC / Método SolicitarRelatorioPrecoUnico
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2910 / Programa WAS291 / TranID WAGC / Método SolicitarRelatorioPrecoUnico
        /// </remarks>
        /// <param name="numeroPv">Número do Ponto de Venda (Número Estabelecimento) que se deseja solicitar o relatório.</param>
        /// <param name="mesAnoDe">Mês inicial que relatório deve contemplar.</param>
        /// <param name="mesAnoAte">Mês final até o qual o relatório deve contemplar.</param>
        /// <param name="codigoRetorno">Cógigo retornado pelo Mainframe indicando o status da execução do comando.</param>
        public void SolicitarRelatorioPrecoUnico(
            Int32 numeroPv,
            DateTime mesAnoDe,
            DateTime mesAnoAte,
            out Int16 codigoRetorno)
        {
            try
            {
                //Instancia classe de agente
                var agente = new Agentes.GerencieExtrato();

                //Solicita Relatório
                agente.SolicitarRelatorioPrecoUnico(numeroPv, mesAnoDe, mesAnoAte, out codigoRetorno);
            }
            catch (PortalRedecardException ex)
            {
                throw new PortalRedecardException(ex.Codigo, ex.Fonte, ex);
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
            }
        }
    }
}