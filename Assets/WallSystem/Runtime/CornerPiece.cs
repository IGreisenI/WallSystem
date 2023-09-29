using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerPiece : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;

    public void Init(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    public void SetModel(GameObject prefab)
    {
        if (_prefab) DestroyImmediate(_prefab);

        if (!prefab) return;
            
        _prefab = Instantiate(prefab, transform);
        _prefab.transform.position = transform.position;
        _prefab.transform.rotation = Quaternion.RotateTowards(_prefab.transform.rotation, transform.rotation, 360);
    }

}
