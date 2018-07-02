using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using Com.Google.Atap.Tangoservice;
using Java.Lang;

namespace App1
{
    [Activity(Label = "NavigateRouteActivity")]
    public class NavigateRouteActivity : Activity
    {
        private string Tag = "NavigateRouteActivity";
        private Tango tango;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_navigate_route);

            var button = (Button)FindViewById(Resource.Id.back);
            button.Click += delegate
            {
                DisconnectTango();
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };


            tango = new Tango(this, new Runnable(() =>
            {
                // now load the latest adf...
                StartTango();
            }));
        }

        protected override void OnResume()
        {
            base.OnResume();
            StartTango();
        }

        private void StartTango()
        {
            var listAdfs = tango.ListAreaDescriptions();

            // take the first...
            var uuid = listAdfs[0];
            var metadata = tango.LoadAreaDescriptionMetaData(uuid);
            var name = new String(metadata.Get(TangoAreaDescriptionMetaData.KeyName)).ToString();

            Log.Debug("Tag", "ADF loading: " + name);

            InitializeMotionTracking(uuid);
        }

        protected override void OnPause()
        {
            base.OnPause();
            DisconnectTango();
            // tango.Disconnect();
        }

        private void DisconnectTango()
        {
            tango.DisconnectCamera(TangoCameraIntrinsics.TangoCameraColor);
            tango.Disconnect();
        }

        private void InitializeMotionTracking(string uuid)
        {
            var config = ConfigInitialize.SetupTangoConfigForNavigating(tango, uuid);

            try
            {
                tango.Connect(config);
                TangoAddListeners();
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
        }

        private void TangoAddListeners()
        {

            var pairs = FramePairsInitializer.GetPairs();

            var tangoNavigateListener = new TangoNavigateListener(this);
            tango.ConnectListener(pairs, tangoNavigateListener);
        }
    }
}