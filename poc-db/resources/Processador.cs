using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using System.ServiceModel;
using Redecard.PN.Comum;

namespace Redecard.Emissores.UploaderRobot
{
    public class Processador
    {


        public FileInfo FileInfoOrigem { get; set; }

        public String PastaTemporaria
        {
            get
            {
                String retorno = ConfigurationManager.AppSettings["pastaTemporaria"];

                return retorno;

            }
        }
        public String PastaDestino
        {
            get
            {
                String retorno = ConfigurationManager.AppSettings["pastaBackup"];

                return retorno;

            }
        }
        public String ServiceName { get; set; }
        public String Ano { get; set; }
        public String Mes { get; set; }
        public String Emissor { get; set; }
        public String NomeArquivoDestino
        {
            get
            {
                String retorno = string.Empty;

                if (!String.IsNullOrEmpty(Ano) && !String.IsNullOrEmpty(Mes) && !String.IsNullOrEmpty(Emissor))
                {
                    retorno = String.Format("{0}_{1}_{2}.txt", Emissor, Mes, Ano);

                }
                return retorno;
            }

        }

        public Processador(FileInfo infoArquivo)
        {
            FileInfoOrigem = infoArquivo;
        }


        private FileInfo FileInfoTemporario { get; set; }

        private StreamWriter StrWriterDestino { get; set; }

        private ArquivoEmissoresServico.DadosArquivo DadosArquivoProcessado { get; set; }

        /// <summary>
        /// Processa o arquivo para separar por Emissor e enviar ao Banco de dados
        /// </summary>
        /// <param name="infoArquivo"></param>
        public void ProcessaArquivo()
        {
            DadosArquivoProcessado = new ArquivoEmissoresServico.DadosArquivo();
            string input = null;
            Boolean retorno = true;
            try
            {

                StreamReader objStreamReader = FileInfoOrigem.OpenText();
                StrWriterDestino = null;

                Int32 linha = 0;
                while ((input = objStreamReader.ReadLine()) != null)
                {
                    if (linha == 0)        //Header do Arquivo
                    {
                        DadosArquivoProcessado = new ArquivoEmissoresServico.DadosArquivo();

                        if (!string.IsNullOrEmpty(input.Trim()))
                        {
                            if (input.StartsWith("0"))
                            {
                                Emissor = input.Substring(1, 3);      //Localiza o Emissor
                            }
                            else if (input.StartsWith("EMISSOR"))
                            {
                                Emissor = input.Substring(8, 4).Trim();      //Localiza o Emissor

                                PreencheDadosArquivo(input);
                            }
                            DadosArquivoProcessado.CodigoEmissor = Emissor;
                            FileInfoTemporario = new FileInfo(Path.Combine(PastaTemporaria, Emissor + ".tmp"));
                            StrWriterDestino = FileInfoTemporario.CreateText();

                            if (input.StartsWith("EMISSOR"))
                            {
                                StrWriterDestino.WriteLine(input);
                            }
                            linha++;
                        }
                    }
                    else
                    {
                        if (input.StartsWith("EMISSOR"))     //Segunda Linha.
                        {
                            PreencheDadosArquivo(input);
                        }
                        if (input.Trim() == ("9" + Emissor))          //Trailler do Arquivo
                        {
                            retorno = FecharArquivo();
                            StrWriterDestino = null;
                            linha = 0;
                        }
                        else
                        {
                            StrWriterDestino.WriteLine(input);

                        }
                    }
                }

                if (!object.Equals(StrWriterDestino, null))
                {
                    retorno = FecharArquivo();
                }

                objStreamReader.Close();

                if (FileInfoOrigem.Exists && retorno)
                    MoverArquivo(FileInfoOrigem.FullName);
            }
            catch (IOException ex)
            {

                EventLog.WriteEntry(ServiceName, String.Concat("Erro de Entrada/Saida: ", input, ex.StackTrace), EventLogEntryType.Error);
            }
            catch (Exception ex)
            {

                EventLog.WriteEntry(ServiceName, String.Concat("Falha ao processar arquivo: ", input, ex.StackTrace), EventLogEntryType.Error);
            }
        }
        void PreencheDadosArquivo(String linha)
        {

            Ano = linha.Trim().Substring(linha.Trim().Length - 4, 4);

            //Verifica mês de emissão e converte mês do arquivo para mês anterior ao da emissão
            Mes = Convert.ToString(Convert.ToInt32(linha.Trim().Substring(linha.Trim().Length - 7, 2)) - 1);


            Mes = String.Concat("00", Mes).Right(2);
            //Se o mês de emissão for Janeiro, converte mês do arquivo para o mês de Dezembro e ano do arquivo para ano anterior
            if (Mes == "00")
            {
                Ano = Convert.ToString(Convert.ToInt32(Ano) - 1);
                Mes = "12";
            }
            DadosArquivoProcessado.Ano = Ano;
            DadosArquivoProcessado.Mes = Mes;
            DadosArquivoProcessado.DataCriacao = DateTime.Now;

        }

