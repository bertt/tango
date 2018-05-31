using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Com.Google.Atap.Tangoservice;
using Com.Projecttango.Tangosupport;
using Android.Views;
using Android.Content;
using Android.Widget;

namespace App1
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static string Tag = "Ajax";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartActivityForResult(Tango.GetRequestPermissionIntent(Tango.PermissiontypeAdfLoadSave), 0);

            SetContentView(Resource.Layout.activity_main);
            var button = (Button)FindViewById(Resource.Id.listadfs);
            button.Click += delegate
            {
                var intent = new Intent(this, typeof(ListAdfsActivity));
                StartActivity(intent);
            };
            var buttonRec = (Button)FindViewById(Resource.Id.recording);
            buttonRec.Click += delegate
            {
                var intent = new Intent(this, typeof(CreateRouteActivity));
                StartActivity(intent);
            };
            var buttonNav = (Button)FindViewById(Resource.Id.navigating);
            buttonNav.Click += delegate
            {
                var intent = new Intent(this, typeof(NavigateRouteActivity));
                StartActivity(intent);
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

    }
}

