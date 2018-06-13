
using System;
using Com.Google.AR.Core;
using Urho;
using Urho.Droid;
using Urho.Resources;

namespace App1
{
    public class BertApp : Application
    {
        Viewport viewport;
        public ARCoreComponent ArCore { get; private set; }
        MonoDebugHud fps;

        [Preserve]
        public BertApp(ApplicationOptions options) : base(options) { }

        protected override void Start()
        {
            // 3d scene with octree and ambient light
            var scene = new Scene(Context);
            var octree = scene.CreateComponent<Octree>();

            var cameraNode = scene.CreateChild(name: "Camera");
            var camera = cameraNode.CreateComponent<Urho.Camera>();

            var lightNode = cameraNode.CreateChild();
            lightNode.SetDirection(new Vector3(1f, -1.0f, 1f));
            var light = lightNode.CreateComponent<Light>();
            light.Range = 10;
            light.LightType = LightType.Directional;
            light.CastShadows = true;
            Renderer.ShadowMapSize *= 4;

            // Viewport
            viewport = new Viewport(Context, scene, camera, null);
            Renderer.SetViewport(0, viewport);

            // ARCore component
            ArCore = scene.CreateComponent<ARCoreComponent>();
            ArCore.ARFrameUpdated += OnARFrameUpdated;
            ArCore.ConfigRequested += ArCore_ConfigRequested;
            ArCore.Run();

            fps = new MonoDebugHud(this);
            fps.FpsOnly = true;
            fps.Show(Color.Blue, 25);

        }

        private void ArCore_ConfigRequested(Config config)
        {
            config.SetPlaneFindingMode(Config.PlaneFindingMode.Horizontal);
            config.SetLightEstimationMode(Config.LightEstimationMode.AmbientIntensity);
            config.SetUpdateMode(Config.UpdateMode.LatestCameraImage); //non blocking
        }

        private void OnARFrameUpdated(Frame obj)
        {
            throw new NotImplementedException();
        }
    }
}