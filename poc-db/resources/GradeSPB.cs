using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.OutrasEntidades;
using Redecard.PN.Comum;

namespace Redecard.PN.OutrasEntidades.Negocio
{
    public class GradeSPB : RegraDeNegocioBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mensagem"></param>
        /// <param name="retorno"></param>
        /// <returns></returns>
        public List<Modelo.BancoGrade> ConsultarBanco(out string mensagem, out string retorno)
        {

            try
            {
                List<Modelo.BancoGrade> lstRetorno = new Agentes.PB().ConsultarBanco(out mensagem, out retorno);

                //Filtra os registros vazios
                if (lstRetorno != null && lstRetorno.Count > 0)
                    lstRetorno = lstRetorno.Where(grade => grade.Ispb > 0).ToList();

                //Não retorna lista nula, e sim, lista vazia
                if (lstRetorno == null)
                    lstRetorno = new List<Modelo.BancoGrade>();

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ispb"></param>
        /// <param name="usuario"></param>
        /// <param name="codRetorno"></param>
        /// <param name="mensagem"></param>
        /// <param name="retorno"></param>
        /// <param name="dataContabil"></param>
        /// <returns></returns>
        public List<Modelo.GradeLiquidacao> ExtrairDadosSPB(string ispb, string usuario, out int codRetorno,
            out string mensagem, out string retorno, out string dataContabil)
        {

            try
            {
                // Verifica o tamanho do campo usuario o programa Mainframe esta definido com o tamanho 20.
                if (usuario.Length > 20)
                    usuario = usuario.Left(20);

                List<Modelo.GradeLiquidacao> lstRetorno = new Agentes.PB().ExtrairDadosSPB(ispb.PadLeft(8, '0'), usuario, out codRetorno, out mensagem, out retorno, out dataContabil);

                //Filtra os registros vazios
                if (lstRetorno != null && lstRetorno.Count > 0)
                    lstRetorno = lstRetorno.Where(grade => grade.Tipo > 0).ToList();

                //Não retorna lista nula, e sim, lista vazia
                if (lstRetorno == null)
                    lstRetorno = new List<Modelo.GradeLiquidacao>();

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ispb"></param>
        /// <param name="usuario"></param>
        /// <param name="codRetorno"></param>
        /// <param name="mensagem"></param>
        /// <param name="retorno"></param>
        /// <param name="dataContabil"></param>
        /// <returns></returns>
        public Modelo.GradeLiquidacaoBandeira ExtrairDetalhesSPB(string ispb, string usuario, out int codRetorno,
            out string mensagem, out string retorno, out String dataContabil)
        {

            try
            {
                // Verifica o tamanho do campo usuario o programa Mainframe esta definido com o tamanho 20.
                if (usuario.Length > 20)
                    usuario = usuario.Left(20);

                Modelo.GradeLiquidacaoBandeira itRetorno = new Agentes.PB().ExtrairDetalhesSPB(ispb.PadLeft(8, '0'), usuario, out codRetorno, out mensagem, out retorno, out dataContabil);

                return itRetorno;
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
    }
}
