using Android.Content;
using Android.Util;
using Android.Views;
using System.Collections.Generic;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Graphics;
using System;
using Android.Media;
using Android.Animation;
using Android.Views.Animations;
using Java.Util;
using Android.Support.V4.View;
using Java.Lang;

namespace MonkeyBox.Android
{
    public class MonkeyView : ImageView//, ScaleGestureDetector.IOnScaleGestureListener, GestureDetector.IOnGestureListener
    {
        // float lastX, lastY;

        protected ScaleGestureDetector PinchDetector;
        protected GestureDetector Detector;
        protected VelocityTracker VelocityTracker;

        protected Monkey monkey;

        //        protected ImageView Image { get; set; }

        // protected BitmapDrawable Drawable { get { return (BitmapDrawable)Image.Drawable; } }

        DisplayMetrics Metrics { get; set; }

        public Monkey Monkey {
            get {
                return monkey;
            }
            set {
                var needsRedraw = monkey != null;
                monkey = value;
                if (needsRedraw)
                    Invalidate();
            }
        }

        public Rect CurrentBounds {
            get {
                var x = (int)GetX();
                var y = (int)GetY();

                var rect = new RectF(Drawable.Bounds); //Left, Top, Right, Bottom);
                var bounds = Matrix.MapRect(rect);

                return new Rect((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom);
            }
        }

        public MonkeyView (Context context, IAttributeSet attrs) :
            base (context, attrs)
        {
            Initialize ();
        }

        public MonkeyView (Context context, Monkey monkey) :
            base (context)
        {
            Monkey = monkey;
            Initialize ();
        }

        public MonkeyView (Context context, IAttributeSet attrs, Monkey monkey) :
            base (context, attrs)
        {
            Monkey = monkey;
            Initialize ();
        }

        public MonkeyView (Context context, IAttributeSet attrs, int defStyle, Monkey monkey) :
            base (context, attrs, defStyle)
        {
            Monkey = monkey;
            Initialize ();
        }

/*        protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure (widthMeasureSpec, heightMeasureSpec);

            var width = System.Math.Max(Drawable.IntrinsicWidth, SuggestedMinimumWidth);
            var height = System.Math.Max(Drawable.IntrinsicHeight, SuggestedMinimumHeight);

            var viewWidth = ResolveSize(width, widthMeasureSpec);
            var viewHeight = ResolveSize(height, heightMeasureSpec);

            SetMeasuredDimension(viewWidth, viewHeight);
        }
        
        protected override void OnDraw (Canvas canvas)
        {
            base.OnDraw (canvas);
            ImageMatrix.Set(canvas.Matrix);
        }
            /*
            //Log.Debug(GetType().Name + " - " + Monkey.Name, "{0}/{1}", GetX(), GetY());

            Drawable.SetBounds(0, 0, Drawable.IntrinsicWidth, Drawable.IntrinsicHeight);

            var matrix = new Matrix();
            //matrix.PreRotate((float)Java.Lang.Math.ToDegrees(Monkey.Rotation), widthOffset, heightOffset);

            var bounds = Drawable.Bounds; //new RectF(Left, Top, Right, Bottom);
            // Apply scaling to monkey image.
            var xOffset =  ((float)Metrics.WidthPixels * Monkey.X) - lastX;
            var yOffset = ((float)Metrics.HeightPixels * Monkey.Y) - lastY;

            matrix.PostScale(Monkey.Scale, Monkey.Scale); // The scale factor makes the result look closer to iOS.
            matrix.PostRotate((float)Java.Lang.Math.ToDegrees(Monkey.Rotation), bounds.CenterX(), bounds.CenterY());
            // Render the image.
            var version = canvas.Save();
            canvas.Concat(matrix);
            Drawable.Draw(canvas);
            canvas.RestoreToCount(version);

            var rect = new RectF(bounds);
            matrix.Reset();
            matrix.PostScale(Monkey.Scale, Monkey.Scale, rect.CenterX(), rect.CenterY()); // The scale factor makes the result look closer to iOS.
            matrix.PostTranslate(xOffset, yOffset);
            // Calculate our view's new bounds from
            // the results of the transforms.
            rect = new RectF(bounds);
            matrix.MapRect(rect);
            Left = (int)rect.Left;
            Right = (int)rect.Right;
            Top = (int)rect.Top;
            Bottom = (int)rect.Bottom;

            Log.Debug(GetType().Name, "{0}'s position: {1}x{2} ({3}x{4}x{5})", Monkey.Name, Left, Top, Monkey.X, Monkey.Y, Monkey.Z);
        }       
*/
        void Initialize ()
        {
//            Metrics = new DisplayMetrics();
//
//            var manager = ((MainActivity)Context).WindowManager;
//            manager.DefaultDisplay.GetMetrics(Metrics);
//
//            var windowSize = new RectF(0, 0, Metrics.WidthPixels, Metrics.HeightPixels);
//
//            Log.Debug(GetType().Name, "Window dimensions: {0}x{1}", windowSize.Width(), windowSize.Height());

            // Enable gesture recognizers.
//            PinchDetector = new ScaleGestureDetector(Context, this);
//
//            Detector = new GestureDetector(Context, this);
//            Detector.IsLongpressEnabled = false;

//            VelocityTracker = VelocityTracker.Obtain();
        }

/*        public bool OnDown (MotionEvent e)
        {
            return true;
        }

        public bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            return false;
            Log.Debug(GetType().Name + " - " + Monkey.Name, "WEEEEE: {0} by {1}", velocityX, velocityY);
            VelocityTracker.AddMovement(e2);
            VelocityTracker.ComputeCurrentVelocity(1000);
            var xVelocity = VelocityTracker.XVelocity;
            var yVelocity = VelocityTracker.YVelocity;
            Log.Debug(GetType().Name + " - " + Monkey.Name, "{0}x{1}", xVelocity, yVelocity);
            var anim = new TranslateAnimation(0, e2.RawX - e1.RawX, 0, e2.RawY - e1.RawY);
            //anim.Interpolator = new DecelerateInterpolator();
            anim.Duration = 300;
            anim.FillAfter = true;
            StartAnimation(anim);
            return false;
        }

        public void OnLongPress (MotionEvent e)
        {
            return;
        }

        public bool OnScroll (MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            var startFinger = MotionEventCompat.GetPointerId(e1, 0);
            var currentFinger = MotionEventCompat.GetPointerId(e2, 0);
            if (startFinger != currentFinger) return false;

            lastX = Left;
            lastY = Top;

            Monkey.X = (e2.RawX - (Width * 0.5f) )/ (float)Metrics.WidthPixels;
            Monkey.Y = (e2.RawY  - (Height * 0.5f) ) / (float)Metrics.HeightPixels;
            Log.Debug(GetType().Name + " - " + Monkey.Name + "OnScroll", "e1: {0} x {1}, e2: {2} x {3}", e1.GetX(),e1.GetY(), e2.GetX(), e2.GetY());
            Log.Debug(GetType().Name + " - " + Monkey.Name + "OnScroll", "{0} x {1} (to {2} x {3}) {4:F1} x {5:F1})", distanceX, distanceY, Left, Top, Monkey.X * Metrics.WidthPixels, Monkey.Y * Metrics.HeightPixels);

            Invalidate();

            return false;
        }

        public void OnShowPress (MotionEvent e)
        {
            return;
        }

        public bool OnSingleTapUp (MotionEvent e)
        {
            BringToFront ();
            Invalidate();

            return true;
        }
*/
//        public override bool OnTouchEvent (MotionEvent e)
//        {
//            PinchDetector.OnTouchEvent(e);
//
//            if (PinchDetector.IsInProgress) return true;
//
//            return Detector.OnTouchEvent(e);
//        }

//        #region IOnScaleGestureListener implementation
//
//        public bool OnScale (ScaleGestureDetector detector)
//        {
//            Log.Debug(GetType().Name + " OnScale", "{0}", detector.ScaleFactor);
//            monkey.Scale *= detector.ScaleFactor;
//            Invalidate();
//            return true;
//        }
//
//        public bool OnScaleBegin (ScaleGestureDetector detector)
//        {
//            Log.Debug(GetType().Name + " OnScaleBegin", "{0}", detector.ScaleFactor);
//            return true;
//        }
//
//        public void OnScaleEnd (ScaleGestureDetector detector)
//        {
//            Log.Debug(GetType().Name + " OnScaleEnd", "{0} ({1})", detector.ScaleFactor, PinchDetector.ScaleFactor);
//            // TODO: Update Dropbox with new scale value.
//        }
//
//        #endregion
    }
}

