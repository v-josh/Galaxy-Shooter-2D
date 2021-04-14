using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    private float _enemySpawnTime = 5.0f;

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private GameObject _enemyContainer;

    //Private Variables
    private bool _stopSpawning = false;


    // Start is called before the first frame update
    void Start()
    {
        if(_enemyPrefab != null)
        {
            StartCoroutine(SpawnRoutine() );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    IEnumerator SpawnRoutine()
    {
        while (!_stopSpawning)
        {
            //Generate an enemy at a Random number between -8 and 8 on the X axis, 7 on the Y, and 0 at Z
            //The rotation of the clone is the same as the original and attach it to the Enemy Container
            Instantiate(_enemyPrefab, new Vector3(Random.Range(-8, 8), 7f, 0f), Quaternion.identity, _enemyContainer.transform);
            yield return new WaitForSeconds(_enemySpawnTime);

            
        }
    }
}
