function uploadFile() {
    var files = document.getElementById("fileUpload").files;
    if (files.length == 0) {
        alert('Xin hãy chọn 1 bảng kê!');
    } else {
        if (window.FormData !== undefined) {
            var data = new FormData();
            for (var x = 0; x < files.length; x++) {
                data.append("file" + x, files[x]);
            }

            $.ajax({                
                url: url_uploadFile, type: "POST",
                contentType: false, processData: false,
                data: data,
                success: function (result) {                    
                    alert(result);
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

function searchTran() {
    var carNumber = $('#car_number').val();
    if(carNumber == '') {
        alert("Xin hãy chọn 1 biển số xe");
        return false;
    }
    var detail = $('#chkDetail').prop("checked");
    $.ajax({
        url: url_searchTran, type: 'get',dataType: 'json',
        data: { carNumber: carNumber, fDate: $('#fromDate').val(), tDate: $('#toDate').val(), isDetail: detail },
        success: function (result) {
            console.log(result);
            var tbHtml = '<table class="table">'
            if (detail) {
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
        }
    })
}