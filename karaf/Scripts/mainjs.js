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

function restartBundle(url) {

    var pmUrl = 'http://127.0.0.1:7098/api/v2/app.management.api/bundle.restart/' + url;
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



function showBundleInfo(url) {

    var pmUrl = 'http://127.0.0.1:7098/api/v2/app.management.api/bundle.status/' + url;
    //get info
    $.ajax({
        crossDomain: true,
        url: pmUrl,
        type: 'POST',
        dataType: 'jsonp',
        jsonpCallback: 'callback',
        success: function (result) {

            var route = result.Route;
            $("#tdName").html("<b>" + route.Name + "</b>");
            $("#tdGuid").html(route.GuidData);
            $("#tdDev").html(route.Author);
            $("#tdModel").html(route.Model);
            
            if (route.BundleState == "Active") {
                $("#tdStatus").html("<span class=\"label label-success\">Actve</span>");
            }

            if (route.BundleState == "Stopped") {
                $("#tdStatus").html("<span class=\"label label-warning\">Paused</span>");
            }
            
            if (route.BundleState == "UnInstalled") {
                $("#tdStatus").html("<span class=\"label label-danger\">Un-installed</span>");
            }
            
            
            $('#myModal').on('shown.bs.modal', function () {
                $('#myInput').focus();
            });
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
                var version = "<button data-id=\"" + route.GuidData + "\" class=\"btn btn-primary\" type=\"button\" data-toggle='modal' data-target=\".bs-example-modal-lg\" onclick='showBundleInfo(\"" + route.GuidData + "\")'  >" +
                                  "<span class=\"badge\">" + route.Model + "</span>" +
                               "</button>";
                
                tdGroupId.append(route.GroupId);
                tdModel.append(version);
                tdState.append("<span class=\"" + color + "\">" + route.BundleState + "</span>");
                var msg = "";


                var statusMsg = "";
                var command = "";
                
                if (route.BundleState == "Active") {
                    statusMsg = "Pause Bundle";
                    command = "pauseBundle";
                    
                    msg = "<div class=\"btn-group\" role=\"group\" aria-label=\"...\">" +
                              "<button id=\"activity\" name=\"activity\"  type=\"button\" onclick='" + command + "(\"" + route.GuidData + "\")' class=\"btn btn-default\">" + statusMsg + "</button>" +
                              "<button type=\"button\" class=\"btn btn-default\" >Restart</button>" +
                              "<button id=\"restartBndl\" name=\"restartBndl\" type=\"button\" onclick='restartBundle(\"" + route.GuidData + "\")' class=\"btn btn-default\" >Un-install</button>" +
                              "<button type=\"button\" class=\"btn btn-default\">Info</button>" +
                          "</div>";
                }
                if (route.BundleState == "UnInstalled") {
                    statusMsg = "UnInstalled Bundle";
                    command = "pauseBundle";
                    
                    msg = "<div class=\"btn-group\" role=\"group\" aria-label=\"...\">" +
                              
                          "</div>";
                }
                if (route.BundleState == "Stopped") {
                    statusMsg = "<b>Start</b> - Bundle";
                    command = "unPauseBundle";
                    
                    msg = "<div class=\"btn-group\" role=\"group\" aria-label=\"...\">" +
                              "<button id=\"activity\" name=\"activity\"  type=\"button\" onclick='" + command + "(\"" + route.GuidData + "\")' class=\"btn btn-default\">" + statusMsg + "</button>" +
                              "<button type=\"button\" class=\"btn btn-default\" >Restart</button>" +
                              "<button id=\"restartBndl\" name=\"restartBndl\" type=\"button\" onclick='restartBundle(\"" + route.GuidData + "\")' class=\"btn btn-default\" >Un-install</button>" +
                              "<button type=\"button\" class=\"btn btn-default\">Info</button>" +
                          "</div>";
                }

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

$(document).ready(function () {

    $('button.btn-primary').click(function (ev) {
        ev.preventDefault();
        var uid = $(this).data('id');
        showBundleInfo(uid);
    });
});