using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewServerFixScene : MonoBehaviour
{
    public BTButton closeBtn;


    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(() => { Application.Quit(); });
    }

   
}
