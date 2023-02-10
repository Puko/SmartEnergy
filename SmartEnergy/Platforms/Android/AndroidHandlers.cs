using Android.Widget;
using Microsoft.Maui.Controls.Platform;

namespace SmartEnergy.Platforms.Android
{
    public class AndroidHandlers
    {
        public static void Init()
        {
            Microsoft.Maui.Handlers.EntryHandler.ViewMapper.AppendToMapping("SmartEntryMapping", (h, v) =>
            {
                if (v is Entry)
                    (h.PlatformView as EditText).SetBackground(null);
            });
        }
    }
}
