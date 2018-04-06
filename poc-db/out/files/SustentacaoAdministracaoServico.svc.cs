#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [24/05/2012] – [André Garcia] – [Criação]
*/
#endregion

using DynamicProxyLibrary;
using Redecard.PN.Comum;
using Redecard.PN.Sustentacao.Modelo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.ServiceModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Security.Principal;
using System.Reflection;

namespace Redecard.PN.Sustentacao.Servicos
{

    /// <summary>
    /// Serviço para gerenciamento da administração do Portal de Serviços
    /// </summary>
    /// <remarks>
    /// Serviço para gerenciamento da administração do Portal de Serviços
    /// </remarks> 
    public class SustentacaoAdministracaoServico : ServicoBase, ISustentacaoAdministracaoServico
    {
        /// <summary>
        /// Executa o script no banco de dados informado.
        /// </summary>
        /// <remarks>
        /// Executa o script no banco de dados informado.
        /// </remarks>
        /// <param name="bancoDados">Nome da connection string</param>
        /// <param name="script">Sql que será executado no banco.</param>
        /// <returns>Instância do objeto DataTable</returns>
        public DataTable[] ConsultarSql(String bancoDados, String script)
        {
            try
            {
                Negocio.SustentacaoAdministracao adm = new Negocio.SustentacaoAdministracao();
                return adm.ConsultarSql(bancoDados, script);
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Método que retorna registros da tabela TBPN003 e TBPN002 
        /// </summary>
        /// <param name="email">Parâmetro de pesquisa</param>
        /// <returns>Lista unificada de usuários e entidades</returns>
        public List<UsuarioPV> ConsultarUsuariosPorEmail(String email, Int32 pv, Int32 grupo)
        {
            try
            {
                Negocio.SustentacaoAdministracao negocio = new Negocio.SustentacaoAdministracao();
                return negocio.ConsultarUsuariosPorEmail(email, pv, grupo);
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Executa um script no BD de SQL Server PN
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public DataTable[] ConsultarQuerySQLServer(String script, String usuarioLogado, String infoOperacao, String nomeBanco)
        {
            try
            {
                Negocio.SustentacaoAdministracao adm = new Negocio.SustentacaoAdministracao();
                return adm.ConsultarQuerySQLServer(script, usuarioLogado, infoOperacao, nomeBanco);
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(
                    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// étodo que retorna registros da tabela TBPN026
        /// </summary>
        /// <param name="dataInclusao"></param>
        /// <param name="codigoPv"></param>
        /// <param name="codigoPvAcesso"></param>
        /// <param name="email"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public List<RetornoCancelamento> ConsultarRetornoCancelamento(DateTime dataInclusao, Int32? codigoPv, Int32? codigoPvAcesso, String email, String ip)
        {
            try
            {
                Negocio.SustentacaoAdministracao negocio = new Negocio.SustentacaoAdministracao();
                return negocio.ConsultarRetornoCancelamento(dataInclusao, codigoPv, codigoPvAcesso, email, ip);
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Método utilizado para alterar status e quantidade de tentativas de senhas na tabela TBPN003
        /// </summary>
        /// <param name="codUsrId">Identificacao do usuario na tabela TBPN003</param>
        /// <returns>Código de retorno</returns>
        public Int32 DesbloquearUsuario(Int32 codUsrId, Int32 codEntidade, String usuarioLogado, String nomEmlUsr)
        {
            try
            {
                Negocio.SustentacaoAdministracao negocio = new Negocio.SustentacaoAdministracao();
                return negocio.DesbloquearUsuario(codUsrId, codEntidade, usuarioLogado, nomEmlUsr);
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Método utilizado para alterar status e quantidade de tentativas de senhas na tabela TBPN003
        /// </summary>
        /// <param name="codUsrId">Identificacao do usuario na tabela TBPN003</param>
        /// <returns>Código de retorno</returns>
        public Int32 ExcluirUsuario(Int32 codUsrId, String usuarioLogado, String nomEmlUsr)
        {
            try
            {
                Negocio.SustentacaoAdministracao negocio = new Negocio.SustentacaoAdministracao();
                return negocio.ExcluirUsuario(codUsrId, usuarioLogado, nomEmlUsr);
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codUsrId"></param>
        /// <param name="codEtd"></param>
        /// <param name="usuarioLogado"></param>
        /// <param name="nomEmlUsr"></param>
        /// <returns></returns>
        public Int32 ExcluirRelEtd(Int32 codUsrId, Int32 codEtd, Int32 grupo, String usuarioLogado, String nomEmlUsr)
        {
            try
            {
                Negocio.SustentacaoAdministracao negocio = new Negocio.SustentacaoAdministracao();
                return negocio.ExcluirRelEtd(codUsrId, codEtd, grupo, usuarioLogado, nomEmlUsr);
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Expurgo da tabela de log
        /// </summary>
        /// <remarks>
        /// Expurgo da tabela de log
        /// </remarks>
        public void Expurgo()
        {
            try
            {
                Negocio.SustentacaoAdministracao negocio = new Negocio.SustentacaoAdministracao();
                negocio.Expurgo();
            }
            catch (PortalRedecardException ex)
            {
                throw new FaultException<GeneralFault>(new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
            }
            catch (Exception ex)
            {
                throw new FaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
            }
        }

        /// <summary>
        /// Lista todas o nome de todas as connection strings configuradas no web.config
        /// </summary>
        /// <remarks>
        /// Lista todas o nome de todas as connection strings configuradas no web.config
        /// </remarks>
        /// <returns>Lista de connection strings</returns>
        public List<String> ListarConnectionStrings()
        {
            List<String> listaConnectionStrings = new List<String>();

            foreach (ConnectionStringSettings connectionStringSettings in ConfigurationManager.ConnectionStrings)
            {
                listaConnectionStrings.Add(connectionStringSettings.Name);
            }

            return listaConnectionStrings;
        }

        /// <summary>
        /// Retorna os serviços cadastrados na blacklist situada no web.config
        /// </summary>
        /// <param name="ambiente"></param>
        /// <returns></returns>
        public List<string> ConsultarBlackListServicos(string ambiente)
        {
            List<string> blackListServicos = new List<string>();
            NameValueCollection section = new NameValueCollection();
            switch (ambiente)
            {
                case "Dev":
                    section = (NameValueCollection)ConfigurationManager.GetSection("BlackListDev");
                    break;
                case "Simul":
                    section = (NameValueCollection)ConfigurationManager.GetSection("BlackListSimul");
                    break;
                case "Prod":
                    section = (NameValueCollection)ConfigurationManager.GetSection("BlackListProd");
                    break;
            }

            blackListServicos = section.AllKeys.ToList();
            return blackListServicos;
        }

        /// <summary>
        /// Retorna uma lista de serviços dentro do servidor/ambiente informados
        /// </summary>
        /// <param name="servidor"></param>
        /// <param name="ambiente"></param>
        /// <returns></returns>
        public List<Servico> HealthCheckServicos(string servidor, string ambiente)
        {
            List<Servico> resultadoServicos = new List<Servico>();
            List<string> listaServicos = ListarServicosPN();
            List<string> blackListServicos = ConsultarBlackListServicos(ambiente);

            Thread[] objThread = new Thread[listaServicos.Count];

            foreach (string servico in listaServicos)
            {
                if (!blackListServicos.Contains(servico))
                {
                    int index = listaServicos.FindIndex(a => a.ToString() == servico);
                    objThread[index] = new Thread(() => ConsultaServico(ref resultadoServicos, servico, servidor));
                    objThread[index].Priority = ThreadPriority.AboveNormal;
                    objThread[index].Start();
                }
            }
            foreach (string servico in listaServicos)
            {
                if (!blackListServicos.Contains(servico))
                {
                    int index = listaServicos.FindIndex(a => a.ToString() == servico);
                    objThread[index].Join();
                }
            }
            return resultadoServicos;
        }

        /// <summary>
        /// Consulta um determinado serviço em um servidor, retornando "OK" ou "NOK" para dentro de uma lista informada
        /// </summary>
        /// <param name="resultadoServicos"></param>
        /// <param name="servico"></param>
        /// <param name="servidor"></param>
        private static void ConsultaServico(ref List<Servico> resultadoServicos, string servico, string servidor)
        {
            Servico objServico = new Servico();
            objServico.Nome = servico;

            try
            {
                string factoryText = servidor + "/" + servico;
                DynamicProxyFactory factory = new DynamicProxyFactory(factoryText);
                objServico.Status = "OK";
                resultadoServicos.Add(objServico);
            }
            catch (Exception ex)
            {
                objServico.Status = "NOK";
                resultadoServicos.Add(objServico);
            }
        }

        /// <summary>
        /// Lista todos os endpoints dos web configs dentro de Redecard.PN
        /// </summary>
        /// <returns></returns>
        public List<String> ListarServicosPN()
        {
            List<String> listaServicos = new List<String>();
            List<String> files = new List<String>();
            string diretorio = "";
            if (OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri.AbsoluteUri.Contains("pi"))
                diretorio = @"C:\inetpub\wwwroot\Redecard.PN";
            else
                diretorio = @"D:\inetpub\wwwroot\Redecard.PN";

            files = ArquivosDiretorio(diretorio);
            listaServicos = MostrarServicos(files);

            return listaServicos;
        }

        /// <summary>
        /// Lista todos os endpoints dos web configs dentro de uma lista de diretórios
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private static List<string> MostrarServicos(List<string> files)
        {
            List<string> endpoints = new List<string>();
            List<string> erros = new List<string>();

            foreach (var caminho in files)
            {
                if (caminho.Substring(Math.Max(0, caminho.Length - 4)).Contains(".svc")) 
                {
                    endpoints.Add(caminho.Substring(31));
                }
            }

            return endpoints;
        }

        /// <summary>
        /// Lista todos os diretórios dentro de um caminho
        /// </summary>
        /// <param name="sDir"></param>
        /// <returns></returns>
        public static List<String> ArquivosDiretorio(string sDir)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(ArquivosDiretorio(d));
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }

            return files;
        }

        /// <summary>
        /// Retorna as definições de classe e config para a criação do client proxy, baseado no endpoint informado.
        /// </summary>
        /// <param name="endPointAddress">URL do serviço que se deseja chamar.</param>
        /// <returns>Instância do objeto ProxyInfo.</returns>
        public ProxyInfo GetProxy(String endPointAddress)
        {
            var proxy = default(ProxyInfo);
            String proxyFullPath;
            String proxyConfigFullPath;

            String svcUtilAppPath = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "proxy\\SvcUtil.exe");
            String proxyPath =
                Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "Temporary ASP.NET Files\\pn_sustentacao\\PN_DynamicProxy\\");            
            String serviceName = endPointAddress.Split('/').LastOrDefault();
            StringBuilder sbConcat;

            if (String.IsNullOrEmpty(serviceName))
                throw new FaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, this.GetType().Namespace), "Endpoint Inválido.");

            serviceName = serviceName.Replace(".svc", String.Empty).Replace("?wsdl", String.Empty);

            sbConcat = new StringBuilder();
            sbConcat.Append(proxyPath).Append(serviceName).Append(".cs");
            proxyFullPath = sbConcat.ToString();

            sbConcat = new StringBuilder();
            sbConcat.Append(proxyPath).Append(serviceName).Append(".config");
            proxyConfigFullPath = sbConcat.ToString();

            try
            {
                var processInfo = new ProcessStartInfo(svcUtilAppPath);
                processInfo.CreateNoWindow = true;                
                processInfo.UseShellExecute = false;                
                processInfo.RedirectStandardError = true;
                processInfo.RedirectStandardOutput = true;
                processInfo.Arguments = string.Concat(new string[]
			    {
                    "/targetClientVersion:Version35 \"",
					endPointAddress,
					"\" \"/out:",
					proxyFullPath,
					"\" \"/config:",
					proxyConfigFullPath,
					"\""
			    });

                using (Process process = Process.Start(processInfo))
                {
                    process.Start();
                    process.WaitForExit();

                    proxy = new ProxyInfo();

                    using (StreamReader sr = new StreamReader(proxyFullPath, Encoding.UTF8))
                    {
                        string contentClassProxy;
                        contentClassProxy = sr.ReadToEnd();
                        proxy.Class = contentClassProxy;

                        Console.WriteLine(contentClassProxy);
                    }

                    using (StreamReader sr = new StreamReader(proxyConfigFullPath, Encoding.UTF8))
                    {
                        string contentConfigProxy;
                        contentConfigProxy = sr.ReadToEnd();
                        proxy.Config = contentConfigProxy;

                        Console.WriteLine(contentConfigProxy);
                    }
                }
            }
            catch (FileNotFoundException fnfEx)
            {
                var fEx = new FaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, this.GetType().Namespace), base.RecuperarExcecao(fnfEx));
                throw fEx;

            }           
            catch (Exception ex)
            {
                var fEx = new FaultException<GeneralFault>(new GeneralFault(CODIGO_ERRO, this.GetType().Namespace), base.RecuperarExcecao(ex));
                throw fEx;
            }
            finally
            {
                File.Delete(proxyFullPath);
                File.Delete(proxyConfigFullPath);

                if (File.Exists(proxyFullPath))
                {
                    var fEx = new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, this.GetType().Namespace), String.Format("Não foi possível apagar o arquivo temporário: {0}", proxyFullPath));

                    throw fEx;
                }

                if (File.Exists(proxyConfigFullPath))
                {
                    var fEx = new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, this.GetType().Namespace), String.Format("Não foi possível apagar o arquivo temporário: {0}", proxyConfigFullPath));

                    throw fEx;
                }

            }

            return proxy;
        }        
    
    }
}
