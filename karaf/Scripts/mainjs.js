$('activity').on('click', function (event) {
    alert("in here");
});

$(document).ready(function () {
    $('input').iCheck({
        checkboxClass: 'icheckbox_minimal',
        radioClass: 'iradio_minimal',
        increaseArea: '20%' // optional
    });
});

function pauseBundle(url) {

    var pmUrl = 'http://127.0.0.1:7098/api/v2/app.management.api/bundle.pause/' + url;
    $.ajax({
        crossDomain: true,
        url: pmUrl,
        type: 'POST',
        dataType: 'jsonp',
        jsonpCallback: 'callback',
        success: function(result) {
            location.reload();
        }
    });
}

function unPauseBundle(url) {

    var pmUrl = 'http://127.0.0.1:7098/api/v2/app.management.api/bundle.start/' + url;
    $.ajax({
        crossDomain: true,
        url: pmUrl,
        type: 'POST',
        dataType: 'jsonp',
        jsonpCallback: 'callback',
        success: function (result) {
            location.reload();
        }
    });
}

$(document).ready(function () {
    var pmUrl = 'http://127.0.0.1:7098/api/v2/app.management.api/status';
    $.ajax({
        crossDomain: true,
        url: pmUrl,
        type: 'POST',
        dataType: 'jsonp',
        jsonpCallback: 'callback',
        success: function (bundles) {

            var totalBundle = 0;
            var totalActiveBundle = 0;
            var totalPausedBundle = 0;
            
            $.each(bundles.Routes, function () {
                var route = this;
                totalBundle += 1;
                var tdName = $("<td/>");
                var tdGroupId = $("<td/>");
                var tdModel = $("<td/>");
                var tdState = $("<td/>");
                var tdAction = $("<td/>");

                var color;
                if (route.BundleState == 'Active') {
                    color = "label label-success";
                    totalActiveBundle += 1;
                } else {
                    color = "label label-danger";
                    totalPausedBundle += 1;
                }

                tdName.append("<input type='checkbox' style='vertical-align: middle;' class='btn-primary' /> - </span> <span style='width: 400px; text-decoration: underline'>" + route.Name + "</span>");
                var version = "<button class=\"btn btn-primary\" type=\"button\">" +
                                  "<span class=\"badge\">" + route.Model + "</span>" +
                               "</button>";
                tdGroupId.append(route.GroupId);
                tdModel.append(version);
                tdState.append("<span class=\"" + color + "\">" + route.BundleState + "</span>");



                var statusMsg = "";
                var command = "";
                if (route.BundleState == "Active") {
                    statusMsg = "Pause Bundle";
                    command = "pauseBundle";
                } else {
                    statusMsg = "Start - Bundle";
                    command = "unPauseBundle";
                }

                var msg = "<div class=\"btn-group\" role=\"group\" aria-label=\"...\">" +
                              "<button id=\"activity\" name=\"activity\"  type=\"button\" onclick='"+command+"(\"" + route.GuidData + "\")' class=\"btn btn-default\">" + statusMsg + "</button>" +
                              "<button type=\"button\" class=\"btn btn-default\" >un-install</button>" +
                              "<button type=\"button\" class=\"btn btn-default\">info</button>" +
                          "</div>";

                tdAction.append(msg);
                var newRow = $("<tr/>");

                $("#contr").append((newRow)
                    .append(tdName)
                    .append(tdGroupId)
                    .append(tdModel)
                    .append(tdState)
                    .append(tdAction));
            });

            $("#spnSummary").append("Total Bundles Loaded: " + totalBundle + ", Paused: " + totalPausedBundle, ", Active: " + totalActiveBundle);
        }
    });
});