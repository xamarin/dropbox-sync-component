using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using DropboxSync.Android;
using System.Linq;
using Android.App;
using Android.Views;
using System.Collections;
using System.Collections.Generic;
using MonkeyBox;

namespace MonkeyBox.Android
{
    [Activity (Label = "MonkeyBox.Android", MainLauncher = true)]
    public class MainActivity : Activity
    {
        int count = 1;
        const string DropboxSyncKey = "twjxmah1ytyhlrj";
        //"YOUR_APP_KEY";
        const string DropboxSyncSecret = "be9562vibydmzip";
        //"YOUR_APP_SECRET";
        public DBAccountManager Account { get; private set; }

        public DBDatastore DropboxDatastore { get; set; }

        public IEnumerable<Monkey> Monkeys { get; set; }

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            Account = DBAccountManager.GetInstance (ApplicationContext, DropboxSyncKey, DropboxSyncSecret);     
            Account.LinkedAccountChanged += (sender, e) => Console.WriteLine (e); // TODO: Restart flow.

            if (!Account.HasLinkedAccount) {
                Account.StartLink (this, (int)RequestCode.LinkToDropboxRequest);
            } else {
                InitializeDropbox ();
                Monkeys = GetMonkeys ();
                DrawMonkeys (Monkeys);
            }

        }

        void DrawMonkeys (IEnumerable<Monkey> monkeys)
        {
            // Load Monkeys.
            var nim = new ImageView (this);
            nim.SetImageResource (Resource.Drawable.Nim);
            var mainLayout = FindViewById (Resource.Id.main) as RelativeLayout;
            var layoutParams = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            //            layoutParams.AddRule(LayoutRules.Below)
            mainLayout.AddView (nim, layoutParams);
        }

        void InitializeDropbox ()
        {
            DropboxDatastore = DBDatastore.OpenDefault (Account.LinkedAccount);
            DropboxDatastore.DatastoreChanged += (sender, e) =>  {
                Console.WriteLine ("Datastore needs to be re-synced.");
                e.P0.Sync ();
            };
        }

        IEnumerable<Monkey> GetMonkeys ()
        {
            var table = DropboxDatastore.GetTable ("monkeys");
            var values = new List<Monkey>(6);

            foreach (var row in table.Query ().AsList ()) {
                values.Add(new Monkey { 
                    Name = row.GetString("Name"),
                    Scale = Convert.ToSingle(row.GetString("Scale")),
                    Rotation = Convert.ToSingle(row.GetString("Rotation")),
                    X = Convert.ToSingle(row.GetString("X")),
                    Y = Convert.ToSingle(row.GetString("Y")),
                    Z = Convert.ToInt32(row.GetString("Z"))
                });
            }
            return values;
        }

        protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
        {
            var code = (RequestCode)requestCode;

            if (code == RequestCode.LinkToDropboxRequest && resultCode != Result.Canceled) {
                // Start using dropbox.
                InitializeDropbox ();
                GetMonkeys ();
            }
        }
    }
}


