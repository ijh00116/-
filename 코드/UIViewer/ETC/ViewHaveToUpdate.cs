using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Bundles;

public class ViewHaveToUpdate : MonoBehaviour
{
    const string gameUrl = "https://play.google.com/";
    public BTButton goToGame;

    const string PlayStoreLink = "market://details?id=com.blacktree.idlehero";
    const string AppStoreLink = "itms-apps://itunes.apple.com/app/¾ÛID";

    const string OneStoreLink = "https://onesto.re/0000772322";
    // Start is called before the first frame update
    void Start()
    {
        goToGame.onClick.AddListener(() => { Application.OpenURL(PlayStoreLink); });
    }

}
