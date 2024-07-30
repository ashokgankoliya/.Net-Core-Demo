<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PageTrazeMapping.ascx.vb" Inherits="pages_PageTrazeMapping" %>


<script src="js/validation.js"></script>
<style type="text/css">
    /* ---- button ---- */
    button {
        display: inline-block;
        padding: 3px 10px;
        background: #EEE;
        border: none;
        color: #222;
        cursor: pointer;
    }

    .button:hover,
    .button:active,
    .button.is-checked {
        background-color: #00A2FF;
    }

    button.is-checked {
        color: white;
    }


    .SmallButton {
        background-color: #333D49;
        -moz-border-radius: 28px;
        -webkit-border-radius: 28px;
        border-radius: 28px;
        border: 1px solid #84bbf3;
        display: inline-block;
        cursor: pointer;
        color: #ffffff;
        font-family: arial;
        font-size: 9px;
        padding: 4px 7px;
        text-decoration: none;
        text-shadow: 0px 1px 0px #528ecc;
    }

        .SmallButton:hover {
            background-color: #378de5;
        }
</style>

<style type="text/css">
    .UsersDiv {
        width: 100%;
        float: left;
    }

    .add {
        background: url(images/add_icon.gif) no-repeat 10px 0px;
        text-indent: 30px;
        display: block;
        font-size: 1.2em;
        height: 16px;
    }


    .delete {
        background: url(images/Delete-icon.png) no-repeat 10px 0px;
        text-indent: 30px;
        display: block;
        font-size: 1.2em;
        height: 16px;
    }
</style>
<div>
    <table id="fgrdTrazeMapping" style="display: none;"></table>
</div>


<div id="TrazeId" style="display: none;"></div>

<div id="msgbox-select" title="Alert">
    <p id="msg"></p>
</div>

<div id="DivTrazeMapping" style="display: none;" class="filesTab">
    <table style="text-align: left; border-collapse: collapse; font-size: 14px;">
        <tr style="height: 28px">

            <td colspan="2">
                <div class="Warning" align="center">
                    OOPS! Unable to process your request. Please contact admin!
                </div>
                <div class="YellowWarning" align="center">
                    All fields are required fields, Please try again.
                </div>
            </td>
        </tr>
        <tr style="margin-bottom: 20px;">
            <td style="padding-right: 30px;">FLOWER</td>
            <td colspan="5">
                <input type="text" id="txtFlower" style="width: 350px;" class="AllUpperCaseTextBox" />
                 <input type="hidden" id="hdnFlowerCode" />
            </td>
            <td style="padding-right: 10px;">UOM</td>
            <td>
                <input type="text" id="txtUoM" style="width: 40px;" class="AllUpperCaseTextBox" /></td>
        </tr>
        <tr>
            <td>PACK</td>
            <td>
                <input type="text" id="txtPack" style="width: 40px;" class="AllUpperCaseTextBox ClsNumbersOnly" /></td>
            <td style="padding-left: 10px;padding-right: 20px;">PACK UNIT</td>
            <td>
                <input type="text" id="txtPackUnit" style="width: 40px;" class="AllUpperCaseTextBox" /></td>
            <td style="padding-left: 10px;padding-right: 20px;">INNER PACK</td>
            <td>
                <input type="text" id="txtInnerPack" style="width: 40px;" class="AllUpperCaseTextBox ClsNumbersOnly" /></td>
            <td style="padding-left: 10px;padding-right: 20px;">INNER PACK UNIT</td>
            <td>
                <input type="text" id="txtInnerPackUnit" style="width: 40px;" class="AllUpperCaseTextBox" /></td>
        </tr>
    </table>
</div>


