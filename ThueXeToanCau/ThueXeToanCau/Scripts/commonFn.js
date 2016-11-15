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
    } else if (action == "delNotice") {
        deleteNotice();
    } else if (action == "delUser") {
        deleteUser();
    } else if (action == "delDriver") {
        deleteDriver();
    } else if (action == "delCarType") {
        deleteCarType();
    } else if (action == "delHireType") {
        deleteHireType();
    } else if (action == "delWhoType") {
        deleteWhoType();
    }
    closeDDialog("#notificationDialog");
}

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