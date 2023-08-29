using Game;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Joakim_Karbing {
    public class Unit_Joakim_Karbing : Unit {
        #region Properties

        public new Team_Joakim_Karbing Team => base.Team as Team_Joakim_Karbing;

        #endregion

        private Dictionary<Battlefield.Node, float> firePowerLookup = new Dictionary<Battlefield.Node, float>();
        List<Battlefield.Node> ListOfNodes = new List<Battlefield.Node>();
        private Battlefield.Node BestNode;
        private const float COVER_VALUE = 0.75f;
        private const float SAFE_FIRE_POWER_VALUE = 0.2f;


        //always target enemy with least health, and preferebly one that isnt standing behind cover
        protected override Unit SelectTarget(List<Unit> enemiesInRange) {
            Unit enemyToTarget = null;
            float minHealth = float.MaxValue;

            foreach (var enemy in enemiesInRange) {
                if (enemyToTarget == null || enemy.Health < minHealth) {
                    enemyToTarget = enemy;
                    minHealth = enemy.Health;
                }

                //even if unit has low health, it might still not be the optimal one to shoot depending on if its behind cover. In this case, iterate again and maybe find something better.
                if (enemyToTarget.CurrentNode != null) {
                    if (Team.coverLookup.TryGetValue(enemyToTarget.CurrentNode, out float coverValue)) {
                        if (coverValue > COVER_VALUE) {
                            foreach (var alternativeEnemy in enemiesInRange) {
                                if (!alternativeEnemy.InCover) {
                                    enemyToTarget = alternativeEnemy;
                                }
                            }
                        }
                    }
                }
            }

            return enemyToTarget;
        }


        //logic for knowing where there is least firepower directed at my team. Only relevant for the teamcaptain as everyone else follows.
        public void FirePowerMapLookup() {
            if (this != Team.TeamCaptain) {
                return;
            }

            if (Team.EnemyTeam.Units.Any()) {
                foreach (var node in Battlefield.Instance.Nodes) {
                    if (node is Battlefield.Node castedNode) {
                        float score = 0;
                        foreach (var enemy in Team.EnemyTeam.Units) {
                            float distance = Vector3.Distance(enemy.transform.position, castedNode.WorldPosition);
                            if (distance < Unit.FIRE_RANGE) {
                                score += 1.0f;
                            }
                        }

                        if (score > 0) {
                            if (!firePowerLookup.ContainsKey(castedNode)) {
                                firePowerLookup.Add(castedNode, score / Team.EnemyTeam.Units.Count());
                            }
                            else {
                                firePowerLookup[castedNode] = score / Team.EnemyTeam.Units.Count();
                            }
                        }
                    }
                }
            }
        }


        //Find nodes that are safest to approach the enemy from, this is where the captain will lead the team to get an optimal opening
        private void OptimalNodeToMove() {
            ListOfNodes = new List<Battlefield.Node>();
            foreach (var node in firePowerLookup) {
                if (node.Value == SAFE_FIRE_POWER_VALUE) {
                    ListOfNodes.Add(node.Key);
                }
            }

            if (ListOfNodes.Any()) {
                BestNode =
                    ListOfNodes.OrderBy(x => Vector3.Distance(x.WorldPosition, Team.TeamCaptain.transform.position))
                        .ToList()[0];
            }
        }

        protected override void Start() {
            base.Start();
            Team.m_joakimUnits.Add(this);
            StartCoroutine(UpdateMaps());
            StartCoroutine(TeamLogic());
        }

        void RemoveUnit(Unit_Joakim_Karbing unitToRemove) {
            Team.m_joakimUnits.Remove(unitToRemove);
        }

        protected override GraphUtils.Path GetPathToTarget() {
            return Team.GetMyShortestPath(CurrentNode, TargetNode);
        }

        IEnumerator UpdateMaps() {
            while (true) {
                yield return null;
                FirePowerMapLookup();
                yield return null;
                if (this == Team.TeamCaptain) {
                    OptimalNodeToMove();
                }

                yield return null;
            }
        }

        Vector3 TargetPos() {
            Vector3 combinedPos = new();

            foreach (var enemy in Team.EnemyTeam.Units) {
                combinedPos += enemy.transform.position;
            }

            return combinedPos / Team.EnemyTeam.Units.Count();
        }

        bool CheckIfEnemyIsInRange(Battlefield.Node targetNode) =>
            Team.EnemyTeam.Units.Any(unit =>
                Vector3.Distance(unit.transform.position, targetNode.WorldPosition) < Unit.FIRE_RANGE);

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

                //units follow the current captain around the battlefield. Captain is technically the only unit moving independantly.
                TargetNode = null;
                yield return null;


                //My units start by going to an optimal advantage point where the least amount of units can shoot back, after that they move towards the rest of the enemies.
                if (this == Team.TeamCaptain) {
                    if (BestNode != null && CheckIfEnemyIsInRange(BestNode)) {
                        TargetNode = BestNode;
                    }
                    else {
                        TargetNode = GraphUtils.GetClosestNode<Battlefield.Node>(Battlefield.Instance, TargetPos());
                    }

                    Team.m_teamCaptainTargetNode = TargetNode;
                    yield return null;
                }
                else {
                    TargetNode = Team.m_teamCaptainTargetNode;
                    yield return null;
                }
            }
        }
    }
}