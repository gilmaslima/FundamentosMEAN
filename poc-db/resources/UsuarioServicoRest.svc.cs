#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [09/05/2012] – [André Garcia] – [Criação]
 * 
 * 
(c) Copyright [2014] Rede
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   :
- [28/04/2014] – [André Rentes] – [ALteração da estrutura, comentários nos métodos, atualização novo acesso]
*/
#endregion

using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using AutoMapper;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.Modelo;

namespace Redecard.PN.DadosCadastrais.Servicos
{

    /// <summary>
    /// Serviço para gerenciamento dos Usuários
    /// </summary>
    public class UsuarioServicoRest : ServicoRestBase, IUsuarioServicoRest
    {
        #region Métodos de consulta

        /// <summary>
        /// Retorna a lista de páginas ao qual o usuário possui acesso
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="login">Login do usuário</param>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <returns>Listagem de páginas que o usuário possui acesso</returns>
        public ResponseBaseList<Servicos.Pagina> ConsultarPermissoes(
            String codigoGrupoEntidade,
            String codigoEntidade,
            String login,
            String codigoIdUsuario)
        {
            return this.ExecucaoTratada<ResponseBaseList<Servicos.Pagina>>("Retorna a lista de páginas que o usuário pode acessar no Portal de Serviços", retorno =>
            {
                Int32 codigoRetorno;
                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();

                var retornoNegocio = new Negocio.Usuario().ConsultarPermissoes(
                    out codigoRetorno,
                    Int32.Parse(codigoGrupoEntidade),
                    Int32.Parse(codigoEntidade),
                    login,
                    Int32.Parse(codigoIdUsuario));

                retorno.Itens = Mapper.Map<List<Modelo.Pagina>, List<Servicos.Pagina>>(retornoNegocio).ToArray();
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = codigoRetorno
                };
            });
        }

        /// <summary>
        /// Consultar a data de último acesso do usuário
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="usuario">usuário</param>
        /// <returns>Data de último acesso</returns>
        public ResponseBaseItem<DateTime> ConsultarDataUltimoAcesso(
            String codigoEntidade,
            String codigoGrupoEntidade,
            String usuario)
        {
            return this.ExecucaoTratada<ResponseBaseItem<DateTime>>("Consultar data último acesso", retorno =>
            {
                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();

                var retornoNegocio = new Negocio.Usuario().ConsultarDataUltimoAcesso(
                    Int32.Parse(codigoEntidade),
                    Int32.Parse(codigoGrupoEntidade),
                    usuario);

                retorno.Item = retornoNegocio;
            });
        }

