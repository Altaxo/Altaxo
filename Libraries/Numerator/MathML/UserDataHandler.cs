using System;

namespace MathML
{
	/// <summary>
	/// Standard interface Introduced in DOM Level 3. This is currently not part
	/// of the ms xml implementation, but it is added here. In the future, when MS
	/// gets a standard compliant DOM implementation, this will go away.
	/// 
	/// When associating an object to a key on a node using Node.setUserData() [p.72] the
	/// application can provide a handler that gets called when the node the object is 
	/// associated to is being cloned, imported, or renamed. This can be used by the 
	/// application to implement various behaviors regarding the data it associates 
	/// to the DOM nodes. This interface defines that handler.
	/// </summary>																
	public delegate void UserDataHandler(UserDataOperation operation, String key, object data, 
		MathMLElement src, MathMLElement dst);

    /// <summary>
    /// operation type for UserDataHandler
    /// </summary>
	public enum UserDataOperation
	{
		NODE_CLONED = 1,
		NODE_IMPORTED = 2,
		NODE_DELETED = 3,
		NODE_RENAMED = 4,
		NODE_ADOPTED = 5
	}
}
