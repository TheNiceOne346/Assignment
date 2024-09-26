using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GUI
{
    public class HPBar : MonoBehaviour
    {
        private Controller          m_controller;
        private RectTransform       m_rectTransform;
        private List<Image>         m_hearts;
        private int                 m_iHP = -1;

        private static GameObject   sm_prefab = null;

        #region Properties

        #endregion

        void Start()
        {
            m_rectTransform = GetComponent<RectTransform>();
            m_hearts = new List<Image>();

            // spawn a heart for each HP
            if (m_controller != null)
            {
                GameObject template = transform.Find("HeartTemplate").gameObject;
                for (int i = 0; i < m_controller.MaxHP; i++)
                {
                    GameObject go = Instantiate(template, template.transform.parent);
                    go.SetActive(true);
                    Image img = go.GetComponent<Image>();
                    m_hearts.Add(img);
                }
            }
        }

        void Update()
        {
            if (m_controller == null)
            {
                return;
            }

            // update hearts?
            if (m_iHP != m_controller.HP)
            {
                m_iHP = m_controller.HP;
                for (int i = 0; i < m_hearts.Count; ++i)
                {
                    m_hearts[i].color = i < m_controller.HP ? Color.white : Color.black;
                }
            }

            // place at controller position
            if (m_controller.IsAlive &&
                DungeonCanvas.Instance != null)
            {
                m_rectTransform.anchoredPosition = DungeonCanvas.Instance.GetCanvasPosition(m_controller.transform.position);
            }
            else
            {
                m_rectTransform.anchoredPosition = Vector2.one * -1000.0f;
            }
        }

        public static HPBar Create(Controller controller)
        {
            if (sm_prefab == null)
            {
                sm_prefab = Resources.Load<GameObject>("Prefabs/HPBar");
            }

            GameObject go = GameObject.Instantiate<GameObject>(sm_prefab, DungeonCanvas.Instance.transform);
            go.name = "HPBar_" + controller.name;
            HPBar hpBar = go.GetComponent<HPBar>();
            hpBar.m_controller = controller;
            
            return hpBar;
        }
    }
}