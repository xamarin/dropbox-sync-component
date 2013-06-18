using System;

namespace DropBoxSync.iOS
{
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

