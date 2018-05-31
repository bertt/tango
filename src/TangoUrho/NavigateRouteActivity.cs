using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace App1
{
    [Activity(Label = "NavigateRouteActivity")]
    public class NavigateRouteActivity : Activity
    {
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
        }
    }
}