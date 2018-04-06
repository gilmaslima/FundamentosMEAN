using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Configuration;

namespace Redecard.PN.DadosCadastrais.ISRobo
{
    public class LogProcessamento
    {
        /// <summary>
        /// Entidades que foram processadas
        /// </summary>
        public List<Modelo.Entidade> Entidades { get; set; } 

        /// <summary>
        /// Contrutor privado para não permitir instâncias
        /// </summary>
        private LogProcessamento() : base() { }

        /// <summary>
        /// Carregar o modelo de entidades processadas no dia atual
        /// </summary>
        /// <returns></returns>
        public static LogProcessamento Load(String tipo)
        {
            String absolutePath = GetLogFullPath(tipo);
            LogProcessamento _model = null;

            if (File.Exists(absolutePath)) {

                FileStream fileStream = null;
                try {
                    fileStream = File.Open(absolutePath, FileMode.OpenOrCreate);
                    XmlSerializer serializer = new XmlSerializer(typeof(LogProcessamento));
                    _model = serializer.Deserialize(fileStream) as LogProcessamento;

                    fileStream.Flush();
                    fileStream.Close();
                    fileStream.Dispose();
                }
                finally {
                    if (!object.ReferenceEquals(fileStream, null)) {
                        fileStream.Close();
                        fileStream.Dispose();
                    }
                }
            }
            else {
                _model = new LogProcessamento();
                _model.Entidades = new List<Modelo.Entidade>();
            }

            return _model;
        }

        /// <summary>
        /// Recuperar o caminho absoluto do arquivo de Log
        /// </summary>
        /// <returns></returns>
        public static String GetLogFullPath(String tipo)
        {
            EnsureLogDirectory();
            String _logDirectory = ConfigurationManager.AppSettings["DiretorioLog"];

            String fileName = String.Format("LOG{0}{1}.txt", tipo, DateTime.Now.ToString("yyyy-MM-dd"));
            String absolutePath = String.Concat(_logDirectory, @"\" + fileName);

            return absolutePath;
        }

        /// <summary>
        /// Garantir para que o diretório dos arquivos de Log já exista ou seja criado
        /// </summary>
        public static void EnsureLogDirectory() {
            //String _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LOG");
            String _logDirectory = ConfigurationManager.AppSettings["DiretorioLog"];
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
        }

        /// <summary>
        /// Salvar o arquivo de log com as últimos entidades processadas
        /// </summary>
        public void Save(String tipo) {
            String absolutePath = GetLogFullPath(tipo);
            
            FileStream fileStream = null;
            try {

                fileStream = File.Open(absolutePath, FileMode.OpenOrCreate);
                XmlSerializer serializer = new XmlSerializer(GetType());
                serializer.Serialize(fileStream, this);

                fileStream.Flush();
                fileStream.Close();
            }
            finally {
                if (!object.ReferenceEquals(fileStream, null)) {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }

    }
}
