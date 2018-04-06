/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using AutoMapper;

namespace Redecard.PN.Maximo.Agentes.Mapeadores
{
    /// <summary>
    /// Profile AutoMapper para conversão de Modelos do serviço TGA OrdemServico do Máximo para Modelos de negócio.
    /// </summary>
    internal class TGAOrdemServicoRetorno : Profile
    {
        /// <summary>
        /// Nome do Profile
        /// </summary>
        public override string ProfileName { get { return "TGAOrdemServicoRetorno"; } }

        /// <summary>
        /// Configuração do Profile para mapeamento de Modelo do Serviço TGA Ordem Serviço para Modelo de Negócio
        /// </summary>
        /// <remarks>
        /// Histórico: 18/11/2013 - Criação do método
        /// </remarks>
        protected override void Configure()
        {
            this.AllowNullCollections = true;
            this.AllowNullDestinationValues = true;

            //Propriedades dos modelos de origem (Agentes.TGAOrdemServico):
            //1. Seguem a convenção de nome "Lower Underscore" (minúsculas, com separador underscore)
            this.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();

            //Propriedades dos modelos do destino (Modelo.OrdemServico)            
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
            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Agentes.TGAOrdemServico.t_aluguel, Modelo.OrdemServico.Aluguel>()
                .ForMember(d => d.DataInicioCobranca, s => s.Condition(obj => obj.data_inicio_cobrancaSpecified));

            this.CreateMap<Agentes.TGAOrdemServico.t_autenticacao, Modelo.OrdemServico.Autenticacao>();
            this.CreateMap<Agentes.TGAOrdemServico.t_contato, Modelo.OrdemServico.Contato>();
            this.CreateMap<Agentes.TGAOrdemServico.t_endereco, Modelo.OrdemServico.Endereco>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Agentes.TGAOrdemServico.t_evento_os, Modelo.OrdemServico.EventoOS>()
                .ForMember(d => d.Inicio, s => s.Condition(obj => obj.inicioSpecified))
                .ForMember(d => d.Termino, s => s.Condition(obj => obj.terminoSpecified));    

            this.CreateMap<Agentes.TGAOrdemServico.t_filtro_os, Modelo.OrdemServico.FiltroOS>();
            this.CreateMap<Agentes.TGAOrdemServico.t_horario, Modelo.OrdemServico.Horario>();
            this.CreateMap<Agentes.TGAOrdemServico.t_mes_valor, Modelo.OrdemServico.MesValor>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Agentes.TGAOrdemServico.t_os, Modelo.OrdemServico.OS>()
                .Include<Agentes.TGAOrdemServico.t_os_estendida, Modelo.OrdemServico.OSEstendida>()
                .Include<Agentes.TGAOrdemServico.t_os_detalhada, Modelo.OrdemServico.OSDetalhada>()
                .ForMember(d => d.DataAtendimento, s => s.Condition(obj => obj.data_atendimentoSpecified))
                .ForMember(d => d.DataProgramada, s => s.Condition(obj => obj.data_programadaSpecified));

            this.CreateMap<Agentes.TGAOrdemServico.t_os_criacao, Modelo.OrdemServico.OSCriacao>();
            this.CreateMap<Agentes.TGAOrdemServico.t_os_detalhada, Modelo.OrdemServico.OSDetalhada>();

            this.CreateMap<Agentes.TGAOrdemServico.t_os_estendida, Modelo.OrdemServico.OSEstendida>()
                .Include<Agentes.TGAOrdemServico.t_os_detalhada, Modelo.OrdemServico.OSDetalhada>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Agentes.TGAOrdemServico.t_os_terminal, Modelo.OrdemServico.OSTerminal>()
                .ForMember(d => d.Acao, s => s.Condition(obj => obj.acaoSpecified))
                .ForMember(d => d.Lacrado, s => s.Condition(obj => obj.lacradoSpecified))
                .ForMember(d => d.QuantidadeCheckout, s => s.Condition(obj => obj.quantidade_checkoutSpecified));

            this.CreateMap<Agentes.TGAOrdemServico.t_periodo, Modelo.OrdemServico.Periodo>();
            this.CreateMap<Agentes.TGAOrdemServico.t_ponto_venda, Modelo.OrdemServico.PontoVenda>();
            this.CreateMap<Agentes.TGAOrdemServico.t_acao_terminal, Modelo.OrdemServico.TipoAcaoTerminal>();
            this.CreateMap<Agentes.TGAOrdemServico.d_classificacao, Modelo.OrdemServico.TipoClassificacao>();
            this.CreateMap<Agentes.TGAOrdemServico.d_dia, Modelo.OrdemServico.TipoDia>();
            this.CreateMap<Agentes.TGAOrdemServico.d_meses, Modelo.OrdemServico.TipoMeses>();
            this.CreateMap<Agentes.TGAOrdemServico.d_origem, Modelo.OrdemServico.TipoOrigem>();
            this.CreateMap<Agentes.TGAOrdemServico.d_os_situacao, Modelo.OrdemServico.TipoOSSituacao>();
            this.CreateMap<Agentes.TGAOrdemServico.d_prioridade, Modelo.OrdemServico.TipoPrioridade>();
            this.CreateMap<Agentes.TGAOrdemServico.d_uf, Modelo.OrdemServico.TipoUf>();

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Agentes.TGAOrdemServico.t_venda_digitada, Modelo.OrdemServico.VendaDigitada>()
                .Include<Agentes.TGAOrdemServico.t_venda_digitada_terminal, Modelo.OrdemServico.VendaDigitadaTerminal>()
                .ForMember(d => d.HabilitadaReceptivo, s => s.Condition(obj => obj.habilitada_receptivoSpecified));

            //Tratamento manual dos campos "Specified" gerados na importação do WCF
            this.CreateMap<Agentes.TGAOrdemServico.t_venda_digitada_terminal, Modelo.OrdemServico.VendaDigitadaTerminal>()
                .ForMember(d => d.Cvc2Obrigatorio, s => s.Condition(obj => obj.cvc2_obrigatorioSpecified));
        }
    }
}
