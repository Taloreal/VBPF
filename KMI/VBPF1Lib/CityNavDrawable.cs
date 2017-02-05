namespace KMI.VBPF1Lib
{
    using KMI.Sim.Drawables;
    using System;
    using System.Drawing;

    [Serializable]
    public class CityNavDrawable : AlphaDrawable
    {
        public CityNavDrawable(Point location, string imageName, string clickString) : base(location, imageName, clickString, 70)
        {
        }

        public override void Drawable_Click(object sender, EventArgs e)
        {
            A.MF.OnViewChange(A.I.Views[0].Name);
        }
    }
}

