using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FeedbackGeneratorWaiter : MonoBehaviour
{
    private ComandaJSONWaiter comanda_actual;
    private GameObject textClient;
    private void Start()
    {
        textClient = GameObject.Find("TextClient");
    }
    public (string, float, bool) getFeedback(ComandaWaiter comanda, float temps)
    {


        ArrayList instructions = comanda.getInstructions();
        ArrayList instructions_screen = new ArrayList(comanda_actual.instructions);

        string result_str;

        bool result = true;
        int index = 0;
        int exp = 0;

        if (instructions.Count > instructions_screen.Count)
        {
            result_str = "You put too many instructions. Try again!";

            return (result_str, 0f, false);
                
        }
        else if (instructions.Count < instructions_screen.Count && instructions.Count > 0)
        {
            return ("You still have instructions to put. Don't give up!", 0f, false);
        }
        foreach (GameObject instruction in instructions)
        {

            if (result)
            {
                    
                result = instruction.gameObject.name.Equals(instructions_screen[index]);
                index++;
            }
            else
            {
                return ("The instructions are unordered. Try again" , 0, false);
            }
        }
        if (result && index > 0)
        {
            if (temps > 20f)
            {
                exp = 25;
            }
            else if (temps < 20f)
            {
                exp = 100;
            }

            return ("Succes! Let's go for another order!", exp, true);
        }
        result_str = "Incorrect! Try again";

        return (result_str, 0f, false);


    }



    public void setComandaActual(ComandaJSONWaiter comanda)
    {

        comanda_actual = comanda;
          
    }
}
