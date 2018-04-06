using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.Collections.Generic;
using System.Linq;

namespace Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento
{
    public partial class RecuperacaoProposta : UserControlCredenciamentoBase
    {

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {

                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Carregar dados Iniciais da tela
        /// </summary>
        public void CarregarDadosIniciais()
        {
            var propostas = new List<Modelo.PropostaPendente>();
            List<Modelo.PropostaPendente> propostasIncompletas = Servicos.ServicosWF.RecuperarPropostasIncompletas(Credenciamento.Proposta.CodigoTipoPessoa, Credenciamento.Proposta.NumeroCnpjCpf, null);
            List<Modelo.PropostaPendente> propostasCanceladasOuAtivas = Servicos.ServicosGE.RecuperarPropostasCanceladasOuAtivas(Credenciamento.Proposta.CodigoTipoPessoa, Credenciamento.Proposta.NumeroCnpjCpf);

            ExibirMensagemProposta(propostasIncompletas, propostasCanceladasOuAtivas);

            //Alimenta propostas com dados retornados
            propostas.AddRange(propostasIncompletas);
            propostas.AddRange(propostasCanceladasOuAtivas);

            if (propostas.Count == 0)
            {
                ConsultaProximaSequencia();
                CarregarDadosDomicilioBancarioCreditoPorPontoVenda(new Modelo.PropostaPendente { NroEstabelecimento = (Int32)Credenciamento.Proposta.NumeroMatriz });
                CarregarDadosPorPontoVenda(new Modelo.PropostaPendente { NroEstabelecimento = (Int32)Credenciamento.Proposta.NumeroMatriz });
                ListaProprietariosPontoVenda(new Modelo.PropostaPendente { NroEstabelecimento = (Int32)Credenciamento.Proposta.NumeroMatriz });
            }

            gvPropostas.DataSource = propostas;
            gvPropostas.DataBind();
        }

        /// <summary>
        /// Exibe regra de mensagem sobre PVs Ativos, Cancelados e Propostas pendentes.
        /// </summary>
        /// <param name="lstPropPendente">Lista de propostas pendentes</param>
        /// <param name="propostasCanceladasOuAtivas">Propostas cancelas ou ativas</param>
        private void ExibirMensagemProposta(List<Modelo.PropostaPendente> lstPropPendente, List<Modelo.PropostaPendente> propostasCanceladasOuAtivas)
        {
            var lstCanceladas = propostasCanceladasOuAtivas.Where(obj => obj.Categoria == 'X' || obj.Categoria == 'E');
            var lstAtivas = propostasCanceladasOuAtivas.Where(obj => obj.Categoria != 'X' && obj.Categoria != 'E');

            bool bPropPendente = false;
            bool bPVAtivo = false;
            bool bPVCancelado = false;

            if (lstPropPendente != null && lstPropPendente.Count() > 0)
                bPropPendente = true;

            if (lstAtivas != null && lstAtivas.Count() > 0)
                bPVAtivo = true;

            if (lstCanceladas != null && lstCanceladas.Count() > 0)
                bPVCancelado = true;

            //Controle de mensagem
            if (bPropPendente)
            {
                if (!bPVAtivo && !bPVCancelado)
                    lblMensagemPropostas.Text = "Já existe uma proposta em andamento para este CNPJ/CPF.";

                if (bPVAtivo)
                    lblMensagemPropostas.Text = "Existe um PV Ativo e uma proposta pendente para este CNPJ/CPF. Efetue a continuidade da duplicação do PV selecionando a proposta pendente.";

                if (bPVCancelado)
                    lblMensagemPropostas.Text = "Existe um PV Cancelado e uma proposta pendente para este CNPJ/CPF. Efetue a continuidade recredenciamento selecionando a proposta pendente.";
            }
            else
            {
                if (bPVAtivo)
                    lblMensagemPropostas.Text = "Existe um PV Ativo para este CNPJ/CPF. Verifique o procedimento para realizar a duplicação do PV.";

                if (bPVCancelado)
                    lblMensagemPropostas.Text = "Existe um PV Cancelado para este CNPJ/CPF. Verifique procedimento para realizar o recredenciamento.";
            }

        }

        /// <summary>
        /// Data Bound das linhas da grid
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void gvPropostas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ((Label)e.Row.FindControl("lblEstabelecimento")).Text = ((Modelo.PropostaPendente)e.Row.DataItem).NroEstabelecimento.ToString();
                    Label cpf_cnpj = ((Label)e.Row.FindControl("lblCNPJ"));
                    if (((Modelo.PropostaPendente)e.Row.DataItem).TipoPessoa == 'J')
                        cpf_cnpj.Text = String.Format(@"{0:00\.000\.000\/0000\-00}", ((Modelo.PropostaPendente)e.Row.DataItem).CNPJ);
                    else
                        cpf_cnpj.Text = String.Format(@"{0:000\.000\.000\-00}", ((Modelo.PropostaPendente)e.Row.DataItem).CNPJ);

                    ((Label)e.Row.FindControl("lblRazaoSocial")).Text = ((Modelo.PropostaPendente)e.Row.DataItem).RazaoSocial;
                    ((Label)e.Row.FindControl("lblRamo")).Text = ((Modelo.PropostaPendente)e.Row.DataItem).Ramo;
                    Label tipoEstabelecimento = (Label)e.Row.FindControl("lblTipoEstabelecimento");
                    if (((Modelo.PropostaPendente)e.Row.DataItem).TipoEstabelecimento == 0)
                        tipoEstabelecimento.Text = "Autônomo";
                    else if (((Modelo.PropostaPendente)e.Row.DataItem).TipoEstabelecimento == 1)
                        tipoEstabelecimento.Text = "Filial";
                    else
                        tipoEstabelecimento.Text = "Matriz";

                    ((Label)e.Row.FindControl("lblCategoria")).Text = ((Modelo.PropostaPendente)e.Row.DataItem).Categoria.ToString();
                    ((Label)e.Row.FindControl("lblEndereco")).Text = ((Modelo.PropostaPendente)e.Row.DataItem).EnderecoComercial;

                    Image status = (Image)e.Row.FindControl("imgSituacao");
                    if (((Modelo.PropostaPendente)e.Row.DataItem).StatusProposta == 1)
                        status.ImageUrl = "../_layouts/Redecard.Comum/Images/sinal_amarelo.png";
                    else if (((Modelo.PropostaPendente)e.Row.DataItem).Categoria == 'X' || ((Modelo.PropostaPendente)e.Row.DataItem).Categoria == 'E')
                        status.ImageUrl = "../_layouts/Redecard.Comum/Images/sinal_vermelho.png";
                    else
                        status.ImageUrl = "../_layouts/Redecard.Comum/Images/sinal_verde.png";

                    ((HiddenField)e.Row.FindControl("hiddenNumSeq")).Value = ((Modelo.PropostaPendente)e.Row.DataItem).NumSequencia.ToString();
                    ((HiddenField)e.Row.FindControl("hiddenStatus")).Value = ((Modelo.PropostaPendente)e.Row.DataItem).StatusProposta.ToString();
                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Propostas em Andamento", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }

        }

