using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Camera))]
    public class DungeonCamera : MonoBehaviour
    {
        private Camera                  m_camera;
        private static DungeonCamera    sm_instance;

        #region Properties

        public Camera Camera => m_camera;

        public static DungeonCamera Instance => sm_instance;

        #endregion

        private void OnEnable()
        {
            m_camera = GetComponent<Camera>();
            sm_instance = this;
        }
    }
}