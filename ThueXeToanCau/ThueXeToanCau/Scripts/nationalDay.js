$(function () {
    var s = new Date().toLocaleString();
    $('#dayStart').datepicker({ value: s });
    $('#dayEnd').datepicker({ value: s });

});

function openNationalDay(id, name, sDate, eDate) {
    name = resetValue(name, "");    

    $("#dayId").val(id);
    $("#dayName").val(name);
    if (sDate == undefined) {
        $("#dayStart").val('');
    } else {
        $("#dayStart").val(formatDate(sDate));
    }
    if (eDate == undefined) {
        $("#dayEnd").val('');
    } else {
        $("#dayEnd").val(formatDate(eDate));
    }
    $("#nationalDialog").show();
}

function saveNationalDay() {
    $.ajax({
        url: url_addUpdateNationalDay, type: 'post',
        contentType: 'application/json',
        data: JSON.stringify({
            ID: $("#dayId").val(), DayName: $("#dayName").val(),
            StartDate: $('#dayStart').val(), EndDate: $('#dayEnd').val()
        }),
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}

function confirmDelNationalDay(id, name) {
    $("#dayId").val(id);
    openNotification("Bạn có chắc chắn xóa " + name + " ?", "delNationalDay");
}

function deleteNationalDay() {
    $.ajax({
        url: url_deleteNationalDay, type: 'post',
        data: { id: $("#dayId").val() },
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}