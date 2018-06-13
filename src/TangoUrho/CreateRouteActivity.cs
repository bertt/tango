using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using Com.Google.Atap.Tangoservice;
using Com.Projecttango.Tangosupport;
using Java.Lang;
using System;
using System.Collections.Generic;
using Urho;
using Urho.Droid;


namespace App1
{
    [Activity(Label = "CreateRouteActivity")]
    public class CreateRouteActivity : Activity
    {
        private Tango tango;
        private string Tag = "CreateRoute";
        private TangoUpdateListener tangoUpdateListener;
        static object Lock = new object();
        private TangoPointCloudManager pointCloudManager;
        // private IWindowManager windowManager;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_create_route);
            pointCloudManager = new TangoPointCloudManager();
            //windowManager = this.ApplicationContext.GetSystemService(Android.Content.Context.WindowService).JavaCast<IWindowManager>();

            var button = (Button)FindViewById(Resource.Id.back);
            button.Click += delegate
            {
                // DisconnectTango();
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };

            var buttonSave = (Button)FindViewById(Resource.Id.save);
            buttonSave.Click += delegate
            {
                // todo: ask for name
                var dt = DateTime.Now;
                var t = System.String.Format("{0:HH:mm:ss}", dt);
                var saveTask = new SaveAdfTask(this, tango, "z " + t);
                var res = saveTask.Execute();
            };

            //var surface = UrhoSurface.CreateSurface(this);
            //surface.SetBackgroundColor(new Android.Graphics.Color(255,0,0));
            //var mLayout = (AbsoluteLayout)FindViewById(Resource.Id.absoluteLayout1);
            //mLayout.AddView(surface);
            // var app = await surface.Show<UrhoScene>(new ApplicationOptions("Data"));
        }

        public void PointCloudIsAvailable()
        {
            RunOnUiThread(() =>
            {
                var buttonSave = (Button)FindViewById(Resource.Id.save);
                buttonSave.Enabled = true;
            });
        }
        protected override void OnResume()
        {
            base.OnResume();
            StartTango();
        }

        private void DisconnectTango()
        {
            tango.DisconnectCamera(TangoCameraIntrinsics.TangoCameraColor);
            tango.Disconnect();
        }

        private TangoConfig GetTangoConfig(Tango tango)
        {
            var config = ConfigInitialize.SetupTangoConfig(tango);
            return config;
        }

        private void StartTango()
        {
            tango = new Tango(this, new Runnable(() =>
            {
                Log.Debug(Tag, "TangoRunnable");
                try
                {
                    TangoSupport.Initialize();
                    var tangoConfig = GetTangoConfig(tango);
                    tango.Connect(tangoConfig);
                    TangoAddListeners();
                    tango.ConnectTextureId(TangoCameraIntrinsics.TangoCameraColor, -1);
                    
                }
                catch (TangoOutOfDateException e)
                {
                    Log.Error(Tag, "TangoOutOfDateException", e);
                }
                catch (TangoErrorException e)
                {
                    Log.Error(Tag, "TangoErrorException", e);
                }
                catch (TangoInvalidException e)
                {
                    Log.Error(Tag, "TangoInvalidException", e);
                }
            }));
        }


        private void TangoAddListeners()
        {
            var framePairs = new List<TangoCoordinateFramePair>()
            {
                new TangoCoordinateFramePair(
                    TangoPoseData.CoordinateFrameStartOfService,
                    TangoPoseData.CoordinateFrameDevice)
            };

            framePairs.Add(new TangoCoordinateFramePair(TangoPoseData.CoordinateFrameAreaDescription, TangoPoseData.CoordinateFrameDevice));
            framePairs.Add(new TangoCoordinateFramePair(TangoPoseData.CoordinateFrameAreaDescription, TangoPoseData.CoordinateFrameStartOfService));

            tangoUpdateListener = new TangoUpdateListener(this);
            tango.ConnectListener(framePairs, tangoUpdateListener);
        }
    }
}
