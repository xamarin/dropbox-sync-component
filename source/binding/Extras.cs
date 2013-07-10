using System;
using MonoTouch.Foundation;
using System.Threading.Tasks;

namespace DropBoxSync.iOS
{
	public partial class DBDatastore : NSObject, IDisposable
	{
		public static Task<DBDatastore> OpenDefaultStoreForAccountAsync (DBAccount account)
		{
			return Task.Factory.StartNew (p => {
				DBError err;
				var results = OpenDefaultStoreForAccount ((DBAccount)p, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			}, account);
		}

		public Task<NSDictionary> SyncAsync (DBError error)
		{
			return Task.Factory.StartNew (p => {
				var results = Sync ((DBError)p);
				return results;
			}, error);
		}

	}

	public partial class DBDatastoreManager : NSObject, IDisposable
	{
		public Task<DBDatastore> OpenDefaultDatastoreAsync ()
		{
			return Task.Factory.StartNew (()=> {
				DBError err;
				var results = OpenDefaultDatastore (out err);
				if (err != null)
					throw new DBException (err);
				return results;
			});
		}

		public Task<string []> ListDatastoreIdsAsync ()
		{
			return Task.Factory.StartNew (()=> {
				DBError err;
				var results = ListDatastoreIds (out err);
				if (err != null)
					throw new DBException (err);
				return results;
			});
		}

		public Task<DBDatastore> OpenDatastoreAsync (string datastoreId)
		{
			return Task.Factory.StartNew (p => {
				DBError err;
				var results = OpenDatastore ((string)p, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			}, datastoreId);
		}

		public Task<DBDatastore> CreateDatastoreAsync ()
		{
			return Task.Factory.StartNew (()=> {
				DBError err;
				var results = CreateDatastore (out err);
				if (err != null)
					throw new DBException (err);
				return results;
			});
		}

		public Task<bool> DeleteDatastoreAsync (string datastoreId)
		{
			return Task.Factory.StartNew (p => {
				DBError err;
				var results = DeleteDatastore ((string)p, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			}, datastoreId);
		}
	}

	public partial class DBFilesystem : NSObject, IDisposable
	{
		public Task<DBFileInfo []> ListFolderAsync (DBPath path)
		{
			return Task.Factory.StartNew (p => {
				DBError err;
				var results = ListFolder ((DBPath)p, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			}, path);
		}

		public Task<DBFileInfo> FileInfoForPathAsync (DBPath path)
		{
			return Task.Factory.StartNew (p => {
				DBError err;
				var results = FileInfoForPath ((DBPath)p, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			}, path);
		}

		public Task<DBFile> OpenFileAsync (DBPath path)
		{
			return Task.Factory.StartNew (p => {
				DBError err;
				var results = OpenFile ((DBPath)p, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			}, path);
		}

		public Task<DBFile> CreateFileAsync (DBPath path)
		{
			return Task.Factory.StartNew (p => {
				DBError err;
				var results = CreateFile ((DBPath)p, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			}, path);
		}

		public Task<DBFile> OpenThumbnailAsync (DBPath path, DBThumbSize size, DBThumbFormat format)
		{
			return Task<DBFile>.Factory.StartNew (() => {
				DBError err;
				var results = OpenThumbnail (path, size, format, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			});
		}

		public Task<bool> CreateFolderAsync (DBPath path)
		{
			return Task.Factory.StartNew (p => {
				DBError err;
				var results = CreateFolder ((DBPath)p, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			}, path);
		}

		public Task<bool> DeletePathAsync (DBPath path)
		{
			return Task.Factory.StartNew (p => {
				DBError err;
				var results = DeletePath ((DBPath)p, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			}, path);
		}

		public Task<bool> MovePathAsync (DBPath fromPath, DBPath toPath)
		{
			return Task<bool>.Factory.StartNew (() => {
				DBError err;
				var results = MovePath (fromPath, toPath, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			});
		}

		public Task<string> FetchShareLinkAsync (DBPath path, bool shorten)
		{
			return Task<string>.Factory.StartNew (() => {
				DBError err;
				var results = FetchShareLink (path, shorten, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			});
		}
	}

	public partial class DBFile : NSObject, IDisposable
	{
		public Task<NSFileHandle> ReadHandleAsync ()
		{
			return Task.Factory.StartNew (()=> {
				DBError err;
				var results = ReadHandle (out err);
				if (err != null)
					throw new DBException (err);
				return results;
			});
		}

		public Task<NSData> ReadDataAsync ()
		{
			return Task.Factory.StartNew (()=> {
				DBError err;
				var results = ReadData (out err);
				if (err != null)
					throw new DBException (err);
				return results;
			});
		}

		public Task<string> ReadStringAsync ()
		{
			return Task.Factory.StartNew (()=> {
				DBError err;
				var results = ReadString (out err);
				if (err != null)
					throw new DBException (err);
				return results;
			});
		}

		public Task<bool> WriteContentsOfFileAsync (string localPath, bool shouldSteal)
		{
			return Task.Factory.StartNew (()=> {
				DBError err;
				var results = WriteContentsOfFile (localPath, shouldSteal, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			});
		}

		public Task<bool> WriteDataAsync (NSData data)
		{
			return Task.Factory.StartNew (p => {
				DBError err;
				var results = WriteData ((NSData)p, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			}, data);
		}

		public Task<bool> WriteStringAsync (string aString)
		{
			return Task.Factory.StartNew (p => {
				DBError err;
				var results = WriteString ((string)p, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			}, aString);
		}

		public Task<bool> UpdateAsync ()
		{
			return Task.Factory.StartNew (() => {
				DBError err;
				var results = Update (out err);
				if (err != null)
					throw new DBException (err);
				return results;
			}); 
		}
	}

	public partial class DBTable : NSObject, IDisposable
	{
		public Task<DBRecord []> QueryAsync (NSDictionary filter)
		{
			return Task.Factory.StartNew (p => {
				DBError err;
				var results = Query ((NSDictionary)p, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			}, filter);
		}

		public Task<DBRecord> GetRecordAsync (string recordId)
		{
			return Task.Factory.StartNew (p => {
				DBError err;
				var results = GetRecord ((string)p, out err);
				if (err != null)
					throw new DBException (err);
				return results;
			}, recordId);
		}

		public Task<DBRecord> GetOrInsertRecordAsync (string recordId, NSDictionary fields, bool inserted, DBError error)
		{
			return Task<DBRecord>.Factory.StartNew (() => {
				var results = GetOrInsertRecord (recordId, fields, inserted, error);
				if (error != null)
					throw new DBException (error);
				return results;
			});
		}
	}

	public partial class DBRecord : NSObject, IDisposable
	{
		public NSObject this [string key]
		{
			get{ return this.ObjectForKey (key);}
			set{ this.SetObject (value,key);}
		}
	}

	public partial class DBAccount : NSObject, IDisposable
	{
	}

	public partial class DBAccountManager : NSObject, IDisposable
	{
	}

	public partial class DBFileInfo : NSObject, IDisposable
	{
	}

	public partial class DBFileStatus : NSObject, IDisposable
	{
	}

	public partial class DBPath : NSObject, IDisposable
	{
	}

	public partial class DBAccountInfo : NSObject, IDisposable
	{
	}
}

