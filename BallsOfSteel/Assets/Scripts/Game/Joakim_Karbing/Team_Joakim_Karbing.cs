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

        public Unit_Joakim_Karbing m_teamCaptain;

        #region Properties

        public override Color Color => m_myFancyColor;

        #endregion
    }
}