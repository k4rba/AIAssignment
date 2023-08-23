using AI;
using Game;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Joakim_Karbing
{
    public class Unit_Joakim_Karbing : Unit
    {
        #region Properties

        public new Team_Joakim_Karbing Team => base.Team as Team_Joakim_Karbing;

        #endregion

        protected override Unit SelectTarget(List<Unit> enemiesInRange)
        {
            return enemiesInRange[Random.Range(0, enemiesInRange.Count)];
        }

        protected override void Start()
        {
            base.Start();

            
            //iterate and add all team members into a list, then start by choosing a captain at random
            foreach (var teamMember in Team.m_joakimUnits) {
                Team.m_joakimUnits.Add(this);
            }
            // Team.m_teamCaptain = Team.m_joakimUnits[Random.Range(0, Team.m_joakimUnits.Count)];
            Team.m_teamCaptain = Team.m_joakimUnits[0];
            
            StartCoroutine(StupidLogic());
        }

        IEnumerator StupidLogic()
        {
            while (true)
            {
                // wait (or take cover)
                TargetNode = null;
                yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));

                // move randomly
                TargetNode = Battlefield.Instance.GetRandomNode();
                yield return new WaitForSeconds(Random.Range(4.0f, 6.0f));
            }
        }
    }
}