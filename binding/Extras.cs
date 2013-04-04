using System;
using MonoTouch.Foundation;
using System.Threading.Tasks;

namespace DropBoxSync.iOS
{
	public partial class DBFilesystem : NSObject
	{
		public Task<DBFileInfo []> ListFolderAsync (DBPath path)
		{
			DBError err;
			return Task<DBFileInfo []>.Factory.StartNew (() => ListFolder (path, out err)); 
		}

		public Task<DBFileInfo> FileInfoForPathAsync (DBPath path)
		{
			DBError err;
			return Task<DBFileInfo>.Factory.StartNew (() => FileInfoForPath (path, out err)); 
		}

		public Task<DBFile> OpenFileAsync (DBPath path)
		{
			DBError err;
			return Task<DBFile>.Factory.StartNew (() => OpenFile (path, out err)); 
		}

		public Task<DBFile> CreateFileAsync (DBPath path)
		{
			DBError err;
			return Task<DBFile>.Factory.StartNew (() => CreateFile (path, out err)); 
		}

		public Task<bool> CreateFolderAsync (DBPath path)
		{
			DBError err;
			return Task<bool>.Factory.StartNew (() => CreateFolder (path, out err)); 
		}

		public Task<bool> DeletePathAsync (DBPath path)
		{
			DBError err;
			return Task<bool>.Factory.StartNew (() => DeletePath (path, out err)); 
		}

		public Task<bool> MovePathAsync (DBPath fromPath, DBPath toPath)
		{
			DBError err;
			return Task<bool>.Factory.StartNew (() => MovePath (fromPath, toPath, out err)); 
		}
	}

	public partial class DBFile : NSObject
	{
		public Task<NSFileHandle> ReadHandleAsync ()
		{
			DBError err;
			return Task<NSFileHandle>.Factory.StartNew (() => ReadHandle (out err)); 
		}

		public Task<NSData> ReadDataAsync ()
		{
			DBError err;
			return Task<NSData>.Factory.StartNew (() => ReadData (out err)); 
		}

		public Task<string> ReadStringAsync ()
		{
			DBError err;
			return Task<string>.Factory.StartNew (() => ReadString (out err)); 
		}

		public Task<bool> WriteContentsOfFileAsync (string localPath, bool shouldSteal)
		{
			DBError err;
			return Task<bool>.Factory.StartNew (() => WriteContentsOfFile (localPath, shouldSteal, out err)); 
		}

		public Task<bool> WriteDataAsync (NSData data)
		{
			DBError err;
			return Task<bool>.Factory.StartNew (() => WriteData (data, out err)); 
		}

		public Task<bool> WriteStringAsync (string aString)
		{
			DBError err;
			return Task<bool>.Factory.StartNew (() => WriteString (aString, out err)); 
		}

		public Task<bool> UpdateAsync ()
		{
			DBError err;
			return Task<bool>.Factory.StartNew (() => Update (out err)); 
		}
	}
}

