using System;

namespace DropBoxSync.iOS
{
	[Flags]
	public enum DBDatastoreStatus : uint
	{
		Connected = 1 << 0, // The API is connected with the server
		Downloading = 1 << 1, // Changes are currently downloading
		Uploading = 1 << 2, // Changes are currently uploading
		Incoming = 1 << 3, // There are remote changes waiting to be synced
		Outgoing = 1 << 4 // There are local changes waiting to be synced
	}

	public enum DBErrorCode
	{
		Unknown = 0,

		CoreSystem = 1, // System error, out of memory, etc

		Params = 2000, // An error due to data passed into the API
		ParamsInvalid, // A parameter is invalid, such as a nil object
		ParamsNotFound, // A file corresponding to a provided path was not found
		ParamsExists, // File already exists and was opened exclusively
		ParamsAlreadyOpen, // File was already open
		ParamsParent, // Parent does not exist or is not a folder
		ParamsNotEmpty, // Directory is not empty
		ParamsNotCached, // File was not yet in cache
		ParamsDisallowed, // App is not allowed to perform this operation
		ParamsNoThumb, // No thumbnail is available
		ParamsIndex, // Index is out of bounds
		ParamsType, // Value is of the wrong type

		System = 3000, // An error in the library occurred
		SystemDiskSpace, // An error happened due to insufficient local disk space

		Network = 4000, // An error occurred making a network request
		NetworkTimeout, // A connection timed out
		NetworkNoConnection, // No network connection available
		NetworkSSL, // Unable to verify the server's SSL certificate. Often caused by an out-of-date clock
		NetworkServer, // Unexpected server error

		NetworkQuota = 4006, // The user's Dropbox space is full

		Auth = 5000, // An authentication related problem occurred
		AuthUnlinked, // The user is no longer linked
		AuthInvalidApp, // An invalid app key or secret was provided
	}

	public enum DBFileState
	{
		Downloading,
		Idle,
		Uploading,
	}

	[Flags]
	public enum DBSyncStatus : uint
	{
		Downloading = 1,
		Uploading = 2,
		Syncing = 4,
		Active = 8
	}

	public enum DBThumbSize
	{
		XS,
		S,
		M,
		L,
		XL
	}

	public enum DBThumbFormat
	{
		Jpg,
		Png
	}

	public enum DBResolutionRule
	{
		Remote, // Resolves conflicts by always taking the remote change. This is the
				// default resolution strategy.
		Local, 	// Resolves conflicts by always taking the local change.
		Max, 	// Resolves conflicts by taking the largest value, based on type-specific
				// ordering.
		Min, 	// Resolves conflicts by taking the smallest value, based on type-specific
				// ordering.
		Sum 	// Resolves conflicts by preserving additions or subtractions to a numeritcal
				// value, which allows you to treat it as a counter or accumulator without
				// losing updates. For non-numerical values this rule behaves as
				// DBResolutionRemote.
	}
}

