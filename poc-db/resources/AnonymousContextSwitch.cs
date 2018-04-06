using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.SharePoint;

namespace Redecard.Portal.Helper
{
    /// <summary>
    /// Classe utilitária que permite que um código que requer autenticação no site seja executado sem esta necessidade
    /// URL de apoio: http://jcapka.blogspot.com/2010/05/making-linq-to-sharepoint-work-for.html
    /// </summary>
    public static class AnonymousContextSwitch
    {
        public static void RunWithElevatedPrivilegesAndContextSwitch(SPSecurity.CodeToRunElevated secureCode)
        {
            try
            {   
                //If there is a SPContext.Current object and there is no known user, we need to take action                
                bool nullUser = (SPContext.Current != null && SPContext.Current.Web.CurrentUser == null);
                HttpContext backupCtx = HttpContext.Current;
                if (nullUser)
                    HttpContext.Current = null;
                SPSecurity.RunWithElevatedPrivileges(secureCode);
                if (nullUser)
                    HttpContext.Current = backupCtx;
            }
            catch (Exception ex)
            {
                string errorMessage = "Error running code in null http context";
                //Use your favourite form of logging to log the error message and exception ....            
            }
        }

        /*
        EXEMPLO DE USO COM RESPOSITÓRIO DE NOTÍCIA
        /// <summary>
        ///   Carrega todas as notícias da lista "Notícias".
        /// </summary>
        private void Refresh()
        {
            try
            {
                string currentWebUrl = SPContext.Current.Web.Url;

                using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTONoticia, NotíciasItem>>())
                {        
                    AnonymousContextSwitch.RunWithElevatedPrivelligesAndContextSwitch(
                    delegate
                    {
                            GrvNoticias.DataSource = repository.GetAllItems();
                            GrvNoticias.DataBind();
                    });
                }
                    
            
            }
                //Todos os repositórios disparam o erro para que seja tratado aqui para ser exibido da forma desejada.
            catch (Exception)
            {
                LblMensagem.Text = "Ocorreu um erro durante o Refresh da página.";
            }
        }
        */
    }
}
