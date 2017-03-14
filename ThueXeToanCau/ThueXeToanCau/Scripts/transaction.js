$(function () {
    $('#fromDate').datepicker();
    $('#toDate').datepicker();
});

function uploadFile() {
    var files = document.getElementById("fileUpload").files;
    if (files.length == 0) {
        alert('Xin hãy chọn 1 bảng kê!');
    } else {        
        if (window.FormData !== undefined) {
            document.getElementById("uploadfile").value = "Đang cập nhật xin chờ...";
            document.getElementById("uploadfile").disabled = true;
            $(".overlayDiv").show();
            var data = new FormData();
            for (var x = 0; x < files.length; x++) {
                data.append("file" + x, files[x]);
            }

            $.ajax({                
                url: url_uploadFile, type: "POST",
                contentType: false, processData: false,
                data: data,
                success: function (result) {
                    $(".overlayDiv").hide();
                    alert(result);
                    document.getElementById("uploadfile").value = "Tải lên file khác";
                    document.getElementById("uploadfile").disabled = false;
                },
                error: function (err) {
                    $(".overlayDiv").hide();
                }
            });
        } else {
            alert("Trình duyệt của bạn không hỗ trợ HTML5");
        }
    }
}
function uploadFileBank() {
    var files = document.getElementById("fileUpload").files;
    if (files.length == 0) {
        alert('Xin hãy chọn 1 bảng kê!');
    } else {
        if (window.FormData !== undefined) {
            document.getElementById("uploadfilebank").value = "Đang cập nhật xin chờ...";
            document.getElementById("uploadfilebank").disabled = true;
            $(".overlayDiv").show();
            var data = new FormData();
            for (var x = 0; x < files.length; x++) {
                data.append("file" + x, files[x]);
            }

            $.ajax({
                url: url_uploadFile, type: "POST",
                contentType: false, processData: false,
                data: data,
                success: function (result) {
                    $(".overlayDiv").hide();
                    alert(result);
                    document.getElementById("uploadfilebank").value = "Tải lên file khác";
                    document.getElementById("uploadfilebank").disabled = false;
                },
                error: function (err) {
                    $(".overlayDiv").hide();
                }
            });
        } else {
            alert("Trình duyệt của bạn không hỗ trợ HTML5");
        }
    }
}
function searchCarNumber() {
    $('#car_number').autocomplete({
        source: url_searchCarNumber + '?keyword=' + $("#car_number").val(),
        select: function (event, ui) {
            $(event.target).val(ui.item.value);
            return false;
        },
        minLength: 1
    });
}
function searchCarNumberBank() {
    $('#car_number').autocomplete({
        source: url_searchCarNumber + '?keyword=' + $("#car_number").val(),
        select: function (event, ui) {
            $(event.target).val(ui.item.value);
            return false;
        },
        minLength: 1
    });
}
function searchTran() {
    var carNumber = $('#car_number').val();
    //if(carNumber == '') {
    //    alert("Xin hãy chọn 1 biển số xe");
    //    return false;
    //}
    var detail = $('#chkDetail').prop("checked");
    $.ajax({
        url: url_searchTran, type: 'get',dataType: 'json',
        data: { carNumber: carNumber, fDate: $('#fromDate').val(), tDate: $('#toDate').val(), isDetail: detail },
        success: function (result) {
            if (!result.ErrMess) {
                var tbHtml = '<table class="table">'
                if (result.length == 0) {
                    tbHtml += '<tr>Không tìm thấy dữ liệu phù hợp</tr>';
                } else if (detail) {
                    tbHtml += '<tr><th>STT</th><th>Loại Giao Dịch</th><th>Biển Số Xe</th><th>Ngày Giao Dịch</th><th>Số Tiền</th><th>Ghi Chú</th></tr>';
                    $.each(result, function (idx, obj) {
                        tbHtml += '<tr><td>' + (idx + 1) + '</td><td>' + obj.type + '</td><td>' + obj.car_number + '</td><td>'
                            + convertDate(obj.date, false) + '</td><td>' + obj.money + '</td><td>' + obj.note + '</td></tr>'
                    });
                } else {
                    tbHtml += '<tr><th>Biển số xe</th><th>Tổng Số Giao Dịch</th><th>Tổng Giá Trị Giao Dịch</th></tr>';
                    tbHtml += '<tr><td>' + result[0].carNum + '</td><td>' + result[0].count + '</td><td>' + result[0].sum + '</td></tr>';
                }
                tbHtml += '</table>';
                $("#tranResult").html(tbHtml);
            } else {
                alert(result.ErrMess);
            }
        }
    })
}
function searchTranBank() {
    var carNumber = $('#car_number').val();
    //if(carNumber == '') {
    //    alert("Xin hãy chọn 1 biển số xe");
    //    return false;
    //}
    var detail = true;
    $.ajax({
        url: url_searchTran, type: 'get', dataType: 'json',
        data: { carNumber: carNumber},
        success: function (result) {
            //alert(result.responseText);
            if (!result.ErrMess) {
                var tbHtml = '<table class="table">'
                if (result.length == 0) {
                    tbHtml += '<tr>Không tìm thấy dữ liệu phù hợp</tr>';
                } else if (detail) {
                    tbHtml += '<tr><th>STT</th><th>Biển số xe</th><th>Tên tài xế</th><th>Số tài khoản</th><th>Tên Ngân Hàng</th></tr>';
                    $.each(result, function (idx, obj) {
                        tbHtml += '<tr><td>' + (idx + 1) + '</td><td>' + obj.car_number + '</td><td>' + obj.driver_name + '</td>'
                            + '<td>' + obj.bank_number + '</td><td>' + obj.bank_name + '</td></tr>'
                    });
                } else {
                    //tbHtml += '<tr><th>Biển số xe</th><th>Tổng Số Giao Dịch</th><th>Tổng Giá Trị Giao Dịch</th></tr>';
                    //tbHtml += '<tr><td>' + result[0].carNum + '</td><td>' + result[0].count + '</td><td>' + result[0].sum + '</td></tr>';
                }
                tbHtml += '</table>';
                $("#tranResult").html(tbHtml);
            } else {
                alert(result.ErrMess);
            }
        }
    })
}
function deleteFile() {
    if (confirm("CẢNH BÁO: BẠN CHẮC CHẮN MUỐN XÓA HẾT DỮ LIỆU NÀY?")) {
        
        var formdata = new FormData();
        formdata.append("id", "1");
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/Transaction/DelTransaction');
        xhr.send(formdata);
        document.getElementById("delall").value = "Đang cập nhật xin chờ...";
        document.getElementById("delall").disabled = true;
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                if (xhr.responseText == "1") {
                    alert("Bạn đã xóa thành công!");
                    window.location.href = "/Transaction/index";
                } else {
                    alert("Chương trình đang cập nhật, xin quay lại sau!");
                }
                document.getElementById("delall").value = "Xóa sạch dữ liệu!!!";
                document.getElementById("delall").disabled = false;
            }
        }
    }
}