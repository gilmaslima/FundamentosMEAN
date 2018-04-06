using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Redecard.PN.Comum
{
    public static class ClassFactory
    {
        private static UnityContainer container;

        private static UnityContainer Container
        {
            get
            {
                if (container == null)
                {
                    container = new UnityContainer();
                    container.AddNewExtension<Interception>();
                }
                return container;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <returns></returns>
        public static T GetInstance<T, K>() where K : T
        {
            //Singleton
            return GetInstance<T, K>(new ContainerControlledLifetimeManager());
        }

        public static T GetInstance<T, K>(LifetimeManager lifeTimeManager) where K : T
        {
            if (!Container.IsRegistered<T>())
            {                
                Container.RegisterType<T, K>(
                    lifeTimeManager,
                    new InjectionConstructor(),
                    new InterceptionBehavior<LoggingBehavior>())
                    .Configure<Interception>()
                    .SetInterceptorFor<T>(new InterfaceInterceptor());
            }
            return Container.Resolve<T>();
        }
    }
}