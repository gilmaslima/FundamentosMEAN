/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using AutoMapper;

namespace Redecard.PN.Maximo.Agentes.Mapeadores
{
    /// <summary>
    /// Profile AutoMapper para conversão de Modelos do serviço TGA Terminal do Máximo para Modelos de negócio.
    /// </summary>
    internal class TGATerminalRetorno : Profile
    {
        /// <summary>
        /// Nome do Profile
        /// </summary>
        public override string ProfileName { get { return "TGATerminalRetorno"; } }

        /// <summary>
        /// Configuração do Profile para mapeamento de Modelo do Serviço TGA Terminal para Modelo de Negócio
        /// </summary>
        /// <remarks>
        /// Histórico: 18/11/2013 - Criação do método
        /// </remarks>
        protected override void Configure()
        {
            this.AllowNullCollections = true;
            this.AllowNullDestinationValues = true;

            //Propriedades dos modelos de origem (Agentes.TGATerminal):
            //1. Seguem a convenção de nome "Lower Underscore" (minúsculas, com separador underscore)
            this.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();

            //Propriedades dos modelos do destino (Modelo.Terminal)            
            //utilizam a convenção de nome "Pascal Case"
            this.DestinationMemberNamingConvention = new PascalCaseNamingConvention();

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
            this.CreateMap<Agentes.TGATerminal.t_autenticacao, Modelo.Terminal.Autenticacao>();
            this.CreateMap<Agentes.TGATerminal.t_chave_valor, Modelo.Terminal.ChaveValor>();
            this.CreateMap<Agentes.TGATerminal.t_chip, Modelo.Terminal.Chip>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Agentes.TGATerminal.t_filtro_terminal, Modelo.Terminal.FiltroTerminal>()
                .ForMember(d => d.Situacao, s => s.Condition(obj => obj.situacaoSpecified));

            this.CreateMap<Agentes.TGATerminal.t_integrador, Modelo.Terminal.Integrador>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Agentes.TGATerminal.t_os_atendimento_base, Modelo.Terminal.OSAtendimentoBase>()
                .ForMember(d => d.DataAtendimento, s => s.Condition(obj => obj.data_atendimentoSpecified));

            this.CreateMap<Agentes.TGATerminal.t_terminal, Modelo.Terminal.Terminal>()
                .Include<Agentes.TGATerminal.t_terminal_detalhado, Modelo.Terminal.TerminalDetalhado>();

            this.CreateMap<Agentes.TGATerminal.t_terminal_base, Modelo.Terminal.TerminalBase>();
            this.CreateMap<Agentes.TGATerminal.t_terminal_consulta, Modelo.Terminal.TerminalConsulta>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Agentes.TGATerminal.t_terminal_detalhado, Modelo.Terminal.TerminalDetalhado>()
                .ForMember(d => d.DataAtualizacaoTg, s => s.Condition(obj => obj.data_atualizacao_tgSpecified))
                .ForMember(d => d.DataCompra, s => s.Condition(obj => obj.data_compraSpecified))
                .ForMember(d => d.DataInstalacao, s => s.Condition(obj => obj.data_instalacaoSpecified))
                .ForMember(d => d.DataRecebimento, s => s.Condition(obj => obj.data_recebimentoSpecified))
                .ForMember(d => d.Proprietario, s => s.Condition(obj => obj.proprietarioSpecified))
                .ForMember(d => d.TipoConexao, s => s.Condition(obj => obj.tipo_conexaoSpecified));

            this.CreateMap<Agentes.TGATerminal.t_terminal_status, Modelo.Terminal.TerminalStatus>()
                .Include<Agentes.TGATerminal.t_terminal, Modelo.Terminal.Terminal>();

            this.CreateMap<Agentes.TGATerminal.d_terminal_proprietario, Modelo.Terminal.TipoTerminalProprietario>();
            this.CreateMap<Agentes.TGATerminal.d_terminal_status, Modelo.Terminal.TipoTerminalStatus>();
            this.CreateMap<Agentes.TGATerminal.d_terminal_tipo_conexao, Modelo.Terminal.TipoTerminalTipoConexao>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Agentes.TGATerminal.t_venda_digitada, Modelo.Terminal.VendaDigitada>()
                .Include<Agentes.TGATerminal.t_venda_digitada_terminal, Modelo.Terminal.VendaDigitadaTerminal>()
                .ForMember(d => d.HabilitadaReceptivo, s => s.Condition(obj => obj.habilitada_receptivoSpecified));

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Agentes.TGATerminal.t_venda_digitada_terminal, Modelo.Terminal.VendaDigitadaTerminal>()
                .ForMember(d => d.Cvc2Obrigatorio, s => s.Condition(obj => obj.cvc2_obrigatorioSpecified));
        }
    }
}
