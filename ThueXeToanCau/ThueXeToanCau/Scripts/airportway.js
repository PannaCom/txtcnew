$(function () {   
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

function openAirportWay(id) {
    $("#airportWayId").val(id);
    if (id == 0) {
        $("#airport_name").val('');
        $("#car_from").val('');
        $("#lon1").val('');
        $("#lat1").val('');
        $("#car_to").val('');
        $("#lon2").val('');
        $("#lat2").val('');
        $("#airportWayDialog").show();
    } else {
        $.ajax({
            url: url_getAirportWay, type: 'get',
            data: { aId: id }, dataType: 'json',
            success: function (obj) {
                if (obj == null) {
                    alert("Không tìm thấy thông tin, vui lòng thử lại sau!");
                    return false;
                }
                if (!obj.errMess) {
                    $("#airport_name").val(obj.airport_name);
                    $("#car_from").val(resetValue(obj.car_from));
                    $("#lon1").val(resetValue(obj.lon1));
                    $("#lat1").val(resetValue(obj.lat1));
                    $("#car_to").val(resetValue(obj.car_to));
                    $("#lon2").val(resetValue(obj.lon2));
                    $("#lat2").val(resetValue(obj.lat2));

                    $("#airportWayDialog").show();
                } else {
                    alert(obj.errMess);
                }
            },
            error: function (err) {
                alert(err_generalMess);
            }
        });
    }
}

function saveAirportWay() {    
    var aw = {
        id: $("#airportWayId").val(), airport_name: $("#airport_name").val(),
        car_from: $("#car_from").val(), lon1: $("#lon1").val(), lat1: $("#lat1").val(),
        car_to: $("#car_to").val(), lon2: $("#lon2").val(), lat2: $("#lat2").val()
    };
    $("#airportWayDialog").hide();
    $.ajax({
        url: url_addUpdateAirportWay, type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(aw),
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}

function confirmDelAirportWay(id, name) {
    $("#airportWayId").val(id);
    openNotification("Bạn có chắc chắn xóa " + name + " ?", "delAirportWay");
}

function deleteAirportWay() {
    $.ajax({
        url: url_deleteAirportWay, type: 'post',
        data: { id: $("#airportWayId").val() },
        success: function (rs) {
            if (rs == '') {
                location.reload();
            } else {
                alert(rs);
            }
        }
    });
}