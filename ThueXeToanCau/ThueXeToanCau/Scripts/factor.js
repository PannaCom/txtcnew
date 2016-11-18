function openFactor(id, name, val) {
    $("#factorId").val(id);
    $("#fName").text(name);
    $("#fVal").val(val);
    $("#factorDialog").show();
}

function saveFactor() {
    $.ajax({
        url: url_addUpdateFactor, type: 'post',
        contentType: 'application/json',
        data: JSON.stringify({ id: $("#factorId").val(), name: $("#fName").text(), val: $("#fVal").val() }),
        success: function (rs) {
            $("#factorDialog").hide();
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}