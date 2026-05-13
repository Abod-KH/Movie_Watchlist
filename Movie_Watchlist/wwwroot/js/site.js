// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Global AJAX Configuration
const antiForgeryToken =
    $('input[name="__RequestVerificationToken"]').val();

$.ajaxSetup({

    beforeSend: function (xhr) {

        if (antiForgeryToken) {

            xhr.setRequestHeader(
                "RequestVerificationToken",
                antiForgeryToken
            );
        }
    },
     error: function (xhr) {

        switch (xhr.status) {

            case 401:
                window.location.href = "/Account/Login";
                break;

            case 403:
                
                window.location.href = "/Account/AccessDenied";
                break;

            case 404:
                
                window.location.href = "/Error/NotFound";
                break;

            case 500:
                alert("Server error");
                break;

            default:
                alert("Something went wrong");
        }
    }
});

function switchButtonState(btn, config) {
    btn.removeClass(config.removeCls).addClass(config.addCls);
    btn.attr("data-action", config.newAction);
    btn.html(config.icon);

}
