## Authenticating with Dropbox

You'll need to add snippets of code in the right places to successfully link a user's Dropbox account to your app. Here are all the snippets you'll need for your app:

**AppDelegate.cs**

```csharp
using DropBoxSync.iOS;
...

// Get your own App Key and Secret from https://www.dropbox.com/developers/apps
// Also don't forget to set CFBundleURLSchemes to db-YOUR_APP_KEY in your Info.plist
const string appKey = "YOUR_APP_KEY";
const string appSecret = "YOUR_APP_SECRET";

public override bool FinishedLaunching (UIApplication app, NSDictionary options)
{
	
	// The account manager stores all the account info. Create this when your app launches
	var accountMgr = new DBAccountManager (key: appKey, secret: appSecret);
	DBAccountManager.SetSharedManager (accountMgr);
	var account = accountMgr.LinkedAccount;
	
	if (account != null) {
		var filesystem = new DBFilesystem (account);
		DBFilesystem.SetSharedFilesystem (filesystem);
	}	
	...
}

public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
{
	var account = DBAccountManager.SharedManager.HandleOpenURL (url);
	if (account != null) {
		var filesystem = new DBFilesystem (account);
		DBFilesystem.SetSharedFilesystem (filesystem);
		Console.WriteLine ("App linked successfully!");
		return true;
	}
	Console.WriteLine ("App is not linked");
	return false;
}

```
**Info.plist**

You'll need to register for the url scheme **db-APP_KEY** to complete the authentication flow. The easiest way to do this is to double click on your app's Info.plist file and select the **Advanced Tab** then look for **URL Types Section** then clic **Add URL Type** now set **URL Schemes** to **db-APP_KEY** (i.e.	db-aaa111bbb2222) leave other fields **as is** then save.

Once you've added all the above code, the last thing to do is trigger the link new user flow. Add this snippet to a button in your View Controller to trigger the Link to Dropbox screen.

```csharp
DBAccountManager.SharedManager.LinkFromController (NavigationController)
```
With just these three changes, your app can show the Dropbox OAuth screen and successfully link your account. After linking the account, you should see "App linked successfully" in the console output.

## Listing folders

Once you've linked your app to a Dropbox account, the first thing you'll want to do is list the contents of your App Folder. If you copied the sample code above, once you're authenticated you should have a properly authorized DBFilesystem object stored in `DBFilesystem.SharedFilesystem`, which is the object that allows you to list folders and open, move or delete files.

```csharp
void ListFiles ()
{
	DBError error;
	var contents = DBFilesystem.SharedFilesystem.ListFolder (path, out error);
	foreach (DBFileInfo info in contents) {
		Console.WriteLine (info.Path);
	}	
}
```
All of the Sync API method calls are synchronous, meaning they wait until they can return the requested data (unless an error occurs and an exception is thrown). While it's sometimes convenient to call these from the main thread while you're developing your app, you should make sure all DBFilesystem and DBFile calls are done from a background thread before releasing, to keep your UI responsive.

## Working with files

The first time a user uses your app, their App Folder won't contain any files, so you'll need to create one:

```csharp
void CreateFile ()
{
	DBError error;
	var newPath = DBPath.Root.ChildPath ("hello.txt");
	var file = DBFilesystem.SharedFilesystem.CreateFile (newPath, out error);
	file.WriteString ("Hello World!", out error);
}
```
Writing to the file will succeed immediately and then the Sync API will send the file to Dropbox. Even if you are offline, the write will succeed and will be automatically synced to the server once your app comes back online. There are a couple of other ways to write to a file too, depending on how your app works.

Reading a file is just as easy: you can call `DBFile.ReadString` to get the file's contents decoded as a UTF8 string. If the file is not cached however, this operation can take a long time while the file downloads, so you should make sure you don't do this from your app's main thread. To find out out when a file downloads you need to register an observer.

## Watching for changes with observers

All the main objects in the Sync API allow you to register a callback that will get called when something about a file changes. Here's an example of how to find out when a file has changed:

```csharp
// Class level variables
DBFile file;
...

// First, create a file for you to change
DBPath path = DBPath.Root.ChildPath ("change-me.txt");
DBError err; 
file = DBFilesystem.SharedFilesystem.CreateFile (path, err);

// Next, register for changes on that file
file.AddObserver (this, ()=> {

    // This Callback will be called every time your file changes

    // if newerStatus is not null, it means a newer version is available
    DBFileStatus newerStatus = file.NewerStatus;
    if (newerStatus =! null) {

        if (!newerStatus.Cached) {
            Console.WriteLine ("newerStatus.Cached == false; this means the file downloading");
        } else {
			DBError error;
            // Update to the newly available version and print it out
            file.Update (out error);
            Console.WriteLine ("The file is done downloading: " + file.ReadString(out error);
        }
    }
}];
```
In the example above, every time you edit "change-me.txt" in your app's App Folder it will print to the console once it's starts downloading, and print the new contents when it finishes downloading. When you don't want to receive more updates, such as when your controller is offscreen, you can unregister for updates like this:

```csharp
file.RemoveObserver (self);
```

## Documentation

If you're ready to explore more of what the Sync API has to offer, check out the full [iOS SDK documentation](https://www.dropbox.com/developers/sync/docs/ios).