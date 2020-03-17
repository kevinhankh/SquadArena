using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    GameObject Selected;
    public List<GameObject> PlayerSquad;

    const float RAYCAST_RANGE = 1000000;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectUnit();
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (Selected != null)
            {
                MoveUnit();
            }
        }
    }

    public void AddUnit(GameObject p)
    {
        PlayerSquad.Add(p);
    }

    public void RemoveUnit(GameObject p)
    {
        PlayerSquad.Remove(p);
    }

    public List<GameObject> GetPlayerSquad()
    {
        return PlayerSquad;
    }

    void SelectUnit()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, RAYCAST_RANGE))
        {
            if (Selected != null)
            {
                Selected.transform.GetChild(1).gameObject.SetActive(false);
            }
            if (hit.transform.gameObject.tag == "PlayerSquad")
            {
                if(hit.transform.parent == null)
                {
                    Selected = hit.transform.gameObject;
                    Selected.transform.GetChild(1).gameObject.SetActive(true);
                }
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

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, RAYCAST_RANGE))
        {
            switch (Selected.name)
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
