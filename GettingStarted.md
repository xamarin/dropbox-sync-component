## Authenticating with Dropbox

Add the following lines of code to link a user's Dropbox account to your
app:

### In AppDelegate.cs

```csharp
using DropBoxSync.iOS;
...

// Get your own App Key and Secret from https://www.dropbox.com/developers/apps
const string DropboxSyncKey = "YOUR_APP_KEY";
const string DropboxSyncSecret = "YOUR_APP_SECRET";

public override bool FinishedLaunching (UIApplication app, NSDictionary options)
{
	
	// The account manager stores all the account info. Create this when your app launches
	var manager = new DBAccountManager (DropboxSyncKey, DropboxSyncSecret);
	DBAccountManager.SetSharedManager (manager);

	var account = manager.LinkedAccount;
	if (account != null) {
		var filesystem = new DBFilesystem (account);
		DBFilesystem.SetSharedFilesystem (filesystem);
	}	

	// ...
}

public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
{
	var account = DBAccountManager.SharedManager.HandleOpenURL (url);
	if (account != null) {
		var filesystem = new DBFilesystem (account);
		DBFilesystem.SetSharedFilesystem (filesystem);
		Console.WriteLine ("App linked successfully!");
		return true;
	} else {
		Console.WriteLine ("App is not linked");
		return false;
	}
}

```

### In Info.plist

You'll need to register the url scheme "db-APP_KEY" to complete the
authentication flow. Double-click on your app's Info.plist file, select
the Advanced Tab, find the URL Types Section, then click Add URL Type
and set URL Schemes to db-APP_KEY (i.e.	"db-aaa111bbb2222").

### Link the user

Once you've added the code above, you're ready to link the user's
Dropbox account from your UI. For example, add this snippet to a UI
event handler in one of your controllers:

```csharp
DBAccountManager.SharedManager.LinkFromController (myController)
```

This will show the Dropbox OAuth screen and ask the user to link their
account.

## Listing folders

Once you've linked your app to a Dropbox account, you may want to list
the contents of your app's exclusive Dropbox folder. If you used the
sample code above, once you're authenticated you should have a properly
authorized `DBFilesystem` instance stored in
`DBFilesystem.SharedFilesystem`, which is the object that allows you to
list folders; and open, move or delete files.

```csharp
void ListFiles (string path)
{
	DBError error;

	var contents = DBFilesystem.SharedFilesystem.ListFolder (path, out error);
	foreach (DBFileInfo info in contents) {
		Console.WriteLine (info.Path);
	}	
}
```

Sync API method calls involving file reads are synchronous, meaning they wait until the
requested data is available, or until an error occurs and an exception
is thrown. You should make sure all DBFilesystem and DBFile calls are
done from a background thread to keep your UI responsive.

## Working with files

Initially, your app's folder in your user's Dropbox won't contain any
files, so you'll need to create one:

```csharp
void CreateFile ()
{
	DBError error;

	var dbpath = DBPath.Root.ChildPath ("hello.txt");
	var file = DBFilesystem.SharedFilesystem.CreateFile (dbpath, out error);
	file.WriteString ("Hello World!", out error);
}
```

Writing to the file will succeed immediately, and the Sync API will sync
the file to Dropbox asynchronously. Even if you are offline, the write
will succeed and it will be automatically synced to the server once your
app comes back online.

Reading a file is just as easy: you can call `DBFile.ReadString` to get
a file's contents as a UTF8 string. If the file is not cached, this
operation can take a while, so always call this method on a background
thread.

## Watching for changes

Many objects in the Sync API allow you to register a callback
that will get called when something about a file changes. Here's an
example of how to find out when a file has changed:

```csharp
void CreateAndWatchFile ()
{
	// First, create a file to change for demo purposes.
	DBError err; 
	DBPath path = DBPath.Root.ChildPath ("change-me.txt");
	DBFile file = DBFilesystem.SharedFilesystem.CreateFile (path, out err);

	// Next, register for changes on that file.
	file.AddObserver (this, () => {
			DBFileStatus status = file.NewerStatus;

			// If file.NewerStatus is null, the file hasn't changed.
			if (status == null) return;

			if (status.Cached) {
				DBError error;
				file.Update (out error);
				Console.WriteLine ("The updated file has finished downloading");
			} else {
				Console.WriteLine ("The file is still downloading");
			}
	});
}
```

In the example above, every time you edit "change-me.txt" in your app's
Dropbox folder, the callback will print to the console when the file
starts downloading,
and print again when it finishes downloading.

To stop listening for updates:

```csharp
file.RemoveObserver (this);
```

## Documentation

To explore the full Dropbox Sync API, check out our [iOS SDK documentation](https://www.dropbox.com/developers/sync/docs/ios).
