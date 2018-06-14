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
        private Tango tango;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_navigate_route);

            var button = (Button)FindViewById(Resource.Id.back);
            button.Click += delegate
            {
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };


            tango = new Tango(this, new Runnable(() =>
            {
                var listAdfs = tango.ListAreaDescriptions();

                // take the first...
                var uuid = listAdfs[0];
                var metadata = tango.LoadAreaDescriptionMetaData(uuid);
                var name = new String(metadata.Get(TangoAreaDescriptionMetaData.KeyName)).ToString();

                Log.Debug("Tag", "ADF loading: " + name);

                InitializeMotionTracking(uuid);

                // now load the latest adf...

            }));
        }

        private void InitializeMotionTracking(string uuid)
        {
            var config = ConfigInitialize.SetupTangoConfigForNavigating(tango, uuid);

        }


        private void TangoAddListeners()
        {

            var pairs = FramePairsInitializer.GetPairs();

            //var tangoUpdateListener = new TangoUpdateListener(this);
            //tango.ConnectListener(pairs, tangoUpdateListener);
        }

    }
}