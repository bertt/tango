using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Google.Atap.Tangoservice;
using Com.Projecttango.Tangosupport;
using Java.Lang;

namespace App1
{
    [Activity(Label = "App1", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private Tango _tango;
        private bool _isConnected;
        private IWindowManager _windowManager;
        private TangoPointCloudManager _pointCloudManager;
        public static string Tag = "GameActivity";
        private TangoConfig _tangoConfig;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            //_pointCloudManager = new TangoPointCloudManager();
            //_windowManager = ApplicationContext.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            var intent = Tango.GetRequestPermissionIntent(Tango.PermissiontypeAdfLoadSave);
            StartActivityForResult(intent, 1);
        }

        protected override void OnResume()
        {
            base.OnResume();

            _tango = new Tango(this, new Runnable(() =>
            {
                Log.Debug(Tag, "TangoRunnable");
                try
                {
                    TangoSupport.Initialize();
                    _tangoConfig = SetupTangoConfig(_tango);
                    _tango.Connect(_tangoConfig);
                    _isConnected = true;
                    var adfs = _tango.ListAreaDescriptions();

                    // get the number of adfs
                    var nr = adfs.Count;
                    var textView = FindViewById<TextView>(Resource.Id.textView2);

                    RunOnUiThread(() => textView.Text = nr.ToString());
                }
                catch (TangoOutOfDateException e)
                {
                    Log.Error(Tag, "TangoOutOfDateException", e);
                }
                catch (TangoErrorException e)
                {
                    // this exception gets thrown
                    Log.Error(Tag, "TangoErrorException", e);
                }
                catch (TangoInvalidException e)
                {
                    Log.Error(Tag, "TangoInvalidException", e);
                }
            }));
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

                return config;
            }
            catch (TangoErrorException e)
            {
                Log.Debug("gaat iets mis", "TangoErrorException", e);
                throw;
            }
        }
    }
}

