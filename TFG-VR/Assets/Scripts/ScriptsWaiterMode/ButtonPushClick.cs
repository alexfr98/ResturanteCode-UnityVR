using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonPushClick : MonoBehaviour
{
    //Probar a hacerlo todo public para poder mirar como va cambiando los valores y comparar con los valores que yo he declarado
    private float MinLocalY;
    private float MaxLocalY;

    private bool isWaiting = false;
    private bool isBeingTouched = false;
    private bool isClicked = false;

    private GameObject panel;

    public Material blueMat;
    // Start is called before the first frame update

    private Vector3 buttonUpPosition;
    private Vector3 buttonDownPosition;

    void Start()
    {

        MinLocalY = transform.localPosition.y - 0.04f;
        MaxLocalY = transform.localPosition.y;

        //Start with button up
        buttonDownPosition = new Vector3(transform.localPosition.x, MinLocalY, transform.localPosition.z);
        buttonUpPosition = new Vector3(transform.localPosition.x, MaxLocalY, transform.localPosition.z);

    }

    // Update is called once per frame
    void Update()
    {

        if (!isClicked)
        {
            //Se supone que aquí bloqueamos la altura máxima del botón
            if(transform.localPosition.y >= MaxLocalY)
            {
                //Volvemos a la posición 
                transform.localPosition = buttonUpPosition;
            }

            //Cuando lleguemos a la posición por debajo de MinLocalY, entregaremos el pedido y bloqueamos el botón en esa posición
            if(transform.localPosition.y <= MinLocalY)
            {
                isClicked = true;
                transform.localPosition = buttonDownPosition;
                OnButtonDown();
            }
        }
    }
        
    public void OnButtonDown()
    {
        panel = GameObject.Find("BasicPanel");

        //Enviamos el pedido para ver si está bien



        ComandaWaiter send_comanda = panel.GetComponent<AttInstructions>().GetHamburguer();
        controladorPartidaWaiter.current.arriveOrder(send_comanda);

        
        //Devolvemos el botón a su lugar inicial 2 segundos más tarde de haberlo pulsado
        StartCoroutine("Waiting");
        StartCoroutine("ButtonUp");
        
    }

    IEnumerator Waiting()
    {
        isWaiting = true;
        yield return new WaitForSeconds(3);
        isWaiting = false;

    }
    IEnumerator ButtonUp()
    {
        while (isWaiting)
        {
            yield return new WaitForSeconds(0.1f);
        }

        GetComponent<MeshRenderer>().material = blueMat;
        transform.localPosition = new Vector3(transform.localPosition.x, MaxLocalY, transform.localPosition.z);
        isClicked = false;
    }


}
