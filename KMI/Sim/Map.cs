namespace KMI.Sim
{
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.Drawing;

    [Serializable]
    public class Map
    {
        public ArrayList places = new ArrayList();

        protected static float PathLength(ArrayList points)
        {
            float num = 0f;
            PointF empty = PointF.Empty;
            foreach (PointF tf2 in points)
            {
                if (!empty.IsEmpty)
                {
                    num += Utilities.DistanceBetweenIsometric(empty, tf2);
                }
                empty = tf2;
            }
            return num;
        }

        public ArrayList ShortestPath(Place begin, Place end, ref ArrayList speeds, ref float totalTime)
        {
            foreach (Place place in this.places)
            {
                place.tempDistance = float.MaxValue;
                place.tempIsDead = false;
                place.tempNextLink = null;
                if (place == end)
                {
                    place.tempDistance = 0f;
                }
            }
            Place place2 = null;
            foreach (Place place in this.places)
            {
                float maxValue = float.MaxValue;
                foreach (Place place3 in this.places)
                {
                    if (!(place3.tempIsDead || (place3.tempDistance >= maxValue)))
                    {
                        place2 = place3;
                        maxValue = place3.tempDistance;
                    }
                }
                foreach (Place place4 in place2.LinkedPlaces)
                {
                    float num2 = place2.tempDistance + (Utilities.DistanceBetweenIsometric(place2.Location, place4.Location) / Math.Min(place2.SpeedLimit, place4.SpeedLimit));
                    if (place4.UnderConstruction)
                    {
                        num2 = float.MaxValue;
                    }
                    if (place4.tempDistance > num2)
                    {
                        place4.tempDistance = num2;
                        place4.tempNextLink = place2;
                    }
                }
                place2.tempIsDead = true;
            }
            totalTime = begin.tempDistance;
            if (begin.tempDistance > 1.701412E+38f)
            {
                return null;
            }
            ArrayList list = new ArrayList();
            speeds = new ArrayList();
            for (place2 = begin; place2 != null; place2 = place2.tempNextLink)
            {
                list.Add(place2.Location);
                if (place2.tempNextLink != null)
                {
                    float num3 = Math.Min(place2.SpeedLimit, place2.tempNextLink.SpeedLimit);
                    speeds.Add(num3);
                }
            }
            return list;
        }
    }
}

