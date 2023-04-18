using Android.Content;
using Android.Views;
using Microsoft.Maui.Platform;
using View = Android.Views.View;

namespace SmartEnergy.Platforms.Android
{
    public static class ViewExtensions
    {
        public static void HandleClick(this View view, Action action)
        {
            view.Click += (sender, args) => action.Invoke();
        }

        public static void StartDragOnLongPress(this View view, Action action)
        {
            view.LongClick += (sender, args) =>
            {
                action.Invoke();
                var item = new ClipData.Item(string.Empty);
                var mimeTypes = new List<string> { ClipDescription.MimetypeTextPlain };

                var dragShadowBuilder = new View.DragShadowBuilder(view);
                var data = new ClipData(string.Empty, mimeTypes.ToArray(), item);

                if (OperatingSystem.IsAndroidVersionAtLeast(24))
                    view.StartDragAndDrop(data, dragShadowBuilder, null,
                        (int)DragFlags.Global | (int)DragFlags.GlobalUriRead);
                else
                    view.StartDrag(data, dragShadowBuilder, null, (int)DragFlags.Global | (int)DragFlags.GlobalUriRead);

            };
        }

        public static void HandleDrop(this View view, IView sc, Action<Point> point)
        {
            Point scroll = new Point(0,0);
            view.SetOnScrollChangeListener(new ScrollListener(p =>
            {
                scroll = p;
            }));

            view.SetOnDragListener(new DragAndDropGestureHandler(p =>
            {
                var w = view.Width;
                var h = view.Height;
                
                var x = ConvertRange(0, w, 0, sc.DesiredSize.Width, p.X + scroll.X);
                var y = ConvertRange(0, h, 0, sc.DesiredSize.Height, p.Y + scroll.Y);
                
                point.Invoke(new Point(x, y));

            }));
        }

        private static double ConvertRange(
            double originalStart, double originalEnd,
            double newStart, double newEnd, 
            double value) 
        {
            double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
            return (double)(newStart + ((value - originalStart) * scale));
        }
    }
}
