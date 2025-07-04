﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace poligon_inter.Model;

class BrokerIni
{
    //albo to zmienić na dictionary, albo dodać dictionary na inne pliki ini
    // a tu zostawić taki na tylko to 
    //na pewno dojedzie ini z kolorami ustawianymi przez urzytkownika 
    // i może drugi ze schematami bazy danych, co też będzie mógł modyfikować urzytkownik
    private readonly IniFile iniFile;



    static public IniFile LoadIni(string inis)
    {
        IniFile ini;
        if (File.Exists(Directory.GetCurrentDirectory() + "\\" + inis))
        {
            ini = new IniFile(Directory.GetCurrentDirectory() + "\\" + inis);
        }
        else
        {
            ini = new IniFile(Tools.GetUserAppDataPath + "\\" + inis);
        }
        return ini;
    }

    static public IniFile LoadIniProject() => LoadIni(Tools.GetProjectName + ".ini");
    private IniFile GetIni()
    {
        if (iniFile == null)
        {
            //to przerobić, powinno być już lokalnie a nie w tools
            // tools należy rozłozyć na części składowe i usunąć
            return LoadIniProject();
        }
        return iniFile;
    }

    public BrokerIni()
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
        get => GetIntValue(GetCurrentMethod()) == 0 ? 5 : GetIntValue(GetCurrentMethod());
        set => SetIntValue(GetCurrentMethod(), value);
    }


    public int WindowLeft 
    {
        get => GetIntValue(GetCurrentMethod()) == 0 ? 5 : GetIntValue(GetCurrentMethod());
        set => SetIntValue(GetCurrentMethod(), value);
    }



    public int WindowHeight
    {
        get => GetIntValue(GetCurrentMethod()) == 0 ?  450: GetIntValue(GetCurrentMethod());
        //get => GetIntValue(GetCurrentMethod());
        set => SetIntValue(GetCurrentMethod(), value);
    }

    public int WindowWidth
    {
        get => GetIntValue(GetCurrentMethod())==0? 800 : GetIntValue(GetCurrentMethod()) ;
        //get => GetIntValue(GetCurrentMethod());
        set => SetIntValue(GetCurrentMethod(), value);
    }
            //tu coś jest nie tak, coś wcześniej myślałem o tym i schszaniłęm ...{...}

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
            //return WindowState.Normal;
            
        }
        set 
        {
            iniFile.SetValue("General", GetCurrentMethod(), value.ToString());
        }
    }

    public int LastWidth
    {
        get => GetIntValue(GetCurrentMethod()) == 0 ? 800 : GetIntValue(GetCurrentMethod());
        //get => GetIntValue(GetCurrentMethod());
        set => SetIntValue(GetCurrentMethod(), value);
    }

    public int LastHeihgt
    {
        get => GetIntValue(GetCurrentMethod()) == 0 ? 450 : GetIntValue(GetCurrentMethod());
        //get => GetIntValue(GetCurrentMethod());
        set => SetIntValue(GetCurrentMethod(), value);
    }

    public int LastLeft
    {
        get => GetIntValue(GetCurrentMethod()) == 0 ? 5 : GetIntValue(GetCurrentMethod());
        //get => GetIntValue(GetCurrentMethod());
        set => SetIntValue(GetCurrentMethod(), value);
    }

    public int LastTop
    {
        get => GetIntValue(GetCurrentMethod()) == 0 ? 5 : GetIntValue(GetCurrentMethod());
        //get => GetIntValue(GetCurrentMethod());
        set => SetIntValue(GetCurrentMethod(), value);
    }
    #endregion Metody publiczne

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string GetCurrentMethod()
    {
        var st = new StackTrace();
        var sf = st.GetFrame(1);
        if (IsNotNull(sf))
        {
            var sx = sf.GetMethod();
            if (IsNotNull(sx)) return sx.Name.Substring(4);
        }
        return string.Empty;
    }
    private static bool IsNotNull([NotNullWhen(true)] object? obj) => obj != null;
    private int GetIntValue(string mert) => iniFile.GetIniValue("General", mert);
    private void SetIntValue(string met, int val) => iniFile.SetValue("General", met, val.ToString());
    private bool GetBoolValue(string mert) => iniFile.GetBoolValue("General", mert);
    private void SetBoolValue(string met, bool val) => iniFile.SetValue("General", met, val.ToString());
    private string GetStringValue(string mert) => iniFile.GetStringValue("General", mert);
    private void SetStringValue(string met, string val) => iniFile.SetValue("General", met, val);
    /*
    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
    */

}
