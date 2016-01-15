using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TinyIoC;

namespace MFilesImporter
{
  public static class TinyIoCExtensions
  {

    [Flags]
    public enum RegisterTypes
    {
      AsInterfaceTypes = 1,
      AsObjects = 2
    }

    [Flags]
    public enum RegisterOptions
    {
      AsMultiInstance = 1,
      AsSingleton = 2
    }

    public static IEnumerable<Type> GetInterfacesFromNamespace(string @namespace, Assembly[] assemblies = null)
    {
      if (assemblies == null)
        assemblies = new[] { Assembly.GetExecutingAssembly() };

      var interfaces = new List<Type>();
      foreach (var a in assemblies)
        foreach (var t in a.DefinedTypes)
        {
          if (t.IsInterface && t.Namespace == @namespace)
            interfaces.Add(t);
        }
      return interfaces;
    }

    private static Type GetMostDerivedInterface(Type[] types)
    {
      Type mostDerived = null;
      int mostDerivedInterfaceCount = 0;
      foreach (var t in types)
      {
        int interfaceCount = t.GetInterfaces().Length;
        if (mostDerived == null)
        {
          mostDerived = t;
          mostDerivedInterfaceCount = interfaceCount;
        }
        else if (interfaceCount > mostDerivedInterfaceCount)
        {
          mostDerived = t;
          mostDerivedInterfaceCount = interfaceCount;
        }
      }
      return mostDerived;
    }

    public static Type GetImplementingType(Type @interface, Assembly[] assemblies = null)
    {
      if (assemblies == null)
        assemblies = new[] { Assembly.GetExecutingAssembly() };

      foreach (var a in assemblies)
        foreach (var t in a.SafeGetTypes())
        {
          if (t.IsClass && !t.IsAbstract)
          {
            var i = GetMostDerivedInterface(t.GetInterfaces());

            if (i != null && i.FullName == @interface.FullName)
              return t;
          }
        }

      return null;
    }

    public static void RegisterInterfaceImplementations(this TinyIoCContainer container,
                                                         string @namespace,
                                                         RegisterOptions options = RegisterOptions.AsMultiInstance,
                                                         RegisterTypes registerTypes = RegisterTypes.AsInterfaceTypes,
                                                         Assembly[] assemblies = null)
    {
      if (assemblies == null)
        assemblies = new [] { Assembly.GetExecutingAssembly() };

      var interfaces = GetInterfacesFromNamespace(@namespace, assemblies);

      foreach (var i in interfaces)
      {
        var implementation = GetImplementingType(i, assemblies);
        if (implementation != null)
        {
          if ((registerTypes & RegisterTypes.AsObjects) == RegisterTypes.AsObjects)
          {
            var o = container.Register(typeof(object), implementation, i.Name);
            if (options == RegisterOptions.AsSingleton)
              o.AsSingleton();
            else
              o.AsMultiInstance();
          }
          if ((registerTypes & RegisterTypes.AsInterfaceTypes) == RegisterTypes.AsInterfaceTypes)
          {
            var o = container.Register(i, implementation);
            if (options == RegisterOptions.AsSingleton)
              o.AsSingleton();
            else
              o.AsMultiInstance();
          }
        }
      }

    }
   
  }
}
