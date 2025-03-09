using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [SerializeField] private Terrain _terrain;
    [SerializeField] private float _detailDistance = 500f;
    [SerializeField] private float _treeDistance = 500f;

    private void Awake()
    {
        _terrain.detailObjectDistance = _detailDistance;
        _terrain.treeDistance = _treeDistance;
    }
}