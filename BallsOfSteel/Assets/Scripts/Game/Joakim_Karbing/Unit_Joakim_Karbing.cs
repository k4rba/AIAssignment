using System;
using AI;
using Game;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Joakim_Karbing {
    public class Unit_Joakim_Karbing : Unit {
        #region Properties

        public new Team_Joakim_Karbing Team => base.Team as Team_Joakim_Karbing;
        public Brain m_brain;

        #endregion

        protected override Unit SelectTarget(List<Unit> enemiesInRange) {
            return enemiesInRange[Random.Range(0, enemiesInRange.Count)];
        }

        protected override void Start() {
            base.Start();

            m_brain = GetComponent<Brain>();
            m_brain.Tree.StartTree(m_brain);
            Team.m_joakimUnits.Add(this);

            StartCoroutine(TeamLogic());
        }

        void RemoveUnit(Unit_Joakim_Karbing unitToRemove) {
            Team.m_joakimUnits.Remove(unitToRemove);
        }
        
        IEnumerator TeamLogic() {
            while (true) {
                m_brain.Tree.Blackboard.SetValue("captain", Team.TeamCaptain);
                
                Unit_Joakim_Karbing value = m_brain.Tree.Blackboard.GetValue("captain", new Unit_Joakim_Karbing());
                
                if (value) {
                Debug.Log("curr leader: " + value);    
                }

                foreach (var unit in Team.m_joakimUnits) {
                    if (unit == null) {
                        Team.UnitToRemove.Add(unit);
                    }
                }

                foreach (var unit in Team.UnitToRemove) {
                    RemoveUnit(unit);
                }

                Team.UnitToRemove.Clear();

                TargetNode = null;
                yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));

                if (this == Team.TeamCaptain) {
                    TargetNode = Battlefield.Instance.GetRandomNode();
                    Team.m_teamCaptainTargetNode = TargetNode;
                    yield return new WaitForSeconds(Random.Range(4.0f, 6.0f));
                }
                else {
                    TargetNode = Team.m_teamCaptainTargetNode;
                    yield return new WaitForSeconds(Random.Range(4.0f, 6.0f));
                }
            }
        }
    }
}