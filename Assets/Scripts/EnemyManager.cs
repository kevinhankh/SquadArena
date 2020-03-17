using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> Enemies;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddEnemy(GameObject e)
    {
        Enemies.Add(e);
    }

    public void RemoveEnemy(GameObject e)
    {
        Enemies.Remove(e);
    }

    public List<GameObject> GetEnemies()
    {
        return Enemies;
    }
}
