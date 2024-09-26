using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField]
        public AnimationCurve   m_fallCurve;

        void Start()
        {
            StartCoroutine(SpawnLogic("EnemyTemplate", 22.0f, 10.0f));
            StartCoroutine(SpawnLogic("HeartTemplate", 18.0f, 12.0f));
        }

        IEnumerator SpawnLogic(string templateName, float fSpawnTime, float fMinSpawnTime)
        {
            yield return new WaitForSeconds(1.0f);

            // get template
            GameObject template = transform.Find(templateName).gameObject;

            // start spawn loop
            while(HeroController.Instance != null) 
            {
                Dungeon.Node spawnNode = null;
                for (int i = 0; i < 20; ++i)
                {
                    Dungeon.Node node = Dungeon.Instance.GetRandomNode();
                    if (node != null && node.Owner == null)
                    {
                        spawnNode = node;
                        break;
                    }
                }

                if (spawnNode != null)
                {
                    // spawn enemy and claim node
                    GameObject go = Instantiate(template, Dungeon.Instance.transform);
                    go.name = template.name.Replace("Template", "");
                    go.transform.rotation = Quaternion.Euler(0.0f, Random.Range(-180.0f, 180), 0.0f);
                    go.SetActive(true);

                    // enemy?
                    EnemyController enemy = go.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.enabled = false;
                        enemy.Node = spawnNode;
                    }

                    // heart?
                    Heart heart = go.GetComponent<Heart>();
                    if (heart != null)
                    {
                        heart.enabled = false;
                    }

                    // fall in from sky
                    Vector3 vSource = spawnNode.Position + Vector3.up * 20.0f;
                    for (float f = 0.0f; f < m_fallCurve[m_fallCurve.length - 1].time; f += Time.deltaTime)
                    {
                        float fPrc = m_fallCurve.Evaluate(f);
                        go.transform.position = Vector3.Lerp(vSource, spawnNode.Position, fPrc);
                        yield return null;
                    }

                    // enable
                    yield return new WaitForSeconds(1.0f);
                    if (enemy != null) enemy.enabled = true;
                    if (heart != null) heart.enabled = true;
                }

                // reduce spawn time
                fSpawnTime = Mathf.Max(fSpawnTime - 2.0f, fMinSpawnTime);
                yield return new WaitForSeconds(fSpawnTime);
            }
        }
    }
}