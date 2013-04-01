The Dropbox Sync API allows you to give your app its own private Dropbox client and leave the syncing to Dropbox.

- Focus on your data. The Sync API handles all the caching, retrying, and file change notifications.
- Writes are local so changes are immediate. The Sync API syncs to Dropbox behind the scenes.
- Your app works great even when offline and automatically syncs when it's back online.

Here's an example:

##Authenticating with Dropbox

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