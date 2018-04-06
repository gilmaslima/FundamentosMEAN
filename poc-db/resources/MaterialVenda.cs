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
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Redecard.PN.Comum;

namespace Redecard.PN.OutrosServicos.Dados
{
    /// <summary>
    /// Métodos de acesso a dados para o módulo de Outros Serviços
    /// </summary>
    public class MaterialVenda : BancoDeDadosBase
    {
        /// <summary>
        /// Códigos de Erro do método de inclusão de kits
        /// </summary>
        private Dictionary<Int32, Int32> _incluirKitErrorCode = new Dictionary<Int32, Int32>() { 
            { 1, 9101 },            // Ocorreu um erro na inclusão da solicitação
            { -1, 9101 },           // Ocorreu um erro na inclusão da entrega
            { -2, 9102 },           // Ocorreu um erro na inclusão do kit
            { -4, 9104 },           // Remessa já existe
            { -6, 6    },           // CEP Inválido
            { -5, 9105 },           // PV não encontrado na base do GS
            { -7, 9107 },           // PV cancelado ou c/ problemas de fraude
            { -8, 9108 },           // Kit não encontrado
            { -9, 9109 },           // Kit desativado
            { -10, 9110 },          // Região de distribuição não encontrada
            { -11, 9111 },          // Regra de distribuição não encontrada
            { -12, 9112 },          // PV está com trava de entrega
            { -18, 9118 },          // Solicitação está sendo feita dentro do período mínimo não permitido
            { -19, 9119 },          // Quantidade solicitada de Kits superior a quantidade permitida
            { -20, 9120 },          // Quantidade solicitada de Kits de Tecnologia superior a quantidade permitida
            { -200019, 200019 },    // Já existe remessa automática priorizada. Se desejar faça uma reclamação
            { -200020, 200020 },    // Já existe remessa automática baixada no mês
            { -300001, 300001 },    // Ainda não expirou o prazo mínimo para a solicitação do KIT
            { -300002, 300002 },    // Pedido excede a quantidade máxima deste KIT no período
            { -9092, 9092 },        // Endereço Temporário com regra de Distribuição Incompatível

            { 60013, 60013 },        // Opção Inválida
            { 60006, 60006 },        // PV não encontrado
            { 60004, 60004 }         // Número do PV não informado
        };

