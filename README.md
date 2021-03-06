# tango

## Hardware

### Lenovo Phab 2 Pro

<img src="tango.jpg">

### Asus Zenfone AR

<img src= "http://gadgethubspot.com/wp-content/myimages/2017/07/zpa2.png"/>

## C samples

todo

## Java samples

- $ git clone https://github.com/googlesamples/tango-examples-java.git

- Start Android Studio

- Import Project (Eclipse ADT, Gradle, etc. )

- Open java_basic_examples directory

- run/debug on device

## Unity3D

### 1] Installation Hello World

sample: 1_only_a_box

- Unity3D -> new project

- Build settings -> Android -> Switch platform

- Player settings -> Other settings -> Identification -> Package Name change to something

- import package -> custom package -> TangoSDK_Ikariotikos_Unity5.unitypackage

- Only when using Unity2017: replace google_unity_wrapper.aar (otherwise there will be crash on startup)

- add TangoPrefabs -> Tango Manager

- replace default camera with Tango camera

- add a box and a texture for color_

- Enable Tango Manager - Video Overlay with Method = Texture and Raw Bytes

### 2] Demo motion tracking

sample: 2_motion_tracking

- Add some object (like a sphere)

- Tango Camera -> Enable Tango AR Screen (Script)

- Tango Camera -> Camera -> Clear flags -> Solid Color


<img src="2_motion_tracking.jpg" height="400">

## 3] Demo list adfs

Sample: 3_list_adfs

- Add Tango camera

- Tango Manager -> Auto-connect to service -> disable

- Tango Manager -> Tango Application (Script) -> Pose Mode -> Local Area Description (Load existing)

- add script with:

```
using UnityEngine;
using Tango;


public class startup : MonoBehaviour, ITangoLifecycle
{
    private TangoApplication m_tangoApplication;
    // Use this for initialization
    void Start () {
        Debug.Log("ajax list adf startup" );

        m_tangoApplication = FindObjectOfType<TangoApplication>();
        if (m_tangoApplication != null)
        {
            m_tangoApplication.Register(this);
            m_tangoApplication.RequestPermissions();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTangoPermissions(bool permissionsGranted)
    {
        if (permissionsGranted)
        {
            AreaDescription[] list = AreaDescription.GetList();
        
            if (list.Length > 0)
            {
                foreach (var adf in list)
                {
                    Debug.Log("ajax adf found:" + adf.m_uuid);
                }
            }
            else
            {
                // No Area Descriptions available.
                Debug.Log("ajax No area descriptions available.");
            }
        }
    }

    public void OnTangoServiceConnected()
    {
    }

    public void OnTangoServiceDisconnected()
    {
    }
}
``` 

- Connect Android Device manager and search for string 'ajax'

<img src="3_list_adfs.png">

### 4] Demo Area learning

Prerequisites: There should be an ADF available of same room you're now in. Otherwise no localisation is possble and the image will
not disappear. Code loads the last available ADF file.

https://web.archive.org/web/20170326085044/https://developers.google.com/tango/apis/unity/unity-howto-area-learning

- Tango Manager -> Auto-connect to service -> disable

- Tango Manager -> Tango Application (Script) -> Pose Mode -> Local Area Description (Load existing)

- Add GameObject with script:

```
using System.Collections;
using UnityEngine;
using Tango;

public class AreaLearningStartup : MonoBehaviour, ITangoLifecycle
{
    private TangoApplication m_tangoApplication;

    public void Start()
    {
        m_tangoApplication = FindObjectOfType<TangoApplication>();
        if (m_tangoApplication != null)
        {
            m_tangoApplication.Register(this);
            m_tangoApplication.RequestPermissions();
        }
    }

    public void OnTangoPermissions(bool permissionsGranted)
    {
        if (permissionsGranted)
        {
            AreaDescription[] list = AreaDescription.GetList();
            AreaDescription mostRecent = null;
            AreaDescription.Metadata mostRecentMetadata = null;
            if (list.Length > 0)
            {
                // Find and load the most recent Area Description
                mostRecent = list[0];
                mostRecentMetadata = mostRecent.GetMetadata();
                foreach (AreaDescription areaDescription in list)
                {
                    AreaDescription.Metadata metadata = areaDescription.GetMetadata();
                    if (metadata.m_dateTime > mostRecentMetadata.m_dateTime)
                    {
                        mostRecent = areaDescription;
                        mostRecentMetadata = metadata;
                    }
                }

                m_tangoApplication.Startup(mostRecent);
            }
            else
            {
                // No Area Descriptions available.
                Debug.Log("No area descriptions available.");
            }
        }
    }

    public void OnTangoServiceConnected()
    {
    }

    public void OnTangoServiceDisconnected()
    {
    }
}
```

- Add UI 

UI -> canvas

Set image to: Assets -> Tango SDK -> examples -> common -> textures -> relocalize_screen

Click 'set native size'

- Select Tango Manager -> Add script RelocalizingOverlay (Assets > TangoSDK > Examples > AreaLearning > Scripts )

- Set property RelocalizingOverlay -> set property Relocalization Overlay to Canvas -> Image


