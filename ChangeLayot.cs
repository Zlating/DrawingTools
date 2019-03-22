using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingTools
{
    public static class ChangeLayot
    {

        // массивы:
        public static int[] fFormat = new int[5]; //a0; a1; a2; a3; a4;
        public static bool[] fFormatReverse = new bool[5]; // false = горизонтальный, true = вертикальный
        public static int[] fFormatExtend = new int[5]; // 1..6
        // текущие настройки
        public static int now_fFormatNow = 3; // a3
        public static bool now_fFormatReverse = false; // =
        public static int now_fFormatExtend = 1; // a4x1
        public static double width = 0;
        public static double height = 0;

        // расширенние формата
        public static void ext_format(int k)
        {
            // k = параметр, понижения формата или повышения
            if (k == 0)
            {
                if (now_fFormatExtend != 6)
                {
                    // если формат не а0 и не = Nx1;
                    if ((now_fFormatNow != 0) && (fFormatExtend[now_fFormatNow] == 1))
                    {
                        now_fFormatExtend++;
                        fFormatExtend[now_fFormatNow]++;
                    }
                    now_fFormatExtend++;
                    fFormatExtend[now_fFormatNow]++;
                }
            }
            else if (k == 1)
            {
                if (now_fFormatExtend != 1)
                {
                    if ((now_fFormatExtend == 3) && (now_fFormatNow != 0))
                    {
                        now_fFormatExtend--;
                        fFormatExtend[now_fFormatNow]--;
                    }
                    now_fFormatExtend--;
                    fFormatExtend[now_fFormatNow]--;
                }
            }
            vChangeFormat();
        }

        public static void ch_format(int k)
        {
            if (k == 0)
            {
                if (now_fFormatNow != 4)
                {
                    now_fFormatNow++;
                    now_fFormatExtend = fFormatExtend[now_fFormatNow];
                }
            }
            else if (k == 1)
            {
                if (now_fFormatNow != 0)
                {
                    now_fFormatNow--;
                    now_fFormatExtend = fFormatExtend[now_fFormatNow];
                }
            }

            vChangeFormat();
        }

        public static void vChangeFormat()
        {

            #region Формат
            switch (ChangeLayot.now_fFormatNow)
            {
                case 4:
                    ChangeLayot.width = 297;
                    ChangeLayot.height = 210;
                    break;
                case 3:
                    ChangeLayot.width = 420;
                    ChangeLayot.height = 297;
                    break;
                case 2:
                    ChangeLayot.width = 594;
                    ChangeLayot.height = 420;
                    break;
                case 1:
                    ChangeLayot.width = 841;
                    ChangeLayot.height = 594;
                    break;
                case 0:
                    ChangeLayot.width = 1189;
                    ChangeLayot.height = 841;
                    break;
            }
            #endregion
            #region Расширенный формат
            
            #endregion
        }
    }
}
