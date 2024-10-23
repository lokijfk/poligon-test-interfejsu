using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace poligon_inter.Model
{
    class IniBroker
    {
        private IniFile? iniFile = null;

        private IniFile? GetIni()
        {
            if (iniFile == null)
            {
                return Tools.LoadIniProject();
            }
            return iniFile;
        }

        public IniBroker()
        { 
            iniFile = GetIni();
        }

        #region Metody publiczne
        
        public bool ExtenderIsExpanded
        {
            get => GetBoolValue(GetCurrentMethod());
            set => SetBoolValue(GetCurrentMethod(), value);
        }
        
        public int WindowTop 
        {
            get => GetIntValue(GetCurrentMethod());
            set => SetIntValue(GetCurrentMethod(), value);
        }


        public int WindowLeft 
        {
            get => GetIntValue(GetCurrentMethod());
            set => SetIntValue(GetCurrentMethod(), value);
        }



        public int WindowHeight
        {
            get => GetIntValue(GetCurrentMethod()) == 0 ? GetIntValue(GetCurrentMethod()) : 450;
            //get => GetIntValue(GetCurrentMethod());
            set => SetIntValue(GetCurrentMethod(), value);
        }

        public int WindowWidth
        {
            get => GetIntValue(GetCurrentMethod())==0? GetIntValue(GetCurrentMethod()): 800;
            //get => GetIntValue(GetCurrentMethod());
            set => SetIntValue(GetCurrentMethod(), value);
        }
                
        public WindowState CurMainWindowState { 
            get
            {
                switch (iniFile.GetValue(GetCurrentMethod()))
                {
                    case "Normal": { return WindowState.Normal;       }
                    case "Maximized": { return WindowState.Maximized; }
                    case "Minimized": { return WindowState.Minimized; }
                    default: { return WindowState.Normal; }
                }
                return WindowState.Normal;
                //return iniFile.GetValue(GetCurrentMethod()).ToEnum();
            }
            set 
            {
                if (value.ToString() == "Maximized")
                {
                    iniFile.SetValue("General", "LastWidth", iniFile.GetValue("WindowWidth"));
                    iniFile.SetValue("General", "LastHeihgt", iniFile.GetValue("WindowHeight"));
                }else if(iniFile.GetValue(GetCurrentMethod()) == "Maximized")
                {
                    iniFile.SetValue("General", "WindowWidth", iniFile.GetValue("LastWidth"));
                    iniFile.SetValue("General", "WindowHeight" , iniFile.GetValue("LastHeihgt"));
                }
                iniFile.SetValue("General", GetCurrentMethod(), value.ToString());
            }
        }

        #endregion Metody publiczne

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            return sf.GetMethod().Name.Substring(4);
        }
        private int GetIntValue(string mert) => iniFile.GetIniValue("General", mert);
        private void SetIntValue(string met, int val) => iniFile.SetValue("General", met, val.ToString());
        private bool GetBoolValue(string mert) => iniFile.GetBoolValue("General", mert);
        private void SetBoolValue(string met, bool val) => iniFile.SetValue("General", met, val.ToString());

        /*
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        */

    }
}
