using System.Reflection;
using Castle.MicroKernel.Registration;
using CluedIn.Core;
using CluedIn.Core.Providers;
using CluedIn.Core.Server;
using CluedIn.ExternalSearch.Providers.Gleif;
using ComponentHost;
using Constants = CluedIn.ExternalSearch.Providers.Gleif.Constants;

namespace CluedIn.Provider.Gleif
{
    [Component(Constants.ComponentName, "Providers", ComponentType.Service, ServerComponents.ProviderWebApi, Components.Server, Components.DataStores, Isolation = ComponentIsolation.NotIsolated)]
    public sealed class GleifProviderProviderComponent : ServiceApplicationComponent<IServer>
    {
        /**********************************************************************************************************
         * CONSTRUCTOR
         **********************************************************************************************************/

        /// <summary>
        /// Initializes a new instance of the <see cref="GleifProviderProviderComponent" /> class.
        /// </summary>
        /// <param name="componentInfo">The component information.</param>
        public GleifProviderProviderComponent(ComponentInfo componentInfo) : base(componentInfo)
        {
            // Dev. Note: Potential for compiler warning here ... CA2214: Do not call overridable methods in constructors
            //   this class has been sealed to prevent the CA2214 waring being raised by the compiler
            Container.Register(Component.For<GleifProviderProviderComponent>().Instance(this));
        }

        /**********************************************************************************************************
         * METHODS
         **********************************************************************************************************/

        /// <summary>Starts this instance.</summary>
        public override void Start()
        {
            var asm = Assembly.GetAssembly(typeof(GleifProviderProviderComponent));
            Container.Register(Types.FromAssembly(asm).BasedOn<IProvider>().WithServiceFromInterface().If(t => !t.IsAbstract).LifestyleSingleton());

            State = ServiceState.Started;
        }

        /// <summary>Stops this instance.</summary>
        public override void Stop()
        {
            if (State == ServiceState.Stopped)
                return;

            State = ServiceState.Stopped;
        }
    }
}
