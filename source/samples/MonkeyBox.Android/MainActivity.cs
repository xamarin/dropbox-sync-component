using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using DropboxSync.Android;
using Android.Accounts;

namespace MonkeyBox.Android
{
  [Activity (Label = "MonkeyBox.Android", MainLauncher = true)]
  public class MainActivity : Activity
  {
    int count = 1;
    const string DropboxSyncKey = "twjxmah1ytyhlrj"; //"YOUR_APP_KEY";
    const string DropboxSyncSecret = "be9562vibydmzip"; //"YOUR_APP_SECRET";

    public DBAccountManager Account { get; private set; }

    protected override void OnCreate (Bundle bundle)
    {
      base.OnCreate (bundle);

      Account = DBAccountManager.GetInstance(ApplicationContext, DropboxSyncKey, DropboxSyncSecret);     
      Account.LinkedAccountChanged += (sender, e) => Console.WriteLine(e.ToString());
      if (!Account.HasLinkedAccount)
      {
        Account.StartLink(this, (int)RequestCode.LinkToDropboxRequest);
      }

      // Set our view from the "main" layout resource
      SetContentView (Resource.Layout.Main);

      // Get our button from the layout resource,
      // and attach an event to it
      Button button = FindViewById<Button> (Resource.Id.myButton);
      
      button.Click += delegate {
        button.Text = string.Format ("{0} clicks!", count++);
      };
    }

    protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
    {
      var code = (RequestCode)requestCode;

      if (code == RequestCode.LinkToDropboxRequest)
      {
        // Start using dropbox.
      }
    }
  }
}


