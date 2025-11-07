using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PADMA.Core.Native;

internal static class SwissEphemerisNative
{
    // Юлианская дата (грегорианский календарь)
    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern double swe_julday(
        int year,
        int month,
        int day,
        double hour,
        int gregflag);

    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int swe_utc_to_jd(
        int iyear, int imonth, int iday, int ihour, int imin, double dsec,
        int gregflag, double[] dret, StringBuilder serr);


    // Обратное преобразование юлианской даты в календарную
    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void swe_revjul(
        double jd,
        int gregflag,
        out int year,
        out int month,
        out int day,
        out double hour);

    // Расчёт положения планеты (UT)
    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int swe_calc_ut(
        double tjd_ut,
        int ipl,
        int iflag,
        [Out] double[] xx,
        StringBuilder serr);

    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int swe_houses_ex(
        double tjd_ut,
        int iflag,
        double geolat,
        double geolon,
        int hsys,
        [Out] double[] cusps,
        [Out] double[] ascmc);

    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int swe_lun_eclipse_when_glob(
        double tjd_start,          // JD(UT) — откуда искать
        int ifl,                   // флаги, например SEFLG_SWIEPH
        int ifltype,               // SE_ECL_ALLTYPES_LUNAR
        [Out] double[] tret,       // массив длиной >= 10
        int backward,              // 0 — вперёд, 1 — назад
        StringBuilder serr);       // строка для ошибок

    // --- Eclipse (global when) ---
    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int swe_lun_eclipse_when(
        double tjd_start,          // JD(UT) start
        int ifl,                   // SEFLG_SWIEPH etc.
        int ifltype,               // SE_ECL_ALLTYPES_LUNAR
        [Out] double[] tret,       // tret[0] = max (JD UT)
        int backward,              // 0 forward, 1 backward
        StringBuilder serr);
    
    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int swe_sol_eclipse_when_glob(
        double tjd_start,          // JD(UT) start
        int ifl,                   // SEFLG_SWIEPH etc.
        int ifltype,               // SE_ECL_ALLTYPES_SOLAR
        [Out] double[] tret,       // tret[0] = max (JD UT)
        int backward,              // 0 forward, 1 backward
        StringBuilder serr);

    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int swe_lun_occult_when_glob(
        double tjd_start,
        int ipl,                  // SE_SUN для солнечных затмений, SE_MOON для лунных
        IntPtr starname,          // null
        int ifl,
        int ifltype,
        [Out] double[] tret,
        int backward,
        StringBuilder serr);

    // ЛУННОЕ затмение: локальный поиск (требует geopos[3], tret[], attr[])
    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int swe_lun_eclipse_when_loc(
        double tjd_start,
        int ifl,
        double[] geopos,        // [lon, lat, alt]
        double[] tret,          // длина >= 10
        double[] attr,          // длина >= 20
        int backward,           // 0=вперёд, 1=назад
        StringBuilder serr);

    // СОЛНЕЧНОЕ затмение: локальный поиск (требует geopos[3], tret[], attr[])
    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int swe_sol_eclipse_when_loc(
        double tjd_start,
        int ifl,
        double[] geopos,        // [lon, lat, alt]
        double[] tret,          // длина >= 10
        double[] attr,          // длина >= 20
        int backward,           // 0=вперёд, 1=назад
        StringBuilder serr);

    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int swe_lun_eclipse_how(
        double tjd_ut,          // JD(UT) момента затмения (tret[0])
        int ifl,                // SEFLG_SWIEPH и т.п.
        double[] geopos,        // [lon, lat, alt] в градусах и метрах
        double[] attr,          // длина >= 20, attr[0] = величина затмения
        StringBuilder serr);    // буфер для ошибок (256 символов достаточно)

    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int swe_rise_trans(
        double tjd_ut,
        int ipl,
        string starname,
        int iflag,
        int rsmi,
        double[] geopos,
        double atpress,
        double attemp,
        double[] tret,
        StringBuilder serr);

    // Установка сидерического режима
    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void swe_set_sid_mode(
        int sid_mode,
        double t0,
        double ayan_t0);
    
    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void swe_set_topo(
        double geolon, 
        double geolat, 
        double altitude);


    // Получение текущей айанамсы
    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern double swe_get_ayanamsa_ut(double tjd_ut);

    // Инициализация пути к эфемеридам (необязательно, если dll рядом)
    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void swe_set_ephe_path(string path);

    // Очистка памяти библиотеки
    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void swe_close();
}
