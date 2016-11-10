function openCarType(id, name) {
    name = resetValue(name, "");
    
    $("#carTypeId").val(id);
    $("#n_Name").val(name);
    $("#carTypeDialog").show();
}

function saveCarType() {
    $.ajax({
        url: url_addUpdateCarType, type: 'post',
        contentType: 'application/json',
        data: JSON.stringify({ id: $("#carTypeId").val(), name: $("#n_Name").val() }),
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}

function confirmDelCarType(id, name) {
    $("#carTypeId").val(id);
    openNotification("Bạn có chắc chắn xóa " + name + " ?", "delCarType");
}

function deleteCarType() {
    $.ajax({
        url: url_deleteCarType, type: 'post',
        data: { id: $("#carTypeId").val() },
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}