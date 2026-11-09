using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem
{
#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
    public partial class DialogueTreeView : GraphView
#else
    public class DialogueTreeView : GraphView
#endif
    {
        DialogueTreeSO tree;
        private Vector2 newNodePosition;
        private string nodeViewPath;

#if !UNITY_6000_0_OR_NEWER
        public new class UxmlFactory : UxmlFactory<DialogueTreeView, GraphView.UxmlTraits> { }
#endif
        public DialogueTreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.actionKey && evt.keyCode == KeyCode.C)
            {
                CopySelection();
                evt.StopPropagation();
            }
            else if (evt.actionKey && evt.keyCode == KeyCode.V)
            {
                Paste();
                evt.StopPropagation();
            }
        }

        NodeView FindNodeView(NodeInternal node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        private void RemoveGraphElement(GraphElement visualElement)
        {
            RemoveElement(visualElement);
        }

        internal void PopulateView(DialogueTreeSO tree, string styleSheetPath)
        {
            if (tree == null)
            {
                return;
            }
            this.tree = tree;
            this.tree.RemoveGraphElement = (element) =>
            {
                RemoveGraphElement(element);
            };

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);
            styleSheets.Add(styleSheet);

            nodeViewPath = styleSheetPath.Split(new string[] { "DialogueTreeEditor" }, StringSplitOptions.None)[0];

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if (tree.startNode == null)
            {
                StartNodeInternal startNode = tree.CreateNode(typeof(StartNodeInternal)) as StartNodeInternal;
                tree.OnStartNodeChanged(startNode);
            }

            // Create node views
            tree.nodes.ForEach(node => CreateNodeView(node));

            // Create edges
            tree.nodes.ForEach(node =>
            {
                var children = tree.GetChildren(node);

                foreach (NodeInternal child in children)
                {
                    if (child != null)
                    {
                        NodeView parentView = FindNodeView(node);
                        NodeView childView = FindNodeView(child);

                        foreach (Port output in GetParentOutputs(parentView, child))
                        {
                            Edge edge = output.ConnectTo(GetChildInput(childView));
                            AddElement(edge);
                        }
                    }
                }
            });
            ScrollToStartNode();
        }

        private void ScrollToStartNode()
        {
            if (tree != null && tree.startNode != null)
            {
                UpdateViewTransform(tree.startNode.position * -1, Vector3.one);
            }
        }

        private List<Port> GetParentOutputs(NodeView parentView, NodeInternal child)
        {
            List<Port> outputs = new List<Port>();
            if (parentView is StartNodeView)
            {
                outputs.Add(((StartNodeView)parentView).output);
            }
            else if (parentView is DialogueNodeView)
            {
                outputs.Add(((DialogueNodeView)parentView).output);
            }
            else if (parentView is ChoiceNodeView)
            {
                ChoiceNodeView choiceNodeView = parentView as ChoiceNodeView;
                foreach (var pair in choiceNodeView.outputs)
                {
                    Port output = pair.Key;
                    ChoiceInternalView choicePortView = pair.Value;

                    if (choicePortView.choice.child == child)
                    {
                        outputs.Add(output);
                    }
                }
            }
            else if (parentView is IfNodeView)
            {
                IfNodeView ifNodeView = parentView as IfNodeView;
                if (ifNodeView.ifNode.elseChild == child)
                {
                    outputs.Add(ifNodeView.elseOutput);
                }
                foreach (var pair in ifNodeView.outputs)
                {
                    Port output = pair.Key;
                    IfPortView ifPortView = pair.Value;

                    if (ifPortView.ifPort.child == child)
                    {
                        outputs.Add(output);
                    }
                }
            }
            else if (parentView is EventNodeView)
            {
                outputs.Add(((EventNodeView)parentView).output);
            }
            return outputs;
        }

        private Port GetChildInput(NodeView childView)
        {
            if (childView is StartNodeView)
            {
                return null;
            }
            else if (childView is DialogueNodeView)
            {
                return ((DialogueNodeView)childView).input;
            }
            else if (childView is ChoiceNodeView)
            {
                return ((ChoiceNodeView)childView).input;
            }
            else if (childView is IfNodeView)
            {
                return ((IfNodeView)childView).input;
            }
            else if (childView is EventNodeView)
            {
                return ((EventNodeView)childView).input;
            }
            else if (childView is EndNodeView)
            {
                return ((EndNodeView)childView).input;
            }
            else
            {
                return null;
            }
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (tree == null)
            {
                return graphViewChange;
            }
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(element =>
                {
                    NodeView nodeView = element as NodeView;
                    if (nodeView != null)
                    {
                        if (nodeView is StartNodeView)
                        {
                            tree.OnStartNodeChanged(null);
                        }
                        if (nodeView is ChoiceNodeView)
                        {
                            ChoiceNodeView choiceNodeView = nodeView as ChoiceNodeView;
                            foreach (KeyValuePair<Port, ChoiceInternalView> keyValuePair in choiceNodeView.outputs)
                            {
                                Port output = keyValuePair.Key;
                                var edgeToDelete = output.connections.FirstOrDefault();
                                if (edgeToDelete != null)
                                {
                                    edgeToDelete.input.Disconnect(edgeToDelete);
                                    edgeToDelete.output.Disconnect(edgeToDelete);
                                    choiceNodeView.tree.RemoveGraphElement(edgeToDelete);
                                }
                            }
                        }
                        else if (nodeView is IfNodeView)
                        {
                            IfNodeView ifNodeView = nodeView as IfNodeView;
                            foreach (KeyValuePair<Port, IfPortView> keyValuePair in ifNodeView.outputs)
                            {
                                Port output = keyValuePair.Key;
                                var edgeToDelete = output.connections.FirstOrDefault();
                                if (edgeToDelete != null)
                                {
                                    edgeToDelete.input.Disconnect(edgeToDelete);
                                    edgeToDelete.output.Disconnect(edgeToDelete);
                                    ifNodeView.tree.RemoveGraphElement(edgeToDelete);
                                }
                            }
                            Port elseOutput = ifNodeView.elseOutput;
                            if (elseOutput != null)
                            {
                                var elseEdgeToDelete = elseOutput.connections.FirstOrDefault();
                                if (elseEdgeToDelete != null)
                                {
                                    elseEdgeToDelete.input.Disconnect(elseEdgeToDelete);
                                    elseEdgeToDelete.output.Disconnect(elseEdgeToDelete);
                                    ifNodeView.tree.RemoveGraphElement(elseEdgeToDelete);
                                }
                            }
                        }
                        tree.DeleteNode(nodeView.node);
                    }

                    Edge edge = element as Edge;
                    if (edge != null)
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;

                        int index = -1;
                        if (parentView is ChoiceNodeView)
                        {
                            ChoiceNodeView choiceNodeView = parentView as ChoiceNodeView;
                            index = choiceNodeView.outputs[edge.output].index;
                        }
                        else if (parentView is IfNodeView)
                        {
                            IfNodeView ifNodeView = parentView as IfNodeView;
                            if (ifNodeView.elseOutput != edge.output)
                            {
                                index = ifNodeView.outputs[edge.output].index;
                            }
                        }

                        tree.RemoveChild(parentView.node, index);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;

                    if (parentView is ChoiceNodeView)
                    {
                        ChoiceNodeView choiceNodeView = parentView as ChoiceNodeView;
                        ChoiceInternalView choicePortView = choiceNodeView.outputs[edge.output];
                        choicePortView.choice.OnChildChanged(childView.node);
                    }
                    else if (parentView is IfNodeView)
                    {
                        IfNodeView ifNodeView = parentView as IfNodeView;
                        if (edge.output == ifNodeView.elseOutput)
                        {
                            ifNodeView.ifNode.OnElseChildChanged(childView.node);
                        }
                        else
                        {
                            IfPortView ifPortView = ifNodeView.outputs[edge.output];
                            ifPortView.ifPort.child = childView.node;
                        }
                    }
                    else
                    {
                        tree.AddChild(parentView.node, childView.node);
                    }
                });
            }
            if (!EditorUtility.IsDirty(tree))
            {
                EditorUtility.SetDirty(tree);
            }

            return graphViewChange;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (tree == null)
            {
                Debug.LogWarning("There's no selected tree.");
                return;
            }

            newNodePosition = evt.mousePosition;
            var types = TypeCache.GetTypesDerivedFrom<NodeInternal>();
            foreach (var type in types)
            {
                if (!type.Equals(typeof(StartNodeInternal)))
                {
                    evt.menu.AppendAction(tree.GetNodeName(type), (action) => CreateNode(type));
                }
            }
        }

        void CreateNode(System.Type type)
        {
            if (tree == null)
            {
                Debug.LogWarning("There's no selected tree.");
                return;
            }
            NodeInternal node = tree.CreateNode(type);
            NodeView nodeView = CreateNodeView(node);
            if (nodeView != null)
            {
                nodeView.SetPosition(new Rect(contentViewContainer.WorldToLocal(newNodePosition), Vector2.zero));
            }
        }

        NodeView CreateNodeView(NodeInternal node)
        {
            NodeView nodeView;
            if (node is StartNodeInternal)
            {
                string uxml = nodeViewPath + "NodeViews/StartNode/StartNodeView.uxml";
                nodeView = new StartNodeView(node, uxml);
            }
            else if (node is DialogueNodeInternal)
            {
                string uxml = nodeViewPath + "NodeViews/DialogueNode/DialogueNodeView.uxml";
                nodeView = new DialogueNodeView(node, uxml);
            }
            else if (node is ChoiceNodeInternal)
            {
                string uxml = nodeViewPath + "NodeViews/ChoiceNode/ChoiceNodeView.uxml";
                nodeView = new ChoiceNodeView(node, uxml, tree);
            }
            else if (node is EventNodeInternal)
            {
                string uxml = nodeViewPath + "NodeViews/EventNode/EventNodeView.uxml";
                nodeView = new EventNodeView(node, uxml);
            }
            else if (node is IfNodeInternal)
            {
                string uxml = nodeViewPath + "NodeViews/IfNode/IfNodeView.uxml";
                nodeView = new IfNodeView(node, uxml, tree);
            }
            else if (node is EndNodeInternal)
            {
                string uxml = nodeViewPath + "NodeViews/EndNode/EndNodeView.uxml";
                nodeView = new EndNodeView(node, uxml);
            }
            else
            {
                return null;
            }
            AddElement(nodeView);
            return nodeView;
        }

        private void CopySelection()
        {
            var selectedNodeViews = selection
                .OfType<NodeView>()
                .Where(n => !(n is StartNodeView))
                .ToList();

            if (selectedNodeViews.Count == 0)
                return;

            DialogueClipboard clipboard = new();

            foreach (var view in selectedNodeViews)
            {
                clipboard.nodes.Add(view.node);
            }

            EditorGUIUtility.systemCopyBuffer = JsonUtility.ToJson(clipboard);
        }

        private void Paste()
        {
            if (string.IsNullOrEmpty(EditorGUIUtility.systemCopyBuffer))
                return;

            DialogueClipboard clipboard = JsonUtility.FromJson<DialogueClipboard>(EditorGUIUtility.systemCopyBuffer);

            if (clipboard == null || clipboard.nodes.Count == 0)
                return;

            foreach (var original in clipboard.nodes)
            {
                NodeInternal clone = null;
                if (original is DialogueNodeInternal)
                    clone = tree.CreateNode(typeof(DialogueNodeInternal));
                else if (original is ChoiceNodeInternal)
                    clone = tree.CreateNode(typeof(ChoiceNodeInternal));
                else if (original is IfNodeInternal)
                    clone = tree.CreateNode(typeof(IfNodeInternal));
                else if (original is EventNodeInternal)
                    clone = tree.CreateNode(typeof(EventNodeInternal));
                else if (original is EndNodeInternal)
                    clone = tree.CreateNode(typeof(EndNodeInternal));

                if (clone != null)
                {
                    Vector2 offset = new Vector2(30, 30);

                    clone.CopyFrom(original);
                    clone.guid = GUID.Generate().ToString();
                    clone.position = original.position + offset;

                    CreateNodeView(clone);
                }
            }

            EditorUtility.SetDirty(tree);
        }

        public void ClearView()
        {
            ClearSelection();
            DeleteElements(graphElements.ToList());
            tree = null;
        }
    }
}
