using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Generador_Mapas : MonoBehaviour
{
    Mapa[] generado = new Mapa[8];
    public GameObject[] Inicios;
    public GameObject[] Destinos;
    public Grid grid;
    public GameObject prefab;
    public LayerMask mascaras;

    Mapa[] padres = new Mapa[2];
    Mapa final;

    void Start()
    {
        for(int i = 0; i < generado.Length; i++)
        {
            generado[i] = new Mapa(59, grid, prefab, mascaras, Inicios[i], Destinos[i], 0.5f);
            generado[i].makeRandomMap(i);
            generado[i].HacerElVecindario();
            generado[i].BFSAlgoritm();
            generado[i].CalculateFitnnes();
            Debug.Log("Fitness de Generado[" + i + "] es de: " + generado[i].fitness);
        }
    }

    IEnumerator Procrear()
    {
        do
        {
            CalculateFitnnes();
            SeleccionPadres();
            CruceYMutacion();
            yield return new WaitForEndOfFrame();
        } while (difucltObt <= wishDificult);
    }

    public void SeleccionPadres()
    {
        var ordenado = piezas.OrderByDescending(v => this.fitness).ToArray();
		padres[0] = ordenado[0];
        padres[1] = ordenado[1];
    }

    public void CruceYMutacion()
    {
        Mapa temp, P1, P2;
        P1 = padres[0];
        P2 = padres[1];
        for(int i = 0; i < colores.Length; i++)
        {
            temp = colores[i].objeto.GetComponent<RawImage>().color;
            temp.r = Random.Range(0.0f, 1.0f) >= 0.5f ? P1.r : P2.r;
            temp.g = Random.Range(0.0f, 1.0f) >= 0.5f ? P1.g : P2.g;
            temp.b = Random.Range(0.0f, 1.0f) >= 0.5f ? P1.b : P2.b;


            if(Random.Range(0.0f, 1.0f) <= Mutacion.value)
            {
                temp.r = Random.Range(0.0f, 1.0f) >= 0.5f ? temp.r + 0.2f * CrossOver.value : temp.r - 0.2f * CrossOver.value;
                if(temp.r > 1.0f) temp.r = 1.0f;
                temp.g = Random.Range(0.0f, 1.0f) >= 0.5f ? temp.g + 0.2f * CrossOver.value : temp.g - 0.2f * CrossOver.value;
                if(temp.g > 1.0f) temp.g = 1.0f;
                temp.b = Random.Range(0.0f, 1.0f) >= 0.5f ? temp.b + 0.2f * CrossOver.value : temp.b - 0.2f * CrossOver.value;
                if(temp.b > 1.0f) temp.b = 1.0f;
            } 
            colores[i].objeto.GetComponent<RawImage>().color = temp;
        }

}

public class Mapa
{
    Grid grid;
    GameObject prefab, inicio, fin;
    LayerMask maska;
    public Ficha[] piezas;
    int sizeMap;
    int checkedPiece = 0;
    Vector3Int _PositionGrid;
    Queue visitables = new Queue();

    public float fitness;

    float wishDificult, difucltObt;
    int Steps = 0, canReach = 0;

    public Mapa(int _sizeMap, Grid _grid, GameObject _prefab, LayerMask _mask, GameObject _inicio, GameObject _fin, float _wishDif) 
    {
        sizeMap = _sizeMap;
        piezas = new Ficha[sizeMap];
        grid = _grid;
        prefab = _prefab;
        maska = _mask;
        inicio = _inicio;
        fin = _fin;
        wishDificult = _wishDif;
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
        }
    }

    public void HacerElVecindario()
    {
        inicio.GetComponent<Ficha>().KnowTheNeighbour();
        for(int i = 0; i < piezas.Length; i++)
        {
            piezas[i].KnowTheNeighbour();
        }
        fin.GetComponent<Ficha>().KnowTheNeighbour();
    }

    public void BFSAlgoritm()
    {
        int breakPoint = 0;
        visitables.Enqueue(inicio.GetComponent<Ficha>());
        do
        {
            Ficha temp = visitables.Count != 0 ? (Ficha)visitables.Dequeue() : null;
            if(temp == null)
                break;
            for(int j = 0; j < temp.vecinos.Count; j++)
            {
                if(temp.vecinos[j].vecinos.Contains(temp))
                {
                    temp.vecinos[j].padre = temp;
                    visitables.Enqueue(temp.vecinos[j]);
                }
            }
            if(breakPoint > 1000)
                break;
            breakPoint++;
        } while(visitables.Count != 0);


        Ficha rec = fin.GetComponent<Ficha>();
        while(rec.padre != null)
        {
            Steps++;
            rec = rec.padre != null ?  rec.padre : null;
        }
        if(rec.gameObject == inicio)
        {
            canReach = 1;
        }
        Debug.Log(canReach > 0 ? "Can Reach" : "Can't Reach");
        Debug.Log("Steps: " + Steps);
    }

    public void CalculateFitnnes()
    {
        difucltObt = wishDificult - ( 1.0f - Remap(Steps, 11, 60, 0.5f, 1.0f));
        fitness = difucltObt + canReach;
    }

    float Remap(float _Value, float _a1, float _b1, float _a2, float _b2)
    {
        return (_a2 + (_Value - _a1) * (_b2 - _a2) / (_b1 - _a1));
    }   

    ~Mapa() {}
}
