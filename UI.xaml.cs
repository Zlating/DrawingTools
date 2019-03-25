using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tekla.Structures;
using Tekla.Structures.Drawing;

using ui = Tekla.Structures.Drawing.UI;

namespace DrawingTools
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DrawingHandler dh = new DrawingHandler();
        Drawing draw
        {
            get { return dh.GetActiveDrawing(); }
        }

        DrawingObjectEnumerator SelectedObjects
        {
            get
            {
                return dh.GetDrawingObjectSelector().GetSelected();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // Инициализация смены формата
            chOk.FontSize = 16;
            vChangeFormat();
            bScaleSlider.Value = 5;

            dicScales.Add(new arkScales(1, 26));

            for(int i = 1; i < 20; i++)
            {
                dicScales.Add(new arkScales(i * 25 - 1, (i*25)+25+1));
            }


        }
        #region Смена формата 



        private void ChOk_Click(object sender, RoutedEventArgs e)
        {
#warning macro or api
            try
            {
                DrawingHandler dh = new DrawingHandler();
                if (dh.GetConnectionStatus() == false) return;
                Drawing draw = dh.GetActiveDrawing();
                draw.Layout.SheetSize.Height = Convert.ToDouble(ChangeLayot.now_fFormatReverse == true ? ChangeLayot.width : ChangeLayot.height);
                draw.Layout.SheetSize.Width = Convert.ToDouble(ChangeLayot.now_fFormatReverse == true ? ChangeLayot.height : ChangeLayot.width);
                draw.Modify();
                draw.CommitChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           
        }

        private void chREVERSE_Click(object sender, RoutedEventArgs e)
        {
            ChangeLayot.now_fFormatReverse = !ChangeLayot.now_fFormatReverse;
            ChangeLayot.fFormatReverse[ChangeLayot.now_fFormatNow] = ChangeLayot.now_fFormatReverse;
            vChangeFormat();
        }
        void chRIGHT_Click(object sender, RoutedEventArgs e)
        {
            ChangeLayot.ext_format(0);
            vChangeFormat();
        }

        void chLEFT_Click(object sender, RoutedEventArgs e)
        {
            ChangeLayot.ext_format(1);
            vChangeFormat();
        }
        void chDOWN_Click(object sender, RoutedEventArgs e)
        {
            ChangeLayot.ch_format(0);
            vChangeFormat();
        }

        void chUP_Click(object sender, RoutedEventArgs e)
        {
            ChangeLayot.ch_format(1);
            vChangeFormat();
        }


        private void vChangeFormat()
        {
            ChangeLayot.vChangeFormat();
            double temp = 0;
            string s = "";
            s = "A" + ChangeLayot.now_fFormatNow.ToString();
            if (ChangeLayot.now_fFormatExtend >= 2)
            {
                s += "x" + ChangeLayot.now_fFormatExtend;
                temp = ChangeLayot.width;
                ChangeLayot.width = ChangeLayot.height;
                ChangeLayot.height = temp;
                ChangeLayot.width *= ChangeLayot.now_fFormatExtend;
            }
            chOk.Content = (ChangeLayot.now_fFormatReverse == true ? "▐ " : "▬ ") + s + "\n" + ChangeLayot.height + "x" + ChangeLayot.width;
        }

        #endregion

        // Размеры
        #region Размеры
        private void ChangeDimensionType(int type)
        {
            DrawingObjectEnumerator does = SelectedObjects;
            while(does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(StraightDimensionSet))
                {
                    StraightDimensionSet sds = dobj as StraightDimensionSet;
                    switch(type)
                    {
                        case 1:
                        case 2:
                        case 3:
                            sds.Attributes.DimensionType =
                                (DimensionSetBaseAttributes.DimensionTypes)Enum.Parse(typeof(DimensionSetBaseAttributes.DimensionTypes), type.ToString());
                            break;
                        default:
                            break;
                    }
                    sds.Attributes.ShortDimension = DimensionSetBaseAttributes.ShortDimensionTypes.Inside;
                sds.Modify();
            }
            }
        }

        private void DimMIXED_Click(object sender, RoutedEventArgs e) => ChangeDimensionType(3);

        private void DimRelative_Click(object sender, RoutedEventArgs e) => ChangeDimensionType(1);

        private void DimABSOLUTE_Click(object sender, RoutedEventArgs e) => ChangeDimensionType(2);

        private void DimInsideDimension_Click(object sender, RoutedEventArgs e) => ChangeDimensionType(0);


        #endregion

        // Точность размера
        #region Accuracy
        private void DimAccuracy1_Click(object sender, RoutedEventArgs e) => ChangeAccuracy(1);

        void ChangeAccuracy(int type)
        {
            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(StraightDimensionSet))
                {
                    StraightDimensionSet sds = dobj as StraightDimensionSet;
                    sds.Attributes.Format.Precision =
                        (DimensionSetBaseAttributes.DimensionValuePrecisions)
                        Enum.Parse(typeof(DimensionSetBaseAttributes.DimensionValuePrecisions), type.ToString());


                    sds.Modify();
                }
            }
        }

        private void DimAccuracy05_Click(object sender, RoutedEventArgs e) => ChangeAccuracy(2);
        private void DimAccuracy01_Click(object sender, RoutedEventArgs e) => ChangeAccuracy(10);
        private void DimAccuracy001_Click(object sender, RoutedEventArgs e) => ChangeAccuracy(1000);



        #endregion

        // Угловой размер
        #region Угловой размер
        private void chTriangle(int type)
        {
            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(AngleDimension))
                {
                    AngleDimension sds = dobj as AngleDimension;

                    int value = 1000;
                    bool b = int.TryParse(baseAngle.Text, out value);
                    sds.Attributes.TriangleBase = b == true ? value : 1000;
                    sds.Attributes.Type = (AngleTypes)Enum.Parse(typeof(AngleTypes), type.ToString());

                    sds.Modify();
                }
            }
        }

        private void chTriangle_running()
        {
            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(AngleDimension))
                {
                    AngleDimension sds = dobj as AngleDimension;

                    int value = 1000;
                    bool b = int.TryParse(baseAngle.Text, out value);
                    sds.Attributes.TriangleBase = b == true ? value : 1000;
                    sds.Modify();
                }
            }
        }

        private void BaseAngle_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) => chTriangle_running();

        private void BTriangle_Click(object sender, RoutedEventArgs e) => chTriangle(2);
        private void BAnglePlace_Click(object sender, RoutedEventArgs e) => chTriangle(1);
        private void BAngle_Click(object sender, RoutedEventArgs e) => chTriangle(3);

        #endregion
        #region Префикс\Постфикс размеров
        private void TPrefix_KeyUp(object sender, System.Windows.Input.KeyEventArgs e) => chPrefixPostfix(0, tPrefix.Text);

        private void TPostfix_KeyUp(object sender, System.Windows.Input.KeyEventArgs e) => chPrefixPostfix(1, tPostfix.Text);

        private void chPrefixPostfix(int type, string text)
        {

            MessageBox.Show("Disconnected");
            return;

            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(StraightDimensionSet))
                {
                    StraightDimensionSet sds = dobj as StraightDimensionSet;
                    DrawingObjectEnumerator dimensions = sds.GetObjects();
                    while (dimensions.MoveNext())
                    {
                        try
                        {
                            StraightDimension sd = dimensions.Current as StraightDimension;

                            ContainerElement ce = null;

                            TextElement te = new TextElement(text);
                            te.Font = sds.Attributes.Text.Font;
                            if (type == 0)
                            {
                                ce = sd.Attributes.DimensionValuePrefix;
                                ce.Clear();
                                ce.Add(te);
                            }
                            else
                            {
                                ce = sd.Attributes.DimensionValuePostfix;
                                ce.Clear();
                                ce.Add(te);
                            }
                            double dist = sd.Distance;
                            var vector = new Tekla.Structures.Geometry3d.Vector(sd.UpDirection.X, sd.UpDirection.Y, sd.UpDirection.Z);
#warning Размер улетает после его модификации!
                            sd.Modify();
                          
                            sd.UpDirection = vector;
                            sd.Modify();
                        }
                        catch { }
                    }
                }
            }
        }

        private void BPrefix_Click(object sender, RoutedEventArgs e) => chPrefixPostfix(0, tPrefix.Text);
        private void BPostifx_Click(object sender, RoutedEventArgs e) => chPrefixPostfix(1, tPostfix.Text);

        #endregion

        #region Свойства вида

        private void BScale_Click(object sender, RoutedEventArgs e)
        {
            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(View))
                {
                    View v = dobj as View;
                    v.Attributes.Scale = Convert.ToDouble(bScaleSlider.Value);
                    v.Modify();
                }
            }

           
        }

        List<arkScales> dicScales = new List<arkScales>();

        public class arkScales
        {
            public double min = 0;
            public double max = 0;
            public arkScales(double min, double max)
            {
                this.min = min;
                this.max = max;
            }
        }

        private void BSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double val = e.NewValue;

            if (val != 1)
            {
                if (val == bScaleSlider.Minimum)
                {
                    for (int i = 0; i < dicScales.Count; i++)
                    {
                        //
                        if (dicScales[i].min == val)
                        {
                            bScaleSlider.Minimum = dicScales[i - 1].min;
                            bScaleSlider.Maximum = dicScales[i - 1].max;
                        }

                    }
                }
                if (val == bScaleSlider.Maximum)
                {
                    for (int i = dicScales.Count - 2; i >= 0; i--)
                    {
                        //
                        if (dicScales[i].max == val)
                        {
                            bScaleSlider.Minimum = dicScales[i + 1].min;
                            bScaleSlider.Maximum = dicScales[i + 1].max;
                        }

                    }
                }
            }
            bScale.Content = $"М 1:{(int)e.NewValue}";
        }

        private void BScaleSlider_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            bScaleSlider.Value += (e.Delta > 0) ? 1 : -1;

        }

        private void BTruncation_Click(object sender, RoutedEventArgs e) => Truncation(Convert.ToDouble(tTrunc.Text));


        private void BTruncation_Refresh_Click(object sender, RoutedEventArgs e) => Truncation(-1);


        private void Truncation(double value)
        {
            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(View))
                {
                    View v = dobj as View;
                    if (value != -1)
                    {
                        v.Attributes.Shortening.MinimumLength = value;
                    }
                    else
                    {
                        v.Attributes.Shortening.MinimumLength++;
                        v.Modify();
                        v.Attributes.Shortening.MinimumLength--;
                        v.Modify();
                    }
                    v.Modify();
                }
            }
        }

        private void BWS_Click(object sender, RoutedEventArgs e) => bWeld(1);

        private void BWE_Click(object sender, RoutedEventArgs e) => bWeld(2);

        private void BWN_Click(object sender, RoutedEventArgs e) => bWeld(0);


        private void bWeld(int type)
        {
            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(View))
                {

                    ui.DrawingObjectSelector dos = dh.GetDrawingObjectSelector();
                    dos.UnselectAllObjects();
                    dos.SelectObject(dobj);
                    Tekla.Structures.TeklaStructures.Connect();
                    MacroBuilder akit = new MacroBuilder();

                    akit.TreeSelect("view_dial", "gratCastUnitDrawingAttributesMenuTree", "Сварной шов");
                    akit.ValueChange("view_dial", "WeldVisibility", type.ToString());
                    akit.PushButton("view_modify", "view_dial");
                    akit.PushButton("view_cancel", "view_dial");
                    akit.Run();

                    dos.UnselectAllObjects();

                }
            }
        }


        #endregion

        #region Деталь

        private int HelperName(object sender)
        {
            return Convert.ToInt32(((Control)sender).Name.Replace("p", "").Replace("c",""));
        }

        private void PFalse_Click(object sender, RoutedEventArgs e)
        {
            int type = HelperName(sender);
            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(Part))
                {
                    Part part = dobj as Part;
                    part.Attributes.Representation = (Part.Representation)(Enum.Parse(
                        typeof(Part.Representation), type.ToString()));

                    part.Modify();
                }
            }
        }

        private void Pc1_Click(object sender, RoutedEventArgs e)
        {
            int type = HelperName(sender);
            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(Part))
                {
                    Part part = dobj as Part;
                    if (type == 1)
                    {
                        part.Attributes.DrawCenterLine = !part.Attributes.DrawCenterLine;
                    }
                    else if (type == 2)
                    {
                        part.Attributes.DrawReferenceLine = !part.Attributes.DrawReferenceLine;
                    }
                    else
                    {
                        part.Attributes.DrawCenterLine = false;
                        part.Attributes.DrawReferenceLine = false;
                    }
                    part.Modify();
                }
            }
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                MessageBox.Show(dobj.GetType().ToString());
            }
        }
    }
}
