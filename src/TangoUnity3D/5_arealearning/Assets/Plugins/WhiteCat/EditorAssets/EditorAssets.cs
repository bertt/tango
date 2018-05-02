#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace WhiteCatEditor
{
	/// <summary>
	/// EditorAssets
	/// </summary>
	public class EditorAssets : FolderLocator<EditorAssets>
	{
		public static readonly Texture addTexture;
		public static readonly Texture removeTexture;

		public static readonly Texture refreshTexture;
		public static readonly Texture bigDiamondTexture;
		public static readonly Texture smallDiamondTexture;

		public static readonly Texture editTexture;

		public static readonly Texture selectNodeTexture;
		public static readonly Texture selectLineTexture;
		public static readonly Texture moveInPlaneTexture;
		public static readonly Texture moveInSpaceTexture;
		public static readonly Texture detailInfoTexture;

		public static readonly Texture insertNodeTexture;
		public static readonly Texture insertNodeBackTexture;
		public static readonly Texture insertNodeForwardTexture;
		public static readonly Texture removeNodeTexture;

		public static readonly Texture nextTexture;
		public static readonly Texture prevTexture;

		public static readonly Texture roundGradientTexture;


		static EditorAssets()
		{
			LoadAsset(ref addTexture, "add.png");
			LoadAsset(ref removeTexture, "remove.png");

			LoadAsset(ref refreshTexture, "refresh.png");
			LoadAsset(ref bigDiamondTexture, "bigDiamond.png");
			LoadAsset(ref smallDiamondTexture, "smallDiamond.png");

			LoadAsset(ref editTexture, "edit.png");

			LoadAsset(ref selectNodeTexture, "selectNode.png");
			LoadAsset(ref selectLineTexture, "selectLine.png");
			LoadAsset(ref moveInPlaneTexture, "moveInPlane.png");
			LoadAsset(ref moveInSpaceTexture, "moveInSpace.png");
			LoadAsset(ref detailInfoTexture, "detailInfo.png");

			LoadAsset(ref insertNodeTexture, "insertNode.png");
			LoadAsset(ref insertNodeBackTexture, "insertNodeBack.png");
			LoadAsset(ref insertNodeForwardTexture, "insertNodeForward.png");
			LoadAsset(ref removeNodeTexture, "removeNode.png");

			LoadAsset(ref nextTexture, "next.png");
			LoadAsset(ref prevTexture, "prev.png");

			LoadAsset(ref roundGradientTexture, "roundGradient.png");
		}


		private static GUIContent _horizontalDragGUIContent;
		/// <summary> 水平拖动的图标 </summary>
		public static GUIContent horizontalDragGUIContent
		{
			get
			{
				if (_horizontalDragGUIContent == null)
				{
					var image = AssetDatabase.LoadAssetAtPath<Texture2D>(directory + "/horizontalDrag.png");
					_horizontalDragGUIContent = new GUIContent(image, "Drag Value");
				}
				return _horizontalDragGUIContent;
			}
		}


		private static GUIStyle _toolBarGUIStyleDark;
		private static GUIStyle _toolBarGUIStyleLight;
		/// <summary> 在场景中显示的工具条 </summary>
		public static GUIStyle toolBarGUIStyle
		{
			get
			{
				if (EditorGUIUtility.isProSkin)
				{
					if (_toolBarGUIStyleDark == null)
					{
						var background = AssetDatabase.LoadAssetAtPath<Texture2D>(directory + "/toolBarBackgroundDark.png");
						_toolBarGUIStyleDark = new GUIStyle();
						_toolBarGUIStyleDark.border = new RectOffset(1, 23, 1, 26);
						_toolBarGUIStyleDark.normal.background = background;
					}
					return _toolBarGUIStyleDark;
				}
				else
				{
					if (_toolBarGUIStyleLight == null)
					{
						var background = AssetDatabase.LoadAssetAtPath<Texture2D>(directory + "/toolBarBackgroundLight.png");
						_toolBarGUIStyleLight = new GUIStyle();
						_toolBarGUIStyleLight.border = new RectOffset(1, 23, 1, 26);
						_toolBarGUIStyleLight.normal.background = background;
					}
					return _toolBarGUIStyleLight;
				}
			}
		}

	} // class EditorAssets

} // namespace WhiteCatEditor

#endif // UNITY_EDITOR