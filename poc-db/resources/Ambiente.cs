using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Redecard.PN.DadosCadastrais.ISRobo
{
    public class Ambiente
    {
        public Ambiente()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Define o ambiente que está executando a aplicacao
        public string Sigla
        {
            get
            {
                string sigla = null;
                if (Convert.ToString(ConfigurationManager.AppSettings["HOST"]).Trim().ToLower() ==
                    Convert.ToString(ConfigurationManager.AppSettings["HOST_D"]).Trim().ToLower())
                {
                    sigla = "D";
                }
                else if (Convert.ToString(ConfigurationManager.AppSettings["HOST"]).Trim().ToLower() ==
                    Convert.ToString(ConfigurationManager.AppSettings["HOST_S"]).Trim().ToLower())
                {
                    sigla = "S";
                }
                else if (Convert.ToString(ConfigurationManager.AppSettings["HOST"]).Trim().ToLower() ==
                    Convert.ToString(ConfigurationManager.AppSettings["HOST_P"]).Trim().ToLower())
                {
                    sigla = "P";
                }
                else
                {
                    sigla = "L";
                }

                return sigla;
            }
        }
        #endregion
    }
}
