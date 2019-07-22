using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generador_Mapas : MonoBehaviour
{
    Mapa[] generado = new Mapa[8];
    public GameObject[] Inicios;
    public GameObject[] Destinos;
    public Grid grid;
    public GameObject prefab;
    public LayerMask mascaras;

    void Start()
    {
        for(int i = 0; i < generado.Length; i++)
        {
            generado[i] = new Mapa(60, grid, prefab, mascaras);
            generado[i].makeRandomMap(i);
        }
    }

    // IEnumerator Procrear()
    // {
    //     do
    //     {
    //         CalculateFitnnes();
    //         SeleccionPadres();
    //         CruceYMutacion();
    //         yield return new WaitForEndOfFrame();
    //     } while (difucltObt <= wishDificult);
    // }

    // public void SeleccionPadres()
    // {
    //     var ordenado = piezas.OrderByDescending(v => this.fitness).ToArray();
	// 	padres[0] = ordenado[0];
    //     padres[1] = ordenado[1];
    // }

    // public void CruceYMutacion()
    // {
    //     Mapa temp, P1, P2;
    //     P1 = padres[0];
    //     P2 = padres[1];
    //     for(int i = 0; i < colores.Length; i++)
    //     {
    //         temp = colores[i].objeto.GetComponent<RawImage>().color;
    //         temp.r = Random.Range(0.0f, 1.0f) >= 0.5f ? P1.r : P2.r;
    //         temp.g = Random.Range(0.0f, 1.0f) >= 0.5f ? P1.g : P2.g;
    //         temp.b = Random.Range(0.0f, 1.0f) >= 0.5f ? P1.b : P2.b;


    //         if(Random.Range(0.0f, 1.0f) <= Mutacion.value)
    //         {
    //             temp.r = Random.Range(0.0f, 1.0f) >= 0.5f ? temp.r + 0.2f * CrossOver.value : temp.r - 0.2f * CrossOver.value;
    //             if(temp.r > 1.0f) temp.r = 1.0f;
    //             temp.g = Random.Range(0.0f, 1.0f) >= 0.5f ? temp.g + 0.2f * CrossOver.value : temp.g - 0.2f * CrossOver.value;
    //             if(temp.g > 1.0f) temp.g = 1.0f;
    //             temp.b = Random.Range(0.0f, 1.0f) >= 0.5f ? temp.b + 0.2f * CrossOver.value : temp.b - 0.2f * CrossOver.value;
    //             if(temp.b > 1.0f) temp.b = 1.0f;
    //         } 
    //         colores[i].objeto.GetComponent<RawImage>().color = temp;
    //     }
    // }
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
