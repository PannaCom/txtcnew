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
    }
    closeDDialog("#notificationDialog");
}