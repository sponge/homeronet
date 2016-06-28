using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Homero.Plugin;
using Ninject;

namespace Homero.Core.Manager.Assembly
{
    public class PluginAppDomain
    {
        private AppDomain _internalAppDomain;
        private IKernel _kernel;
        private AppDomain _parentAppDomain;
        private string _pluginPath;

        public PluginAppDomain(string pluginFilename, IKernel kernel)
        {
            _kernel = kernel;
            var domainSetup = new AppDomainSetup
            {
                ShadowCopyFiles = "true" // lmao
            };
            _internalAppDomain = AppDomain.CreateDomain(pluginFilename, AppDomain.CurrentDomain.Evidence, domainSetup);
            _parentAppDomain = AppDomain.CurrentDomain;
//            _internalAppDomain.AssemblyResolve += InternalAppDomainOnAssemblyResolve;
            Load();
        }

        private System.Reflection.Assembly InternalAppDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return _parentAppDomain.GetAssemblies().FirstOrDefault(x => x.FullName == args.RequestingAssembly.FullName);
        }

        public void Load()
        {
            var pluginAssembly = System.Reflection.Assembly.Load(File.ReadAllBytes(_pluginPath));
            _kernel.Load(pluginAssembly);
        }

        public void Unload()
        {
            // Shutdown all plugins first.
            var plugins = new List<Type>();
            foreach (var assembly in _internalAppDomain.GetAssemblies())
            {
                plugins = plugins.Concat(GetPlugins(assembly)).ToList();
            }

            foreach (var pluginType in plugins)
            {
                try
                {
                    var plugin = _kernel.Get(pluginType) as IPlugin;
                    plugin?.Shutdown();
                    _kernel.Unbind(pluginType);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("shutdown error");
                    Debug.WriteLine(e);
                }
            }

            AppDomain.Unload(_internalAppDomain);
        }

        public void Reload()
        {
            Unload();
            Load();
        }

        private List<Type> GetPlugins(System.Reflection.Assembly assembly)
        {
            return assembly.GetTypes().Where(t => t.BaseType == typeof(IPlugin)).ToList();
        }
    }
}