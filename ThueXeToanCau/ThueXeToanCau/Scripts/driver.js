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
    //email: date_time total_moneys province code   car_made
    var driverObj = {
        id: 0, name: $("#tname").val(), pass: $("#tPass").val(),phone: $("#tphone").val(), car_model: $("#car_model").val(),
        card_identify: $("#car").val(), car_years: $("#car_year").val(), car_size: $("#car_size").val(), car_number: $("#car_number").val(),
        car_type: $("#car_type").val(), car_price: $("#car_price").val(), address: $("#address").val(), license: $("#tLicense").val()
    };
    $.ajax({
        url: url_addUpdateDriver, type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(driverObj),
        success: function (rs) {
            alert(rs);
        }
    })
}