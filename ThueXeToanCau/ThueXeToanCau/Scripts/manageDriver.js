var isExistInfo = false;
$(function () {
    var options = {
        map: "#map-canvas"
    };
    $("#tCMND_err").hide();
    $("#car_number_err").hide();
    $("#address").geocomplete(options)
      .bind("geocode:result", function (event, result) {
          $("#lat").val(result.geometry.location.lat());
          $("#lon").val(result.geometry.location.lng());
          //alert("long" + result.geometry.location.lng() + ", lat=" + result.geometry.location.lat());
      })
      .bind("geocode:error", function (event, status) {
          $.log("ERROR: " + status);
      })
      .bind("geocode:multiple", function (event, results) {
          $.log("Multiple: " + results.length + " results found");
      });
});

function validateExistInfo(cmnd, carNumber) {    
    $.ajax({
        url: url_validateExistInfo, type: 'get',
        data: { card_identify: cmnd, car_number: carNumber },
        success: function (result) {
            if (result != '') {
                isExistInfo = true;
                if (cmnd != '') {
                    $("#tCMND").css("border", "1px solid red");
                    $("#tCMND_err").show();
                } else {
                    $("#car_number").css("border", "1px solid red");
                    $("#car_number_err").show();
                }
            } else {
                isExistInfo = false;
                if (cmnd != '') {
                    $("#tCMND").css("border", "1px solid #ccc");
                    $("#tCMND_err").hide();
                } else {
                    $("#car_number").css("border", "1px solid #ccc");
                    $("#car_number_err").hide();
                }
            }
        }
    });
}

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

function openDriver(dId, name, phone, card_identify, license, address, car_number,
            car_made, car_model, car_size, car_type, car_years,total_moneys) {
    name = resetValue(name, "");
    phone = resetValue(phone, "");
    card_identify = resetValue(card_identify, "");
    license = resetValue(license, "");
    address = resetValue(address, "");
    car_number = resetValue(car_number, "");
    car_made = resetValue(car_made, "");
    car_model = resetValue(car_model, "");
    car_size = resetValue(car_size, 5);
    car_type = resetValue(car_type, '');    
    car_years = resetValue(car_years, 2016);

    $("#driverId").val(dId);
    $("#tname").val(name);
    $("#tphone").val(phone); 
    $("#car_model").val(car_model);
    $("#tCMND").val(card_identify);
    $('#car_year option[value=' + car_years + ']').prop('selected', true);
    $('#car_size option[value=' + car_size + ']').prop('selected', true);
    $("#car_number").val(car_number);
    $("#total_moneys").val(total_moneys);
    //if(car_type != '') {
    //    $('#car_type option[value=' + car_type + ']').prop('selected', true);
    //}        
    $("#address").val(address);
    $("#tLicense").val(license);
    if (car_made != '') {
        $('#car option[value=' + car_made + ']').prop('selected', true);
    }
    $.ajax({
        url: "/Driver/getLocation?phone=" + phone,
        cache: false
    }).done(function (html) {
        var ll = html.split("_");
        $("#lat").val(ll[0]);
        $("#lon").val(ll[1]);
        //alert(html);
    });
    $("#driverDialog").show();
}

