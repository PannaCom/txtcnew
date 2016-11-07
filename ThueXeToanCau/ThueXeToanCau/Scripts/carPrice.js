function openCarPrice(carId, car_size, price, multiple, multiple2) {    
    car_size = resetValue(car_size, 5);
    price = resetValue(price, 5000);
    multiple = resetValue(multiple, 1);
    multiple2 = resetValue(multiple2, 1);

    $("#cp_ID").val(carId);
    $('#cp_car_type option[value=' + car_size + ']').prop('selected', true);
    $("#cp_Price").val(price);
    $("#cp_Multiple").val(multiple);
    $("#cp_Multiple2").val(multiple2);
    $("#carPriceDialog").show();
}

function saveCarPrice() {
    $.ajax({
        url: url_addUpdateCarPrice, type: 'post',
        contentType: 'application/json',
        data: JSON.stringify({
            ID: $("#cp_ID").val(), car_size: $("#cp_car_type").val(), price: $("#cp_Price").val(),
            multiple: $("#cp_Multiple").val(), multiple2: $("#cp_Multiple2").val()
        }),
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    })
}

function confirmDelCarPrice(cpId, car_size) {
    $("#cp_ID").val(cpId);
    openNotification("Bạn có chắc chắn xóa " + car_size + " ?", "delCarPrice");
}

function deleteCarPrice() {
    $.ajax({
        url: url_deleteCarPrice, type: 'post',
        data: { cpId: $("#cp_ID").val() },
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}


function resetValue(obj, defaultVal) {
    if (defaultVal == undefined) defaultVal = '';
    if (obj == undefined || obj == null) return defaultVal;
    return obj;
}

function closeDDialog(dID) {
    $(dID).hide();
}

function openNotification(txt, action) {
    $("#lbNotification").text(txt);
    $("#notifyAction").val(action);
    $("#notificationDialog").show();
}

function notifyOK() {
    var action = $("#notifyAction").val();
    if (action == "delCarPrice") {
        deleteCarPrice();
    }
    closeDDialog("#notificationDialog");
}