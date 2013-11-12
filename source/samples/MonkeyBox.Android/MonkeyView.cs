using Android.Content;
using Android.Util;
using Android.Views;
using System.Collections.Generic;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Graphics;
using System;

namespace MonkeyBox.Android
{
    public class MonkeyView : View
    {
        static readonly Dictionary<string,int> ResourceMap = new Dictionary<string, int> {
            { "Fred", Resource.Drawable.Fred },
            { "George", Resource.Drawable.George },
            { "Hootie", Resource.Drawable.Hootie },
            { "Julian", Resource.Drawable.Julian },
            { "Nim", Resource.Drawable.Nim },
            { "Pepe", Resource.Drawable.Pepe }
        };

        protected Monkey monkey;

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

        protected ImageView Image { get; set; }

        protected BitmapDrawable Drawable { get { return (BitmapDrawable)Image.Drawable; } }

        DisplayMetrics Metrics {
            get;
            set;
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

        protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
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

            var version = canvas.Save();

            var widthOffset = Drawable.IntrinsicWidth * 0.5f;
            var heightOffset = Drawable.IntrinsicHeight * 0.5f;

            Drawable.SetBounds(0, 0, Drawable.IntrinsicWidth, Drawable.IntrinsicHeight);

            var matrix = new Matrix(Image.ImageMatrix);
            matrix.PreRotate((float)Java.Lang.Math.ToDegrees(Monkey.Rotation), widthOffset, heightOffset);
            matrix.PreScale(Monkey.Scale - 0.05f, Monkey.Scale - 0.05f);

            var rect = new RectF(Drawable.Bounds.Left, Drawable.Bounds.Top, Drawable.Bounds.Right, Drawable.Bounds.Bottom);
            matrix.MapRect(rect);

            widthOffset = (rect.Right - rect.Left) * 0.5f;
            heightOffset = (rect.Bottom - rect.Top) * 0.5f;

            matrix.PostTranslate(((float)Metrics.WidthPixels * Monkey.X) - widthOffset, ((float)Metrics.HeightPixels * Monkey.Y) - heightOffset);

            canvas.Concat(matrix);

            Drawable.Draw(canvas);

            Log.Debug(GetType().Name, "{0}'s position: {1}x{2} ({3}x{4}x{5})", Monkey.Name, Left, Top, Monkey.X, Monkey.Y, Monkey.Z);

            canvas.RestoreToCount(version);
        }

        void Initialize ()
        {
            Image = new ImageView (Context);
            var resource = ResourceMap[Monkey.Name];

            Log.Debug(GetType().Name, "{0}'s resource: {1}", Monkey.Name, resource);

            Image.SetImageResource (resource);

            Metrics = new DisplayMetrics();

            var manager = ((MainActivity)Context).WindowManager;
            manager.DefaultDisplay.GetMetrics(Metrics);

            var windowSize = new RectF(0, 0, Metrics.WidthPixels, Metrics.HeightPixels);

            Log.Debug(GetType().Name, "Window dimensions: {0}x{1}", windowSize.Width(), windowSize.Height());

        }
    }
}

