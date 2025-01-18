using BlackTree.Core;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackTree
{
    public class AbsorbLifeBullet :MonoBehaviour
    {
        [SerializeField] ParticleSystem particle;
        Vector3 startpos;
        Vector3 endpos;
        public void Shoot(Vector2 _startPos,Vector2 _endPos)
        {
            particle.gameObject.SetActive(Player.Cloud.optiondata.appearEffect);
            startpos = _startPos;
            endpos = _endPos;
            this.transform.position = startpos;

            StartCoroutine(Ishoot());
        }

        IEnumerator Ishoot()
        {
            float currentTime=0;
            while (true)
            {
                endpos = Model.Player.Unit.userUnit._view.transform.position;
                startpos = this.transform.position;

                Vector2 dir= (endpos - startpos).normalized;
                this.transform.Translate(dir * Time.deltaTime*17);

                if(Vector3.Distance(this.transform.position, Player.Unit.userUnit._view.transform.position)<2)
                {
                    break;
                }
                yield return null;
            }

            float skillValue=Model.Player.Skill.Get(Definition.SkillKey.AbsorbLife).SkillValue(0,1);

            Model.Player.Unit.IncreaseHpForAbsorbSkill(skillValue);

            this.gameObject.SetActive(false);
        }

    }

}
