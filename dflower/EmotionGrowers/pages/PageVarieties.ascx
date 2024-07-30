<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PageVarieties.ascx.vb" Inherits="pages_PageVarieties" %>

<style>
    .add {
        background: url(images/add_icon.gif) no-repeat 10px 0px;
        text-indent: 30px;
        display: block;
        font-size: 1.2em;
        height: 16px;
    }

    .trSelectedHeader {
        background-color: red !important;
    }

    /**/
    .largepip {
        display: inline-block;
        margin: 10px 10px 0 0;
    }
    /*#EditVarieties input[type="text"] {
        height: 17px;
        background-color:cornsilk;
    }
    #EditVarieties select {
        background-color:cornsilk;
    }*/
</style>


<table>
    <tr>
        <td>

            <div id="DivVarietiesForFgrd">
                <table id="fgrdVarieties" style="display: none;"></table>
                <div id="DeleteConfirmVarieties" style="display: none;" title="Alert"></div>
                <div id="OldVarietiesId" style="display: none;"></div>
            </div>
        </td>
    </tr>
</table>

<div id="EditVarieties" style="display: none; height: 150px !important;" class="filesTab">
    <table style="text-align: left; border-collapse: collapse; font-size: 14px; width: 100%;">
        <tbody>
            <tr>
                <td style="width: 150px;">Category</td>
                <td>
                    <select id="lstVar_Category" style="width: 203px; margin-top: 5px" />
                </td>
            </tr>
            <tr>
                <td style="width: 150px;">Variety Name</td>
                <td>
                    <input type="text" id="txtVar_Variety" style="width: 200px; text-transform: uppercase;" maxlength="20">
                </td>
            </tr>
            <tr>
                <td style="width: 150px;">Short Code</td>
                <td>
                    <input type="text" id="txtVar_ShortCode" style="width: 25px; text-transform: uppercase;" maxlength="2">
                </td>
            </tr>
 <%--           <tr>
                <td style="width: 150px;">Small Image File</td>
                <td valign='top'>
                    <input type="file" id="VarietyImg" style="display: none;" />
                    <button id="btnVarietyImg">No file found</button>
                </td>
            </tr>
            <tr>
                <td style="width: 150px;">Large Image File</td>
                <td valign='top'>
                    <input type="file" id="largeImg" style="display: none;" />
                    <button id="btnlargeImg">No file found</button>
                </td>
            </tr>--%>
        </tbody>
    </table>
</div>

<div id="divVarietiesID" style="display: none;"></div>

