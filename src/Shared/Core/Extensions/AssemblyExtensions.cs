using System.Reflection;

namespace Shared.Core.Extensions;

public static class AssemblyExtensions
{
    /// <summary>
    /// Finds all assemblies that reference the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to check for references.</param>
    /// <returns>An enumerable of assemblies that reference the specified assembly.</returns>
    public static IEnumerable<Assembly> GetReferencingAssemblies(this Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        // Get all loaded assemblies in the current AppDomain
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Find and return assemblies that reference the specified assembly
        return loadedAssemblies.Where(a =>
            a.GetReferencedAssemblies().Any(referenced => referenced.FullName == assembly.FullName)
        );
    }

    // ref: https://stackoverflow.com/a/7889272/581476
    // https://haacked.com/archive/2012/07/23/get-all-types-in-an-assembly.aspx/
    public static IEnumerable<Type?> GetLoadableTypes(this Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t != null);
        }
    }
}
