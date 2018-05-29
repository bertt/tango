using Com.Google.Atap.Tangoservice;

namespace App1
{
    public class ConfigInitialize
    {
        public static TangoConfig SetupTangoConfig(Tango tango)
        {
            // Create a new Tango Configuration and enable the MotionTrackingActivity API.
            TangoConfig config = tango.GetConfig(TangoConfig.ConfigTypeDefault);
            config.PutBoolean(TangoConfig.KeyBooleanMotiontracking, true);
            // Tango service should automatically attempt to recover when it enters an invalid state.
            config.PutBoolean(TangoConfig.KeyBooleanAutorecovery, true);
            TangoConfig.ConfigTypeAreaDescription
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

    }
}