using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Game.GUI
{
    [RequireComponent(typeof(Canvas))]
    public class DungeonCanvas : MonoBehaviour
    {
        private class Message
        {
            RectTransform       m_transform;
            Vector3             m_vPosition;
            float               m_fTime = 0.0f;
            CanvasGroup         m_group;

            const float         MESSAGE_TIME = 4.0f;

            #region Properties

            public bool IsAlive => m_fTime < MESSAGE_TIME;

            public float Alpha => Mathf.Min(m_fTime * 2.5f, 1.0f - Mathf.Clamp01(m_fTime - (MESSAGE_TIME - 1.0f)));

            public Vector3 Position => m_vPosition + m_fTime * Vector3.up * 0.4f;

            #endregion

            public Message(GameObject template, string message, Vector3 vPosition, Color color)
            {
                GameObject go = Instantiate(template, sm_instance.transform);
                go.name = "Message";
                Text txt = go.GetComponent<Text>();
                txt.text = message;
                txt.color = color;
                m_group = go.GetComponent<CanvasGroup>();
                m_vPosition = vPosition;
                m_transform = go.GetComponent<RectTransform>();
                go.SetActive(true);
                Update();
            }

            public void Update()
            {
                m_fTime += Time.deltaTime;
                m_transform.anchoredPosition = sm_instance.GetCanvasPosition(Position);
                m_group.alpha = Alpha;
            }

            public void Kill()
            {
                Destroy(m_group.gameObject);
            }
        }

        private CanvasScaler    m_scaler;
        private List<Message>   m_messages = new List<Message>();
        private GameObject      m_messageTemplate;

        static DungeonCanvas    sm_instance;

        #region Properties

        public static DungeonCanvas Instance => sm_instance;

        #endregion

        private void OnEnable()
        {
            sm_instance = this;
            m_scaler = GetComponent<CanvasScaler>();
            m_messageTemplate = transform.Find("MessageTemplate").gameObject;
        }

        private void Start()
        {
            StartCoroutine(Clock());
        }

        public Vector2 GetCanvasPosition(Vector3 vWorldPosition)
        {
            if (DungeonCamera.Instance != null)
            {
                Vector3 vSP = DungeonCamera.Instance.Camera.WorldToViewportPoint(vWorldPosition);
                if (vSP.z > 0.0f)
                {
                    return new Vector2(vSP.x * m_scaler.referenceResolution.x, vSP.y * m_scaler.referenceResolution.y);
                }
            }

            return new Vector2(-1000.0f, -1000.0f);
        }

        public void AddMessage(string message, Vector3 vPosition, Color color)
        {
            m_messages.Add(new Message(m_messageTemplate, message, vPosition, color));
        }

        private void Update()
        {
            // update messages
            foreach (Message msg in m_messages)
            {
                msg.Update();

                if(!msg.IsAlive)
                {
                    msg.Kill();
                }
            }
            m_messages.RemoveAll(msg => !msg.IsAlive);
        }

        private IEnumerator Clock()
        {
            yield return new WaitForSeconds(1.0f);

            int iSeconds = 0;
            int iMinutes = 0;
            Text clock = transform.Find("Clock").GetComponent<Text>();
            while (HeroController.Instance != null &&
                   HeroController.Instance.IsAlive)
            {
                iSeconds++;
                if (iSeconds >= 60)
                {
                    iSeconds = 0;
                    iMinutes++;
                }

                clock.text = iMinutes.ToString("00") + ":" + iSeconds.ToString("00");
                yield return new WaitForSeconds(1.0f);
            }

            // game over!
            transform.Find("GameOver").gameObject.SetActive(true);
        }
    }
}