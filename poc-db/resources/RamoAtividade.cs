using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

namespace Redecard.PN.DadosCadastrais.Dados
{
    public class RamoAtividade : BancoDeDadosBase
    {
        /// <summary>
        /// Consulta os Ramos de Atividade
        /// </summary>
        /// <param name="codGrupoRamo">Código do grupo ramo</param>
        /// <param name="codRamoAtividade">Código do ramo atividade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Ramos de Atividades</returns>
        public List<Modelo.RamoAtividade> Consultar(Int32? codGrupoRamo, Int32? codRamoAtividade, out Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Consultar os Ramos de Atividade"))
            {
                Log.GravarLog(EventoLog.InicioDados, new { codGrupoRamo, codRamoAtividade });
                try
                {
                    codigoRetorno = 0;
                    var ramosAtividade = new List<Modelo.RamoAtividade>();

                    GenericDatabase db = base.SybaseGE();
                    using (DbCommand command = db.GetStoredProcCommand("spge5046"))
                    {                        
                        db.AddInParameter(command, "@COD_GRU_RAM", DbType.Int32, codGrupoRamo);
                        db.AddInParameter(command, "@COD_RAM_ATVD", DbType.Int32, codRamoAtividade);
                                    
                        using (IDataReader dataReader = db.ExecuteReader(command))
                        {
                            while (dataReader.Read())
                            {
                                ramosAtividade.Add(new Modelo.RamoAtividade
                                {
                                     Descricao = Convert.ToString(dataReader["des_ram_atvd"]),
                                     CodigoSituacao = Convert.ToChar(dataReader["cod_sit_ram_atvd"]),
                                     IndicaMoeda = Convert.ToChar(dataReader["indc_moed"]),
                                     QuantidadeDefaultParcela = Convert.ToString(dataReader["qtd_dflt_prcl"]).ToInt32(0),
                                     QuantidadeMaximaParcela = Convert.ToString(dataReader["qtd_mxma_prcl"]).ToInt32(0),
                                     CodigoGrupoRamo = Convert.ToString(dataReader["cod_gru_ram"]).ToInt32(0),
                                     CodigoRamoAtividade = Convert.ToString(dataReader["cod_ram_atvd"]).ToInt32(0)
                                });                                
                            }
                        }
                    }

                    Log.GravarLog(EventoLog.FimDados, new { codigoRetorno, ramosAtividade });
                    return ramosAtividade;
                }
                catch (Exception ex)
                {
                    throw new PortalRedecardException(500, FONTE, ex);
                }
            }
        }
    }
}
