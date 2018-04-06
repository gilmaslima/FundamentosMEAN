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
using Microsoft.SharePoint.BusinessData.Administration;
using System.Xml;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Configuration;
using System.IO;


namespace Redecard.PN.Sustentacao.SharePoint.CONTROLTEMPLATES.sustentacao
{
    public partial class ServicosMetodo : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["Factory"].ToString()) && !String.IsNullOrEmpty(Request.QueryString["Method"].ToString()) && !String.IsNullOrEmpty(Request.QueryString["Binding"].ToString()))
            {
                lblResult.Text = "";
                ltrlTxtResult.Visible = false;
                ltrlTxtResult.Text = "";
                grdResult.DataSource = null;
                grdResult.DataBind();

                string factoryText = Request.QueryString["Factory"];
                string methodName = Request.QueryString["Method"];
                string binding = Request.QueryString["Binding"];
                ltrlMethod.Text = methodName;
                ltrlFactory.Text = factoryText;
                BuscarMetodoFactory(factoryText, methodName, binding);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factoryText"></param>
        /// <param name="methodName"></param>
        /// <param name="binding"></param>
        private void BuscarMetodoFactory(string factoryText, string methodName, string binding)
        {
            DynamicProxyFactory factory = new DynamicProxyFactory(factoryText);

            string sContract = "";

            foreach (ServiceEndpoint endpoint in factory.Endpoints)
            {
                if (endpoint.Binding.GetType().Name == binding)
                {
                    sContract = endpoint.Contract.Name;

                    DynamicProxy proxy = factory.CreateProxyEndpoint(sContract, endpoint);
                    Type proxyType = proxy.ProxyType;

                    MethodInfo method = proxyType.GetMethods().Where(p => p.Name == methodName).FirstOrDefault();
                    List<ParameterInfo> methodParameters = method.GetParameters().ToList();

                    rptParameters.DataSource = methodParameters;
                    rptParameters.DataBind();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnviar_Click(object sender, System.EventArgs e)
        {
            try
            {
                DynamicProxyFactory factory = new DynamicProxyFactory(ltrlFactory.Text);

                string factoryText = Request.QueryString["Factory"];
                string methodName = Request.QueryString["Method"];
                string binding = Request.QueryString["Binding"];
                string sContract = "";

                foreach (ServiceEndpoint endpoint in factory.Endpoints)
                {
                    if (endpoint.Binding.GetType().Name == binding)
                    {
                        sContract = endpoint.Contract.Name;

                        DynamicProxy proxy = factory.CreateProxyEndpoint(sContract, endpoint);
                        Type proxyType = proxy.ProxyType;
                        MethodInfo method = proxyType.GetMethods().Where(p => p.Name == methodName).FirstOrDefault();

                        List<ParameterInfo> methodParameters = method.GetParameters().ToList();
                        List<object> listaParametros = new List<object>();

                        foreach (RepeaterItem item in rptParameters.Items)
                        {
                            TextBox teste = ((TextBox)rptParameters.Items[item.ItemIndex].Controls[1]);
                            Type propType = method.GetParameters()[item.ItemIndex].ParameterType;
                            var converter = System.ComponentModel.TypeDescriptor.GetConverter(propType);
                            var convertResult = new object();
                            if (teste.Text != "null")
                                convertResult = converter.ConvertFrom(teste.Text);
                            else
                                convertResult = null;

                            listaParametros.Add(convertResult);
                        }

                        lblResult.Text = "Sucesso na chamada:";
                        object result = proxy.CallMethod(ltrlMethod.Text, listaParametros.ToArray());

                        PopularResultados(result);
                    }
                }
            }
            catch (Exception ex)
            {
                lblResult.Text = "Houve um erro na chamada:";
                ltrlTxtResult.Visible = true;
                ltrlTxtResult.Text = ex.InnerException.ToString();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptParameters_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            var textbox = e.Item.FindControl("txtValor");
            textbox.ID = "txtValor" + (e.Item.ItemIndex);
        }

        protected void PopularResultados(object result)
        {
            List<object> listaResult = new List<object>();
            IEnumerable enumerable = result as IEnumerable;
            if (enumerable != null)
            {
                foreach (object element in enumerable)
                {
                    listaResult.Add(element);
                }
            }
            else
            {
                listaResult.Add(result);
            }
            grdResult.DataSource = listaResult;
            grdResult.DataBind();
        }
    }
}
