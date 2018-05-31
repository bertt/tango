using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Com.Google.Atap.Tangoservice;
using Java.Lang;
using System.Collections.Generic;

namespace App1
{
    [Activity(Label = "ListAdfsActivity")]
    public class ListAdfsActivity : Activity
    {
        private string Tag = "ListAdfs";
        private Tango tango;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_listadfs);

            var button = (Button)FindViewById(Resource.Id.back);
            button.Click += delegate
            {
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };
            ShowAdfs();
        }

        protected override void OnResume()
        {
            base.OnResume();

        }

        private void ShowAdfs()
        {
            var adfs = new List<string>();

            tango = new Tango(this, new Runnable(() =>
            {
                var listAdfs = tango.ListAreaDescriptions();
                foreach (var uuid in listAdfs)
                {
                    var metadata = tango.LoadAreaDescriptionMetaData(uuid);
                    var name = new String(metadata.Get(TangoAreaDescriptionMetaData.KeyName)).ToString();
                    var uuid_in = new String(metadata.Get(TangoAreaDescriptionMetaData.KeyUuid)).ToString();

                    adfs.Add(name);
                }

                RunOnUiThread(() =>
                {
                    var lv = (ListView)FindViewById(Resource.Id.listView1);
                    var adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, adfs);
                    lv.Adapter = adapter;
                });
            }));
        }
    }
}