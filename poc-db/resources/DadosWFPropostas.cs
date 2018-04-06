using System;
using System.Collections.Generic;
using Redecard.PN.OutrasEntidades.Modelo;
using Redecard.PN.Comum;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using System.ServiceModel;

namespace Redecard.PN.OutrasEntidades.Dados
{
    public class DadosWFPropostas : BancoDeDadosBase
    {

        ///<summary>  
        /// Listar uma ou mais propostas através do CNPJ ou CPF
        ///</summary>/// <param name="CodTipoPessoa">Código do Tipo de Pessoa</param>
        /// <param name="NumCnpjCpf">Número do CNPJ/CPF do estabelecimento</param>
        /// <param name="IndSeqProp">Indicador da Sequencia da Proposta</param>
        /// 
        public List<PropostaPorCNPJCPF> ConsultaPropostaPorCNPJCPF(Char codigoTipoPessoa, Int64 numeroCnpjCpf, Int32 indicadorSequenciaProp)
        {
            using (var log = Redecard.PN.Comum.Logger.IniciarLog("Consultar todas as propriedades do DadosWFProposta.ConsultaPropostaPorCNPJCPF"))
            {
                List<PropostaPorCNPJCPF> listaRetorno = new List<PropostaPorCNPJCPF>();
                log.GravarLog(EventoLog.InicioDados);
                
                try
                {
                    #if !DEBUG

                    GenericDatabase db = base.SybaseWF();
                    
                    using (DbCommand command = db.GetStoredProcCommand("spwf5047"))
                    {

                        // Cria parâmetros necessários
                        db.AddInParameter(command, "@cod_tip_pes", DbType.AnsiString, codigoTipoPessoa);
                        db.AddInParameter(command, "@num_cgc_estb", DbType.Decimal, numeroCnpjCpf);

                        db.AddInParameter(command, "@cod_cnl", DbType.Decimal, indicadorSequenciaProp);
                        db.AddInParameter(command, "@cod_cel", DbType.Int32, null);
                        db.AddInParameter(command, "@cod_age_cnl_flco", DbType.Int32, null);

                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            if (reader.FieldCount != 0)
                            {
                                PropostaPorCNPJCPF objProposta = new PropostaPorCNPJCPF();

                                while (reader.Read())
                                {
                                    objProposta.CodigoRetorno = (Int32)reader["cod_ret"];
                                    objProposta.DescricaoRetorno = reader["des_ret"].ToString();
                                    objProposta.Data = reader["data"].ToString();
                                    objProposta.Hora = reader["hora"].ToString();
                                    objProposta.NomeRazaoSocial = reader["nom_rzsc_estb"].ToString();
                                    objProposta.CodigoCelula = (Int32)reader["cod_cel"];
                                    objProposta.CodigoAgenciaCanalFiliacao = (Int32)reader["cod_age_cnl_flco"];

                                    listaRetorno.Add(objProposta);
                                }
                            }
                        }
                    }

#else
                    PropostaPorCNPJCPF objProposta = new PropostaPorCNPJCPF();

                    objProposta.CodigoRetorno = 0;
                    objProposta.DescricaoRetorno = "N - Proposta cadastrada na Redecard (Em cadastramento)";
                    objProposta.Data = "10/09/2014";
                    objProposta.Hora = "16:35:00";
                    objProposta.NomeRazaoSocial = "Nome Razão Social Empresa";
                    objProposta.CodigoCelula = 826;
                    objProposta.CodigoAgenciaCanalFiliacao = 0;

                    listaRetorno.Add(objProposta);
                    #endif

                    return listaRetorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(500, String.Concat(FONTE, ".OutrasEntidades"), ex);
                }

            }
        }


    }
}
