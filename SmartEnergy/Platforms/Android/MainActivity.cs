﻿using Android.App;
using Android.Content.PM;
using Android.Views;

namespace SmartEnergy;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    //public override bool DispatchTouchEvent(MotionEvent ev)
    //{
    //    return true;
    //}

    //public override bool OnTouchEvent(MotionEvent e)
    //{
    //    return base.OnTouchEvent(e);
    //}
}
