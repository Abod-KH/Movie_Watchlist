// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const getAntiforgeryToken = () => $('input[name="__RequestVerificationToken"]').val();

// 2. Global AJAX Configuration
$.ajaxSetup({
    type: 'POST', 
    headers: {
        "RequestVerificationToken": getAntiforgeryToken()
    },
   
});