        /// <summary>
        /// Recupera a linha selecionada da grid
        /// </summary>
        /// <param name="existeSelecao">Existe campo selecionado</param>
        /// <returns>Retorna Proposta Pendente</returns>
        private Modelo.PropostaPendente GetSelectedRow(out Boolean existeSelecao)
        {
            existeSelecao = false;
            Modelo.PropostaPendente proposta = new Modelo.PropostaPendente();

            GridViewRowCollection rows = gvPropostas.Rows;

            for (int i = 0; i < rows.Count; i++)
            {
                RadioButton rb = (RadioButton)rows[i].FindControl("rbSelect");
                if (rb.Checked)
                {
                    existeSelecao = true;
                    Int64 cpf_cnpj = 0;
                    Int64.TryParse(((Label)rows[i].FindControl("lblCNPJ")).Text.Replace(".", "").Replace("/", "").Replace("-", ""), out cpf_cnpj);

                    proposta.TipoPessoa = ((Label)rows[i].FindControl("lblCNPJ")).Text.Contains("/") ? 'J' : 'F';
                    proposta.CNPJ = cpf_cnpj;
                    proposta.NroEstabelecimento = ((Label)rows[i].FindControl("lblEstabelecimento")).Text.ToInt32();
                    proposta.NumSequencia = ((HiddenField)rows[i].FindControl("hiddenNumSeq")).Value.ToInt32Null();
                    proposta.StatusProposta = ((HiddenField)rows[i].FindControl("hiddenStatus")).Value.ToInt32Null();
                }
            }

            return proposta;
        }

        /// <summary>
        /// Evento de botão Confirmar
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Recuperação de Proposta - Continuar"))
            {
                try
                {
                    Boolean existeSelecao;
                    Modelo.PropostaPendente propostaSelecionada = GetSelectedRow(out existeSelecao);

                    if (existeSelecao)
                    {
                        if (propostaSelecionada.StatusProposta == 1)
                        {
                            Credenciamento.Proposta.CodigoTipoMovimento = 'A';

                            CarregarPropostaEmAndamento(propostaSelecionada);
                            CarregarDadosEnderecos(propostaSelecionada);
                            CarregarDadosTecnologia(propostaSelecionada);
                            CarregarDadosDomicilioBancario(propostaSelecionada);
                            CarregarDadosProprietarios(propostaSelecionada);
                            CarregarDadosServicos(propostaSelecionada);

                        }
                        else
                        {
                            ConsultaProximaSequencia();

                            if (propostaSelecionada.StatusProposta == 2)
                            {
                                Credenciamento.Proposta.CodigoTipoMovimento = 'D';
                                GravaInfoTipoEstabCredenciamento(propostaSelecionada);
                                //CarregarDadosRecredenciamento(propostaSelecionada); vem por último para atualizar o PDV selecionado que vem 0 no método GravaInfoTipoEstabCredenciamento(propostaSelecionada);
                                CarregarDadosRecredenciamento(propostaSelecionada);
                            }
                            else if (propostaSelecionada.StatusProposta == 3)
                            {
                                ListaProprietariosPontoVenda(propostaSelecionada);
                                CarregarDadosDomicilioBancarioCreditoPorPontoVenda(propostaSelecionada);
                                Credenciamento.Proposta.CodigoTipoMovimento = 'U';
                                GravaInfoTipoEstabCredenciamento(propostaSelecionada);
                                CarregarDadosPorPontoVenda(propostaSelecionada);
                            }
                        }

                        Continuar(sender, e);
                    }
                    else
                        base.ExibirPainelExcecao("Selecione uma proposta pendente", "300");

                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Credenciamento", ex);
                    SharePointUlsLog.LogErro(ex);
                    ExibirPainelExcecao(ex.Fonte, ex.Codigo);
                }
                catch (Exception ex)
                {
                    ex.HandleException(this);
                }
            }
        }

