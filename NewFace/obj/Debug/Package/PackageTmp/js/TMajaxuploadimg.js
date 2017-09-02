var totalimgCount = 5;//允许上传的总个数
$(function () {
    $(".upload_div a").live('click', function () {
        delimg(this);
    })
})
//上传图片
function ajaxImgFileUpload(obj_id) {
    var objid = $(obj_id).attr("id");
    $.ajaxFileUpload({
        url: '/Handler/ImgUploadHandler.ashx', //用于文件上传的服务器端请求地址
        secureuri: false,           //一般设置为false
        fileElementId: $(obj_id).attr("id"), //文件上传控件的id属性
        data: { method: "FilesSend", DirPath: "images" },
        dataType: 'json', //返回值类型 一般设置为json
        success: function (data, status)  //服务器成功响应处理函数
        {
            debugger;
            if (typeof (data.Result) != 'undefined') {
                if (data.Result == 'success') {
                    var url = data.Message;
                    RanderImgHtml(objid, url);

                } else {
                    alert(data.Message);
                }
            }
            //清除当前file值
            $("#" + objid).val("");
        }
    });
};

//删除图片
function delimg(obj) {
    var imgpath = $(obj).parent().find("img").attr("src");
    $.post("/handler/ImgUploadHandler.ashx", { method: "FilesDelete", imgpath: imgpath });
    var imglist = $(obj).parent().parent().find("img").length - 1;
    if (imglist < totalimgCount) {
        $(obj).parent().parent().find(".uploader_input_wrp").show();
    }
    $(obj).parent().remove();

}
//绑定加载图片预览
function BindImgList(imgstr, objid) {
    if (!objid)
    {
        objid = "uploader_input";
    }
    if (imgstr.length > 0) {
        var imglist = imgstr.split(',');
        $.each(imglist, function (index, url) {
            RanderImgHtml(objid,url);
        })
    }
}
//渲染html显示
function RanderImgHtml(objid,imgurl)
{
    var imglist = $("#" + objid).parent().parent().find("img").length;
    var imgtotal = totalimgCount - imglist;
    if (imgtotal > 0) {
        $("#" + objid).parent().before('<div class="upload_div"><img src="' + imgurl + '"><a href="javascript:void(0)" class="close_link">-</a></div>');
        imgtotal--;
        if (imgtotal == 0) {
            $("#" + objid).parent().hide();
        }
    } else {
        $("#" + objid).parent().hide();
    }
}