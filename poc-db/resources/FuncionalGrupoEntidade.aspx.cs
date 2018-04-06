using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.AdministracaoServico;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais
{
    public partial class FuncionalGrupoEntidade : LayoutsPageBase
    {
        #region [ Eventos ]

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlErro.Visible = false;
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Funcional Grupo Entidade - Botão Enviar"))
            {
                try
                {
                    Perfil perfil = null;
                    var funcional = txtFuncional.Text;
                    var grupoEntidade = ddlGrupoEntidade.SelectedValue.ToInt32();

                    var codigoRetorno = SalvarFuncionalGrupoEntidade(funcional, grupoEntidade);

                    if (codigoRetorno == 0)
                        perfil = new Perfil { Funcional = funcional, GrupoEntidade = grupoEntidade };

                    MostrarResultado(codigoRetorno, perfil);
                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Funcional Grupo Entidade - Botão Enviar", ex);
                    SharePointUlsLog.LogErro(ex);
                    MostraErro(ex);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Funcional Grupo Entidade - Botão Enviar", ex);
                    SharePointUlsLog.LogErro(ex);
                    MostraErro(ex);
                }
            }
        }        

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Funcional Grupo Entidade - Botão Buscar"))
            {
                try
                {
                    var funcional = txtFuncional.Text;
                    var codigoRetorno = 0;

                    var perfil = BuscarFuncionalGrupoEntidade(funcional);
                    if (perfil == null)
                        codigoRetorno = 1;

                    MostrarResultado(codigoRetorno, perfil);
                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Funcional Grupo Entidade - Botão Enviar", ex);
                    SharePointUlsLog.LogErro(ex);
                    MostraErro(ex);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Funcional Grupo Entidade - Botão Buscar", ex);
                    SharePointUlsLog.LogErro(ex);
                    MostraErro(ex);
                }
            }
        }

        protected void btnDeletar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Funcional Grupo Entidade - Botão Deletar"))
            {
                try
                {
                    var funcional = ltrFuncional.Text;
                    var codigoRetorno = DeletarFuncionalGrupoEntidade(funcional);

                    MostrarResultado(codigoRetorno, null);
                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Funcional Grupo Entidade - Botão Enviar", ex);
                    SharePointUlsLog.LogErro(ex);
                    MostraErro(ex);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Funcional Grupo Entidade - Botão Deletar", ex);
                    SharePointUlsLog.LogErro(ex);
                    MostraErro(ex);
                }
            }

        }

        #endregion

        #region [ Métodos ]

        private Perfil BuscarFuncionalGrupoEntidade(String funcional)
        {
            var retorno = new Perfil();

            using (var log = Logger.IniciarLog("Funcional Grupo Entidade - Serviço busca perfil"))
            {
                log.GravarLog(EventoLog.ChamadaServico, funcional);

                using (var contexto = new ContextoWCF<AdministracaoServicoClient>())
                {
                    retorno = contexto.Cliente.ConsultarFuncionalGrupoEntidade(funcional);
                }

                log.GravarLog(EventoLog.RetornoServico, retorno);
            }

            return retorno;
        }

        private Int32 SalvarFuncionalGrupoEntidade(String funcional, Int32 grupoEntidade)
        {
            Int32 retorno = 0;

            using (var log = Logger.IniciarLog("Funcional Grupo Entidade - Serviço Salvar ou Atualizar Perfil"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    funcional,
                    grupoEntidade
                });

                using (var contexto = new ContextoWCF<AdministracaoServicoClient>())
                {
                    retorno = contexto.Cliente.InserirFuncionalGrupoEntidade(funcional, grupoEntidade);
                }

                log.GravarLog(EventoLog.RetornoServico, retorno);
            }

            return retorno;
        }

        private Int32 DeletarFuncionalGrupoEntidade(String funcional)
        {
            Int32 retorno = 0;

            using (var log = Logger.IniciarLog("Funcional Grupo Entidade - Serviço Deletar Perfil"))
            {
                log.GravarLog(EventoLog.ChamadaServico, funcional);

                using (var contexto = new ContextoWCF<AdministracaoServicoClient>())
                {
                    retorno = contexto.Cliente.DeletarFuncionalGrupoEntidade(funcional);
                }

                log.GravarLog(EventoLog.RetornoServico, retorno);
            }

            return retorno;
        }

        private void MostrarResultado(Int32 codigoRetorno, Perfil perfil)
        {
            ltrCodigoRetorno.Text = codigoRetorno.ToString();

            if (perfil != null)
            {
                ltrFuncional.Text = perfil.Funcional;
                ltrGrupoEntidade.Text = perfil.GrupoEntidade == 14 ? "Atendimento Perfil 1" : "Atendimento Perfil 2";
                btnDeletar.Visible = true;
            }
            else
            {
                ltrFuncional.Text = String.Empty;
                ltrGrupoEntidade.Text = String.Empty;
                btnDeletar.Visible = false;
            }
        }

        private void MostraErro(Exception ex)
        {
            pnlErro.Visible = true;
            ltrErro.Text = ex.Message;
        }

        #endregion
    }
}
