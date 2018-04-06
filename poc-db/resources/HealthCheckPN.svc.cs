#region Histórico do Arquivo
/*
(c) Copyright [2016] Redecard S.A.
Autor       : [Raphael Ivo]
Empresa     : [Iteris]
Histórico   :
- [19/09/2016] – [Raphael Ivo] – [Criação]
*/
#endregion

using DynamicProxyLibrary;
using Microsoft.SharePoint.Client.Services;
using Redecard.PN.Sustentacao.SharePoint.SustentacaoAdministracaoServico;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using System.Threading;
using System.Configuration;
using System.Collections.Specialized;
using System.Reflection;

namespace Redecard.PN.Sustentacao.SharePoint
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class HealthCheckPN : IHealthCheckPN
    {
        /// <summary>
        /// Responde disponibilidade do servidor/servico PN informado
        /// </summary>
        /// <returns></returns>
        public List<Servico> HealthCheckServicos(string nomeServidor)
        {
            string appsetting = TratarServidor(nomeServidor);
            List<Servico> resultadoServicos = new List<Servico>();

            if (!String.IsNullOrEmpty(appsetting))
            {
                try
                {
                    string[] words = appsetting.Split(';');
                    string servidor = words[0];
                    string ambiente = words[1];
                    resultadoServicos = ConsultaServicosBack(servidor, ambiente);
                }
                catch (Exception ex)
                {
                    Servico objServico = new Servico();
                    objServico.Nome = "Erro";
                    objServico.Status = "NOK - Não foi possível conectar ao servidor " + nomeServidor;
                    resultadoServicos.Add(objServico);
                }
            }
            else
            {
                Servico objServico = new Servico();
                objServico.Nome = "Erro";
                objServico.Status = "NOK - Servidor " + nomeServidor + " inválido.";
                resultadoServicos.Add(objServico);
            }

            return resultadoServicos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nomeServidor"></param>
        /// <returns></returns>
        private string TratarServidor(string nomeServidor)
        {
            string appsetting = "";

            try
            {
                NameValueCollection section = (NameValueCollection)ConfigurationManager.GetSection("ServidoresPN");
                appsetting = section[nomeServidor];
                return appsetting;
            }
            catch (Exception ex)
            {
                appsetting = "Erro: " + ex.ToString();
                return appsetting;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="servicos"></param>
        /// <returns></returns>
        private List<Servico> ConsultaServicosBack(string servidor, string ambiente)
        {
            List<Servico> resultadoServicos = new List<Servico>();
            string factoryText = servidor + "/PN/Sustentacao/SustentacaoAdministracaoServico.svc";
            string methodName = "HealthCheckServicos";
            string binding = "NetTcpBinding";

            if (OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri.AbsoluteUri.Contains("pi"))
            {
                factoryText = servidor + "/Sustentacao/SustentacaoAdministracaoServico.svc";
                binding = "WSHttpBinding";
            }

            DynamicProxyFactory factory = new DynamicProxyFactory(factoryText);
            ServiceEndpoint endpoint = factory.Endpoints.Where(a => a.Binding.GetType().Name == binding).FirstOrDefault();
            string sContract = endpoint.Contract.Name;
            DynamicProxy proxy = factory.CreateProxyEndpoint(sContract, endpoint);
            List<object> listaParametros = new List<object>();
            listaParametros.Add(servidor);
            listaParametros.Add(ambiente);

            object result = proxy.CallMethod(methodName, listaParametros.ToArray());
            IEnumerable enumerable = result as IEnumerable;
            if (enumerable != null)
            {
                foreach (object element in enumerable)
                {
                    Type myType = element.GetType();
                    Servico novoServico = new Servico();
                    IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
                    novoServico.Nome = props[0].GetValue(element, null).ToString();
                    novoServico.Status = props[1].GetValue(element, null).ToString();
                    resultadoServicos.Add(novoServico);
                }
            }
            return resultadoServicos;
        }
    }
}
