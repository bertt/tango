using Android.App;
using Android.Content;
using Android.Hardware.Display;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Google.Atap.Tangoservice;
using Com.Projecttango.Tangosupport;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_create_route);
            pointCloudManager = new TangoPointCloudManager();
            //windowManager = this.ApplicationContext.GetSystemService(Android.Content.Context.WindowService).JavaCast<IWindowManager>();

            var button = (Button)FindViewById(Resource.Id.back);
            button.Click += delegate
            {
                DisconnectTango();
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };

            var buttonSave = (Button)FindViewById(Resource.Id.save);
            buttonSave.Click += delegate
            {
                // todo: ask for name
                var dt = DateTime.Now;
                var t = System.String.Format("{0:HH:mm:ss}", dt);
                var saveTask = new SaveAdfTask(tango, "z " + t);
                saveTask.Execute();
            };
        }

        public void PointCloudIsAvailable()
        {
            RunOnUiThread(() =>
            {
                var buttonSave = (Button)FindViewById(Resource.Id.save);
                buttonSave.Enabled = true;
            });
        }

        /**
        public void UpdatePointCloud(TangoPointCloudData pointCloud)
        {
            //pointCloudManager.UpdatePointCloud(pointCloud);
        }*/

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
            
            
            // Create a new Tango configuration and enable the Camera API.
            /**var config = tango.GetConfig(TangoConfig.ConfigTypeDefault);
            config.PutBoolean(TangoConfig.KeyBooleanColorcamera, true);
            config.PutBoolean(TangoConfig.KeyBooleanDepth, true);
            config.PutInt(TangoConfig.KeyIntDepthMode, TangoConfig.TangoDepthModePointCloud);
            return config;*/
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
                    StartupTango();
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


        private void StartupTango()
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
