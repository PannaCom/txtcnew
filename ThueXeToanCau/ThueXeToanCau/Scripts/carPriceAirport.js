function openCarPrice(carId, airport, car_size, price_go, price_back, price_two) {
    airport = resetValue(airport, "");
    car_size = resetValue(car_size, 5);
    price_go = resetValue(price_go, 5000);
    price_back = resetValue(price_back, 5000);
    price_two = resetValue(price_two, 5000);

    $("#cp_ID").val(carId);
    $("#cp_airport_name").val(airport);
    $('#cp_car_type option[value=' + car_size + ']').prop('selected', true);
    $("#cp_Price_go").val(price_go);
    $("#cp_Price_back").val(price_back);
    $("#cp_Price_two").val(price_two);
    $("#carPriceDialog").show();
}

function saveCarPrice() {
    var airport = $("#cp_airport_name").val().trim();
    $("#cp_airport_name").css("border", "1px solid #ccc");
    if(airport == '') {
        $("#cp_airport_name").css("border", "1px solid red");
        return false;
    }
    $.ajax({
        url: url_addUpdateCarPriceAirport, type: 'post',
        contentType: 'application/json',
        data: JSON.stringify({
            id: $("#cp_ID").val(), airport_name: airport, car_size: $("#cp_car_type").val(),
            price_go_way: $("#cp_Price_go").val(), price_back_way: $("#cp_Price_back").val(), price_two_way: $("#cp_Price_two").val()
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
        url: url_deleteCarPriceAirport, type: 'post',
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