        Boolean FecharArquivo()
        {
            Boolean retorno = false;
            StrWriterDestino.Close();

            using (System.IO.FileStream stream =
            new System.IO.FileStream(FileInfoTemporario.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                DadosArquivoProcessado.NomeArquivo = NomeArquivoDestino;
                DadosArquivoProcessado.Arquivo = stream;
                DadosArquivoProcessado.Tamanho = (Int32)DadosArquivoProcessado.Arquivo.Length;
                retorno = SalvarArquivo(DadosArquivoProcessado);

            }
            FileInfoTemporario.Delete();
            return retorno;
        }


        /// <summary>
        /// Salva o arquivo no banco de dados
        /// </summary>
        /// <param name="dadosArquivo"></param>
        private Boolean SalvarArquivo(ArquivoEmissoresServico.DadosArquivo dadosArquivo)
        {
            Boolean retorno = false;
            try
            {
                using (ArquivoEmissoresServico.ArquivoEmissoresServicoClient client = new ArquivoEmissoresServico.ArquivoEmissoresServicoClient())
                {

                    client.SalvarArquivo(dadosArquivo.Ano, dadosArquivo.CodigoEmissor, dadosArquivo.DataCriacao, dadosArquivo.Mes, dadosArquivo.NomeArquivo, (int)dadosArquivo.Arquivo.Length, dadosArquivo.Arquivo);
                    retorno = true;
                    EventLog.WriteEntry(ServiceName, String.Format("Arquivo - {0} - Gravado com sucesso!", dadosArquivo.NomeArquivo), EventLogEntryType.Information);

                }

            }
            catch (FaultException<PortalRedecardException> ex)
            {
                EventLog.WriteEntry(ServiceName, "Falha ao gravar arquivo: " + ex.StackTrace, EventLogEntryType.Error);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ServiceName, "Falha ao gravar arquivo: " + ex.StackTrace, EventLogEntryType.Error);

            }
            return retorno;
        }



        /// <summary>
        /// Move o arquivo para a pasta de backup
        /// </summary>
        /// <param name="arquivo"></param>
        private void MoverArquivo(String arquivo)
        {
            try
            {

                FileInfo flInfo = new FileInfo(arquivo);
                if (flInfo.Exists)
                {
                    string nomeArquivoDestino = string.Empty;
                    if (!string.IsNullOrEmpty(flInfo.Extension))
                    {
                        nomeArquivoDestino = string.Concat(flInfo.Name.Substring(0, flInfo.Name.IndexOf(flInfo.Extension)), "-", DateTime.Now.ToString("yyyyMMddHHmmss"), ".txt");
                    }
                    else
                    {
                        nomeArquivoDestino = string.Concat(flInfo.Name, "-", DateTime.Now.ToString("yyyyMMddHHmmss"));
                    }
                    flInfo.MoveTo(Path.Combine(PastaDestino, nomeArquivoDestino));
                    EventLog.WriteEntry(ServiceName, String.Format("Arquivo - {0} - movido com sucesso!", nomeArquivoDestino), EventLogEntryType.Information);
                }
            }
            catch (FileNotFoundException ex)
            {
                EventLog.WriteEntry(ServiceName, "Arquivo Não encontrado: " + ex.StackTrace, EventLogEntryType.Error);
            }
            catch (Exception ex)
            {

                EventLog.WriteEntry(ServiceName, "Falha ao mover arquivo emissor para a pasta de backup: " + ex.StackTrace, EventLogEntryType.Error);
            }

        }

        internal void Close()
        {
            StrWriterDestino.Close();

        }
    }
}
