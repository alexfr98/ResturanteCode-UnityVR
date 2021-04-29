using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JSONReaderWaiter : MonoBehaviour
{

    public TextAsset jsonFileWaiter;
    private ConjuntComandesWaiter orderInJsonWaiter;
    private ArrayList orderJSONWaiter;

    void Start(){
        this.orderInJsonWaiter = JsonUtility.FromJson<ConjuntComandesWaiter>(jsonFileWaiter.text);
        var ordersToShow = orderInJsonWaiter.orders.Where((value) => value.level == "1").ToList();
        this.orderJSONWaiter = new ArrayList(ordersToShow);

    }


    public ArrayList getComandaInJSON(){

        return this.orderJSONWaiter;
        

    }

    public void unlockLevel(int level){


        var newList = new ArrayList(this.orderInJsonWaiter.orders.Where((value) => value.level == "" + level).ToList());
        this.orderJSONWaiter.AddRange(newList);
        
    }



    public ComandaJSONWaiter randomComanda(){

        var random = new System.Random();
        var index = random.Next(this.orderJSONWaiter.Count);
        return this.orderJSONWaiter[index] as ComandaJSONWaiter;

    }

    public ComandaJSONWaiter orderTutorial(int i)
    {
        return this.orderJSONWaiter[i] as ComandaJSONWaiter;

    }









}
