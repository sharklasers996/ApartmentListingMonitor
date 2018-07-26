using System;
using System.Linq;
using System.Reflection;

namespace AparmentListingMonitor
{
    public static class PluginHelper
    {
        public static IApartmentListingWebsitePlugin GetPlugin(string url)
        {
            var host = new Uri(url).Host;

            var assemblyExportedTypes = Assembly.GetExecutingAssembly().GetExportedTypes();
            foreach (var type in assemblyExportedTypes)
            {
                var typeInterfaces = type.GetInterfaces();
                if (typeInterfaces.FirstOrDefault(x => x == typeof(IApartmentListingWebsitePlugin)) != null)
                {
                    var ctorInfo = type.GetConstructor(Type.EmptyTypes);
                    var plugin = (IApartmentListingWebsitePlugin)ctorInfo.Invoke(null);
                    if (plugin.Host == host)
                    {
                        return plugin;
                    }
                }
            }

            return null;
        }
    }
}