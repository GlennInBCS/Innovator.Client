using System;
using System.Collections.Generic;
using System.Linq;
#if SERIALIZATION
using System.Runtime.Serialization;
#endif

namespace Innovator.Client
{
  /// <summary>
  /// Represents an exception that was returned from the server as a SOAP fault 
  /// containing information about properties being validated
  /// </summary>
  /// <remarks>
  /// To create a new instance of this class, use <see cref="ElementFactory.ValidationException(string, IReadOnlyItem, string[])"/>
  /// or one of the other overloads
  /// </remarks>
#if SERIALIZATION
  [Serializable]
#endif
  public class ValidationException : ServerException
  {
    /// <summary>
    /// Gets the item being validated
    /// </summary>
    public IReadOnlyItem Item
    {
      get
      {
        var item = _fault.ElementByName("detail").ElementByName("item");
        if (!item.Exists)
          return Innovator.Client.Item.GetNullItem<IReadOnlyItem>();
        var aml = _fault.AmlContext;
        return aml.Item(aml.Type(item.Attribute("type").Value), aml.Id(item.Attribute("id").Value));
      }
    }
    /// <summary>
    /// Gets the properties for which there were validation errors
    /// </summary>
    public IEnumerable<string> Properties
    {
      get
      {
        return _fault.ElementByName("detail").ElementByName("properties")
                     .Elements().Select(e => e.Value);
      }
    }

    internal ValidationException(string message
      , IReadOnlyItem item, params string[] properties)
      : base(message, properties.Any() ? "1001" : "1")
    {
      CreateDetailElement(item, properties);
    }
    internal ValidationException(string message, Exception innerException
      , IReadOnlyItem item, params string[] properties)
      : base(message, properties.Any() ? "1001" : "1", innerException, false)
    {
      CreateDetailElement(item, properties);
    }
#if SERIALIZATION
    public ValidationException(SerializationInfo info, StreamingContext context)
      : base(info, context) { }
#endif
    internal ValidationException(Element fault, string database, Command query)
      : base(fault, database, query) { }

    private IElement CreateDetailElement(IReadOnlyItem item, params string[] properties)
    {
      var detail = _fault.ElementByName("detail");
      detail.Add(new AmlElement(_fault.AmlContext, "item"
        , new Attribute("type", item.Type().Value)
        , new Attribute("id", item.Id())));
      if (properties.Any())
      {
        var props = new AmlElement(_fault.AmlContext, "properties");
        foreach (var prop in properties)
        {
          props.Add(new AmlElement(_fault.AmlContext, "property", prop));
        }
        detail.Add(props);
      }
      if (!detail.Exists)
        _fault.Add(detail);
      return detail;
    }
  }
}
