using System;
using MonoTouch.Foundation;
using System.Threading.Tasks;

namespace DropBoxSync.iOS
{
	public partial class DBFilesystem : NSObject
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
	}

	public partial class DBFile : NSObject
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

	public class DBException : Exception
	{
		public DBError NativeError { get; private set; }

		public DBException (DBError err)
		{
			NativeError = err;
		}

		public override string Message {
			get {
				switch (NativeError.Code) {
				case DBErrorCode.CoreSystem:
					return "System error";
					
				case DBErrorCode.Params:
					return "An error due to data passed into the SDK";
					
				case DBErrorCode.ParamsInvalid:
					return "A parameter is invalid, maybe was null?";
					
				case DBErrorCode.ParamsNotFound:
					return "A file corresponding to a provided path was not found";
					
				case DBErrorCode.ParamsExists:
					return "File already exists and was opened exclusively";
					
				case DBErrorCode.ParamsAlreadyOpen:
					return "File was already open";
					
				case DBErrorCode.ParamsParent:
					return "Parent does not exist or is not a folder";
					
				case DBErrorCode.ParamsNotEmpty:
					return "Directory is not empty";
					
				case DBErrorCode.ParamsNotCached:
					return "File was not yet in cache";
					
				case DBErrorCode.System:
					return "An error in the Dropbox library occurred";
					
				case DBErrorCode.SystemDiskSpace:
					return "An error happened due to insufficient disk space";
					
				case DBErrorCode.Network:
					return "An error occurred making a network request";
					
				case DBErrorCode.NetworkTimeout:
					return "A connection timed out";
					
				case DBErrorCode.NetworkNoConnection:
					return "No network connection available";
					
				case DBErrorCode.NetworkSSL:
					return "Unable to verify the server's SSL certificate. Often caused by an out-of-date clock";
					
				case DBErrorCode.NetworkServer:
					return "Unexpected server error";
					
				case DBErrorCode.Auth:
					return "An authentication related problem occurred";
					
				case DBErrorCode.AuthUnlinked:
					return "The user is no longer linked";
					
				case DBErrorCode.AuthInvalidApp:
					return "An invalid app key or secret was provided";
					
				case DBErrorCode.Unknown:
				default:
					return "Unknown Error";
					
				}
			}
		}
	}
}