        /// <summary>
        /// Inclui uma ou mais solicitações de kits de materiais
        /// </summary>
        /// <returns>Retorna um código de retorno indicando sucesso ou falha na operação</returns>
        public Int32 IncluirKit(Modelo.Kit[] kits, Int32 codigoPV, String descricaoPV, String usuario, String solicitante, String remessaExtra, Boolean enderecoTemporario, Modelo.Endereco endereco = null)
        {
            GenericDatabase db = base.SybaseGS();
            DbTransaction trans = null;
            DbConnection con = null;
            Int32 codigoRetorno = 0;
            Int32 codigoEndereco = 0;
            Int32 regiao = 0;

            using (con = db.CreateConnection())
            {
                try
                {

                    con.Open();
                    trans = con.BeginTransaction();

                    // verifica se é necessário a atualização de endereço temporário
                    if (enderecoTemporario && !object.ReferenceEquals(endereco, null))
                    {
                        // validar cep do endereço temporário informado
                        regiao = this.ValidarCEP(endereco.CEP, db);
                        if (regiao > 0)
                        {
                            // verificar área de distribuição dos kits
                            foreach (Modelo.Kit kit in kits)
                                codigoRetorno = this.ValidarAreaDistribuicao(regiao, kit.CodigoKit, db);

                            if (codigoRetorno == 0)
                            {
                                // validar cep do endereço cadastrado, utilizado na regra de distribuição
                                String _cep = this.ConsultarCEPPV(codigoPV, out codigoRetorno);
                                if (codigoRetorno == 0)
                                {
                                    regiao = this.ValidarCEP(_cep, db);
                                    if (regiao > 0)
                                    {
                                        // verificar área de distribuição dos kits
                                        foreach (Modelo.Kit kit in kits)
                                            codigoRetorno = this.ValidarAreaDistribuicao(regiao, kit.CodigoKit, db);

                                        if (codigoRetorno == 0)
                                        {
                                            String cep = endereco.CEP.Replace("-", String.Empty);
                                            if (cep == _cep)
                                            {
                                                // fazer chamada de inclusão de endereço temporário
                                                codigoEndereco = this.IncluirEnderecoTemporario(endereco, codigoPV, db);
                                            }
                                            else
                                                codigoRetorno = -9092; // Regra de distribuição inválida
                                        }
                                    }
                                    else
                                        codigoRetorno = -6;
                                }
                            }
                        }
                        else
                            codigoRetorno = -6;
                    }
                    if (codigoRetorno == 0)
                    {
                        // inclui as novas solicitações
                        for (Int32 i = 0; i < kits.Length; i++)
                        {
                            Modelo.Kit kit = kits[i];
                            codigoRetorno = this.IncluirSolicitacaoKit(codigoPV, kit, codigoEndereco, solicitante, usuario, db);
                            if (codigoRetorno > 0)
                                break;
                        }
                        if (codigoRetorno == 0)
                            trans.Commit();
                        else
                        {
                            trans.Rollback();
                            if (_incluirKitErrorCode.ContainsKey(codigoRetorno))
                                codigoRetorno = _incluirKitErrorCode[codigoRetorno];
                        }
                    }
                    else
                    {
                        trans.Rollback();
                        if (_incluirKitErrorCode.ContainsKey(codigoRetorno))
                            codigoRetorno = _incluirKitErrorCode[codigoRetorno];
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new PortalRedecardException(507, FONTE, ex);
                }
            }
            return codigoRetorno;
        }

        /// <summary>
        /// Inclui nova solicitação de KIT de material
        /// </summary>
        /// <returns>
        /// Código de retorno indicando sucesso (Igual a 0 (Zero)) ou indicando falha (Diferente de 0 (Zero)) 
        /// </returns>
        private Int32 IncluirSolicitacaoKit(Int32 codigoPV, Modelo.Kit kit, Int32 codigoEndereco, String nomeSolicitante,
            String idUsuario, GenericDatabase db)
        {

            using (Logger Log = Logger.IniciarLog("Inclui nova solicitação de KIT de material"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    Int32 codigoRetorno = 0;
                    using (DbCommand command = db.GetStoredProcCommand("spgs0929"))
                    {
                        // adiciona os parâmetros de entrada necessários
                        db.AddInParameter(command, "@NUM_PDV", DbType.Int32, codigoPV);
                        db.AddInParameter(command, "@COD_KIT", DbType.Int32, kit.CodigoKit);
                        db.AddInParameter(command, "@COD_ENDR", DbType.Int32, codigoEndereco);
                        db.AddInParameter(command, "@COD_MOT", DbType.Int32, kit.Motivo.CodigoMotivo);
                        db.AddInParameter(command, "@NUM_MATR_USR", DbType.AnsiString, idUsuario);
                        db.AddInParameter(command, "@NOM_GRUP", DbType.AnsiString, "Internet");
                        db.AddInParameter(command, "@INDC_RMSA_EXTA", DbType.AnsiString, "N");
                        db.AddInParameter(command, "@INDC_TIP_ENT", DbType.AnsiString, "0");
                        db.AddInParameter(command, "@NOM_SLCT", DbType.AnsiString, nomeSolicitante);
                        db.AddInParameter(command, "@QTD_KIT", DbType.Int32, kit.Quantidade);
                        db.AddOutParameter(command, "@COD_SLC_KIT", DbType.Int32, 4);
                        db.AddInParameter(command, "@BATCH", DbType.AnsiString, "0");
                        db.AddInParameter(command, "@ORIGEM", DbType.AnsiString, "INTERNET");
                        db.AddOutParameter(command, "@COD_CNL_ENT", DbType.Int32, 4);
                        db.AddOutParameter(command, "@DAT_PRVT", DbType.DateTime, 10);
                        db.AddInParameter(command, "@INDC_TIP_ARQ", DbType.Int32, 0);
                        db.AddOutParameter(command, "@COD_ENT", DbType.Int32, 4);
                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });

                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            Log.GravarLog(EventoLog.RetornoDados);

                            while (reader.Read())
                            {
                                codigoRetorno = reader["RETORNO"].ToString().ToInt32();
                                break;
                            }
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { codigoRetorno });
                    return codigoRetorno;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(507, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Atualiza o endereço temporário para a entrega dos kits solicitados
        /// </summary>
        /// <returns>
        /// O novo código de endereço temporário
        /// </returns>
        private Int32 IncluirEnderecoTemporario(Modelo.Endereco endereco, Int32 codigoPV, GenericDatabase db)
        {

            using (Logger Log = Logger.IniciarLog("Atualiza o endereço temporário para a entrega dos kits solicitados"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    Int32 codigo = 0;
                    using (DbCommand command = db.GetStoredProcCommand("spgs0057"))
                    {
                        // adiciona os parâmetros de entrada necessários
                        db.AddInParameter(command, "@NUM_CEP_PAR", DbType.AnsiString, endereco.CEP);
                        db.AddInParameter(command, "@NOM_UF_PAR", DbType.AnsiString, endereco.UF);
                        db.AddInParameter(command, "@NOM_CID_PAR", DbType.AnsiString, endereco.Cidade);
                        db.AddInParameter(command, "@NOM_ENDR_CMPM_PAR", DbType.AnsiString, endereco.Complemento);
                        db.AddInParameter(command, "@NOM_CTTO_PAR", DbType.AnsiString, endereco.Contato);
                        db.AddInParameter(command, "@NUM_DDD_FAX_PAR", DbType.AnsiString, endereco.DDDFax);
                        db.AddInParameter(command, "@NUM_DDD_TEL_PAR", DbType.AnsiString, endereco.DDDTelefone);
                        db.AddInParameter(command, "@NOM_EMAL_PAR", DbType.AnsiString, endereco.Email);
                        db.AddInParameter(command, "@NOM_ENDR_PAR", DbType.AnsiString, endereco.DescricaoEndereco);
                        db.AddInParameter(command, "@NUM_FAX_PAR", DbType.AnsiString, endereco.Fax);
                        db.AddInParameter(command, "@NUM_ENDR_PAR", DbType.AnsiString, endereco.Numero);
                        db.AddInParameter(command, "@NUM_RML_PAR", DbType.AnsiString, endereco.Ramal);
                        db.AddInParameter(command, "@NOM_SITE_PAR", DbType.AnsiString, endereco.Site);
                        db.AddInParameter(command, "@NOM_BRR_PAR", DbType.AnsiString, endereco.Bairro);
                        db.AddInParameter(command, "@NUM_TEL_PAR", DbType.AnsiString, endereco.Telefone);
                        db.AddInParameter(command, "@BATCH", DbType.AnsiString, "0");
                        db.AddInParameter(command, "@NUM_PDV", DbType.Int32, codigoPV);
                        db.AddInParameter(command, "@INDC_END_TMP", DbType.AnsiString, "1");
                        db.AddInParameter(command, "@TIP_ENDR_PAR", DbType.Int32, null);

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });
                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            Log.GravarLog(EventoLog.RetornoDados);
                            while (reader.Read())
                            {
                                codigo = reader[0].ToString().ToInt32();
                            }
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { codigo });
                    return codigo;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(507, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta o cep cadastrado para o ponto de venda
        /// </summary>
        /// <param name="numeroPV"></param>
        /// <returns>O CEP cadastrado para o PV</returns>
        private String ConsultarCEPPV(Int32 numeroPV, out Int32 codigoRetorno)
        {

            using (Logger Log = Logger.IniciarLog("Consulta o cep cadastrado para o ponto de venda"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    String _cep = String.Empty;
                    codigoRetorno = 0;
                    GenericDatabase db = base.SybaseGE();
                    using (DbCommand command = db.GetStoredProcCommand("spge6028"))
                    {
                        // adiciona os parâmetros de entrada necessários
                        db.AddInParameter(command, "@NUM_PDV", DbType.Int32, numeroPV);
                        db.AddInParameter(command, "@OPCAO", DbType.AnsiString, '1');

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });
                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            Log.GravarLog(EventoLog.RetornoDados);

                            if (reader.FieldCount == 2)
                            {
                                // caso tenha retornado somente duas colunas, recuperar código de erro
                                while (reader.Read())
                                {
                                    codigoRetorno = reader["RETORNO"].ToString().ToInt32();
                                }
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    // retornou ao menos um registro, quer dizer, o cep foi encontrado na tabela
                                    _cep = String.Concat(reader[12].ToString(), reader[13].ToString());
                                    break;
                                }
                            }
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { codigoRetorno, _cep });
                    return _cep;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(507, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Usa a procedure spgs0177 para validar o CEP informado no
        /// endereço temporário
        /// </summary>
        /// <returns>
        /// Retorna a região do CEP informado
        /// </returns>
        private Int32 ValidarCEP(String cep, GenericDatabase db)
        {

            using (Logger Log = Logger.IniciarLog("Usa a procedure spgs0177 para validar o CEP informado no endereço temporário"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    Int32 regiao = -6;
                    using (DbCommand command = db.GetStoredProcCommand("spgs0177"))
                    {
                        // adiciona os parâmetros de entrada necessários
                        db.AddInParameter(command, "@cep_inicial", DbType.AnsiString, cep);

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });
                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            Log.GravarLog(EventoLog.RetornoDados);
                            while (reader.Read())// retornou ao menos um registro, quer dizer, o cep foi encontrado na tabela
                            {
                                regiao = reader[0].ToString().ToInt32();
                                break;
                            }
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { regiao });
                    return regiao;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(507, FONTE, ex);
                }
            }
        }
        /// <summary>
        /// Verificar área de distribuição do kit
        /// </summary>
        /// <returns>
        /// Código de retorno indicando sucesso (Igual a 0 (Zero)) ou falha (Diferente de 0 (Zero))
        /// </returns>
        private Int32 ValidarAreaDistribuicao(Int32 regiao, Int32 codigoKit, GenericDatabase db)
        {

            using (Logger Log = Logger.IniciarLog("Verificar área de distribuição do kit"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    Int32 codigoRetorno = -9092;
                    using (DbCommand command = db.GetStoredProcCommand("spgs0182"))
                    {
                        // adiciona os parâmetros de entrada necessários
                        db.AddInParameter(command, "@id_regiao_distribuicao", DbType.Int32, regiao);
                        db.AddInParameter(command, "@id_kit", DbType.Int32, codigoKit);

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });

                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            Log.GravarLog(EventoLog.RetornoDados);
                            while (reader.Read())// retornou ao menos um registro, quer dizer que a área foi encontrada na tabela
                            {
                                codigoRetorno = 0;
                                break;
                            }
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { codigoRetorno });
                    return codigoRetorno;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(507, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta um paramêtro no banco de dados do GS
        /// </summary>
        public String ConsultarParametro(Int32 codigoParam, out Int32 codigoRetorno)
        {

            using (Logger Log = Logger.IniciarLog("Consulta um paramêtro no banco de dados do GS"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    codigoRetorno = 0;
                    GenericDatabase db = base.SybaseGS();
                    String valorParametro = String.Empty;

                    using (DbCommand command = db.GetStoredProcCommand("spgs5081"))
                    {
                        // adiciona os parâmetros de entrada necessários
                        db.AddInParameter(command, "@cod_par", DbType.Int32, codigoParam);
                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });

                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            Log.GravarLog(EventoLog.RetornoDados);
                            if (reader.FieldCount > 1) // retorno 2 colunas, ocorreu um erro
                            {
                                while (reader.Read())
                                {
                                    codigoRetorno = reader["cod_err"].ToString().ToInt32();
                                    valorParametro = "";
                                    break;
                                }
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    object retorno = reader[0];
                                    if (!object.ReferenceEquals(retorno, null))
                                        valorParametro = retorno.ToString();
                                }
                            }
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { valorParametro });
                    return valorParametro;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(507, FONTE, ex);
                }
            }
        }
        /// <summary>
        /// Consultar a composição de um Kit para o estabelecimento
        /// </summary>
        /// <param name="codigoKit"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public List<Modelo.Material> ConsultarComposicaoKit(Int32 codigoKit)
        {

            using (Logger Log = Logger.IniciarLog("Consultar a composição de um Kit para o estabelecimento"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    GenericDatabase db = base.SybaseGS();
                    List<Modelo.Material> materiais = new List<Modelo.Material>();

                    using (DbCommand command = db.GetStoredProcCommand("spgs0125"))
                    {
                        // adiciona os parâmetros de entrada necessários
                        db.AddInParameter(command, "@id_kit", DbType.Int32, codigoKit);
                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });

                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            Log.GravarLog(EventoLog.RetornoDados);
                            while (reader.Read())
                            {
                                materiais.Add(new Modelo.Material()
                                {
                                    CodigoMaterial = reader["cod_mtrs"].ToString().ToInt32(),
                                    DescricaoMaterial = reader["des_mtrs"].ToString()
                                });
                            }
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { materiais });
                    return materiais;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(507, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Lista as últimas remessas enviadas para o estabelecimento
        /// </summary>
        public List<Modelo.Remessa> ConsultarUltimasRemessas(Int32 codigoPV)
        {

            using (Logger Log = Logger.IniciarLog("Lista as últimas remessas enviadas para o estabelecimento"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
#if !DEBUG
                    GenericDatabase db = base.SybaseGS();
                    List<Modelo.Remessa> remessas = new List<Modelo.Remessa>();

                    using (DbCommand command = db.GetStoredProcCommand("spgs0209"))
                    {
                        // adiciona os parâmetros de entrada necessários
                        db.AddInParameter(command, "@id_pv", DbType.Int32, codigoPV);
                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });

                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            while (reader.Read())
                            {
                                Log.GravarLog(EventoLog.RetornoDados);
                                remessas.Add(new Modelo.Remessa()
                                {
                                    DataRemessa = reader["DATA_ENTREGA"].ToString().ToDate(),
                                    NumeroProtocolo = reader["codRemessa"].ToString().ToDecimal(),
                                    Kit = new Modelo.Kit() { DescricaoKit = reader["des_kit"].ToString() },
                                    Quantidade = reader["qtd_ent_kit"].ToString().ToInt32(),
                                    Motivo = new Modelo.Motivo() { DescricaoMotivo = reader["des_mot"].ToString() }
                                });
                            }
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { remessas });

                    return remessas;
#else
                    List<Modelo.Remessa> remessas = new List<Modelo.Remessa>();
                    remessas.Add(new Modelo.Remessa()
                    {
                        DataRemessa = DateTime.Now,
                        NumeroProtocolo = 1111,
                        Kit = new Modelo.Kit() { DescricaoKit = "KIT DE EXEMPLO" },
                        Quantidade = 1,
                        Motivo = new Modelo.Motivo() { DescricaoMotivo = "MOTIVO DE EXEMPLO" }
                    });
                    Log.GravarLog(EventoLog.FimDados, new { remessas });
                    return remessas;
#endif


                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new PortalRedecardException(507, FONTE, ex);
                }
            }
        }
        /// <summary>
        /// Listar os kits disponíveis para o estabelecimento
        /// </summary>
        public List<Modelo.Kit> ConsultarKits(Int32 numeroPV, String categoria)
        {

            using (Logger Log = Logger.IniciarLog("Listar os kits disponíveis para o estabelecimento"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    GenericDatabase db = base.SybaseGS();
                    List<Modelo.Kit> motivos = new List<Modelo.Kit>();

                    using (DbCommand command = db.GetStoredProcCommand("spgs0261"))
                    {
                        // adiciona os parâmetros de entrada necessários
                        db.AddInParameter(command, "@id_pv", DbType.Int32, numeroPV);
                        db.AddInParameter(command, "@categoria", DbType.AnsiString, categoria);
                        db.AddInParameter(command, "@canal", DbType.AnsiString, "PORTAL");

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });
                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            Log.GravarLog(EventoLog.RetornoDados);
                            while (reader.Read())
                            {
                                motivos.Add(new Modelo.Kit()
                                {
                                    CodigoKit = reader["cod_kit"].ToString().ToInt32(),
                                    DescricaoKit = reader["des_kit"].ToString(),
                                });
                            }
                        }
                    }

                    Log.GravarLog(EventoLog.FimDados, new { motivos });
                    return motivos;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(507, FONTE, ex);
                }
            }
        }
        /// <summary>
        /// Listar os motivos relacionados por tipo de soliticação
        /// </summary>
        public List<Modelo.Motivo> ConsultarMotivos(Int32 tipoSolicitacao)
        {

            using (Logger Log = Logger.IniciarLog("Listar os motivos relacionados por tipo de soliticação"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    GenericDatabase db = base.SybaseGS();
                    List<Modelo.Motivo> motivos = new List<Modelo.Motivo>();

                    using (DbCommand command = db.GetStoredProcCommand("spgs0140"))
                    {
                        // adiciona os parâmetros de entrada necessários
                        db.AddInParameter(command, "@id_tipo_solicitacao", DbType.Int32, tipoSolicitacao);

                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });
                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            Log.GravarLog(EventoLog.RetornoDados);
                            while (reader.Read())
                            {
                                motivos.Add(new Modelo.Motivo()
                                {
                                    CodigoMotivo = reader["cod_mot"].ToString().ToInt32(),
                                    DescricaoMotivo = reader["des_mot"].ToString(),
                                });
                            }
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { motivos });
                    return motivos;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(507, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Lista as próximas remessas enviadas para o estabelecimento
        /// </summary>
        public List<Modelo.Remessa> ConsultarProximasRemessas(Int32 codigoPV)
        {

            using (Logger Log = Logger.IniciarLog("Lista as próximas remessas enviadas para o estabelecimento"))
            {
                Log.GravarLog(EventoLog.InicioDados);
                try
                {
                    GenericDatabase db = base.SybaseGS();
                    List<Modelo.Remessa> remessas = new List<Modelo.Remessa>();

                    using (DbCommand command = db.GetStoredProcCommand("spgs0399"))
                    {
                        // adiciona os parâmetros de entrada necessários
                        db.AddInParameter(command, "@NUM_PDV", DbType.Int32, codigoPV);
                        Log.GravarLog(EventoLog.ChamadaDados, new { command.Parameters });

                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            Log.GravarLog(EventoLog.RetornoDados);
                            while (reader.Read())
                            {
                                remessas.Add(new Modelo.Remessa()
                                {
                                    DataRemessa = reader["ANO_MES"].ToString().ToDate(),
                                    Origem = reader["txt_org_ent"].ToString(),
                                    Kit = new Modelo.Kit() { DescricaoKit = reader["des_kit"].ToString() },
                                    Quantidade = reader["qtd_ent_kit"].ToString().ToInt32(),
                                    Motivo = new Modelo.Motivo() { DescricaoMotivo = reader["INDC_RECL"].ToString() }
                                });
                            }
                        }
                    }
                    Log.GravarLog(EventoLog.FimDados, new { remessas });
                    return remessas;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(507, FONTE, ex);
                }
            }
        }
    }
}