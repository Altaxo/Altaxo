using Altaxo.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Units
{
  public class UserDefinedUnitEnvironment
  {
    private string _name;

    private string _quantity;

    private QuantityWithUnitGuiEnvironment _environment;

    #region Serialization

    /// <summary>
    /// 2017-09-26 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(UserDefinedUnitEnvironment), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (UserDefinedUnitEnvironment)obj;
        info.AddValue("Name", s.Name);
        info.AddValue("Quantity", s.Quantity);
        info.AddValue("Environment", s.Environment);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var name = info.GetString("Name");
        var quantity = info.GetString("Quantity");
        var env = (QuantityWithUnitGuiEnvironment)info.GetValue("Environment", null);

        return new UserDefinedUnitEnvironment(name, quantity, env);
      }
    }

    #endregion Serialization

    public string Name { get { return _name; } }

    public string Quantity { get { return _quantity; } }

    public QuantityWithUnitGuiEnvironment Environment { get { return _environment; } }

    public UserDefinedUnitEnvironment(string environmentName, string quantityName, QuantityWithUnitGuiEnvironment environment)
    {
      _name = environmentName;
      _quantity = quantityName;
      _environment = environment;
    }
  }
}
