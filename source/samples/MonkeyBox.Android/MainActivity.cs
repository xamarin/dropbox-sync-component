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
    public class MainActivity : Activity, ScaleGestureDetector.IOnScaleGestureListener, GestureDetector.IOnGestureListener
    {
        //"YOUR_APP_KEY";
        const string DropboxSyncKey = "twjxmah1ytyhlrj";
        //"YOUR_APP_SECRET";
        const string DropboxSyncSecret = "be9562vibydmzip";

        public DBAccountManager Account { get; private set; }

        public DBDatastore DropboxDatastore { get; set; }

        public IEnumerable<Monkey> Monkeys { get; set; }

        static readonly Dictionary<string,int> ResourceMap = new Dictionary<string, int> {
            { "Fred", Resource.Id.Fred },
            { "George", Resource.Id.George },
            { "Hootie", Resource.Id.Hootie },
            { "Julian", Resource.Id.Julian },
            { "Nim", Resource.Id.Nim },
            { "Pepe", Resource.Id.Pepe }
        };

        ScaleGestureDetector PinchDetector {
            get;
            set;
        }

        GestureDetector Detector {
            get;
            set;
        }

        Monkey CurrentMonkey {
            get {
                return CurrentFocus != null ? ((MonkeyView)CurrentFocus).Monkey : ((MonkeyView)MainLayout.GetChildAt(MainLayout.ChildCount - 1)).Monkey;
            }
        }

        RelativeLayout MainLayout {
            get;
            set;
        }

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            // Add our touch handlers.
            PinchDetector = new ScaleGestureDetector(this, this);
            Detector = new GestureDetector(this, this);
            Detector.IsLongpressEnabled = false;

            MainLayout = (RelativeLayout)FindViewById (Resource.Id.main);
            MainLayout.Touch += HandleTouch;

            // Disable touch handling by the views themselves.
            for(var i = 0; i < MainLayout.ChildCount; i++) {
                var view = MainLayout.GetChildAt(i);
                Log.Debug(GetType().Name + " - OnCreate", "View {0} disabled.", i);
                //view.Enabled = false;
                view.Focusable = true;
                view.FocusableInTouchMode = true;
                view.RequestFocus();
            }

            // Setup Dropbox.
            Account = DBAccountManager.GetInstance (ApplicationContext, DropboxSyncKey, DropboxSyncSecret);     
            Account.LinkedAccountChanged += (sender, e) => Console.WriteLine (e); // TODO: Restart flow.

            if (!Account.HasLinkedAccount) {
                Account.StartLink (this, (int)RequestCode.LinkToDropboxRequest);
            } else {
                StartApp ();
            }
        }

        void HandleTouch (object sender, View.TouchEventArgs e)
        {
            var hit = new Rect((int)e.Event.RawX, (int)e.Event.RawY, (int)e.Event.RawX + 1, (int)e.Event.RawY + 1);
            var currentView = (MonkeyView)CurrentFocus;

            if (currentView == null || !currentView.Drawable.Bounds.Contains(hit)) {
                currentView = null;
                for(var i = MainLayout.ChildCount - 1; i > -1; i--) {
                    var view = (MonkeyView)MainLayout.GetChildAt(i);
                    if (view.Drawable.Bounds.Contains(hit)) {
                        currentView = view;
                        currentView.RequestFocus();
                        break;
                    }
                }
            }

            if (currentView == null) {
                e.Handled = false;
                return;
            }

//            Log.Debug(GetType().Name + " HandleTouch", "Handling touch events");

            PinchDetector.OnTouchEvent(e.Event);

            e.Handled = PinchDetector.IsInProgress || Detector.OnTouchEvent (e.Event);
        }

        #region IOnScaleGestureListener implementation

        public bool OnScale (ScaleGestureDetector detector)
        {
            Log.Debug(GetType().Name + " " + CurrentMonkey.Name + " OnScale", "{0}", detector.ScaleFactor);
            var view = (MonkeyView)MainLayout.FindFocus();
            var bounds = view.Drawable.Bounds;

            view.PivotX = bounds.ExactCenterX();
            view.PivotY = bounds.ExactCenterY();

            view.ScaleX *= detector.ScaleFactor;
            view.ScaleY *= detector.ScaleFactor;

            return true;
        }

        public bool OnScaleBegin (ScaleGestureDetector detector)
        {
            Log.Debug(GetType().Name + " " + CurrentMonkey.Name + " OnScaleBegin", "{0}", detector.ScaleFactor);
            return true;
        }

        public void OnScaleEnd (ScaleGestureDetector detector)
        {
            Log.Debug(GetType().Name + " " + CurrentMonkey.Name + " OnScaleEnd", "{0} ({1})", detector.ScaleFactor, PinchDetector.ScaleFactor);
            UpdateDropbox();
        }

        #endregion

        public bool OnDown (MotionEvent e)
        {
            Log.Debug(GetType().Name + " " + CurrentMonkey.Name + " OnDown", "Handling touch events");
            return true;
        }

        public bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            Log.Debug(GetType().Name + " " + CurrentMonkey.Name + " OnFling", "Handling touch events");
            return false;
        }

        public void OnLongPress (MotionEvent e)
        {
            Log.Debug(GetType().Name + " " + CurrentMonkey.Name + " OnLongPress", "Handling touch events");
            return;
        }

        public bool OnScroll (MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            Log.Debug(GetType().Name + " " + CurrentMonkey.Name + " OnScroll", "Handling touch events");
            return false;
        }

        public void OnShowPress (MotionEvent e)
        {
            Log.Debug(GetType().Name + " " + CurrentMonkey.Name + " OnShowPress", "Handling touch events");
            return;
        }

        public bool OnSingleTapUp (MotionEvent e)
        {
            Log.Debug(GetType().Name + " " + CurrentMonkey.Name + " OnSingleTapUp", "Handling touch events");
            return false;
        }

        void StartApp ()
        {
            InitializeDropbox ();
            Monkeys = GetMonkeys ();
            DrawMonkeys (Monkeys);
        }

        void DrawMonkeys (IEnumerable<Monkey> monkeys)
        {
            var mainLayout = FindViewById (Resource.Id.main) as RelativeLayout;
            if (mainLayout == null) 
                throw new ApplicationException("Missing our main layout. Please ensure the layout xml is included in the project.");

            foreach (var monkey in monkeys.OrderBy(m => m.Z))
            {
                var id = ResourceMap[monkey.Name];
                var mv = mainLayout.FindViewById<MonkeyView>(id);
                mv.Monkey = monkey;
                mv.BringToFront();
                mv.RequestFocus();
                Log.Debug(GetType().Name + " " + mv.Monkey.Name + " OnSingleTapUp", "Handling touch events");
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

        void UpdateDropbox ()
        {
            Log.Debug(GetType().Name + " UpdateDropbox");
            // TODO: Update dropbox.
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


