<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PagePictures.ascx.vb" Inherits="pages_PagePictures" %>

<style>
    .add {
        background: url(images/add_icon.gif) no-repeat 10px 0px;
        text-indent: 30px;
        display: block;
        font-size: 1.2em;
        height: 16px;
    }
    F
    .trSelectedHeader {
        background-color: red !important;
    }

    /**/
    .largepip {
        display: inline-block;
        margin: 10px 10px 0 0;
    }
    /*#EditPictures input[type="text"] {
        height: 17px;
        background-color:cornsilk;
    }
    #EditPictures select {
        background-color:cornsilk;
    }*/
</style>


<table>
    <tr>
        <td>

            <div id="DivPicturesForFgrd">
                <table id="fgrdPictures" style="display: none;"></table>
                <div id="DeleteConfirmPictures" style="display: none;" title="Alert"></div>
                <div id="OldPicturesId" style="display: none;"></div>
            </div>
        </td>
    </tr>
</table>

<div id="EditPictures" style="display: none; height: 150px !important;" class="filesTab">
    <table style="text-align: left; border-collapse: collapse; font-size: 14px; width: 100%;">
        <tbody>
            <tr>
                <td style="width: 150px;">Category</td>
                <td>
                    <select id="lstVar_Category" style="width: 200px; margin-top: 5px" />
                </td>
            </tr>
            <tr>
                <td style="width: 150px;">Variety Name</td>
                <td>
                    <select id="lstVar_Variety" style="width: 200px; margin-top: 5px" />
                </td>
            </tr>
            <tr>
                <td style="width: 150px;">Color</td>
                <td>
                    <select id="txtVar_Color" style="width: 200px; margin-top: 5px"></select>    
                   <%-- <input type="text" id="txtVar_Color" style="width: 25px; text-transform: uppercase;" maxlength="20">--%>
                </td>
            </tr>
            <tr>
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
            </tr>
        </tbody>
    </table>
</div>

<div id="divPicturesID" style="display: none;"></div>

