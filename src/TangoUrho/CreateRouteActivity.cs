using Android.App;
using Android.Content;
using Android.Hardware.Display;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Google.Atap.Tangoservice;
using Com.Projecttango.Tangosupport;
using Java.Lang;
using System.Collections.Generic;

namespace App1
{
    [Activity(Label = "CreateRouteActivity")]
    public class CreateRouteActivity : Activity
    {
        // can use these?
        // pointCloudManager = new TangoPointCloudManager();
        // windowManager = ApplicationContext.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

        private Tango tango;
        private string Tag = "CreateRoute";
        private SurfaceView surfaceView;
        private TangoUpdateListener tangoUpdateListener;
        static object Lock = new object();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_create_route);


            surfaceView = (SurfaceView)FindViewById(Resource.Id.surfaceView1);

            var button = (Button)FindViewById(Resource.Id.back);
            button.Click += delegate
            {
                DisconnectTango();
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };

            var buttonSAve = (Button)FindViewById(Resource.Id.save);
            button.Click += delegate
            {
                // todo: start save ADF function...
            };

            var displayManager = (DisplayManager)GetSystemService("display");

        }

        protected override void OnResume()
        {
            base.OnResume();
            StartTango();

            // OpenGL.Point
            // GLES20.glBindTexture(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, mTextures[0])
        }

        private void DisconnectTango()
        {
            tango.DisconnectCamera(TangoCameraIntrinsics.TangoCameraColor);
            tango.Disconnect();
        }

        private TangoConfig GetTangoConfig(Tango tango)
        {
            // Create a new Tango configuration and enable the Camera API.
            var config = tango.GetConfig(TangoConfig.ConfigTypeDefault);
            config.PutBoolean(TangoConfig.KeyBooleanColorcamera, true);
            config.PutBoolean(TangoConfig.KeyBooleanDepth, true);
            config.PutInt(TangoConfig.KeyIntDepthMode, TangoConfig.TangoDepthModePointCloud);
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
            tangoUpdateListener = new TangoUpdateListener(this);
            tango.ConnectListener(framePairs, tangoUpdateListener);
        }
    }
}
