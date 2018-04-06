/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using AutoMapper;

namespace Redecard.PN.Maximo.Agentes.Mapeadores
{
    /// <summary>
    /// Profile AutoMapper para conversão de Modelos de negócio para Modelos do serviço TGA Terminal do Máximo.
    /// </summary>
    internal class TGATerminalChamada : Profile
    {
        /// <summary>
        /// Nome do Profile
        /// </summary>
        public override string ProfileName { get { return "TGATerminalChamada"; } }

        /// <summary>
        /// Configuração do Profile para mapeamento de Modelo de Negócio para Modelo do Serviço TGA Terminal Máximo
        /// </summary>
        /// <remarks>
        /// Histórico: 18/11/2013 - Criação do método
        /// </remarks>
        protected override void Configure()
        {
            this.AllowNullCollections = true;
            this.AllowNullDestinationValues = true;

            //Propriedades dos modelos de origem (Modelo.Terminal)
            //utilizam a convenção de nome "Pascal Case"
            this.SourceMemberNamingConvention = new PascalCaseNamingConvention();

            //Propriedades dos modelos do destino (Agentes.TGATerminal):
            //1. Seguem a convenção de nome "Lower Underscore" (minúsculas, com separador underscore)
            this.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();

            //Mapeamento entre classes dos dois domínios            
            this.MapearClasses();
        }

        /// <summary>
        /// Mapeamento entre as classes
        /// </summary>
        /// <remarks>
        /// Histórico: 18/11/2013 - Criação do método
        /// </remarks>
        private void MapearClasses()
        {
            this.CreateMap<Modelo.Terminal.Autenticacao, Agentes.TGATerminal.t_autenticacao>();
            this.CreateMap<Modelo.Terminal.ChaveValor, Agentes.TGATerminal.t_chave_valor>();
            this.CreateMap<Modelo.Terminal.Chip, Agentes.TGATerminal.t_chip>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Modelo.Terminal.FiltroTerminal, Agentes.TGATerminal.t_filtro_terminal>()
                .ForMember(d => d.situacaoSpecified, s => s.MapFrom(obj => obj.Situacao.HasValue));

            this.CreateMap<Modelo.Terminal.Integrador, Agentes.TGATerminal.t_integrador>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Modelo.Terminal.OSAtendimentoBase, Agentes.TGATerminal.t_os_atendimento_base>()
                .ForMember(d => d.data_atendimentoSpecified, s => s.MapFrom(obj => obj.DataAtendimento.HasValue));

            this.CreateMap<Modelo.Terminal.Terminal, Agentes.TGATerminal.t_terminal>()
                .Include<Modelo.Terminal.TerminalDetalhado, Agentes.TGATerminal.t_terminal_detalhado>();

            this.CreateMap<Modelo.Terminal.TerminalBase, Agentes.TGATerminal.t_terminal_base>();
            this.CreateMap<Modelo.Terminal.TerminalConsulta, Agentes.TGATerminal.t_terminal_consulta>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Modelo.Terminal.TerminalDetalhado, Agentes.TGATerminal.t_terminal_detalhado>()                
                .ForMember(d => d.data_atualizacao_tgSpecified, s => s.MapFrom(obj => obj.DataAtualizacaoTg.HasValue))
                .ForMember(d => d.data_compraSpecified, s => s.MapFrom(obj => obj.DataCompra.HasValue))
                .ForMember(d => d.data_instalacaoSpecified, s => s.MapFrom(obj => obj.DataInstalacao.HasValue))
                .ForMember(d => d.data_recebimentoSpecified, s => s.MapFrom(obj => obj.DataRecebimento.HasValue))
                .ForMember(d => d.proprietarioSpecified, s => s.MapFrom(obj => obj.Proprietario.HasValue))
                .ForMember(d => d.tipo_conexaoSpecified, s => s.MapFrom(obj => obj.TipoConexao.HasValue));

            this.CreateMap<Modelo.Terminal.TerminalStatus, Agentes.TGATerminal.t_terminal_status>()
                .Include<Modelo.Terminal.Terminal, Agentes.TGATerminal.t_terminal>();

            this.CreateMap<Modelo.Terminal.TipoTerminalProprietario, Agentes.TGATerminal.d_terminal_proprietario>();
            this.CreateMap<Modelo.Terminal.TipoTerminalStatus, Agentes.TGATerminal.d_terminal_status>();
            this.CreateMap<Modelo.Terminal.TipoTerminalTipoConexao, Agentes.TGATerminal.d_terminal_tipo_conexao>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Modelo.Terminal.VendaDigitada, Agentes.TGATerminal.t_venda_digitada>()
                .Include<Modelo.Terminal.VendaDigitadaTerminal, Agentes.TGATerminal.t_venda_digitada_terminal>()
                .ForMember(d => d.habilitada_receptivoSpecified, s => s.MapFrom(obj => obj.HabilitadaReceptivo.HasValue));

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Modelo.Terminal.VendaDigitadaTerminal, Agentes.TGATerminal.t_venda_digitada_terminal>()
                .ForMember(d => d.cvc2_obrigatorioSpecified, s => s.MapFrom(obj => obj.Cvc2Obrigatorio.HasValue));
        }
    }
}
