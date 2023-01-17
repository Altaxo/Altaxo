using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Altaxo.Serialization.Xml
{

  public class XmlSerializationVersionTest
  {
    private record AssemblyNameVersion(string Assembly, string Name, int Version)
    {
    }


    [Fact]
    public void TestAltaxoCore()
    {
      TestAssembly(typeof(Altaxo.Geometry.PointD2D).Assembly);
    }

    [Fact]
    public void TestAltaxoBase()
    {
      TestAssembly(typeof(Altaxo.Data.DataTable).Assembly);
    }
    [Fact]
    public void TestAltaxoDom()
    {
      TestAssembly(typeof(Altaxo.Main.GraphExportBindingDoozer).Assembly);
    }

    private static void TestAssembly(Assembly ass)
    {
      var versionHash = new Dictionary<AssemblyNameVersion, XmlSerializationSurrogateForAttribute>();
      var allTypes = ass.GetTypes();
      foreach (var type in allTypes)
      {
        var attributes = type.GetCustomAttributes().OfType<XmlSerializationSurrogateForAttribute>();

        // erstes Kriterium:
        // nur eines der Attribute darf den SerializationTyp enthalten
        // alle Attribute müssen verschieden voneinander sein

        versionHash.Clear();
        XmlSerializationSurrogateForAttribute versionOfTypeAttribute = null;

        foreach (var attribute in attributes)
        {
          var assnv = new AssemblyNameVersion(attribute.AssemblyName, attribute.TypeName, attribute.Version);
          Assert.True(attribute.Version >= 0);

          if (attribute.SerializationType is not null)
          {
            // there should exist only one attribute for which SerializationType is not null
            Assert.Null(versionOfTypeAttribute);
            versionOfTypeAttribute = attribute;
          }
          else
          {
            // for attributes for which SerializationType is null
            // AssemblyName and TypeName must be given
            Assert.False(string.IsNullOrEmpty(attribute.AssemblyName));
            Assert.False(string.IsNullOrEmpty(attribute.TypeName));

            // for attributes for which SerializationType is null
            // each attribute must be distinct from the other
            Assert.DoesNotContain(assnv, versionHash.Keys);
            versionHash.Add(assnv, attribute);
          }
        }

        if (versionOfTypeAttribute is not null)
        {
          // if there exist a attribute to serialize the current version (for which SerializationType is not null),
          // then this attribute should have an equal or a higher version number than the existing attributes
          // with the same assembly name and type name
          Assert.Null(versionHash.Keys.FirstOrDefault(e => e.Assembly == versionOfTypeAttribute.AssemblyName && e.Name == versionOfTypeAttribute.TypeName && e.Version > versionOfTypeAttribute.Version));
        }
      }
    }

  }
}
