using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{

    public GameObject PlayerPrefab;

    void Awake()
    {
        Instantiate(PlayerPrefab, transform.position, Quaternion.identity);
    }

}
