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
    public class MainActivity : Activity, ScaleGestureDetector.IOnScaleGestureListener, MoveGestureDetector.IOnMoveGestureListener, RotateGestureDetector.IOnRotateGestureListener, GestureDetector.IOnGestureListener
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

        MoveGestureDetector MoveDetector {
            get;
            set;
        }

        RotateGestureDetector RotationDetector {
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
            MoveDetector = new MoveGestureDetector(this, this);
            RotationDetector = new RotateGestureDetector(this, this);

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

            // Figure out which view gets this touch event.
            if (currentView == null) {
                currentView = ViewRespondingToHitTest (hit);
            }

            var handled = false;

            if (currentView != null) {
                foreach (var result in ProcessDetectors(e.Event)) {
                    if (result) {
                        handled = true;
                        Log.Debug(GetType().Name + " " + CurrentMonkey.Name + " HandleTouch", "One or more detectors handled the touch.");
                    }
                }
            }

            e.Handled = handled;
        }

        MonkeyView ViewRespondingToHitTest (Rect hit)
        {
            MonkeyView currentView = default(MonkeyView);
            for (var i = MainLayout.ChildCount - 1; i > -1; i--) {
                var view = (MonkeyView)MainLayout.GetChildAt (i);
                if (view.CurrentBounds.Contains (hit)) {
                    currentView = view;
                    currentView.RequestFocus ();
                    break;
                }
            }
            return currentView;
        }

        DisplayMetrics Metrics {
            get {
                var metrics = new DisplayMetrics();
                WindowManager.DefaultDisplay.GetMetrics(metrics);
                return metrics;
            }
        }

        IEnumerable<bool> ProcessDetectors(MotionEvent e) {

            RotationDetector.OnTouchEvent(e);
            yield return RotationDetector.IsInProgress();

            PinchDetector.OnTouchEvent(e);
            yield return PinchDetector.IsInProgress;

            MoveDetector.OnTouchEvent(e);
            yield return MoveDetector.IsInProgress();

            yield return Detector.OnTouchEvent(e);
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

            view.Monkey.Scale = view.ScaleX;

            return true;
        }

        public bool OnScaleBegin (ScaleGestureDetector detector)
        {
            return true;
        }

        public void OnScaleEnd (ScaleGestureDetector detector)
        {
            UpdateDropbox();
        }

        #endregion

        #region IOnMoveGestureListener implementation

        public bool OnMove (MoveGestureDetector detector)
        {
            var positionOffset = detector.FocusDelta;

            var view = (MonkeyView)MainLayout.FindFocus();

            view.TranslationX += positionOffset.X;
            view.TranslationY += positionOffset.Y;

            view.Monkey.X = view.TranslationX / (float)Metrics.WidthPixels;
            view.Monkey.Y = view.TranslationY / (float)Metrics.HeightPixels;

            Log.Debug(GetType().Name + " " + CurrentMonkey.Name + " OnMove", "Handling touch events");
            return true;
        }

        public bool OnMoveBegin (MoveGestureDetector detector)
        {
            return true;
        }

        public void OnMoveEnd (MoveGestureDetector detector)
        {
            Log.Debug(GetType().Name + " " + CurrentMonkey.Name + " OnMoveEnd", "Handling touch events");
            UpdateDropbox();
        }

        #endregion

        #region IOnRotateGestureListener implementation

        public bool OnRotate (RotateGestureDetector detector)
        {
            var view = (MonkeyView)MainLayout.FindFocus();
            var bounds = view.Drawable.Bounds;

            Log.Debug(GetType().Name + " " + CurrentMonkey.Name + " OnRotate", "Rotating {0:F3} degrees to {1:F3}.", detector.RotationDegreesDelta, view.RotationX);

            view.PivotX = bounds.ExactCenterX();
            view.PivotY = bounds.ExactCenterY();

            view.Monkey.Rotation -= detector.RotationDegreesDelta ;//* (float)(180.0 / Math.PI);
            view.Rotation = view.Monkey.Rotation;

            return true;
        }

        public bool OnRotateBegin (RotateGestureDetector detector)
        {
            return true;
        }

        public void OnRotateEnd (RotateGestureDetector detector) {  }

        #endregion

        public bool OnDown (MotionEvent e)
        {
            return true; // Must be true, or we won't get future event notifications.
        }

        public bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            return false;
        }

        public void OnLongPress (MotionEvent e)
        {
            return;
        }

        public bool OnScroll (MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return false;
        }

        public void OnShowPress (MotionEvent e)
        {
            return;
        }

        public bool OnSingleTapUp (MotionEvent e)
        {
            Log.Debug(GetType().Name + " " + CurrentMonkey.Name + " OnSingleTapUp", "Handling touch events");

            var currentView = (MonkeyView)CurrentFocus;
            var hit = new Rect((int)e.RawX, (int)e.RawY, (int)e.RawX + 1, (int)e.RawY + 1);

            if (!currentView.CurrentBounds.Contains(hit)) {
                currentView = ViewRespondingToHitTest(hit);
                if (currentView == null) return true;
                currentView.BringToFront();
                currentView.RequestFocus();
            }
            return true;
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
                monkey.Scale += 0.1f;

                mv.Monkey = monkey;


                mv.PivotX = mv.Drawable.Bounds.ExactCenterX();
                mv.PivotY = mv.Drawable.Bounds.ExactCenterY();

                mv.Rotation = monkey.Rotation;

                mv.ScaleX = monkey.Scale;
                mv.ScaleY = monkey.Scale;

                mv.TranslationX = monkey.X * (float)Metrics.WidthPixels;
                mv.TranslationY = monkey.Y * (float)Metrics.HeightPixels;

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
                    Monkeys = GetMonkeys();
                    DrawMonkeys(Monkeys);
                }
            };
        }

        void UpdateDropbox ()
        {
            Log.Debug(GetType().Name + " UpdateDropbox", "TODO: Update dropbox data");
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


