<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="Index" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="refresh" content="60" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta http-equiv="Content-Type" content="text/html;charset=utf-8;" />
    <title>제21대 국회의원선거(20')</title>
    <link rel="stylesheet" type="text/css" href="./css/reset.css" />
    <link rel="stylesheet" type="text/css" href="./css/style.css" />
    <script type="text/javascript" src="./js/jquery-3.1.1.min.js"></script>
    <script type="text/javascript" src="./js/jqbar.js"></script>
    <script type="text/javascript">
        var sum = 60; // 새로고침 시간        

        $(document).ready(function () {
            // 새로고침
            setInterval(function () {
                $(".timer").html("(" + --sum + "초 후 새로고침)");
            }, 1000);

            // 그래프
            $(".bar").each(function () {
                $(this).jqbar({
                    value: $(this).find(".blind").html()
                });
            });

            // 확인창
            $("span.aspNetDisabled").on("click", function () {
                alert("1위만 선택할 수 있습니다.");
            });

            // 라디오 버튼 1개만 선택
            $("input:radio").click(function () {
                $(":input").each(function (index) {
                    if ($(this).attr("type").toLowerCase() == "radio") {
                        if (this.checked) {
                            if (event.target != this) {
                                $(this).prop("checked", false);
                            }
                        }
                    }
                });
            });
        });

        function confirm_save() {
            if ($(":input:checked").length == 0) {
                alert("당선/확실/유력 중 1가지를 선택하세요.");
                return false;
            }
            else if (!confirm("저장하시겠습니까?")) {
                return false;
            }

            return true;
        }

        function confirm_reset() {
            if ($(":input:checked").length == 0) {
                alert("초기화할 값이 없습니다.");
                return false;
            }
            else if (!confirm("초기화하시겠습니까?")) {
                return false;
            }

            return true;
        }
    </script>
</head>

<body>
    <form id="form1" runat="server">

        <ul class="main_menu">
            <asp:Repeater runat="server" ID="sdListRepeater">
                <ItemTemplate>
                    <li class="sd_list">
                        <a href='<%# string.Format("Index.aspx?sdcode={0}&mode={1}", Eval("sdcode"), Eval("mode")) %>'>
                            <%# Eval("sdnamess") %>
                        </a>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>


        <ul class="sub_menu">
            <asp:Repeater runat="server" ID="sggListRepeater">
                <ItemTemplate>
                    <li class="sgg_list">
                        <a href='<%# string.Format("Index.aspx?sdcode={0}&sggcode={1}&mode={2}", Eval("sdcode"), Eval("sggcode"), Eval("mode")) %>'>
                            <span class="dispname"><%# Eval("dispname") %></span>
                            <%# Eval("sgnamess") %>
                        </a>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>


        <div class="left_contents">
            ■ 제21대 국회의원선거 >
            <asp:Label runat="server" ID="sdname" Font-Bold="true" />
            >
            <asp:Label runat="server" ID="sggname" Font-Bold="true" />

            <ul>
                <asp:Repeater runat="server" ID="huboListRepeater">
                    <ItemTemplate>
                        <li class="hubo_list">
                            <table class="hubo_table">
                                <colgroup>
                                    <col width="10%" />
                                    <col width="12%" />
                                    <col width="23%" />
                                    <col width="33%" />
                                    <col width="7%" />
                                    <col width="15%" />
                                </colgroup>
                                <tr>
                                    <td class="giho" rowspan="2">기호<br>
                                        <span><%# Eval("giho") %></span>
                                    </td>
                                    <td rowspan="2">
                                        <img class="photo" src='<%# string.Format("./image/{0}.jpg", Eval("huboid")) %>' />
                                    </td>
                                    <td class="name" colspan="2">
                                        <span><%# Eval("hbname") %></span>
                                        <%# Eval("jdname") %> (<%# Eval("age") %>)
                                    </td>
                                    <td class="seq">
                                        <%# Eval("seq") %>
                                    </td>
                                    <td class="pangb" rowspan="2">
                                        <asp:RadioButtonList runat="server" ID="pangbList" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ggpyo">
                                        <%# Eval("ggpyo") %>표 (<%# Eval("ggrate") %>%)
                                    </td>
                                    <td class="bar" colspan="2">
                                        <span class="blind"><%# Eval("ggrate") %></span>
                                    </td>
                                </tr>
                            </table>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>

            <p class="offer">(중앙선거관리위원회 제공)</p>
        </div>


        <div class="right_contents">
            <p class="timer">(60초 후 새로고침)</p>

            <table class="result_table">
                <colgroup>
                    <col width="50%" />
                    <col width="50%" />
                </colgroup>
                <caption>
                    <asp:Label runat="server" ID="publishtime" />
                </caption>
                <tr>
                    <th>총 투표수</th>
                    <td>
                        <asp:Label runat="server" ID="tupyo" />
                    </td>
                </tr>
                <tr>
                    <th>개표수</th>
                    <td>
                        <asp:Label runat="server" ID="gaepyo_sum" />
                    </td>
                </tr>
                <tr>
                    <th>미개표수</th>
                    <td>
                        <asp:Label runat="server" ID="no_gaepyo_sum" />
                    </td>
                </tr>
                <tr>
                    <th>개표율</th>
                    <td>
                        <asp:Label runat="server" ID="gaepyo_rate" />%
                    </td>
                </tr>
                <tr>
                    <th>1,2위 표차</th>
                    <td>
                        <asp:Label runat="server" ID="diff" />
                    </td>
                </tr>
            </table>

            <asp:Button runat="server" ID="saveBtn" CssClass="result_btn" Text="저장" OnClientClick="return confirm_save();" OnClick="saveBtn_Click" />
            <asp:Button runat="server" ID="resetBtn" CssClass="result_btn" Text="초기화" OnClientClick="return confirm_reset();" OnClick="resetBtn_Click" />
        </div>

    </form>


    <script type="text/javascript">
        function urlParam(name) {
            var results = new RegExp("[\?&]" + name + "=([^&#]*)").exec(window.location.href);

            if (results == null)
                return null;
            else
                return results[1] || 0;
        }

        // 읽기 모드일 경우
        if (urlParam("mode") == "read") {
            $(".offer").attr("style", "display: block;");
            $(".hubo_table .pangb").remove();
            $("#saveBtn").remove();
            $("#resetBtn").remove();
        }

        // 선택 메뉴 체크(CSS)
        var sdcode = urlParam("sdcode");
        var sggcode = urlParam("sggcode");

        // 메인 메뉴 체크
        if (sdcode == "" || sdcode == null) {
            // 서브 메뉴 체크
            if (sggcode == "" || sggcode == null) {
                $(".sd_list:first-child>a").addClass("selected");
                $(".sgg_list:first-child>a").addClass("selected");
            }
            else {
                location.href = "Index.aspx?sdcode=" + sggcode.substring(1, 3) + "00" + "&sggcode=" + sggcode;
            }
        }
        else {
            for (var i = 0; i < $(".sd_list>a").length; i++) {
                if (($(".sd_list>a")[i]).href.indexOf("sdcode=" + sdcode) != -1) {
                    $(".sd_list>a").eq(i).addClass("selected");
                    break;
                }
            }

            // 서브 메뉴 체크
            if (sggcode == "" || sggcode == null) {
                $(".sgg_list:first-child>a").addClass("selected");
            }
            else {
                for (var i = 0; i < $(".sgg_list>a").length; i++) {
                    if (($(".sgg_list>a")[i]).href.indexOf("sggcode=" + sggcode) != -1) {
                        $(".sgg_list>a").eq(i).addClass("selected");
                        break;
                    }
                }
            }
        }
    </script>
</body>

</html>
