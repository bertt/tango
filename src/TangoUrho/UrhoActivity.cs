using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Urho;
using Urho.Droid;

namespace App1
{
    [Activity(Label = "UrhoActivity")]
    public class UrhoActivity : Activity
    {
        Urho.Application app;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var mLayout = new RelativeLayout(this);
            // var surface = UrhoSurface.CreateSurface(this);// (this, , true);
            var scene = new DemoScene(new ApplicationOptions("Data"));
            var surface = UrhoSurface.CreateSurface(this);
            mLayout.AddView(surface);
            SetContentView(mLayout);
            // var type = Type.GetType(Intent.GetStringExtra("Type"));

            // next thing works:
            // app = await surface.Show<UrhoScene>(new ApplicationOptions("Data"));

            // next thing works:
            app = await surface.Show<BertApp>(new ApplicationOptions("Data"));

            // app = await surface.Show(Type.GetType(Intent.GetStringExtra("Type")), new ApplicationOptions("Data"));


        }

        protected override void OnResume()
        {
            UrhoSurface.OnResume();
            base.OnResume();
        }

        protected override void OnPause()
        {
            UrhoSurface.OnPause();
            base.OnPause();
        }

        public override void OnLowMemory()
        {
            UrhoSurface.OnLowMemory();
            base.OnLowMemory();
        }

        protected override void OnDestroy()
        {
            UrhoSurface.OnDestroy();
            base.OnDestroy();
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (e.KeyCode == Android.Views.Keycode.Back)
            {
                this.Finish();
                return false;
            }

            if (!UrhoSurface.DispatchKeyEvent(e))
                return false;
            return base.DispatchKeyEvent(e);
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            UrhoSurface.OnWindowFocusChanged(hasFocus);
            base.OnWindowFocusChanged(hasFocus);
        }
    }
}
