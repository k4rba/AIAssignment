using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Nodes
{
    public class CompareBool_Joakim_Karbing : DecoratorNode
    {
        public string           m_key = "VariableName";
        public bool              m_iValue = false;
        static string[]         sm_opCodes = new string[] { " == ", " != "};

        #region Properties

        #endregion

        protected override State OnUpdate()
        {
            m_state = State.Failure;
            if (Tree != null && Tree.Blackboard != null)
            {
                bool iValue = Tree.Blackboard.GetValue(m_key, new bool());
                if (iValue) {
                    m_state = State.Success;
                }
                else {
                    m_state = State.Failure;
                }
            }

            if (m_state == State.Running)
            {
                m_state = m_child.Update();
            }

            return m_state;
        }
    }
}