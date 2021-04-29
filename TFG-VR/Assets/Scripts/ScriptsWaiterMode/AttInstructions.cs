using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class AttInstructions : MonoBehaviour
{
    private ComandaWaiter current_hamburguer;
    private float conditionalSeparationAxisZ = 0.22f;
    private float conditionalSeparationAxisY = 0.18f;

    private GameObject debugText;
    private GameObject debugText2;

    private GameObject downBreadPanel;
    private Vector3 downBreadPanelPosition;
    private float lateralPanelScale = 1.4f;

    // Start is called before the first frame update
    void Start()
    {


        downBreadPanel = GameObject.Find("DownBreadPanel");
        downBreadPanelPosition = downBreadPanel.transform.position;
        current_hamburguer = new ComandaWaiter();

        //pruebas
    }


    private void OnTriggerStay(Collider other)
    {

        //Detectamos los paneles que hemos soltado y aún no hemos colocado
        //if (other.tag == "panel" && !other.gameObject.transform.GetComponent<OVRGrabbable>().isGrabbed)
        //{

        if (other.tag == "panel" && !other.GetComponent<OVRGrabbable>().isGrabbed && !other.GetComponent<panelScript>().getIsInPanel())
        {
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Audio/panelSound"), other.transform.position, 1.0f);
            //Queremos la posicion encima del panel y debajo de la ultima instruccion que se ha añadido.
            //Si no hemos puesto ninguna instrucción, simplemente lo posicionamos arriba de todo del panel

            if (current_hamburguer.getInstructions().Count == 0)
            {

                other.gameObject.transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, this.transform.position.y + 0.9f, this.transform.position.z);
                other.gameObject.transform.rotation = Quaternion.Euler(180, 180, 90);

                current_hamburguer.addInstruction(other.gameObject);

                //New
                if (other.GetComponent<panelScript>().getIsFor() || other.GetComponent<panelScript>().getIsIf())
                {
                    //Añadimos como instruccion el closing
                    current_hamburguer.addInstruction(other.gameObject.transform.GetChild(2).gameObject);

                }

                other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                other.gameObject.GetComponent<panelScript>().setIsInPanel(true);
                //New
                if (other.GetComponent<panelScript>().getIsIfElse())
                {
                    //Añadimos como instruccion el else y el closing
                    current_hamburguer.addInstruction(other.gameObject.transform.GetChild(2).gameObject);
                    current_hamburguer.addInstruction(other.gameObject.transform.GetChild(4).gameObject);

                }
                if (!other.gameObject.GetComponent<panelScript>().getItWasInPanel())
                {

                    controladorPartidaWaiter.current.createPanel(other.name);
                }

                //Si el freeze no está dentro se caen los paneles
            }

            //Si el panel que estoy poniendo no lo coloco entre ninguna de los paneles ya existentes, se coloca justo debajo.
            else if (other.gameObject.transform.position.y < current_hamburguer.getLastInstruction().transform.position.y)
            {

                other.gameObject.transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, current_hamburguer.getLastInstruction().transform.position.y - conditionalSeparationAxisY, this.transform.position.z);

                other.gameObject.transform.rotation = Quaternion.Euler(180, 180, 90);
                current_hamburguer.addInstruction(other.gameObject);

                //Congelamos la posición del panel. Si el freeze no está dentro del if/else se caen los paneles. 
                other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                //New. Añadimos el ClosingPanel como ingrediente
                if (other.GetComponent<panelScript>().getIsFor() || other.GetComponent<panelScript>().getIsIf())
                {
                    current_hamburguer.addInstruction(other.gameObject.transform.GetChild(2).gameObject);
                }

                //New
                if (other.GetComponent<panelScript>().getIsIfElse())
                {
                    //Añadimos como instruccion el else y el closing
                    current_hamburguer.addInstruction(other.gameObject.transform.GetChild(2).gameObject);
                    current_hamburguer.addInstruction(other.gameObject.transform.GetChild(4).gameObject);

                }
                other.gameObject.GetComponent<panelScript>().setIsInPanel(true);

                if (other.gameObject.GetComponent<panelScript>().getItWasInPanel())
                {

                    reOrderFunction(other);
                }

                //Volvemos a crear los paneles que se añaden al panel principal.
                //Sólo lo hacemos para los paneles que aún no han sido utilizados.
                else
                {
                    controladorPartidaWaiter.current.createPanel(other.name);
                }

                UndoManager.current.addCurrentObject(other.gameObject);


            }


            //En cualquier otro caso la instrucción estará entre otras dos ya existentes. 
            else
            {

                int i = 0;

                //Recorremos los ingredientes hasta que encontremos entre que dos paneles hemos puesto el nuevo panel
                while (!other.gameObject.GetComponent<panelScript>().getIsInPanel() || i == current_hamburguer.getInstructions().Count - 1)
                {

                    if (other.gameObject.transform.position.y < ((GameObject)current_hamburguer.getInstructions()[i]).transform.position.y && other.gameObject.transform.position.y > ((GameObject)current_hamburguer.getInstructions()[i + 1]).transform.position.y)
                    {
                        //Este debería ser parte del else, pero si no lo pongo, se buguea cuando intento colocar entre el closingPanel y otro panel
                        if (((GameObject)current_hamburguer.getInstructions()[i]).name == "ClosingPanel")
                        {

                            //Colocamos entre dos paneles básicos
                            other.gameObject.GetComponent<panelScript>().setIsInPanel(true);

                            //Colocamos la nueva instruccion entre los dos paneles
                            other.gameObject.transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, ((GameObject)current_hamburguer.getInstructions()[i]).transform.position.y - other.transform.localScale.x, this.transform.position.z);

                        }
                        else if (((GameObject)current_hamburguer.getInstructions()[i]).name == "ElsePanel")
                        {

                            //Colocamos entre el else y closing panel
                            GameObject ifElsePanel = ((GameObject)current_hamburguer.getInstructions()[i]).transform.parent.gameObject;

                            other.gameObject.GetComponent<panelScript>().setIsInPanel(true);
                            //other.gameObject.transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, ((GameObject)current_hamburguer.getInstructions()[i]).transform.position.y - conditionalSeparationAxisY, this.transform.position.z + conditionalSeparationAxisZ);
                            other.gameObject.transform.position = new Vector3(1, 1, 1);

                            other.gameObject.GetComponent<panelScript>().setIsInElse(true);
                            other.gameObject.GetComponent<panelScript>().setIsInIfElse(true);
                            other.gameObject.GetComponent<panelScript>().setIsInside(ifElsePanel);
                            ifElsePanel.GetComponent<panelScript>().setPanelsInsideElse(ifElsePanel.GetComponent<panelScript>().getPanelsInsideElse() + 1);
                            //Congelamos la posición del panel. Si el freeze no está dentro del if/else se caen los paneles. 
                        }

                        else if (((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().getIsInElse())
                        {
                            //Colocamos entre dos paneles de dentro del if
                            GameObject IfElsePanel = ((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().getIsInside();

                            other.gameObject.GetComponent<panelScript>().setIsInPanel(true);
                            other.gameObject.transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, ((GameObject)current_hamburguer.getInstructions()[i]).transform.position.y - other.transform.localScale.x, this.transform.position.z + conditionalSeparationAxisZ);
                            other.gameObject.GetComponent<panelScript>().setIsInElse(true);
                            other.gameObject.GetComponent<panelScript>().setIsInIfElse(true);

                            other.gameObject.GetComponent<panelScript>().setIsInside(IfElsePanel);

                            IfElsePanel.GetComponent<panelScript>().setPanelsInsideElse(IfElsePanel.GetComponent<panelScript>().getPanelsInsideElse() + 1);

                        }


                        else if (((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().getIsIfElse())
                        {
                            //Colocamos entre el If y closing panel
                            other.gameObject.GetComponent<panelScript>().setIsInPanel(true);
                            other.gameObject.transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, ((GameObject)current_hamburguer.getInstructions()[i]).transform.position.y - conditionalSeparationAxisY, this.transform.position.z + conditionalSeparationAxisZ);
                            other.gameObject.GetComponent<panelScript>().setIsInIf(true);

                            other.gameObject.GetComponent<panelScript>().setIsInIfElse(true);
                            other.gameObject.GetComponent<panelScript>().setIsInside((GameObject)current_hamburguer.getInstructions()[i]);
                            ((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().setPanelsInsideIf(((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().getPanelsInsideIf() + 1);
                            //Congelamos la posición del panel. Si el freeze no está dentro del if/else se caen los paneles. 
                        }

                        else if (((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().getIsInIf() && ((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().getIsInIfElse())
                        {
                            //Colocamos entre dos paneles de dentro del if
                            GameObject IfElsePanel = ((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().getIsInside();

                            other.gameObject.GetComponent<panelScript>().setIsInPanel(true);
                            other.gameObject.transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, ((GameObject)current_hamburguer.getInstructions()[i]).transform.position.y - other.transform.localScale.x, this.transform.position.z + conditionalSeparationAxisZ);
                            other.gameObject.GetComponent<panelScript>().setIsInIf(true);
                            other.gameObject.GetComponent<panelScript>().setIsInIfElse(true);

                            IfElsePanel.GetComponent<panelScript>().setPanelsInsideIf(IfElsePanel.GetComponent<panelScript>().getPanelsInsideIf() + 1);

                            other.gameObject.GetComponent<panelScript>().setIsInside(IfElsePanel);
                        }

                        else if (((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().getIsFor())
                        {

                            other.gameObject.GetComponent<panelScript>().setIsInPanel(true);
                            other.gameObject.transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, ((GameObject)current_hamburguer.getInstructions()[i]).transform.position.y - conditionalSeparationAxisY, this.transform.position.z + conditionalSeparationAxisZ);
                            other.gameObject.GetComponent<panelScript>().setIsInFor(true);
                            other.gameObject.GetComponent<panelScript>().setIsInside((GameObject)current_hamburguer.getInstructions()[i]);
                            ((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().setPanelsInside(((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().getPanelsInside() + 1);
                            //Congelamos la posición del panel. Si el freeze no está dentro del if/else se caen los paneles. 


                        }
                        else if (((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().getIsInFor())
                        {
                            //Colocamos entre dos paneles for
                            GameObject forTwoPanel = ((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().getIsInside();

                            other.gameObject.GetComponent<panelScript>().setIsInPanel(true);
                            other.gameObject.transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, ((GameObject)current_hamburguer.getInstructions()[i]).transform.position.y - other.transform.localScale.x, this.transform.position.z + conditionalSeparationAxisZ);
                            other.gameObject.GetComponent<panelScript>().setIsInFor(true);

                            forTwoPanel.GetComponent<panelScript>().setPanelsInside(forTwoPanel.GetComponent<panelScript>().getPanelsInside() + 1);

                            other.gameObject.GetComponent<panelScript>().setIsInside(forTwoPanel);

                        }

                        else if (((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().getIsIf())
                        {
                            //Colocamos entre el If y closing panel
                            other.gameObject.GetComponent<panelScript>().setIsInPanel(true);
                            other.gameObject.transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, ((GameObject)current_hamburguer.getInstructions()[i]).transform.position.y - conditionalSeparationAxisY, this.transform.position.z + conditionalSeparationAxisZ);
                            other.gameObject.GetComponent<panelScript>().setIsInIf(true);
                            other.gameObject.GetComponent<panelScript>().setIsInside((GameObject)current_hamburguer.getInstructions()[i]);
                            ((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().setPanelsInside(((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().getPanelsInside() + 1);
                            //Congelamos la posición del panel. Si el freeze no está dentro del if/else se caen los paneles. 
                        }

                        else if (((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().getIsInIf())
                        {
                            //Colocamos entre dos paneles de dentro del if
                            GameObject IfPanel = ((GameObject)current_hamburguer.getInstructions()[i]).GetComponent<panelScript>().getIsInside();

                            other.gameObject.GetComponent<panelScript>().setIsInPanel(true);
                            other.gameObject.transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, ((GameObject)current_hamburguer.getInstructions()[i]).transform.position.y - other.transform.localScale.x, this.transform.position.z + conditionalSeparationAxisZ);
                            other.gameObject.GetComponent<panelScript>().setIsInIf(true);

                            IfPanel.GetComponent<panelScript>().setPanelsInside(IfPanel.GetComponent<panelScript>().getPanelsInside() + 1);

                            other.gameObject.GetComponent<panelScript>().setIsInside(IfPanel);

                        }

                        //Aquí no entra cuando sacamos una instrucción de un panel especial y lo ponemos debajo del closing
                        else
                        {

                            //Colocamos entre dos paneles básicos
                            other.gameObject.GetComponent<panelScript>().setIsInPanel(true);

                            //Colocamos la nueva instruccion entre los dos paneles
                            other.gameObject.transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, ((GameObject)current_hamburguer.getInstructions()[i]).transform.position.y - other.transform.localScale.x, this.transform.position.z);

                        }

                        //Insertamos la instrucción a la lista justo delante de la que estamos obeservando en ese momento                       
                        current_hamburguer.insertInstruction(i + 1, other.gameObject);

                        other.gameObject.transform.rotation = Quaternion.Euler(180, 180, 90);
                        other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        //New
                        if (other.GetComponent<panelScript>().getIsFor() || other.GetComponent<panelScript>().getIsIf())
                        {
                            current_hamburguer.insertInstruction(i + 2, other.gameObject.transform.GetChild(2).gameObject);

                        }

                        //Si no ha estado en el panel creamos otro panel encima de la mesa cuando lo pongamos
                        if (!other.gameObject.GetComponent<panelScript>().getItWasInPanel())
                        {
                            controladorPartidaWaiter.current.createPanel(other.name);
                        }

                        //Cuando acabmos de colocar el panel entre otros dos paneles, recolocamos los paneles. Cambiar el método y sacar el for del metodo
                        reOrderFunction(other);

                    }
                    else
                    {
                        i++;
                    }
                }

            }



        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public ComandaWaiter GetHamburguer()
    {
        return this.current_hamburguer;
    }

    public void deleteHamburguer()
    {
        foreach (GameObject panel in current_hamburguer.getInstructions())
        {
            Destroy(panel);
        }
        current_hamburguer = new ComandaWaiter();
    }

    public void reOrderFunction(Collider other)
    {
        for (int x = 1; x <= current_hamburguer.getInstructions().Count - 1; x++)
        {
            //Sólo queremos modificar las posiciones de las instrucciones padre, los hijos se moveran con ellos. 
            if (((GameObject)current_hamburguer.getInstructions()[x]).name != "ClosingPanel" && ((GameObject)current_hamburguer.getInstructions()[x]).name != "ElsePanel")
            {
                //Con este reescalamos el else de un if-else
                if (((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getIsIfElse())
                {
                    reScaleDoubleFunction(other, x);

                    ((GameObject)current_hamburguer.getInstructions()[x]).transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, ((GameObject)current_hamburguer.getInstructions()[x - 1]).transform.position.y - conditionalSeparationAxisY, this.transform.position.z);

                    //Si recoloco una condición dentro del for o del if, la vuelvo a colocar dentro. No saco ninguna fuera ya que estamos ampliando la escala para añadir condiciones entro del bucle

                }

                //Si es un for, miramos si lo hemos de reescalar y seteamos la cantidad de paneles que estan dentro del for
                else if (((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getIsFor() || ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getIsIf())
                {
                    reScaleSingleFunction(other, x);
                    ((GameObject)current_hamburguer.getInstructions()[x]).transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, ((GameObject)current_hamburguer.getInstructions()[x - 1]).transform.position.y - conditionalSeparationAxisY, this.transform.position.z);

                }

                else if (((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getIsInFor() || ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getIsInIf() || ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getIsInElse())
                {

                    ((GameObject)current_hamburguer.getInstructions()[x]).transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, ((GameObject)current_hamburguer.getInstructions()[x - 1]).transform.position.y - conditionalSeparationAxisY, this.transform.position.z + conditionalSeparationAxisZ);

                }


                //Si no es ninguna condición, simplemente la coloca debajo
                else
                {

                    ((GameObject)current_hamburguer.getInstructions()[x]).transform.position = new Vector3(this.transform.position.x + other.transform.localScale.y, ((GameObject)current_hamburguer.getInstructions()[x - 1]).transform.position.y - conditionalSeparationAxisY, this.transform.position.z);

                }

            }


        }
    }

    public void reScaleDoubleFunction(Collider other, int x)
    {

        if ((((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInsideIf() != ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getReescaleSizeIf()) && ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInsideIf() > 0)
        {
            if (((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInsideIf() < ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getReescaleSizeIf())
            {

                int panelsLeft = ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getReescaleSizeIf() - ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInsideIf();
                float xScale = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).localScale.x;
                float yScale = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).localScale.y;
                float zNewScale = lateralPanelScale * ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInsideIf();

                //No entenido porque es -0.1f
                float xPosition = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position.x;
                float yNewPosition = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position.y + (0.1f * panelsLeft);
                float zPosition = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position.z;

                //Entiendo que es lo anterior * 2 pero sigo sin entender porque los valores son tan pequeños
                float xPosition2 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position.x;
                float yNewPosition2 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position.y + (0.2f * panelsLeft);
                float zPosition2 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position.z;

                //No entenido porque es -0.1f
                float xPosition3 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(3).position.x;
                float yNewPosition3 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(3).position.y + (0.2f * panelsLeft);
                float zPosition3 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(3).position.z;

                //Entiendo que es lo anterior * 2 pero sigo sin entender porque los valores son tan pequeños
                float xPosition4 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(4).position.x;
                float yNewPosition4 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(4).position.y + (0.2f * panelsLeft);
                float zPosition4 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(4).position.z;


                ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).localScale = new Vector3(xScale, yScale, zNewScale);
                ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position = new Vector3(xPosition, yNewPosition, zPosition);
                ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position = new Vector3(xPosition2, yNewPosition2, zPosition2);
                ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(3).position = new Vector3(xPosition3, yNewPosition3, zPosition3);
                ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(4).position = new Vector3(xPosition4, yNewPosition4, zPosition4);

                ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().setReescaleSizeIf(((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getReescaleSizeIf() - panelsLeft);



            }

            else if (((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInsideIf() > ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getReescaleSizeIf() && other.GetComponent<panelScript>().getIsInIfElse() && other.GetComponent<panelScript>().getIsInIf())
            {
                //La primera vez no queremos que entre
                if (((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInsideIf() > 1)
                {
                    float xScale = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).localScale.x;
                    float yScale = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).localScale.y;
                    float zNewScale = lateralPanelScale * ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInsideIf();

                    //No entenido porque es -0.1f
                    float xPosition = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position.x;
                    float yNewPosition = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position.y - 0.1f;
                    float zPosition = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position.z;

                    //Entiendo que es lo anterior * 2 pero sigo sin entender porque los valores son tan pequeños
                    float xPosition2 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position.x;
                    float yNewPosition2 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position.y - 0.2f;
                    float zPosition2 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position.z;

                    //No entenido porque es -0.1f
                    float xPosition3 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(3).position.x;
                    float yNewPosition3 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(3).position.y - 0.2f;
                    float zPosition3 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(3).position.z;

                    //Entiendo que es lo anterior * 2 pero sigo sin entender porque los valores son tan pequeños
                    float xPosition4 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(4).position.x;
                    float yNewPosition4 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(4).position.y - 0.2f;
                    float zPosition4 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(4).position.z;

                    ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).localScale = new Vector3(xScale, yScale, zNewScale);
                    ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position = new Vector3(xPosition, yNewPosition, zPosition);
                    ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position = new Vector3(xPosition2, yNewPosition2, zPosition2);
                    ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(3).position = new Vector3(xPosition3, yNewPosition3, zPosition3);
                    ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(4).position = new Vector3(xPosition4, yNewPosition4, zPosition4);

                    ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().setReescaleSizeIf(((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getReescaleSizeIf() + 1);
                }

            }

        }

        //Entramos cuando la cantidad de paneles sea mayor/menor al reescalado, exceptuando si el numero de paneles es < a 0.
        else if ((((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInsideElse() != ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getReescaleSizeElse()) && ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInsideElse() > 0)
        {

            GameObject ifElsePanel = (GameObject)current_hamburguer.getInstructions()[x];
            if (ifElsePanel.GetComponent<panelScript>().getPanelsInsideElse() < ifElsePanel.GetComponent<panelScript>().getReescaleSizeElse())
            {


                int panelsLeft = ifElsePanel.GetComponent<panelScript>().getReescaleSizeElse() - ifElsePanel.GetComponent<panelScript>().getPanelsInsideElse();
                float xScale = ifElsePanel.transform.GetChild(3).localScale.x;
                float yScale = ifElsePanel.transform.GetChild(3).localScale.y;
                float zNewScale = lateralPanelScale * ifElsePanel.GetComponent<panelScript>().getPanelsInsideElse();

                //No entenido porque es -0.1f
                float xPosition = ifElsePanel.transform.GetChild(3).position.x;
                float yNewPosition = ifElsePanel.transform.GetChild(3).position.y + (0.1f * panelsLeft);
                float zPosition = ifElsePanel.transform.GetChild(3).position.z;

                //Entiendo que es lo anterior * 2 pero sigo sin entender porque los valores son tan pequeños
                float xPosition2 = ifElsePanel.transform.GetChild(4).position.x;
                float yNewPosition2 = ifElsePanel.transform.GetChild(4).position.y + (0.2f * panelsLeft);
                float zPosition2 = ifElsePanel.transform.GetChild(4).position.z;

                ifElsePanel.transform.GetChild(3).localScale = new Vector3(xScale, yScale, zNewScale);
                ifElsePanel.transform.GetChild(3).position = new Vector3(xPosition, yNewPosition, zPosition);
                ifElsePanel.transform.GetChild(4).position = new Vector3(xPosition2, yNewPosition2, zPosition2);

                ifElsePanel.GetComponent<panelScript>().setReescaleSizeElse(((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getReescaleSizeElse() - panelsLeft);

            }


            else if (ifElsePanel.GetComponent<panelScript>().getPanelsInsideElse() > ifElsePanel.transform.gameObject.GetComponent<panelScript>().getReescaleSizeElse())
            {

                //La primera vez no queremos que entre
                if (ifElsePanel.GetComponent<panelScript>().getPanelsInsideElse() > 1)
                {

                    float xScale = ifElsePanel.transform.GetChild(3).localScale.x;
                    float yScale = ifElsePanel.transform.GetChild(3).localScale.y;
                    float zNewScale = lateralPanelScale * ifElsePanel.GetComponent<panelScript>().getPanelsInsideElse();

                    //No entenido porque es -0.1f
                    float xPosition = ifElsePanel.transform.GetChild(3).position.x;
                    float yNewPosition = ifElsePanel.transform.GetChild(3).position.y - 0.1f;
                    float zPosition = ifElsePanel.transform.GetChild(3).position.z;

                    //Entiendo que es lo anterior * 2 pero sigo sin entender porque los valores son tan pequeños
                    float xPosition2 = ifElsePanel.transform.GetChild(4).position.x;
                    float yNewPosition2 = ifElsePanel.transform.GetChild(4).position.y - 0.2f;
                    float zPosition2 = ifElsePanel.transform.GetChild(4).position.z;

                    ifElsePanel.transform.GetChild(3).localScale = new Vector3(xScale, yScale, zNewScale);
                    ifElsePanel.transform.GetChild(3).position = new Vector3(xPosition, yNewPosition, zPosition);
                    ifElsePanel.transform.GetChild(4).position = new Vector3(xPosition2, yNewPosition2, zPosition2);

                    ifElsePanel.GetComponent<panelScript>().setReescaleSizeElse(ifElsePanel.GetComponent<panelScript>().getReescaleSizeElse() + 1);
                }

            }

        }


    }

    public void reScaleSingleFunction(Collider other, int x)
    {

        if (((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInside() < ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getReescaleSize() && (((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInside() > 0))
        {

            int panelsLeft = ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getReescaleSize() - ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInside();
            float xScale = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).localScale.x;
            float yScale = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).localScale.y;
            float zNewScale = lateralPanelScale * ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInside();

            //No entenido porque es -0.1f
            float xPosition = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position.x;
            float yNewPosition = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position.y + (0.1f * panelsLeft);
            float zPosition = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position.z;

            //Entiendo que es lo anterior * 2 pero sigo sin entender porque los valores son tan pequeños
            float xPosition2 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position.x;
            float yNewPosition2 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position.y + (0.2f * panelsLeft);
            float zPosition2 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position.z;

            ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).localScale = new Vector3(xScale, yScale, zNewScale);
            ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position = new Vector3(xPosition, yNewPosition, zPosition);
            ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position = new Vector3(xPosition2, yNewPosition2, zPosition2);

            ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().setReescaleSize(((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getReescaleSize() - panelsLeft);



        }
        else if (((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInside() > ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getReescaleSize() && (other.GetComponent<panelScript>().getIsInFor() || other.GetComponent<panelScript>().getIsInIf()))
        {
            //La primera vez no queremos que entre
            if (((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInside() > 1)
            {
                float xScale = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).localScale.x;
                float yScale = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).localScale.y;
                float zNewScale = lateralPanelScale * ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getPanelsInside();

                //No entenido porque es -0.1f
                float xPosition = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position.x;
                float yNewPosition = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position.y - 0.1f;
                float zPosition = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position.z;

                //Entiendo que es lo anterior * 2 pero sigo sin entender porque los valores son tan pequeños
                float xPosition2 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position.x;
                float yNewPosition2 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position.y - 0.2f;
                float zPosition2 = ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position.z;

                ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).localScale = new Vector3(xScale, yScale, zNewScale);
                ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(1).position = new Vector3(xPosition, yNewPosition, zPosition);
                ((GameObject)current_hamburguer.getInstructions()[x]).transform.GetChild(2).position = new Vector3(xPosition2, yNewPosition2, zPosition2);

                ((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().setReescaleSize(((GameObject)current_hamburguer.getInstructions()[x]).GetComponent<panelScript>().getReescaleSize() + 1);
            }


        }
    }

}