        /// <summary>
        /// Carrega lista de serviços
        /// </summary>
        /// <param name="proposta">Proposta Pendente</param>
        private void CarregarDadosServicos(Modelo.PropostaPendente proposta)
        {
            Char tipoPessoa = (Char)proposta.TipoPessoa;
            Int64 cpfCnpj = (Int64)proposta.CNPJ;
            Int32 numeroSequencia = (Int32)proposta.NumSequencia;

            var servicos = Servicos.ServicosWF.ConsultaServicos(tipoPessoa, cpfCnpj, numeroSequencia);

            Credenciamento.Servicos = new List<Modelo.Servico>();

            foreach (WFServicos.ConsultaServico servico in servicos)
            {
                Credenciamento.Servicos.Add(new Modelo.Servico
                {
                    CodigoServico = servico.CodServico,
                    CodigoRegimeServico = servico.CodRegimeServico,
                    QuantidadeMinimaConsulta = (Int32)servico.QtdeMinimaConsulta,
                    ValorFranquia = servico.ValorFranquia
                });
            }
        }

        /// <summary>
        /// Grava Informação do Tipo de Estabelecimento
        /// </summary>
        /// <param name="proposta">Proposta Pendente</param>
        private void GravaInfoTipoEstabCredenciamento(Modelo.PropostaPendente proposta)
        {
            Int64 numCNPJ = (Int64)proposta.CNPJ;
            var dadosPv = Servicos.ServicosGE.ConsultaTipoEstabelecimento(numCNPJ, null);

            Credenciamento.Proposta.CodigoTipoEstabelecimento = dadosPv.CodTipoEstabelecimento;
            Credenciamento.Proposta.NumeroPontoDeVenda = 0;
            Credenciamento.Proposta.NumeroMatriz = dadosPv.NumPdvMatriz;
        }

        public event EventHandler Continuar;

        /// <summary>
        /// Carrega dados de uma proposta na sessão
        /// </summary>
        /// <param name="proposta">Proposta Pendente</param>
        private void CarregarPropostaEmAndamento(Modelo.PropostaPendente proposta)
        {
            Char tipoPessoa = (Char)proposta.TipoPessoa;
            Int64 cpfCnpj = (Int64)proposta.CNPJ;
            Int32 numeroSequencia = (Int32)proposta.NumSequencia;
            var dados = Servicos.ServicosWF.CarregarDadosPropostaEmAndamento(tipoPessoa, cpfCnpj, numeroSequencia);

            Credenciamento.Proposta.CodigoTipoPessoa = (Char)dados.CodTipoPessoa;
            Credenciamento.Proposta.NumeroCnpjCpf = (Int64)dados.NumCnpjCpf;
            Credenciamento.Proposta.DataFundacao = (DateTime)dados.DataFundacao;
            Credenciamento.Proposta.IndicadorSequenciaProposta = dados.IndSeqProp.Value;

            if (!String.IsNullOrEmpty(dados.RazaoSocial))
                Credenciamento.Proposta.RazaoSocial = dados.RazaoSocial.Trim();

            Credenciamento.Proposta.CodigoGrupoRamo = (Int32)dados.CodGrupoRamo;
            Credenciamento.Proposta.CodigoRamoAtividade = (Int32)dados.CodRamoAtividade;
            Credenciamento.Proposta.IndicadorSequenciaProposta = (Int32)dados.IndSeqProp;
            Credenciamento.Proposta.NumeroPontoDeVenda = dados.NumPdv;
            Credenciamento.Proposta.IndicadorEnderecoIgualComercial = (Char)dados.IndEnderecoIgualCom;

            if (!String.IsNullOrEmpty(dados.PessoaContato))
                Credenciamento.Proposta.PessoaContato = dados.PessoaContato.Trim();
            if (!String.IsNullOrEmpty(dados.NomeEmail))
                Credenciamento.Proposta.NomeEmail = dados.NomeEmail.Trim();
            if (!String.IsNullOrEmpty(dados.NomeHomePage))
                Credenciamento.Proposta.NomeHomePage = dados.NomeHomePage.Trim();
            if (!String.IsNullOrEmpty(dados.NumDDD1))
                Credenciamento.Proposta.NumeroDDD1 = dados.NumDDD1.Trim();
            Credenciamento.Proposta.NumeroTelefone1 = dados.NumTelefone1;
            Credenciamento.Proposta.Ramal1 = dados.Ramal1;
            if (!String.IsNullOrEmpty(dados.NumDDD2))
                Credenciamento.Proposta.NumeroDDD2 = dados.NumDDD2.Trim();
            Credenciamento.Proposta.NumeroTelefone2 = dados.NumTelefone2;
            Credenciamento.Proposta.Ramal2 = dados.Ramal2;
            if (!String.IsNullOrEmpty(dados.NumDDDFax))
                Credenciamento.Proposta.NumeroDDDFax = dados.NumDDDFax.Trim();
            Credenciamento.Proposta.NumeroTelefoneFax = dados.NumTelefoneFax;
            Credenciamento.Proposta.CodigoFilial = dados.CodFilial;
            Credenciamento.Proposta.CodigoGerencia = dados.CodGerencia;
            Credenciamento.Proposta.CodigoCarteira = dados.CodCarteira;
            Credenciamento.Proposta.CodigoZona = dados.CodZona;
            Credenciamento.Proposta.CodigoHoraFuncionamentoPV = dados.CodHoraFuncionamentoPV;
            Credenciamento.Proposta.CodigoTipoEstabelecimento = dados.CodTipoEstabelecimento;
            Credenciamento.Proposta.NumeroMatriz = dados.NumeroMatriz;
            if (!String.IsNullOrEmpty(dados.NomeFatura))
                Credenciamento.Proposta.NomeFatura = dados.NomeFatura.Trim();
            Credenciamento.Proposta.NumeroGrupoComercial = dados.NumGrupoComercial;
            Credenciamento.Proposta.NumeroGrupoGerencial = dados.NumGrupoGerencial;
            Credenciamento.Proposta.CodigoLocalPagamento = dados.CodLocalPagamento;
            Credenciamento.Proposta.NumeroCentralizadora = dados.NumCentralizadora;
            Credenciamento.Proposta.DataCadastroProposta = (DateTime)dados.DataCadastroProposta;
            Credenciamento.Proposta.NumeroCPFVendedor = dados.NumCPFVendedor;
            if (!String.IsNullOrEmpty(dados.CodigoCNAE))
                Credenciamento.Proposta.CodigoCNAE = dados.CodigoCNAE.Trim();
            Credenciamento.Proposta.IndicadorEnvioExtratoEmail = dados.IndEnvioExtratoEmail;
            if (!String.IsNullOrEmpty(dados.CodigoCampanha))
                Credenciamento.Proposta.CodigoCampanha = dados.CodigoCampanha.Trim();
            Credenciamento.Proposta.CodigoFaseFiliacao = dados.CodFaseFiliacao;
            Credenciamento.Proposta.IndicadorRegiaoLoja = dados.IndRegiaoLoja != null ? (Char)dados.IndRegiaoLoja : 'R';
            Credenciamento.Proposta.NumeroSolicitacao = (Int32)dados.NumOcorrencia;
            Credenciamento.Proposta.ValorTaxaAdesao = dados.ValorTaxaAdesao;

        }

