/*
(c) Copyright 2012 Redecard S.A. 
Autor    : William Resendes Raposo
Empresa  : Resource IT Solution
Histórico:
 - 28/12/2012 – William Resendes Raposo – Versão inicial
*/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.FMS.Sharepoint.Servico.FMS;
using System.Collections.Generic;

namespace Redecard.PN.FMS.Sharepoint.WebParts.CadastroIPs
{ 
    /// <summary>
    /// Publica o serviço 'Cadastro de Ips' para chamada aos serviços do webservice referentes ao FMS - PN.
    /// </summary>
    public partial class CadastroIPsUserControl : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnAtualiza.Visible = base.GetSessaoAtual.UsuarioMaster();

            if (!IsPostBack)
            {
                BuscaListaDeIPs();
            }
        }

        private void BuscaListaDeIPs()
        {
            try
            {
                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    IPsAutorizados[] ips = objClient.BuscarListaIPs(GetSessaoAtual.CodigoEntidade, GetSessaoAtual.GrupoEntidade, GetSessaoAtual.LoginUsuario);
                    foreach (IPsAutorizados ip in ips)
                    {
                        lstIPs.Items.Add(new ListItem(ip.NumeroIP, ip.NumeroIP));
                    }

                    chkIpsAutorizados.Checked = (ips.Length > 0) && ips[0].EhPassivelValidacao;
                }

            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }


        protected void btnAtualiza_Click(object sender, EventArgs e)
        {
            IncluiIPs();
            BuscaListaDeIPs();
        }

        private void IncluiIPs()
        {
            try
            {
                using (Servico.FMS.FMSClient objClient = new Servico.FMS.FMSClient())
                {
                    List<IPsAutorizados> listaIps = new List<IPsAutorizados>();

                    String[] listaIpAutorizado = hdnListaDeIps.Value.Split('|');

                    foreach (String item in listaIpAutorizado)
                    {
                        listaIps.Add(new IPsAutorizados()
                        {
                            NumeroIP = item,
                        });
                    }
                    objClient.IncluirListaIps(listaIps.ToArray(), chkIpsAutorizados.Checked, GetSessaoAtual.CodigoEntidade, GetSessaoAtual.GrupoEntidade);
                }
            }
            catch (Exception ex)
            {
                base.OnError(ex);
            }
        }

        protected override bool CarregarParametrosSistema()
        {
            return false;
        }
    }
}
