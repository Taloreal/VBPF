namespace KMI.VBPF1Lib
{
    using KMI.Sim.Drawables;
    using System;
    using System.Drawing;

    [Serializable]
    public class RefrigeratorDrawable : Drawable
    {
        public RefrigeratorDrawable(Point location, string image) : base(location, image)
        {
            base.clickString = " ";
        }

        public override void Drawable_Click(object sender, EventArgs e)
        {
            if (!A.MF.Disabled(A.MF.mnuActionsCreditShopForFood))
            {
                A.MF.mnuActionsCreditShopForFood.PerformClick();
            }
        }
    }
}

