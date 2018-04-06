#region Histórico do Arquivo
/*
(c) Copyright [2015] Redecard S.A.
Autor       : [Raphael Ivo]
Empresa     : [Iteris]
Histórico   :
    [12/02/2016] – [Raphael Ivo] – [Criação]
*/
#endregion

using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.ServiceModel;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

using Redecard.PN.Comum;
using Redecard.PN.Sustentacao.SharePoint.SustentacaoAdministracaoServico;
using Redecard.PN.Sustentacao.AdministracaoDados;
using System.Data;
using System.Security;
using Microsoft.SharePoint.Utilities;
using System.Collections;
using Microsoft.SharePoint.Navigation;
using System.Reflection;
using System.Collections.Generic;
using DynamicProxyLibrary;
using System.ServiceModel.Description;


namespace Redecard.PN.Sustentacao.SharePoint.CONTROLTEMPLATES.sustentacao
{
    public partial class ServicosConsulta : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ltrlMethodNetTcp.Text = "";
            ltrlMethodHttp.Text = "";
            rptMethodsHttp.DataSource = null;
            rptMethodsNetTcp.DataSource = null;
            rptMethodsNetTcp.DataBind();
            rptMethodsHttp.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factoryText"></param>
        private void BuscarFactory(string factoryText)
        {
            try
            {
                DynamicProxyFactory factory = new DynamicProxyFactory(factoryText);
                lblResult.Text = "Sucesso na Busca:";
                ltrlTxtResult.Visible = false;
                BuscarMetodos(factory);
            }
            catch (Exception ex)
            {
                if (ex is UriFormatException)
                {
                    lblResult.Text = "URL Inválida";
                }
                else
                {
                    lblResult.Text = "Houve um erro na busca:";
                    ltrlTxtResult.Visible = true;
                    ltrlTxtResult.Text = ex.InnerException.ToString();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        private void BuscarMetodos(DynamicProxyFactory factory)
        {
            foreach (ServiceEndpoint endpoint in factory.Endpoints)
            {
                if (endpoint.Binding.GetType().Name == "BasicHttpBinding")
                {
                    string sContractNetTcp = endpoint.Contract.Name;
                    ltrlMethodBasic.Text = endpoint.Contract.Name + " (" + endpoint.Binding.Name + ")";
                    rptMethodsBasic.DataSource = ResolverProxy(factory, sContractNetTcp);
                    rptMethodsBasic.DataBind();
                }

                if (endpoint.Binding.GetType().Name == "NetTcpBinding")
                {
                    string sContractNetTcp = endpoint.Contract.Name;
                    ltrlMethodNetTcp.Text = endpoint.Contract.Name + " (" + endpoint.Binding.Name + ")";
                    rptMethodsNetTcp.DataSource = ResolverProxy(factory, sContractNetTcp);
                    rptMethodsNetTcp.DataBind();
                }

                if (endpoint.Binding.GetType().Name == "WSHttpBinding")
                {
                    string sContractWSHttp = endpoint.Contract.Name;
                    ltrlMethodHttp.Text = endpoint.Contract.Name + " (" + endpoint.Binding.Name + ")";
                    rptMethodsHttp.DataSource = ResolverProxy(factory, sContractWSHttp);
                    rptMethodsHttp.DataBind();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="sContract"></param>
        /// <returns></returns>
        private List<string> ResolverProxy(DynamicProxyFactory factory, string sContract)
        {
            DynamicProxy proxy = factory.CreateProxy(sContract);
            Type proxyType = proxy.ProxyType;
            MethodInfo[] methods = proxyType.GetMethods();
            List<string> listaMethods = new List<string>();

            foreach (var method in methods)
            {
                if (!method.DeclaringType.FullName.Contains("System"))
                    listaMethods.Add(method.Name);
            }

            return listaMethods;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBuscar_Click(object sender, System.EventArgs e)
        {
            BuscarFactory(txtFactory.Text);
        }
    }
}
