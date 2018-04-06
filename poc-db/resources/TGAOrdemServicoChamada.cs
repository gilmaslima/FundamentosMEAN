/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using AutoMapper;

namespace Redecard.PN.Maximo.Agentes.Mapeadores
{
    /// <summary>
    /// Profile AutoMapper para conversão de Modelos de negócio para Modelos do serviço TGA Ordem Serviço do Máximo.
    /// </summary>
    internal class TGAOrdemServicoChamada : Profile
    {
        /// <summary>
        /// Nome do Profile
        /// </summary>
        public override string ProfileName { get { return "TGAOrdemServicoChamada"; } }

        /// <summary>
        /// Configuração do Profile para mapeamento de Modelo de Negócio para Modelo do Serviço TGA Ordem Serviço Máximo
        /// </summary>
        /// <remarks>
        /// Histórico: 18/11/2013 - Criação do método
        /// </remarks>
        protected override void Configure()
        {
            this.AllowNullCollections = true;
            this.AllowNullDestinationValues = true;

            //Propriedades dos modelos de origem (Modelo.OrdemServico)
            //utilizam a convenção de nome "Pascal Case"
            this.SourceMemberNamingConvention = new PascalCaseNamingConvention();

            //Propriedades dos modelos do destino (Agentes.TGAOrdemServico):
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
            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Modelo.OrdemServico.Aluguel, Agentes.TGAOrdemServico.t_aluguel>()
                .ForMember(d => d.data_inicio_cobrancaSpecified, s => s.MapFrom(obj => obj.DataInicioCobranca.HasValue));
 
            this.CreateMap<Modelo.OrdemServico.Autenticacao, Agentes.TGAOrdemServico.t_autenticacao>();
            this.CreateMap<Modelo.OrdemServico.Contato, Agentes.TGAOrdemServico.t_contato>();
            this.CreateMap<Modelo.OrdemServico.Endereco, Agentes.TGAOrdemServico.t_endereco>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Modelo.OrdemServico.EventoOS, Agentes.TGAOrdemServico.t_evento_os>()
                .ForMember(d => d.inicioSpecified, s => s.MapFrom(obj => obj.Inicio.HasValue))
                .ForMember(d => d.terminoSpecified, s => s.MapFrom(obj => obj.Termino.HasValue));    

            this.CreateMap<Modelo.OrdemServico.FiltroOS, Agentes.TGAOrdemServico.t_filtro_os>();
            this.CreateMap<Modelo.OrdemServico.Horario, Agentes.TGAOrdemServico.t_horario>();
            this.CreateMap<Modelo.OrdemServico.MesValor, Agentes.TGAOrdemServico.t_mes_valor>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Modelo.OrdemServico.OS, Agentes.TGAOrdemServico.t_os>()
                .Include<Modelo.OrdemServico.OSEstendida, Agentes.TGAOrdemServico.t_os_estendida>()
                .Include<Modelo.OrdemServico.OSDetalhada, Agentes.TGAOrdemServico.t_os_detalhada>()
                .ForMember(d => d.data_atendimentoSpecified, s => s.MapFrom(obj => obj.DataAtendimento.HasValue))
                .ForMember(d => d.data_programadaSpecified, s => s.MapFrom(obj => obj.DataProgramada.HasValue));

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Modelo.OrdemServico.OSCriacao, Agentes.TGAOrdemServico.t_os_criacao>()
                .ForMember(d => d.data_agendadaSpecified, s => s.MapFrom(obj => obj.DataAgendada.HasValue));

            this.CreateMap<Modelo.OrdemServico.OSDetalhada, Agentes.TGAOrdemServico.t_os_detalhada>();
            this.CreateMap<Modelo.OrdemServico.OSEstendida, Agentes.TGAOrdemServico.t_os_estendida>()
                .Include<Modelo.OrdemServico.OSDetalhada, Agentes.TGAOrdemServico.t_os_detalhada>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Modelo.OrdemServico.OSTerminal, Agentes.TGAOrdemServico.t_os_terminal>()
                .ForMember(d => d.acaoSpecified, s => s.MapFrom(obj => obj.Acao.HasValue))
                .ForMember(d => d.lacradoSpecified, s => s.MapFrom(obj => obj.Lacrado.HasValue))
                .ForMember(d => d.quantidade_checkoutSpecified, s => s.MapFrom(obj => obj.QuantidadeCheckout.HasValue));                

            this.CreateMap<Modelo.OrdemServico.Periodo, Agentes.TGAOrdemServico.t_periodo>();
            this.CreateMap<Modelo.OrdemServico.PontoVenda, Agentes.TGAOrdemServico.t_ponto_venda>();
            this.CreateMap<Modelo.OrdemServico.TipoAcaoTerminal, Agentes.TGAOrdemServico.t_acao_terminal>();
            this.CreateMap<Modelo.OrdemServico.TipoClassificacao, Agentes.TGAOrdemServico.d_classificacao>();
            this.CreateMap<Modelo.OrdemServico.TipoDia, Agentes.TGAOrdemServico.d_dia>();
            this.CreateMap<Modelo.OrdemServico.TipoMeses, Agentes.TGAOrdemServico.d_meses>();
            this.CreateMap<Modelo.OrdemServico.TipoOrigem, Agentes.TGAOrdemServico.d_origem>();
            this.CreateMap<Modelo.OrdemServico.TipoOSSituacao, Agentes.TGAOrdemServico.d_os_situacao>();
            this.CreateMap<Modelo.OrdemServico.TipoPrioridade, Agentes.TGAOrdemServico.d_prioridade>();
            this.CreateMap<Modelo.OrdemServico.TipoUf, Agentes.TGAOrdemServico.d_uf>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Modelo.OrdemServico.VendaDigitada, Agentes.TGAOrdemServico.t_venda_digitada>()
                .Include<Modelo.OrdemServico.VendaDigitadaTerminal, Agentes.TGAOrdemServico.t_venda_digitada_terminal>()
                .ForMember(d => d.habilitada_receptivoSpecified, s => s.MapFrom(obj => obj.HabilitadaReceptivo.HasValue));

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Modelo.OrdemServico.VendaDigitadaTerminal, Agentes.TGAOrdemServico.t_venda_digitada_terminal>()
                .ForMember(d => d.cvc2_obrigatorioSpecified, s => s.MapFrom(obj => obj.Cvc2Obrigatorio.HasValue));                
        }
    }
}
