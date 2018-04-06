using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Configuration;

using Redecard.PN.Comum;
using System.Threading;
using System.ServiceModel;
namespace Redecard.Emissores.UploaderRobot
{
    public partial class ServiceUploader : ServiceBase
    {
        //private FileSystemWatcher fileWatcher = null;

        Thread threadProcessamento;
        Boolean servicoIniciado;
        public const String SERVICENAME = "UploadEmissoresRobot";
        public ServiceUploader()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            servicoIniciado = true;

            //this.ServiceName = SERVICENAME;
            threadProcessamento = new Thread(this.Processar);

            threadProcessamento.Start();
        }

        /// <summary>
        /// Método usado na thread para processamento
        /// </summary>
        private void Processar()
        {
            string pastaMonitorada = ConfigurationManager.AppSettings["pastaMonitorada"];
            string arquivo = string.Empty;
            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(pastaMonitorada);

                while (servicoIniciado)
                {
                    foreach (FileInfo fliInfo in dInfo.GetFiles())
                    {
                        if (fliInfo.Exists)
                        {
                            //SalvarArquivo(fInfo.FullName);
                            arquivo = fliInfo.FullName;

                            Processador processador = new Processador(fliInfo);
                            processador.ServiceName = SERVICENAME;
                            processador.ProcessaArquivo();
                            //processador.Close();

                            //ProcessaArquivo(fInfo);
                        }
                    }
                    if (servicoIniciado)
                    {
                        Int32 intervalo = Int32.Parse(ConfigurationManager.AppSettings["IntervaloEntidades"]);
                        Double timeStamp = (Double)intervalo / 60000;

                        EventLog.WriteEntry(String.Format("------ Aguardar novo processamento Entidades. [Sleep({0} minutos)] -------", intervalo.ToString()));
                        
                        Thread.Sleep((Int32)TimeSpan.FromMinutes(intervalo).TotalMilliseconds);
                    }
                }

            }
            catch (ThreadStartException ex)
            {
                EventLog.WriteEntry(SERVICENAME, String.Concat("Erro no processamento: ", arquivo, " - ", ex.StackTrace), EventLogEntryType.Error);
            }
            catch (ThreadAbortException ex)
            {
                EventLog.WriteEntry(SERVICENAME, String.Concat("Erro no processamento: ", arquivo, " - ", ex.StackTrace), EventLogEntryType.Error);
            }           
            catch (FileNotFoundException ex)
            {
                EventLog.WriteEntry(SERVICENAME, "Arquivo Não encontrado: " + ex.Message, EventLogEntryType.Error);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(SERVICENAME, String.Concat("Erro no processamento: ", arquivo, " - ", ex.StackTrace), EventLogEntryType.Error);
            }
           // Thread.CurrentThread.Abort();
        }


        protected override void OnStop()
        {
            servicoIniciado = false;
            threadProcessamento.Join(new TimeSpan(0, 1, 0));
        }


        /// <summary>
        /// Move o arquivo para a pasta de backup
        /// </summary>
        /// <param name="arquivo"></param>
        private void MoverArquivo(String arquivo)
        {
            try
            {
                string pastaDestino = ConfigurationManager.AppSettings["pastaBackup"];

                FileInfo flInfo = new FileInfo(arquivo);
                if (flInfo.Exists)
                {
                    string nomeArquivoDestino = string.Empty;
                    if (!string.IsNullOrEmpty(flInfo.Extension))
                    {
                        nomeArquivoDestino = string.Concat(flInfo.Name.Substring(0, flInfo.Name.IndexOf(flInfo.Extension) - 1), "-", DateTime.Now.ToString("yyyyMMddHHmmss"), flInfo.Extension);
                    }
                    else
                    {
                        nomeArquivoDestino = string.Concat(flInfo.Name, "-", DateTime.Now.ToString("yyyyMMddHHmmss"));
                    }
                    flInfo.MoveTo(Path.Combine(pastaDestino, nomeArquivoDestino));
                    EventLog.WriteEntry(SERVICENAME, String.Format("Arquivo - {0} - movido com sucesso!", nomeArquivoDestino), EventLogEntryType.Information);
                }
            }
            catch (FileNotFoundException ex)
            {
                EventLog.WriteEntry(SERVICENAME, "Arquivo Não encontrado: " + ex.Message, EventLogEntryType.Error);
            }
            catch (Exception ex)
            {

                EventLog.WriteEntry(SERVICENAME, "Falha ao mover arquivo emissor para a pasta de backup: " + ex.Message, EventLogEntryType.Error);
            }

        }
        /// <summary>
        /// Processa o arquivo para separar por Emissor e enviar ao Banco de dados
        /// </summary>
        /// <param name="infoArquivo"></param>
        private void ProcessaArquivo(FileInfo infoArquivo)
        {

            FileInfo objFileInfoDestino = null;
            StreamWriter objStreamWriter = null;
            String pastaTemporaria = ConfigurationManager.AppSettings["pastaTemporaria"];
            string mes = "";
            string ano = "";
            string emissor = "";
            string nomeDestino = "";

            ArquivoEmissoresServico.DadosArquivo dadosArquivo = new ArquivoEmissoresServico.DadosArquivo();
            try
            {

                StreamReader objStreamReader = infoArquivo.OpenText();
                string input = null;
                objStreamWriter = null;

                while ((input = objStreamReader.ReadLine()) != null)
                {
                    if ((input.Substring(0, 1) == "0") || (input.IndexOf("EMISSOR") >= 0))        //Header do Arquivo
                    {
                        dadosArquivo = new ArquivoEmissoresServico.DadosArquivo();

                        emissor = input.Substring(1, 3);      //Localiza o Emissor
                        dadosArquivo.CodigoEmissor = emissor;
                        objFileInfoDestino = new FileInfo(Path.Combine(pastaTemporaria, input.Substring(1, 3) + ".tmp"));
                        objStreamWriter = objFileInfoDestino.CreateText();

                        if ((input.IndexOf("EMISSOR") >= 0))
                            objStreamWriter.WriteLine(input);
                    }
                    else
                    {
                        if (input.Substring(0, 7) == "EMISSOR")     //Segunda Linha.
                        {

                            ano = input.Trim().Substring(input.Trim().Length - 4, 4);

                            //Verifica mês de emissão e converte mês do arquivo para mês anterior ao da emissão
                            mes = Convert.ToString(Convert.ToInt32(input.Trim().Substring(input.Trim().Length - 7, 2)) - 1);

                            if (mes.Length == 1)
                            {
                                mes = "0" + mes;
                            }

                            //Se o mês de emissão for Janeiro, converte mês do arquivo para o mês de Dezembro e ano do arquivo para ano anterior
                            if (mes == "00")
                            {
                                ano = Convert.ToString(Convert.ToInt32(ano) - 1);
                                mes = "12";
                            }
                            dadosArquivo.Ano = ano;
                            dadosArquivo.Mes = mes;
                            dadosArquivo.DataCriacao = DateTime.Now;
                        }
                        if (input.Trim() == ("9" + emissor))          //Trailler do Arquivo
                        {
                            objStreamWriter.Close();
                            nomeDestino = String.Format("{0}_{1}_{2}.txt", emissor, mes, ano);


                            using (System.IO.FileStream stream =
                            new System.IO.FileStream(objFileInfoDestino.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                            {
                                dadosArquivo.NomeArquivo = nomeDestino;
                                dadosArquivo.Arquivo = stream;
                                dadosArquivo.Tamanho = (Int32)dadosArquivo.Arquivo.Length;
                                SalvarArquivo(dadosArquivo);

                            }
                            objFileInfoDestino.Delete();
                        }
                        else
                        {
                            objStreamWriter.WriteLine(input);

                        }
                    }
                }

                if (!object.Equals(objStreamWriter, null))
                {
                    objStreamWriter.Close();
                    nomeDestino = String.Format("{0}_{1}_{2}.txt", emissor, mes, ano);


                    using (System.IO.FileStream stream =
                    new System.IO.FileStream(objFileInfoDestino.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        dadosArquivo.NomeArquivo = nomeDestino;
                        dadosArquivo.Arquivo = stream;
                        dadosArquivo.Tamanho = (Int32)dadosArquivo.Arquivo.Length;
                        SalvarArquivo(dadosArquivo);

                    }
                    objFileInfoDestino.Delete();
                }

                objStreamReader.Close();

                if (infoArquivo.Exists)
                    MoverArquivo(infoArquivo.FullName);
            }
            catch (IOException ex)
            {

                EventLog.WriteEntry(SERVICENAME, "Erro de Entrada/Saida: " + ex.Message, EventLogEntryType.Error);
            }
            catch (Exception ex)
            {

                EventLog.WriteEntry(SERVICENAME, "Falha ao processar arquivo: " + ex.Message, EventLogEntryType.Error);
            }
        }


        /// <summary>
        /// Salva o arquivo no banco de dados
        /// </summary>
        /// <param name="dadosArquivo"></param>
        private void SalvarArquivo(ArquivoEmissoresServico.DadosArquivo dadosArquivo)
        {
            try
            {
                using (ArquivoEmissoresServico.ArquivoEmissoresServicoClient client = new ArquivoEmissoresServico.ArquivoEmissoresServicoClient())
                {

                    client.SalvarArquivo(dadosArquivo.Ano, dadosArquivo.CodigoEmissor, dadosArquivo.DataCriacao, dadosArquivo.Mes, dadosArquivo.NomeArquivo, (int)dadosArquivo.Arquivo.Length, dadosArquivo.Arquivo);

                    EventLog.WriteEntry(SERVICENAME, String.Format("Arquivo - {0} - Gravado com sucesso!", dadosArquivo.NomeArquivo), EventLogEntryType.Information);

                }

            }
            catch (FaultException<PortalRedecardException> ex)
            {
                EventLog.WriteEntry(SERVICENAME, "Falha ao gravar arquivo: " + ex.Message, EventLogEntryType.Error);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(SERVICENAME, "Falha ao gravar arquivo: " + ex.Message, EventLogEntryType.Error);

            }
        }

    }
}
