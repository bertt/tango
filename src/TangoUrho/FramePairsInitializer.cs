using System.Collections.Generic;
using Com.Google.Atap.Tangoservice;

namespace App1
{
    public static class FramePairsInitializer
    {
        public static List<TangoCoordinateFramePair>  GetPairs()
        {
            var framePairs = new List<TangoCoordinateFramePair>()
            {
                new TangoCoordinateFramePair(
                    TangoPoseData.CoordinateFrameStartOfService,
                    TangoPoseData.CoordinateFrameDevice)
            };

            framePairs.Add(new TangoCoordinateFramePair(TangoPoseData.CoordinateFrameAreaDescription, TangoPoseData.CoordinateFrameDevice));
            framePairs.Add(new TangoCoordinateFramePair(TangoPoseData.CoordinateFrameAreaDescription, TangoPoseData.CoordinateFrameStartOfService));

            return framePairs;
        }
    }
}