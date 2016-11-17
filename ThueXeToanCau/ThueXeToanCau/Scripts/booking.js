$(function () {
    //$('#from_datetime').datetimepicker({
    //    dayOfWeekStart: 1, lang: 'en',
    //    disabledDates: ['1986/01/08', '1986/01/09', '1986/01/10'],
    //    startDate: '2016/11/11/17/2016 12:00:00 AM'
    //});
    //$('#to_datetime').datetimepicker({
    //    dayOfWeekStart: 1, lang: 'en',
    //    disabledDates: ['1986/01/08', '1986/01/09', '1986/01/10'],
    //    startDate: '2016/11/11/17/2016 12:00:00 AM'
    //});
    var s = new Date().toLocaleString();
    $('#from_datetime').datetimepicker({ value: s, step: 10 });
    $('#to_datetime').datetimepicker({ value: s, step: 10 });

    //var options = {
    //    map: ".map_canvas"
    //};
    $("#car_from").geocomplete()
      .bind("geocode:result", function (event, result) {
          $("#lon1").val(result.geometry.location.lng());
          $("#lat1").val(result.geometry.location.lat());
      })
      .bind("geocode:error", function (event, status) {
          $.log("ERROR: " + status);
      })
      .bind("geocode:multiple", function (event, results) {
          $.log("Multiple: " + results.length + " results found");
      });
    $("#car_to").geocomplete()
      .bind("geocode:result", function (event, result) {
          $("#lon2").val(result.geometry.location.lng());
          $("#lat2").val(result.geometry.location.lat());
      })
      .bind("geocode:error", function (event, status) {
          $.log("ERROR: " + status);
      })
      .bind("geocode:multiple", function (event, results) {
          $.log("Multiple: " + results.length + " results found");
      });
});


function searchPhone() {
    $('#ipSearch').autocomplete({
        source: url_searchPhone + '?keyword=' + $("#ipSearch").val(),
        select: function (event, ui) {
            $(event.target).val(ui.item.value);
            return false;
        },
        minLength: 1
    });
}

function searchBooking() {
    var txtSearch = $("#ipSearch").val();
    if (txtSearch == '') {
        alert('Xin hãy nhập số điện thoại hoặc tên');
        return false;
    }
    $.ajax({
        url: url_searchBooking, type: 'get', dataType: 'json',
        data: { keyword: txtSearch, hireType: $("#s_car_hire_type").val(), whoType: $("#s_car_who_hire").val() },
        success: function (result) {
            if (!result.ErrMess) {
                var tbHtml = '<table class="table"><tr><th>STT</th><th>Tên</th><th>SĐT</th><th>Điểm Đi</th><th>Điểm Đến</th>'
                    + '<th>Loại Xe</th><th>Hình Thức Thuê</th><th>Đối Tượng Thuê</th><th>Ngày Đi</th><th>Ngày Về</th>'
                    + '<th>Giá Cước</th><th></th><th></th></tr>'
                $.each(result, function (idx, obj) {
                    tbHtml += '<tr><td>' + (idx + 1) + '</td><td>' + obj.name + '</td><td>' + obj.phone + '</td><td>' + obj.car_from
                        + '</td><td>' + obj.car_to + '</td><td>' + obj.car_type + ' chỗ</td><td>' + obj.car_hire_type + '</td><td>'
                        + obj.car_who_hire + '</td><td>' + convertJsDate(obj.from_datetime).toLocaleString() + '</td><td>'
                        + convertJsDate(obj.to_datetime).toLocaleString() + '</td><td>' + obj.book_price
                        + '</td><td><a href="#" onclick="openBooking(' + obj.id + ')">Sửa</a></td>'
                        + '<td><a href="#" onclick="confirmDelBooking(' + obj.id + ')">Xóa</a></td></tr>';
                });
                tbHtml += '</table>';
                $("#bookingResult").html(tbHtml);
            } else {
                alert(result.ErrMess);
            }
        }
    })
}

function openBooking(id) {
    $("#bookingId").val(id);
    $.ajax({
        url: url_getBooking, type: 'get',
        data: {bId: id}, dataType: 'json',
        success: function (obj) {
            if (obj == null) {
                alert("Không tìm thấy lịch đặt bạn yêu cầu!");
                return false;
            }
            if (!obj.errMess) {
                $("#b_Name").val(obj.name);
                $("#b_Phone").val(obj.phone);
                $("#car_from").val(obj.car_from);
                $("#lon1").val(obj.lon1);
                $("#lat1").val(obj.lat1);
                $("#car_to").val(obj.car_to);
                $("#lon2").val(obj.lon2);
                $("#lat2").val(obj.lat2);
                $('#car_type option[value=' + obj.car_type + ']').prop('selected', true);
                $('#car_hire_type option[value=\'' + obj.car_hire_type + '\']').prop('selected', true);
                $('#car_who_hire option[value=\'' + obj.car_who_hire + '\']').prop('selected', true);
                $("#from_datetime").val(convertJsDate(obj.from_datetime).toLocaleString());
                $("#to_datetime").val(convertJsDate(obj.to_datetime).toLocaleString());
                
                $("#bookingDialog").show();
            } else {
                alert(obj.errMess);
            }
        },
        error: function (err) {
            alert(err_generalMess);
        }
    })
}

function saveBooking() {    
    var booking = {
        id: $("#bookingId").val(), 
        car_from: $("#car_from").val(), lon1: $("#lon1").val(), lat1: $("#lat1").val(), car_to: $("#car_to").val(),
        lon2: $("#lon2").val(), lat2: $("#lat2").val(), car_type: $('#car_type').val(), car_hire_type: $('#car_hire_type').val(),
        car_who_hire: $('#car_who_hire').val(), from_datetime: $("#from_datetime").val(), to_datetime: $("#to_datetime").val(),
        name: $("#b_Name").val(), phone: $("#b_Phone").val()
    };
    $("#bookingDialog").hide();
    $.ajax({
        url: url_addUpdateBooking, type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(booking),
        success: function (rs) {
            if (rs == '') {
                searchBooking();
            } else {
                alert(rs);
            }
        }
    });
}

function confirmDelBooking(id, name) {
    $("#bookingId").val(id);
    openNotification("Bạn có chắc chắn xóa lịch đặt này ?", "delBooking");
}

function deleteBooking() {
    $.ajax({
        url: url_deleteBooking, type: 'post',
        data: { id: $("#bookingId").val() },
        success: function (rs) {
            if (rs == '') {
                searchBooking();
            } else {
                alert(rs);
            }
        }
    });
}