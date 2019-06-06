using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tekla.Structures;
using Tekla.Structures.Drawing;

using ui = Tekla.Structures.Drawing.UI;
using tsg = Tekla.Structures.Geometry3d;
using tsm = Tekla.Structures.Model;
using System.Collections;

namespace DrawingTools
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class UI : Window
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


        private void CommitChanges()
        {
            draw.CommitChanges();
        }

        public UI()
        {
            InitializeComponent();

            

            // Инициализация смены формата
            chOk.FontSize = 16;
            vChangeFormat();
            //bScaleSlider.Value = 5;

            dicScales.Add(new arkScales(1, 26));

            for(int i = 1; i < 20; i++)
            {
                dicScales.Add(new arkScales(i * 25 - 1, (i*25)+25+1));
            }

            List<string> styles = new List<string> { "light", "dark" };
            SetUpThemeApp(styles[0]);

            string env = SettingsEnvironment.GetEnvironmentModel.Trim().Replace(".","");
            
            if (env == "")
            {
                env = cenv.SelectedItem.ToString();
            }
            SettingsEnvironment.Init(env);
           foreach(var v in cenv.Items)
            {
                ComboBoxItem cbi = v as ComboBoxItem;
                if (cbi.Name == env)
                {
                    //cenv.SelectedItem = cbi;
                    break;
                }
            }

            DrawingHandler.SetMessageExecutionStatus(DrawingHandler.MessageExecutionModeEnum.BY_COMMIT);
            this.Title = "DrawingTools v" + Assembly.GetExecutingAssembly().GetName().Version;
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
            CommitChanges();
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
            CommitChanges();
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
            CommitChanges();
        }

        private void BaseAngle_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) => chTriangle_running();

        private void BTriangle_Click(object sender, RoutedEventArgs e) => chTriangle(2);
        private void BAnglePlace_Click(object sender, RoutedEventArgs e) => chTriangle(1);
        private void BAngle_Click(object sender, RoutedEventArgs e) => chTriangle(3);

        #endregion
        #region Префикс\Постфикс размеров
        private void TPrefix_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                chPrefixPostfix(0, tPrefix.Text);
            }
        }

        private void TPostfix_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                chPrefixPostfix(1, tPostfix.Text);
            }
        }

        private void chPrefixPostfix(int type, string text)
        {
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
                        chPrefixPostfix(dimensions.Current as StraightDimension, text, type);
                    }
                }
                else if (dobj.GetType() == typeof(StraightDimension))
                {
                    chPrefixPostfix(dobj as StraightDimension, text, type);
                }
            }
            CommitChanges();
        }


        private void chPrefixPostfix(StraightDimension sd, string text, int type)
        {

            ui.DrawingObjectSelector dos = dh.GetDrawingObjectSelector();
            dos.UnselectAllObjects();
            dos.SelectObject(sd);

            string[] lines = text.Split(new char[] { '/' });

            ContainerElement ce = new ContainerElement();

            TextElement te = new TextElement("");
            te.Font = sd.GetDimensionSet().Attributes.Text.Font;
            te.Font.Height = sd.GetDimensionSet().Attributes.Text.Font.Height - 1;

         
            DrawingObjectEnumerator related = sd.GetRelatedObjects(new Type[] { typeof(Bolt) });

            bool bolt = related.GetSize() > 0 ? true : false;


            TeklaStructures.Connect();
            MacroBuilder akit = new MacroBuilder();
            akit.Callback("acmd_display_selected_drawing_object_dialog", "", "View_10 window_1");
            akit.TabChange("dim_dial", "tabWndDimAttrib", "tabMarks");

            string itype = type == 0 ? "prefix" : "postfix";

            int count = (type == 0 ? 
                sd.Attributes.DimensionValuePrefix.Count : sd.Attributes.DimensionValuePostfix.Count);
            if (type == 0)
            {
                akit.PushButton($"butPrefixMark", "dim_dial");
            }
            else
            {
                akit.PushButton($"butPostfixMark", "dim_dial");
            }
            for (int i = 1; i <= 100; i++)
            {
                akit.TableSelect($"dim_{itype}_mark_dial", "gr_mark_selected_elements", new int[] { 1 });
                akit.PushButton("gr_remove_element", $"dim_{itype}_mark_dial");
            }
           

           
            for (int i = 0; i < lines.Length; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    akit.TableSelect($"dim_{itype}_mark_dial", "gr_mark_elements", new int[] { bolt == true ? 22 : 19 });
                    akit.Activate($"dim_{itype}_mark_dial", "gr_mark_elements");
                }
                akit.TableSelect($"dim_{itype}_mark_dial", "gr_mark_elements", new int[] { bolt == true ? 18 : 15 });
                akit.Activate($"dim_{itype}_mark_dial", "gr_mark_elements");
                akit.ValueChange("gr_mark_text", "gr_text", lines[i]);
                akit.PushButton("gr_mark_prompt_ok", "gr_mark_text");
                akit.TableSelect($"dim_{itype}_mark_dial", "gr_mark_elements", new int[] { bolt == true ? 21 : 18 });
                akit.Activate($"dim_{itype}_mark_dial", "gr_mark_elements");
            }
            for (int i = 1; i <= lines.Length * 6; i++)
            {
                akit.TableSelect($"dim_{itype}_mark_dial", "gr_mark_selected_elements", new int[] { i });
                akit.ValueChange($"dim_{itype}_mark_dial", "height", te.Font.Height.ToString().Replace(",","."));
            }

            akit.PushButton("butModify", $"dim_{itype}_mark_dial");
            akit.PushButton("butOk", $"dim_{itype}_mark_dial");
            akit.PushButton("dim_cancel", "dim_dial");
            akit.Run();
            CommitChanges();
        }

        private void BPrefix_Click(object sender, RoutedEventArgs e) => chPrefixPostfix(0, tPrefix.Text);
        private void BPostifx_Click(object sender, RoutedEventArgs e) => chPrefixPostfix(1, tPostfix.Text);

        #endregion

        #region Свойства вида

        private void BLevelLow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BLevelHigh_Click(object sender, RoutedEventArgs e)
        {
            changeLevelView(tLevelHigh.Text);
        }


        private void changeLevelView(string val)
        {
            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(View))
                {
                    try
                    {
                        View v = dobj as View;
                        ui.DrawingObjectSelector dos = dh.GetDrawingObjectSelector();
                        dos.UnselectAllObjects();
                        dos.SelectObject(dobj);
                        Tekla.Structures.TeklaStructures.Connect();
                        MacroBuilder akit = new MacroBuilder();

                        val += "/";
                        string[] value = val.Split(new char[] { '/' });

                        //MessageBox.Show(value.Length.ToString());

                        akit.Callback("acmd_display_selected_drawing_object_dialog", "", "View_10 window_1");
                        if (value[0].Trim() != "")
                        {
                            akit.ValueChange("view_dial", "gr_view_depth_pos", value[0]);
                        }
                        if (value[1].Trim() != "")
                        {
                            akit.ValueChange("view_dial", "gr_view_depth_neg", value[1]);
                        }

                        akit.PushButton("view_modify", "view_dial");
                        akit.PushButton("view_cancel", "view_dial");
                        akit.Run();

                    }
                    catch { }
                }
            }
            CommitChanges();
        }


        private void BScale_Click(object sender, RoutedEventArgs e)
        {
            ChangeScale();
        }

        void ChangeScale()
        {
            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(View))
                {
                    try
                    {
                        View v = dobj as View;
                        v.Attributes.Scale = Convert.ToDouble(bScaleText.Text); //Convert.ToDouble(bScaleSlider.Value);
                        v.Modify();
                    }
                    catch { }
                }
            }
            CommitChanges();
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

        private void BScaleText_TextChanged(object sender, TextChangedEventArgs e)
        {
            bScale.Content = $"М 1:{bScaleText.Text}";
        }

        private void BScaleText_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                ChangeScale();
            }
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
            CommitChanges();
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
                    akit.Callback("acmd_display_selected_drawing_object_dialog", "", "View_10 window_1");
                    akit.TreeSelect("view_dial", "gratCastUnitDrawingAttributesMenuTree", "Метка сварного шва");
                    akit.ValueChange("view_dial", "gr_vwel_create", type.ToString());
                    //akit.Callback("acmd_display_selected_drawing_object_dialog", "", "View_10 window_1");
                    //akit.TreeSelect("view_dial", "gratCastUnitDrawingAttributesMenuTree", "Сварной шов");
                    //akit.ValueChange("view_dial", "WeldVisibility", type.ToString());
                    akit.PushButton("view_modify", "view_dial");
                    akit.PushButton("view_cancel", "view_dial");
                    akit.Run();

                    //dos.UnselectAllObjects();

                }
            }
            CommitChanges();
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
            CommitChanges();
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
            CommitChanges();
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //Fusion.ho

            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                MessageBox.Show(dobj.GetType().ToString());
            }


            Dictionary<List<string>, List<string>> dic = new Dictionary<List<string>, List<string>>();
            // Стандарт качества
            foreach(KeyValuePair<List<string>, List<string>> keys in dic)
            {
                List<string> k = keys.Key;
                List<string> v = keys.Value;
            }
            // Var
            foreach(var k in dic)
            {
                var key = k.Key;
                var val = k.Value;
            }
        }

        private void ThemeChange(object sender, SelectionChangedEventArgs e)
        {

        }

        bool b = false;
        private void SetUpThemeApp(string s = "")
        {
            string style = s == "" ? (b == false ? "light" : "dark") : s;
            var uri = new Uri("Resources/" + style + ".xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            b = !b;
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            

        }

        private void Theme_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BSectionText_Click(object sender, RoutedEventArgs e)
        {
            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(SectionMark))
                {
                    SectionMark sm = dobj as SectionMark;

                    var p1 = sm.LeftPoint;
                    var p2 = sm.RightPoint;

                    Tekla.Structures.Geometry3d.Vector v = new Tekla.Structures.Geometry3d.Vector(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);

                    double vx = Math.Round(v.X, 0);
                    double vy = Math.Round(v.Y, 0);
                    //MessageBox.Show(v.ToString());

                    double y = SettingsEnvironment.MARK_SECTION[1];
                    double line_section = SettingsEnvironment.MARK_SECTION[2];
                    double x = SettingsEnvironment.MARK_SECTION[0];
                    double line_secion_x = SettingsEnvironment.MARK_SECTION[3];

                    string text = tSectionText.Text;
                    int count = text.Length;
                    if (text.Contains(" "))
                    {
                        count--;
                    }

                    if (vy == 0)
                    {
                        x = x + x / 3 * (count - 1);
                        line_secion_x = line_secion_x + (line_secion_x / 4) * (count - 1);
                    }
                    else
                    {
                        y = y + 0.5 * (count - 1);
                        line_section = line_section +((line_section / 4) * (count - 1)) + 0.25;
                       
                    }
                    sm.Attributes.LeftSymbol.Position.Y = y;
                    sm.Attributes.RightSymbol.Position.Y = y;
                    sm.Attributes.LineLengthOffset = line_section;
                    sm.Attributes.LeftSymbol.Position.X = x;
                    sm.Attributes.RightSymbol.Position.X = -1 * x;
                    sm.Attributes.LineWidthOffsetLeft = line_secion_x;
                    sm.Attributes.LineWidthOffsetRight = line_secion_x;
                    sm.Attributes.MarkName = text;
                    sm.Modify();
                }
            }
            CommitChanges();
        }

        private void Cenv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string env = (cenv.SelectedItem as ComboBoxItem).Name;
            SettingsEnvironment.Init(env);
        }

        private void HideObjects(object sender, RoutedEventArgs e)
        {
            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(Part))
                {
                    tsm.ModelObject mobj = new tsm.Model().SelectModelObject((dobj as Part).ModelIdentifier);
                    tsm.Part part = mobj as tsm.Part;

                    List<string> ids = new List<string>();

                    ids.Add(part.Identifier.GUID.ToString());
                    tsm.ModelObjectEnumerator childs = part.GetChildren();
                    while (childs.MoveNext())
                    {
                        if (childs.Current is tsm.RebarGroup)
                        {
                            ids.Add(childs.Current.Identifier.GUID.ToString());
                        }
                    }

                    ArrayList toHide = new ArrayList();
                    string result = "";

                    DrawingObjectEnumerator viewspart = dobj.GetView().GetAllObjects(typeof(Part));
                    while(viewspart.MoveNext())
                    {
                        if (!ids.Contains((viewspart.Current as Part).ModelIdentifier.GUID.ToString()))
                        {
                            toHide.Add(viewspart.Current as DrawingObject);
                            result += (viewspart.Current as Part).ModelIdentifier.ID + Environment.NewLine;
                        }
                    }
                    //MessageBox.Show(result);
                    ui.DrawingObjectSelector s = dh.GetDrawingObjectSelector();
                    s.UnselectAllObjects();
                    s.SelectObjects(toHide, true);
                    foreach(var v in toHide)
                    {
                       // s.SelectObject(v as DrawingObject);
                    }
                    
                }
            }
        }


        private void HideObjects_Old()
        {
            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(View))
                {
                    View v = dobj as View;
                    DrawingObjectEnumerator parts = v.GetAllObjects(typeof(Part));
                    tsg.AABB aABB = v.RestrictionBox;
                    //MessageBox.Show(aABB.MaxPoint.ToString());
                    int counter = 0;
                    //MessageBox.Show(parts.GetSize().ToString());
                    while (parts.MoveNext())
                    {
                        tsm.ModelObject mobj = new tsm.Model().SelectModelObject((parts.Current as Part).ModelIdentifier);
                        tsg.AABB ab = null;
                        if (mobj.GetType() == typeof(tsm.Beam))
                        {
                            tsm.Part part = mobj as tsm.Part;
                            ab = GetAABB(part.GetSolid());
                        }
                        else if (mobj.GetType() == typeof(tsm.Reinforcement))
                        {
                            tsm.RebarGroup rg = mobj as tsm.RebarGroup;
                            ab = GetAABB(rg.GetSolid());
                        }
                        else
                        {
                            MessageBox.Show(mobj.GetType().ToString());
                        }
                        if (ab != null)
                        {
                            //MessageBox.Show("asd");
                            //if (aABB.IsInside(new tsg.LineSegment(ab.MaxPoint, ab.MinPoint)))
                            //if (aABB.IsInside(ab.GetCenterPoint()))
                            //MessageBox.Show(ab.MinPoint.ToString());

                            string result1 = $"{aABB.MaxPoint}:{aABB.MinPoint}";
                            string result2 = $"{ab.MaxPoint}:{ab.MinPoint}";

                            MessageBox.Show(result1 + Environment.NewLine + result2);

                            if (aABB.IsInside(ab.MinPoint) && aABB.IsInside(ab.MaxPoint))
                            {
                                counter++;
                            }
                        }
                    }

                    MessageBox.Show(counter.ToString());

                }
            }
        }

        private tsg.AABB GetAABB(tsm.Solid s)
        {
            return new tsg.AABB(s.MinimumPoint, s.MaximumPoint);
        }

        private void BViewName_Click(object sender, RoutedEventArgs e)
        {
            DrawingObjectEnumerator does = SelectedObjects;
            while (does.MoveNext())
            {
                DrawingObject dobj = does.Current;
                if (dobj.GetType() == typeof(View))
                {
                    View v = dobj as View;
                    //v.Name = tViewName.tViewName;
                    v.Attributes.TagsAttributes.TagA1.TagContent.Clear();
                    v.Attributes.TagsAttributes.TagA1.TagContent.Add(
                        new TextElement(tViewName.Text, new FontAttributes(DrawingColors.Green, 5, "GOST 2.304 type A", false, false)));
                    v.Modify();
                }
            }
            CommitChanges();
        }


        /* Разработал
         * RUSSIA
         * USERDEFINED.ru_6_fam_dop USERDEFINED.ru_6_fam
         * 
         * Проверил
         * USERDEFINED.ru_7_fam_dop USERDEFINED.ru_7_fam
         * 
         */
    }
}
