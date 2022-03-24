using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DXFReaderNET;
using DXFReaderNET.Entities;
namespace ShowDimStyles
{
    public partial class Form1 : Form
    {
        internal enum FunctionsEnum
        {
            None,
            Dimension,
            DimensionLine,
            DimensionCircle,
            DimensionArc,
        }
        private FunctionsEnum CurrentFunction = FunctionsEnum.None;

        private Line SelectedLine = null;
        private Circle SelectedCircle = null;
        private Arc SelectedArc = null;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Parent = dxfReaderNETControl1;
            label1.BackColor = System.Drawing.Color.Transparent;
            label1.Text = "";
            newDrawing();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            newDrawing();
        }

        private void newDrawing()
        {
            dxfReaderNETControl1.NewDrawing();
            dxfReaderNETControl1.CursorSelectionSize = 8;
            int k;
            Random rnd = new Random();

            dxfReaderNETControl1.NewDrawing();
            dxfReaderNETControl1.SetLimits(new Vector2(-1, -1), new Vector2(-25, 125));
            dxfReaderNETControl1.ZoomLimits();
            int n = 3;
            for (k = 0; k < n; k++)
            {
                Vector3 pStart = new Vector3(rnd.NextDouble() * 100, rnd.NextDouble() * 100, 0);
                Vector3 pEnd = new Vector3(rnd.NextDouble() * 100, rnd.NextDouble() * 100, 0);
                dxfReaderNETControl1.AddLine(pStart, pEnd, (short)rnd.Next(1, 7));

            }

            for (k = 0; k < n; k++)
            {
                Vector3 c = new Vector3(rnd.NextDouble() * 100, rnd.NextDouble() * 100, 0);
                double r = rnd.NextDouble() * 20;
                dxfReaderNETControl1.AddCircle(c, r, (short)rnd.Next(1, 7));
            }

            for (k = 0; k < n; k++)
            {
                Vector3 c = new Vector3(rnd.NextDouble() * 100, rnd.NextDouble() * 100, 0);
                double r = rnd.NextDouble() * 25;
                dxfReaderNETControl1.AddArc(c, r, rnd.NextDouble() * 360, rnd.NextDouble() * 360, (short)rnd.Next(1, 7));
            }


            dxfReaderNETControl1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dxfReaderNETControl1.CustomCursor = CustomCursorType.CrossHairSquare;
            CurrentFunction = FunctionsEnum.Dimension;
            label1.Text = "Select entity with a mouse click";
        }

        private void SetPoint()

        {
            EntityObject entity = null;
            Vector2 p = dxfReaderNETControl1.CurrentWCSpoint;
            switch (CurrentFunction)
            {

                case FunctionsEnum.DimensionArc:

                    dxfReaderNETControl1.AddAngularDimension(SelectedArc, p.ToVector3(), dxfReaderNETControl1.DXF.DrawingVariables.DimStyle, dxfReaderNETControl1.DXF.CurrentColor.Index);
                    Refresh();
                    label1.Text = "";
                    CurrentFunction = FunctionsEnum.None;
                    break;
                case FunctionsEnum.DimensionCircle:

                    dxfReaderNETControl1.AddRadialDimension(SelectedCircle, p.ToVector3(), dxfReaderNETControl1.DXF.DrawingVariables.DimStyle, dxfReaderNETControl1.DXF.CurrentColor.Index);
                    Refresh();
                    label1.Text = "";
                    CurrentFunction = FunctionsEnum.None;
                    break;
                case FunctionsEnum.DimensionLine:


                    dxfReaderNETControl1.AddAlignedDimension(SelectedLine, p.ToVector3(), dxfReaderNETControl1.DXF.DrawingVariables.DimStyle, dxfReaderNETControl1.DXF.CurrentColor.Index);
                    Refresh();
                    label1.Text = "";
                    CurrentFunction = FunctionsEnum.None;
                    break;

                case FunctionsEnum.Dimension:


                    dxfReaderNETControl1.CustomCursor = CustomCursorType.CrossHair;

                    entity = dxfReaderNETControl1.GetEntity(p);
                    if (entity != null)
                    {
                        switch (entity.Type)
                        {
                            case EntityType.Line:
                                SelectedLine = (Line)entity;
                                CurrentFunction = FunctionsEnum.DimensionLine;
                                label1.Text = "Select dimension position";
                                break;
                            case EntityType.Circle:
                                SelectedCircle = (Circle)entity;
                                CurrentFunction = FunctionsEnum.DimensionCircle;
                                label1.Text = "Select dimension position";
                                break;
                            case EntityType.Arc:
                                SelectedArc = (Arc)entity;
                                CurrentFunction = FunctionsEnum.DimensionArc;
                                label1.Text = "Select dimension position";
                                break;
                        }

                    }
                    else
                    {
                        label1.Text = "No entity found";
                        dxfReaderNETControl1.CustomCursor = CustomCursorType.CrossHair;
                    }
                    break;
            }
        }

        private void dxfReaderNETControl1_MouseDown(object sender, MouseEventArgs e)
        {
            SetPoint();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dxfReaderNETControl1.ShowDimStyles();
        }

        private void dxfReaderNETControl1_MouseMove(object sender, MouseEventArgs e)
        {
            Vector2 p = dxfReaderNETControl1.CurrentWCSpoint;
            switch (CurrentFunction)
            {
                case FunctionsEnum.DimensionLine:
                    dxfReaderNETControl1.ShowRubberBandAlignedDimension(SelectedLine, p);
                    break;
                case FunctionsEnum.DimensionCircle:

                    dxfReaderNETControl1.ShowRubberBandLine(SelectedCircle.Center.ToVector2(), p);
                    break;


            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            button1.Left = this.Width - button1.Width - 28;
            button2.Left = button1.Left;
            button3.Left = button1.Left;
            label1.Top = this.Height - label1.Height;

        }
    }
}
