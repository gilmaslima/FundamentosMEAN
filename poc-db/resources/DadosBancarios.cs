#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Agnaldo Costa]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [14/06/2012] – [Agnaldo Costa] – [Criação]
 *
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Microsoft.SharePoint;

namespace Redecard.PN.DadosCadastrais.Negocio
{
    public class DadosBancarios : RegraDeNegocioBase
    {
        /// <summary>
        /// Converte data para formato especificado
        /// </summary>
        /// <param name="value">Valor da data</param>
        /// <param name="format">Formato</param>
        /// <returns>Datetime</returns>
        public static DateTime ParseDate(object value, string format)
        {
            DateTime date;

            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;

            if (DateTime.TryParseExact(value.ToString(), format, provider, System.Globalization.DateTimeStyles.None, out date))
                return date;
            else
                return default(DateTime);
        }

        /// <summary>
        /// Consulta os dados bancários de Crédito ou Débito
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="tipoDado">Tipo de dados a ser consultado.
        /// C - Crédito
        /// D - Débito
        /// </param>
        /// <returns>Retorna a lista de Dados Bancários</returns>
        public List<Modelo.DadosBancarios> Consultar(Int32 codigoEntidade, String tipoDados, out Int32 codigoRetorno)
        {
            try
            {
                var dados = new Dados.DadosBancarios();

                List<Modelo.DadosBancarios> lista = new List<Modelo.DadosBancarios>();
                List<Modelo.DadosBancarios> listaDadosBancarios = dados.Consultar(codigoEntidade, out codigoRetorno);
                IEnumerable<Modelo.DadosBancarios> dadosB = from x in listaDadosBancarios
                                                            where x.TipoOperacao == tipoDados
                                                            select x;
                if (dadosB.Count() > 0)
                {
                    foreach (Modelo.DadosBancarios b in dadosB)
                    {
                        lista.Add(b);
                    }
                }
                //else
                //{
                //    lista.AddRange(listaDadosBancarios);
                //}
                return lista;
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
        /// Consulta uma relação de agências (Nome, Cidade, Estado) a partir dos
        /// parametros informados
        /// </summary>
        /// <returns></returns>
        public List<Modelo.Agencia> ConsultarAgencias(Int32 codigoAgencia, Int32 codigoBanco, out Int32 codigoRetorno)
        {
            try
            {
                var dados = new Dados.DadosBancarios();
                return dados.ConsultarAgencias(codigoAgencia, codigoBanco, out codigoRetorno);
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
        /// Consulta as tarifas de Transmissão
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="tipoDados">Tipo de dados a ser consultado.
        /// CR - Crédito
        /// DB - Débito
        /// </param>
        /// <returns></returns>
        public Modelo.Tarifas ConsultarTarifas(Int32 codigoEntidade, String tipoDados, out Int32 codigoRetorno)
        {
            try
            {
                var dados = new Dados.DadosBancarios();
                return dados.ConsultarTarifas(codigoEntidade, tipoDados, out codigoRetorno);
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
        /// Consulta os dados dos Domicílios Bancários da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoRetorno">Código de erro retornado</param>
        /// <returns>Dados do Domicilio Bancário</returns>
        public List<Modelo.DadosDomiciolioBancario> ConsultarDadosDomiciliosBancarios(Int32 codigoEntidade, out Boolean permissaoAlteracao, out Int32 codigoRetorno)
        {
            try
            {
                var dados = new Dados.DadosBancarios();
                codigoRetorno = 0;
                List<Modelo.DadosDomiciolioBancario> listaDomicilios = dados.ConsultarDadosDomiciliosBancarios(codigoEntidade, out permissaoAlteracao, out codigoRetorno);

                AgruparBandeirasDomicilios(ref listaDomicilios);

                return listaDomicilios;
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
        /// Agrupa a listagem de bandeiras com seus domicílios
        /// </summary>
        /// <param name="listaDomicilios"></param>
        public void AgruparBandeirasDomicilios(ref List<Modelo.DadosDomiciolioBancario> listaDomicilios)
        {
            Int32 codigoBanco = 0;
            String codigoAgencia = "";
            String numeroConta = "";
            Modelo.DadosDomiciolioBancario domicilioAux = null;
            List<Modelo.DadosDomiciolioBancario> listaAux = new List<Modelo.DadosDomiciolioBancario>();

            var lista = listaDomicilios.OrderBy(o => o.CodigoBanco).ThenBy(o => o.CodigoAgencia).ThenBy(o => o.NumeroConta);

            foreach (Modelo.DadosDomiciolioBancario domicilio in lista)
            {
                if (codigoBanco != domicilio.CodigoBanco
                    || codigoAgencia != domicilio.CodigoAgencia
                    || numeroConta != domicilio.NumeroConta)
                {
                    codigoBanco = domicilio.CodigoBanco;
                    codigoAgencia = domicilio.CodigoAgencia;
                    numeroConta = domicilio.NumeroConta;

                    domicilioAux = new Modelo.DadosDomiciolioBancario();

                    var grpDomicilios = listaDomicilios.FindAll(dom => (dom.CodigoBanco == codigoBanco)
                                                                    && (dom.CodigoAgencia == codigoAgencia)
                                                                    && (dom.NumeroConta == numeroConta));

                    //Dados do domicílio
                    domicilioAux.CodigoBanco = domicilio.CodigoBanco;
                    domicilioAux.NomeBanco = domicilio.NomeBanco;
                    domicilioAux.CodigoAgencia = domicilio.CodigoAgencia;
                    domicilioAux.NomeAgencia = domicilio.NomeAgencia;
                    domicilioAux.NumeroConta = domicilio.NumeroConta;
                    domicilioAux.Centralizado = domicilio.Centralizado;

                    domicilioAux.BandeirasCredito = new List<Modelo.Bandeira>();
                    domicilioAux.BandeirasDebito = new List<Modelo.Bandeira>();

                    foreach (Modelo.DadosDomiciolioBancario dom in grpDomicilios)
                    {
                        if (dom.BandeirasCredito.Count > 0)
                        {
                            domicilioAux.BandeirasCredito.Add(dom.BandeirasCredito[0]);
                        }

                        if (dom.BandeirasDebito.Count > 0)
                        {
                            domicilioAux.BandeirasDebito.Add(dom.BandeirasDebito[0]);
                        }
                    }

                    listaAux.Add(domicilioAux);
                }
            }

            listaDomicilios = listaAux;

        }

        /// <summary>
        /// Consulta bancos cadastradados na base DR
        /// </summary>
        /// <returns>Lista de Bancos</returns>
        public List<Modelo.Banco> ConsultarBancos()
        {
            try
            {
                var dados = new Dados.DadosBancarios();
                return dados.ConsultarBancos();
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
        /// Consulta bancos cadastradados na base DR para confirmação positiva
        /// </summary>
        /// <returns>Lista de Bancos</returns>
        public List<Modelo.Banco> ConsultarBancosConfirmacaoPositiva()
        {
            try
            {
                var dados = new Dados.DadosBancarios();
                return dados.ConsultarBancosConfirmacaoPositiva();
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
        /// Consulta os bancos com Confirmação Eletrônica disponível
        /// </summary>
        /// <returns>Lista de bancos com confirmação eletrônica</returns>
        public List<Modelo.Banco> ConsultarBancosConfirmacaoEletronica()
        {
            try
            {
                var dados = new Dados.DadosBancarios();
                return dados.ConsultarBancosConfirmacaoEletronica();
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
        /// Valida a Alteração dos dados do Domicílio Bancários da Entidade
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="dadosBancarios">Dados do Domicílio Bancário</param>
        /// <param name="confirmacaoEletronica">Indica se há confirmação eletrônica
        /// S - Sim, há confirmação eletrônica
        /// N - Não, não há confirmação eletrônica
        /// </param>
        /// <param name="aguardarDocumento">Indica se ocorre espera de recebimento de Documento
        /// S - Sim. Não há Confirmação Eletrônica
        /// N - Não. Há Confirmação Eletrônica
        /// </param>
        /// <param name="codigoRetorno">Código de erro retornado pela procedure</param>
        /// <returns>
        /// True - Alteração Válida
        /// False - Alteração inválida
        /// </returns>
        public Boolean ValidarAlteracaoDomicilioBancario(Int32 codigoEntidade, Modelo.DadosDomiciolioBancario dadosBancarios, String confirmacaoEletronica, String aguardarDocumento, out Int32 codigoRetorno)
        {
            try
            {
                codigoRetorno = 0;
                var dados = new Dados.DadosBancarios();
                return dados.ValidarAlteracaoDomicilioBancario(codigoEntidade, dadosBancarios, confirmacaoEletronica, aguardarDocumento, out codigoRetorno);
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
        /// Grava uma nova solicitação de alteração de domicílio bancário
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="grupoSolicitacao">Grupo de Solicitação</param>
        /// <param name="cnpj">CNPJ</param>
        /// <param name="numeroRequisicao">Retorno com o número da Requisição</param>
        /// <param name="numeroSolicitacao">Retorno com o número da Solicitação</param>
        /// <returns></returns>
        public Int32 InserirSolicitacaoAlteracaoDomicilioBancario(Int32 codigoEntidade, String grupoSolicitacao, String cnpj,
            out Int32 numeroRequisicao, out Int32 numeroSolicitacao)
        {
            try
            {
                var dados = new Dados.DadosBancarios();
                return dados.InserirSolicitacaoAlteracaoDomicilioBancario(codigoEntidade, grupoSolicitacao, cnpj, out numeroRequisicao, out numeroSolicitacao);
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
        /// Grava as alterações do Domicílio Bancário
        /// </summary>
        /// <param name="numeroRequisicao">Número de Requisição gerado pela proc SPWM0123</param>
        /// <param name="numeroSolicitacao">Número de Solicitação gerado pela proc SPWM0123</param>
        /// <param name="tipoOperacao">Tipo da operação bancária: CR - Crédito; DB - Débito; CDC - Construcard</param>
        /// <param name="codigoBanco">Código do Banco</param>
        /// <param name="agencia">Agência</param>
        /// <param name="conta">Número da conta</param>
        /// <param name="aguardaDocumento">Indica necessidade do envio de documento
        /// S - Sem confirmação eletrônica
        /// N - Com confirmação eletrônica
        /// </param>
        /// <param name="confirmacaoEletronica">Indica se há confirmação eletrônica
        /// S - Com confirmação eletrônica
        /// N - Sem confirmação eletrônica
        /// </param>
        /// <param name="canal">Canal de alteração</param>
        /// <param name="celula">Célula de alteração</param>
        /// <param name="tipoTransacao">Tipo da transação</param>
        /// <returns></returns>
        public Int32 InserirAlteracaoDomicilioBancario(Int32 numeroRequisicao, Int32 numeroSolicitacao, String tipoOperacao, String codigoBanco, String agencia,
            String conta, String aguardaDocumento, String confirmacaoEletronica, String canal, String celula, String tipoTransacao)
        {
            try
            {
                var dados = new Dados.DadosBancarios();
                Int32 codigoRetorno = 0;
                //Grava metadados de informação do domicílio

                //Campo: TIPDOMALT
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "TIPDOMALT", tipoOperacao);
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: BANCODIGI
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "BANCODIGI", codigoBanco);
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: AGENCDIGI
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "AGENCDIGI", agencia);
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: CONTCDIGI
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "CONTCDIGI", conta);
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: FLGSDOCUM
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "FLGSDOCUM", aguardaDocumento);
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: FLGSCONFE
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "FLGSCONFE", confirmacaoEletronica);
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: FLGEXPAND
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "FLGEXPAND", "N");
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: ALTCANAL
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "ALTCANAL", canal);
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: ALTCELULA
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "ALTCELULA", celula);
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: ALTAGENCI (agência solicitante)
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "ALTAGENCI", "00");
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: ALTAGENTE (agente solicitante)
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "ALTAGENTE", "IS");
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: FLGSKIT
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "FLGSKIT", "N");
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: FLGELECTR
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "FLGELECTR", "N");
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: ALTORIGEM
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "ALTORIGEM", "IS");
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: ALTPERFIL
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "ALTPERFIL", "1");
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: TIPTRAN
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "TIPTRAN", tipoTransacao);
                if (codigoRetorno != 0)
                    return codigoRetorno;

                //Campo: FRMAPG *Forma de Pagamento (Sempre Zero segunda a Bia)
                codigoRetorno = dados.InserirAlteracaoDomicilioBancario(numeroRequisicao, numeroSolicitacao, "FRMAPG", "0");
                if (codigoRetorno != 0)
                    return codigoRetorno;

                return codigoRetorno;
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
        /// Consultar os Dados de Domicílio Bancário Alterados na data atual
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <returns>Último domicílio alterado</returns>
        public List<Modelo.DadosDomiciolioBancario> ConsultarDomiciliosAlterados(Int32 codigoEntidade)
        {
            try
            {
                var dados = new Dados.DadosBancarios();
                List<Modelo.DadosDomiciolioBancario> listaDados = dados.ConsultarDomiciliosAlterados(codigoEntidade);

                listaDados = FiltrarAlteracoesDomicilio(listaDados);

                return listaDados;
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

        private List<Modelo.DadosDomiciolioBancario> FiltrarAlteracoesDomicilio(List<Modelo.DadosDomiciolioBancario> listaDados)
        {
            try
            {
                List<Modelo.DadosDomiciolioBancario> ultimasSolicitacoes = new List<Modelo.DadosDomiciolioBancario>();
                Modelo.DadosDomiciolioBancario ultima = new Modelo.DadosDomiciolioBancario();
                if (listaDados.Count > 0)
                {
                    //var listaOrdenada = listaDados.OrderBy(o => o.DataSolicitacao); //.ThenBy(o => o.HoraSolicitacao).ToList();

                    ultima = listaDados[listaDados.Count - 1];

                    ultimasSolicitacoes = listaDados.FindAll(o => (o.DataSolicitacao == ultima.DataSolicitacao) 
                                                                && (o.CodigoBanco == ultima.CodigoBanco)
                                                                && (o.CodigoAgencia == ultima.CodigoAgencia)
                                                                && (o.NumeroConta == ultima.NumeroConta));
                }

                return ultimasSolicitacoes;
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
        /// Listar os Dados de Domicílio Bancário Alterados com Status Pendente
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <returns>Lista de domicílios alterados pendentes de aprovação</returns>
        public List<Modelo.DadosDomiciolioBancario> ListarDomiciliosAlterados(Int32 codigoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Listar Domicilios Alterados"))
            {
                try
                {
                    return new Dados.DadosBancarios().ConsultarDomiciliosAlterados(codigoEntidade);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Cancela a alteração de Domicílio Bancária solicitada
        /// </summary>
        /// <param name="numeroSolicitacao">Código da Solicitação de Alteração</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoRetorno">Código de retorno de erro da procedure</param>
        /// <returns></returns>
        public void CancelarAlteracao(Int32 numeroSolicitacao, Int32 codigoEntidade, out Int32 codigoRetorno)
        {
            try
            {
                var dados = new Dados.DadosBancarios();
                codigoRetorno = 0;
                dados.CancelarAlteracao(numeroSolicitacao, codigoEntidade, out codigoRetorno);
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
        /// Consulta os Produtos Flex cadastrados para um estabelecimento
        /// </summary>
        /// <param name="codigoCCA">Código da CCA</param>
        /// <param name="codigoFeature">Código da Feature</param>
        /// <param name="codigoEntidade">Código do estabelecimento</param>
        /// <param name="codigoRetorno">Código de retorno da procedure</param>
        /// <returns>Retorna a lista dos Produtos Flex de um estabelecimento</returns>        
        public List<Modelo.ProdutoFlex> ConsultarProdutosFlex(Int32 codigoEntidade, Int32? codigoCCA, Int32? codigoFeature, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Produtos Flex"))
            {
                try
                {
                    return new Dados.DadosBancarios()
                        .ConsultarProdutosFlex(codigoEntidade, codigoCCA, codigoFeature, out codigoRetorno);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        #region .ConsultarDadosBancarios.
        /// <summary>
        /// Faz a consulta dos dados bancarios debito/credito
        /// </summary>
        /// <param name="CodigoEntidade">PV</param>
        /// <param name="CodigoRetorno">Código de retorno da SP</param>
        /// <returns>Entidade <code>Modelo.DadosBancarios</code> com os dados.</returns>
        public Modelo.DadosBancarios ConsultarDadosBancarios(Int32 CodigoEntidade, out Int32 CodigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Dados bancarios"))
            {
                try
                {
                    return new Dados.DadosBancarios()
                        .ConsultarDadosBancarios(CodigoEntidade, out CodigoRetorno);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        #endregion
    }
}
