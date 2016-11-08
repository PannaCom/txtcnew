function openNotice(nId, name) {
    name = resetValue(name, "");
    
    $("#noticeId").val(nId);
    $("#n_Name").val(name);
    $("#noticeDialog").show();
}

function saveNotice() {
    $.ajax({
        url: url_addUpdateNotice, type: 'post',
        contentType: 'application/json',
        data: JSON.stringify({ id: $("#noticeId").val(), name: $("#n_Name").val() }),
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}

function confirmDelNotice(nId, name) {
    $("#noticeId").val(nId);
    openNotification("Bạn có chắc chắn xóa " + name + " ?", "delNotice");
}

function deleteNotice() {
    $.ajax({
        url: url_deleteNotice, type: 'post',
        data: { nId: $("#noticeId").val() },
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}