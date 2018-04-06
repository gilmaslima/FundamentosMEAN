/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/09/29 - Tiago Barbosa dos Santos - Versão Inicial
- 2012/10/31 - Tiago Barbosa dos Santos - Trava de Domicilio
*/
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Data;
using System.Data.OracleClient;
using System.Data.Common;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

using Redecard.PN.Comum;
using Redecard.PN.Emissores.Modelos;

namespace Redecard.PN.Emissores.Dados
{
    public class DadosEmissores : BancoDeDadosBase
    {
        #region ObtemPV
        /// <summary>
        /// Obtem o PV a partir do Número
        /// </summary>
        /// <param name="PV">Número PV</param>
        /// <returns></returns>
        public PontoVenda ObtemPV(int numPV, out int codigoRetorno)
        {
            Logger.IniciarLog("ObtemPV");
            try
            {
                GenericDatabase db = this.SybaseGE();
                PontoVenda pv = null;
                codigoRetorno = 0;

                using (DbCommand command = db.GetStoredProcCommand("spge6002"))
                {
                    // Adicionando Parametros
                    db.AddInParameter(command, "@NUM_PDV", DbType.Int32, numPV);

                    db.AddInParameter(command, "@COD_ACES_INTN", DbType.AnsiString, null);
                    db.AddInParameter(command, "@PRIM_VEZ", DbType.AnsiString, null);
                    db.AddInParameter(command, "@CLIENTE", DbType.AnsiString, null);
                    db.AddInParameter(command, "@VALIDA_CITE", DbType.AnsiString, "N");
                    db.AddInParameter(command, "@SGL_TIP_PROD", DbType.AnsiString, null);
                    Logger.GravarLog("Chamada procedure spge6002", command.Parameters);

                    using (IDataReader leitor = db.ExecuteReader(command))
                    {

                        while (leitor.Read())
                        {
                            codigoRetorno = leitor["RETORNO"].ToString().ToInt32();
                            Logger.GravarLog("Retorno chamada procedure spge6002", codigoRetorno);
                            if (codigoRetorno == 0)
                            {
                                pv = new PontoVenda();
                                pv.Codigo = numPV;
                                pv.RazaoSocial = leitor["RAZAOSOC"].ToString().Trim();
                                pv.NomeComercial = leitor["NOMFAT"].ToString().Trim();
                                pv.TipoPessoa = leitor["TIPOPESS"].ToString().Trim();

                                pv.Endereco = new EnderecoPadrao()
                                {
                                    Endereco = leitor["ENDLOJA"].ToString().Trim(),
                                    NumeroEndereco = leitor["NUMLOJA"].ToString().Trim(),
                                    Complemento = leitor["CPMLOJA"].ToString().Trim(),
                                    Cep = String.Format("{0}-{1}", leitor["CEPLOJA"].ToString(), leitor["CCEPLOJA"].ToString()),
                                    Bairro = leitor["BRRLOJA"].ToString().Trim(),
                                    Cidade = leitor["CIDLOJA"].ToString().Trim(),
                                    Uf = leitor["UFLOJA"].ToString().Trim()
                                };

                                pv.EnderecoEntrega = new EnderecoPadrao()
                                {
                                    Endereco = leitor["ENDCRRS"].ToString().Trim(),
                                    NumeroEndereco = leitor["NUMCRRS"].ToString().Trim(),
                                    Complemento = leitor["CPMLOJA"].ToString().Trim(),
                                    Cep = String.Format("{0}-{1}", leitor["CEPCRRS"].ToString(), leitor["CCEPCCRS"].ToString()),
                                    Bairro = leitor["BRRCRRS"].ToString().Trim(),
                                    Cidade = leitor["CIDCRRS"].ToString().Trim(),
                                    Uf = leitor["UFCRRS"].ToString().Trim()
                                };

                                pv.PessoaContato = leitor["PESCTO"].ToString().Trim();
                                pv.Telefone = new DadosTelefone() { DDD = leitor["DDD"].ToString().Trim(), Telefone = leitor["FONE"].ToString().Trim(), Ramal = leitor["RAMAL"].ToString().Trim() };
                                pv.Telefone2 = new DadosTelefone() { DDD = leitor["DDD2"].ToString().Trim(), Telefone = leitor["FONE2"].ToString().Trim(), Ramal = leitor["RAMAL2"].ToString().Trim() };
                                pv.Fax = new DadosTelefone() { DDD = leitor["DDDFAX"].ToString().Trim(), Telefone = leitor["FAX"].ToString().Trim() };

                                pv.CodigoRamoAtividade = leitor["RAMO"].ToString().Trim();
                                pv.DescricaoRamoAtividade = leitor["DESRAMO"].ToString().Trim();

                                pv.Email = leitor["EMAIL"].ToString().Trim();
                                pv.Cnpj = leitor["CNPJ"].ToString().Trim();
                                pv.Centralizadora = leitor["CENTRAL"].ToString().Trim().ToUpper().CompareTo("S") == 0;
                                pv.TipoEstabelecimento = leitor["TIPOEST"].ToString().Trim();
                                pv.CodigoCentral = leitor["CODCENTRAL"].ToString().ToInt32();

                                pv.DadosBancarioCredito = new DadosBancarios()
                                {
                                    CodBanco = leitor["BCOCRE"].ToString().ToInt16(),
                                    NomeBanco = leitor["NOMBCOCRE"].ToString().Trim(),
                                    NomeAgencia = leitor["NOMAGECRE"].ToString().Trim(),
                                    NumAgencia = leitor["AGECRE"].ToString().ToInt16(),
                                    NumContaCorrente = leitor["CTACRE"].ToString().ToInt16()
                                };

                                pv.DadosBancarioDebito = new DadosBancarios()
                                {
                                    CodBanco = leitor["BCODEB"].ToString().ToInt16(),
                                    NomeBanco = leitor["NOMBCODEB"].ToString().Trim(),
                                    NomeAgencia = leitor["NOMAGEDEB"].ToString().Trim(),
                                    NumAgencia = leitor["AGEDEB"].ToString().ToInt16(),
                                    NumContaCorrente = leitor["CTADEB"].ToString().ToInt16()
                                };

                                pv.DadosBancarioMaestro = new DadosBancarios()
                                {
                                    CodBanco = leitor["BCOMST"].ToString().ToInt16(),
                                    NomeBanco = leitor["NOMBCOMST"].ToString().Trim(),
                                    NomeAgencia = leitor["NOMAGEMST"].ToString().Trim(),
                                    NumAgencia = leitor["AGEMST"].ToString().ToInt16(),
                                    NumContaCorrente = leitor["CTAMST"].ToString().ToInt16()
                                };

                                pv.DadosBancarioConstrucard = new DadosBancarios()
                                {
                                    CodBanco = leitor["BCOCTC"].ToString().ToInt16(),
                                    NomeBanco = leitor["NOMBCOCTC"].ToString().Trim(),
                                    NomeAgencia = leitor["NOMAGECTC"].ToString().Trim(),
                                    NumAgencia = leitor["AGECTC"].ToString().ToInt16(),
                                    NumContaCorrente = leitor["CTACTC"].ToString().ToInt16()
                                };

                                pv.ListaProprietarios = new List<DadosProprietario>();

                                pv.ListaProprietarios.Add(new DadosProprietario()
                                {
                                    Nome = leitor["NOMPRP1"].ToString().Trim(),
                                    DataNascimento = leitor["DTAPRP1"].ToString().Trim().ToDate(),
                                    TipoPessoa = leitor["TPPPRP1"].ToString().Trim(),
                                    CPF = leitor["CPFPRP1"].ToString().ToInt32()
                                });
                                pv.ListaProprietarios.Add(new DadosProprietario()
                                {
                                    Nome = leitor["NOMPRP2"].ToString().Trim(),
                                    DataNascimento = leitor["DTAPRP2"].ToString().Trim().ToDate(),
                                    TipoPessoa = leitor["TPPPRP2"].ToString().Trim(),
                                    CPF = leitor["CPFPRP2"].ToString().ToInt32()
                                });
                                pv.ListaProprietarios.Add(new DadosProprietario()
                                    {
                                        Nome = leitor["NOMPRP3"].ToString().Trim(),
                                        DataNascimento = leitor["DTAPRP3"].ToString().Trim().ToDate(),
                                        TipoPessoa = leitor["TPPPRP3"].ToString().Trim(),
                                        CPF = leitor["CPFPRP3"].ToString().ToInt32()
                                    });

                                pv.NomePlaqueta1 = leitor["PLAQ1"].ToString().Trim();
                                pv.NomePlaqueta2 = leitor["PLAQ2"].ToString().Trim();
                            }
                        }
                    }
                }
                Logger.GravarLog("Fim - ObtemPV", pv);
                return pv;
            }
            catch (ArgumentNullException ex)
            {
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (DbException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(501, FONTE, ex);
            }
        }
        #endregion

        #region Solicitacao de Tecnologia

        #region Download Arquivo TXT
        /// <summary>
        /// Consulta o arquivo para download
        /// </summary>
        /// <param name="codEmissor">Código do emissor</param>
        /// <param name="mesArquivo">Mes da consulta</param>
        /// <param name="anoArquivo">Ano da consulta</param>
        /// <returns></returns>
        public byte[] DownloadArquivo(string codEmissor, string mesArquivo, string anoArquivo)
        {
            Logger.IniciarLog("DownloadArquivo");
            byte[] buffer = null;

            Database db = this.SQLServerPN();
            try
            {
                //using (DbCommand command = db.GetStoredProcCommand("sp_cons_arquivo_emissores"))
                using (DbCommand command = db.GetSqlStringCommand(@"SELECT FILEDATA
		FROM dbo.TBPN078  
			WHERE MES = @MES 
				AND ANO = @ANO
				AND EMISSOR=@EMISSOR"))
                {

                    db.AddInParameter(command, "EMISSOR", DbType.Int32, codEmissor);
                    db.AddInParameter(command, "MES", DbType.Int32, mesArquivo);
                    db.AddInParameter(command, "ANO", DbType.Int32, anoArquivo);

                    Logger.GravarLog("Execução da procedure sp_cons_arquivo_emissores", command.Parameters);
                    using (IDataReader leitor = db.ExecuteReader(command))
                    {

                        if (leitor.Read())
                        {
                            buffer = (byte[])leitor["FILEDATA"];
                        }
                    }
                }
                Logger.GravarLog("Fim da chamada");
                return buffer;
            }
            catch (ArgumentNullException ex)
            {
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (DbException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(500, FONTE, ex);
            }
        }
        /// <summary>
        /// Obtém arquivos disponíveis no período informado
        /// </summary>
        /// <param name="codEmissor">Código do Emissor</param>
        /// <param name="anoInicial">Ano inicial da consulta</param>
        /// <param name="anoFinal">Ano fianl da consulta</param>
        /// <returns></returns>
        public List<DownloadMes> ObterPeriodosDisponiveis(string codEmissor, string anoInicial, string anoFinal)
        {
            Logger.IniciarLog("ObterPeriodosDisponiveis");

            try
            {
                Database db = this.SQLServerPN();
                String[] meses = { "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };

                List<DownloadMes> lstRetorno = new List<DownloadMes>();
                //using (DbCommand command = db.GetStoredProcCommand("sp_cons_arquivos_periodo"))
                using (DbCommand command = db.GetSqlStringCommand(@"
                    SELECT EMISSOR, MES, ANO
                    FROM dbo.TBPN078 
                    WHERE ANO BETWEEN @ANOINICIAL AND @ANOFINAL
                    AND EMISSOR = @EMISSOR
                    ORDER BY  ANO, MES"))
                {

                    db.AddInParameter(command, "EMISSOR", DbType.Int32, codEmissor);
                    db.AddInParameter(command, "ANOINICIAL", DbType.Int32, anoInicial);
                    db.AddInParameter(command, "ANOFINAL", DbType.Int32, anoFinal);
                    Logger.GravarLog("Chamada a procedure sp_cons_arquivos_periodo", command.Parameters);
                    using (IDataReader leitor = db.ExecuteReader(command))
                    {

                        while (leitor.Read())
                        {
                            lstRetorno.Add(new DownloadMes() { Mes = meses[leitor["MES"].ToString().ToInt32() - 1], MesId = leitor["MES"].ToString().ToInt32(), Ano = leitor["ANO"].ToString().ToInt32() });
                        }
                    }
                }
                Logger.GravarLog("Fim da chamada");
                return lstRetorno;

            }
            catch (ArgumentNullException ex)
            {
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (DbException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(500, FONTE, ex);
            }

        }
        public bool SalvarArquivo(byte[] arquivo, string nomeArquivo, string codEmissor, string mesArquivo, string anoArquivo, DateTime dataCriacao)
        {
            Logger.IniciarLog("Chamada método SalvarArquivo");
            try
            {
                Database db = this.SQLServerPN();
                bool retorno = false;
                if (!ExisteArquivo(codEmissor, mesArquivo, anoArquivo))
                {
                    //                using (DbCommand command = db.GetStoredProcCommand("sp_ins_arquivo_emissores"))
                    using (DbCommand command = db.GetSqlStringCommand(@"
                    INSERT INTO dbo.TBPN078 
                            (
		                    NAMEFILE,
		                    FILEDATA,
		                    EMISSOR,
		                    MES,
		                    ANO,
		                    DATACRIACAO
                            )
                            VALUES (
		                    @NAMEFILE,
		                    @FILEDATA,
		                    @EMISSOR,
		                    @MES,
		                    @ANO,
		                    @DATACRIACAO
                            )"))
                    {


                        db.AddInParameter(command, "NAMEFILE", DbType.String, nomeArquivo);
                        db.AddInParameter(command, "FILEDATA", DbType.Binary, arquivo);
                        db.AddInParameter(command, "EMISSOR", DbType.String, codEmissor);
                        db.AddInParameter(command, "MES", DbType.String, mesArquivo);
                        db.AddInParameter(command, "ANO", DbType.String, anoArquivo);
                        db.AddInParameter(command, "DATACRIACAO", DbType.DateTime, dataCriacao);

                        Logger.GravarLog("Chamada à procedure sp_ins_arquivo_emissores", command.Parameters);
                        db.ExecuteNonQuery(command);
                        retorno = true;

                    }
                }
                else
                {
                    retorno = AtualizarArquivo(arquivo, nomeArquivo, codEmissor, mesArquivo, anoArquivo, dataCriacao);

                }
                Logger.GravarLog("fim da chamada", retorno);
                return retorno;
            }
            catch (ArgumentNullException ex)
            {
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (DbException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(500, FONTE, ex);
            }
        }

        public bool ExisteArquivo(string codEmissor, string mesArquivo, string anoArquivo)
        {
            Logger.IniciarLog("Chamada método ExisteArquivo");
            try
            {
                Database db = this.SQLServerPN();
                bool retorno = false;
                //                using (DbCommand command = db.GetStoredProcCommand("sp_cons_existe_arquivo_emissores"))
                using (DbCommand command = db.GetSqlStringCommand(@"
                    SELECT Count(1) Qtd FROM TBPN078 WHERE EMISSOR = @EMISSOR AND  MES = @MES AND  ANO = @ANO"))
                {


                    db.AddInParameter(command, "EMISSOR", DbType.String, codEmissor);
                    db.AddInParameter(command, "MES", DbType.String, mesArquivo);
                    db.AddInParameter(command, "ANO", DbType.String, anoArquivo);

                    Logger.GravarLog("Chamada à procedure sp_ins_arquivo_emissores", command.Parameters);

                    Int32 quantiade = db.ExecuteScalar(command).ToString().ToInt32();
                    retorno = quantiade > 0;

                }
                Logger.GravarLog("fim da chamada", retorno);
                return retorno;
            }
            catch (ArgumentNullException ex)
            {
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (DbException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(500, FONTE, ex);
            }
        }
        public bool AtualizarArquivo(byte[] arquivo, string nomeArquivo, string codEmissor, string mesArquivo, string anoArquivo, DateTime dataCriacao)
        {
            Logger.IniciarLog("Chamada método AtualizarArquivo");
            try
            {
                Database db = this.SQLServerPN();
                bool retorno = false;
                //                using (DbCommand command = db.GetStoredProcCommand("sp_alt_arquivo_emissor"))
                using (DbCommand command = db.GetSqlStringCommand(@"
                    UPDATE dbo.TBPN078 
	                SET NAMEFILE = @NAMEFILE,
		                FILEDATA=@FILEDATA,
		                DATACRIACAO=@DATACRIACAO
	                WHERE EMISSOR = @EMISSOR 
	                AND  MES = @MES 
	                AND  ANO = @ANO"))
                {


                    db.AddInParameter(command, "NAMEFILE", DbType.String, nomeArquivo);
                    db.AddInParameter(command, "FILEDATA", DbType.Binary, arquivo);
                    db.AddInParameter(command, "EMISSOR", DbType.String, codEmissor);
                    db.AddInParameter(command, "MES", DbType.String, mesArquivo);
                    db.AddInParameter(command, "ANO", DbType.String, anoArquivo);
                    db.AddInParameter(command, "DATACRIACAO", DbType.DateTime, dataCriacao);
                    Logger.GravarLog("Chamada à procedure sp_alt_arquivo_emissores", command.Parameters);

                    db.ExecuteNonQuery(command);
                    retorno = true;
                }
                Logger.GravarLog("fim da chamada", retorno);
                return retorno;
            }
            catch (ArgumentNullException ex)
            {
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (DbException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(500, FONTE, ex);
            }
        }


        #endregion

        /// <summary>
        /// Lista os dados de Solicitação de Tecnologia a partir do número do PV
        /// </summary>
        /// <param name="numPV"></param>
        /// <returns></returns>
        public List<DadosSolicitacaoTecnologia> ConsultarTecnologia(int numPV)
        {
            Logger.IniciarLog("Chamada ConsultarTecnologia");
            try
            {
                GenericDatabase db = SybaseTG();

                List<DadosSolicitacaoTecnologia> dadosSolicitacao = null;
#if DEBUG
                dadosSolicitacao = new List<DadosSolicitacaoTecnologia>();
                DadosSolicitacaoTecnologia dado = new DadosSolicitacaoTecnologia();

                for (int i = 1; i < 5; i++)
                {
                    dado.Data = DateTime.Now.AddDays(-i);
                    dado.NumeroSolicitacao = new Random(i * 100).Next();
                    dado.TipoEquipamento = "P";

                    dado.DataInstalacao = DateTime.Now.AddDays(-i);

                    dado.Status = "A";
                    dadosSolicitacao.Add(dado);
                }
#else
                using (DbCommand command = db.GetStoredProcCommand("sptg5151"))
                {
                    db.AddInParameter(command, "@NUM_PDV", DbType.Int32, numPV);
                    Logger.GravarLog("Chamada à procedure sptg5151", command.Parameters);
                    using (IDataReader leitor = db.ExecuteReader(command))
                    {
                        dadosSolicitacao = new List<DadosSolicitacaoTecnologia>();
                        while (leitor.Read())
                        {
                            DadosSolicitacaoTecnologia dado = new DadosSolicitacaoTecnologia();
                            dado.Data = leitor["dth_cdst_fct"].ToString().ToDate();
                            dado.NumeroSolicitacao = leitor["num_hdsk_estb"].ToString().ToInt32();
                            dado.TipoEquipamento = leitor["cod_tip_eqpm"].ToString();

                            dado.DataInstalacao = leitor["dth_inst_term"].ToString().ToDateTimeNull();

                            dado.Status = leitor["cod_sit_fct"].ToString();
                            dadosSolicitacao.Add(dado);
                        }
                    }
                }
#endif
                if (dadosSolicitacao.Count == 0) return null;

                Logger.GravarLog("fim da chamada", dadosSolicitacao);
                return dadosSolicitacao;
            }
            catch (ArgumentNullException ex)
            {
                throw new PortalRedecardException(502, FONTE, ex);
            }
            catch (DbException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(502, FONTE, ex);
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(502, FONTE, ex);
            }

        }

        #endregion

        #region Integradores
        /// <summary>
        /// Lista os Integradores
        /// </summary>
        /// <returns></returns>
        public List<Integrador> ConsultarIntegrador(string codIntegrador, string situacao)
        {
            Logger.IniciarLog("Início método ConsultarIntegrador");
            try
            {
                GenericDatabase db = SybaseTG();
                List<Integrador> integradores = new List<Integrador>();
                using (DbCommand command = db.GetStoredProcCommand("sptg5091"))
                {

                    db.AddInParameter(command, "@COD_INTG", DbType.AnsiString, codIntegrador);
                    db.AddInParameter(command, "@SIT_INTG", DbType.AnsiString, situacao);

                    Logger.GravarLog("Chamada à procedure sptg5091", command.Parameters);
                    using (IDataReader leitor = db.ExecuteReader(command))
                    {
                        integradores = new List<Integrador>();
                        while (leitor.Read())
                        {
                            Integrador integrador = new Integrador();

                            integrador.Codigo = leitor["cod_intg"].ToString();
                            integrador.Descricao = leitor["nom_intg"].ToString();
                            integrador.Situacao = leitor["cod_sit_intg"].ToString();
                            integrador.CodigoUltimaAtualizacao = leitor["cod_opid_ult_atlz"].ToString().ToInt32();
                            integrador.DataAtualizacao = leitor["dth_ult_atlz"].ToString().ToDate();
                            //integrador.DataAtualizacaoTabela = leitor["dth_atlz_tab"].ToString().ToDate();

                            integradores.Add(integrador);
                        }
                    }
                }

                if (integradores.Count == 0) return null;
                Logger.GravarLog("Retorno do método ", integradores);

                return integradores;
            }
            catch (ArgumentNullException ex)
            {
                throw new PortalRedecardException(502, FONTE, ex);
            }
            catch (DbException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(502, FONTE, ex);
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(502, FONTE, ex);
            }
        }

        #endregion

        #region Equipamentos
        /// <summary>
        /// Lista os Equipamentos (Maquinetas)
        /// </summary>
        /// <returns></returns>
        public List<Equipamento> ConsultarEquipamento(out int codigoRetorno, out string mensagemRetorno)
        {
            Logger.IniciarLog("Início método ConsultarEquipamento");

            try
            {
                GenericDatabase db = SybaseTG();
                List<Equipamento> equipamentos = null;
                codigoRetorno = 0;
                mensagemRetorno = string.Empty;

                using (DbCommand command = db.GetStoredProcCommand("sptg5085"))
                {
                    // Adicionar Parametros
                    db.AddInParameter(command, "@COD_TIP_EQPM", DbType.AnsiString, null);
                    db.AddInParameter(command, "@COD_SIT_EQPM", DbType.AnsiString, null);
                    db.AddInParameter(command, "@IND_TCNL_CMPH", DbType.AnsiString, null);

                    Logger.GravarLog("Chamada à procedure sptg5085", command.Parameters);
                    using (IDataReader leitor = db.ExecuteReader(command))
                    {
                        equipamentos = new List<Equipamento>();
                        while (leitor.Read())
                        {
                            if (leitor.FieldCount > 2)
                            {
                                Equipamento equipamento = new Equipamento();

                                equipamento.Codigo = leitor["cod_tip_eqpm"].ToString();
                                equipamento.Descricao = leitor["des_tip_eqpm"].ToString();
                                equipamento.ValorMinimo = leitor["val_min_algl_eqpm"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(leitor["val_min_algl_eqpm"].ToString().Trim().Replace(",", "."));
                                equipamento.ValorMaximo = leitor["val_max_algl_eqpm"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(leitor["val_max_algl_eqpm"].ToString().Trim().Replace(",", "."));
                                equipamento.ValorVenda = leitor["val_dflt_algl_eqpm"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(leitor["val_dflt_algl_eqpm"].ToString().Trim().Replace(",", "."));
                                equipamento.DataUltAtualizacao = leitor["dth_ult_atlz"].ToString().ToDate();
                                equipamento.IndTecnologia = leitor["ind_tcnl_cmph"].ToString();
                                equipamento.Situacao = leitor["cod_sit_eqpm"].ToString();

                                equipamentos.Add(equipamento);
                            }
                            else
                            {
                                codigoRetorno = leitor["cod_err"].ToString().ToInt32();
                                mensagemRetorno = leitor["des_err"].ToString();
                            }
                            Logger.GravarLog("Retorno chamada ao método HIS ", new { });
                        }
                    }
                }

                if (equipamentos.Count == 0) return null;
                Logger.GravarLog("Retorno do método ", new { codigoRetorno, equipamentos });
                return equipamentos.Where(x => x.Codigo.Trim() == "PDV" || x.Codigo.Trim() == "POS").ToList();
            }
            catch (ArgumentNullException ex)
            {
                throw new PortalRedecardException(502, FONTE, ex);
            }
            catch (DbException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(502, FONTE, ex);
            }
            catch (Exception ex)
            {
                throw new PortalRedecardException(502, FONTE, ex);
            }
        }

        #endregion

        #region Consulta Trava Domicilio
        /// <summary>
        /// Consulta de Limite do Emissor
        /// </summary>
        /// <param name="numEmissor">Numero do Emissor</param>
        /// <returns>Valor Limite</returns>
        public decimal ConsultaLimite(int numEmissor, out int codigoRetorno, out string mensagemRetorno)
        {
            Logger.IniciarLog("Início método ConsultaLimite");
            try
            {
                decimal limite = 0;

                GenericDatabase db = SybaseFB();
                codigoRetorno = 0;
                mensagemRetorno = string.Empty;
                using (DbCommand command = db.GetStoredProcCommand("spfb5143"))
                {

                    db.AddInParameter(command, "@PM_COD_BCO_CMPS", DbType.Int32, numEmissor);

                    Logger.GravarLog("Chamada à procedure spfb5143 ", command.Parameters);

                    using (IDataReader leitor = db.ExecuteReader(command))
                    {
                        while (leitor.Read())
                        {
                            codigoRetorno = leitor["cod_err"].ToString().ToInt32();
                            mensagemRetorno = leitor["des_err"].ToString();

                            Logger.GravarLog("Retorno chamada procedure", new { codigoRetorno, mensagemRetorno });

                            if (codigoRetorno == 0)
                            {
                                limite = leitor[5].ToString().ToDecimal();
                            }
                        }
                    }
                }

                Logger.GravarLog("Retorno do método ", new { codigoRetorno, mensagemRetorno, limite });

                return limite;
            }
            catch (ArgumentNullException ex)
            {
                throw new PortalRedecardException(510, FONTE, ex);
            }
            catch (DbException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(510, FONTE, ex);
            }
            catch (Exception ex)
            {

                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(510, FONTE, ex);
            }
        }

        /// <summary>
        /// Consulta PV Travado
        /// </summary>
        /// <param name="numEmissor">Emissor</param>
        /// <param name="cnpj">CNPJ</param>
        /// <returns>PVs</returns>
        public List<DadosPV> ConsultaPVTravado(int numEmissor, int cnpj)
        {
            Logger.IniciarLog("Início método ConsultaPVTravado");
            try
            {
                List<DadosPV> dados = null;

                GenericDatabase db = SybaseGE();

                using (DbCommand command = db.GetStoredProcCommand("spge5618"))
                {
                    // Adicionar Parametros
                    db.AddInParameter(command, "@EMISSOR", DbType.Int32, numEmissor);
                    db.AddInParameter(command, "@CNPJ", DbType.Int32, cnpj);
                    Logger.GravarLog("Chamada à procedure spge5618 ", command.Parameters);
                    using (IDataReader leitor = db.ExecuteReader(command))
                    {
                        dados = new List<DadosPV>();
                        while (leitor.Read())
                        {
                            DadosPV dado = new DadosPV();
                            dado.NumPV = leitor["num_pdv"].ToString().ToInt32();
                            dado.NomeEstabelecimento = leitor["nom_rzoa_scla_estb"].ToString();
                            dado.CGC = leitor["num_cgc_estb"].ToString();
                            dado.TipoPod = leitor["sgl_tip_prod"].ToString();
                            dado.Situacao = leitor["situacao"].ToString();
                            //dado.TipoPV = leitor["r1_num_pdv"].ToString();
                            //dado.Indicador = leitor["indc_vd_dgtd_car_rco"].ToString();

                            dados.Add(dado);
                        }
                    }
                }

                if (dados.Count == 0) return null;

                Logger.GravarLog("Retorno do método ", dados);

                return dados;
            }
            catch (ArgumentNullException ex)
            {
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (DbException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(501, FONTE, ex);
            }

        }

        /// <summary>
        /// Consulta PV Não Travado
        /// </summary>
        /// <param name="numEmissor">Emissor</param>
        /// <param name="cnpj">CNPJ</param>
        /// <returns>PVs</returns>
        public List<DadosPV> ConsultaPVNaoTravado(int numEmissor, int cnpj)
        {
            Logger.IniciarLog("Início método ConsultaPVNaoTravado");
            try
            {
                List<DadosPV> dados = null;
                GenericDatabase db = SybaseGE();
                using (DbCommand command = db.GetStoredProcCommand("spge5617"))
                {

                    // Adicionar Parametros
                    db.AddInParameter(command, "@EMISSOR", DbType.Int32, numEmissor);
                    db.AddInParameter(command, "@CNPJ", DbType.Int32, cnpj);
                    Logger.GravarLog("Chamada à procedure spge5617", command.Parameters);
                    using (IDataReader leitor = db.ExecuteReader(command))
                    {
                        dados = new List<DadosPV>();
                        while (leitor.Read())
                        {
                            DadosPV dado = new DadosPV();
                            dado.NumPV = leitor["num_pdv"].ToString().ToInt32();
                            dado.NomeEstabelecimento = leitor["nom_rzoa_scla_estb"].ToString();
                            dado.CGC = leitor["num_cgc_estb"].ToString();
                            dado.TipoPod = leitor["sgl_tip_prod"].ToString();
                            dado.Situacao = string.Empty;
                            dado.TipoPV = leitor["r1_num_pdv"].ToString();
                            dado.Indicador = leitor["indc_vd_dgtd_car_rco"].ToString();

                            dados.Add(dado);
                        }
                    }
                }
                if (dados.Count == 0) return null;
                Logger.GravarLog("Retorno do método ", dados);
                return dados;
            }
            catch (ArgumentNullException ex)
            {
                throw new PortalRedecardException(509, FONTE, ex);
            }
            catch (DbException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (Exception ex)
            {

                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(501, FONTE, ex);
            }
        }

        public TotaisPV ConsultaTotaisPV(int numEmissor, int cnpj)
        {
            Logger.IniciarLog("Início método ConsultaPVNaoTravado");
            try
            {
                TotaisPV totais = null;
                GenericDatabase db = SybaseGE();
                using (DbCommand command = db.GetStoredProcCommand("spge5619"))
                {

                    // Adicionar Parametros
                    db.AddInParameter(command, "@EMISSOR", DbType.Int32, numEmissor);
                    db.AddInParameter(command, "@CNPJ", DbType.Int32, cnpj);
                    Logger.GravarLog("Chamada à procedure spge5619", command.Parameters);
                    using (IDataReader leitor = db.ExecuteReader(command))
                    {

                        if (leitor.Read())
                        {
                            totais = new TotaisPV()
                            {
                                TotalNaoDomiciliados = leitor["TOTNDOM"].ToString().ToInt32(),
                                TotalDomiciliados = leitor["TOTSDOM"].ToString().ToInt32(),
                                TotalCancelados = leitor["TOTCANC"].ToString().ToInt32()
                            };
                        }
                    }
                }

                Logger.GravarLog("Retorno do método ", totais);
                return totais;
            }
            catch (ArgumentNullException ex)
            {
                throw new PortalRedecardException(509, FONTE, ex);
            }
            catch (DbException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(501, FONTE, ex);
            }
            catch (Exception ex)
            {

                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(501, FONTE, ex);
            }
        }


        #endregion

        #region DadosVendedor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipoPessoa"></param>
        /// <param name="CpfCnpj"></param>
        /// <returns></returns>
        public DadosVendedor ConsultaVendedor(string tipoPessoa, string CpfCnpj)
        {
            Logger.IniciarLog("Início método ConsultaVendedor");
            try
            {
                GenericDatabase db = SybaseWF();//WF
                DadosVendedor vendedores = null;
                using (DbCommand command = db.GetStoredProcCommand("spwf5001"))
                {
                    db.AddInParameter(command, "@cod_tip_pes", DbType.String, tipoPessoa);
                    db.AddInParameter(command, "@num_cgc_estb", DbType.Decimal, CpfCnpj.ToDecimal());

                    Logger.GravarLog("Chamada à procedure spwf5001", command.Parameters);
                    using (IDataReader leitor = db.ExecuteReader(command))
                    {
                        while (leitor.Read())
                        {
                            if (leitor.FieldCount > 2)
                            {
                                vendedores = new DadosVendedor
                                    {
                                        Cpf = leitor["cod_cpf_vdd_pdv"].ToString(),
                                        DataFundacao = leitor["dat_fund_estb_srsa"].ToString().ToDate()
                                    };
                            }
                        }
                    }
                }

                Logger.GravarLog("Retorno do método ", vendedores);
                return vendedores;
            }
            catch (ArgumentNullException ex)
            {
                throw new PortalRedecardException(511, FONTE, ex);
            }
            catch (DbException ex)
            {
                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(511, FONTE, ex);
            }
            catch (Exception ex)
            {

                Logger.GravarErro("Exceção:", ex);
                throw new PortalRedecardException(511, FONTE, ex);
            }
        }
        #endregion

        #region PrePagamento

        /// <summary>
        /// Pré-Pagamento Detalhado
        /// </summary>
        /// <param name="numEmissor">Emissor</param>
        /// <param name="dataInicial">Data Inicial</param>
        /// <param name="dataFinal">Data Final</param>
        /// <returns></returns>
        public List<SaldoPrePagamento> SaldoDetalhadoPrePagamento(int numEmissor, DateTime dataInicial, DateTime dataFinal)
        {
            using (Logger Log = Logger.IniciarLog("Início Saldo Detalhado do Pré Pagamento"))
            {
                try
                {
                    List<SaldoPrePagamento> Saldo = new List<SaldoPrePagamento>();

                    OracleDatabase db = this.OracleRQ();
                    using (DbCommand command = db.GetStoredProcCommand("rq.sprq0009"))
                    {

                        db.AddInParameter(command, "p_COD_BAC", DbType.Int32, numEmissor);
                        db.AddInParameter(command, "dataInicioPeriodo", DbType.DateTime, dataInicial);
                        db.AddInParameter(command, "dataFimPeriodo", DbType.DateTime, dataFinal);
                        db.AddInParameter(command, "tipoSolicitacao", DbType.Int32, 2);

                        OracleParameter resultPrePagamentos = new OracleParameter();
                        resultPrePagamentos.Direction = ParameterDirection.Output;
                        resultPrePagamentos.ParameterName = "resultCursor";
                        resultPrePagamentos.Size = 100;
                        resultPrePagamentos.Value = null;
                        resultPrePagamentos.OracleType = OracleType.Cursor;
                        command.Parameters.Add(resultPrePagamentos);

                        using (IDataReader leitor = db.ExecuteReader(command))
                        {
                            while (leitor.Read())
                            {
                                SaldoPrePagamento s = new SaldoPrePagamento();

                                s.SaldoLiquido = leitor["VAL_SLDO_LQDO"].ToString().ToDecimal();
                                s.SaldoPagar = leitor["VAL_BRUT"].ToString().ToDecimal();
                                s.ValorAntecipado = leitor["VAL_SLDO_ANTC"].ToString().ToDecimal();
                                s.Vencimento = leitor["DATA_VENC"].ToString().ToDate();

                                Saldo.Add(s);
                            }
                        }
                    }

                    return Saldo;
                }
                catch (ArgumentNullException ex)
                {
                    throw new PortalRedecardException(509, FONTE, ex);
                }
                catch (DbException ex)
                {
                    throw new PortalRedecardException(509, FONTE, ex);
                }
                catch (Exception ex)
                {
                    throw new PortalRedecardException(509, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Saldo Detalhado do Pré Pagamento
        /// </summary>
        /// <param name="numEmissor">Emissor</param>
        /// <param name="dataInicial">Data Inicial</param>
        /// <param name="dataFinal">data Final</param>
        /// <returns></returns>
        public SaldoPrePagamento SaldoConsolidadoPrePagamento(int numEmissor, DateTime dataInicial, DateTime dataFinal)
        {
            using (Logger Log = Logger.IniciarLog("Início Saldo Detalhado do Pré Pagamento"))
            {
                Log.GravarLog(EventoLog.InicioDados, new { numEmissor, dataInicial, dataFinal });

                try
                {
                    OracleDatabase db = this.OracleRQ();

                    SaldoPrePagamento saldo = new SaldoPrePagamento();
                    using (DbCommand command = db.GetStoredProcCommand("rq.sprq0009"))
                    {

                        db.AddInParameter(command, "p_COD_BAC", DbType.Int32, numEmissor);
                        db.AddInParameter(command, "dataInicioPeriodo", DbType.Date, dataInicial);
                        db.AddInParameter(command, "dataFimPeriodo", DbType.Date, dataFinal);
                        db.AddInParameter(command, "tipoSolicitacao", DbType.Int32, 1); // 1 indica consolidado

                        OracleParameter resultPrePagamentos = new OracleParameter();
                        resultPrePagamentos.Direction = ParameterDirection.Output;
                        resultPrePagamentos.ParameterName = "resultCursor";
                        resultPrePagamentos.Size = 100;
                        resultPrePagamentos.Value = null;
                        resultPrePagamentos.OracleType = OracleType.Cursor;
                        command.Parameters.Add(resultPrePagamentos);

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });

                        using (IDataReader leitor = db.ExecuteReader(command))
                        {
                            Log.GravarLog(EventoLog.RetornoDados);
                            while (leitor.Read())
                            {
                                saldo.SaldoLiquido = leitor["VAL_SLDO_LQDO"].ToString().ToDecimal();
                                saldo.SaldoPagar = leitor["VAL_BRUT"].ToString().ToDecimal();
                                saldo.ValorAntecipado = leitor["VAL_SLDO_ANTC"].ToString().ToDecimal();
                                saldo.Vencimento = leitor["DATA_VENC"].ToString().ToDate();
                            }
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { saldo });
                    return saldo;
                }
                catch (ArgumentNullException ex)
                {
                    throw new PortalRedecardException(509, FONTE, ex);
                }
                catch (DbException ex)
                {
                    throw new PortalRedecardException(509, FONTE, ex);
                }
                catch (Exception ex)
                {
                    throw new PortalRedecardException(509, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Executa a procedure que ajusta a carga de confirmados
        /// </summary>
        /// <returns></returns>
        public Boolean AjustarCargaConfirmados()
        {
            using (Logger Log = Logger.IniciarLog("Início Ajustar Carga Confirmados"))
            {
                Log.GravarLog(EventoLog.InicioDados);

                try
                {
                    OracleDatabase db = this.OracleRQ();

                    using (DbCommand command = db.GetStoredProcCommand("rq.sprq0028"))
                    {
                        Log.GravarLog(EventoLog.ChamadaDados);
                        db.ExecuteNonQuery(command);
                    }
                    Log.GravarLog(EventoLog.FimDados);

                    return true;
                }
                catch (ArgumentNullException ex)
                {
                    throw new PortalRedecardException(509, FONTE, ex);
                }
                catch (DbException ex)
                {
                    throw new PortalRedecardException(509, FONTE, ex);
                }
                catch (Exception ex)
                {
                    throw new PortalRedecardException(509, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta os Pré-Pagamentos carregados do KN de acordo com os parâmetros filtrados
        /// </summary>
        /// <param name="codigoBacen">Código do Emissor do Pré-Pagamento. 
        /// Passar como 0 para retornar dos os dados.</param>
        /// <param name="dataInicial">Período de Vencimento inicial dos Pré-Pagamentos</param>
        /// <param name="dataFinal">Período de Vencimento final dos Pré-Pagamentos</param>
        /// <param name="bandeiras">Listagem de bandeiras e código EmissorBandeira a serem filtradas</param>
        /// <returns>Listagem de Pré-Pagamentos retornados</returns>
        public List<Modelos.PrePagamento> ConsultarPrePagamento(Int32 codigoBacen, DateTime dataInicial, DateTime dataFinal, List<Bandeira> bandeiras)
        {
            using (Logger Log = Logger.IniciarLog("Início Consulta os Pré-Pagamentos carregados do KN de acordo com os parâmetros filtrados"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    OracleDatabase db = base.OracleRQ();
                    List<PrePagamento> prePagamentos = new List<PrePagamento>();

                    using (DbCommand cmd = db.GetStoredProcCommand("rq.sprq0031"))
                    {
                        db.AddInParameter(cmd, "p_data_inicio", DbType.Date, dataInicial);
                        db.AddInParameter(cmd, "p_data_fim", DbType.Date, dataFinal);
                        if (codigoBacen == 0)
                            db.AddInParameter(cmd, "p_cod_bac", DbType.Int32, DBNull.Value);
                        else
                            db.AddInParameter(cmd, "p_cod_bac", DbType.Int32, codigoBacen);

                        OracleParameter prePagamentoCursor = new OracleParameter();
                        prePagamentoCursor.Direction = ParameterDirection.Output;
                        prePagamentoCursor.ParameterName = "p_pre_pagamento_cur";
                        prePagamentoCursor.Size = 100;
                        prePagamentoCursor.Value = null;
                        prePagamentoCursor.OracleType = OracleType.Cursor;
                        cmd.Parameters.Add(prePagamentoCursor);

                        OracleParameter codigoRetorno = new OracleParameter();
                        codigoRetorno.Direction = ParameterDirection.Output;
                        codigoRetorno.ParameterName = "p_codigo_retorno";
                        codigoRetorno.Value = 0;
                        codigoRetorno.OracleType = OracleType.Int32;
                        cmd.Parameters.Add(codigoRetorno);

                        OracleParameter mensagemRetorno = new OracleParameter();
                        mensagemRetorno.Direction = ParameterDirection.Output;
                        mensagemRetorno.ParameterName = "p_mensagem_retorno";
                        mensagemRetorno.Value = "";
                        mensagemRetorno.OracleType = OracleType.VarChar;
                        cmd.Parameters.Add(mensagemRetorno);

                        Log.GravarLog(EventoLog.ChamadaDados, new { cmd.Parameters });

                        using (IDataReader dr = db.ExecuteReader(cmd))
                        {
                            Log.GravarLog(EventoLog.RetornoDados, new { codigoRetorno, mensagemRetorno });

                            while (dr.Read())
                            {
                                PrePagamento prePagamento = PreencherModeloPrePagamento(dr);
                                prePagamentos.Add(prePagamento);
                            }

                            prePagamentos = this.FiltrarBandeiraEmissor(prePagamentos, bandeiras);

                            Log.GravarLog(EventoLog.RetornoDados, new { prePagamentos });
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { prePagamentos });
                    return prePagamentos;
                }
                catch (ArgumentNullException ex)
                {
                    throw new PortalRedecardException(509, FONTE, ex);
                }
                catch (DbException ex)
                {
                    throw new PortalRedecardException(509, FONTE, ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(509, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Preeenche o modelo do Pré-Pagamento de acordo com o retorno do RQ
        /// </summary>
        /// <param name="leitorPrepagamento">IDataReader com um registro de Pré-Pagamento</param>
        /// <returns>Modelo do Pré-Pagamento</returns>
        private PrePagamento PreencherModeloPrePagamento(IDataReader leitorPrepagamento)
        {
            PrePagamento prePagamento = new PrePagamento();

            if (leitorPrepagamento != null)
            {
                prePagamento.DataVencimento = leitorPrepagamento["dat_vcto"].ToString().ToDate();
                prePagamento.Bandeira = new Bandeira()
                {
                    Codigo = leitorPrepagamento["cod_bndr"].ToString().ToInt32()
                };
                prePagamento.Bandeira.EmissoresBandeiras = new List<EmissorBandeira>();
                prePagamento.Bandeira.EmissoresBandeiras.Add(new EmissorBandeira()
                {
                    Codigo = leitorPrepagamento["cod_bac_bndr"].ToString().ToInt64()
                });
                prePagamento.Banco = new Emissor()
                {
                    Codigo = leitorPrepagamento["cod_bac"].ToString().ToInt32()
                };


                prePagamento.ValorBruto = leitorPrepagamento["val_sldo_rcbr"].ToString().ToDouble();
                prePagamento.ValorFEE = leitorPrepagamento["val_sldo_fee"].ToString().ToDouble();
                prePagamento.ValorPagarReceber = leitorPrepagamento["val_sldo_lqdo"].ToString().ToDouble();
                prePagamento.SaldoAntecipado = leitorPrepagamento["val_sldo_antc"].ToString().ToDouble();

                prePagamento.SaldoEstoque = prePagamento.ValorPagarReceber - prePagamento.SaldoAntecipado;
            }

            return prePagamento;
        }

        /// <summary>
        /// Filtra os Pré-Pagamanetos de acordo com as bandeiras e código ICA/BID
        /// </summary>
        /// <param name="prePagamentos">Lista de Pré-Pagamentos retornada pelo RQ</param>
        /// <param name="bandeiras">Listagem de bandeiras e ICA/BID para filtrar os dados</param>
        /// <returns>Listagem de Pré-Pagamentos filtradas</returns>
        private List<PrePagamento> FiltrarBandeiraEmissor(List<PrePagamento> prePagamentos, List<Bandeira> bandeiras)
        {
            List<PrePagamento> prePagamentosFiltrados = new List<PrePagamento>();
            List<PrePagamento> _prePagamentosFiltrados;
            if (bandeiras.Count == 0)
            {
                prePagamentosFiltrados = prePagamentos;
            }
            else
            {
                foreach (Bandeira bandeira in bandeiras)
                {
                    _prePagamentosFiltrados = new List<PrePagamento>();

                    if (bandeira.EmissoresBandeiras.Count > 0)
                    {
                        foreach (EmissorBandeira emissorBandeira in bandeira.EmissoresBandeiras)
                        {
                            _prePagamentosFiltrados.AddRange(prePagamentos.Where(pre =>
                                                pre.Bandeira.Codigo == bandeira.Codigo
                                                && pre.Bandeira.EmissoresBandeiras[0].Codigo == emissorBandeira.Codigo
                                                ).ToList());
                        }
                    }
                    else
                    {
                        _prePagamentosFiltrados = prePagamentos.Where(pre =>
                                                pre.Bandeira.Codigo == bandeira.Codigo).ToList();
                    }

                    prePagamentosFiltrados.AddRange(_prePagamentosFiltrados);
                }
            }

            return prePagamentosFiltrados;
        }

        /// <summary>
        /// Consulta todos os Bancos(Emissores) com Pré-Pagamentos carregados
        /// </summary>
        /// <returns>List of Modelos.Banco Listagem Bancos(Emissores) com Pré-Pagamentos</returns>
        public List<Modelos.Emissor> ConsultarEmissores()
        {
            using (Logger Log = Logger.IniciarLog("Início Consulta todos os Bancos(Emissores) com Pré-Pagamentos carregados"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    OracleDatabase db = base.OracleRQ();
                    List<Emissor> bancosPrePagamento = new List<Emissor>();

                    using (DbCommand cmd = db.GetStoredProcCommand("rq.sprq0030"))
                    {
                        OracleParameter bacenCursor = new OracleParameter(
                            "p_bac_cur", null);
                        bacenCursor.Direction = ParameterDirection.Output;
                        bacenCursor.Size = 100;
                        bacenCursor.OracleType = OracleType.Cursor;
                        cmd.Parameters.Add(bacenCursor);

                        OracleParameter codigoRetorno = new OracleParameter(
                            "p_codigo_retorno", null);
                        codigoRetorno.Direction = ParameterDirection.Output;
                        codigoRetorno.OracleType = OracleType.Int32;
                        codigoRetorno.Value = 0;
                        cmd.Parameters.Add(codigoRetorno);

                        OracleParameter mensagemRetorno = new OracleParameter(
                            "p_mensagem_retorno", null);
                        mensagemRetorno.Direction = ParameterDirection.Output;
                        mensagemRetorno.OracleType = OracleType.VarChar;
                        mensagemRetorno.Value = "";
                        cmd.Parameters.Add(mensagemRetorno);

                        Log.GravarLog(EventoLog.ChamadaDados, new { cmd.Parameters });

                        using (IDataReader dr = db.ExecuteReader(cmd))
                        {
                            Log.GravarLog(EventoLog.RetornoDados, new { codigoRetorno, mensagemRetorno });

                            while (dr.Read())
                            {
                                Emissor bancoPrePagamento = PreencherModeloBancoPrePagamento(dr);
                                bancosPrePagamento.Add(bancoPrePagamento);
                            }

                            Log.GravarLog(EventoLog.RetornoDados, new { bancosPrePagamento });
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { bancosPrePagamento });
                    return bancosPrePagamento;
                }
                catch (ArgumentNullException ex)
                {
                    throw new PortalRedecardException(509, FONTE, ex);
                }
                catch (DbException ex)
                {
                    throw new PortalRedecardException(509, FONTE, ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(509, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Preenche um objeto com a classe Modelo de Bancos
        /// </summary>
        /// <param name="leitorBanco">IDataReader com um registro de Banco</param>
        /// <returns>Modelos.Banco</returns>
        private Modelos.Emissor PreencherModeloBancoPrePagamento(IDataReader leitorBanco)
        {
            Emissor banco = new Emissor();

            if (leitorBanco != null)
            {
                banco.Codigo = leitorBanco["cod_bac"].ToString().ToInt32();
            }

            return banco;
        }

        /// <summary>
        /// Consulta todos os Códigos Emissor-Bandeira existente para os Pré-Pagamentos
        /// </summary>
        /// <param name="codigoBacen">Código do Emissor a filtrar as bandeiras</param>
        /// <returns>List of Modelos.Bandeira Listagem de Emissor-Bandeira com Pré-Pagamentos</returns>
        public List<Modelos.Bandeira> ConsultarEmissoresBandeiras(Int32 codigoBacen)
        {
            using (Logger Log = Logger.IniciarLog("Início Consulta todos os Códigos Emissor-Bandeira existente para os Pré-Pagamentos"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    OracleDatabase db = base.OracleRQ();
                    List<Bandeira> emissoresBandeiras = new List<Bandeira>();

                    using (DbCommand cmd = db.GetStoredProcCommand("rq.sprq0029"))
                    {
                        if (codigoBacen == 0)
                            db.AddInParameter(cmd, "p_codigo_bacen", DbType.Int32, DBNull.Value);
                        else
                            db.AddInParameter(cmd, "p_codigo_bacen", DbType.Int32, codigoBacen);

                        OracleParameter bandeiraCursor = new OracleParameter(
                            "p_bdnr_cur", null);
                        bandeiraCursor.Direction = ParameterDirection.Output;
                        bandeiraCursor.Size = 100;
                        bandeiraCursor.OracleType = OracleType.Cursor;
                        cmd.Parameters.Add(bandeiraCursor);

                        OracleParameter codigoRetorno = new OracleParameter(
                            "p_codigo_retorno", null);
                        codigoRetorno.Direction = ParameterDirection.Output;
                        codigoRetorno.OracleType = OracleType.Int32;
                        cmd.Parameters.Add(codigoRetorno);

                        OracleParameter mensagemRetorno = new OracleParameter(
                            "p_mensagem_retorno", null);
                        mensagemRetorno.Direction = ParameterDirection.Output;
                        mensagemRetorno.OracleType = OracleType.VarChar;
                        mensagemRetorno.Value = "";
                        cmd.Parameters.Add(mensagemRetorno);

                        Log.GravarLog(EventoLog.ChamadaDados, new { cmd.Parameters });

                        using (IDataReader dr = db.ExecuteReader(cmd))
                        {
                            Log.GravarLog(EventoLog.RetornoDados, new { codigoRetorno, mensagemRetorno });

                            while (dr.Read())
                            {
                                Bandeira emissorBandeira = PreencherModeloEmissorBandeira(dr);
                                emissoresBandeiras.Add(emissorBandeira);
                            }

                            emissoresBandeiras = this.AgruparICABID(emissoresBandeiras);

                            Log.GravarLog(EventoLog.RetornoDados, new { emissoresBandeiras });
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { emissoresBandeiras });
                    return emissoresBandeiras;
                }
                catch (ArgumentNullException ex)
                {
                    throw new PortalRedecardException(509, FONTE, ex);
                }
                catch (DbException ex)
                {
                    throw new PortalRedecardException(509, FONTE, ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(509, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Preenche um objeto com a classe Modelo de Bandeira com seus Códigos ICA/BID
        /// </summary>
        /// <param name="leitrEmissorBandeira">IDataReader com um registro de uma bandeira Bandeira</param>
        /// <returns>Bandeira</returns>
        private Bandeira PreencherModeloEmissorBandeira(IDataReader leitorEmissorBandeira)
        {
            Bandeira bandeira = new Bandeira();

            if (leitorEmissorBandeira != null)
            {
                bandeira.Codigo = leitorEmissorBandeira["cod_bndr"].ToString().ToInt32();
                bandeira.EmissoresBandeiras = new List<EmissorBandeira>();
                bandeira.EmissoresBandeiras.Add(new EmissorBandeira()
                {
                    Codigo = leitorEmissorBandeira["cod_bac_bndr"].ToString().ToInt64()
                });
            }

            return bandeira;
        }

        /// <summary>
        /// Agrupa os ICA/BIDs de cada bandeira como uma lista de EmissorBandeira
        /// </summary>
        /// <param name="emissoresBandeiras">List of Bandeira para agrupamentos</param>
        /// <returns>List of Bandeira para agrupadas</returns>
        private List<Bandeira> AgruparICABID(List<Bandeira> emissoresBandeiras)
        {
            List<EmissorBandeira> icaBID = new List<EmissorBandeira>();
            List<Bandeira> listaAgrupada = new List<Bandeira>();
            List<Bandeira> _listaAgrupada = new List<Bandeira>();
            Bandeira novaBndr;

            Int32 codBac = 0;
            emissoresBandeiras = emissoresBandeiras.OrderBy(emi => emi.Codigo).ToList();

            foreach (Bandeira bandeira in emissoresBandeiras)
            {
                if (codBac != bandeira.Codigo)
                {
                    codBac = bandeira.Codigo;

                    novaBndr = new Bandeira()
                    {
                        Codigo = bandeira.Codigo
                    };

                    icaBID = new List<EmissorBandeira>();
                    _listaAgrupada = emissoresBandeiras.Where(emi => emi.Codigo == bandeira.Codigo).ToList();
                    foreach (Bandeira bndr in _listaAgrupada)
                    {
                        icaBID.Add(bndr.EmissoresBandeiras[0]);
                    }

                    novaBndr.EmissoresBandeiras = new List<EmissorBandeira>();
                    novaBndr.EmissoresBandeiras.AddRange(icaBID);
                    listaAgrupada.Add(novaBndr);
                }
            }

            return listaAgrupada;
        }

        /// <summary>
        /// Consulta todas as Bandeiras cadastradas no Oracle DR
        /// </summary>
        /// <returns>List of Modelos.Bandeira </returns>
        public List<Modelos.Bandeira> ConsultarBandeiras()
        {
            using (Logger Log = Logger.IniciarLog("Início Consulta todas as Bandeiras cadastradas no Oracle DR"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    OracleDatabase db = base.OracleDR();
                    List<Bandeira> bandeiras = new List<Bandeira>();

                    //SPDR010, out: cur_OUT
                    using (DbCommand cmd = db.GetStoredProcCommand("dr.SP_DR_TBDR0201_ALL"))
                    {
                        OracleParameter resultBacCursor = new OracleParameter(
                            "resultCursor", null);
                        resultBacCursor.Direction = ParameterDirection.Output;
                        resultBacCursor.Size = 100;
                        resultBacCursor.OracleType = OracleType.Cursor;
                        cmd.Parameters.Add(resultBacCursor);

                        Log.GravarLog(EventoLog.ChamadaDados, new { cmd.Parameters });

                        using (IDataReader dr = db.ExecuteReader(cmd))
                        {
                            Log.GravarLog(EventoLog.RetornoDados);

                            while (dr.Read())
                            {
                                Bandeira bandeira = PreencherModeloBandeira(dr);
                                bandeiras.Add(bandeira);
                            }

                            Log.GravarLog(EventoLog.RetornoDados, new { bandeiras });
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { bandeiras });
                    return bandeiras;
                }
                catch (ArgumentNullException ex)
                {
                    throw new PortalRedecardException(512, FONTE, ex);
                }
                catch (DbException ex)
                {
                    throw new PortalRedecardException(512, FONTE, ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(512, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leitorBandeira"></param>
        /// <returns></returns>
        private Bandeira PreencherModeloBandeira(IDataReader leitorBandeira)
        {
            Bandeira bandeira = new Bandeira();

            if (leitorBandeira != null)
            {
                bandeira.Codigo = leitorBandeira["COD_BNDR"].ToString().ToInt32();
                bandeira.Descricao = leitorBandeira["NOM_BNDR"].ToString();
            }

            return bandeira;
        }

        /// <summary>
        /// Consulta a listagem de Bancos reconhecidos pelo Bacen na base Sybase DR
        /// </summary>
        /// <returns>List of Modelos.Banco </returns>
        public List<Modelos.Emissor> ConsultarBancos()
        {
            using (Logger Log = Logger.IniciarLog("Início Consulta a listagem de Bancos reconhecidos pelo Bacen na base Sybase DR"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    GenericDatabase db = base.SybaseDR();
                    List<Modelos.Emissor> bancos = new List<Emissor>();

                    using (DbCommand cmd = db.GetStoredProcCommand("spdr0060"))
                    {
                        Log.GravarLog(EventoLog.ChamadaDados);

                        using (IDataReader dr = db.ExecuteReader(cmd))
                        {
                            Log.GravarLog(EventoLog.RetornoDados);

                            while (dr.Read())
                            {
                                Emissor banco = PreencherModeloBanco(dr);
                                bancos.Add(banco);
                            }

                            Log.GravarLog(EventoLog.RetornoDados, new { bancos });
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { bancos });
                    return bancos;
                }
                catch (ArgumentNullException ex)
                {
                    throw new PortalRedecardException(505, FONTE, ex);
                }
                catch (DbException ex)
                {
                    throw new PortalRedecardException(505, FONTE, ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(505, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leitorBanco"></param>
        /// <returns></returns>
        private Emissor PreencherModeloBanco(IDataReader leitorBanco)
        {
            Emissor banco = new Emissor();

            if (leitorBanco != null)
            {
                banco.Codigo = leitorBanco["cod_bco"].ToString().ToInt32();
                banco.Descricao = leitorBanco["nom_bco"].ToString();
            }

            return banco;
        }

        /// <summary>
        /// Realiza a carga de Pré-Pagamentos na base do RQ
        /// </summary>
        /// <param name="prePagamentos">Listagem de Pré-Pagamentos a carregar</param>
        /// <param name="confirmados">Indica se os Pré-Pagamentos são do tipo Confirmados ou Agendados/Parcelados.
        /// True - Grava na tabela TBRQ0006; False - Grava na tabela TBRQ0008 </param>
        /// <returns>List of PrePagamento - Listagem de pré-pagamentos que retornaram erro</returns>
        public List<PrePagamento> CarregarPrePagamentos(List<PrePagamento> prePagamentos, Boolean confirmados)
        {
            using (Logger Log = Logger.IniciarLog("Início Consulta a listagem de Bancos reconhecidos pelo Bacen na base Sybase DR"))
            {
                Log.GravarLog(EventoLog.InicioDados, new { prePagamentos, confirmados });

                OracleDatabase db = base.OracleRQ();
                String queryCarga;

                List<PrePagamento> prePagamentosErro = new List<PrePagamento>();
                PrePagamento prePagamentoErro;

                foreach (PrePagamento prePagamento in prePagamentos)
                {
                    try
                    {
                        queryCarga = @"INSERT INTO {0} (COD_BAC, COD_BNDR, COD_BAC_BNDR, DAT_VCTO, DAT_PCM, 
                                                    VAL_SLDO_FEE, VAL_SLDO_ANTC, VAL_SLDO_LQDO, VAL_SLDO_RCBR) VALUES
                                                   (:codBac, :codBndr, :codBacBndr, :dtVcto, :dtPcm, 
                                                    :vlFee, :vlAntc, :vlLqd, :vlRcbr)";

                        queryCarga = String.Format(queryCarga, confirmados ? "rq.tbrq0007" : "rq.tbrq0008");

                        prePagamentoErro = new PrePagamento();

                        using (DbCommand cmd = db.GetSqlStringCommand(queryCarga))
                        {
                            db.AddInParameter(cmd, "codBac", DbType.Int32, prePagamento.Banco.Codigo);
                            db.AddInParameter(cmd, "codBndr", DbType.Int32, prePagamento.Bandeira.Codigo);
                            db.AddInParameter(cmd, "codBacBndr", DbType.Int64, prePagamento.Bandeira.EmissoresBandeiras[0].Codigo);
                            db.AddInParameter(cmd, "dtVcto", DbType.DateTime, prePagamento.DataVencimento);
                            db.AddInParameter(cmd, "dtPcm", DbType.DateTime, prePagamento.DataProcessamento);
                            db.AddInParameter(cmd, "vlFee", DbType.Double, prePagamento.ValorFEE);
                            db.AddInParameter(cmd, "vlAntc", DbType.Double, prePagamento.SaldoAntecipado);
                            db.AddInParameter(cmd, "vlLqd", DbType.Double, prePagamento.SaldoEstoque);
                            db.AddInParameter(cmd, "vlRcbr", DbType.Double, prePagamento.ValorPagarReceber);

                            Log.GravarLog(EventoLog.ChamadaDados, new { cmd.Parameters });

                            Int32 codigoRetorno = db.ExecuteNonQuery(cmd);

                            Log.GravarLog(EventoLog.RetornoDados, new { codigoRetorno });
                        }
                    }
                    catch (ArgumentNullException ex)
                    {
                        Log.GravarErro(ex);
                        prePagamentoErro = prePagamento;
                        prePagamentoErro.ConfirmacaoCarga = false;
                        prePagamentoErro.MensagemErroCarga = ex.Message;
                        prePagamentosErro.Add(prePagamentoErro);
                    }
                    catch (DbException ex)
                    {
                        Log.GravarErro(ex);
                        prePagamentoErro = prePagamento;
                        prePagamentoErro.ConfirmacaoCarga = false;
                        prePagamentoErro.MensagemErroCarga = ex.Message;
                        prePagamentosErro.Add(prePagamentoErro);
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        prePagamentoErro = prePagamento;
                        prePagamentoErro.ConfirmacaoCarga = false;
                        prePagamentoErro.MensagemErroCarga = ex.Message;
                        prePagamentosErro.Add(prePagamentoErro);
                    }
                }

                Log.GravarLog(EventoLog.FimDados, new { prePagamentosErro });
                return prePagamentosErro;
            }
        }


        /// <summary>
        /// Exclui todos os pré-pagamentos carregados na base afim de realizar uma nova.
        /// </summary>
        /// <param name="tabela">Indica se a tabela é a de Confirmados ou Parcelados ou Temporária</param>
        /// <returns>Retorna se a execuçãou foi feita com sucesso</returns>
        public Boolean ExcluirPrePagamentos(String tabela)
        {
            using (Logger Log = Logger.IniciarLog("Início Exclui todos os pré-pagamentos parcelados carregados na base afim de realizar uma nova."))
            {
                Log.GravarLog(EventoLog.InicioDados);
                Boolean confirmacao = false;

                try
                {
                    OracleDatabase db = base.OracleRQ();
                    String queryCarga;
                    Int32 codigoRetorno = 0;

                    queryCarga = String.Format(@"DELETE FROM {0}", tabela);

                    Log.GravarLog(EventoLog.ChamadaDados);
                    using (DbCommand cmd = db.GetSqlStringCommand(queryCarga))
                    {
                        codigoRetorno = db.ExecuteNonQuery(cmd);
                        Log.GravarLog(EventoLog.RetornoDados, new { codigoRetorno });
                    }

                    confirmacao = (codigoRetorno == 0) || (codigoRetorno == 1);
                    Log.GravarLog(EventoLog.FimDados, new { confirmacao });
                }
                catch (ArgumentNullException ex)
                {
                    throw new PortalRedecardException(512, FONTE, ex);
                }
                catch (DbException ex)
                {
                    throw new PortalRedecardException(512, FONTE, ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(512, FONTE, ex);
                }

                return confirmacao;
            }
        }

        #endregion
    }
}