        /// <summary>
        /// Carregar dados de endereço
        /// </summary>
        /// <param name="proposta">Proposta Pendente</param>
        private void CarregarDadosEnderecos(Modelo.PropostaPendente proposta)
        {
            Char tipoPessoa = (Char)proposta.TipoPessoa;
            Int64 cpfCnpj = (Int64)proposta.CNPJ;
            Int32 numeroSequencia = (Int32)proposta.NumSequencia;

            var enderecos = Servicos.ServicosWF.CarregarDadosEnderecos(tipoPessoa, cpfCnpj, numeroSequencia);

            Credenciamento.Enderecos = new List<Modelo.Endereco>();
            foreach (var endereco in enderecos)
            {
                Credenciamento.Enderecos.Add(new Modelo.Endereco
                {
                    NumeroCNPJ = endereco.NumCNPJ.Value,
                    NumeroSequenciaProposta = endereco.NumSeqProp.Value,
                    CodigoTipoPessoa = (Modelo.TipoPessoa)endereco.CodTipoPessoa.Value,
                    IndicadorTipoEndereco = (Char)endereco.IndTipoEndereco,
                    CodigoCep = endereco.CodigoCep,
                    CodigoComplementoCep = endereco.CodComplementoCep,
                    Logradouro = endereco.Logradouro,
                    ComplementoEndereco = endereco.ComplementoEndereco,
                    Estado = endereco.Estado,
                    Cidade = endereco.Cidade,
                    Bairro = endereco.Bairro,
                    NumeroEndereco = endereco.NumeroEndereco,
                    UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario,
                    DataHoraUltimaAtualizacao = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Carrega dados da tecnologia
        /// </summary>
        /// <param name="proposta">Proposta Pendente</param>
        private void CarregarDadosTecnologia(Modelo.PropostaPendente proposta)
        {
            Char tipoPessoa = (Char)proposta.TipoPessoa;
            Int64 cpfCnpj = (Int64)proposta.CNPJ;
            Int32 numeroSequencia = (Int32)proposta.NumSequencia;

            var tecnologia = Servicos.ServicosWF.CarregarDadosTecnologia(tipoPessoa, cpfCnpj, numeroSequencia);
            if (!String.IsNullOrEmpty(tecnologia.NomeContato))
                Credenciamento.Tecnologia.NomeContato = tecnologia.NomeContato.Trim();
            if (!String.IsNullOrEmpty(tecnologia.Observacao))
                Credenciamento.Tecnologia.Observacao = tecnologia.Observacao.Trim();
            if (tecnologia.IndEnderecoIgualComercial != null)
                Credenciamento.Tecnologia.IndicadorEnderecoIgualComercial = tecnologia.IndEnderecoIgualComercial.Value;
            if (!String.IsNullOrEmpty(tecnologia.NumDDD))
                Credenciamento.Tecnologia.NumeroDDD = tecnologia.NumDDD.Trim();
            if (tecnologia.NumTelefone != null)
                Credenciamento.Tecnologia.NumeroTelefone = tecnologia.NumTelefone.Value;
        }

        /// <summary>
        /// Carregar dados de Domicílio Bancário
        /// </summary>
        /// <param name="proposta">Proposta Pendente</param>
        private void CarregarDadosDomicilioBancario(Modelo.PropostaPendente proposta)
        {
            Char tipoPessoa = (Char)proposta.TipoPessoa;
            Int64 cpfCnpj = (Int64)proposta.CNPJ;
            Int32 numeroSequencia = (Int32)proposta.NumSequencia;

            var domicilioBancarioCredito = Servicos.ServicosWF.CarregarDadosDomicilioBancario(tipoPessoa, cpfCnpj, numeroSequencia, 1);
            var domicilioBancarioDebito = Servicos.ServicosWF.CarregarDadosDomicilioBancario(tipoPessoa, cpfCnpj, numeroSequencia, 3);
            var domicilioBancarioConstrucard = Servicos.ServicosWF.CarregarDadosDomicilioBancario(tipoPessoa, cpfCnpj, numeroSequencia, 4);


            if (domicilioBancarioCredito != null)
            {
                Credenciamento.DomiciliosBancarios.Add(new Modelo.DomicilioBancario
                {
                    IndicadorTipoOperacaoProd = 1,
                    CodigoBanco = (Int32)domicilioBancarioCredito.CodBancoCompensacao,
                    NomeBanco = domicilioBancarioCredito.NomeBanco,
                    CodigoAgencia = (Int32)domicilioBancarioCredito.CodigoAgencia,
                    NumeroContaCorrente = domicilioBancarioCredito.NumContaCorrente,
                    TipoDomicilioBancario = Modelo.TipoDomicilioBancario.Credito,
                    IndicadorTipoAcaoBanco = Modelo.TipoAcaoBanco.Inclusao,
                    UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario,
                    DataHoraUltimaAtualizacao = DateTime.Now,
                    //CodigoTipoPessoa = domicilioBancarioConstrucard.CodTipoPessoa.Value,
                    //NumeroCNPJ = domicilioBancarioConstrucard.NumCNPJ.Value,
                    //NumeroSeqProp = domicilioBancarioConstrucard.NumSeqProp.Value,
                    //IndicadorDomicilioDuplicado = domicilioBancarioConstrucard.IndDomicilioDuplicado.Value,
                    //IndConfirmacaoDomicilio = domicilioBancarioConstrucard.IndConfirmacaoDomicilio.Value
                });
            }

            if (domicilioBancarioDebito != null)
            {
                Credenciamento.DomiciliosBancarios.Add(new Modelo.DomicilioBancario
                {
                    IndicadorTipoOperacaoProd = 3,
                    CodigoBanco = (Int32)domicilioBancarioDebito.CodBancoCompensacao,
                    NomeBanco = domicilioBancarioDebito.NomeBanco,
                    CodigoAgencia = (Int32)domicilioBancarioDebito.CodigoAgencia,
                    NumeroContaCorrente = domicilioBancarioDebito.NumContaCorrente,
                    TipoDomicilioBancario = Modelo.TipoDomicilioBancario.Debito,
                    IndicadorTipoAcaoBanco = Modelo.TipoAcaoBanco.Inclusao,
                    UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario,
                    DataHoraUltimaAtualizacao = DateTime.Now,
                    //CodigoTipoPessoa = domicilioBancarioConstrucard.CodTipoPessoa.Value,
                    //NumeroCNPJ = domicilioBancarioConstrucard.NumCNPJ.Value,
                    //NumeroSeqProp = domicilioBancarioConstrucard.NumSeqProp.Value,
                    //IndicadorDomicilioDuplicado = domicilioBancarioConstrucard.IndDomicilioDuplicado.Value,
                    //IndConfirmacaoDomicilio = domicilioBancarioConstrucard.IndConfirmacaoDomicilio.Value
                });
            }

            if (domicilioBancarioConstrucard != null)
            {
                Credenciamento.DomiciliosBancarios.Add(new Modelo.DomicilioBancario
                {
                    IndicadorTipoOperacaoProd = 4,
                    CodigoBanco = (Int32)domicilioBancarioConstrucard.CodBancoCompensacao,
                    NomeBanco = domicilioBancarioConstrucard.NomeBanco,
                    CodigoAgencia = (Int32)domicilioBancarioConstrucard.CodigoAgencia,
                    NumeroContaCorrente = domicilioBancarioConstrucard.NumContaCorrente,
                    TipoDomicilioBancario = Modelo.TipoDomicilioBancario.Construcard,
                    IndicadorTipoAcaoBanco = Modelo.TipoAcaoBanco.Inclusao,
                    UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario,
                    DataHoraUltimaAtualizacao = DateTime.Now,

                    //CodigoTipoPessoa = domicilioBancarioConstrucard.CodTipoPessoa.Value,
                    //NumeroCNPJ = domicilioBancarioConstrucard.NumCNPJ.Value,
                    //NumeroSeqProp = domicilioBancarioConstrucard.NumSeqProp.Value,
                    //IndicadorDomicilioDuplicado = domicilioBancarioConstrucard.IndDomicilioDuplicado.Value,
                    //IndConfirmacaoDomicilio = domicilioBancarioConstrucard.IndConfirmacaoDomicilio.Value
                });
            }
        }

        /// <summary>
        /// Carrega lista de proprietários
        /// </summary>
        /// <param name="proposta">Proposta Pendente</param>
        private void CarregarDadosProprietarios(Modelo.PropostaPendente proposta)
        {
            Char tipoPessoa = (Char)proposta.TipoPessoa;
            Int64 cpfCnpj = (Int64)proposta.CNPJ;
            Int32 numeroSequencia = (Int32)proposta.NumSequencia;

            var proprietarios = Servicos.ServicosWF.CarregarDadosProprietarios(tipoPessoa, cpfCnpj, numeroSequencia);
            Credenciamento.Proprietarios = new List<Modelo.Proprietario>();

            foreach (var proprietario in proprietarios)
            {
                Credenciamento.Proprietarios.Add(new Modelo.Proprietario
                {
                    NumeroCNPJ = cpfCnpj,
                    CodigoTipoPessoa = (Modelo.TipoPessoa)(Int32)tipoPessoa,
                    NumeroSequenciaProposta = numeroSequencia,
                    NumeroCNPJCPFProprietario = (Int64)proprietario.NumCNPJCPFProprietario,
                    NomeProprietario = proprietario.NomeProprietario,
                    ParticipacaoAcionaria = (Double)proprietario.ParticipacaoAcionaria,
                    CodigoTipoPesssoaProprietario = (Modelo.TipoPessoa)(Int32)proprietario.CodTipoPesProprietario,
                });
            }
        }

        /// <summary>
        /// Carrega dados de Ofertas Preço Único
        /// </summary>
        /// <param name="proposta">Proposta Pendente</param>
        private void CarregarDadosOfertasPrecoUnico(Modelo.PropostaPendente proposta)
        {
            Char tipoPessoa = (Char)proposta.TipoPessoa;
            Int64 cpfCnpj = (Int64)proposta.CNPJ;
            Int32 numeroSequencia = (Int32)proposta.NumSequencia;

            var ofertaPrecoUnico = Servicos.ServicosWF.ConsultaOfertaPrecoUnico('L', tipoPessoa, cpfCnpj, numeroSequencia, null).FirstOrDefault();

            if (ofertaPrecoUnico != default(WFOfertas.ConsultaOfertaPrecoUnico))
            {
                Credenciamento.OfertasPrecoUnico.Add(new Modelo.OfertaPrecoUnico()
                {
                    CodigoTipoPessoa = ofertaPrecoUnico.CodigoTipoPessoa,
                    CodigoUsuario = ofertaPrecoUnico.CodigoUsuario,
                    CodigosOfertaPrecoUnico = ofertaPrecoUnico.CodigosOfertaPrecoUnico,
                    DataHoraUltimaAtualizacao = ofertaPrecoUnico.DataHoraUltimaAtualizacao,
                    NumeroCNPJ = ofertaPrecoUnico.NumeroCNPJ,
                    NumeroSequenciaProposta = ofertaPrecoUnico.NumeroSequenciaProposta
                });
            }
        }

        /// <summary>
        /// Consulta o próximo número de sequência da proposta
        /// </summary>
        private void ConsultaProximaSequencia()
        {
            Char tipoPessoa = Credenciamento.Proposta.CodigoTipoPessoa;
            Int64 numCnpjCpf = Credenciamento.Proposta.NumeroCnpjCpf;

            Credenciamento.Proposta.IndicadorSequenciaProposta = Servicos.ServicosWF.ConsultaProximaSequencia(tipoPessoa, numCnpjCpf);
        }

        /// <summary>
        /// Lista Propietários por Ponto de Venda
        /// </summary>
        /// <param name="propostaSelecionada">Proposta Selecionada</param>
        private void ListaProprietariosPontoVenda(Modelo.PropostaPendente propostaSelecionada)
        {
            if (propostaSelecionada.NroEstabelecimento != null && propostaSelecionada.NroEstabelecimento != 0)
            {
                Int32 numPdv = (Int32)propostaSelecionada.NroEstabelecimento;
                var proprietarios = Servicos.ServicosGE.ListaProprietariosPontoVenda(numPdv);

                Credenciamento.Proprietarios = new List<Modelo.Proprietario>();

                foreach (var proprietario in proprietarios)
                {
                    Credenciamento.Proprietarios.Add(new Modelo.Proprietario
                    {
                        NumeroCNPJCPFProprietario = (Int64)proprietario.NumCNPJCPFProprietario,
                        NomeProprietario = proprietario.NomeProprietario,
                        ParticipacaoAcionaria = (Double)proprietario.ValorParticipacaoSocietaria,
                        CodigoTipoPesssoaProprietario = (Modelo.TipoPessoa)(Int32)proprietario.CodTipoPessoaProprietario
                    });
                }
            }
        }

        /// <summary>
        /// Carrega dados para um recredenciamento
        /// </summary>
        /// <param name="propostaSelecionada">Proposta Selecionada</param>
        private void CarregarDadosRecredenciamento(Modelo.PropostaPendente proposta)
        {
            Char tipoPessoa = (Char)proposta.TipoPessoa;
            Int64 cpfCnpj = (Int64)proposta.CNPJ;

            Credenciamento.Proposta.NumeroPontoDeVenda = proposta.NroEstabelecimento;

            //var dadosRecredenciamento = Servicos.ServicosGE.CarregarDadosRecredenciamento(tipoPessoa, cpfCnpj);

            //Credenciamento.Proposta.NumeroPontoDeVenda = dadosRecredenciamento.NumPdv;

            //Credenciamento.Proposta.CodigoGrupoRamo = (Int32)dadosRecredenciamento.CodGrupoRamo;
            //Credenciamento.Proposta.CodigoRamoAtividade = (Int32)dadosRecredenciamento.CodRamoAtivididade;
            //Credenciamento.Proposta.RazaoSocial = dadosRecredenciamento.RazaoSocial;
            //Credenciamento.Proposta.DataFundacao = (DateTime)dadosRecredenciamento.DataFundacao;
            //Credenciamento.Proposta.PessoaContato = dadosRecredenciamento.PessoaContato;
            //Credenciamento.Proposta.NomeFatura = dadosRecredenciamento.NomeFatura;
            //Credenciamento.Proposta.NumeroDDD1 = dadosRecredenciamento.NumDDD1;
            //Credenciamento.Proposta.NumeroTelefone1 = dadosRecredenciamento.NumTelefone1;
            //Credenciamento.Proposta.Ramal1 = dadosRecredenciamento.NumRamal1;
            //Credenciamento.Proposta.NumeroDDD2 = dadosRecredenciamento.NumDDD2;
            //Credenciamento.Proposta.NumeroTelefone2 = dadosRecredenciamento.NumTelefone2;
            //Credenciamento.Proposta.Ramal2 = dadosRecredenciamento.NumRamal2;
            //Credenciamento.Proposta.NumeroDDDFax = dadosRecredenciamento.NumDDDFax;
            //Credenciamento.Proposta.NumeroTelefoneFax = dadosRecredenciamento.NumTelefoneFax;
            //Credenciamento.Proposta.NomeEmail = dadosRecredenciamento.Email;
            //Credenciamento.Proposta.CodigoTipoEstabelecimento = dadosRecredenciamento.CodTipoEstabelecimento;
            //Credenciamento.Proposta.NomeHomePage = dadosRecredenciamento.HomePage;
            //Credenciamento.Proposta.CodigoCarteira = dadosRecredenciamento.CodCarteira;
            //Credenciamento.Proposta.CodigoGerencia = dadosRecredenciamento.CodGerencia;
            //Credenciamento.Proposta.CodigoLocalPagamento = dadosRecredenciamento.CodLocalPagamento;
            //Credenciamento.Proposta.IndicadorIATA = dadosRecredenciamento.IndicadorIATA;
            //Credenciamento.Proposta.NumeroGrupoComercial = dadosRecredenciamento.NumGrupoComercial;
            //Credenciamento.Proposta.NumeroGrupoGerencial = dadosRecredenciamento.NumGrupoGerencial;

            //Credenciamento.Enderecos = new List<Modelo.Endereco>();
            //Credenciamento.Enderecos.Add(new Modelo.Endereco
            //{

            //    IndicadorTipoEndereco = '1',
            //    CodigoCep = dadosRecredenciamento.CodCEP,
            //    CodigoComplementoCep = dadosRecredenciamento.CodCompCEP,
            //    Logradouro = dadosRecredenciamento.NomeLogradouro,
            //    NumeroEndereco = dadosRecredenciamento.NumLogradouro,
            //    ComplementoEndereco = dadosRecredenciamento.CompEndereco,
            //    Bairro = dadosRecredenciamento.NomeBairro,
            //    Cidade = dadosRecredenciamento.NomeCidade,
            //    Estado = dadosRecredenciamento.NomeUF,
            //    DataHoraUltimaAtualizacao = DateTime.Now,
            //    UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario
            //});

            //Credenciamento.Enderecos.Add(new Modelo.Endereco
            //{
            //    IndicadorTipoEndereco = '2',
            //    CodigoCep = dadosRecredenciamento.CodCEPCorrespondencia,
            //    CodigoComplementoCep = dadosRecredenciamento.CodCompCEPCorrespondencia,
            //    Logradouro = dadosRecredenciamento.NomeLogradouroCorrespondencia,
            //    NumeroEndereco = dadosRecredenciamento.NumLogradouroCorrespondencia,
            //    ComplementoEndereco = dadosRecredenciamento.CompEnderecoCorrespondencia,
            //    Bairro = dadosRecredenciamento.NomeBairroCorrespondencia,
            //    Cidade = dadosRecredenciamento.NomeCidadeCorrespondencia,
            //    Estado = dadosRecredenciamento.NomeUFCorrespondencia,
            //    DataHoraUltimaAtualizacao = DateTime.Now,
            //    UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario
            //});
        }

        /// <summary>
        /// Carrega dados domicílio bancário crédito da matriz
        /// </summary>
        /// <param name="proposta">Proposta Pendente</param>
        private void CarregarDadosDomicilioBancarioCreditoPorPontoVenda(Modelo.PropostaPendente proposta)
        {
            Int32 numPontoVenda = (Int32)proposta.NroEstabelecimento;
            Char? codTipoOperacao = 'C';
            String siglaProduto = "CR";

            var domBancarioCredito = Servicos.ServicosGE.CarregarDadosDomicilioBancarioPorPontoVenda(numPontoVenda, codTipoOperacao, siglaProduto).FirstOrDefault();

            if (domBancarioCredito != null)
            {

                if (domBancarioCredito.CodigoErro == 0)
                {
                    Credenciamento.DomiciliosBancarios.Add(new Modelo.DomicilioBancario
                    {
                        IndicadorTipoOperacaoProd = 1,
                        IndicadorTipoAcaoBanco = Modelo.TipoAcaoBanco.Inclusao,
                        TipoDomicilioBancario = Modelo.TipoDomicilioBancario.Credito,
                        CodigoBanco = (Int32)domBancarioCredito.CodBancoAtual,
                        CodigoAgencia = (Int32)domBancarioCredito.CodAgenciaAtual,
                        NumeroContaCorrente = domBancarioCredito.NumeroContaAtual,
                        NomeBanco = domBancarioCredito.NomeBancoAtual,
                        DataHoraUltimaAtualizacao = DateTime.Now
                    });
                }
            }
        }

        /// <summary>
        /// Carrega dados da proposta por ponto de venda
        /// </summary>
        /// <param name="proposta">Proposta Pendente</param>
        private void CarregarDadosPorPontoVenda(Modelo.PropostaPendente proposta)
        {
            if (proposta.NroEstabelecimento != null && proposta.NroEstabelecimento != 0)
            {
                Int32 numPdv = (Int32)proposta.NroEstabelecimento;

                var dadosPv = Servicos.ServicosGE.CarregarDadosPorPontoVenda(numPdv);

                Credenciamento.Proposta.CodigoFilial = dadosPv.CodFilial;
                Credenciamento.Proposta.CodigoGrupoRamo = (Int32)dadosPv.CodGrupoRamo;
                Credenciamento.Proposta.CodigoHoraFuncionamentoPV = dadosPv.CodHorarioFuncionamento;
                Credenciamento.Proposta.CodigoRamoAtividade = (Int32)dadosPv.CodRamoAtivididade;
                Credenciamento.Proposta.CodigoZona = dadosPv.CodZona;
                Credenciamento.Proposta.DataFundacao = (DateTime)dadosPv.DataFundacao;
                Credenciamento.Proposta.NomeEmail = dadosPv.Email;
                Credenciamento.Proposta.NomeHomePage = dadosPv.HomePage;
                Credenciamento.Proposta.NomeFatura = dadosPv.NomeFatura;
                Credenciamento.Proposta.NumeroCentralizadora = dadosPv.NumCentralizadora;
                Credenciamento.Proposta.NumeroGrupoComercial = dadosPv.NumGrupoComercial;
                Credenciamento.Proposta.NumeroGrupoGerencial = dadosPv.NumGrupoGerencial;
                Credenciamento.Proposta.NumeroDDD1 = dadosPv.NumDDD1;
                Credenciamento.Proposta.NumeroTelefone1 = dadosPv.NumTelefone1;
                Credenciamento.Proposta.Ramal1 = dadosPv.NumRamal1;
                Credenciamento.Proposta.NumeroDDD2 = dadosPv.NumDDD2;
                Credenciamento.Proposta.NumeroTelefone2 = dadosPv.NumTelefone2;
                Credenciamento.Proposta.Ramal2 = dadosPv.NumRamal2;
                Credenciamento.Proposta.NumeroDDDFax = dadosPv.NumDDDFax;
                Credenciamento.Proposta.NumeroTelefoneFax = dadosPv.NumTelefoneFax;
                Credenciamento.Proposta.NumeroMatriz = dadosPv.NumeroMatriz;
                Credenciamento.Proposta.PessoaContato = dadosPv.PessoaContato;
                Credenciamento.Proposta.RazaoSocial = dadosPv.RazaoSocial;

                Credenciamento.Enderecos = new List<Modelo.Endereco>();

                Credenciamento.Enderecos.Add(new Modelo.Endereco
                {
                    IndicadorTipoEndereco = '1',
                    CodigoCep = dadosPv.CodCEP,
                    CodigoComplementoCep = dadosPv.CodCompCEP,
                    Logradouro = dadosPv.NomeLogradouro,
                    ComplementoEndereco = dadosPv.CompEndereco,
                    Bairro = dadosPv.NomeBairro,
                    Cidade = dadosPv.NomeCidade,
                    Estado = dadosPv.NomeUF,
                    NumeroEndereco = dadosPv.NumLogradouro,
                    DataHoraUltimaAtualizacao = DateTime.Now,
                    UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario
                });

                Credenciamento.Enderecos.Add(new Modelo.Endereco
                {
                    IndicadorTipoEndereco = '2',
                    CodigoCep = dadosPv.CodCEPCorrespondencia,
                    CodigoComplementoCep = dadosPv.CodCompCEPCorrespondencia,
                    Logradouro = dadosPv.NomeLogradouroCorrespondencia,
                    ComplementoEndereco = dadosPv.CompEnderecoCorrespondencia,
                    Bairro = dadosPv.NomeBairroCorrespondencia,
                    Cidade = dadosPv.NomeCidadeCorrespondencia,
                    Estado = dadosPv.NomeUFCorrespondencia,
                    NumeroEndereco = dadosPv.NumLogradouroCorrespondencia,
                    DataHoraUltimaAtualizacao = DateTime.Now,
                    UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario
                });

                Credenciamento.Enderecos.Add(new Modelo.Endereco
                {
                    IndicadorTipoEndereco = '4',
                    CodigoCep = dadosPv.CodCEPTecnologia,
                    CodigoComplementoCep = dadosPv.CodCompCEPTecnologia,
                    Logradouro = dadosPv.NomeLogradouroTecnologia,
                    ComplementoEndereco = dadosPv.CompEnderecoTecnologia,
                    Bairro = dadosPv.NomeBairroTecnologia,
                    Cidade = dadosPv.NomeCidadeTecnologia,
                    Estado = dadosPv.NomeUFTecnologia,
                    NumeroEndereco = dadosPv.NumLogradouroTecnologia,
                    DataHoraUltimaAtualizacao = DateTime.Now,
                    UsuarioUltimaAtualizacao = SessaoAtual.LoginUsuario
                });

            }
        }
    }
}
