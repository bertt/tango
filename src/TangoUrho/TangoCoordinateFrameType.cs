using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App1
{
    /// <summary>
    /// Tango coordinate frame enumerations.
    /// </summary>
    public enum TangoCoordinateFrameType
    {
        /// <summary>
        /// Coordinate system for the entire Earth.
        ///
        /// See WGS84: [http://en.wikipedia.org/wiki/World_Geodetic_System].
        /// </summary>
        TANGO_COORDINATE_FRAME_GLOBAL_WGS84 = 0,

        /// <summary>
        /// Origin within a saved area description.
        /// </summary>
        TANGO_COORDINATE_FRAME_AREA_DESCRIPTION,

        /// <summary>
        /// Origin when the device started tracking.
        /// </summary>
        TANGO_COORDINATE_FRAME_START_OF_SERVICE,

        /// <summary>
        /// Immediately previous device pose.
        /// </summary>
        TANGO_COORDINATE_FRAME_PREVIOUS_DEVICE_POSE,

        /// <summary>
        /// Device coordinate frame.
        /// </summary>
        TANGO_COORDINATE_FRAME_DEVICE,

        /// <summary>
        /// Inertial Measurement Unit.
        /// </summary>
        TANGO_COORDINATE_FRAME_IMU,

        /// <summary>
        /// Display coordinate frame.
        /// </summary>
        TANGO_COORDINATE_FRAME_DISPLAY,

        /// <summary>
        /// Color camera.
        /// </summary>
        TANGO_COORDINATE_FRAME_CAMERA_COLOR,

        /// <summary>
        /// Depth camera.
        /// </summary>
        TANGO_COORDINATE_FRAME_CAMERA_DEPTH,

        /// <summary>
        /// Fisheye camera.
        /// </summary>
        TANGO_COORDINATE_FRAME_CAMERA_FISHEYE,

        /// <summary>
        /// An invalid frame.
        /// </summary>
        TANGO_COORDINATE_FRAME_INVALID,

        /// <summary>
        /// Maximum allowed.
        /// </summary>
        TANGO_MAX_COORDINATE_FRAME_TYPE
    }
}