<script type="text/javascript">

    $(document).ready(new function () {
        
        LoadCategory();
        LoadColorsForPictures();
        
        $("#fgrdPictures").flexigrid({
            url: 'BloomService.asmx/GetPicturesForFgrd',
            dataType: 'xml',
            colModel: [
                { display: '', name: '', width: 20, sortable: true, align: 'Center' },
                { display: '', name: '', width: 20, sortable: true, align: 'Center' },
                { display: 'Category', name: 'Category', width: 50, sortable: true, align: 'left' },
                { display: 'Variety', name: 'Variety', width: 200, sortable: true, align: 'left' },
                { display: 'Color', name: 'Color', width: 200, sortable: true, align: 'left' },
                { display: 'Small Image', name: 'Photo', width: 115, sortable: true, align: 'Center' },
                { display: 'Large Image', name: 'PhotoLarge', width: 115, sortable: true, align: 'Center' }

            ],
            searchitems: [
                { display: 'Variety', name: 'Variety' },
                { display: 'Category', name: 'Category' },
                { display: 'Color', name: 'Color' }
            ],
            buttons: [
                { name: 'Add', bclass: 'add', onpress: AddNewPictures },
            ],
            showTableToggleBtn: true,
            sortname: "Category,Variety,Color",
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
            width: 875,
            striped: true,
            params: [
                { name: 'ExportCSV', value: '' }
            ],
            onSuccess: function () {
                
                var sel = jQuery("#fgrdPictures tbody tr");
                var len = sel.length;
                if (len > 1) {
                    $("#DivPicturesForFgrd #ExportCSV").show();
                }

                var i = 0;
                var t = document.getElementById('fgrdPictures');

                $("#fgrdPictures tr").each(function () {

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
                
                 if ($("#divPicturesID").html() != "0" && $("#fgrdPictures #row" + $("#divColorsID").html()).css('display') == "table-row") {
                    var id = $("#divPicturesID").html();
                    $("#fgrdPictures [id^=row]").removeClass("trSelectedHeader");
                    $("#fgrdPictures td.unsorted").addClass("sorted");
                    $("#fgrdPictures td.sorted").removeClass("unsorted");
                    $("#fgrdPictures #row" + id).addClass("trSelectedHeader");
                    $("#fgrdPictures #row" + id).removeClass("trSelected");
                    $("#fgrdPictures #row" + id + " td.sorted").addClass("unsorted");
                    $("#fgrdPictures #row" + id + " td.unsorted").removeClass("sorted");


                }
                else {
                    $("#divPicturesID").html($("#divPicturesID tr:first td:nth-child(3) div").text());
                    $("#fgrdPictures tr:first").addClass("trSelectedHeader trSelected");
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
    function LoadVarietyFilterDropdown(CAT,Variety) {
        $.ajax({
            type: "POST",
            url: 'BloomService.asmx/LoadVariety',
            data: '{"Category":"' + CAT + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (output, status) {
                $('#lstVar_Variety').empty();
                var data = output.d;
                if (data != null) {
                    $('#lstVar_Variety').append('<option value="0">Select</option>');
                    for (var i in data) {
                        var Value = data[i];
                        $('#lstVar_Variety').append('<option value="' + Value.Variety + '">' + Value.Variety + '</option>');
                    }
                    if (Variety && Variety != '') {
                        $('#lstVar_Variety').val(Variety)
                    }
                }
                   
            },
        });
    }
        //Added by Anubhuti 2023_06_06
     function LoadColorsForPictures() {
        $.ajax({
            type: "POST",
            url: 'BloomService.asmx/LoadColors',
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (output, status) {
                $('#txtVar_Color').empty();
                var TYPEdata = output.d;
                $('#txtVar_Color').append('<option value="0">Select</option>');
                for (var i in TYPEdata) {
                    var Code = TYPEdata[i];
                    $('#txtVar_Color').append('<option value="' + Code.ColorCode + '">' + Code.ColorCode + '</option>');
                }
            }
        })
    }

    $("#EditPictures").dialog({
        autoOpen: false,
        resizable: false,
        modal: true,
        width: 400,
        height: 340,
        title: 'Editing',
        buttons: {
            Save: function () {
                
                var PicturesID = $("#divPicturesID").html();
                var Category = $("#lstVar_Category").val() == "" ? "" : $("#lstVar_Category").val().toUpperCase();
                if (Category == "") {
                    $("#EditPictures").next('div').find(".msgui").html("Please select category..");
                    $("#EditPictures").next('div').find('.msgui').show();
                    ctrlFocus($("#lstVar_Category"));
                    return false;
                }

                var Variety = $("#lstVar_Variety").val() == "" ? "" : $("#lstVar_Variety").val().toUpperCase();
                if (Variety == "") {
                    $("#EditPictures").next('div').find(".msgui").html("Please select variety..");
                    $("#EditPictures").next('div').find('.msgui').show();
                    ctrlFocus($("#lstVar_Variety"));
                    return false;
                }

                //var ShortCode = $("#txtVar_ShortCode").val() == "" ? $("#txtVar_ShortCode").attr('placeholder').toUpperCase() : $("#txtVar_ShortCode").val().toUpperCase();
                // added by Jose on 6/4/2023
                var Color = $("#txtVar_Color").val();
                if (Color == "") {
                    $("#EditPictures").next('div').find(".msgui").html("Please select color..");
                    $("#EditPictures").next('div').find('.msgui').show();
                    ctrlFocus($("#txtVar_Color"));
                    return false;
                }

                var Mode = PicturesID == "" || PicturesID == "0" ? "1" : "2";
                var fileBase64Str = "", fileName = "", LargefileBase64Str = "", LargefileName = "";

                if (document.getElementById("varietyImg") && !!document.getElementById("varietyImg").title) {
                    fileName = document.getElementById("varietyImg").title;
                    //fileBase64Str = document.getElementById("varietyImg").src;
                }

                if (document.getElementById("largeImg") && !!document.getElementById("largeImg").title) {
                    LargefileName = document.getElementById("largeImg").title;
                    //LargefileBase64Str = document.getElementById("testing").src;
               }
                 
                if ($('#VarietyImg').val() != "") {
                    fileName = $('#VarietyImg')[0].files[0].name;
                    fileBase64Str = document.getElementById("varietyImg").src;
                }
                if ($('#largeImg').val() != "") {
                    LargefileName = $('#largeImg')[0].files[0].name;
                    LargefileBase64Str = document.getElementById("LargeImg").src; 
                }
               
                    //Modified by Anubhuti 2023_06_06
                $.ajax({
                    type: "POST",
                    url: 'BloomService.asmx/AddEditDeleteSelectPictures',
                    data: '{"ID":"' + PicturesID + '","Category":"' + Category + '","Variety":"' + Variety + '","Color":"' + Color + '","Mode":"' + Mode + '","fileBase64Str":"' + fileBase64Str + '","fileName":"' + fileName + '","LargefileBase64Str":"' + LargefileBase64Str + '","LargefileName":"' + LargefileName + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (output) {
                        if (output.d != null) {
                            if (output.d.toLowerCase().indexOf("unique key") >= 0) {
                                $("#EditPictures").next('div').find(".msgui").html("Colorde (<b>" + ColorCode.toUpperCase() + "</b>) Already exists!");
                                $("#EditPictures").next('div').find('.msgui').show();
                            }
                            else {
                                $("#EditPictures").dialog("close")
                                $("#fgrdPictures").flexReload({ url: '/url/for/refresh' });
                            }
                        }
                        else {
                            $("#EditPictures").next('div').find(".msgui").html(output.d);
                            $("#EditPictures").next('div').find('.msgui').show();
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

            $("#EditPictures").next('div').find('.msgui').remove();
            $("#EditPictures").next('div').append("<div class='msgui' style='margin-left: 10px;height: 17px;'></div>").css('height', '35px');
            $("#EditPictures").next('div').find(".msgui").css("width", $("#EditPictures").width() - $('#EditPictures').next('div').find(".ui-dialog-buttonset").width());
            $("#EditPictures").next('div').find('.msgui').hide();

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
    //Modified by Anubhuti 2023_06_06
    function AddNewPictures() {
        
        $("#divPicturesID").html("0");
        $("#txtVar_Color").val("");
        $('span.pip').remove();
        $('span.largepip').remove();
        $("#btnVarietyImg").show();
        $("#btnlargeImg").show();
        $("#lstVar_Category").val($("#lstVar_Category option:first").val());
        $("#lstVar_Variety").empty();
        $("#EditPictures").dialog("option", "title", "Pictures Maintenance");
        $("#EditPictures").dialog("open");
    }

    $("[id^=EditPictures_]").die('click').live("click", function (e) {
        
        var sourceCtl = $(this);
        var ID = sourceCtl.attr('id');
        var PicturesID = ID.replace("EditPictures_", "");
        $.ajax({
            type: "POST",
            url: 'BloomService.asmx/GetPicturesByID',
            data: '{"ID":"' + PicturesID + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (output) {
                
                if (output.d != null) {
                    var PicturesDet = output.d;
                    $("#divPicturesID").html(PicturesDet.ID);
                    $("#lstVar_Category").val(PicturesDet.Category);
                    LoadVarietyFilterDropdown(PicturesDet.Category, PicturesDet.Variety);
                    //$("#txtVar_Variety").attr('placeholder', PicturesDet.Variety); 
                    $("#txtVar_Color").val(PicturesDet.Color);     //Modified by Anubhuti 2023_06_06
                    $("#fgrdPictures [id^=row]").removeClass("trSelectedHeader");
                    $("#fgrdPictures #row" + PicturesID).addClass("trSelectedHeader");
                    $("#fgrdPictures td.unsorted").addClass("sorted");
                    $("#fgrdPictures td.sorted").removeClass("unsorted");
                    $("#fgrdPictures #row" + PicturesID).removeClass("trSelected");
                    $("#fgrdPictures #row" + PicturesID + " td.sorted").addClass("unsorted");
                    $("#fgrdPictures #row" + PicturesID + " td.unsorted").removeClass("sorted");
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

                    if (PicturesDet.Photo != "" && (!!PicturesDet.Photo)) {
                        $("VarietyImg").val(PicturesDet.Photo);
                        $("#btnVarietyImg").hide();
                        $("<span class=\"pip\">" +
                            "<img id='varietyImg' style=\"height:50px; width:80px;\" class=\"imageThumb\" src=\"" + smallphotoPath + PicturesDet.Photo + "\" onclick=ShowPreview_edit_SmallImg('" + PicturesDet.Photo + "') title=\"" + PicturesDet.Photo + "\"  />" +
                            "<br/><span class=\"remove\"><a class='fileuploader-action fileuploader-action-remove'><i class=''></i></a></span>" +
                            "</span>").insertAfter("#btnVarietyImg");
                        $(".remove").click(function () {
                            $(this).parent(".pip").remove();
                            $("#VarietyImg").val("");
                            $("#btnVarietyImg").show();

                        });
                    }

                    if (PicturesDet.PhotoLarge != "" && (!!PicturesDet.PhotoLarge)) {
                        $("largeImg").val(PicturesDet.PhotoLarge);
                        $("#btnlargeImg").hide();
                        $("<span class=\"largepip\">" +
                            "<img id='LargeImg' style=\"height:50px; width:80px;\" class=\"imageThumb\" src=\"" + largephotoPath + PicturesDet.PhotoLarge + "\" onclick=ShowPreview_edit_LargeImg('" + PicturesDet.PhotoLarge + "') title=\"" + PicturesDet.PhotoLarge + "\"  />" +
                            "<br/><span class=\"remove\"><a class='fileuploader-action fileuploader-action-remove'><i class=''></i></a></span>" +
                            "</span>").insertAfter("#btnlargeImg");
                        $(".remove").click(function () {
                            $(this).parent(".largepip").remove();
                            $("largeImg").val("");
                            $("#btnlargeImg").show();
                        });
                    }
                    $("#EditPictures").dialog('option', 'title', "Variety :: " + PicturesDet.Variety);
                    $("#EditPictures").dialog("open");
                }
            }
        });
    });

    $("[id^=DeletePictures_]").die("click").live("click", function () {
        
        var sourceCtl = $(this);
        var ID = sourceCtl.attr('id').replace('DeletePictures_', '');
        $('#OldPicturesId').html(ID);
        var Variety = $(this).attr("data-variety");
        $("#DeleteConfirmPictures").html("<p>Are you sure to delete<b> " + Variety + "</b> ?</p>");
        $("#DeleteConfirmPictures").dialog("open");
    });

    $("#DeleteConfirmPictures").dialog({
        autoOpen: false,
        resizable: false,
        title: "Alert",
        width: 400,
        modal: true,
        buttons: {
            Yes: function () {
                
                var adialog = $(this);
                var ID = $("#OldPicturesId").html();
                $.ajax({
                    type: "POST",
                    url: 'BloomService.asmx/AddEditDeleteSelectPictures',
                    data: '{"ID":"' + ID + '","Category":"","Variety":"","Color":"","Mode":"3","fileBase64Str":"","fileName":"","LargefileBase64Str":"","LargefileName":""}', // Modified by Anubhuti 2023_04_25
                    /*data: '{"ID":"' + ID + '","Category":"","Variety":"","ShortCode":"","Mode":"3"}', */
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (output) {
                        
                        if (output.d != null) {
                            adialog.dialog("close");
                            $("#fgrdPictures").flexReload({ url: '/url/for/refresh' });
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
                    $("#EditPictures").siblings('.ui-dialog-buttonpane').find("button:contains('Save')").focus();
                }
                else {
                    $(this).closest('tr').next().find('input').focus();
                }
                return false;
            }
        }
    });

    $("#lstVar_Category").change(function (e) {
        LoadVarietyFilterDropdown($("#lstVar_Category").val());
    });

    $("#lstVar_Category").keydown(function (e) {
        
        var keycodevar = e.which;
        if (keycodevar == 13) {
            $("#lstVar_Variety").focus();
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
