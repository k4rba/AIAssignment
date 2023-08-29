using Game;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Joakim_Karbing {
    public class Team_Joakim_Karbing : Team {
        [SerializeField] private Color m_myFancyColor;

        public List<Unit_Joakim_Karbing> m_joakimUnits;

        public List<Unit_Joakim_Karbing> UnitToRemove = new List<Unit_Joakim_Karbing>();

        [SerializeField] private Unit_Joakim_Karbing m_teamCaptain;

        public Battlefield.Node m_teamCaptainTargetNode;

        public Dictionary<Battlefield.Node, float> coverLookup = new Dictionary<Battlefield.Node, float>();

        private static readonly Vector2Int[] sm_coverDirections = new Vector2Int[]
            { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };


        protected override void Start() {
            base.Start();
            StartCoroutine(UpdateCovers());
        }

        //unit 0 is always the captain for simplicity
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

        //keep covers relevant
        IEnumerator UpdateCovers() {
            while (true) {
                FindAllCovers(EnemyTeam.Units.ToList());
                yield return null;
            }
        }

        //logic to know where covers are (Im looking for where enemies can hide, to know if i should shoot them or choose another target in range without cover)
        public void FindAllCovers(List<Unit> opposingTeam) {
            foreach (var node in Battlefield.Instance.Nodes) {
                if (node is Battlefield.Node castedNode) {
                    float score = 0.0f;
                    if (opposingTeam.Any()) {
                        foreach (var unit in opposingTeam) {
                            Vector3 dirToUnit = Vector3.Normalize(unit.transform.position - castedNode.WorldPosition);
                            foreach (var dir in sm_coverDirections) {
                                if (Battlefield.Instance[castedNode.Position + dir] is Node_Rock rock) {
                                    Vector3 dirToCover =
                                        Vector3.Normalize(rock.WorldPosition - castedNode.WorldPosition);
                                    score += Vector3.Dot(dirToUnit, dirToCover);
                                }
                            }
                        }
                    }

                    if (score > 0) {
                        if (!coverLookup.ContainsKey(castedNode)) {
                            coverLookup.Add(castedNode, score / opposingTeam.Count);
                        }
                    }
                }
            }
        }

        //i shamelessly copy pasted this with the addition of + target.AdditionalCost on newDistance to avoid mud;
        public GraphUtils.Path GetMyShortestPath(Battlefield.Node start, Battlefield.Node goal) {
            if (start == null ||
                goal == null ||
                start == goal ||
                Battlefield.Instance == null) {
                return null;
            }

            foreach (Battlefield.Node node in Battlefield.Instance.Nodes) {
                node?.ResetPathfinding();
            }

            start.m_fDistance = 0.0f;
            start.m_fRemainingDistance = Battlefield.Instance.Heuristic(goal, start);
            List<Battlefield.Node> open = new List<Battlefield.Node>();
            HashSet<Battlefield.Node> closed = new HashSet<Battlefield.Node>();
            open.Add(start);

            while (open.Count > 0) {
                Battlefield.Node current = open[0];
                for (int i = 1; i < open.Count; ++i) {
                    if (open[i].m_fRemainingDistance < current.m_fRemainingDistance) {
                        current = open[i];
                    }
                }

                open.Remove(current);
                closed.Add(current);

                if (current == goal) {
                    GraphUtils.Path path = new GraphUtils.Path();
                    while (current != null) {
                        path.Add(current.m_parentLink);
                        current = current != null && current.m_parentLink != null ? current.m_parentLink.Source : null;
                    }

                    path.RemoveAll(l => l == null);
                    path.Reverse();
                    return path;
                }
                else {
                    foreach (Battlefield.Link link in current.Links) {
                        if (link.Target is Battlefield.Node target) {
                            if (!closed.Contains(target) &&
                                target.Unit == null) {
                                float newDistance = current.m_fDistance +
                                                    Vector3.Distance(current.WorldPosition, target.WorldPosition) +
                                                    target.AdditionalCost;
                                float newRemainingDistance =
                                    newDistance + Battlefield.Instance.Heuristic(target, start);

                                if (open.Contains(target)) {
                                    if (newRemainingDistance < target.m_fRemainingDistance) {
                                        target.m_fRemainingDistance = newRemainingDistance;
                                        target.m_fDistance = newDistance;
                                        target.m_parentLink = link;
                                    }
                                }
                                else {
                                    target.m_fRemainingDistance = newRemainingDistance;
                                    target.m_fDistance = newDistance;
                                    target.m_parentLink = link;
                                    open.Add(target);
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}