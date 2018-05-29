using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Com.Google.Atap.Tangoservice;
using Java.Lang;
using Android.Util;
using Com.Projecttango.Tangosupport;
using System.Collections.Generic;
using Android.Views;
using Android.Runtime;
using Android.Content;
using Android.Widget;

namespace App1
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Tango tango;
        public static string Tag = "Ajax";
        private TangoConfig _tangoConfig;
        private TangoUpdateListener _tangoUpdateListener;
        private TangoPointCloudManager _pointCloudManager;
        private IWindowManager _windowManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.StartActivityForResult(Tango.GetRequestPermissionIntent(Tango.PermissiontypeAdfLoadSave), 0);

            SetContentView(Resource.Layout.activity_main);
            /**
            _pointCloudManager = new TangoPointCloudManager();
            _windowManager = ApplicationContext.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            */
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
                try
                {
                    Log.Debug(Tag, "TangoRunnable");
                    TangoSupport.Initialize();
                    var tangoConfig = ConfigInitialize.SetupTangoConfig(tango);
                    tango.Connect(tangoConfig);
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
            _tangoUpdateListener = new TangoUpdateListener(this);
            tango.ConnectListener(framePairs, _tangoUpdateListener);
        }

        private void ListAdfs()
        {
            var listAdfs = tango.ListAreaDescriptions();
            foreach (var adf in listAdfs)
            {
                Log.Debug(Tag, adf);
            }
        }
    }
}

