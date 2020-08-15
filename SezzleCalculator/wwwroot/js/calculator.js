var isDisplayingResult = false;
var webSocket;

function CalcButtonClickEventHandler(val) {
    if (document.getElementById("calc_display").innerHTML.length >= 25) {
        window.alert("Max Input Reached");
    } else {
        if (!isDisplayingResult) { // if we are showing the result of a calc, our next button push should clear the display. Otherwise append to it.
            var charArray = ["-", "+", "/", "*"]; // calc inputs that we want to put a space before and after. This is only for visual formatting.

            if (charArray.includes(val)) {
                val = " " + val + " ";
            }
        } else {
            document.getElementById("calc_display").innerHTML = "";
            isDisplayingResult = false;
        }

        document.getElementById("calc_display").innerHTML = document.getElementById("calc_display").innerHTML + val;
    }
} 

function ClearInput() {
    document.getElementById("calc_display").innerHTML = "";
}

function EqualsButtonEventHandler() {
    if (!isDisplayingResult) {
        $.ajax({
            type: "POST",
            url: "/calculator",
            data: JSON.stringify(document.getElementById("calc_display").innerHTML),
            dataType: "JSON",
            contentType: "application/json",
        }).done(function (data) {
            document.getElementById("calc_display").innerHTML = document.getElementById("calc_display").innerHTML = data.result
            isDisplayingResult = true;
        }).fail(function () {
            document.getElementById("calc_display").innerHTML = document.getElementById("calc_display").innerHTML = "NaN";
            isDisplayingResult = true;
        });
    }
}

function GetCalculationHistory() {
    $.get("/calculator", function (data) {
        if (data.length != 0) {
            $("#calc_history tr").remove();
            for (var i = 0; i < data.length; i++) {
                AppendRowToHistoryTable(data[i]);
            }
        }
    }, "json");
}

function AppendRowToHistoryTable(data) {
    $("#calc_history").append("<tr><td>" + data.CalcString + " = " + data.Result + "</td></tr>");
}


(function () {
    // Load the inital Calc history and display on the page. 
    GetCalculationHistory();

    // configure web sockets to listen for new clac history objects from the server.
    var getWebSocketMessages = function (onMessageReceived) {
        var url = `wss://${location.host}/WebSocket`;

        webSocket = new WebSocket(url);

        webSocket.onmessage = onMessageReceived;
    };

    window.onbeforeunload = function () { // close the web socket if the page is refreshed
        webSocket.close();
    };

    getWebSocketMessages(function (result) {
        $("#calc_history tr").remove();
        var data = JSON.parse(result.data);
        for (var i = 0; i < data.length; i++) {
            AppendRowToHistoryTable(data[i]);
        }
    });
}());