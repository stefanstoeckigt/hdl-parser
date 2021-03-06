﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace Schematix.EntityDrawning
{
    [Serializable]
    public class My_Image : My_RectangleFigure
    {
        private Bitmap bitmap;

        public My_Image(EntityDrawningCore core, Bitmap bitmap)
            : base(core)
        {
            this.bitmap = bitmap;
            draw = Draw;
        }

        public My_Image(EntityDrawningCore core, Rectangle rect, Bitmap bitmap)
            : base(core, rect)
        {
            this.bitmap = bitmap;
            draw = Draw;
        }

        public My_Image(My_Image item)
            : base(item)
        {
            this.bitmap = item.bitmap;
            draw = Draw;
        }

        public override void AddToGraphicsPath(System.Drawing.Drawing2D.GraphicsPath path)
        {
            path.AddRectangle(rect);
        }

        private new void Draw(object sender, PaintEventArgs e)
        {
            Graphics dc = e.Graphics;
            if (selected == false)
            {
                dc.DrawImage(bitmap, rect);
                dc.DrawRectangle(pen, rect);
            }
            else
            {
                Brush brush2 = new SolidBrush(Color.FromArgb(128, EntityDrawningCore.SelectedColor));
                Pen pen2 = new Pen(Color.FromArgb(128, EntityDrawningCore.SelectedColor));

                dc.FillRectangle(brush2, rect);
                dc.DrawRectangle(pen2, rect);

                Brush br = new SolidBrush(Color.Black);
                foreach (Point p in Points)
                {
                    dc.FillRectangle(br, new Rectangle(p.X - 2, p.Y - 2, 4, 4));
                }
            }
        }

        public override bool IsSelected(Point pt)
        {
            if (base.IsSelected(pt) == true)
                return true;

            if ((pt.X >= rect.Left) && (pt.X <= rect.Right) && (pt.Y >= rect.Top) && (pt.Y <= rect.Bottom))
                return true;
            else
                return false;
        }
    }
}