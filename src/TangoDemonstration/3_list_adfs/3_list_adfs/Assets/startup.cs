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
