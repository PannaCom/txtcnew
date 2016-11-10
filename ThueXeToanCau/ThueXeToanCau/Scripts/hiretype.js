function openHireType(id, name) {
    name = resetValue(name, "");
    
    $("#hireTypeId").val(id);
    $("#n_Name").val(name);
    $("#hireTypeDialog").show();
}

function saveHireType() {
    $.ajax({
        url: url_addUpdateHireType, type: 'post',
        contentType: 'application/json',
        data: JSON.stringify({ id: $("#hireTypeId").val(), name: $("#n_Name").val() }),
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}

function confirmDelHireType(id, name) {
    $("#hireTypeId").val(id);
    openNotification("Bạn có chắc chắn xóa " + name + " ?", "delHireType");
}

function deleteHireType() {
    $.ajax({
        url: url_deleteHireType, type: 'post',
        data: { id: $("#hireTypeId").val() },
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}