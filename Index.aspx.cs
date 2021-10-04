using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Index : Page
{
    DataTable dt;
    List<Hubo> hubo = new List<Hubo>();
    string sdcode = "";
    string sggcode = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        // 세션
        if (Session["empno"] == null)
            Session["empno"] = Request["userid"];

        // 리디렉션
        //string strRefer = Request.ServerVariables["HTTP_REFERER"];
        //if (strRefer == null)
        //{
        //    if (Session["empno"] == null)
        //        Page.RegisterClientScriptBlock("done", @"<script>alert('권한이 없거나 잘못된 방식으로 접근중입니다!');location.href='http://sis.seoul.co.kr/';</script>");
        //}
        //else if (strRefer.IndexOf("http://sis.seoul.co.kr/other/vote/go_vote.php") == -1 && strRefer.IndexOf("http://system6.seoul.co.kr/other/vote") == -1)
        //{
        //    Page.RegisterClientScriptBlock("done", @"<script>alert('권한이 없거나 잘못된 방식으로 접근중입니다!');location.href='http://sis.seoul.co.kr/';</script>");
        //}

        // 시도 코드
        if (Request["sdcode"] == "" || Request["sdcode"] == null)
        {
            // 없으면 첫번째 시도
            dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"select top 1 sdcode from sd order by srt")), "SELECT");

            sdcode = dt.Rows[0]["sdcode"].ToString();
        }
        else
        {
            sdcode = Request["sdcode"];
        }

        // 선거구 코드
        if (Request["sggcode"] == "" || Request["sggcode"] == null)
        {
            // 없으면 해당 시도의 첫번째 선거구
            dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"select top 1 sggcode from sgg where sdcode = '{0}' order by sgw_srt", sdcode)), "SELECT");

            sggcode = dt.Rows[0]["sggcode"].ToString();
        }
        else
        {
            sggcode = Request["sggcode"];
        }

        if (!IsPostBack)
        {
            SetMainMenu();
            SetSubMenu();
            SetMenuLabel();
            SetHuboTable();
            SetResultTable();
        }
    }

    // 메인 메뉴(시도)
    private void SetMainMenu()
    {
        List<SD> sd = new List<SD>();

        dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"select * from sd order by srt")), "SELECT");

        foreach (DataRow dr in dt.Rows)
        {
            sd.Add(new SD()
            {
                sdcode = dr["sdcode"].ToString(),
                sdnamess = dr["sdnamess"].ToString(),
                mode = Request["mode"]
            });
        }

        sdListRepeater.DataSource = sd;
        sdListRepeater.DataBind();
    }

    // 서브 메뉴(선거구)
    private void SetSubMenu()
    {
        List<SGG> sgg = new List<SGG>();

        dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"select * from sgg where sdcode = '{0}' order by sgw_srt", sdcode)), "SELECT");

        foreach (DataRow dr in dt.Rows)
        {
            sgg.Add(new SGG()
            {
                sdcode = dr["sdcode"].ToString(),
                sggcode = dr["sggcode"].ToString(),
                sgnamess = dr["sgnamess"].ToString(),
                dispname = GetPangb(dr["sggcode"].ToString()),
                mode = Request["mode"]
            });
        }

        sggListRepeater.DataSource = sgg;
        sggListRepeater.DataBind();
    }

    // 선거구 판정 구분(★,☆,..)
    private string GetPangb(string sggcode)
    {
        DataTable dt2 = Util.ExecuteQuery(new SqlCommand(string.Format(@"select seq, (select dispname from pangb where pangb = hubo.pangb) as dispname from hubo where sggcode = '{0}' and pangb > 0", sggcode)), "SELECT");

        if (dt2.Rows.Count > 0)
        {
            if (dt2.Rows[0]["seq"].ToString() == "1")
                return dt2.Rows[0]["dispname"].ToString();
            else
                return "<span style='color: red;'>?</span>";
        }
        else
        {
            return "";
        }
    }

    // 선택 메뉴 라벨
    private void SetMenuLabel()
    {
        // 시도
        dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"select sdnamess from sd where sdcode = {0}", sdcode)), "SELECT");

        if (dt.Rows.Count > 0)
            sdname.Text = dt.Rows[0]["sdnamess"].ToString();

        // 선거구
        dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"select sgnamess from sgg where sggcode = '{0}'", sggcode)), "SELECT");

        if (dt.Rows.Count > 0)
            sggname.Text = dt.Rows[0]["sgnamess"].ToString();
    }

    // 후보 테이블
    private void SetHuboTable()
    {
        dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"select b.*, isnull(a.ggpyo, 0) as ggpyo, isnull(a.ggrate, 0) as ggrate, (select webdispname from pangb where pangb = b.pangb) as webdispname from hubo b left outer join hbpyo_v a on a.huboid = b.huboid where b.sggcode = '{0}' order by b.giho", sggcode)), "SELECT");

        foreach (DataRow dr in dt.Rows)
        {
            hubo.Add(new Hubo()
            {
                giho = dr["giho"].ToString(),
                huboid = dr["huboid"].ToString(),
                jdname = dr["jdname"].ToString(),
                hbname = dr["hbname"].ToString(),
                age = dr["age"].ToString(),
                ggpyo = SetComma(Convert.ToDouble(dr["ggpyo"])),
                ggrate = SetRound(Convert.ToDouble(dr["ggrate"])),
                seq = (dr["seq"].ToString() == "1" || dr["seq"].ToString() == "2") ? ((dr["seq"].ToString() == "1") ? "<span class=seq1>" + dr["seq"].ToString() + "위</span>" : "<span class=seq2>" + dr["seq"].ToString() + "위</span>") : "",
                pangb = dr["pangb"].ToString(),
                webdispname = dr["webdispname"].ToString()
            });
        }

        huboListRepeater.DataSource = hubo;
        huboListRepeater.DataBind();

        SetPangbTable();
    }

    // 후보 판정 테이블
    private void SetPangbTable()
    {
        RadioButtonList pangbList;

        dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"select * from (select webdispname, min(pangb) pangb from pangb group by webdispname) a order by pangb desc")), "SELECT");

        for (int i = 0; i < huboListRepeater.Items.Count; i++)
        {
            pangbList = (RadioButtonList)huboListRepeater.Items[i].FindControl("pangbList");

            // 라디오 버튼 세팅  
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["webdispname"].ToString() != "")
                {
                    if (hubo[i].seq.Contains("1위"))
                        pangbList.Items.Add(new ListItem(" " + dr["webdispname"].ToString(), hubo[i].huboid + "|" + dr["pangb"].ToString(), true));
                    else
                        pangbList.Items.Add(new ListItem(" " + dr["webdispname"].ToString(), hubo[i].huboid + "|" + dr["pangb"].ToString(), false));
                }
            }

            // 라디오 버튼 값 세팅    
            switch (hubo[i].pangb)
            {
                case "1": // 유력
                    pangbList.SelectedIndex = 2;
                    break;
                case "2": // 확실
                    pangbList.SelectedIndex = 1;
                    break;
                case "3": // 당선
                case "7":
                case "9":
                    pangbList.SelectedIndex = 0;
                    break;
                case "0": // 미확정
                default:
                    break;
            }
        }
    }

    // 결과 테이블
    private void SetResultTable()
    {
        dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"select * from gaepyo_v where sggcode = '{0}'", sggcode)), "SELECT");

        // 집계 시간
        string pt = dt.Rows[0]["publishtime"].ToString();
        if (pt.Length == 12)
            publishtime.Text = (pt.Substring(4, 1) == "0" ? pt.Substring(5, 1) : pt.Substring(4, 2)) + "월 " + (pt.Substring(6, 1) == "0" ? pt.Substring(7, 1) : pt.Substring(6, 2)) + "일 " + pt.Substring(8, 2) + ":" + pt.Substring(10, 2) + " 기준";
        else
            publishtime.Text = "<span style='color: red;'>개표 시작 전</span>";

        // 총 투표수
        tupyo.Text = SetComma(Convert.ToDouble(dt.Rows[0]["tupyo"]));
        // 개표수
        gaepyo_sum.Text = SetComma(Convert.ToDouble(dt.Rows[0]["gaepyo_sum"]));
        // 미개표수
        no_gaepyo_sum.Text = SetComma(Convert.ToDouble(dt.Rows[0]["tupyo"]) - Convert.ToDouble(dt.Rows[0]["gaepyo_sum"]));
        // 개표율
        gaepyo_rate.Text = SetRound(Convert.ToDouble(dt.Rows[0]["gaepyo_rate"]));

        dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"select top(2) * from hbpyo_v where sggcode = '{0}' order by ggpyo desc, huboid", sggcode)), "SELECT");

        // 1,2위 표차
        if (dt.Rows.Count > 0)
            diff.Text = SetComma(Convert.ToDouble(dt.Rows[0]["ggpyo"]) - Convert.ToDouble(dt.Rows[1]["ggpyo"]));
        else
            diff.Text = "-";
    }

    // 저장 버튼 클릭
    protected void saveBtn_Click(object sender, EventArgs e)
    {
        RadioButtonList pangbList;
        string huboid_value = "";
        string pangb_value = "";

        for (int i = 0; i < huboListRepeater.Items.Count; i++)
        {
            pangbList = (RadioButtonList)huboListRepeater.Items[i].FindControl("pangbList");

            // 라디오 버튼 값이 있으면
            if (pangbList.SelectedIndex > -1)
            {
                huboid_value = pangbList.SelectedValue.Split('|')[0];
                pangb_value = pangbList.SelectedValue.Split('|')[1];

                try
                {
                    dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"select seq, pangb from hubo where huboid = '{0}'", huboid_value)), "SELECT");

                    // 현재 1위이고, 무투표 당선이 아니면
                    if (dt.Rows[0]["seq"].ToString() == "1" && dt.Rows[0]["pangb"].ToString() != "9")
                    {
                        // 초기화
                        dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"update hubo set pangb = 0 where sggcode = '{0}' and pangb != 9", sggcode)), "UPDATE");

                        // 업데이트 1
                        dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"update hubo set pangb = '{0}' where huboid = '{1}'", pangb_value, huboid_value)), "UPDATE");

                        // 업데이트 2
                        dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"update vote.dbo.hubo set pangb = '{0}' where huboid = '{1}'", pangb_value, huboid_value)), "UPDATE");

                        // 로그
                        dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"insert into pangb_log (sggcode, sgname, updateuser, client_ip, huboid, pangb, maketime) values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', getdate())", sggcode, sggname.Text, Session["empno"], Request.UserHostAddress, huboid_value, pangb_value)), "INSERT");

                        break;
                    }
                }
                catch (Exception)
                {
                    Confirm("오류 발생! IT개발부로 문의 바랍니다.");
                }
            }
        }

        Confirm("저장되었습니다.");

        Refresh();
    }

    // 초기화 버튼 클릭
    protected void resetBtn_Click(object sender, EventArgs e)
    {
        try
        {
            // 초기화
            dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"update hubo set pangb = 0 where sggcode = '{0}' and pangb != 9", sggcode)), "UPDATE");

            // 로그
            dt = Util.ExecuteQuery(new SqlCommand(string.Format(@"insert into pangb_log (sggcode, sgname, updateuser, client_ip, pangb, maketime) values ('{0}', '{1}', '{2}', '{3}', 0, getdate())", sggcode, sggname.Text, Session["empno"], Request.UserHostAddress)), "INSERT");
        }
        catch (Exception)
        {
            Confirm("오류 발생! IT개발부로 문의 바랍니다.");
        }

        Confirm("초기화되었습니다.");

        Refresh();
    }

    // 새로고침
    private void Refresh()
    {
        SetSubMenu();
        SetHuboTable();
        SetResultTable();
    }

    // 확인창
    private void Confirm(string msg)
    {
        ScriptManager.RegisterClientScriptBlock(this, GetType(), "alert", "alert('" + msg + "');", true);
    }

    // 숫자 세자리마다 콤마
    private string SetComma(double value)
    {
        if (value == 0)
            return "0";
        else
            return string.Format("{0:#,###}", value);
    }

    // 소수점 둘째자리까지 반올림
    private string SetRound(double value)
    {
        return string.Format("{0:F2}", value);
    }
}
