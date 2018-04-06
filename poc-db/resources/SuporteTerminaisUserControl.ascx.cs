using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.MaximoServico;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.SuporteTerminais
{    
    public partial class SuporteTerminaisUserControl : UserControlBase
    {        
        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CarregarTerminais();
            }
        }

        /// <summary>
        /// Carrega os terminais na tela
        /// </summary>
        private void CarregarTerminais()
        {
            using (Logger log = Logger.IniciarLog("Suporte à Terminais - Carregando terminais"))
            {
                try
                {
                    //Prepara objetos para consulta de Ordens de Serviço
                    FiltroOS filtroOS = new FiltroOS();
                    filtroOS.PontoVenda = SessaoAtual.CodigoEntidade.ToString();
                    filtroOS.DataAbertura = new Periodo();
                    filtroOS.DataAbertura.Termino = DateTime.Today.AddDays(1);
                    filtroOS.DataAbertura.Inicio = DateTime.Today.AddDays(-90);

                    //Lista de OSs abertas
                    List<OSDetalhada> ordensServico = null;

                    //Chama serviço máximo, buscando todas as OSs aberta para o PV
                    using (var contexto = new ContextoWCF<MaximoServicoClient>())
                        ordensServico = contexto.Cliente.ConsultarOSAbertaDetalhada(filtroOS);

                    List<Object> terminais = new List<Object>();

                    //Bind das OSs, ou exibição de aviso "Sem dados"
                    if (ordensServico != null && ordensServico.Count > 0)
                    {
                        //Prepara lista de Terminais, com sua OS vinculada no mesmo objeto                        
                        foreach (OSDetalhada osDetalhada in ordensServico)
                        {
                            if (osDetalhada.Terminal != null && osDetalhada.Terminal.Count > 0)
                            {
                                terminais.AddRange(osDetalhada.Terminal.Select(osTerminal => new
                                {
                                    OSDetalhada = osDetalhada,
                                    OSTerminal = osTerminal
                                }).ToArray());
                            }
                            else
                            {
                                terminais.Add(new
                                {
                                    OSDetalhada = osDetalhada,
                                    OSTerminal = default(OSTerminal)
                                });
                            }
                        }                        
                    }

                    if (terminais != null && terminais.Count > 0)
                    {
                        rptTecnologia.DataSource = terminais;
                        rptTecnologia.DataBind();
                    }
                    else
                    {
                        mvTerminais.SetActiveView(vwSemDados);
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Preenche a linha da tabela com os dados de um terminal
        /// </summary>
        protected void rptTecnologia_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {                              
                RepeaterItem repeaterItem = e.Item;
                Object item = repeaterItem.DataItem as Object;
                OSTerminal osTerminal = (OSTerminal)item.GetType().GetProperty("OSTerminal").GetValue(item, null);
                OSDetalhada osDetalhada = (OSDetalhada)item.GetType().GetProperty("OSDetalhada").GetValue(item, null);

                BindItemDadosPrincipais(repeaterItem, osTerminal, osDetalhada);
                BindItemDetalhes(repeaterItem, osTerminal, osDetalhada);                
            }
        }

        /// <summary>
        /// Bind dos valores dos dados principais do item do repeater
        /// </summary>
        /// <param name="item">RepeaterItem</param>
        /// <param name="terminal">Dados do terminal</param>
        /// <param name="ordemServico">Dados da ordem do serviço</param>
        private void BindItemDadosPrincipais(RepeaterItem item, OSTerminal terminal, OSDetalhada ordemServico)
        {
            //recuperação dos controles do repeater
            var lblNumeroTerminal = item.FindControl("lblNumeroTerminal") as Label;
            var lblMaquininhaComFio = item.FindControl("lblMaquininhaComFio") as Label;
            var lblMaquininhaSemFio = item.FindControl("lblMaquininhaSemFio") as Label;
            var imgMaquininhaSemFio = item.FindControl("imgMaquininhaSemFio") as Image;
            var imgMaquininhaComFio = item.FindControl("imgMaquininhaComFio") as Image;
            var lblPrevisaoAtendimento = item.FindControl("lblPrevisaoAtendimento") as Label;
            var lblOrdemServico = item.FindControl("lblOrdemServico") as Label;
            var lblDataSolicitacao = item.FindControl("lblDataSolicitacao") as Label;

            if (terminal != default(OSTerminal))
            {
                lblNumeroTerminal.Text = terminal.NumeroLogico;

                Boolean maquininhaPOO = String.Compare(terminal.TipoEquipamento, "POO", true) == 0;
                Boolean maquininhaPOS = String.Compare(terminal.TipoEquipamento, "POS", true) == 0;

                if (maquininhaPOO)
                {
                    imgMaquininhaSemFio.Visible = true;
                    lblMaquininhaSemFio.Visible = true;
                }

                if (maquininhaPOS)
                {
                    imgMaquininhaComFio.Visible = true;
                    lblMaquininhaComFio.Visible = true;
                }
            }

            String strDataAtendimento = ordemServico.DataProgramada.HasValue ?
                ordemServico.DataProgramada.Value.ToString("dd/MM/yyyy") : "-";
            if (DateTime.MinValue.ToString("dd/MM/yyyy").CompareTo(strDataAtendimento) == 0)
                strDataAtendimento = "-";
            lblPrevisaoAtendimento.Text = strDataAtendimento;
            lblOrdemServico.Text = ordemServico.Numero ?? "-";
            lblDataSolicitacao.Text = ordemServico.DataSituacao.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Bind dos valores dos detalhamento do item do repeater
        /// </summary>
        /// <param name="item">RepeaterItem</param>
        /// <param name="terminal">Dados do terminal</param>
        /// <param name="ordemServico">Dados da ordem do serviço</param>
        private void BindItemDetalhes(RepeaterItem item, OSTerminal terminal, OSDetalhada ordemServico)
        {
            var lblProblemaEncontrado = item.FindControl("lblProblemaEncontrado") as Label;
            var lblContatoNome = item.FindControl("lblContatoNome") as Label;
            var lblContatoEmail = item.FindControl("lblContatoEmail") as Label;
            var lblContatoTelefone = item.FindControl("lblContatoTelefone") as Label;
            var lblEnderecoLogradouro = item.FindControl("lblEnderecoLogradouro") as Label;
            var lblEnderecoCidade = item.FindControl("lblEnderecoCidade") as Label;
            var lblEnderecoCep = item.FindControl("lblEnderecoCep") as Label;

            lblProblemaEncontrado.Text = ordemServico.Observacao ?? "-";

            if (ordemServico.Contato != null)
            {
                lblContatoNome.Text = ToPascalCase(ordemServico.Contato.Nome);
                lblContatoEmail.Text = (ordemServico.Contato.Email ?? String.Empty).ToLower();
                lblContatoTelefone.Text = FormataTelefone(ordemServico.Contato.Celular);
            }

            if (ordemServico.EnderecoAtendimento != null)
            {
                MaximoServico.Endereco endereco = ordemServico.EnderecoAtendimento;

                //logradouro, numero, bairro
                lblEnderecoLogradouro.Text = String.Join(", ", 
                    new String[] { ToPascalCase(endereco.Logradouro), endereco.Numero, ToPascalCase(endereco.Bairro) }
                    .Where(logradouro => !String.IsNullOrWhiteSpace(logradouro)));

                //cidade - estado
                lblEnderecoCidade.Text = String.Join(" - ",
                    new String[] { ToPascalCase(endereco.Cidade), endereco.Estado.ToString() }
                    .Where(cidade => !String.IsNullOrWhiteSpace(cidade)));

                lblEnderecoCep.Text = FormataCep(endereco.Cep);
            }
        }

        /// <summary>
        /// Formata o texto para Pascal Case
        /// </summary>
        /// <param name="text">Texto</param>
        /// <returns>Texto em Pascal Case</returns>
        private String ToPascalCase(String text)
        {
            if(String.IsNullOrWhiteSpace(text))
                return text;

            return CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(text.ToLower());
        }

        /// <summary>
        /// Formata CEP adicionando máscara
        /// </summary>
        /// <param name="cep">CEP</param>
        /// <returns>CEP formatado</returns>
        private String FormataCep(String cep)
        {
            if (String.IsNullOrWhiteSpace(cep))
                return cep;
            IEnumerable<Char> cepNums = cep.Where(d => Char.IsNumber(d));

            return String.Format("{0}-{1}", 
                new String(cepNums.Take(5).ToArray()), 
                new String(cepNums.Skip(5).ToArray()));
        }

        /// <summary>
        /// formata telefone adicionando máscaras
        /// </summary>
        /// <param name="telefone">Telefone</param>
        /// <returns>Telefone formatado</returns>
        private String FormataTelefone(String telefone)
        {
            if (String.IsNullOrWhiteSpace(telefone))
                return telefone;

            //remove zeros à esquerda desnecessários
            telefone = telefone.Trim().TrimStart('0');

            //mantém apenas os números
            IEnumerable<Char> telNums = telefone.Where(d => Char.IsNumber(d));

            //aplica máscara para DDD + 9 dígitos
            if (telNums.Count() >= 11)
            {
                return String.Format("({0}) {1}-{2}",
                    new String(telNums.Take(2).ToArray()),
                    new String(telNums.Skip(2).Take(5).ToArray()),
                    new String(telNums.Skip(7).ToArray()));
            }

            //aplica máscara para DDD + 8 dígitos
            return String.Format("({0}) {1}-{2}",
                new String(telNums.Take(2).ToArray()),
                new String(telNums.Skip(2).Take(4).ToArray()),
                new String(telNums.Skip(6).ToArray()));   
        }

        /// <summary>Voltar para a tela de quadros de menu de Informações Cadastrais</summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            String url = String.Format("{0}/Paginas/pn_InformacoesCadastrais.aspx", base.web.ServerRelativeUrl);
            Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}