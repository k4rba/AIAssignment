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


        //always target enemy with least health, and preferebly one that isnt standing behind cover
        protected override Unit SelectTarget(List<Unit> enemiesInRange) {
            if (enemiesInRange != null && enemiesInRange.Count > 0) {
            }

            Unit enemyToTarget = enemiesInRange[0];
            float minHealth = enemyToTarget.Health;

            foreach (var enemy in enemiesInRange) {
                if (enemy.enabled) {
                    if (enemy.Health < minHealth) {
                        enemyToTarget = enemy;
                        minHealth = enemy.Health;
                    }
                }
                if (enemyToTarget.InCover) {
                    foreach (var alternativeEnemy in enemiesInRange) {
                        if (alternativeEnemy.enabled) {
                            if (!alternativeEnemy.InCover) {
                                enemyToTarget = alternativeEnemy;
                            }
                        }
                    }
                }
            }

            return enemyToTarget;
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
                //wonky method for shrinking the team list of units when unit is destroyed. Got (missing game object). I tried everything.
                //this should happen automatically when using lists (i think) but today it did not. :(
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