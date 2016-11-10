function openWhoType(id, name) {
    name = resetValue(name, "");
    
    $("#whoTypeId").val(id);
    $("#n_Name").val(name);
    $("#whoTypeDialog").show();
}

function saveWhoType() {
    $.ajax({
        url: url_addUpdateWhoType, type: 'post',
        contentType: 'application/json',
        data: JSON.stringify({ id: $("#whoTypeId").val(), name: $("#n_Name").val() }),
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}

function confirmDelWhoType(id, name) {
    $("#whoTypeId").val(id);
    openNotification("Bạn có chắc chắn xóa " + name + " ?", "delWhoType");
}

function deleteWhoType() {
    $.ajax({
        url: url_deleteWhoType, type: 'post',
        data: { id: $("#whoTypeId").val() },
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}