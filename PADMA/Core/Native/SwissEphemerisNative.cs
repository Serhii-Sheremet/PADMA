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

    // Установка сидерического режима
    [DllImport(NativeLibrary.LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void swe_set_sid_mode(
        int sid_mode,
        double t0,
        double ayan_t0);

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
