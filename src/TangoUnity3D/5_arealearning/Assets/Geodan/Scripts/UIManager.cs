using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public enum UIElements
    {
        MaakRoute,
        LoopRoute,
        Instellingen,
        StartNavigatie,
        Laden,
        StopNavigatie,
        StartOpname,
        StopOpname,
        Hervatten,
        Opslaan,
        Annuleren,
        RouteBeheer,
        Profiel
    }

    public Color TabActive;
    public Color TabDefault;

    public GameObject TabMaakRoute;
    public GameObject TabLoopRoute;
    public GameObject NavigationTitleBar;
    public GameObject PanelMaakRoute;
    public GameObject PanelMaakRouteStop;
    public GameObject PanelMaakRouteStopOpslaan;
    public GameObject PanelInstellingen;
    public GameObject PanelInstellingenProfiel;
    public GameObject PanelInstellingenRouteBeheer;
    public GameObject PanelLoopRoute;
    public GameObject PanelLoopRouteLaden;
    public GameObject PanelLoopRoutebezigNavigatie;
    public GameObject PanelMaakRouteOpslaan;

    private RecordingSystem rec;
    private NavigatorSystem nav;
    private bool showSettings;
    private GameObject currentPanel;

    public void Start() {
        currentPanel = PanelLoopRoute;
        
        var gm = FindObjectOfType<GameManager>();

        rec = gm.GetComponent<RecordingSystem>();
        nav = gm.GetComponent<NavigatorSystem>();
        //showSettings = Settings.activeSelf;
    }

    public void ClickUIElement(string element)
    {
        try
        {
            UIElements e = (UIElements)System.Enum.Parse(typeof(UIElements), element);

            switch (e)
            {
                case UIElements.MaakRoute:
                    {
                        //update menu bar
                        NavigationTitleBar.GetComponentInChildren<Text>().text = "Maak route";
                        TabMaakRoute.GetComponent<Image>().color = TabActive;
                        TabLoopRoute.GetComponent<Image>().color = TabDefault;

                        //stop all activities
                        rec.StopRecording();
                        nav.StopNavigation();

                        //switch panel to first screen from tab
                        SetPanel(PanelMaakRoute);
                    }
                    break;
                case UIElements.LoopRoute:
                    {
                        //update menu bar
                        NavigationTitleBar.GetComponentInChildren<Text>().text = "Loop route";
                        TabMaakRoute.GetComponent<Image>().color = TabDefault;
                        TabLoopRoute.GetComponent<Image>().color = TabActive;

                        //stop all activities
                        rec.StopRecording();
                        nav.StopNavigation();

                        //switch panel to first screen from tab
                        SetPanel(PanelLoopRoute);
                    }
                    break;
                case UIElements.Instellingen:
                    {
                        //update menu bar
                        NavigationTitleBar.GetComponentInChildren<Text>().text = "Instellingen";
                        TabMaakRoute.GetComponent<Image>().color = TabDefault;
                        TabLoopRoute.GetComponent<Image>().color = TabDefault;

                        //stop all activities
                        rec.StopRecording();
                        nav.StopNavigation();

                        //switch panel to first screen from tab
                        SetPanel(PanelInstellingen);
                    }
                    break;
                case UIElements.StartNavigatie:
                    {
                        nav.StartNavigation();

                        SetPanel(PanelLoopRoutebezigNavigatie);
                    }
                    break;
                case UIElements.Laden:
                    {
                        SetPanel(PanelLoopRouteLaden);
                    }
                    break;
                case UIElements.StopNavigatie:
                    {
                        nav.StopNavigation();

                        SetPanel(PanelLoopRoute);
                    }
                    break;
                case UIElements.StartOpname:
                    {
                        rec.StartRecording();

                        SetPanel(PanelMaakRouteStop);
                    }
                    break;
                case UIElements.StopOpname:
                    {
                        //2do build pause recording


                        SetPanel(PanelMaakRouteStopOpslaan);
                    }
                    break;
                case UIElements.Hervatten:
                    {
                        //2do build hervatten pause recording


                        SetPanel(PanelMaakRouteStop);
                    }
                    break;
                case UIElements.Opslaan:
                    {
                        //2do build pause recording
                        rec.StopRecording();
                        //BERT: rec.Save()?

                        SetPanel(PanelMaakRouteOpslaan);
                    }
                    break;
                case UIElements.Annuleren:
                    {
                        //2do build cancel recording

                        rec.StopRecording();
                        SetPanel(PanelMaakRoute);
                    }
                    break;
                case UIElements.RouteBeheer:
                    {
                        SetPanel(PanelInstellingenRouteBeheer);
                    }
                    break;
                case UIElements.Profiel:
                    {
                        SetPanel(PanelInstellingenProfiel);
                    }
                    break;
                default:
                    break;
            }
        }
        catch (System.Exception)
        {
            Debug.LogErrorFormat("Parse: Can't convert {0} to enum, please check the spell.", element);
        }
    }
   
    private void SetPanel(GameObject panel)
    {
        if (currentPanel != null)
            currentPanel.SetActive(false);

        panel.SetActive(true);
        currentPanel = panel;
    }

    //public void ToggleNew() {
    //    //toggle on/off
    //    bool state = rec.ToggleOnOff();

    //    //UI stuff
    //    Load.SetActive(!state);
    //    Navigate.SetActive(!state);

    //    if (state)
    //    {
    //        New.GetComponentInChildren<Text>().text = "Stop";
    //        Markers.SetActive(true);
    //    }
    //    else
    //    {
    //        New.GetComponentInChildren<Text>().text = "New";
    //        Markers.SetActive(false);
    //    }
    //}

    //public void ToggleNavigation()
    //{
    //    //toggle on/off
    //    bool state = nav.ToggleOnOff();

    //    //UI stuff
    //    Load.SetActive(!state);
    //    New.SetActive(!state);

    //    if (state)
    //    {
    //        Navigate.SetActive(false);
    //   //     Markers.SetActive(true);
    //        Stop.SetActive(true);
    //    }
    //    else
    //    {
    //        Navigate.SetActive(true);
    //    //    Markers.SetActive(false);
    //        Stop.SetActive(false);
    //    }
    //}

    //public void ToggleSettings()
    //{
    //    //toggle on/off
    //    showSettings = !showSettings;

    //    //UI stuff
    //    Settings.SetActive(showSettings);
    //    Load.SetActive(!showSettings);
    //    New.SetActive(!showSettings);
    //    Navigate.SetActive(!showSettings);   
    //}
}
