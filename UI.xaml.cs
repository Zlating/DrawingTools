using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;



using Tekla.Structures.Drawing;

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

    }
}
