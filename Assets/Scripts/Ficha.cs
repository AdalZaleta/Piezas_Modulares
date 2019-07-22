using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ficha : MonoBehaviour
{
    public GameObject[] piezas;
    public Transform[] conexiones;
    public List<Ficha> vecinos = new List<Ficha>();
    public tipoDeFicha myType;
    public LayerMask maska;

    Queue Vecinos = new Queue();

    void Start()
    {
        Invoke("KnowTheNeighbour", 2.0f);
    }

    [ContextMenu("Select The Piece")]
    public void SelectPiece()
    {
        foreach(GameObject g in piezas)
        {
            g.SetActive(false);
        }
        piezas[(int)myType].SetActive(true);
    }

    public void KnowTheNeighbour()
    {
        RaycastHit hot;
        GameObject temp;
        for(int j = 0; j < transform.childCount; j++)
        {
            if(transform.GetChild(j).gameObject.activeSelf)
            {
                temp = transform.GetChild(j).gameObject;
                conexiones = temp.gameObject.GetComponentsOnlyInChildren<Transform>();
            }
        }

        for(int i = 0 ; i < conexiones.Length; i++)
        {
            if(Physics.Raycast(conexiones[i].position, Vector3.down, out hot, 1.0f, maska))
            {
                temp = hot.collider.gameObject;
                if(temp.GetComponentInParent<Ficha>())
                    vecinos.Add(temp.GetComponentInParent<Ficha>());
            }
        }
    }

    public enum tipoDeFicha
    {
        Abierta = 0,
        Brazo_Derecho = 1,
        Brazo_Izquierdo = 2,
        Centro = 3,
        Cruz = 4,
        Curva_Abierta = 5,
        Curva_Cerrada = 6,
        K = 7,
        Llena = 8,
        Recta = 9,
        Sin_Salida = 10,
        Tridente = 11,
        Y = 12
    }
}
