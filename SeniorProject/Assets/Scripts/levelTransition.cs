using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class levelTransition : MonoBehaviour
{
    public string SceneName;
    public Text textEle;

    void OnTriggerStay(Collider other)
    {

        if (Input.GetMouseButtonUp(0))
        {

            Debug.Log("Levels passed: "+ datehandler.levelT);
            if (datehandler.levelT <= 5)
            {
                SceneManager.LoadScene(SceneName);
                datehandler.levelT++;
            }
            if(datehandler.levelT > 5)
            {
                SceneManager.LoadScene("Ending");
            }

        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log("Levels passed: " + datehandler.levelT);
            if (datehandler.levelT <= 5)
            {
                SceneManager.LoadScene(SceneName);
                datehandler.levelT++;
            }
            if (datehandler.levelT > 5)
            {
                SceneManager.LoadScene("Ending");
            }

        }
    }
}

