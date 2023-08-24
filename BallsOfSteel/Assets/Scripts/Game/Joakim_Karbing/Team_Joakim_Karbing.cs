using System;
using Game;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Joakim_Karbing
{
    public class Team_Joakim_Karbing : Team
    {
        [SerializeField]
        private Color   m_myFancyColor;

        public List<Unit_Joakim_Karbing> m_joakimUnits;
        
        public List<Unit_Joakim_Karbing> UnitToRemove = new List<Unit_Joakim_Karbing>();

        [SerializeField]
        private Unit_Joakim_Karbing m_teamCaptain;

        public Battlefield.Node m_teamCaptainTargetNode;
        
        private void ChooseNewCaptain() {
            if (m_joakimUnits[0] != null) {
                m_teamCaptain = m_joakimUnits[0];
            }
        }

        #region Properties
        
        public Unit_Joakim_Karbing TeamCaptain {
            get {
                ChooseNewCaptain();
                return m_teamCaptain;
            }
        }
        
        public override Color Color => m_myFancyColor;

        #endregion

        private void Awake() {
            Time.timeScale = 5;
        }
    }
}