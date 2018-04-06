using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ServiceModel;
using Redecard.PN.Comum;
using System.Reflection;

namespace Rede.PN.Credenciamento.Sharepoint
{
    internal static class ExceptionExtensions
    {
        /// <summary>
        /// Trata a exceção
        /// </summary>
        /// <param name="ex">Exceção</param>
        /// <param name="controle">Controle onde ocorreu a exceção</param>
        public static void HandleException(this Exception ex, Control controle)
        {
            Int32 codigo;
            String fonte;

            GetExceptionsDetails(ex, out codigo, out fonte);

            Logger.GravarErro("Credenciamento", ex, new { codigo, fonte });
            SharePointUlsLog.LogErro(ex);
            if (controle is UserControlBase)
            {
                ((UserControlBase)controle).ExibirPainelExcecao(fonte, codigo);
            }
        }

        /// <summary>
        /// Busca código do erro e fonte da exceção
        /// </summary>
        /// <param name="ex">Exceção</param>
        /// <param name="codigo">Código do Erro</param>
        /// <param name="fonte">Fonte</param>
        private static void GetExceptionsDetails(Exception ex, out int codigo, out string fonte)
        {
            codigo = UserControlBase.CODIGO_ERRO;
            fonte = UserControlBase.FONTE;
            if (ex != null
             && ex.GetType() != null
             && ex.GetType().GetGenericArguments() != null
             && ex.GetType().GetGenericArguments().Length > 0)
            {
                if (ex.GetType().IsGenericType && ex.GetType().GetGenericArguments()[0].Name == "GeneralFault")
                {
                    PropertyInfo propInfo = ex.GetType().GetProperty("Detail");
                    if (propInfo != null)
                    {
                        Object detail = propInfo.GetValue(ex, null);

                        if (detail != null && propInfo.PropertyType != null)
                        {
                            PropertyInfo codigoErroProp = propInfo.PropertyType.GetProperty("Codigo");
                            Int32? codigoErro = (Int32?)codigoErroProp.GetValue(detail, null);

                            PropertyInfo fonteErroProp = propInfo.PropertyType.GetProperty("Fonte");
                            String fonteErro = (String)fonteErroProp.GetValue(detail, null);

                            codigo = codigoErro.GetValueOrDefault();
                            fonte = fonteErro;
                        }
                    }
                }
                else if (ex.GetType().IsGenericType && ex.GetType().GetGenericArguments()[0].Name == "ModelosErroServicos")
                {
                    PropertyInfo propInfo = ex.GetType().GetProperty("Detail");
                    if (propInfo != null)
                    {
                        Object detail = propInfo.GetValue(ex, null);

                        if (detail != null && propInfo.PropertyType != null)
                        {
                            PropertyInfo codigoErroProp = propInfo.PropertyType.GetProperty("CodErro");
                            codigoErroProp = codigoErroProp ?? propInfo.PropertyType.GetProperty("CodigoErro"); 
                            codigoErroProp = codigoErroProp ?? propInfo.PropertyType.GetProperty("Codigo");

                            Int32? codigoErro = (Int32?)codigoErroProp.GetValue(detail, null);

                            PropertyInfo fonteErroProp = propInfo.PropertyType.GetProperty("Fonte");
                            String fonteErro = (String)fonteErroProp.GetValue(detail, null);

                            codigo = codigoErro.GetValueOrDefault();
                            fonte = fonteErro;
                        }
                    }
                }
            }
        }

        /// <summary>Converte string para Double</summary>
        /// <param name="numero">Número</param>
        /// <param name="numeroPadrao_">Número padrão de retorno (se não for número)</param>
        /// <returns>Retorno</returns>
        public static Double? ToDoubleNull(this String numero, Double? valorPadrao)
        {
            if (String.IsNullOrWhiteSpace(numero))
                return valorPadrao;
            else
            {
                Double retorno;

                //Verifica se o número é válido
                if (Double.TryParse(numero, out retorno))
                    return (Double?)retorno;
                else
                    return valorPadrao;
            }
        }

        /// <summary>Converte string para Double</summary>
        /// <param name="numero">Número</param>
        /// <param name="numeroPadrao_">Número padrão de retorno (se não for número)</param>
        /// <returns>Retorno</returns>
        public static Double? ToDoubleNull(this String numero)
        {
            return numero.ToDoubleNull(null);
        }

        /// <summary>Converte string para Double</summary>
        /// <param name="numero">Número</param>
        /// <param name="numeroPadrao_">Número padrão de retorno (se não for número)</param>
        /// <returns>Retorno</returns>
        public static Double ToDouble(this String numero, Double valorPadrao)
        {
            return numero.ToDoubleNull(valorPadrao).Value;
        }
    }
}
