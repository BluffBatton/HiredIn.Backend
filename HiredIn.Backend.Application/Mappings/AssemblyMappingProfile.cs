using AutoMapper;
using System.Reflection;

namespace HiredIn.Backend.Application.Mappings
{
    public class AssemblyMappingProfileLoader
    {
        public static void ApplyMappingsFromAssembly(IMapperConfigurationExpression config, Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(type =>
                    type.IsClass &&
                    !type.IsAbstract &&
                    typeof(Profile).IsAssignableFrom(type) &&
                    type != typeof(AssemblyMappingProfileLoader))
                .ToList();

            foreach (var type in types)
            {
                if (Activator.CreateInstance(type) is Profile profile)
                {
                    config.AddProfile(profile);
                }

                var configureMethod = type.GetMethod("Configure");
                if (configureMethod != null)
                {
                    var instance = Activator.CreateInstance(type);
                    configureMethod.Invoke(instance, new object[] { config });
                }
            }
        }
    }
}
