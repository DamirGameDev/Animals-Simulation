using System;

namespace Modules.SharedModule.Runtime.Scripts.Configs
{
    /// <summary>
    /// Addressables key the config is loaded by.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigScopeAttribute : Attribute
    {
        public string Scope { get; }

        public ConfigScopeAttribute(string scope)
        {
            Scope = scope;
        }
    }
}
