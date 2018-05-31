using System.Text;
using Android.OS;
using Android.Util;
using Com.Google.Atap.Tangoservice;

namespace App1
{
    public class SaveAdfTask : AsyncTask
    {
        private string adfName;
        private Tango tango;
        private string Tag = "SaveAdfTask";

        public SaveAdfTask(Tango tango, string adfName)
        {
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
            return id;
        }
    }
}