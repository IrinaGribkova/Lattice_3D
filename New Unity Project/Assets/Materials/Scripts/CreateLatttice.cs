using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class CreateLatttice : MonoBehaviour
{
   
    public GameObject menu;
    public GameObject start_modeling;
    public GameObject start_calculating;
    public GameObject title_text;
    public Canvas open_menu;
    public GameObject exit_panel;
    public GameObject warning_panel_add_vector;
    public GameObject warning_panel_execute;
    public Scrollbar scrollbar;
    public GameObject OrthLattice;
    public Material color;
    public Slider opacity_ortho;
   // OrthonormalizedLattice ortho_lattice;

    // Start is called before the first frame update

    void Start()
    {
        menu.gameObject.SetActive(true);
        exit_panel.gameObject.SetActive(false);
        warning_panel_add_vector.gameObject.SetActive(false);
        warning_panel_execute.gameObject.SetActive(false);

       // ortho_lattice =  new OrthonormalizedLattice(3, OrthLattice, color);
    }

    public void Back()
    {
        menu.gameObject.SetActive(true);
    }

    void Close_menu()
    {
        open_menu.gameObject.SetActive(false);
    }

    public void Reset()
    {
        //SceneManager.LoadScene(0);
       Camera.main.transform.position = new Vector3(0,1,-6);
       Camera.main.transform.rotation = new Quaternion(0, 0, 0,0);
       Camera.main.transform.localScale = new Vector3(1,1,1);
    }

    public void CloseApp()
    {
        // Application.Quit();
        exit_panel.gameObject.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void CloseExitPanel()
    {
        exit_panel.gameObject.SetActive(false);
    }

    public void Start_modeling()
    {
        Close_menu();
    }

    public void Change_result()
    {
        if (scrollbar.value == 0)
        {
            Debug.Log("ScrollRect Changed: " + scrollbar.value);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
