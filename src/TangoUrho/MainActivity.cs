using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Com.Google.Atap.Tangoservice;
using Java.Lang;
using Android.Util;
using Com.Projecttango.Tangosupport;
using System.Collections.Generic;
using Android.Views;
using Android.Content;
using Android.Widget;
using Android.Runtime;

namespace App1
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Tango tango;
        public static string Tag = "Ajax";
        private TangoConfig _tangoConfig;
        private TangoUpdateListener tangoUpdateListener;
        private TangoPointCloudManager pointCloudManager;
        private IWindowManager windowManager;
        private bool hasPermissionsRequested = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            //StartActivityForResult(Tango.GetRequestPermissionIntent(Tango.PermissiontypeAdfLoadSave), 0);
            //hasPermissionsRequested = true;

            SetContentView(Resource.Layout.activity_main);
            //pointCloudManager = new TangoPointCloudManager();
            //windowManager = ApplicationContext.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            var button = (Button)FindViewById(Resource.Id.listadfs);
            button.Click += delegate {
                var intent = new Intent(this, typeof(ListAdfsActivity));
                StartActivity(intent);
            };

        }

        protected override void OnResume()
        {
            base.OnResume();
            // Connect();
        }

        private void Connect()
        {
            tango = new Tango(this, new Runnable(() =>
            {


                var listAdfs = tango.ListAreaDescriptions();
                foreach (var adf in listAdfs)
                {
                    Log.Debug(Tag, adf);
                }

                Log.Debug(Tag, "TangoRunnable");
                try
                {
                    TangoSupport.Initialize();
                    _tangoConfig = SetupTangoConfig(tango);
                    tango.Connect(_tangoConfig);
                    StartupTango();
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

