$(function () {        
    $('#fromDate').datepicker();
    $('#toDate').datepicker();
});

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
    var isMobile = isSmallWidth();
    console.log(isMobile);
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
                    tbHtml += '<tr>';
                    if (!isMobile) {
                        tbHtml += '<th>STT</th>';
                    }
                    tbHtml += '<th>Loại Giao Dịch</th><th>Biển Số Xe</th><th>Ngày Giao Dịch</th><th>Số Tiền</th>';
                    if (!isMobile) {
                        tbHtml += '<th>Ghi Chú</th>';
                    }
                    tbHtml += '</tr>';
                    $.each(result, function (idx, obj) {
                        tbHtml += '<tr>';
                        if (!isMobile) {
                            tbHtml += '<td>' + (idx + 1) + '</td>';
                        }
                        tbHtml += '<td>' + obj.type + '</td><td>' + obj.car_number + '</td><td>'
                            + convertDate(obj.date, false) + '</td><td>' + obj.money + '</td></tr>';
                        if (!isMobile) {
                            tbHtml += '<td>' + obj.note + '</td>';
                        }
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