// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Global AJAX Configuration
$.ajaxSetup({
    type: 'POST', 
    beforeSend: function (xhr) {
        const token = $('input[name="__RequestVerificationToken"]').val();
        if (token) {
            xhr.setRequestHeader("RequestVerificationToken", token);
        }
    }
});