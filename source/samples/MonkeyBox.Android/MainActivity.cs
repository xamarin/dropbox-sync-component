using System;
using System.Collections.Generic;

using Android.Content;
using Android.Widget;
using Android.OS;
using Android.App;

using DropboxSync.Android;

using MonkeyBox;
using System.Linq;
using Android.Graphics;
using Android.Util;
using Java.Util;
using Android.Views;

namespace MonkeyBox.Android
{
    [Activity (Label = "MonkeyBox.Android", MainLauncher = true)]
    public class MainActivity : Activity
    {
        //"YOUR_APP_KEY";
        const string DropboxSyncKey = "twjxmah1ytyhlrj";
        //"YOUR_APP_SECRET";
        const string DropboxSyncSecret = "be9562vibydmzip";
        public DBAccountManager Account { get; private set; }

        public DBDatastore DropboxDatastore { get; set; }

        public IEnumerable<Monkey> Monkeys { get; set; }
        public Dictionary<string, MonkeyView> Views { get; set; }

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
                StartApp ();
            }

        }

        void StartApp ()
        {
            InitializeDropbox ();
            Monkeys = GetMonkeys ();
            Views = new Dictionary<string, MonkeyView>(6);
            DrawMonkeys (Monkeys);
        }

        void DrawMonkeys (IEnumerable<Monkey> monkeys)
        {
            var mainLayout = FindViewById (Resource.Id.main) as RelativeLayout;

            foreach (var monkey in monkeys.OrderBy(m => m.Z))
            {
                if (!Views.ContainsKey(monkey.Name)) {
                    var mv = new MonkeyView(this, monkey);
                    var param = ViewGroup.LayoutParams.MatchParent;
                    var layoutParams = new RelativeLayout.LayoutParams(param, param);

                    mainLayout.AddView (mv, layoutParams);
                    Views[monkey.Name] = mv;
                } else {
                    var mv = Views[monkey.Name];
                    mv.BringToFront(); // Handles z-index changes.
                    mv.Monkey = monkey; // Setter calls invalidate, which causes that view to redraw.
                }
            }
        }

        void InitializeDropbox ()
        {
            DropboxDatastore = DBDatastore.OpenDefault (Account.LinkedAccount);
            DropboxDatastore.DatastoreChanged += (sender, e) =>  {
                if (e.P0.SyncStatus.HasIncoming)
                {
                    Console.WriteLine ("Datastore needs to be re-synced.");
                    e.P0.Sync ();
                    DrawMonkeys(GetMonkeys());
                }
            };
        }

        IEnumerable<Monkey> GetMonkeys ()
        {
            var table = DropboxDatastore.GetTable ("monkeys");
            var values = new List<Monkey>(6);
            var results = table.Query ().AsList ();

            if (results.Count == 0) {
                // Generate random monkeys.
                values.AddRange(Monkey.GetAllMonkeys());
            } else {
                // Process existing monkeys.
                foreach (var row in results) {
                    Log.Debug(GetType().Name, row.ToString());
                    values.Add(new Monkey { 
                        Name = row.GetString("Name"),
                        Scale = Convert.ToSingle(row.GetString("Scale")),
                        Rotation = Convert.ToSingle(row.GetString("Rotation")),
                        X = Convert.ToSingle(row.GetString("X")),
                        Y = Convert.ToSingle(row.GetString("Y")),
                        Z = Convert.ToInt32(row.GetString("Z"))
                    });
                }
            }

            return values;
        }

        protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
        {
            var code = (RequestCode)requestCode;

            if (code == RequestCode.LinkToDropboxRequest && resultCode != Result.Canceled) {
                StartApp();
            }
        }
    }
}


