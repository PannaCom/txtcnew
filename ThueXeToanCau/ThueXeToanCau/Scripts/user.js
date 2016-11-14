$(function () {
    $("#u_Name_err").hide();
});

var isExistInfo = false;
function validateExistInfo(uName) {
    $.ajax({
        url: url_validateExistInfo, type: 'get',
        data: { name: uName },
        success: function (result) {
            if (result != '') {
                isExistInfo = true;
                $("#u_Name").css("border", "1px solid red");
                $("#u_Name_err").show();
            } else {
                isExistInfo = false;
                $("#u_Name").css("border", "1px solid #ccc");
                $("#u_Name_err").hide();
            }
        }
    });
}

function openUser(uId, name) {
    name = resetValue(name, "");
    
    $("#userId").val(uId);
    $("#u_Name").val(name);
    $("#userDialog").show();
}

function saveUser() {
    var uName = $("#u_Name").val().trim();
    if (uName == '') {
        $("#u_Name").css("border", "1px solid red");
        return false;
    }
    $("#u_Name").css("border", "1px solid #ccc");
    
    var uId = $("#userId").val(), validPass = true;
    if (uId == 0) {
        if ($("#u_Pasword").val() == '' || $("#u_Pasword").val() != $("#u_PaswordConfirm").val()) {
            validPass = false;
        }
    } else if ($("#u_Pasword").val() != $("#u_PaswordConfirm").val()) {
        validPass = false;
    }
    if(!validPass) {
        $("#u_Pasword").css("border", "1px solid red");
        $("#u_Pasword").val('');
        $("#u_PaswordConfirm").css("border", "1px solid red");
        $("#u_PaswordConfirm").val('');
        return false;
    }
    if (isExistInfo) return false;

    $.ajax({
        url: url_addUpdateUser, type: 'post',
        contentType: 'application/json',
        data: JSON.stringify({ id: uId, name: uName, pass: $("#u_Pasword").val() }),
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}

function confirmDelUser(uId, name) {
    $("#userId").val(uId);
    openNotification("Bạn có chắc chắn xóa " + name + " ?", "delUser");
}

function deleteUser() {
    $.ajax({
        url: url_deleteUser, type: 'post',
        data: { uId: $("#userId").val() },
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}