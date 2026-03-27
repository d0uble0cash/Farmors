using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class calculator_3 : MonoBehaviour{
    public TextMeshProUGUI displaytext;
    private string calcinput = "";
    private double result = 0.0;
    /* void Start(){}
     void Update(){}
     */
    public void onbuttonclick(string bval){
        if (bval == "="){
            calcresult();
        }
        else if (bval == "C"){
            clearinput();
        }
        else {
            calcinput += bval;
            updatedisplay();
        }
    }
    public void calcresult(){
        try{
            result = System.Convert.ToDouble(new System.Data.DataTable().Compute(calcinput, ""));
            calcinput = result.ToString();
            updatedisplay();
        }
        catch(System.Exception){
            calcinput = "ERROR";
            updatedisplay();
        }
    }

    public void clearinput(){
        calcinput = "";
        result = 0.0;
        updatedisplay();
    }

    public void updatedisplay(){
        displaytext.text = calcinput;
    }
}