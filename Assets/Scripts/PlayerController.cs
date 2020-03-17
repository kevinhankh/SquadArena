using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    GameObject Selected;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SelectUnit();
        }
        if (Input.GetMouseButtonDown(1))
        {
            if(Selected!=null)
            {
                MoveUnit();
            }
        }
    }

    void SelectUnit()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 200))
        {
            if(Selected!=null)
            {
                Selected.transform.GetChild(1).gameObject.SetActive(false);
            }
            if(hit.transform.gameObject.tag == "PlayerSquad")
            {
                Selected = hit.transform.gameObject;
                Selected.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                Selected = null;
            }
        }
    }

    void MoveUnit()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 200))
        {
            switch(Selected.name)
            {
                case "CubeUnit":
                    Selected.GetComponent<CubeUnit>().Move(hit.point);
                    break;
                case "TriangleUnit":
                    Selected.GetComponent<TriangleUnit>().Move(hit.point);
                    break;
                case "SphereUnit":
                    Selected.GetComponent<SphereUnit>().Move(hit.point);
                    break;
            }
        }
    }
}