<script type="text/javascript">

    $(document).ready(new function () {
        $("#fgrdTrazeMapping").flexigrid({
            url: 'BloomService.asmx/GetTrazeMapping',
            dataType: 'xml',
            colModel: [
                { display: 'Delete', name: 'Delete', width: 35, sortable: true, align: 'center' },
                { display: 'Edit', name: 'Edit', width: 25, sortable: true, align: 'center' },
                { display: 'FLOWER', name: 'FLOWER', width: 82, sortable: true, align: 'left' },
                { display: 'UOM', name: 'UOM', width: 50, sortable: true, align: 'center' },
                { display: 'PACK', name: 'PACK', width: 50, sortable: true, align: 'center' },
                { display: 'PACK UNIT', name: 'PACKUNIT', width: 68, sortable: true, align: 'center' },
                { display: 'INNER PACK', name: 'INNERPACK', width: 68, sortable: true, align: 'center' },
                { display: 'INNER PACK UNIT', name: 'INNERPACKUNIT', width: 100, sortable: true, align: 'center' }
            ],
            searchitems: [
                { display: 'FLOWER', name: 'FLOWER' },
            ],

            buttons: [
                { name: 'Add New', bclass: 'add', onpress: addclicked },
            ],

            showTableToggleBtn: true,
            sortname: "FLOWER",
            sortorder: "asc",
            usepager: true,
            title: false,
            useRp: true,
            qtype: "FLOWER",
            nomsg: 'No records found!',
            singleSelect: true,
            showToggleBtn: false,
            resizable: false,
            autoload: true,
            showTableToggleBtn: false,
            height: $(window).height() - 300,
            striped: true,


        });

        function addclicked(com, grid) {
            $(".YellowWarning").hide();
            $("#DivTrazeMapping").dialog('option', 'title', "Add New Traze Values");
            $("#DivTrazeMapping").dialog("open");
            $("#txtFlower").val("");
            $("#txtUoM").val("");
            $("#txtPack").val("");
            $("#txtPackUnit").val("");
            $("#txtInnerPack").val("");
            $("#txtInnerPackUnit").val("");
            $("#TrazeId").html("0");
            $(".Warning").hide();
        }

        $("#txtFlower").autocomplete({
        source: function (request, response) {
            var Farm = "";
            $.ajax({
                type: "POST",
                url: 'BloomService.asmx/GetFlowerEasyAutocomplete',
                data: "{'Searchtext': '" + request.term + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    response($.map(data.d, function (flr) {
                        return {
                            label: "[" + flr.Flower.trim() + "] " + flr.Name,
                            Flower: flr.Flower,
                            Name: flr.Name,
                            ID: flr.ID,
                            CAT: flr.CAT,
                            Bunch: flr.BUNCH,
                            ColorCode: flr.ColorCode,
                            UOM: flr.UOM,
                            UNITS: flr.UNITS,
                            Soldas: flr.FType.SOLDAS
                        }
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        open: function (event, ui) {

        },
        select: function (e, i) {
            $("#txtFlower").val("[" + i.item.Flower.trim() + "] " + i.item.Name);
            $("#hdnFlowerCode").val(i.item.Flower.trim());
        },
        change: function (e, i) {
            if (i.item == null || i.item == undefined) {
                $("#txtFlower").val("");
                $(".ui-menu").css("display", "none");
            }
        },
        minLength: 1,
        autoFocus: true
    });

        function validate() {
            $('.YellowWarning').hide();
            if ($("#txtFlower").val() == "") {
                $('.YellowWarning').html("Flower is required to save!");
                $('.YellowWarning').show();
                $("#txtFlower").focus();
                return false;
            }
            return true;
        }

        $("#DivTrazeMapping").dialog({

            autoOpen: false,
            width: 710,
            modal: true,
            resizable: false,
            buttons: {
                Save: function () {
                    if (validate()) {

                        $('.YellowWarning').html("Please wait...");
                        $('.YellowWarning').show();
                        $('.Warning').hide();
                        var TrazeId = $("#TrazeId").html();
                        var FlowerCode = $("#hdnFlowerCode").val().toUpperCase();
                        var UoM = $("#txtUoM").val().toUpperCase();
                        var Pack = $("#txtPack").val().toUpperCase();
                        var PackUnit = $("#txtPackUnit").val().toUpperCase();
                        var InnerPack = $("#txtInnerPack").val().toUpperCase();
                        var InnerPackUnit = $("#txtInnerPackUnit").val().toUpperCase();
                        $.ajax({
                                type: "POST",
                                url: 'BloomService.asmx/SaveTRAZEVALUES',
                                data: '{"ID":"' + TrazeId + '","FLOWER":"' + FlowerCode.toUpperCase() + '","UOM":"' + UoM.toUpperCase() + '","PACK":"' + Pack.toUpperCase() + '","PACKUNIT":"' + PackUnit.toUpperCase() + '","INNERPACK":"' + InnerPack.toUpperCase() + '","INNERPACKUNIT":"' + InnerPackUnit.toUpperCase() + '"}',
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (output) {
                                        $("#fgrdTrazeMapping").flexReload({ url: '/url/for/refresh' });
                                        $("#DivTrazeMapping").dialog("close");
                                        return true;                                  
                                },
                                error: function (e) {
                                    alert(e.responseText);
                                    $('.YellowWarning').hide();
                                    $('.Warning').show();
                                    return false;
                                }
                            });
                    }
                    else
                        return false;

                },
                Cancel: function () {
                    $(this).dialog("close");
                },
            },

            open: function () {
                $(".ui-dialog-buttonpane button:contains('Save')").addClass('dialogbuttonstyle icon-save')
                $(".ui-dialog-buttonpane button:contains('Cancel')").addClass('dialogbuttonstyle icon-cancel')
            }
        });


        $("[id^=EditTrazeValues_]").live("click", function () {
            $('.YellowWarning').hide();
            $('.Warning').hide();
            var ID = $(this).attr("id").split("_")[1];

            $("#TrazeId").html(ID);
            $.ajax({
                type: "POST",
                url: 'BloomService.asmx/GetTRAZEVALUESByID',
                data: '{"ID":"' + ID + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (output) {
                    var List = output.d;
                    if (List.ID == ID) {

                        $("#hdnFlowerCode").val(List.FLOWER);
                        $("#txtFlower").val(List.FLOWERNAME);
                        $("#txtUoM").val(List.UOM);
                        $("#txtPack").val(List.PACK);
                        $("#txtPackUnit").val(List.PACKUNIT);
                        $("#txtInnerPack").val(List.INNERPACK);
                        $("#txtInnerPackUnit").val(List.INNERPACKUNIT);
                        $("#DivTrazeMapping").dialog('option', 'title', 'Traze Values :: Edit');
                        $("#DivTrazeMapping").dialog("open");
                    }
                    else {
                        $("#msg").html("Unable to get the item details, please contact site admin.");
                        $("#msgbox-select").dialog("open");
                    }
                },
                error: function (e) {
                    $("#msg").html("OOPS! Something went wrong, please try again, we will research the issue.");
                    $("#msgbox-select").dialog("open");
                }
            });
        });


        $("[id^=DeleteTrazeValues_]").live("click", function () {
            var ID = $(this).attr("id").split("_")[1];
            $('.YellowWarning').hide();
            $('#TrazeId').html(ID);

            $.ajax({
                type: "POST",
                url: 'BloomService.asmx/DeleteTRAZEVALUESByID',
                data: '{"ID":"' + ID + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (output) {
                    $("#fgrdTrazeMapping").flexReload({ url: '/url/for/refresh' });  
                }

            });
        });

    });
</script>
