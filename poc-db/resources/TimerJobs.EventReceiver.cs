using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Redecard.PN.DadosCadastrais.SharePoint.TimerJobs;
using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.SharePoint.Features.TimerJobs
{
    [Guid("4a8aae84-de63-4731-bb82-01ac5d5f87b5")]
    public class TimerJobsEventReceiver : SPFeatureReceiver
    {        
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            using (Logger Log = Logger.IniciarLog("FeatureActivated"))
            {
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(() =>
                    {
                        SPWeb web = properties.Feature.Parent as SPWeb;                        
                        SPWebApplication webApp = web.Site.WebApplication;

                        //Remove o TimerJob caso já exista
                        foreach (SPJobDefinition job in webApp.JobDefinitions)
                            if (job.Name == TimerJobRelatoriosSuporteTerminais.JobName)
                                job.Delete();

                        String chaveURL = "siteURL";
                        String siteURL = web.Url;
                        
                        var timerJob = new TimerJobRelatoriosSuporteTerminais(webApp);
                        
                        //Remove a URL caso já exista
                        if (timerJob.Properties.ContainsKey(chaveURL))
                            timerJob.Properties.Remove(chaveURL);
                        timerJob.Properties.Add(chaveURL, siteURL);

#if DEBUG
                        SPMinuteSchedule schedule = new SPMinuteSchedule();
                        schedule.BeginSecond = 0;
                        schedule.Interval = 5;
                        timerJob.Schedule = schedule;
#else                        
                        SPDailySchedule schedule = new SPDailySchedule();
                        schedule.BeginHour = 4;
                        schedule.BeginMinute = 0;
                        schedule.BeginSecond = 0;                        
                        schedule.EndHour = 5;
                        schedule.EndMinute = 0;
                        schedule.EndSecond = 0;
                        timerJob.Schedule = schedule;
#endif

                        //Persistência do TimerJob
                        web.AllowUnsafeUpdates = true;
                        timerJob.Update();
                        web.AllowUnsafeUpdates = false;
                    });
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                }
            }
        }
             
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            using (Logger Log = Logger.IniciarLog("FeatureDeactivating"))
            {
                try
                {
                    SPWeb web = properties.Feature.Parent as SPWeb;                    
                    SPWebApplication webApp = web.Site.WebApplication;

                    //Remove o TimerJob
                    web.AllowUnsafeUpdates = true;
                    foreach (SPJobDefinition job in webApp.JobDefinitions)
                        if (job.Name == TimerJobRelatoriosSuporteTerminais.JobName)
                            job.Delete();
                    web.AllowUnsafeUpdates = false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw;
                }
            }
        }
    }
}
