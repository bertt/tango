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
        Frame currentFrame;
        Zone zone;

        [Preserve]
        public BertApp(ApplicationOptions options) : base(options) { }

        protected override void Start()
        {
            var scene = new Scene(Context);
            var octree = scene.CreateComponent<Octree>();

            var cameraNode = scene.CreateChild(name: "Camera");
            var camera = cameraNode.CreateComponent<Urho.Camera>();
            zone = scene.CreateComponent<Zone>();
            zone.AmbientColor = new Color(1, 1, 1) * 0.2f;

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

            var mutantNode = scene.CreateChild();
            mutantNode.Position = new Vector3(0, -0.5f, 0.5f); // 50cm Y, 50cm Z
            mutantNode.SetScale(0.3f);
            var model = mutantNode.CreateComponent<StaticModel>();
            model.CastShadows = true;
            model.Model = ResourceCache.GetModel("Models/Box.mdl");
            model.Material = ResourceCache.GetMaterial("Materials/DefaultGrey.xml");


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

        private void OnARFrameUpdated(Frame arFrame)
        {
            currentFrame = arFrame;
            var anchors = arFrame.UpdatedAnchors;
            var pointcloud = arFrame.AcquirePointCloud();
            var capacity = pointcloud.Points.Capacity();
            var points = pointcloud.Points;

            for(var i = 0; i < capacity; i++)
            {
                var point = points.Get(i);
            }


            var timestamp = pointcloud.Timestamp;
            fps.AdditionalText = "Capacity: " + capacity;

            //TODO: visulize anchors (don't forget ARCore uses RHD coordinate system)

            // Adjust our ambient light based on the light estimates ARCore provides each frame
            var lightEstimate = arFrame.LightEstimate;
            // fps.AdditionalText = "Intensity: " + lightEstimate?.PixelIntensity.ToString("F1");
            zone.AmbientColor = new Color(1, 1, 1) * ((lightEstimate?.PixelIntensity ?? 0.2f) / 2f);
        }
    }
}