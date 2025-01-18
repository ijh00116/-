using BlackTree.Bundles;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewinstallTrap : MonoBehaviour
{
    [SerializeField] protected LayerMask layer;
    float currentTime;

    Color skillColor=new Color(174f/255f, 231f / 255f, 143f / 255f);

    public void Shoot(Vector2 start)
    {
        transform.position = start;
       StartCoroutine( CoShoot());
    }

    private Vector2 distance;
    private IEnumerator CoShoot()
    {
        while (currentTime < 5)
        {
            currentTime += Time.deltaTime;
            transform.Translate(distance * 2 * Time.deltaTime, Space.World);
            yield return null;
        }
        SetOff();
    }

    private void SetOff()
    {
        currentTime = 0;
        this.gameObject.SetActive(false);
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var enemy = collision.GetComponent<ViewEnemy>();
        if ((layer.value & 1 << collision.gameObject.layer) != 1 << collision.gameObject.layer)
            return;

        if (enemy != null)
        {
            var enemycon = Battle.Enemy.GetHashEnemyController(enemy.hash);
            double dmg = Player.Unit.SwordAtk;
            enemycon.DecreaseHp(dmg,BlackTree.Definition.UserDmgType.Critical);

            WorldUIManager.Instance.InstatiateFont(enemy.transform.position, dmg, false, false, skillColor);
        }
    }
}