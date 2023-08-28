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

        #endregion

        protected override Unit SelectTarget(List<Unit> enemiesInRange) {
            return enemiesInRange[Random.Range(0, enemiesInRange.Count)];
        }

        protected override void Start() {
            base.Start();
            Team.m_joakimUnits.Add(this);
            StartCoroutine(TeamLogic());
        }

        void RemoveUnit(Unit_Joakim_Karbing unitToRemove) {
            Team.m_joakimUnits.Remove(unitToRemove);
        }

        protected override GraphUtils.Path GetPathToTarget() {
            return Team.GetMyShortestPath(CurrentNode, TargetNode);
        }

        IEnumerator TeamLogic() {
            while (true) {
                foreach (var unit in Team.m_joakimUnits) {
                    if (unit == null) {
                        Team.UnitToRemove.Add(unit);
                    }
                }

                foreach (var unit in Team.UnitToRemove) {
                    RemoveUnit(unit);
                }

                Team.UnitToRemove.Clear();

                
                //units follow the current captain around the battlefield to allow ganking, captain is the only unit moving independantly
                TargetNode = null;
                yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));

                if (this == Team.TeamCaptain) {
                    TargetNode = Battlefield.Instance.GetRandomNode();
                    Team.m_teamCaptainTargetNode = TargetNode;
                    yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));
                }
                else {
                    TargetNode = Team.m_teamCaptainTargetNode;
                    yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));
                }
            }
        }
    }
}