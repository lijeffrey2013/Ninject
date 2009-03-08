#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using Ninject.Dynamic.Extensions;
using Ninject.Dynamic.Modules;
using Ninject.Events;
using Ninject.Infrastructure.Language;
using Ninject.Modules;

#endregion

namespace Ninject.Dynamic
{
    public class RubyKernel : StandardKernel
    {
        private readonly IDictionary<string, IModule> _rubyModules = new Dictionary<string, IModule>();

        public RubyKernel(params IModule[] modules) : base(modules)
        {
        }


        public RubyKernel(INinjectSettings settings, params IModule[] modules) : base(settings, modules)
        {
        }

        protected override void AddComponents()
        {
            base.AddComponents();
            Components.Add<IRubyEngine, RubyEngine>();
            Components.RemoveAll<IModuleLoader>();
            Components.Add<IModuleLoader, RubyModuleLoader>();
            Components.Add<IDecoratableModuleLoader, ModuleLoader>();
        }

        public void LoadAssemblies(params Type[] types)
        {
            var engine = Components.Get<IRubyEngine>();
            engine.LoadAssemblies(types);
        }

        public override void LoadModule(IModule module)
        {
            module.EnsureArgumentNotNull("module");

            if(module is RubyModule)
            {
                _rubyModules.Add(((RubyModule)module).ScriptPath, module);
                module.OnLoad(this);

                OnModuleLoaded(module);
            }
            else base.LoadModule(module);
        }


    }
}