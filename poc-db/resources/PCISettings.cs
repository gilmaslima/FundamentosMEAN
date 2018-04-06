using System;
using System.Configuration;

namespace Redecard.PN.DadosCadastrais.ServicoPCI {

    /// <summary>
    /// Propriedades de configuração do serviço de informe de bloqueio de usuários
    /// </summary>
    public class PCISettings : ConfigurationSection {

        /// <summary>
        /// Objeto utilizado para garantir o bloqueio do código e só permitir uma execução por vez
        /// </summary>
        static object settingsLock = new object();

        /// <summary>
        /// Objeto estático que armazena as configurações do arquivo App.config
        /// </summary>
        static PCISettings settings = null;

        /// <summary>
        /// Intervalo de execução da rotina de informe (envio de e-mail)
        /// </summary>
        [ConfigurationProperty("interval", DefaultValue = 5, IsRequired = true, IsKey = false)]
        public Int32 Interval {
            get {
                return (Int32)this["interval"];
            }
            set {
                this["interval"] = value;
            }
        }

        /// <summary>
        /// Porta utilizada para a conexão com o servidor SMTP
        /// </summary>
        [ConfigurationProperty("port", DefaultValue = 25, IsRequired = true, IsKey = false)]
        public Int32 Port {
            get {
                return (Int32)this["port"];
            }
            set {
                this["port"] = value;
            }
        }

        /// <summary>
        /// Servidor SMTP que deve ser utilizado para o envio de e-mail
        /// </summary>
        [ConfigurationProperty("smtp", DefaultValue = "pi", IsRequired = true, IsKey = false)]
        public String SmtpServer {
            get {
                return (String)this["smtp"];
            }
            set {
                this["smtp"] = value;
            }
        }

        /// <summary>
        /// E-mail utilizado como &quot;sender&quot; dos informes disparados pelo serviço
        /// </summary>
        [ConfigurationProperty("email", DefaultValue = "faleconosco@userede.com.br", IsRequired = true, IsKey = false)]
        public String EmailAddress {
            get {
                return (String)this["email"];
            }
            set {
                this["email"] = value;
            }
        }

        /// <summary>
        /// Assunto do e-mail enviado pelo serviço
        /// </summary>
        [ConfigurationProperty("subject", DefaultValue = "Bloqueio de Usuários", IsRequired = false, IsKey = false)]
        public String Subject {
            get {
                return (String)this["subject"];
            }
            set {
                this["subject"] = value;
            }
        }

        /// <summary>
        /// Número de entidades que serão processadas a cada execução do serviço
        /// </summary>
        [ConfigurationProperty("numberOfEntities", DefaultValue = 10, IsRequired = true, IsKey = false)]
        public Int32 NumberOfEntities {
            get {
                return (Int32)this["numberOfEntities"];
            }
            set {
                this["numberOfEntities"] = value;
            }
        }

        /// <summary>
        /// Endereço de e-mail fixo para envio do log de usuários com acesso expirando
        /// </summary>
        [ConfigurationProperty("emailTo", DefaultValue = "agnaldo.almeida@iteris.com.br", IsRequired = true, IsKey = false)]
        public String EmailTo
        {
            get
            {
                return (String)this["emailTo"];
            }
            set
            {
                this["emailTo"] = value;
            }
        }

        /// <summary>
        /// Defini se a rotina será executada na inicialização do serviço
        /// </summary>
        [ConfigurationProperty("executeOnInit", DefaultValue = true, IsRequired = true, IsKey = false)]
        public Boolean ExecuteOnInit {
            get {
                return (Boolean)this["executeOnInit"];
            }
            set {
                this["executeOnInit"] = value;
            }
        }

        /// <summary>
        /// Carregar as configurações do arquivo App.config
        /// </summary>
        /// <returns></returns>
        public static PCISettings Load() {
            lock (settingsLock) {
                if (object.ReferenceEquals(settings, null)) {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    settings = config.GetSection("pciSettings") as PCISettings;
                }
                return settings;
            }
        }

        /// <summary>
        /// Construtor privado para não permitir instâncias
        /// </summary>
        private PCISettings() { }
    }
}