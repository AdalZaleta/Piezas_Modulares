using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generador_Mapas : MonoBehaviour
{
    Mapa[] generado = new Mapa[8];
    public Grid grid;
    public GameObject prefab;
    public Vector3Int PositionGrid;
    public LayerMask mascaras;

    void Start()
    {
        PositionGrid = new Vector3Int(Random.Range(-4, 2), Random.Range(0, 11), 0);
        prefab.transform.position = grid.CellToWorld(PositionGrid);
        for(int i = 0; i < generado.Length; i++)
        {
            generado[i] = new Mapa(60, grid, prefab, mascaras);
            generado[i].makeRandomMap(i);
        }
    }

}

public class Mapa
{
    Grid grid;
    GameObject prefab;
    LayerMask maska;
    public Ficha[] piezas;
    int sizeMap;
    int checkedPiece = 0;
    Vector3Int _PositionGrid;
    Queue visitables = new Queue();

    float wishDificult, difucltObt;
    int Steps, canReach;

    public Mapa(int _sizeMap, Grid _grid, GameObject _prefab, LayerMask _mask) 
    {
        sizeMap = _sizeMap;
        piezas = new Ficha[sizeMap];
        grid = _grid;
        prefab = _prefab;
        maska = _mask;
    }

    public void makeRandomMap(int _y)
    {
        RaycastHit hit;
        int breakPoint = 1;
        GameObject go;
        while(checkedPiece < sizeMap)
        {
            _PositionGrid = new Vector3Int(Random.Range(-4, 2), Random.Range(0, 11), 0);
            if (!Physics.Raycast(grid.CellToWorld(_PositionGrid) + (Vector3.up * (_y + 1)), Vector3.down, out hit, 1.0f, maska))
            {
                Debug.Log("Did not Hit");
                go = GameObject.Instantiate(prefab, grid.CellToWorld(_PositionGrid) + (Vector3.up * _y), Quaternion.identity);
                go.transform.SetParent(GameObject.Find("Mapa_"+ (_y + 1)).transform);
                piezas[checkedPiece] = go.GetComponent<Ficha>();
                piezas[checkedPiece].myType = (Ficha.tipoDeFicha)Random.Range(0, 13);
                piezas[checkedPiece].SelectPiece();
                checkedPiece++;
            }
            if(breakPoint >= 2000)
                break;
            breakPoint++;
            Debug.Log("breakpoint");
        }
    }   

    ~Mapa() {}
}
