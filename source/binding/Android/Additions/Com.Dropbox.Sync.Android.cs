using System;
namespace DropboxSync.Android
{
	public sealed partial class DBPath
	{
		public int CompareTo (Java.Lang.Object target)
		{
			return this.CompareTo ((DBPath) target);
		}
	}
	public sealed partial class DBFileInfo
	{
		public int CompareTo (Java.Lang.Object target)
		{
			return this.CompareTo ((DBPath) target);
		}
	}
}

