using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsGraph
{
    public class CsEdge<V, E>
    {
        public CsNode<V, E> targetNode { get; set; } = null;
        public E edgeValue { get; set; }

        public CsEdge(CsNode<V,E> node,E value)
        {
            targetNode = node;
            edgeValue = value;
        }
    }

    public class CsNode<V, E>
    {
        public V nodeValue { get; set; }
        public string name { get; set; }
        public List<CsEdge<V, E>> children { get; set; }
        public bool visited { get; set; }
        public int index { get; set; } = -1;
        public int lowlink { get; set; }
        public bool onStack { get; set; } = false;

        public CsNode(string nodeName)
        {
            name = nodeName;
            children = new List<CsEdge<V, E>>();
            visited = false;
        }

        public void AddChild(CsNode<V, E> childNode, E edgeVal)
        {
            children.Add(new CsEdge<V, E>(childNode, edgeVal));
        }

        public CsEdge<V, E> GetNextUnmarkedChild()
        {
            foreach (CsEdge<V, E> child in children)
            {
                if (!child.targetNode.visited)
                {
                    child.targetNode.visited = true;
                    return child;
                }
            }
            return null;
        }

        public bool HasUnmarkedChild()
        {
            foreach (CsEdge<V, E> child in children)
            {
                if (!child.targetNode.visited)
                {
                    return true;
                }
            }
            return false;
        }

        public void Unmark()
        {
            visited = false;
        }
        public override string ToString()
        {
            return name;
        }
    }

    public class Operation<V, E>
    {
        virtual public bool DoNodeOperation(CsNode<V, E> node)
        {
            Console.Write("\n  {0}", node.ToString());
            return true;
        }

        virtual public bool DoEdgeOperation(E edgeValue)
        {
            Console.Write(" {0}", edgeValue.ToString());
            return true;
        }
    }

    public class CsGraph<V, E>
    {
        public CsNode<V, E> startNode { get; set; }
        public string name { get; set; }
        public bool showBackTrack { get; set; } = false;

        public Dictionary<int, List<CsNode<V, E>>> strongComponent { get; set; } = new Dictionary<int, List<CsNode<V, E>>>();
        int index { get; set; } = 0;
        Stack<CsNode<V, E>> S { get; set; } = new Stack<CsNode<V, E>>();
        int strongComponentId { get; set; } = 0;

        public List<CsNode<V, E>> adjacencyList { get; set; }
        private Operation<V, E> graphOp = null;

        public CsGraph(string graphName)
        {
            name = graphName;
            adjacencyList = new List<CsNode<V, E>>();
            graphOp = new Operation<V, E>();
            startNode = null;
        }

        public Operation<V, E> SetOperation(Operation<V, E> newOp)
        {
            Operation<V, E> temp = graphOp;
            graphOp = newOp;
            return temp;
        }

        public void AddNode(CsNode<V, E> node)
        {
            adjacencyList.Add(node);
        }

        public int FindNodeByName(string name)
        {
            for (int i = 0; i < adjacencyList.Count; ++i)
            {
                if (adjacencyList[i].name == name)
                    return i;
            }
            return -1;
        }

        public void ClearMarks()
        {
            foreach (CsNode<V, E> node in adjacencyList)
                node.Unmark();
        }

        public void Walk()
        {
            if (adjacencyList.Count == 0)
            {
                Console.Write("\n No nodes in the graph");
                return;
            }

            if (startNode == null)
            {
                Console.Write("\n No starting node defined");
                return;
            }

            if (graphOp == null)
            {
                Console.Write("\n No node or edge operation defined");
                return;
            }

            this.Walk(startNode);
            foreach (CsNode<V, E> node in adjacencyList)
            {
                if (!node.visited)
                    Walk(node);

            }

            foreach (CsNode<V, E> node in adjacencyList)
            {
                node.Unmark();
            }
            return;
        }

        public void Walk(CsNode<V, E> node)
        {
            graphOp.DoNodeOperation(node);
            node.visited = true;

            do
            {
                CsEdge<V, E> childEdge = node.GetNextUnmarkedChild();
                if (childEdge == null)
                    return;
                else
                {
                    graphOp.DoEdgeOperation(childEdge.edgeValue);
                    Walk(childEdge.targetNode);
                    if(node.HasUnmarkedChild() || showBackTrack)
                    {
                        graphOp.DoNodeOperation(node);

                    }
                }
            } while (true);
        }

        public void StrongConnect(CsNode<V, E> node)
        {
            node.index = index;
            node.lowlink = index;
            ++index;
            S.Push(node);
            node.onStack = true;

            CsNode<V, E> child = null;

            foreach (CsEdge<V, E> edge in node.children)
            {
                child = edge.targetNode;

                if (child.index == -1)
                {
                    StrongConnect(child);
                    node.lowlink = Math.Min(node.lowlink, child.lowlink);
                }
                else if (child.onStack)
                {
                    node.lowlink = Math.Min(node.lowlink, child.lowlink);
                }
            }

            if (node.lowlink == node.index)
            {
                List<CsNode<V, E>> compNodes = new List<CsNode<V, E>>();
                strongComponent.Add(strongComponentId,compNodes);
                do
                {
                    child = S.Pop();
                    child.onStack = false;
                    strongComponent[strongComponentId].Add(child);
                } while (child != node);
                ++strongComponentId;
            }
        }

        public void StrongComponents()
        {
            strongComponentId = 0;
            index = 0;
            S.Clear();
            foreach (CsNode<V, E> node in adjacencyList)
            {
                if (node.index == -1)
                    StrongConnect(node);
            }
        }

        public void ShowDependencies()
        {
            Console.Write("\n Dependency Table:");
            Console.Write("\n ------------------");

            foreach (var node in adjacencyList)
            {
                Console.Write("\n {0}", node.name);
                for (int i = 0; i < node.children.Count; ++i)
                {
                    Console.Write("\n    {0}",node.children[i].targetNode.name);
                }
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////
    // Test class

    class demoOperation : Operation<string, string>
    {
        override public bool DoNodeOperation(CsNode<string, string> node)
        {
            Console.Write("\n -- {0}", node.name);
            return true;
        }
    }
    class Test
    {
        static void Main(string[] args)
        {
            Console.Write("\n  Testing CsGraph class");
            Console.Write("\n =======================");

            CsNode<string, string> node1 = new CsNode<string, string>("node1");
            CsNode<string, string> node2 = new CsNode<string, string>("node2");
            CsNode<string, string> node3 = new CsNode<string, string>("node3");
            CsNode<string, string> node4 = new CsNode<string, string>("node4");
            CsNode<string, string> node5 = new CsNode<string, string>("node5");

            node1.AddChild(node2, "edge12");
            node1.AddChild(node3, "edge13");
            node2.AddChild(node3, "edge23");
            node2.AddChild(node4, "edge24");
            node3.AddChild(node1, "edge31");
            node5.AddChild(node1, "edge51");
            node5.AddChild(node4, "edge54");

            CsGraph<string, string> graph = new CsGraph<string, string>("Fred");
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);
            graph.AddNode(node4);
            graph.AddNode(node5);

            graph.ShowDependencies();

            graph.startNode = node1;
            Console.Write("\n\n  starting walk at {0}", graph.startNode.name);
            Console.Write("\n  not showing backtracks");
            graph.Walk();

            graph.startNode = node2;
            Console.Write("\n\n  starting walk at {0}", graph.startNode.name);
            graph.showBackTrack = true;
            Console.Write("\n  show backtracks");
            graph.SetOperation(new demoOperation());
            graph.Walk();

            Console.Write("\n\n  Strong Components:");
            graph.StrongComponents();
            foreach (var item in graph.strongComponent)
            {
                Console.Write("\n  component {0}", item.Key);
                Console.Write("\n    ");
                foreach (var elem in item.Value)
                {
                    Console.Write("{0} ", elem.name);
                }
            }

            Console.Write("\n\n");
        }
    }
}
