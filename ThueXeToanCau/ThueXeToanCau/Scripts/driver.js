$(function () {    
    $("#address").geocomplete();    
    //var options = {
    //    map: ".map_canvas"
    //};
    //$("#address").geocomplete(options)
    //  .bind("geocode:result", function (event, result) {
    //      //$("#lon").val(result.geometry.location.lng());
    //      //$("#lat").val(result.geometry.location.lat());
    //  })
    //  .bind("geocode:error", function (event, status) {
    //      $.log("ERROR: " + status);
    //  })
    //  .bind("geocode:multiple", function (event, results) {
    //      $.log("Multiple: " + results.length + " results found");
    //  });
});

function autosearchmodel() {
    var urlSearch = '/Api/getCarModelList?keyword=';
    $('#car_model').autocomplete({
        source: urlSearch + $("#car_model").val(),
        select: function (event, ui) {
            $(event.target).val(ui.item.value);
            return false;
        },
        minLength: 1
    });
}

function registerDriver() {
    var isValid = true;
    $(".divRegister input").each(function (idx, ip) {
        if ($(ip).val().trim() == "") {
            $(ip).css("border", "1px solid red");
            isValid = false;
        } else {
            $(ip).css("border", "1px solid #ccc");
        }
    });    
    if (!isValid) return false;
    if ($("#tPass").val() != $("#tPassConfirm").val()) {
        $("#tPass").css("border", "1px solid red");
        $("#tPass").val('');
        $("#tPassConfirm").css("border", "1px solid red");
        $("#tPassConfirm").val('');
        isValid = false;
    }
    if (!isValid) return false;
    var driverObj = {
        id: 0, name: $("#tname").val(), pass: $("#tPass").val(),phone: $("#tphone").val(), car_model: $("#car_model").val(),
        card_identify: $("#tCMND").val(), car_years: $("#car_year").val(), car_size: $("#car_size").val(), car_number: $("#car_number").val(),
        car_type: $("#car_type").val(), address: $("#address").val(), license: $("#tLicense").val(), car_made: $("#car").val()
    };
    $.ajax({
        url: url_addUpdateDriver, type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(driverObj),
        success: function (rs) {
            if (rs = '') rs = 'Đăng ký thành công';
            alert(rs);
        }
    })
}