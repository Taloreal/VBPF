﻿namespace KMI.Sim
{
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Xml;

    [Serializable]
    public class MapV2 : ActiveObject
    {
        public ArrayList nodes = new ArrayList();

        public void addNode(NodeV2 n, ArrayList connections)
        {
            this.nodes.Add(n);
            if (connections != null)
            {
                n.nodes.AddRange(connections);
                foreach (NodeV2 ev in connections)
                {
                    ev.nodes.Add(n);
                }
            }
        }

        public bool atDistributedNode(PointF currentLocation, NodeV2 node)
        {
            PointF[] cornerPoints = new PointF[4];
            int num = -1;
            int num2 = -1;
            PointF tf = Utilities.cartesianToIsometric((node.centerx * node.width) / 1f, (node.centery * node.height) / 1f, 0f, 0f, 1f, 1f);
            cornerPoints[0] = Utilities.cartesianToIsometric((float) num, (float) num2, node.x - tf.X, node.y - tf.Y, 1f, 1f);
            num = node.width + 1;
            num2 = -1;
            tf = Utilities.cartesianToIsometric((node.centerx * node.width) / 1f, (node.centery * node.height) / 1f, 0f, 0f, 1f, 1f);
            cornerPoints[1] = Utilities.cartesianToIsometric((float) num, (float) num2, node.x - tf.X, node.y - tf.Y, 1f, 1f);
            num = node.width + 1;
            num2 = node.height + 1;
            tf = Utilities.cartesianToIsometric((node.centerx * node.width) / 1f, (node.centery * node.height) / 1f, 0f, 0f, 1f, 1f);
            cornerPoints[2] = Utilities.cartesianToIsometric((float) num, (float) num2, node.x - tf.X, node.y - tf.Y, 1f, 1f);
            num = -1;
            num2 = node.height + 1;
            tf = Utilities.cartesianToIsometric((node.centerx * node.width) / 1f, (node.centery * node.height) / 1f, 0f, 0f, 1f, 1f);
            cornerPoints[3] = Utilities.cartesianToIsometric((float) num, (float) num2, node.x - tf.X, node.y - tf.Y, 1f, 1f);
            return Utilities.PolygonContains(currentLocation, cornerPoints);
        }

        public bool atDistributedNode(PointF currentLocation, string name)
        {
            return this.atDistributedNode(currentLocation, this.getNode(name));
        }

        public bool atNode(PointF currentLocation, NodeV2 node)
        {
            return ((currentLocation.X == node.x) && (currentLocation.Y == node.y));
        }

        public bool atNode(PointF currentLocation, string name)
        {
            return this.atNode(currentLocation, this.getNode(name));
        }

        public PathV2 findPath(NodeV2 start, NodeV2 end)
        {
            NodeV2 ev;
            foreach (NodeV2 ev3 in this.nodes)
            {
                ev3.distance = 0x7ffffffe;
                ev3.isDead = false;
                ev3.nextLink = null;
            }
            start.distance = 0;
            start.isDead = false;
            start.nextLink = null;
            for (int i = 0; i < this.nodes.Count; i++)
            {
                int num4 = 0;
                int distance = 0x7fffffff;
                int num6 = 0;
                while (num6 < this.nodes.Count)
                {
                    if (!(((NodeV2) this.nodes[num6]).isDead || (((NodeV2) this.nodes[num6]).distance >= distance)))
                    {
                        num4 = num6;
                        distance = ((NodeV2) this.nodes[num6]).distance;
                    }
                    num6++;
                }
                ev = (NodeV2) this.nodes[num4];
                for (num6 = 0; num6 < ev.nodes.Count; num6++)
                {
                    NodeV2 ev2 = (NodeV2) ev.nodes[num6];
                    float num7 = ev2.x - ev.x;
                    float num8 = ev2.y - ev.y;
                    float amount = (float) Math.Sqrt((double) ((num7 * num7) + ((num8 * num8) * 4f)));
                    amount += ev2.weight;
                    int num = (int) Utilities.Clamp(amount, 0f, 2.147484E+09f);
                    if (!((ev2.distance <= (ev.distance + num)) || ev.blocked))
                    {
                        ev2.distance = ev.distance + num;
                        ev2.nextLink = ev;
                    }
                }
                ev.isDead = true;
            }
            if ((end.distance < 0) || (end.distance >= 0x7ffffffe))
            {
                return null;
            }
            PathV2 hv = new PathV2();
            for (ev = end; ev != null; ev = ev.nextLink)
            {
                hv.nodes.Insert(0, ev);
            }
            return hv;
        }

        public PathV2 findPath(PointF currentLocation, string end)
        {
            PathV2 hv;
            ArrayList list = new ArrayList();
            do
            {
                float maxValue = float.MaxValue;
                NodeV2 start = null;
                foreach (NodeV2 ev2 in this.nodes)
                {
                    float num2 = Utilities.DistanceBetweenIsometric(currentLocation, (PointF) ev2.Location);
                    if (!((num2 >= maxValue) || list.Contains(ev2)))
                    {
                        start = ev2;
                        maxValue = num2;
                    }
                }
                hv = this.findPath(start, this.getNode(end));
                if (hv == null)
                {
                    list.Add(start);
                }
            }
            while ((hv == null) && (list.Count < this.nodes.Count));
            return hv;
        }

        public PathV2 findPath(string beg, string end)
        {
            return this.findPath(this.getNode(beg), this.getNode(end));
        }

        public NodeV2 getNode(string name)
        {
            foreach (NodeV2 ev in this.nodes)
            {
                if (ev.name.ToUpper() == name.ToUpper())
                {
                    return ev;
                }
            }
            throw new Exception("Could not find node '" + name + "' in place map.");
        }

        public bool load(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            return this.load(doc);
        }

        public bool load(XmlDocument doc)
        {
            try
            {
                XmlElement documentElement = doc.DocumentElement;
                string innerText = documentElement.SelectSingleNode("background").InnerText;
                XmlElement element2 = (XmlElement) documentElement.SelectSingleNode("network");
                XmlElement element3 = (XmlElement) element2.SelectSingleNode("nodes");
                foreach (XmlElement element4 in element3.ChildNodes)
                {
                    NodeV2 ev = new NodeV2 {
                        name = element4.SelectSingleNode("name").InnerText,
                        width = int.Parse(element4.SelectSingleNode("width").InnerText),
                        height = int.Parse(element4.SelectSingleNode("height").InnerText),
                        centerx = float.Parse(element4.SelectSingleNode("centerx").InnerText),
                        centery = float.Parse(element4.SelectSingleNode("centery").InnerText),
                        x = int.Parse(element4.SelectSingleNode("x").InnerText),
                        y = int.Parse(element4.SelectSingleNode("y").InnerText),
                        weight = int.Parse(element4.SelectSingleNode("weight").InnerText)
                    };
                    this.nodes.Add(ev);
                }
                XmlElement element5 = (XmlElement) element2.SelectSingleNode("links");
                foreach (XmlElement element6 in element5.ChildNodes)
                {
                    NodeV2 ev2 = (NodeV2) this.nodes[int.Parse(element6.SelectSingleNode("n1").InnerText)];
                    NodeV2 ev3 = (NodeV2) this.nodes[int.Parse(element6.SelectSingleNode("n2").InnerText)];
                    ev2.nodes.Add(ev3);
                    ev3.nodes.Add(ev2);
                }
            }
            catch (Exception exception)
            {
                frmExceptionHandler.Handle(exception);
                return false;
            }
            return true;
        }

        public bool load(Assembly assembly, string resource)
        {
            XmlDocument doc = new XmlDocument();
            Stream manifestResourceStream = assembly.GetManifestResourceStream(resource);
            doc.Load(manifestResourceStream);
            manifestResourceStream.Close();
            return this.load(doc);
        }
    }
}

