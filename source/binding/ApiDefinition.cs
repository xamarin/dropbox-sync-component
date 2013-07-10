using System;
using System.Drawing;

using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace DropBoxSync.iOS
{
	delegate void DBObserverHandler ();
	delegate void DBAccountManagerObserverHandler (DBAccount account);

	[BaseType (typeof (NSObject))]
	interface DBAccount {

		[Export ("unlink")]
		void Unlink ();

		[Export ("userId")]
		string UserId { get; }

		[Export ("linked")]
		bool Linked { [Bind ("isLinked")] get; }

		[Export ("info")]
		DBAccountInfo Info { get; }

		[Export ("addObserver:block:")]
		void AddObserver (NSObject observer, DBObserverHandler handler);

		[Export ("removeObserver:")]
		void RemoveObserver (NSObject observer);
	}

	[BaseType (typeof (NSObject))]
	interface DBAccountInfo {

		[Export ("displayName")]
		string DisplayName { get; }

		[Export ("userName")]
		string UserName { get; }

		[Export ("orgName")]
		string OrgName { get; }
	}

	[BaseType (typeof (NSObject))]
	interface DBAccountManager {

		[Export ("initWithAppKey:secret:")]
		IntPtr Constructor (string key, string secret);

		[Static, Export ("sharedManager")]
		DBAccountManager SharedManager { get; set; }

		[Export ("linkFromController:")]
		void LinkFromController (UIViewController rootController);

		[Export ("handleOpenURL:")]
		DBAccount HandleOpenURL (NSUrl url);

		[Export ("linkedAccount")]
		DBAccount LinkedAccount { get; }

		[Export ("linkedAccounts")]
		DBAccount [] LinkedAccounts { get; }

		[Export ("addObserver:block:")]
		void AddObserver (NSObject observer, DBAccountManagerObserverHandler handler);
		
		[Export ("removeObserver:")]
		void RemoveObserver (NSObject observer);
	}

	[BaseType (typeof (NSObject))]
	interface DBDatastore {

		[Static]
		[Export ("openDefaultStoreForAccount:error:")]
		DBDatastore OpenDefaultStoreForAccount (DBAccount account, out DBError error);

		[Export ("close")]
		void Close ();

		[Export ("getTables:")]
		DBTable [] GetTables (out DBError error);

		[Export ("getTable:")]
		DBTable GetTable (string tableId);

		[Export ("sync:")]
		NSDictionary Sync ([NullAllowed] DBError error);

		[Export ("open")]
		bool Open { [Bind ("isOpen")] get; }

		[Export ("status")]
		DBDatastoreStatus Status { get; }

		[Export ("addObserver:block:")]
		void AddObserver (NSObject observer, DBObserverHandler handler);

		[Export ("removeObserver:")]
		void RemoveObserver (NSObject observer);

		[Export ("datastoreId")]
		string DatastoreId { get; }

		[Export ("manager")]
		DBDatastoreManager Manager { get; }
	}

	[BaseType (typeof (NSObject))]
	interface DBDatastoreManager {

		[Static, Export ("managerForAccount:")]
		DBDatastoreManager ManagerForAccount (DBAccount account);

		[Export ("openDefaultDatastore:")]
		DBDatastore OpenDefaultDatastore (out DBError error);

		[Export ("listDatastoreIds:")]
		string [] ListDatastoreIds (out DBError error);

		[Export ("openDatastore:error:")]
		DBDatastore OpenDatastore (string datastoreId, out DBError error);

		[Export ("createDatastore:")]
		DBDatastore CreateDatastore (out DBError error);

		[Export ("deleteDatastore:error:")]
		bool DeleteDatastore (string datastoreId, out DBError error);

		[Export ("addObserver:block:")]
		void AddObserver (NSObject obj, DBObserverHandler block);

		[Export ("removeObserver:")]
		void RemoveObserver (NSObject obj);

		[Export ("shutDown")]
		void ShutDown ();

		[Export ("isShutDown")]
		bool IsShutDown { get; }
	}

	[BaseType (typeof (NSError))]
	interface DBError {

		[Export ("code")] [New]
		DBErrorCode Code { get; }
	}

	[BaseType (typeof (NSObject))]
	interface DBFile {

		[Export ("readHandle:")]
		NSFileHandle ReadHandle (out DBError error);

		[Export ("readData:")]
		NSData ReadData (out DBError error);

		[Export ("readString:")]
		string ReadString (out DBError error);

		[Export ("writeContentsOfFile:shouldSteal:error:")]
		bool WriteContentsOfFile (string localPath, bool shouldSteal, out DBError error);

		[Export ("writeData:error:")]
		bool WriteData (NSData data, out DBError error);

		[Export ("writeString:error:")]
		bool WriteString (string aString, out DBError error);

		[Export ("appendData:error:")] // Async
		bool AppendData (NSData data, out DBError error);

		[Export ("appendString:error:")] // Async
		bool AppendString (string aString, out DBError error);

		[Export ("update:")]
		bool Update (out DBError error);

		[Export ("close")]
		bool Close ();

		[Export ("info")]
		DBFileInfo Info { get; }

		[Export ("open")]
		bool Open { [Bind ("isOpen")] get; }

		[Export ("status")]
		DBFileStatus Status { get; }

		[Export ("newerStatus")]
		DBFileStatus NewerStatus { get; }

		[Export ("isThumb")]
		bool IsThumb { get; }

		[Export ("addObserver:block:")]
		void AddObserver (NSObject observer, DBObserverHandler handler);
		
		[Export ("removeObserver:")]
		void RemoveObserver (NSObject observer);
	}

	[BaseType (typeof (NSObject))]
	interface DBFileInfo {
		
		[Export ("path")]
		DBPath Path { get; }

		[Export ("isFolder")]
		bool IsFolder { get; }

		[Export ("modifiedTime")]
		NSDate ModifiedTime { get; }

		[Export ("size")]
		long Size { get; }

		[Export ("thumbExists")]
		bool ThumbExists { get; }

		[Export ("iconName")]
		string IconName { get; }
	}

	[BaseType (typeof (NSObject))]
	interface DBFileStatus {
		
		[Export ("cached")]
		bool Cached { get; }
		
		[Export ("state")]
		DBFileState State { get; }
		
		[Export ("progress")]
		float Progress { get; }
		
		[Export ("error")]
		DBError Error { get; }
	}

	[BaseType (typeof (NSObject))]
	interface DBFilesystem {
		
		[Export ("initWithAccount:")]
		IntPtr Constructor (DBAccount account);

		[Static, Export ("sharedFilesystem")]
		DBFilesystem SharedFilesystem { get; set; }

		[Export ("listFolder:error:")]
		DBFileInfo [] ListFolder (DBPath path, out DBError error);

		[Export ("fileInfoForPath:error:")]
		DBFileInfo FileInfoForPath (DBPath path, out DBError error);

		[Export ("openFile:error:")]
		DBFile OpenFile (DBPath path, out DBError error);

		[Export ("createFile:error:")]
		DBFile CreateFile (DBPath path, out DBError error);

		[Export ("openThumbnail:ofSize:inFormat:error:")]
		DBFile OpenThumbnail (DBPath path, DBThumbSize size, DBThumbFormat format, out DBError error);

		[Export ("createFolder:error:")]
		bool CreateFolder (DBPath path, out DBError error);

		[Export ("deletePath:error:")]
		bool DeletePath (DBPath path, out DBError error);

		[Export ("movePath:toPath:error:")]
		bool MovePath (DBPath fromPath, DBPath toPath, out DBError error);

		[Export ("fetchShareLinkForPath:shorten:error:")]
		string FetchShareLink (DBPath path, bool shorten, out DBError error);

		[Export ("account")]
		DBAccount Account { get; }

		[Export ("completedFirstSync")]
		bool CompletedFirstSync { get; }

		[Export ("shutDown")]
		bool ShutDown { [Bind ("isShutDown")] get; }

		[Export ("status")]
		DBSyncStatus Status { get; }

		[Export ("addObserver:block:")]
		bool AddObserver (NSObject observer, DBObserverHandler handler);

		[Export ("addObserver:forPath:block:")]
		bool AddObserver (NSObject observer, DBPath path, DBObserverHandler handler);

		[Export ("addObserver:forPathAndChildren:block:")]
		bool AddObserverForPathAndChildren (NSObject observer, DBPath path, DBObserverHandler handler);

		[Export ("addObserver:forPathAndDescendants:block:")]
		bool AddObserverForPathAndDescendants (NSObject observer, DBPath path, DBObserverHandler handler);

		[Export ("removeObserver:")]
		void RemoveObserver (NSObject observer);
	}

	[BaseType (typeof (NSObject))]
	interface DBList {

		[Export ("count")]
		uint Count { get; }

		[Export ("objectAtIndex:")]
		NSObject objectAtIndex (uint index);

		[Export ("objectAtIndexedSubscript:")]
		NSObject ObjectAtIndexedSubscript (uint index);

		[Export ("insertObject:atIndex:")]
		void InsertObject (NSObject obj, uint index);

		[Export ("removeObjectAtIndex:")]
		void RemoveObjectAtIndex (uint index);

		[Export ("addObject:")]
		void AddObject (NSObject id);

		[Export ("removeLastObject")]
		void RemoveLastObject ();

		[Export ("replaceObjectAtIndex:withObject:")]
		void ReplaceObjectAtIndex (uint index, NSObject obj);

		[Export ("setObject:atIndexedSubscript:")]
		void SetObject (NSObject obj, uint index);

		[Export ("values")]
		NSObject [] Values { get; }
	}

	[BaseType (typeof (NSObject))]
	interface DBPath {
		
		[Static, Export ("root")]
		DBPath Root { get; }
		
		[Export ("initWithString:")]
		IntPtr Constructor (string path);
		
		[Export ("name")]
		string Name { get; }
		
		[Export ("childPath:")]
		DBPath ChildPath (string childName);

		[Export ("parent")]
		DBPath Parent { get; }

		[Export ("stringValue")]
		string StringValue { get; }
	}

	[BaseType (typeof (NSObject))]
	interface DBRecord {

		[Static]
		[Export ("isValidId:")]
		bool IsValidId (string recordId);

		[Static]
		[Export ("isValidFieldName:")]
		bool IsValidFieldName (string name);

		[Export ("recordId")]
		string RecordId { get; }

		[Export ("table")]
		DBTable Table { get; }

		[Export ("fields")]
		NSDictionary Fields { get; }

		[Export ("objectForKey:")]
		NSObject ObjectForKey (string key);

		[Export ("objectForKeyedSubscript:")]
		NSObject ObjectForKeyedSubscript (NSObject key);

		[Export ("getOrCreateList:")]
		DBList GetOrCreateList (string fieldName);

		[Export ("update:")]
		void Update (NSDictionary fieldsToUpdate);

		[Export ("setObject:forKey:")]
		void SetObject (NSObject obj, string fieldName);

		[Export ("setObject:forKeyedSubscript:")]
		void SetObject (NSObject value, NSObject key);

		[Export ("removeObjectForKey:")]
		void RemoveObject (string fieldName);

		[Export ("deleteRecord")]
		void DeleteRecord ();

		[Export ("deleted")]
		bool Deleted { [Bind ("isDeleted")] get; }
	}

	[BaseType (typeof (NSObject))]
	interface DBTable {

		[Static]
		[Export ("isValidId:")]
		bool IsValidId (string tableId);

		[Export ("query:error:")]
		DBRecord [] Query (NSDictionary filter, out DBError error);

		[Export ("getRecord:error:")]
		DBRecord GetRecord (string recordId, out DBError error);

		[Export ("getOrInsertRecord:fields:inserted:error:")] // TODO: Async
		DBRecord GetOrInsertRecord (string recordId, NSDictionary fields, bool inserted, DBError error);

		[Export ("insert:")]
		DBRecord Insert (NSDictionary fields);

		[Export ("setResolutionRule:forField:")]
		void SetResolutionRule (DBResolutionRule rule, string field);

		[Export ("tableId")]
		string TableId { get; }

		[Export ("datastore")]
		DBDatastore Datastore { get; }
	}
}

