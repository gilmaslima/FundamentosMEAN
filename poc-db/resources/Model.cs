
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Text;

namespace Redecard.PN.DadosCadastrais.ServicoPCI {
    
    /// <summary>
    /// Armazena o modelo de entidade(s)/usuario(s) que foram processados pela rotina de informe
    /// </summary>
    public class Model {

        /// <summary>
        /// Entidades que foram processadas
        /// </summary>
        public List<Entidade> Entidades { get; set; } 

        /// <summary>
        /// Contrutor privado para não permitir instâncias
        /// </summary>
        private Model() : base() { }

        /// <summary>
        /// Carregar o modelo de entidades processadas no dia atual
        /// </summary>
        /// <returns></returns>
        public static Model Load() {
            String absolutePath = GetLogFullPath();
            Model _model = null;

            if (File.Exists(absolutePath)) {

                FileStream fileStream = null;
                try {
                    fileStream = File.Open(absolutePath, FileMode.OpenOrCreate);
                    XmlSerializer serializer = new XmlSerializer(typeof(Model));
                    _model = serializer.Deserialize(fileStream) as Model;

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
                _model = new Model();
                _model.Entidades = new List<Entidade>();
            }

            return _model;
        }

        /// <summary>
        /// Recuperar o caminho absoluto do arquivo de Log
        /// </summary>
        /// <returns></returns>
        public static String GetLogFullPath() {
            EnsureLogDirectory();
            String fileName = String.Format("LOG{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));
            String absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"LOG\" + fileName);

            return absolutePath;
        }

        /// <summary>
        /// Recuperar o HTML do arquivo de modelo de e-mail
        /// </summary>
        /// <returns></returns>
        public static String GetMailHTML()
        {
            StreamReader reader = null;
            try
            {
                String mailModelHtmlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"MAIL\model.htm");
                reader = File.OpenText(mailModelHtmlFile);
                string _html = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
                return _html;
            }
            finally
            {
                if (!object.ReferenceEquals(reader, null))
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
        }

        /// <summary>
        /// Garantir para que o diretório dos arquivos de Log já exista ou seja criado
        /// </summary>
        public static void EnsureLogDirectory() {
            String _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LOG");
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
        }

        /// <summary>
        /// Salvar o arquivo de log com as últimos entidades processadas
        /// </summary>
        public void Save() {
            String absolutePath = GetLogFullPath();
            
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

    /// <summary>
    /// Representa uma entidade processada pelo serviço
    /// </summary>
    public class Entidade {

        /// <summary>
        /// Identifica se os informes para a entidade já foi enviado no dia atual
        /// </summary>
        [XmlAttribute]
        public Boolean Processada { get; set; }

        /// <summary>
        /// Lista de usuários que serão bloqueados para esta entidade
        /// </summary>
        public List<Usuario> Usuarios { get; set; }

        /// <summary>
        /// Código do grupo entidade da entidade atual
        /// </summary>
        [XmlAttribute]
        public Int32 CodigoGrupoEntidade { get; set; }

        /// <summary>
        /// Código da entidade atual
        /// </summary>
        [XmlAttribute]
        public Int32 CodigoEntidade { get; set; }
    }

    /// <summary>
    /// Representa um usuário da entidade que será bloqueado e X dias
    /// </summary>
    public class Usuario {

        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        [XmlAttribute]
        public String NomeUsuario;

        /// <summary>
        /// Código de login do usuário
        /// </summary>
        [XmlAttribute]
        public String Login;

        /// <summary>
        /// Data do último acesso deste usuário, utilizada para verificar daqui a quantos dias o usuário será bloqueado
        /// </summary>
        [XmlAttribute]
        public DateTime DataUltimoAcesso;
    }
}