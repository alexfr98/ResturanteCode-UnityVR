using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComandaContWaiter:MonoBehaviour
{
    private ComandaJSONWaiter actual_order;
    private JSONReaderWaiter ordersJson;
  
    public ComandaContWaiter()
    {
            ordersJson = GameObject.Find("ControllerPartidaWaiter").GetComponent<JSONReaderWaiter>();
        
   }

    public void generateOrder()
    {
        actual_order = ordersJson.randomComanda();
    }

    public void unlocklevels(int nivell){
        ordersJson.unlockLevel(nivell); 
    }

    public ComandaJSONWaiter getActualOrder()
    {
        return this.actual_order;
    }

    public void setOrder(ComandaJSONWaiter comanda)
    {
        actual_order = comanda;
    }

    
}
