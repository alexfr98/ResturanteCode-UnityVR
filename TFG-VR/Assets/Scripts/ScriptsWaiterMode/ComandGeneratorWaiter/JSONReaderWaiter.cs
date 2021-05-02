using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JSONReaderWaiter : MonoBehaviour
{

    public TextAsset jsonFileWaiter;
    private OrderJSONWaiter[] orderInJsonWaiter;
    private ArrayList orderJSONWaiter;

    void Start(){
        this.orderInJsonWaiter = JsonUtility.FromJson<OrderJSONWaiter[]>(jsonFileWaiter.text);
        var ordersToShow = orderInJsonWaiter.Where((value) => value.level == "1").ToList();
        this.orderJSONWaiter = new ArrayList(ordersToShow);

    }


    public ArrayList getOrderInJSON(){

        return this.orderJSONWaiter;
        

    }

    public void unlockLevel(int level){


        var newList = new ArrayList(this.orderInJsonWaiter.Where((value) => value.level == "" + level).ToList());
        this.orderJSONWaiter.AddRange(newList);
        
    }



    public OrderJSONWaiter randomOrder(){

        var random = new System.Random();
        var index = random.Next(this.orderJSONWaiter.Count);
        return this.orderJSONWaiter[index] as OrderJSONWaiter;

    }

    public OrderJSONWaiter orderTutorial(int i)
    {
        return this.orderJSONWaiter[i] as OrderJSONWaiter;

    }









}