<script type="text/javascript">

    $(document).ready(new function () {
        
        LoadCategory();

        $("#fgrdVarieties").flexigrid({
            url: 'BloomService.asmx/GetVarietiesForFgrd',
            dataType: 'xml',
            colModel: [
                { display: '', name: '', width: 20, sortable: true, align: 'Center' },
                { display: '', name: '', width: 20, sortable: true, align: 'Center' },
                { display: 'Category', name: 'Category', width: 50, sortable: true, align: 'left' },
                { display: 'Variety', name: 'Variety', width: 200, sortable: true, align: 'left' },
                { display: 'Short Code', name: 'ShortCode', width: 50, sortable: true, align: 'left' },
                //{ display: 'Small Image', name: 'Photo', width: 115, sortable: true, align: 'Center' },
                //{ display: 'Large Image', name: 'PhotoLarge', width: 115, sortable: true, align: 'Center' }

            ],
            searchitems: [
                { display: 'Variety', name: 'Variety' },
                { display: 'Category', name: 'Category' }
            ],
            buttons: [
                { name: 'Add', bclass: 'add', onpress: AddNewVarieties },
            ],
            showTableToggleBtn: true,
            sortname: "Category,Variety",
            sortorder: "asc",
            usepager: true,
            title: false,
            useRp: true,
            qtype: "",
            nomsg: 'No records found!',
            singleSelect: true,
            showToggleBtn: false,
            resizable: false,
            autoload: true,
            showTableToggleBtn: false,
            height: $(window).height() - 300,
            width: 600,
            striped: true,
            params: [
                { name: 'ExportCSV', value: '' }
            ],
            onSuccess: function () {
                
                var sel = jQuery("#fgrdVarieties tbody tr");
                var len = sel.length;
                if (len > 1) {
                    $("#DivVarietiesForFgrd #ExportCSV").show();
                }

                var i = 0;
                var t = document.getElementById('fgrdVarieties');

                $("#fgrdVarieties tr").each(function () {

                    photoWidth = textWidth($(t.rows[i].cells[5]).text());
                    photoLargeWidth = textWidth($(t.rows[i].cells[6]).text());

                    if (photoWidth > 115) {
                        $(t.rows[i].cells[5]).prop('title', $(t.rows[i].cells[5]).text());
                    }

                    if (photoLargeWidth > 115) {
                        $(t.rows[i].cells[6]).prop('title', $(t.rows[i].cells[6]).text());
                    }

                   
                    i++;
                });
                
                 if ($("#divVarietiesID").html() != "0" && $("#fgrdVarieties #row" + $("#divColorsID").html()).css('display') == "table-row") {
                    var id = $("#divVarietiesID").html();
                    $("#fgrdVarieties [id^=row]").removeClass("trSelectedHeader");
                    $("#fgrdVarieties td.unsorted").addClass("sorted");
                    $("#fgrdVarieties td.sorted").removeClass("unsorted");
                    $("#fgrdVarieties #row" + id).addClass("trSelectedHeader");
                    $("#fgrdVarieties #row" + id).removeClass("trSelected");
                    $("#fgrdVarieties #row" + id + " td.sorted").addClass("unsorted");
                    $("#fgrdVarieties #row" + id + " td.unsorted").removeClass("sorted");


                }
                else {
                    $("#divVarietiesID").html($("#divVarietiesID tr:first td:nth-child(3) div").text());
                    $("#fgrdVarieties tr:first").addClass("trSelectedHeader trSelected");
                }
            },
        });
    });
    function textWidth(text) {
        var calc = '<span style="display:none">' + text + '</span>';
        $('body').append(calc);
        var width = $('body').find('span:last').width();
        $('body').find('span:last').remove();
        return width;
    };

    function LoadCategory() {
        $.ajax({
            type: "POST",
            url: 'BloomService.asmx/LoadTypes',
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (output, status) {
                var data = output.d;
                $('#lstVar_Category').empty();
                for (var i in data) {
                    var cat = data[i];
                    $('#lstVar_Category').append('<option value="' + cat.TYPE + '">' + "(" + cat.TYPE + ") " + cat.NAME + '</option>');
                }
            }
        });

    }

    $("#EditVarieties").dialog({
        autoOpen: false,
        resizable: false,
        modal: true,
        width: 400,
        height: 240,
        title: 'Editing',
        buttons: {
            Save: function () {
                
                var VarietiesID = $("#divVarietiesID").html();
                var Category = $("#lstVar_Category").val() == "" ? "" : $("#lstVar_Category").val().toUpperCase();
                if (Category == "") {
                    $("#EditVarieties").next('div').find(".msgui").html("Please select category..");
                    $("#EditVarieties").next('div').find('.msgui').show();
                    ctrlFocus($("#lstVar_Category"));
                    return false;
                }
                var Variety = $("#txtVar_Variety").val() == "" ? $("#txtVar_Variety").attr('placeholder').toUpperCase() : $("#txtVar_Variety").val().toUpperCase();
                if (Variety == "") {
                    $("#EditVarieties").next('div').find(".msgui").html("Please enter variety..");
                    $("#EditVarieties").next('div').find('.msgui').show();
                    ctrlFocus($("#txtVar_Variety"));
                    return false;
                }

                var ShortCode = $("#txtVar_ShortCode").val() == "" ? $("#txtVar_ShortCode").attr('placeholder').toUpperCase() : $("#txtVar_ShortCode").val().toUpperCase();
                var Mode = VarietiesID == "" || VarietiesID == "0" ? "1" : "2";
                var fileBase64Str = "", fileName = "", LargefileBase64Str = "", LargefileName = "";

                //if (document.getElementById("varietyImg") && !!document.getElementById("varietyImg").title) {
                //    fileName = document.getElementById("varietyImg").title;
                //    //fileBase64Str = document.getElementById("varietyImg").src;
                //}
                //if (document.getElementById("varietyImg") == null) { // Added by Anubhuti 2023_10_08
                //    $("#EditVarieties").next('div').find(".msgui").html("Please Upload small Image..");
                //    $("#EditVarieties").next('div').find('.msgui').show();
                //    ctrlFocus($("#varietyImg"));
                //    return false;
                //}
               // if (document.getElementById("largeImg") && !!document.getElementById("largeImg").title) {
               //     LargefileName = document.getElementById("largeImg").title;
               //     //LargefileBase64Str = document.getElementById("testing").src;
               //}
                 
                //if ($('#VarietyImg').val() != "") {
                //    fileName = $('#VarietyImg')[0].files[0].name;
                //    fileBase64Str = document.getElementById("varietyImg").src;
                //}
                //if ($('#largeImg').val() != "") {
                //    LargefileName = $('#largeImg')[0].files[0].name;
                //    LargefileBase64Str = document.getElementById("LargeImg").src; 
                //}
               

                $.ajax({
                    type: "POST",
                    url: 'BloomService.asmx/AddEditDeleteSelectVarieties',
                    data: '{"ID":"' + VarietiesID + '","Category":"' + Category + '","Variety":"' + Variety + '","ShortCode":"' + ShortCode + '","Mode":"' + Mode + '","fileBase64Str":"' + fileBase64Str + '","fileName":"' + fileName + '","LargefileBase64Str":"' + LargefileBase64Str + '","LargefileName":"' + LargefileName + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (output) {
                        if (output.d != null) {
                            if (output.d.toLowerCase().indexOf("unique key") >= 0) {
                                $("#EditVarieties").next('div').find(".msgui").html("Colorde (<b>" + ColorCode.toUpperCase() + "</b>) Already exists!");
                                $("#EditVarieties").next('div').find('.msgui').show();
                            }
                            else {
                                $("#EditVarieties").dialog("close")
                                $("#fgrdVarieties").flexReload({ url: '/url/for/refresh' });
                            }
                        }
                        else {
                            $("#EditVarieties").next('div').find(".msgui").html(output.d);
                            $("#EditVarieties").next('div').find('.msgui').show();
                        }
                    }
                });
            },
            Cancel: function () {
                $(this).dialog("close");
            },
        },
        open: function () {
            debugger

            $("#EditVarieties").next('div').find('.msgui').remove();
            $("#EditVarieties").next('div').append("<div class='msgui' style='margin-left: 10px;height: 17px;'></div>").css('height', '35px');
            $("#EditVarieties").next('div').find(".msgui").css("width", $("#EditVarieties").width() - $('#EditVarieties').next('div').find(".ui-dialog-buttonset").width());
            $("#EditVarieties").next('div').find('.msgui').hide();

            $(".ui-dialog-buttonpane button:contains('Save')").addClass('dialogbuttonstyle icon-save')
            $(".ui-dialog-buttonpane button:contains('Cancel')").addClass('dialogbuttonstyle icon-cancel')
        }
    });
    function getBase64(file, fname) {
        //return new Promise((resolve, reject) => {
        //    const reader = new FileReader();
        //    reader.readAsDataURL(file);
        //    reader.onload = () => resolve(reader.result, fname);
        //    reader.onerror = error => reject(error);
        //});

        var fileReader = new FileReader();
        fileReader.readAsDataURL(file);
        fileReader.onload = (function (e) {
            
            return e.target.result;
        });

    }
    function AddNewVarieties() {
        
        $("#divVarietiesID").html("0");
        $("#txtVar_Variety").val("");
        $("#txtVar_Variety").attr('placeholder', "");
        $("#txtVar_ShortCode").val("");
        $("#txtVar_ShortCode").attr('placeholder', "");
        $(".pip").remove();  // Modified by Anubhuti 2023_10_09
        $(".largepip").remove();
        $("#btnVarietyImg").show();
        $("#EditVarieties").dialog("option", "title", "Varieties Maintenance");
        $("#EditVarieties").dialog("open");
    }

    $("[id^=EditVarieties_]").die('click').live("click", function (e) {
        
        var sourceCtl = $(this);
        var ID = sourceCtl.attr('id');
        var VarietiesID = ID.replace("EditVarieties_", "");
        $.ajax({
            type: "POST",
            url: 'BloomService.asmx/GetVarietiesByID',
            data: '{"ID":"' + VarietiesID + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (output) {
                
                if (output.d != null) {
                    var VarietiesDet = output.d;
                    $("#divVarietiesID").html(VarietiesDet.ID);
                    $("#lstVar_Category").val(VarietiesDet.Category);
                    $("#txtVar_Variety").val( VarietiesDet.Variety); // Modified by Anubhuti 2023_04_26
                    //$("#txtVar_Variety").attr('placeholder', VarietiesDet.Variety); 
                    $("#txtVar_ShortCode").val("");
                    $("#txtVar_ShortCode").attr('placeholder', VarietiesDet.Code);
                    $("#fgrdVarieties [id^=row]").removeClass("trSelectedHeader");
                    $("#fgrdVarieties #row" + VarietiesID).addClass("trSelectedHeader");
                    $("#fgrdVarieties td.unsorted").addClass("sorted");
                    $("#fgrdVarieties td.sorted").removeClass("unsorted");
                    $("#fgrdVarieties #row" + VarietiesID).removeClass("trSelected");
                    $("#fgrdVarieties #row" + VarietiesID + " td.sorted").addClass("unsorted");
                    $("#fgrdVarieties #row" + VarietiesID + " td.unsorted").removeClass("sorted");
                    $("#VarietyImg").val("");
                    $("largeImg").val("");

                    $("#btnVarietyImg").show();
                    
                    if (document.getElementsByClassName("pip") != undefined && document.getElementsByClassName("pip")[0] != undefined) {
                        document.getElementsByClassName("pip")[0].parentElement.removeChild(document.getElementsByClassName("pip")[0])
                    }

                    $("#btnlargeImg").show();

                    if (document.getElementsByClassName("largepip") != undefined && document.getElementsByClassName("largepip")[0] != undefined) {
                        document.getElementsByClassName("largepip")[0].parentElement.removeChild(document.getElementsByClassName("largepip")[0])
                    }

                   <%-- var imagePathBase = '<%=ConfigurationManager.AppSettings("ProductImages").ToString() %>';
                    var imagePathBase = '/mistyimages/Products';--%>
                    var imagePathBase = '<%=ConfigurationManager.AppSettings("ProductImages").ToString() %>';
                    var largephotoPath = imagePathBase + "/Large/";
                    var smallphotoPath = imagePathBase + '/Small/';

                    //if (VarietiesDet.Photo != "" && (!!VarietiesDet.Photo)) {
                    //    $("VarietyImg").val(VarietiesDet.Photo);
                    //    $("#btnVarietyImg").hide();
                    //    $("<span class=\"pip\">" +
                    //        "<img id='varietyImg' style=\"height:50px; width:80px;\" class=\"imageThumb\" src=\"" + smallphotoPath + VarietiesDet.Photo + "\" onclick=ShowPreview_edit_SmallImg('" + VarietiesDet.Photo + "') title=\"" + VarietiesDet.Photo + "\"  />" +
                    //        "<br/><span class=\"remove\"><a class='fileuploader-action fileuploader-action-remove'><i class=''></i></a></span>" +
                    //        "</span>").insertAfter("#btnVarietyImg");
                    //    $(".remove").click(function () {
                    //        $(this).parent(".pip").remove();
                    //        $("#VarietyImg").val("");
                    //        $("#btnVarietyImg").show();

                    //    });
                    //}

                    //if (VarietiesDet.PhotoLarge != "" && (!!VarietiesDet.PhotoLarge)) {
                    //    $("largeImg").val(VarietiesDet.PhotoLarge);
                    //    $("#btnlargeImg").hide();
                    //    $("<span class=\"largepip\">" +
                    //        "<img id='LargeImg' style=\"height:50px; width:80px;\" class=\"imageThumb\" src=\"" + largephotoPath + VarietiesDet.PhotoLarge + "\" onclick=ShowPreview_edit_LargeImg('" + VarietiesDet.PhotoLarge + "') title=\"" + VarietiesDet.PhotoLarge + "\"  />" +
                    //        "<br/><span class=\"remove\"><a class='fileuploader-action fileuploader-action-remove'><i class=''></i></a></span>" +
                    //        "</span>").insertAfter("#btnlargeImg");
                    //    $(".remove").click(function () {
                    //        $(this).parent(".largepip").remove();
                    //        $("largeImg").val("");
                    //        $("#btnlargeImg").show();
                    //    });
                    //}
                    $("#EditVarieties").dialog('option', 'title', "Variety :: " + VarietiesDet.Variety);
                    $("#EditVarieties").dialog("open");
                }
            }
        });
    });

    $("[id^=DeleteVarieties_]").die("click").live("click", function () {
        
        var sourceCtl = $(this);
        var ID = sourceCtl.attr('id').replace('DeleteVarieties_', '');
        $('#OldVarietiesId').html(ID);
        var Variety = $(this).attr("data-variety");
        $("#DeleteConfirmVarieties").html("<p>Are you sure to delete<b> " + Variety + "</b> ?</p>");
        $("#DeleteConfirmVarieties").dialog("open");
    });

    $("#DeleteConfirmVarieties").dialog({
        autoOpen: false,
        resizable: false,
        title: "Alert",
        width: 400,
        modal: true,
        buttons: {
            Yes: function () {
                
                var adialog = $(this);
                var ID = $("#OldVarietiesId").html();
                $.ajax({
                    type: "POST",
                    url: 'BloomService.asmx/AddEditDeleteSelectVarieties',
                    data: '{"ID":"' + ID + '","Category":"","Variety":"","ShortCode":"","Mode":"3","fileBase64Str":"","fileName":"","LargefileBase64Str":"","LargefileName":""}', // Modified by Anubhuti 2023_04_25
                    /*data: '{"ID":"' + ID + '","Category":"","Variety":"","ShortCode":"","Mode":"3"}', */
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (output) {
                        
                        if (output.d != null) {
                            adialog.dialog("close");
                            $("#fgrdVarieties").flexReload({ url: '/url/for/refresh' });
                        }
                        else {
                            adialog.dialog("close");
                            $("#msg").html("There was an error during save the details!");
                            $("#msgbox-select").dialog("open");
                        }
                    }
                });
            },
            No: function () {
                $(this).dialog("close");
            }
        },
        open: function () {
            $(".ui-dialog-buttonpane button:contains('Yes')").addClass('dialogbuttonstyle icon-save')
            $(".ui-dialog-buttonpane button:contains('No')").addClass('dialogbuttonstyle icon-cancel')
        }
    });

    $('input').keypress(function (e) {
        var key = e.which;
        
        if (key == 13) // the enter key code
        {
            if ($(this).closest('tr').next().find('input')) {
                if ($(this).attr('id') == "txtVar_ShortCode") {
                    $("#EditVarieties").siblings('.ui-dialog-buttonpane').find("button:contains('Save')").focus();
                }
                else {
                    $(this).closest('tr').next().find('input').focus();
                }
                return false;
            }
        }
    });

    $("#lstVar_Category").keydown(function (e) {
        
        var keycodevar = e.which;
        if (keycodevar == 13) {
            $("#txtVar_Variety").focus();
            return false;
        }
    });

    $('#btnVarietyImg').on('click', function () {
        $("#btnUploadImages").show();
        $("#btnVarietyImg").show();
        $('#VarietyImg').trigger('click');
        /*        GetImageByProdCode('bouquetImg')*/
        $("#btnRecipeclick").removeClass("btntoleftWithoutImageAligment");
        $("#btnRecipeclick").addClass("btntoleft");

    });

    $('#btnlargeImg').on('click', function () {
        $("#btnUploadImages").show();
        $("#btnlargeImg").show();
        $('#largeImg').trigger('click');
        /*        GetImageByProdCode('bouquetImg')*/
        $("#btnRecipeclick").removeClass("btntoleftWithoutImageAligment");
        $("#btnRecipeclick").addClass("btntoleft");

    });

    if (window.File && window.FileList && window.FileReader) {
        $("#VarietyImg").on("change", function (e) {
            
            var files = e.target.files,
                filesLength = files.length;
            for (var i = 0; i < filesLength; i++) {
                var f = files[i]
                var fileReader = new FileReader();
                fileReader.onload = (function (e) {
                    var file = e.target;
                    
                    $("#btnVarietyImg").hide();
                    $("<span class=\"pip\">" +
                        "<img id='varietyImg' style=\"height:50px; width:80px;\" class=\"imageThumb\" src=\"" + e.target.result + "\" onclick=ShowPreviewSmallImg('" + f.name + "') title=\"" + f.name + "\"  />" +
                        "<br/><span class=\"remove\"><a class='fileuploader-action fileuploader-action-remove' ><i class=''></i></a></span>" +
                        "</span>").insertAfter("#VarietyImg");
                    $(".remove").click(function () {
                        $(this).parent(".pip").remove();
                        $("#VarietyImg").val("");
                        $("#btnVarietyImg").show();
                    });
                });
                fileReader.readAsDataURL(f);
            }
        });
    }

    if (window.File && window.FileList && window.FileReader) {
        $("#largeImg").on("change", function (e) {
            
            var files = e.target.files,
                filesLength = files.length;
            for (var i = 0; i < filesLength; i++) {
                var f = files[i]
                var fileReader = new FileReader();
                fileReader.onload = (function (e) {
                    var file = e.target;
                    

                $("#btnlargeImg").hide();
                $("<span class=\"largepip\">" +
                    "<img id='LargeImg' style=\"height:50px; width:80px;\" class=\"imageThumb\" src=\"" + e.target.result + "\" onclick=ShowPreviewLargeImg('" + f.name + "') title=\"" + f.name + "\"  />" +
                    "<br/><span class=\"remove\"><a class='fileuploader-action fileuploader-action-remove' ><i class=''></i></a></span>" +
                    "</span>").insertAfter("#largeImg");
                $(".remove").click(function () {
                    $(this).parent(".largepip").remove();
                    $("largeImg").val("");
                    $("#btnlargeImg").show();
                });   
                });
                fileReader.readAsDataURL(f);
            }
        });
    }
    function ShowPreviewSmallImg(imgName) {
        var newTab = window.open();
        newTab.document.body.innerHTML = '<img src="' + $("#varietyImg").attr("src") + '" width="100px" height="100px">';
    }
    function ShowPreview_edit_SmallImg(imgName) {
        window.open($("#varietyImg").attr("src"), '_blank');
    }

    function ShowPreviewLargeImg(imgName) {
        var newTab = window.open();
        newTab.document.body.innerHTML = '<img src="' + $("#LargeImg").attr("src") + '" width="100px" height="100px">';
    }
    function ShowPreview_edit_LargeImg(imgName) {
        window.open($("#LargeImg").attr("src"), '_blank');
    }


</script>
