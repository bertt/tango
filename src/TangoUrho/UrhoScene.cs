using Urho;
using Urho.Gui;

namespace App1
{
    public class UrhoScene : Application
    {
        private string Tag = "UrhoScene";

        [Preserve]
        public UrhoScene() : base(new ApplicationOptions(assetsFolder: "Data") { Height = 1024, Width = 576, Orientation = ApplicationOptions.OrientationType.Portrait }) { }

        [Preserve]
        public UrhoScene(ApplicationOptions opts) : base(opts) { }


        protected override void Start()
        {
            base.Start();
            var cache = ResourceCache;
            var helloText = new Text()
            {
                Value = "Hello World from UrhoSharp",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            helloText.SetColor(new Color(0f, 1f, 0f));
            helloText.SetFont(font: cache.GetFont("Fonts/Anonymous Pro.ttf"), size: 30);
            UI.Root.AddChild(helloText);

            //Graphics.SetWindowIcon(cache.GetImage("Textures/UrhoIcon.png"));
            //Graphics.WindowTitle = "UrhoSharp Sample";

            // Subscribe to Esc key:
            // Input.SubscribeToKeyDown(args => { if (args.Key == Key.Esc) Exit(); });
        }


    }
}