using Android.Views;
using AndroidX.Core.Widget;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;
using ScrollView = Android.Widget.ScrollView;

namespace SmartEnergy.Platforms.Android
{
    public class ScrollListener : Java.Lang.Object, AView.IOnScrollChangeListener
    {
        private readonly Action<Point> _action;

        public ScrollListener(Action<Point> action = null)
        {
            _action = action;
        }

        public void OnScrollChange(AView v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
        {
            _action.Invoke(new Point(scrollX, scrollY));
        }
    }

    public class DragAndDropGestureHandler : Java.Lang.Object, AView.IOnDragListener
    {
        private readonly Action<Point> _action;

        public DragAndDropGestureHandler(Action<Point> action = null)
        {
            _action = action;
        }

        public bool OnDrag(AView v, DragEvent e)
        {
            var x = e.GetX();
            var y = e.GetY();
            
            System.Diagnostics.Debug.WriteLine(e.Action);

            switch (e.Action)
            {
                case DragAction.Ended:
                    break;
                case DragAction.Started:
                    break;
                case DragAction.Location:
                    break;
                case DragAction.Drop:
                    _action?.Invoke(new Point(x, y));
                    break;
                case DragAction.Entered:
                    break;
                case DragAction.Exited:
                    break;
            }

            return true;
        }
    }
}