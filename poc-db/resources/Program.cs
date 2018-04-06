using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using log4net;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Rede.AppConciliador.LogMonitoracao;

namespace Rede.AppConciliador
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        private const string mutexID = "Rede-PN-App-Conciliador";
        /// <summary>
        /// 
        /// </summary>
        private static string existingBucketName = default(String);
        /// <summary>
        /// 
        /// </summary>
        private static string AWSAccessKeyId = default(String);
        /// <summary>
        /// 
        /// </summary>
        private static string AWSSecretKey = default(String);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static ILog logger = null;

        /// <summary>
        /// 
        /// </summary>
        static ILog Logger
        {
            get
            {
                if (object.ReferenceEquals(logger, null))
                {
                    logger = LogManager.GetLogger(typeof(Program));
                }
                return logger;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="awsBucketName"></param>
        /// <param name="awsAccessKeyId"></param>
        /// <param name="awsSecretKey"></param>
        static void PopulateParameters(string awsBucketName = "", string awsAccessKeyId = "", string awsSecretKey = "")
        {
            existingBucketName = (String.IsNullOrEmpty(awsBucketName) ? ConfigurationManager.AppSettings["awsBucketName"] : awsBucketName);

            AWSAccessKeyId = (String.IsNullOrEmpty(awsAccessKeyId) ? ConfigurationManager.AppSettings["awsAccessKeyId"] : awsAccessKeyId);

            AWSSecretKey = (String.IsNullOrEmpty(awsSecretKey) ? ConfigurationManager.AppSettings["awsSecretKey"] : awsSecretKey);

            // gravar log dos parametros
            Logger.Info(String.Format("Parâmetros obtidos: [{0}, {1}, {2}]", existingBucketName, AWSAccessKeyId, AWSSecretKey));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Process PriorProcess()
        // Returns a System.Diagnostics.Process pointing to
        // a pre-existing process with the same name as the
        // current one, if any; or null if the current process
        // is unique.
        {
            Process curr = Process.GetCurrentProcess();
            Process[] procs = Process.GetProcessesByName(curr.ProcessName);

            foreach (Process p in procs)
            {
                if ((p.Id != curr.Id) &&
                    (p.MainModule.FileName == curr.MainModule.FileName))
                    return p;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        static bool IsFileLocked(string file)
        {
            FileStream stream = null;

            try
            {
                var fileInfo = new FileInfo(file);
                stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //O arquivo ainda está indisponível para uso por um desses motivos:
                //- Ainda está copiado para a pasta
                //- Está em uso por outro processo
                //- Não existe a o caminho definido em 'file' é inválido
                Logger.Warn(String.Format("O arquivo '{0}' ainda está indisponível para uso.", file));
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            //arquivo liberado para uso
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        static void Main(string[] args)
        {
            if (PriorProcess() != null)
            {
                Logger.Info("Processamento anterior ainda em execução.");
                return;
            }
            else
            {
                Logger.Info("Iniciando processamento dos arquivos do Conciliador.");
                String filePath = String.Empty;
                // preencher parametros de configuração do bucket da AWS
                PopulateParameters();
                try
                {
                    var uploadFilesFolder = Path.Combine(ConfigurationManager.AppSettings["uploadDir"], "upload");
                    var filteredFiles = Directory.GetFiles(uploadFilesFolder, "*.*")
                            .Where(file => !file.ToLower().Contains(".info")).ToList();

                    if (filteredFiles.Count > 0)
                    {
                        filteredFiles.ForEach(delegate (string file)
                        {
                            filePath = file;
                            if (!IsFileLocked(file))
                            {
                                RegionEndpoint endpoint = RegionEndpoint.USEast1;
                                TransferUtility fileTransferUtility = new
                                        TransferUtility(AWSAccessKeyId, AWSSecretKey, endpoint);
                                string key = string.Format("Rede/{0}", Path.GetFileName(file));
                                TransferUtilityUploadRequest fileTransferUtilityRequest = new TransferUtilityUploadRequest
                                {
                                    BucketName = existingBucketName,
                                    FilePath = filePath,
                                    Key = key,
                                    StorageClass = S3StorageClass.ReducedRedundancy,
                                    PartSize = 5000 * 5000, // 5MB
                                    CannedACL = S3CannedACL.PublicRead
                                };

                                fileTransferUtilityRequest.UploadProgressEvent += FileUploadProgressEvent;

                                Logger.Info(String.Format("Iniciando transferência do arquivo {0}.", filePath));

                                DateTime inicio = DateTime.Now;
                                fileTransferUtility.Upload(fileTransferUtilityRequest);
                                DateTime fim = DateTime.Now;

                                GravarLogHistorico(filePath,
                                        inicio,
                                        fim,
                                        LogHistorico.StatusEnvio.Sucesso,
                                        "Processo de upload do arquivo concluído.");

                                try
                                {
                                    // mover arquivo assim que a transferência for concluída
                                    File.Move(file, file.Replace("upload", "uploaded"));
                                }
                                catch (IOException ioException)
                                {
                                    // um arquivo de mesmo nome já existe já existe, mover o arquivo com outro nome
                                    Logger.Warn(String.Format("O arquivo especificado já existe na pasta de 'uploaded', o arquivo será movido com outro nome. Mensagem original : {0}.", ioException.Message));
                                    File.Move(file, String.Concat(file, String.Format("_{0}", Guid.NewGuid().ToString("N"))));
                                }
                                catch (Exception exception)
                                {
                                    Logger.Error("Erro desconhecido ao mover o arquivo para a pasta de carregados do servidor, validar o espaço em disco e dados de acesso a pasta.", exception);
                                }
                            }
                        });
                    }
                    else
                    {
                        Logger.Info("Não existem novos arquivos para carregar no S3 do Conciliador.");
                        GravarLogHistorico(String.Empty,
                                DateTime.Now,
                                DateTime.Now,
                                LogHistorico.StatusEnvio.Sucesso,
                                "Não existem novos arquivos para carregar no S3 do Conciliador.");
                    }

                    Logger.Info("Processo de upload dos arquivos concluído.");
                    Environment.Exit(0); // Saída com sucesso no Control+M
                }
                catch (AmazonS3Exception s3Exception)
                {
                    Logger.Error("Erro no S3 AWS ao transferir arquivo para o S3 da AWS.", s3Exception);
                    GravarLogHistorico(filePath,
                            DateTime.Now,
                            DateTime.Now,
                            LogHistorico.StatusEnvio.Erro,
                            s3Exception.Message);
                    Environment.Exit(10); // Saída com erro no Control+M
                }
                catch (Exception exception)
                {
                    Logger.Error("Erro desconhecido ao transferir arquivo para o S3 da AWS.", exception);
                    GravarLogHistorico(filePath,
                            DateTime.Now,
                            DateTime.Now,
                            LogHistorico.StatusEnvio.Erro,
                            exception.Message);
                    Console.Write(11); // Saída com erro no Control+M
                    Environment.Exit(11);
                }
            }
        }
        /// <summary>
        /// Grava Histórico de log para o envio de arquivo
        /// </summary>
        /// <param name="nomeArquivo"></param>
        /// <param name="inicio"></param>
        /// <param name="fim"></param>
        /// <param name="status"></param>
        /// <param name="mensagem"></param>
        private static void GravarLogHistorico(String nomeArquivo, DateTime inicio, DateTime fim, LogHistorico.StatusEnvio status, String mensagem)
        {
            try
            {
                LogHistorico
                    .GravarHistorico(nomeArquivo,
                        inicio,
                        fim,
                        status,
                        mensagem);
            }
            catch (NullReferenceException ex)
            {
                Logger.Error("Erro ao gravar log de envio de arquivos!", ex);
            }
            catch (Exception ex)
            {
                Logger.Error("Erro inesperado ao gravar log de envio de arquivos!", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FileUploadProgressEvent(object sender, UploadProgressArgs e)
        {
            if (e.PercentDone % 5 == 0)
            {
                if (e.PercentDone == 100)
                {
                    Logger.Info(String.Format("Transferência do arquivo '{0}' concluída.", e.FilePath));
                }
                else
                {
                    Logger.Info(String.Format("Transferido {0}% de {1} bytes.", e.PercentDone, e.TotalBytes));
                }
#if DEBUG
                DrawTextProgressBar(e.PercentDone, 100);
#endif
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="total"></param>
        private static void DrawTextProgressBar(int progress, int total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;
            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }
            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }
            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }
    }
}