using System;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using Com.Google.Atap.Tangoservice;
using Java.Lang;

namespace App1
{
    public class SaveAdfTask : AsyncTask
    {
        private string adfName;
        private Tango tango;
        private string Tag = "SaveAdfTask";
        private Context context;

        public SaveAdfTask(Context context, Tango tango, string adfName)
        {
            this.context = context;
            this.adfName = adfName;
            this.tango = tango;
        }
        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            Log.Debug(Tag, "Starting saving adf...");
            var id = tango.SaveAreaDescription();
            Log.Debug(Tag, "ADF saved, id: " + id);
            var metadata = tango.LoadAreaDescriptionMetaData(id);
            metadata.Set(TangoAreaDescriptionMetaData.KeyName, Encoding.ASCII.GetBytes(adfName));
            tango.SaveAreaDescriptionMetadata(id, metadata);
            return adfName;
        }

        protected override void OnPostExecute(Java.Lang.Object result)
        {
            Toast.MakeText(context, "Adf saved:" + result, ToastLength.Long).Show();
            base.OnPostExecute(result);
        }

    }
}