function saveDriver() {
    var isValid = true;
    $("#driverDialog input").each(function (idx, ip) {
        if (ip.id != 'tPass' && ip.id != 'tPassConfirm' && $(ip).val().trim() == "") {
            $(ip).css("border", "1px solid red");
            isValid = false;
        } else {
            $(ip).css("border", "1px solid #ccc");
        }
    });
    if (!isValid) return false;

    isValid = true;
    var dId = $("#driverId").val();
    if (dId == 0) {
        if ($("#tPass").val() == '' || $("#tPass").val() != $("#tPassConfirm").val()) {
            isValid = false;
        }
    } else if ($("#tPass").val() != $("#tPassConfirm").val()) {
        isValid = false;
    }
    if (!isValid) {
        $("#tPass").css("border", "1px solid red");
        $("#tPass").val('');
        $("#tPassConfirm").css("border", "1px solid red");
        $("#tPassConfirm").val('');
        return false;
    }
    if ($("#tname").val() == '') {
        alert("Nhập tên tài xế");
        isValid = false;
        return false;
    }
    if ($("#tphone").val() == '') {
        alert("Nhập số điện thoại");
        isValid = false;
        return false;
    }
    if ($("#address").val() == '' || $("#lon").val() == '') {
        alert("Nhập địa chỉ tài xế hay đón khách");
        isValid = false;
        return false;
    }
    if ($("#car_model").val() == '') {
        alert("Nhập model xe");
        isValid = false;
        return false;
    }

    if (isExistInfo) return false;
    var driverObj = {
        id: dId, name: $("#tname").val(), pass: $("#tPass").val(), phone: $("#tphone").val(), car_model: $("#car_model").val(),
        card_identify: $("#tCMND").val(), car_years: $("#car_year").val(), car_size: $("#car_size").val(), car_number: $("#car_number").val(),
        car_type: $("#car_type").val(), address: $("#address").val(), license: $("#tLicense").val(), car_made: $("#car").val()
        , total_moneys: $("#total_moneys").val()
    };
    var driverObj2 = {
        id: 0, car_number: $("#car_number").val(), lat: $("#lat").val(), lon: $("#lon").val(), phone: $("#tphone").val()
    };
    $.ajax({
        url: url_addUpdateDriver2, type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(driverObj2),
        success: function (rs) {
            if (rs == '') {
                $.ajax({
                    url: url_addUpdateDriver, type: 'post',
                    contentType: 'application/json',
                    data: JSON.stringify(driverObj),
                    success: function (rs) {
                        if (rs == '') {
                            alert("Đăng Ký Thành Công!");
                            location.reload();
                        } else {
                            alert(rs);
                        }
                    }
                })
            } else {
                alert(rs);
            }
        }
    })
    //$.ajax({
    //    url: url_addUpdateDriver, type: 'post',
    //    contentType: 'application/json',
    //    data: JSON.stringify(driverObj),
    //    success: function (rs) {
    //        if (rs == '') {
    //            location.reload();
    //        } else {
    //            alert(rs);
    //        }
    //    }
    //})
}
function saveDriver2() {
    var isValid = true;
    //$("#driverDialog input").each(function (idx, ip) {
    //    if (ip.id != 'tPass' && ip.id != 'tPassConfirm' && $(ip).val().trim() == "") {
    //        $(ip).css("border", "1px solid red");
    //        isValid = false;
    //    } else {
    //        $(ip).css("border", "1px solid #ccc");
    //    }
    //});
    //if (!isValid) return false;

    isValid = true;
    var dId = $("#driverId").val();
    if (dId == 0) {
        if ($("#tPass").val() == '' || $("#tPass").val() != $("#tPassConfirm").val()) {
            alert("Chưa nhập mật khẩu hoặc mật khẩu không trùng nhau");
            isValid = false;
            return false;
        }
    } else if ($("#tPass").val() != $("#tPassConfirm").val()) {
        alert("Nhập mật khẩu không trùng nhau");
        isValid = false;
        return false;
    }
    if (!isValid) {
        $("#tPass").css("border", "1px solid red");
        $("#tPass").val('');
        $("#tPassConfirm").css("border", "1px solid red");
        $("#tPassConfirm").val('');
        return false;
    }
    if ($("#tname").val() == '') {
        alert("Nhập tên tài xế");
        isValid = false;
        return false;
    }
    if ($("#tphone").val() == '') {
        alert("Nhập số điện thoại");
        isValid = false;
        return false;
    }
    if ($("#address").val() == '' || $("#lon").val() == '') {
        alert("Nhập địa chỉ tài xế hay đón khách");
        isValid = false;
        return false;
    }
    if ($("#car_model").val() == '') {
        alert("Nhập model xe");
        isValid = false;
        return false;
    }
    
    if (isExistInfo) return false;
    var driverObj = {
        id: dId, name: $("#tname").val(), pass: $("#tPass").val(), phone: $("#tphone").val(), car_model: $("#car_model").val(),
        card_identify: $("#tCMND").val(), car_years: $("#car_year").val(), car_size: $("#car_size").val(), car_number: $("#car_number").val(),
        car_type: $("#car_type").val(), address: $("#address").val(), license: $("#tLicense").val(), car_made: $("#car").val()
        , total_moneys: $("#total_moneys").val()
    };
    var driverObj2 = {
        id: 0, car_number: $("#car_number").val(), lat: $("#lat").val(), lon: $("#lon").val(), phone: $("#tphone").val()
    };
    $.ajax({
        url: url_addUpdateDriver2, type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(driverObj2),
        success: function (rs) {
            if (rs == '') {
                $.ajax({
                    url: url_addUpdateDriver, type: 'post',
                    contentType: 'application/json',
                    data: JSON.stringify(driverObj),
                    success: function (rs) {
                        if (rs == '') {
                            alert("Đăng Ký Thành Công! Mời Đăng Nhập Tài Xế");
                            window.location.href = "/Driver/Log";
                        } else {
                            alert(rs);
                        }
                    }
                })
            } else {
                alert(rs);
            }
        }
    })
}
function confirmDelDriver(dId, name) {
    $("#driverId").val(dId);
    openNotification("Bạn có chắc chắn xóa " + name + " ?", "delDriver");
}

function deleteDriver() {
    $.ajax({
        url: url_deleteDriver, type: 'post',
        data: { dId: $("#driverId").val() },
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}