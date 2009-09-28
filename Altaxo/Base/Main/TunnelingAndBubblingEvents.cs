using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
   public class BubblingEventArgs : EventArgs
    {
    }

   

  public interface ISupportsTunnelingAndBubblingEvents
  {
   
    /// <summary>
    /// Informs about an event that bubbles from a child node up to the root node.
    /// The handler should send this event up to the parent node.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void EhBubblingEvent(object sender, BubblingEventArgs e);

    /// <summary>
    /// Informs about an event that tunnels from a root node down to all child nodes. The handler
    /// should enumerate over all child nodes and send the event down to them.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void EhTunnelingEvent(object sender, TunnelingEventArgs e);

   // event Action<object, BubblingEventArgs> BubblingEvent;
   // event Action<object, TunnelingEventArgs> TunnelingEvent;
  }


  public class TunnelingEventArgs 
  {
  }

  public class PreviewDisposeEventArgs : TunnelingEventArgs
  {
    public static readonly PreviewDisposeEventArgs Empty = new PreviewDisposeEventArgs();
    private PreviewDisposeEventArgs() { }
  }

	public class DisposeEventArgs : TunnelingEventArgs
	{
    public static readonly DisposeEventArgs Empty = new DisposeEventArgs();
    private DisposeEventArgs() { }
	}

	public class DocumentPathChangedEventArgs : TunnelingEventArgs
	{
		public static readonly DocumentPathChangedEventArgs Empty = new DocumentPathChangedEventArgs();
    private DocumentPathChangedEventArgs() { }
	}

}
