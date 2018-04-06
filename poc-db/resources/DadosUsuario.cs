/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios
{
    /// <summary>
    /// Classe DTO auxiliar para o processo de criação/edição 
    /// de usuários no módulo Administração de Usuários
    /// </summary>
    [Serializable]
    public class DadosUsuario
    {
        #region [ Propriedades preenchidas no Passo 1: Dados de Cadastro ]

        /// <summary>
        /// Código do Usuário. Se nulo, é novo usuário
        /// </summary>
        public Int32? CodigoIdUsuario { get; set; }

        /// <summary>
        /// Nome do Usuário
        /// </summary>
        public String Nome { get; set; }

        /// <summary>
        /// E-mail do usuário
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// E-mail temporário do usuário
        /// </summary>
        public String EmailTemporario { get; set; }

        /// <summary>
        /// CPF do usuário
        /// </summary>
        public Int64? Cpf { get; set; }
        
        /// <summary>
        /// Senha do usuário
        /// </summary>
        public String Senha { get; set; }

        /// <summary>
        /// E-mail secundário do usuário
        /// </summary>
        public String EmailSecundario { get; set; }

        /// <summary>
        /// DDD do Celular do usuário
        /// </summary>
        public Int32? CelularDdd { get; set; }

        /// <summary>
        /// Número de Celular do Usuário (sem DDD)
        /// </summary>
        public Int32? CelularNumero { get; set; }

        /// <summary>
        /// Tipo do Usuário, de acordo com suas permissões.
        /// Propriedade preenchida apenas no modo de Edição de Usuário
        /// </summary>
        public String TipoUsuario { get; set; }

        /// <summary>
        /// Origem do Usuário (A: Aberta, F: Fechada)
        /// </summary>
        public Char Origem { get; set; }

        /// <summary>
        /// Usuário é legado
        /// </summary>
        public Boolean Legado { get; set; }

        /// <summary>
        /// Indica se o usuário é Master (TipoUsuario="M")
        /// </summary>
        public Boolean UsuarioMaster { get { return String.Compare("M", this.TipoUsuario, true) == 0; } }

        /// <summary>
        /// Indica se o usuário foi criado pela parte Aberta
        /// </summary>
        public Boolean OrigemAberta { get { return this.Origem == 'A'; } }

        /// <summary>
        /// Indica se o usuário foi criado pela parte Migração
        /// </summary>
        public Boolean OrigemMigracao { get { return this.Origem == 'M'; } }

        /// <summary>
        /// Indica se o usuário foi criado pela parte Fechada
        /// </summary>
        public Boolean OrigemFechada { get { return this.Origem == 'F'; } }

        /// <summary>
        /// Status do usuário
        /// </summary>
        public Status Status { get; set; }

        #endregion

        #region [ Propriedades preenchidas no Passo 2: Estabelecimentos Permitidos ]

        /// <summary>
        /// Lista de Estabelecimentos Permitidos ao usuário
        /// </summary>
        public List<Int32> Estabelecimentos { get; set; }

        /// <summary>
        /// Descrição dos Estabelecimentos (utilizado apenas quando Usuário logado em uma Filial,
        /// editando outro usuário)
        /// </summary>
        [Obsolete]
        public Dictionary<Int32, String> DescricaoEstabelecimentos { get; set; }

        /// <summary>
        /// Getter: Código dos Estabelecimentos Permitidos ao usuário separados por ","
        /// </summary>
        public String CodigoEstabelecimentos
        {
            get
            {
                //Concatena o código dos estabelecimentos
                return String.Join(",", this.Estabelecimentos.Select(codigo => codigo.ToString()).ToArray());
            }
        }

        #endregion

        #region [ Propriedades preenchidas no Passo 3: Permissões ]
        
        /// <summary>
        /// Lista de serviços (básicos e transacionais) permitidos ao usuário
        /// </summary>
        public List<Int32> Servicos { get; set; }

        /// <summary>
        /// Getter: Código dos serviços (Básicos + Transacionais) 
        /// que o usuário tem permissão, separados por ","
        /// </summary>
        public String CodigoServicos
        {
            get
            {
                //Prepara lista única contendo todas as permissões
                var codigoServicos = this.Servicos ?? new List<Int32>();

                //Concatena os códigos dos serviços
                return String.Join(",", codigoServicos.Distinct().Select(codigo => codigo.ToString()).ToArray());
            }
        }
        
        #endregion

        #region [ Construtores ]

        public DadosUsuario()
        {
            Estabelecimentos = new List<Int32>();
            Servicos = new List<Int32>();
        }

        #endregion

        #region [ Override ]

        /// <summary>
        /// Método sobrescrito.
        /// </summary>
        public override string ToString()
        {
            var str = new StringBuilder()
                .Append("Código Usuário: ").Append(CodigoIdUsuario).Append("<br/>")
                .Append("Nome: ").Append(Nome).Append("<br/>")
                .Append("Email: ").Append(Email).Append("<br/>")
                .Append("Cpf: ").Append(Cpf).Append("<br/>")
                .Append("Senha: ").Append(Senha).Append("<br/>")
                .Append("Email Secundário: ").Append(EmailSecundario).Append("<br/>")
                .Append("Celular: ").Append(CelularDdd).Append("-").Append(CelularNumero).Append("<br/>")
                .Append("TipoUsuario: ").Append(TipoUsuario).Append("<br/>"); 

            if (Estabelecimentos != null && Estabelecimentos.Count > 0)
                str.Append("Estabelecimentos:")
                    .Append("<br/>&nbsp;&nbsp;-&nbsp;")
                    .Append(String.Join("<br/>&nbsp;&nbsp;-&nbsp;", 
                        Estabelecimentos.OrderBy(e => e).Select(pv => pv.ToString()).ToArray()))
                    .Append("<br/>");

            if (Servicos != null && Servicos.Count > 0)
                str.Append("Serviços: ")
                    .Append("<br/>&nbsp;&nbsp;-&nbsp;")
                    .Append(String.Join("<br/>&nbsp;&nbsp;-&nbsp;", 
                        Servicos.OrderBy(s => s).Select(p => p.ToString()).ToArray()))
                    .Append("<br/>");

            return str.ToString();
        }

        #endregion
    }
}