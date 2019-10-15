using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Raspisanie2019
{
    class Db
    {
        public static string login{ get; set;}
        public static string password{ get; set; }
        public static SqlConnection myConnection { get; set; }
        public static string ipServ { get; } = "10.0.0.103";

        public static int id_role;
        public static int kod_ft;
        public static int id_program;

        //public static List<int> Permits;
        public static Dictionary<int, string> Permits = new Dictionary<int, string>(0);
        public static Dictionary<int, string> Faks = new Dictionary<int, string>(0);
        //public static string ConString { get; set; } = "Server = " + ipServ + "; Database = " + "DONNTU" + ";User Id=" + login + ";Password=" + password + ";";
        public static bool PingServ()
        {
            return new Ping().Send(ipServ, 50).Status == IPStatus.Success;
        }
        public static bool OpenCon()
        {
            myConnection = new SqlConnection
            {
                ConnectionString = "Server = " + ipServ + "; Database = " + "DONNTU" + ";User Id=" + login + ";Password=" + password + ";" + " MultipleActiveResultSets = True "
            };
            try
            {
                myConnection.Open();
                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }
        public class SQLCommands
        {
            #region
            //public static string CheckRole = @"select TOP(1) r.id_role from [T_ROLES] r " +
            //    @"inner join " +
            //    @"(select [id_role] from [T_MANAGE_ROLES] where " +
            //    @"[id_login] = (select [id_login] from[T_LOGIN] where [login] = @login) " +
            //    @"and " +
            //    @"[id_program] = (select[id_program] from [T_PROGRAM] where [exe_name] = @nameOfExe)) " +
            //    @"as a on r.id_role = a.id_role " +
            //    @"order by r.role_priority DESC";
            #endregion

            /// <summary>
            /// Запрос на получение кода роли, кода факультета и кода программы
            /// </summary>
            public static string CheckRole =
                @"select TOP(1) mr.id_role, mr.kod_ft, p.id_program from t_login l " +
                @"inner join T_MANAGE_ROLES as mr on mr.id_login = l.id_login " +
                @"inner join T_ROLES_PROGRAMS as rp on rp.id_role = mr.id_role " +
                @"inner join T_PROGRAM as p on p.id_program = rp.id_program " +
                @"inner join T_ROLES as r on r.id_role = mr.id_role " +
                @"where(l.[login]  = @login and p.exe_name = @nameOfExe) " +
                @"order by r.role_priority, mr.id ";

            /// <summary>
            /// Запрос на получение прав
            /// </summary>
            public static string GetPermits =
                @"select p.id_permit, p.permit_name from T_PERMIT p " +
                @"inner join T_ROLE_PERMIT as rp on rp.id_permit = p.id_permit " +
                @"where(p.id_program = @id_program and rp.id_role = @id_role) " +
                @"order by p.id_permit ";

            public static string GetFaks =
                @"select k_ft, fak_name from [T_FAC_DB] " +
                @"where actual = 1 " +
                @"order by k_ft ";

            /// <summary>
            /// Запрос на получение всех видов подготовки
            /// </summary>
            public static string GetVidPodgot =
                @"select distinct svp.k_vid_podg, svp.[name] from Gruppa g " +
                @"inner join [sp_vid_podg] as svp on g.VID_PODG = svp.k_vid_podg " +
                @"where g.actual = 1 and svp.ACTUAL = 1 ";
            /// <summary>
            /// Запрос на получение специальностей
            /// </summary>
            public static string GetSpec =
                @"select cod_sp, [name], snam from Special " +
                @"where aktual = 1 ";

            /// <summary>
            /// запрос на получение годов набора
            /// </summary>
            public static string GetYears =
                @"select distinct GOD_NABORA from Gruppa " +
                @"where actual = 1 and GOD_NABORA <> 15 " +
                @"order by GOD_NABORA ";

            /// <summary>
            /// запрос на получение всех групп
            /// </summary>
            public static string GetGrupps =
                @"select KOD_GRUP, NAME_GRUP from gruppa " +
                @"where actual = 1 ";

            /// <summary>
            /// запрос на получение NPPG
            /// </summary>
            public static string GetNPPG =
                @"select * from Archiv.dbo.uch_gr " +
                @"where k_fak = @k_fak " +
                @"and god_nabora = @god_nabora " +
                @"and aktual = 1 " +
                @"and vid_podg = @vid_podg " +
                @"and spec = @spec ";

            /// <summary>
            /// запрос на получение дисциплин
            /// </summary>
            public static string GetDiscips =
                @"SELECT up.k_discip, up.lekcii, up.prakt, up.laborat, up.k_proekt, up.k_rab, d.name_rus " +
                @"FROM [ARCHIV].[dbo].[uch_plan] up " +
                @"inner join DONNTU.dbo.DISCIP as d on d.cod_disc = up.k_discip " +
                @"where up.npp_g = @npp_g " +
                @"and up.k_fak = @k_fak " +
                @"and up.n_sem = @nomer_semestra " +
                @"and d.actual = 1 " +
                @"and (up.lekcii <> 0 or up.prakt <> 0 or up.laborat <> 0 or up.k_proekt <> 0 or up.k_rab <> 0) " +
                @"and up.not_kont = 0 " +
                @"order by d.snam_rus ";

            /// <summary>
            /// запрос на получение видов занятий
            /// </summary>
            public static string GetVidZanyatiy =
                @"select id, name_nagruzka " +
                @"from[ARCHIV].[dbo].[NAGRUZKA_VID] " +
                @"where(id between 1 and 3) " +
                @"or(id between 6 and 7) ";

            /// <summary>
            /// запрос на получение корпусов
            /// </summary>
            public static string GetKorpusa =
                @"SELECT distinct aud_korpus from VW_AUD_UCH ";

            /// <summary>
            /// запрос на получение аудиторий
            /// </summary>
            public static string GetAuditorii =
                @"select id_aud, aud_nomer from [DONNTU].[dbo].[VW_AUD_UCH] " +
                @"where ID_TIP_NAZNACHENYA = 1 and aud_korpus = @aud_korpus ";

            /// <summary>
            /// запрос на получение преподавателей
            /// </summary>
            public static string GetPreps =
                @"select distinct tabn_s, fio " +
                @"from [DONNTU].[dbo].[VW_PersonalPPSALL] ";

            /// <summary>
            /// запрос на добавление данных регистрации
            /// </summary>
            public static string InsertRegData =
                @"insert into [UUS].[dbo].[RASP_YEAR] (KOD_GRUP,UCH_GOD,N_SEM,KOD_FO,VID_PODG,KURS,NPP_G,K_FT) " +
                @"values " +
                @"(@kod_grup, @uch_god, @n_sem, " +
                @"(select k_fo from [ARCHIV].[dbo].[uch_gr] ug " +
                @"inner join[DONNTU].[dbo].[Gruppa] as g on ug.npp_g = g.npp_g_up and ug.k_fak = g.k_fak " +
                @"where g.KOD_GRUP = @kod_grup), " +
                @"@vid_podg, @kurs, @npp_g, @k_ft) ";

            /// <summary>
            /// запрос на получение id добавленной записи
            /// </summary>
            public static string GetIdRaspYear =
                @"select top(1) id from [UUS].[dbo].[RASP_YEAR] " +
                @"order by id DESC ";
            /// <summary>
            /// запрос на добавление пар расписания
            /// </summary>
            public static string InsertToRaspDiscip =
                "insert UUS.dbo.[RASP_DISCIP] " +
                "(ID_RASP_YEAR,D_NED,PR_NED,ID_RASP_TIME,ID_NAGRUZKA_VID,ID_AUD,K_DISCIP,DATE_S, PODGRUPPA) " +
                "values(@id_rasp_year, " +
                "(select id from [UUS].[dbo].[SP_DAY] " +
                " where [day] like @d_ned), " +
                "@pr_ned, " +
                "(select [id] from [UUS].[dbo].[RASP_Time] " +
                "where nom = @i and actual = 1), " +
                "@vid_podg, @codeAudit, @codeDisc, GETDATE(), @codepodgr)";

            /// <summary>
            /// запрос по назначению преподавателя для учебной пары расписания
            /// </summary>
            public static string InsertToRaspPps =
                "insert into [UUS].[dbo].[RASP_PPS]" +
                "(ID_RASP_DISCIP, TABN_S, DATE_S)" +
                "values" +
                "((select top(1) id from[UUS].[dbo].[RASP_DISCIP] order by id DESC), @tabn_s, GETDATE())";

            /// <summary>
            /// запрос для проверки существования расписания
            /// </summary>
            public static string GetFromRaspYear =
                "select * from [UUS].[dbo].[RASP_YEAR] " +
                "where KOD_GRUP = @kod_grup " +
                "and UCH_GOD = @uch_god " +
                "and vid_podg = @vid_podg " +
                "and k_ft = @k_ft " +
                "and n_sem = @n_sem";

            public static string GetRasps =
                "select " +
                "ry.id as id, ry.KOD_GRUP as kod_grup, ry.UCH_GOD as uch_god, ry.N_SEM as n_sem, KOD_FO as kod_fo, " +
                "ry.vid_podg as vid_podg, ry.KURS as kurs, ry.NPP_G as npp_g, ry.K_FT as k_ft, g.KOD_SPEC as kod_spec, " +
                "g.GOD_NABORA as god_nabora, g.NAME_GRUP_RUS as name_grup, tfd.fak_name as fak_name, svp.[name] as namepodg " +
                "from[UUS].[dbo].[RASP_YEAR] ry " +
                "inner join[DONNTU].[dbo].[Gruppa] as g on ry.KOD_GRUP = g.KOD_GRUP " +
                "inner join[DONNTU].[dbo].[SPECIAL] as s on s.COD_SP = g.KOD_SPEC " +
                "inner join[ARCHIV].[dbo].[T_FAC_DB] as tfd on ry.k_ft = tfd.k_ft " +
                "inner join[DONNTU].[dbo].[sp_vid_podg] as svp on svp.k_vid_podg = ry.VID_PODG ";
			///// <summary>
			///// запрос на получение пар
			///// </summary>
			//public static string GetFromRaspDiscip =
			//    "select * from [UUS].[dbo].[RASP_DISCIP] " +
			//    "where ID_RASP_YEAR = @id_rasp_year";

			///// <summary>
			///// запрос на получение дня недели по коду
			///// </summary>
			//public static string GetDay =
			//    "select [name] from[UUS].[dbo].[SP_DAY] " +
			//    "where id = @id_ned";

			/// <summary>
			/// запрос на получение пар по id_rasp_year с приджоенными днями неделями и номерами пар
			/// очень неудобно получать отдельно дни недели и номера пар
			/// </summary>
			public static string GetFromRaspDiscipSpDayVWRaspTime =
				" select * from [UUS].[dbo].[RASP_DISCIP] rd " +
				"inner join[UUS].[dbo].[SP_DAY] as sd on sd.id = rd.D_NED " +
				"inner join[UUS].[dbo].[RASP_Time] as vrt on vrt.ID = rd.ID_RASP_TIME " +
				"inner join [UUS].[dbo].[RASP_PPS] as rp on rp.ID_RASP_DISCIP = rd.ID " +
				"where rd.ID_RASP_YEAR = @id_rasp_year and rd.ACTUAL = 1 " +
                "order by sd.OrderName";

            public static string DeleteAll =
                "delete from [UUS].[dbo].[RASP_DISCIP] " +
                "where ID_RASP_YEAR = @id_rasp_year";

            public static string GetYearsFromRasp =
                "select distinct uch_god from [UUS].[dbo].[RASP_YEAR] " +
                "order by uch_god";

            public static string GetRaspsPoId =
                "select * from [UUS].[dbo].[RASP_DISCIP]" +
                "where id_rasp_year = @id_rasp_year";

            /// <summary>
            /// Дополнительные параметры для запросов
            /// </summary>
            public struct Params
            {
                /// <summary>
                /// код факультета для Gruppa
                /// </summary>
                public static string kod_ftGruppa = "and k_fak = @kod_ft ";
                /// <summary>
                /// код факультета для 
                /// </summary>
                public static string kod_ftSpec = "and cod_fak = @kod_ft ";
                /// <summary>
                /// вид подготовки
                /// </summary>
                public static string vid_podg = "and vid_podg = @vid_podg ";
                /// <summary>
                /// год набора
                /// </summary>
                public static string god_nabora = "and god_nabora = @god_nabora ";
                /// <summary>
                /// код специальности
                /// </summary>
                public static string kod_spec = "and kod_spec = @kod_spec ";

                public struct ForGetRasps
                {
                    public static string id = "and id = @id ";

                    public static string kod_grup = "and ry.kod_grup = @kod_grup ";

                    public static string uch_god = "and uch_god = @uch_god ";

                    public static string n_sem = "and n_sem = @n_sem ";

                    public static string vid_podg = "and ry.vid_podg = @vid_podg ";

                    public static string k_ft = "and ry.k_ft = @k_ft ";

                    public static string god_nabora = "and god_nabora = @god_nabora ";

                    public static string kod_spec = "and kod_spec = @kod_spec ";
                }
                
            }
        }
    }
    
    /// <summary>
    /// Один день недели
    /// </summary>
    public class Cells
    {
        public string Day { get; set; }
        public myCell FirstPair { get; set; }
        public myCell SecondPair { get; set; }
        public myCell ThirdPair { get; set; }
        public myCell FourthPare { get; set; }
        public myCell FifthPair { get; set; }
        public Cells(string Day)
        {
            this.Day = Day;
            FirstPair = new myCell();
            SecondPair = new myCell();
            ThirdPair = new myCell();
            FourthPare = new myCell();
            FifthPair = new myCell();
        }
        public myCell this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return FirstPair;
                    case 1:
                        return SecondPair;
                    case 2:
                        return ThirdPair;
                    case 3:
                        return FourthPare;
                    case 4:
                        return FifthPair;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        FirstPair = value;
                        break;
                    case 1:
                        SecondPair = value;
                        break;
                    case 2:
                        ThirdPair = value;
                        break;
                    case 3:
                        FourthPare = value;
                        break;
                    case 4:
                        FifthPair = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    /// <summary>
    /// Данные ячейки
    /// </summary>
    public class OneItem
    {
        public string AllAttr { get; set; } = "";
        public string NameGr { get; set; }
        public int CodeGr { get; set; }
        public string NameDisc { get; set; } = null;
        public int CodeDisc { get; set; }
        public string NameVid { get; set; } = null;
        public int CodeVid { get; set; }
        public string NameAudit { get; set; } = null;
        public int CodeAudit { get; set; }
        public string NamePrep { get; set; } = null;
        public int CodePrep { get; set; }
        public int IdRaspYear { get; set; }
        public int CodePodgr { get; set; } = -1;
        public int NomerKorpusa { get; set; }
        public OneItem() { }
        public OneItem(int CodeDisc, int CodeVid, int CodeAudit, int CodePrep, int CodePodgr)
        {
            this.CodeDisc = CodeDisc;
            this.CodeVid = CodeVid;
            this.CodeAudit = CodeAudit;
            this.CodePrep = CodePrep;
            this.CodePodgr = CodePodgr;
            using (SqlCommand SCKorp = new SqlCommand(Db.SQLCommands.GetKorpusa, Db.myConnection))
            {
                SCKorp.CommandText += "where id_aud = @aud_id";
                SCKorp.Parameters.AddWithValue("@aud_id", CodeAudit);
                using (SqlDataReader SDRKorp = SCKorp.ExecuteReader())
                {
                    SDRKorp.Read();
                    NomerKorpusa = Convert.ToInt32(SDRKorp["AUD_KORPUS"]);
                }
            }
        }

        public void DrawItem()
        {
            try
            {
                using (SqlCommand SCItem = new SqlCommand())
                {
                    if (CodePodgr == 1 || CodePodgr == 2)
                        AllAttr += string.Format("{0} подгруппа: ", CodePodgr);
                    SCItem.Connection = Db.myConnection;
                    SCItem.CommandText = "select snam_rus from DONNTU.dbo.DISCIP where cod_disc = @arg";
                    SCItem.Parameters.AddWithValue("@arg", CodeDisc);
                    using (SqlDataReader SDR = SCItem.ExecuteReader())
                    {
                        SDR.Read();
                        AllAttr += SDR.GetString(0) + " ";
                    }
                    SCItem.CommandText = "select name_nagruzka " +
                        "from[ARCHIV].[dbo].[NAGRUZKA_VID] " +
                        "where id = @arg";
                    SCItem.Parameters[0].Value = CodeVid;
                    using (SqlDataReader SDR = SCItem.ExecuteReader())
                    {
                        SDR.Read();
                        AllAttr += string.Format(" - {0} ", SDR.GetString(0));
                    }
                    SCItem.CommandText = "select AUD_NOMER_S from [DONNTU].[dbo].[VW_AUD_UCH] where ID_AUD = @arg";
                    SCItem.Parameters[0].Value = CodeAudit;
                    using (SqlDataReader SDR = SCItem.ExecuteReader())
                    {
                        SDR.Read();
                        AllAttr += string.Format("({0}) ",SDR.GetString(0));
                    }
                    SCItem.CommandText = "select distinct fio " +
                        "from[DONNTU].[dbo].[VW_PersonalPPSALL] " +
                        "where TABN_S = @arg";
                    SCItem.Parameters[0].Value = CodePrep;
                    using (SqlDataReader SDR = SCItem.ExecuteReader())
                    {
                        SDR.Read();
                        AllAttr += SDR.GetString(0);
                    }
                }
            }
            catch { }
        }
    }

    /// <summary>
    /// Класс с командами копирования, вставки и сохранения, не доделаны: копия/вставка
    /// </summary>
    static class Commands
    {
        public static RoutedCommand CopyCells = new RoutedCommand();
        public static RoutedCommand InsertCells = new RoutedCommand();
        public static RoutedCommand Save = new RoutedCommand();
    }

}