        /// <summary>
        /// Consultar usuários pela código e senha informada
        /// </summary>
        /// <param name="codigo">Código do usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="entidade">Modelo Entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        public ResponseBaseList<Servicos.Usuario> ConsultarPorCodigoESenha(
            String codigo,
            String senha,
            Entidade entidade)
        {
            return this.Consultar(codigo, senha, entidade);
        }

        /// <summary>
        /// Consultar usuários pela senha informada
        /// </summary>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="entidade">Modelo Entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        public ResponseBaseList<Servicos.Usuario> ConsultarPorSenha(
            String senha,
            Entidade entidade)
        {
            return this.Consultar(null, senha, entidade);
        }

        /// <summary>
        /// Consultar os Usuários com o cpf informado relacionados a Entidade
        /// </summary>
        /// <param name="cpfUsuario"></param>
        /// <param name="codigoEntidade"></param>
        /// <param name="codigoGrupoEntidade"></param>
        /// <returns></returns>
        public ResponseBaseList<Servicos.Usuario> ConsultarPorCpf(
            String cpfUsuario,
            String codigoEntidade,
            String codigoGrupoEntidade)
        {
            return this.ExecucaoTratada<ResponseBaseList<Servicos.Usuario>>("Consultar usuários", retorno =>
            {
                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                Mapper.CreateMap<Modelo.PaginaMenu, Servicos.PaginaMenu>();
                Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                var retornoNegocio = new Negocio.Usuario().ConsultarPorCpf(
                    Int64.Parse(cpfUsuario),
                    Int32.Parse(codigoEntidade),
                    Int32.Parse(codigoGrupoEntidade));

                retorno.Itens = Mapper.Map<List<Modelo.Usuario>, List<Servicos.Usuario>>(retornoNegocio).ToArray();
            });
        }

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="codigo">Código do usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="entidade">Entidade que o usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        public ResponseBaseList<Servicos.Usuario> Consultar(
            String codigo,
            String senha,
            Servicos.Entidade entidade)
        {
            return this.ExecucaoTratada<ResponseBaseList<Servicos.Usuario>>("Consultar usuários", retorno =>
            {
                Modelo.Entidade modeloEntidade = null;
                Int32 codigoRetorno;

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                Mapper.CreateMap<Modelo.PaginaMenu, Servicos.PaginaMenu>();
                Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                if (entidade != null && entidade != default(Servicos.Entidade))
                {
                    Mapper.CreateMap<Servicos.Entidade, Modelo.Entidade>();
                    Mapper.CreateMap<Servicos.TipoEntidade, Modelo.TipoEntidade>();
                    Mapper.CreateMap<Servicos.Status, Modelo.Status>();
                    Mapper.CreateMap<Servicos.GrupoEntidade, Modelo.GrupoEntidade>();

                    // Convert Data Contract Entity para Business Entity
                    modeloEntidade = Mapper.Map<Servicos.Entidade, Modelo.Entidade>(entidade);
                }

                var retornoNegocio = new Negocio.Usuario().Consultar(codigo, senha, modeloEntidade, out codigoRetorno);

                retorno.Itens = Mapper.Map<List<Modelo.Usuario>, List<Servicos.Usuario>>(retornoNegocio).ToArray();
            });
        }

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="codigo">Código do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        public ResponseBaseList<Servicos.Usuario> Consultar(
            String codigo)
        {
            return this.Consultar(codigo, null, default(Servicos.Entidade));
        }

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="codigo">Código do usuário</param>      
        /// <param name="entidade">Entidade que o usuário pertence</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        public ResponseBaseList<Servicos.Usuario> Consultar(
            String codigo,
            Servicos.Entidade entidade)
        {
            return this.Consultar(codigo, null, entidade);
        }

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="entidade">Entidade que o usuário pertence</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        public ResponseBaseList<Servicos.Usuario> Consultar(
            Servicos.Entidade entidade)
        {
            return this.Consultar(null, null, entidade);
        }

        /// <summary>
        /// Método de consulta de usuário em uma entidade, pelo e-mail temporário
        /// </summary>
        /// <param name="emailTemporario">E-mail temporário do usuário</param>
        /// <param name="codigoEntidade">Código da Entidade que o usuário pertence</param>
        /// <param name="codigoGrupoEntidade">Grupo da entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de Modelo Usuario</returns>
        public ResponseBaseItem<Servicos.Usuario> ConsultarPorEmailTemporario(
            String emailTemporario,
            String codigoGrupoEntidade,
            String codigoEntidade)
        {
            return this.ExecucaoTratada<ResponseBaseItem<Servicos.Usuario>>("Consultar usuário por e-mail temporário", retorno =>
            {
                Int32 codigoRetorno;

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                Mapper.CreateMap<Modelo.PaginaMenu, Servicos.PaginaMenu>();
                Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                var retornoNegocio = new Negocio.Usuario().ConsultarPorEmailTemporario(
                    emailTemporario,
                    new Modelo.Entidade()
                    {
                        Codigo = Int32.Parse(codigoEntidade),
                        GrupoEntidade = new Modelo.GrupoEntidade
                        {
                            Codigo = Int32.Parse(codigoGrupoEntidade)
                        }
                    },
                    out codigoRetorno);

                retorno.Item = Mapper.Map<Modelo.Usuario, Servicos.Usuario>(retornoNegocio);
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = codigoRetorno
                };
            });
        }

        /// <summary>
        /// Consulta os usuários por CPF e por Status
        /// </summary>
        /// <param name="cpf"></param>
        /// <param name="codigoGrupoEntidade"></param>
        /// <param name="codigoEntidade"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ResponseBaseList<Servicos.Usuario> ConsultarPorCpfPrincipalPorStatus(
            String cpf,
            String codigoGrupoEntidade,
            String codigoEntidade,
            Int32[] status)
        {
            return this.ExecucaoTratada<ResponseBaseList<Servicos.Usuario>>("Consultar usuário por CPF", retorno =>
            {
                Int32 codigoRetorno;

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                Mapper.CreateMap<Modelo.PaginaMenu, Servicos.PaginaMenu>();
                Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                Modelo.Entidade modeloEntidade = null;

                if (!String.IsNullOrWhiteSpace(codigoEntidade) && codigoEntidade != "0")
                {
                    modeloEntidade = new Modelo.Entidade
                    {
                        Codigo = Int32.Parse(codigoEntidade),
                        GrupoEntidade = new Modelo.GrupoEntidade { Codigo = int.Parse(codigoGrupoEntidade) }
                    };
                }

                var retornoNegocio = new Negocio.Usuario().ConsultarPorCpfPrincipalPorStatus(
                    Int64.Parse(cpf),
                    modeloEntidade,
                    status,
                    out codigoRetorno);

                retorno.Itens = Mapper.Map<Modelo.Usuario[], Servicos.Usuario[]>(retornoNegocio);
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = codigoRetorno
                };
            });
        }

        /// <summary>
        /// Método de consulta de usuário em uma entidade, pelo e-mail
        /// </summary>
        /// <param name="emailUsuario">E-mail do usuário</param>
        /// <param name="codigoEntidade">Código da Entidade que o usuário pertence</param>
        /// <param name="codigoGrupoEntidade">Grupo da entidade</param>
        /// <param name="status">Código de retorno</param>
        /// <returns>Listagem de Modelo Usuario</returns>
        public ResponseBaseList<Servicos.Usuario> ConsultarPorEmailPrincipalPorStatus(
            String emailUsuario,
            String codigoGrupoEntidade,
            String codigoEntidade,
            Int32[] status)
        {
            return this.ExecucaoTratada<ResponseBaseList<Servicos.Usuario>>("Consultar usuário por e-mail principal", retorno =>
            {
                Int32 codigoRetorno;

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                Mapper.CreateMap<Modelo.PaginaMenu, Servicos.PaginaMenu>();
                Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                Modelo.Entidade modeloEntidade = null;

                if (!String.IsNullOrWhiteSpace(codigoEntidade) && codigoEntidade != "0")
                {
                    modeloEntidade = new Modelo.Entidade
                    {
                        Codigo = Int32.Parse(codigoEntidade),
                        GrupoEntidade = new Modelo.GrupoEntidade { Codigo = Int32.Parse(codigoGrupoEntidade) }
                    };
                }

                var retornoNegocio = new Negocio.Usuario().ConsultarPorEmailPrincipalPorStatus(
                    emailUsuario,
                    modeloEntidade,
                    status,
                    out codigoRetorno);

                retorno.Itens = Mapper.Map<Modelo.Usuario[], Servicos.Usuario[]>(retornoNegocio);
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = codigoRetorno
                };
            });
        }


        /// <summary>
        /// Método de consulta de usuário em uma entidade, pelo e-mail
        /// </summary>
        /// <param name="emailUsuario">E-mail do usuário</param>
        /// <param name="codigoEntidade">Código da Entidade que o usuário pertence</param>
        /// <param name="codigoGrupoEntidade">Grupo da entidade</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de Modelo Usuario</returns>
        public ResponseBaseItem<Servicos.Usuario> ConsultarPorEmailPrincipal(
            String emailUsuario,
            String codigoGrupoEntidade,
            String codigoEntidade)
        {
            return this.ExecucaoTratada<ResponseBaseItem<Servicos.Usuario>>("Consultar usuário por e-mail principal", retorno =>
            {
                Int32 codigoRetorno;

                Modelo.Entidade modeloEntidade = (Int32.Parse(codigoEntidade) == 0)
                    ? null
                    : modeloEntidade = new Modelo.Entidade()
                    {
                        Codigo = Int32.Parse(codigoEntidade),
                        GrupoEntidade = new Modelo.GrupoEntidade
                        {
                            Codigo = Int32.Parse(codigoGrupoEntidade)
                        }
                    };

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                Mapper.CreateMap<Modelo.PaginaMenu, Servicos.PaginaMenu>();
                Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                var retornoNegocio = new Negocio.Usuario().ConsultarPorEmailPrincipal(
                    emailUsuario,
                    modeloEntidade,
                    out codigoRetorno);

                retorno.Item = Mapper.Map<Modelo.Usuario, Servicos.Usuario>(retornoNegocio);
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = codigoRetorno
                };
            });
        }

        /// <summary>
        /// Consultar usuários
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários</returns>
        public ResponseBaseList<Servicos.Usuario> Consultar()
        {
            return this.Consultar(null, null, default(Servicos.Entidade));
        }

        /// <summary>
        /// Retorna os dados de usuário e estabelecimento
        /// </summary>
        /// <param name="codigoGrupoEntidade">Código do grupo entidade</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoIdUsuario">Código do Id do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Modelo Servicos.Usuario</returns>
        public ResponseBaseItem<Servicos.Usuario> Consultar(
            String codigoGrupoEntidade,
            String codigoEntidade,
            String codigoUsuario)
        {
            return this.ExecucaoTratada<ResponseBaseItem<Servicos.Usuario>>("Método responsável pela atualização das permissões de usuário no Sybase", retorno =>
            {
                Int32 codigoRetorno;
                String mensagemRetorno = default(String);

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                var retornoNegocio = new Negocio.Usuario().Consultar(
                    Int32.Parse(codigoGrupoEntidade),
                    Int32.Parse(codigoEntidade),
                    codigoUsuario,
                    out codigoRetorno);

                Servicos.Usuario usuario = Mapper.Map<Modelo.Usuario, Servicos.Usuario>(retornoNegocio);

                if (!object.ReferenceEquals(usuario, null))
                {
                    List<Menu> menuNavegacao = this.ConsultarMenu(codigoUsuario, codigoGrupoEntidade, codigoEntidade, usuario.CodigoIdUsuario.ToString()).Itens.ToList();
                    usuario.Menu = menuNavegacao.ToArray();

                    List<Pagina> paginasPermissoes = this.ConsultarPermissoes(codigoGrupoEntidade, codigoEntidade, codigoUsuario, usuario.CodigoIdUsuario.ToString()).Itens.ToList();
                    if (codigoRetorno == 0 && !object.ReferenceEquals(paginasPermissoes, null))
                        usuario.Paginas = paginasPermissoes.ToArray();
                }
                else
                {
                    mensagemRetorno = "Código de Erro: " + codigoRetorno.ToString();
                }

                retorno.Item = usuario;
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = codigoRetorno,
                    MensagemRetorno = mensagemRetorno
                };
            });
        }

        /// <summary>
        /// Método usado para validar um usuário no Portal Redecard
        /// </summary>
        /// <param name="codigoGrupoEntidade">Código do Grupo da Entidade</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoUsuario">Código do Usuário</param>
        /// <param name="senhaCriptografada">Senha do usuário criptografada</param>
        /// <param name="pvKomerci">Indica se o PV possui Komerci</param>
        /// <returns>Número de retorno</returns>
        public BaseResponse Validar(
            String codigoGrupoEntidade,
            String codigoEntidade,
            String codigoUsuario,
            String senhaCriptografada,
            String pvKomerci)
        {
            return this.ExecucaoTratada<BaseResponse>("Método usado para validar o usuário especificado no Portal Redecard", retorno =>
            {
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = new Negocio.Usuario().Validar(
                        Int32.Parse(codigoGrupoEntidade),
                        Int32.Parse(codigoEntidade),
                        codigoUsuario,
                        senhaCriptografada,
                        Boolean.Parse(pvKomerci))
                };
            });
        }

        /// <summary>
        /// Consulta usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Modelo Usuario</returns>
        public ResponseBaseItem<Servicos.Usuario> ConsultarPorID(
            String codigoIdUsuario)
        {
            return this.ExecucaoTratada<ResponseBaseItem<Servicos.Usuario>>("Consultar usuários", retorno =>
            {
                Int32 codigoRetorno;

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                var retornoNegocio = new Negocio.Usuario().ConsultarPorID(
                    Int32.Parse(codigoIdUsuario),
                    out codigoRetorno);

                retorno.Item = Mapper.Map<Modelo.Usuario, Servicos.Usuario>(retornoNegocio);
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = codigoRetorno
                };
            });
        }

        /// <summary>
        /// Consulta o menu de navegação do usuário no Portal de Serviços
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <returns>Listagem de Menu</returns>
        public ResponseBaseList<Servicos.Menu> ConsultarMenu(
            String codigoUsuario,
            String codigoGrupoEntidade,
            String codigoEntidade,
            String codigoIdUsuario)
        {
            return this.ExecucaoTratada<ResponseBaseList<Servicos.Menu>>("Consultar Menu", retorno =>
            {
                List<Menu> servicosMenu = new List<Menu>();

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                var retornoNegocio = new Negocio.Menu().Consultar(
                    codigoUsuario,
                    Int32.Parse(codigoGrupoEntidade),
                    Int32.Parse(codigoEntidade),
                    Int32.Parse(codigoIdUsuario));

                if (retornoNegocio.Count > 0)
                {
                    // Converter de Modelo.Menu para Servico.Menu
                    foreach (Modelo.Menu item in retornoNegocio)
                    {
                        Servicos.Menu root = new Servicos.Menu()
                        {
                            Codigo = item.Codigo,
                            Texto = item.Texto,
                            Observacoes = item.Observacoes,
                            FlagMenu = item.FlagMenu,
                            ServicoBasico = item.ServicoBasico
                        };
                        item.Paginas.ForEach(delegate (Modelo.PaginaMenu pagina)
                        {
                            root.Paginas.Add(new PaginaMenu()
                            {
                                TextoBotao = pagina.TextoBotao,
                                Url = pagina.Url
                            });
                        });
                        if (item.Items.Count > 0)
                            this.CarregarMenuFilhos(root, item);
                        servicosMenu.Add(root);
                    }
                }
                retorno.Itens = servicosMenu.ToArray();
            });
        }

        /// <summary>
        /// Método de consulta de usuários para reenvio do Welcome
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de usuários para reenvio do Welcome</returns>
        public ResponseBaseList<Servicos.Usuario> ConsultarReenvioWelcome()
        {
            return this.ExecucaoTratada<ResponseBaseList<Servicos.Usuario>>("Método de consulta de usuários para reenvio do Welcome", retorno =>
            {
                Int32 codigoRetorno;

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();

                var retornoNegocio = new Negocio.Usuario().ConsultarReenvioWelcome(
                    out codigoRetorno);

                retorno.Itens = Mapper.Map<List<Modelo.Usuario>, List<Servicos.Usuario>>(retornoNegocio).ToArray();
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = codigoRetorno
                };
            });
        }

        /// <summary>
        /// Cosulta hash de envio de e-mail do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>listagem de UsuarioHash</returns>
        public ResponseBaseList<Servicos.UsuarioHash> ConsultarHash(
            String codigoIdUsuario,
            String status,
            String hash)
        {
            return this.ExecucaoTratada<ResponseBaseList<Servicos.UsuarioHash>>("Método de consulta de hash para envio de e-mail", retorno =>
            {
                Int32 codigoRetorno;

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.UsuarioHash, Servicos.UsuarioHash>();
                Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                var retornoNegocio = new Negocio.Usuario().ConsultarHash(
                    String.IsNullOrWhiteSpace(codigoIdUsuario) ? (Int32?)null : Int32.Parse(codigoIdUsuario),
                    String.IsNullOrWhiteSpace(status) ? (Comum.Enumerador.Status?)null : (Comum.Enumerador.Status)Enum.Parse(typeof(Comum.Enumerador.Status), status),
                    String.IsNullOrWhiteSpace(hash) ? (Guid?)null : Guid.Parse(hash),
                    out codigoRetorno);

                retorno.Itens = Mapper.Map<List<Modelo.UsuarioHash>, List<Servicos.UsuarioHash>>(retornoNegocio).ToArray();
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = codigoRetorno
                };
            });
        }

        /// <summary>
        /// Cosulta hash de envio de e-mail do usuário
        /// </summary>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Listagem de UsuarioHash</returns>
        public ResponseBaseList<Servicos.UsuarioHash> ConsultarHash()
        {
            return this.ExecucaoTratada<ResponseBaseList<Servicos.UsuarioHash>>("Método de consulta de hash para envio de e-mail", retorno =>
            {
                Int32 codigoRetorno;

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.UsuarioHash, Servicos.UsuarioHash>();
                Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                var retornoNegocio = new Negocio.Usuario().ConsultarHash(
                    null,
                    null,
                    null,
                    out codigoRetorno);

                retorno.Itens = Mapper.Map<List<Modelo.UsuarioHash>, List<Servicos.UsuarioHash>>(retornoNegocio).ToArray();
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = codigoRetorno
                };
            });
        }

        /// <summary>
        /// Reinicia o hash de confirmação de e-mail.
        /// Exclui hash anterior (caso exista para o usuário) e insere um novo hash.
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="status">Status</param>
        /// <param name="emailExpira">Indica se o e-mail expira</param>
        /// <param name="codigoRetorno">Código de retorno</param>
        /// <returns>Hash</returns>
        public ResponseBaseItem<Servicos.UsuarioHash> ReiniciarHash(
            String codigoIdUsuario)
        {
            return this.ExecucaoTratada<ResponseBaseItem<Servicos.UsuarioHash>>("Método de reinicio de hash de envio de e-mail", retorno =>
            {
                Int32 codigoRetorno;

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.UsuarioHash, Servicos.UsuarioHash>();
                Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                var retornoNegocio = new Negocio.Usuario().ReiniciarHash(
                    Int32.Parse(codigoIdUsuario),
                    out codigoRetorno);

                retorno.Item = Mapper.Map<Modelo.UsuarioHash, Servicos.UsuarioHash>(retornoNegocio);
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = codigoRetorno
                };
            });
        }

        /// <summary>
        /// Método para inserir hash na base de dados a partir da camada de serviço
        /// </summary>
        /// <param name="codigoIdUsuario">ID do usuário</param>
        /// <param name="status">Status do usuário</param>
        /// <param name="diasExpiracao">Dias corridos para expiração do hash</param>
        /// <param name="dataGeracaoHash">Data customizada em que o hash foi gerado</param>
        /// <returns></returns>
        public ResponseBaseItem<Guid> InserirHash(
            String codigoIdUsuario,
            String status,
            String diasExpiracao,
            String dataGeracaoHash)
        {
            return this.ExecucaoTratada<ResponseBaseItem<Guid>>("Método para inserção de hash de envio de e-mail", retorno =>
            {
                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                retorno.Item = new Negocio.Usuario().InserirHash(
                    Int32.Parse(codigoIdUsuario),
                    (Comum.Enumerador.Status)Enum.Parse(typeof(Comum.Enumerador.Status), status),
                    Double.Parse(diasExpiracao),
                    String.IsNullOrWhiteSpace(dataGeracaoHash) ? (DateTime?)null : DateTime.Parse(dataGeracaoHash));
            });
        }

        #endregion

        #region Métodos privados

        /// <summary>
        /// Converte objeto Modelo.Menu para Servicos.Menu
        /// </summary>
        /// <param name="root">Objeto Servicos.Menu</param>
        /// <param name="item">Objeto Modelo.Menu</param>
        private void CarregarMenuFilhos(Servicos.Menu root, Modelo.Menu item)
        {
            foreach (Modelo.Menu menuItem in item.Items)
            {
                Servicos.Menu subItem = new Servicos.Menu()
                {
                    Codigo = menuItem.Codigo,
                    Texto = menuItem.Texto,
                    Observacoes = menuItem.Observacoes,
                    FlagMenu = menuItem.FlagMenu
                };

                menuItem.Paginas.ForEach(delegate (Modelo.PaginaMenu pagina)
                {
                    subItem.Paginas.Add(new PaginaMenu()
                    {
                        TextoBotao = pagina.TextoBotao,
                        Url = pagina.Url
                    });
                });

                if (menuItem.Items.Count > 0)
                    this.CarregarMenuFilhos(subItem, menuItem);

                root.Items.Add(subItem);
            }
        }

        #endregion

        #region Métodos da Confirmação positiva

        /// <summary>
        /// Incrementa a quantidade de confirmações positivas inválidas em 1 para o usuário
        /// especificado nos paramêtros
        /// </summary>
        /// <param name="codigoIdUsuario">ID do usuário</param>
        /// <returns>Código de retorno</returns>
        public BaseResponse IncrementarQuantidadeConfirmacaoPositiva(
            String codigoIdUsuario)
        {
            return this.ExecucaoTratada<BaseResponse>("Incrementa a quantidade de confirmações positivas inválidas em 1", retorno =>
            {
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = new Negocio.Usuario().IncrementarQuantidadeConfirmacaoPositiva(
                        Int32.Parse(codigoIdUsuario))
                };
            });
        }

        /// <summary>
        /// Atualiza a quantidade de confirmações positivas inválidas para 0 para o usuário
        /// especificado nos paramêtros
        /// </summary>
        /// <param name="codigoIdUsuario">ID do usuário</param>
        /// <returns>Código de retorno</returns>
        public BaseResponse ReiniciarQuantidadeConfirmacaoPositiva(
            String codigoIdUsuario)
        {
            return this.ExecucaoTratada<BaseResponse>("Atualiza a quantidade de confirmações positivas inválidas para 0", retorno =>
            {
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = new Negocio.Usuario().ReiniciarQuantidadeConfirmacaoPositiva(
                        Int32.Parse(codigoIdUsuario))
                };
            });
        }

        /// <summary>
        /// Incrementa a quantidade de senhas inválidas em 1 para o usuário especificado nos paramêtros
        /// </summary>
        /// <param name="codigoIdUsuario">ID do usuário</param>
        /// <returns>Código de retorno/Quantidade senhas inválidas</returns>
        public BaseResponse IncrementarQuantidadeSenhaInvalida(
            String codigoIdUsuario)
        {
            return this.ExecucaoTratada<BaseResponse>("Incrementa a quantidade de senhas inválidas em 1 para o usuário especificado nos paramêtros", retorno =>
            {
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = new Negocio.Usuario().IncrementarQuantidadeSenhaInvalida(
                        Int32.Parse(codigoIdUsuario))
                };
            });
        }

        /// <summary>
        /// Atualiza a quantidade de senhas inválidas para 0 para o usuário
        /// </summary>
        /// <param name="codigoIdUsuario">ID do Usuário</param>
        /// <returns>Código de retorno/Quantidade senhas inválidas</returns>
        public BaseResponse ReiniciarQuantidadeSenhaInvalida(
            String codigoIdUsuario)
        {
            return this.ExecucaoTratada<BaseResponse>("Atualiza a quantidade de senhas inválidas para 0 para o usuário", retorno =>
            {
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = new Negocio.Usuario().ReiniciarQuantidadeSenhaInvalida(
                        Int32.Parse(codigoIdUsuario))
                };
            });
        }

        /// <summary>
        /// Efetua a validação positiva (Dados Obrigatórios) do usuário no Portal Redecard de Serviços. Caso retorne 0 
        /// a confirmação foi ben sucedida, caso contrário, retorna o código do erro
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="cnpjEstabelecimento">CNPJ da entidade</param>
        /// <returns>Código de retorno</returns>
        public BaseResponse ValidarConfirmacaoPositivaObrigatoria(
            String codigoEntidade,
            String cnpjEstabelecimento)
        {
            return this.ExecucaoTratada<BaseResponse>("Efetua a validação positiva (Dados Obrigatórios) do usuário no Portal Redecard de Serviços", retorno =>
            {
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = new Negocio.Usuario().ValidarConfirmacaoPositivaObrigatoria(
                        Int32.Parse(codigoEntidade),
                        Decimal.Parse(cnpjEstabelecimento))
                };
            });
        }

        /// <summary>
        /// Efetua a validação positiva (Dados Variavéis) do usuário no Portal Redecard de Serviços. Caso retorne 0 
        /// a confirmação foi ben sucedida, caso contrário, retorna o código do erro
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="perguntas">Listagem de perguntas</param>
        /// <param name="perguntasIncorretas">Perguntas que tiveram a resposta preenchida incorretamente</param>
        /// <returns>Código de retorno</returns>
        public ResponseBaseList<Servicos.Pergunta> ValidarConfirmacaoPositivaVariavel(
            String codigoEntidade,
            List<Servicos.Pergunta> perguntas)
        {
            return this.ExecucaoTratada<ResponseBaseList<Servicos.Pergunta>>("Efetua a validação positiva (Dados Variavéis) do usuário no Portal Redecard de Serviços", retorno =>
            {
                List<Modelo.Pergunta> perguntasIncorretas = default(List<Modelo.Pergunta>);

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Servicos.Pergunta, Modelo.Pergunta>();
                Mapper.CreateMap<Modelo.Pergunta, Servicos.Pergunta>();

                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = new Negocio.Usuario().ValidarConfirmacaoPositivaVariavel(
                        Int32.Parse(codigoEntidade),
                        (perguntas.Count > 0 ? Mapper.Map<List<Servicos.Pergunta>, List<Modelo.Pergunta>>(perguntas) : null),
                        out perguntasIncorretas)
                };
                retorno.Itens = Mapper.Map<List<Servicos.Pergunta>>(perguntasIncorretas).ToArray();
            });
        }

        /// <summary>
        /// Efetua a validação positiva (Dados Variavéis) do usuário no Portal Redecard de Serviços com múltiplas entidades.
        /// Caso retorne 0 a confirmação foi ben sucedida, caso contrário, retorna o código do erro
        /// </summary>
        /// <param name="codigoEntidade">Códigos da entidades</param>
        /// <param name="perguntas">Listagem de perguntas</param>
        /// <param name="perguntasIncorretas">Dicionário contendo listas de perguntas que tiveram a resposta preenchida incorretamente</param>
        /// <returns>Dicionário com todos os retornos relacionado a cada entidade validada</returns>
        public DicionarioPerguntasResponse ValidarConfirmacaoPositivaVariavel(
            Int32[] codigoEntidades,
            List<Servicos.Pergunta> perguntas)
        {
            return this.ExecucaoTratada<DicionarioPerguntasResponse>("Efetua a validação positiva (Dados Variavéis) do usuário no Portal Redecard de Serviços", retorno =>
            {
                Dictionary<Int32, List<Modelo.Pergunta>> perguntasIncorretas = default(Dictionary<Int32, List<Modelo.Pergunta>>);

                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Servicos.Pergunta, Modelo.Pergunta>();
                Mapper.CreateMap<Modelo.Pergunta, Servicos.Pergunta>();

                retorno.Validado = new Negocio.Usuario().ValidarConfirmacaoPositivaVariavel(
                        codigoEntidades,
                        (perguntas.Count > 0 ? Mapper.Map<List<Servicos.Pergunta>, List<Modelo.Pergunta>>(perguntas) : null),
                        out perguntasIncorretas);
                retorno.Perguntas = Mapper.Map<Dictionary<Int32, List<Modelo.Pergunta>>>(perguntasIncorretas);
            });
        }

        /// <summary>
        /// Verifica se o CPF/CNPJ informado paz parte da relação dos sócios vinculados ao PV informado
        /// </summary>
        /// <param name="codigoEntidade">Codigo da entidade (número do PV)</param>
        /// <param name="cpfCnpjSocio">CPF/CNPJ do sócio a ser verificado</param>
        /// <returns>
        ///     TRUE: CPF/CNPJ consta na relação dos sócios relacionados ao PV
        ///     FALSE: CPF/CNPJ não consta na relação dos sócios relacionados ao PV
        /// </returns>
        public ResponseBaseItem<Boolean> ValidarCpfCnpjSocio(
            String codigoEntidade,
            String cpfCnpjSocio)
        {
            return this.ExecucaoTratada<ResponseBaseItem<Boolean>>("Valida se CPF/CNPJ do sócio está relacionado ao PV informado", retorno =>
            {
                retorno.Item = new Negocio.Usuario().ValidarCpfCnpjSocio(
                        Int32.Parse(codigoEntidade),
                        Int64.Parse(cpfCnpjSocio));
            });
        }

        #endregion

        #region Dupla custodia

        /// <summary>
        /// Aprovação da dupla custódia
        /// </summary>
        /// <param name="codigoUsuario">Códoigo do usuário</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código do grupo da entidade</param>
        /// <param name="nomeResponsavel">Nome do responsável da aprovação</param>
        /// <param name="tipoManutencao">Tipo de manutenção</param>
        /// <param name="nomeSistema">Nome do sistema origem</param>
        /// <returns>Código de retorno</returns>
        public BaseResponse AprovacaoDuplaCustodia(
            String codigoUsuario,
            String codigoEntidade,
            String codigoGrupoEntidade,
            String nomeResponsavel,
            String tipoManutencao,
            String nomeSistema)
        {
            return this.ExecucaoTratada<BaseResponse>("Aprovação Dupla Custódia", retorno =>
            {
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = new Negocio.Usuario().AprovacaoDuplaCustodia(
                        codigoUsuario,
                        Int32.Parse(codigoEntidade),
                        Int32.Parse(codigoGrupoEntidade),
                        nomeResponsavel,
                        tipoManutencao,
                        nomeSistema)
                };
            });
        }

        #endregion

        #region Passo 1 Criação de usuário

        /// <summary>
        /// Verificar Se Usuarios Estao Aguardando Confirmacao
        /// </summary>
        /// <param name="email">e-mail</param>
        /// <returns></returns>
        public ResponseBaseList<Int32> VerificarSeUsuariosEstaoAguardandoConfirmacao(
            String email)
        {
            return this.ExecucaoTratada<ResponseBaseList<Int32>>("Serviço Get VerificarSeUsuariosEstaoAguardandoConfirmacao", retorno =>
            {
                retorno.Itens = new Negocio.Usuario().VerificarSeUsuariosEstaoAguardandoConfirmacao(email);
            });
        }

        /// <summary>
        /// Obter Usuario Aguardando Confirmacao Master
        /// </summary>
        /// <param name="email">e-mail</param>
        /// <returns></returns>
        public ResponseBaseList<Modelo.EntidadeServicoModel> GetUsuarioAguardandoConfirmacaoMaster(
            String email)
        {
            return this.ExecucaoTratada<ResponseBaseList<Modelo.EntidadeServicoModel>>("Serviço Get Usuario Aguardando Confirmacao Master", retorno =>
            {
                retorno.Itens = new Negocio.Usuario().GetUsuarioAguardandoConfirmacaoMaster(email);
            });
        }

        #endregion

        #region Passo 2 criação usuário

        /// <summary>
        /// Cria Usuario Varios Pvs
        /// </summary>
        /// <param name="entidadesSelecionadas"></param>
        /// <param name="emailExpira"></param>
        /// <param name="entidadesPossuemUsuMaster"></param>
        /// <param name="entidadesNPossuemUsuMaster"></param>
        /// <param name="hash"></param>
        /// <param name="codigoRetorno"></param>
        /// <param name="mensagem"></param>
        /// <returns></returns>
        public CriarUsuarioPvsResponse CriarUsuarioVariosPvs(
            Modelo.EntidadeServicoModel[] entidadesSelecionadas,
            String emailExpira)
        {
            return this.ExecucaoTratada<CriarUsuarioPvsResponse>("Criar Usuario para Varios Pvs", retorno =>
            {
                Modelo.EntidadeServicoModel[] entidadesNaoPossuemUsuarioMaster;
                Modelo.EntidadeServicoModel[] entidadesPossuemUsuarioMaster;
                Guid hash = default(Guid);
                Int32 codigoRetorno = default(Int32);
                String mensagem = default(String);

                new Negocio.Usuario().CriarUsuarioVariosPvs(
                    entidadesSelecionadas,
                    Double.Parse(emailExpira),
                    out entidadesPossuemUsuarioMaster,
                    out entidadesNaoPossuemUsuarioMaster,
                    out hash,
                    out codigoRetorno,
                    out mensagem);

                retorno.EntidadesNaoPossuemUsuarioMaster = entidadesNaoPossuemUsuarioMaster;
                retorno.EntidadesPossuemUsuarioMaster = entidadesPossuemUsuarioMaster;
                retorno.Hash = hash;
                retorno.StatusRetorno = new StatusRetorno()
                {
                    CodigoRetorno = codigoRetorno,
                    MensagemRetorno = mensagem
                };
            });
        }

        /// <summary>
        /// Consultar Emails Usu Master
        /// </summary>
        /// <param name="pvs"></param>
        /// <param name="cpf"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public ResponseBaseList<Modelo.EntidadeServicoModel> ConsultarEmailsUsuMaster(
            Int32[] pvs,
            String cpf,
            String email)
        {
            return this.ExecucaoTratada<ResponseBaseList<Modelo.EntidadeServicoModel>>("Consultar Emails Usu Master", retorno =>
            {
                retorno.Itens = new Negocio.Usuario().ConsultarEmailsUsuMaster(
                    pvs,
                    Int64.Parse(cpf),
                    email);
            });
        }

        /// <summary>
        /// Atualiza Status Por Pvs
        /// </summary>
        /// <param name="pvs"></param>
        /// <param name="cpf"></param>
        /// <param name="email"></param>
        /// <param name="status"></param>
        public BaseResponse AtualizarStatusPorPvs(
            Int32[] pvs,
            String cpf,
            String email,
            String status)
        {
            return this.ExecucaoTratada<BaseResponse>("Atualizar Status por Pvs", retorno =>
            {
                new Negocio.Usuario().AtualizarStatusPorPvs(
                    pvs,
                    Int64.Parse(cpf),
                    email,
                    (Comum.Enumerador.Status)Enum.Parse(typeof(Comum.Enumerador.Status), status));
            });
        }

        #endregion

        /// <summary>
        /// Obtem o retorno da comum
        /// </summary>
        /// <returns></returns>
        public ResponseBaseItem<String> ObterRetornoComum()
        {
            return this.ExecucaoTratada<ResponseBaseItem<String>>("ObterRetornoComum", retorno =>
            {
                retorno.Item = Util.ObterRetornoComum();
            });
        }

        /// <summary>
        /// Método responsável pela atualização da senha de um usuário.
        /// </summary>
        /// <param name="usuario">Modelo do usuário</param>
        /// <param name="senha">Nova senha</param>
        /// <param name="pvKomerci">Inidicador se o PV desse usuário é possui Komerci</param>
        /// <param name="senhaTemporaria">Indicador se a senha é temporária</param>
        /// <returns>Código de retorno</returns>
        public ResponseBaseItem<Int32> AtualizarSenha(Usuario usuario, String senha, Boolean pvKomerci, Boolean senhaTemporaria)
        {

            return this.ExecucaoTratada<ResponseBaseItem<Int32>>("Atualiza a senha do usuário na Confirmação Positiva", retorno =>
            {
                // Cria mapeamento, caso já exista utilizará o existente
                Mapper.CreateMap<Usuario, Modelo.Usuario>();
                Mapper.CreateMap<Entidade, Modelo.Entidade>();
                Mapper.CreateMap<GrupoEntidade, Modelo.GrupoEntidade>();
                Mapper.CreateMap<TipoEntidade, Modelo.TipoEntidade>();
                Mapper.CreateMap<Status, Modelo.Status>();

                Modelo.Usuario modeloUsuario = Mapper.Map<Modelo.Usuario>(usuario);
            
                retorno.Item = new Negocio.Usuario().AtualizarSenha(modeloUsuario, senha, pvKomerci, senhaTemporaria);
                
            });

        }

        /// <summary>
        /// Método responsável pela atualização da senha de um usuário.
        /// </summary>
        /// <param name="codigoIdUsuario">Código do Id do usuário</param>
        /// <param name="senha">Nova senha</param>
        /// <param name="pvKomerci">Inidicador se o PV desse usuário é possui Komerci</param>
        /// <param name="pvs">Pvs relacionados ao usuário que a senha será alterada</param>
        /// <param name="senhaTemporaria">Indicador se a senha é temporária</param>
        /// <param name="atualizarStatus"></param>
        /// <returns>Código de retorno</returns>
        public ResponseBaseItem<Int32> AtualizarSenhaUsuarioNovoAcesso(Int32 codigoIdUsuario,
            String senha,
            Boolean pvKomerci,
            int[] pvs,
            Boolean senhaTemporaria,
            Boolean? atualizarStatus = null)
        {
            return this.ExecucaoTratada<ResponseBaseItem<Int32>>("atualização da senha de um usuário no SQL - REST", retorno =>
            {
                int codigoRetorno = new Negocio.Usuario().AtualizarSenha(codigoIdUsuario, senha, pvKomerci, pvs, senhaTemporaria, atualizarStatus);

                retorno = new ResponseBaseItem<int>();
                retorno.Item = codigoRetorno;
                retorno.StatusRetorno = new StatusRetorno() { CodigoRetorno = codigoRetorno };
            });
        }

        /// <summary>
        /// Atualizar e-mail do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código Id do usuário</param>
        /// <param name="email">E-mail do usuário</param>
        /// <param name="diasExpiracaoEmail">Quantidade de dias que um email enviado irá expirar</param>
        /// <param name="dataExpiracaoEmail">Data válida que iniciará a contagem de dias para expiração do e-mail</param>
        /// <param name="hashEmail">Hash de envio de e-mail</param>
        /// <returns>Código de retorno</returns>
        public AlterarEmailResponse AtualizarEmail(Int32 codigoIdUsuario, String email, String diasExpiracaoEmail,
            String dataExpiracaoEmail)
        {
            Guid hashEmail;

            return this.ExecucaoTratada<AlterarEmailResponse>("Atualiza e-mail", retorno =>
            {
                retorno.Item = new Negocio.Usuario().AtualizarEmail(codigoIdUsuario, email,
                    String.IsNullOrWhiteSpace(diasExpiracaoEmail) ? (Int32?)null : Int32.Parse(diasExpiracaoEmail),
                    String.IsNullOrWhiteSpace(dataExpiracaoEmail) ? (DateTime?)null : DateTime.Parse(dataExpiracaoEmail), out hashEmail);

                retorno.Hash = hashEmail;
                
            });     
        }

        /// <summary>
        /// Atualisa o(s) usuário(s) para o status AguardandoConfirRecSenha e cria um hash de e-mail
        /// </summary>
        /// <param name="codigoIdUsuario">Identificador do usuário</param>
        /// <param name="email">Email</param>
        /// <param name="diasExpiracaoEmail">Dias de expiração do e-mail</param>
        /// <param name="dataExpiracaoEmail">Data de inicio</param>
        /// <param name="pvsSelecionados">PVs relacionados ao e-mail do usuário que</param>
        public AtualizarRecuperacaoSenhaResponse AtualizarStatusParaAguardandoConfirRecSenha(Int32 codigoIdUsuario, string email, double? diasExpiracaoEmail, DateTime? dataExpiracaoEmail, int[] pvsSelecionados)
        {
            Guid hashEmail = default(Guid);

            return this.ExecucaoTratada<AtualizarRecuperacaoSenhaResponse>("Método responsável por atualizar o status do usuário e criar uma hash de recuperação de senha - REST", retorno =>
            {
                new Negocio.Usuario().AtualizarStatusParaAguardandoConfirRecSenha(codigoIdUsuario, email, diasExpiracaoEmail, dataExpiracaoEmail, pvsSelecionados, out hashEmail);

                retorno.HashEmail = hashEmail;

            });
        }

    }
}