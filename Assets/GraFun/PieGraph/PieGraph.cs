using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieGraph : MonoBehaviour
{
    private float[] values;
    public Image[] wedges;
    public GameObject analizer;

    public void makeGraph()
    {
        Runner variables = analizer.GetComponent<Runner>();
        values = new float[4]{variables.killer, variables.achiever, variables.explorer, variables.socializer};
        //values = new float[4]{2, 10, 5, 7};
        float total = 0;
        float zRotation = 0;
        for(int i = 0; i < values.Length; i++)
        {
            total += values[i];
        }

        for(int i = 0; i < values.Length; i++)
        {
            Image newWedge = Instantiate (wedges[i]) as Image;
            newWedge.transform.SetParent(transform,false);
            newWedge.fillAmount = values[i] / total;
            newWedge.transform.rotation = Quaternion.Euler(new Vector3(0,0,zRotation));
            zRotation -= newWedge.fillAmount * 360;
        }
    }
}
