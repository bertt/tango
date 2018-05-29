using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Com.Google.Atap.Tangoservice;
using Java.Lang;
using Android.Util;
using Com.Projecttango.Tangosupport;
using System.Collections.Generic;

namespace App1
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Tango _tango;
        public static string Tag = "Ajax";
        private TangoConfig _tangoConfig;
        private bool _isConnected;
        private TangoUpdateListener _tangoUpdateListener;
        private TangoPointCloudManager _pointCloudManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            _pointCloudManager = new TangoPointCloudManager();

            Connect();
        }

        private void Connect()
        {
            _tango = new Tango(this, new Runnable(() =>
            {
                Log.Debug(Tag, "TangoRunnable");
                try
                {
                    TangoSupport.Initialize();
                    _tangoConfig = SetupTangoConfig(_tango);
                    _tango.Connect(_tangoConfig);
                    StartupTango();
                    _isConnected = true;
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
            _tangoUpdateListener = new TangoUpdateListener(this);
            _tango.ConnectListener(framePairs, _tangoUpdateListener);
        }




        private TangoConfig SetupTangoConfig(Tango tango)
        {
            try
            {
                // Create a new Tango Configuration and enable the MotionTrackingActivity API.
                TangoConfig config = tango.GetConfig(TangoConfig.ConfigTypeDefault);
                config.PutBoolean(TangoConfig.KeyBooleanMotiontracking, true);
                // Tango service should automatically attempt to recover when it enters an invalid state.
                config.PutBoolean(TangoConfig.KeyBooleanAutorecovery, true);

                config.PutBoolean(TangoConfig.KeyBooleanColorcamera, true);
                config.PutBoolean(TangoConfig.KeyBooleanDepth, true);
                // NOTE: Low latency integration is necessary to achieve a precise alignment of
                // virtual objects with the RBG image and produce a good AR effect.
                config.PutBoolean(TangoConfig.KeyBooleanLowlatencyimuintegration, true);
                // Drift correction allows motion tracking to recover after it loses tracking.
                // The drift corrected pose is is available through the frame pair with
                // base frame AREA_DESCRIPTION and target frame DEVICE.
                config.PutBoolean(TangoConfig.KeyBooleanDriftCorrection, true);
                var c = config.GetString(TangoConfig.KeyStringDatasetsPath);
                config.PutInt(TangoConfig.KeyIntDepthMode, TangoConfig.TangoDepthModePointCloud);


                //var files = Directory.GetFiles(c);

                return config;
            }
            catch (TangoErrorException e)
            {
                Log.Error(Tag, "TangoErrorException", e);
                throw;
            }
        }

    }
}

