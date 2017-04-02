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
                    window.location.href = "/Transaction/index";
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
function uploadFileOwn() {
    var files = document.getElementById("fileUpload").files;
    if (files.length == 0) {
        alert('Xin hãy chọn 1 bảng kê!');
    } else {
        if (window.FormData !== undefined) {
            document.getElementById("uploadfileown").value = "Đang cập nhật xin chờ...";
            document.getElementById("uploadfileown").disabled = true;
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
                    window.location.href = "/Transaction/Own";
                    document.getElementById("uploadfileown").value = "Tải lên file khác";
                    document.getElementById("uploadfileown").disabled = false;
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
        alert('Xin hãy chọn 1 bảng tài khoản tài xế đúng theo mẫu!');
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
                    window.location.href = "/Transaction/Bank";
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
function uploadFileSalary() {
    var files = document.getElementById("fileUpload").files;
    if (files.length == 0) {
        alert('Xin hãy chọn 1 bảng kê!');
    } else {
        if (document.getElementById("fromDate").value == "") {
            alert("Nhập từ ngày!");
            document.getElementById("fromDate").focus();
            return;
        }
        if (document.getElementById("toDate").value == "") {
            alert("Nhập đến ngày!");
            document.getElementById("toDate").focus();
            return;
        }
        if (window.FormData !== undefined) {
            document.getElementById("uploadfilesalary").value = "Đang cập nhật xin chờ...";
            document.getElementById("uploadfilesalary").disabled = true;
            $(".overlayDiv").show();
            var data = new FormData();
            for (var x = 0; x < files.length; x++) {
                data.append("file" + x, files[x]);
            }
            data.append("from_date", document.getElementById("fromDate").value);
            data.append("to_date", document.getElementById("toDate").value);
            $.ajax({
                url: url_uploadFile, type: "POST",
                contentType: false, processData: false,
                data: data,
                success: function (result) {
                    $(".overlayDiv").hide();
                    alert(result);
                    window.location.href = "/Transaction/Salary";
                    document.getElementById("uploadfilesalary").value = "Tải lên file khác";
                    document.getElementById("uploadfilesalary").disabled = false;
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
                    tbHtml += '<tr><th>STT</th><th>Biển số xe</th><th>Tên tài xế</th><th>Số tài khoản</th><th>Tên Ngân Hàng</th><th></th></tr>';
                    $.each(result, function (idx, obj) {
                        tbHtml += '<tr><td>' + (idx + 1) + '</td><td>' + obj.car_number + '</td><td>' + obj.driver_name + '</td>'
                            + '<td>' + obj.bank_number + '</td><td>' + obj.bank_name + '</td><td><input type=button value="Xóa" onclick="DelBank(' + obj.id + ');" id=delbank_' + obj.id + '></td></tr>'
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
function searchTranSalary() {
    var carNumber = $('#car_number').val();
    //if(carNumber == '') {
    //    alert("Xin hãy chọn 1 biển số xe");
    //    return false;
    //}
    var detail = true;
    $.ajax({
        url: url_searchTran, type: 'get', dataType: 'json',
        data: { carNumber: carNumber, fDate: $('#fromDate').val(), tDate: $('#toDate').val(), isDetail: detail },
        success: function (result) {
            //alert(result.responseText);
            if (!result.ErrMess) {
                var tbHtml = '<table class="table">'
                if (result.length == 0) {
                    tbHtml += '<tr>Không tìm thấy dữ liệu phù hợp</tr>';
                } else if (detail) {
                    tbHtml += '<tr><th>STT</th><th>Biển số xe</th><th>Tên tài xế</th><th>Số tiền</th><th>Số tài khoản</th><th>Tên Ngân Hàng</th><th>Từ ngày</th><th>Đến ngày</th></tr>';
                    $.each(result, function (idx, obj) {
                        tbHtml += '<tr><td>' + (idx + 1) + '</td><td>' + obj.car_number + '</td><td>' + obj.driver_name + '</td><td>' + obj.money + '</td>'
                            + '<td>' + obj.bank_number + '</td><td>' + obj.bank_name + '</td><td>' + parseDate(obj.from_date) + '</td><td>' + parseDate(obj.to_date) + '</td></tr>'
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
function searchTranOwn() {
    var carNumber = $('#car_number').val();
    //if(carNumber == '') {
    //    alert("Xin hãy chọn 1 biển số xe");
    //    return false;
    //}
    var detail = true;
    $.ajax({
        url: url_searchTran, type: 'get', dataType: 'json',
        data: { carNumber: carNumber, fDate: $('#fromDate').val(), tDate: $('#toDate').val()},
        success: function (result) {
            //alert(result.responseText);
            if (!result.ErrMess) {
                var tbHtml = '<table class="table">'
                if (result.length == 0) {
                    tbHtml += '<tr>Không tìm thấy dữ liệu phù hợp</tr>';
                } else if (detail) {
                    tbHtml += '<tr><th>STT</th><th>Biển số xe</th><th>Tên tài xế</th><th>Công nợ tháng</th><th>Ngày phải nộp</th><th>Công nợ kỳ</th><th>Ngày phải nộp</th><th>Công nợ năm</th><th>Ngày phải nộp</th><th>Ngày upload</th></tr>';
                    $.each(result, function (idx, obj) {
                        tbHtml += '<tr><td>' + (idx + 1) + '</td><td>' + obj.car_number + '</td><td>' + obj.driver_name + '</td><td>' + obj.money_month + '</td>'
                            + '<td>' + parseDate(obj.date_month) + '</td><td>' + obj.money_period + '</td><td>' + parseDate(obj.date_period) + '</td><td>' + obj.money_year + '</td><td>' + parseDate(obj.date_year) + '</td><td>' + parseDate(obj.date_time) + '</td></tr>'
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
function looksLikeIsoDate(s) {
    return isoDateRegex.test(s);
}
function looksLikeMSDate(s) {
    return /^\/Date\(/.test(s);
}
function parseMSDate(s) {
    // Jump forward past the /Date(, parseInt handles the rest
    return new Date(parseInt(s.substr(6)));
}
function formatDate(d) {
    if (hasTime(d)) {
        var s = (d.getMonth() + 1) + '/' + d.getDate() + '/' + d.getFullYear();
        s += ' ' + d.getHours() + ':' + zeroFill(d.getMinutes()) + ':' + zeroFill(d.getSeconds());
    } else {
        var s = (d.getMonth() + 1) + '/' + d.getDate() + '/' + d.getFullYear();
    }

    return s;
}
function parseDate(s) {
    var d;
    if (looksLikeMSDate(s))
        d = parseMSDate(s);
    else if (looksLikeIsoDate(s))
        d = parseIsoDate(s);
    else
        return null;

    return formatDate(d);
}
function parseIsoDate(s) {
    var m = isoDateRegex.exec(s);

    // Is this UTC, offset, or undefined? Treat undefined as UTC.
    if (m.length == 7 ||                // Just the y-m-dTh:m:s, no ms, no tz offset - assume UTC
        (m.length > 7 && (
            !m[7] ||                    // Array came back length 9 with undefined for 7 and 8
            m[7].charAt(0) != '.' ||    // ms portion, no tz offset, or no ms portion, Z
            !m[8] ||                    // ms portion, no tz offset
            m[8] == 'Z'))) {            // ms portion and Z
        // JavaScript's weirdo date handling expects just the months to be 0-based, as in 0-11, not 1-12 - the rest are as you expect in dates.
        var d = new Date(Date.UTC(m[1], m[2] - 1, m[3], m[4], m[5], m[6]));
    } else {
        // local
        var d = new Date(m[1], m[2] - 1, m[3], m[4], m[5], m[6]);
    }

    return d;
}
function deleteFile() {
    if (confirm("CẢNH BÁO: BẠN CHẮC CHẮN MUỐN XÓA DỮ LIỆU NÀY?")) {
        if (document.getElementById("fromDate").value == "") {
            alert("Nhập từ ngày!");
            document.getElementById("fromDate").focus();
            return;
        }
        if (document.getElementById("toDate").value == "") {
            alert("Nhập đến ngày!");
            document.getElementById("toDate").focus();
            return;
        }
        var formdata = new FormData();
        formdata.append("id", "1");
        formdata.append("fDate", $('#fromDate').val());
        formdata.append("tDate", $('#toDate').val());
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
function deleteFileSalary() {
    if (confirm("CẢNH BÁO: BẠN CHẮC CHẮN MUỐN XÓA DỮ LIỆU NÀY?")) {
        if (document.getElementById("fromDate").value == "") {
            alert("Nhập từ ngày!");
            document.getElementById("fromDate").focus();
            return;
        }
        if (document.getElementById("toDate").value == "") {
            alert("Nhập đến ngày!");
            document.getElementById("toDate").focus();
            return;
        }
        var formdata = new FormData();
        formdata.append("id", "1");
        formdata.append("fDate", $('#fromDate').val());
        formdata.append("tDate", $('#toDate').val());
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/Transaction/DelSalary');
        xhr.send(formdata);
        document.getElementById("delallsalary").value = "Đang cập nhật xin chờ...";
        document.getElementById("delallsalary").disabled = true;
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                if (xhr.responseText == "1") {
                    alert("Bạn đã xóa thành công!");
                    window.location.href = "/Transaction/Salary";
                } else {
                    alert("Chương trình đang cập nhật, xin quay lại sau!");
                }
                document.getElementById("delallsalary").value = "Xóa dữ liệu!!!";
                document.getElementById("delallsalary").disabled = false;
            }
        }
    }
}
function deleteFileOwn() {
    if (confirm("CẢNH BÁO: BẠN CHẮC CHẮN MUỐN XÓA DỮ LIỆU NÀY?")) {
        if (document.getElementById("fromDate").value == "") {
            alert("Nhập từ ngày!");
            document.getElementById("fromDate").focus();
            return;
        }
        if (document.getElementById("toDate").value == "") {
            alert("Nhập đến ngày!");
            document.getElementById("toDate").focus();
            return;
        }
        var formdata = new FormData();
        formdata.append("id", "1");
        formdata.append("fDate", $('#fromDate').val());
        formdata.append("tDate", $('#toDate').val());
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/Transaction/DelOwn');
        xhr.send(formdata);
        document.getElementById("delallown").value = "Đang cập nhật xin chờ...";
        document.getElementById("delallown").disabled = true;
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                if (xhr.responseText == "1") {
                    alert("Bạn đã xóa thành công!");
                    window.location.href = "/Transaction/Own";
                } else {
                    alert("Chương trình đang cập nhật, xin quay lại sau!");
                }
                document.getElementById("delallown").value = "Xóa dữ liệu!!!";
                document.getElementById("delallown").disabled = false;
            }
        }
    }
}
function DelBank(id) {
    if (confirm("CẢNH BÁO: BẠN CHẮC CHẮN MUỐN XÓA DỮ LIỆU NÀY?")) {

        var formdata = new FormData();
        formdata.append("id", id);
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/Transaction/DelBank');
        xhr.send(formdata);
        document.getElementById("delbank_"+id).value = "Đang cập nhật xin chờ...";
        document.getElementById("delbank_" + id).disabled = true;
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                if (xhr.responseText == "1") {
                    alert("Bạn đã xóa thành công!");
                    window.location.href = "/Transaction/Bank";
                } else {
                    alert("Chương trình đang cập nhật, xin quay lại sau!");
                }
                document.getElementById("delbank_" + id).value = "Xóa";
                document.getElementById("delbank_" + id).disabled = false;
            }
        }
    }
}
function DelAllBank(id) {
    if (confirm("CẢNH BÁO: BẠN CHẮC CHẮN MUỐN XÓA DỮ LIỆU NÀY?")) {

        var formdata = new FormData();
        formdata.append("id", "1");
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/Transaction/DelAllBank');
        xhr.send(formdata);
        document.getElementById("delallbank").value = "Đang cập nhật xin chờ...";
        document.getElementById("delallbank").disabled = true;
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                if (xhr.responseText == "1") {
                    alert("Bạn đã xóa thành công!");
                    window.location.href = "/Transaction/Bank";
                } else {
                    alert("Chương trình đang cập nhật, xin quay lại sau!");
                }
                document.getElementById("delallbank").value = "Xóa sạch dữ liệu";
                document.getElementById("delallbank").disabled = false;
            }
        }
    }
}
function toBank() {
    var url = "";
    if (document.getElementById("fromDate").value == "") {
        alert("Nhập từ ngày!");
        document.getElementById("fromDate").focus();
        return;
    }
    if (document.getElementById("toDate").value == "") {
        alert("Nhập đến ngày!");
        document.getElementById("toDate").focus();
        return;
    }
    var type = 0;
    if (document.getElementById("typeMoney").checked) type = 1;
    url += "from_date=" + document.getElementById("fromDate").value + "&" + "to_date=" + document.getElementById("toDate").value + "&type=" + type;

    window.open("/Transaction/baocao?" + url, "_blank");
    alert("Mở file excel vừa download, sau đó save as dưới dạng file .xlsx để upload lương cho tài xế ở mục Upload lương sau khi ngân hàng đã chuyển tiền!");
}