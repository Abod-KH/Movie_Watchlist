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

// Global Search Autocomplete
$(document).ready(function () {
    let searchTimeout;
    const $input = $('#globalSearchInput');
    const $results = $('#autocompleteResults');

    $input.on('input', function () {
        clearTimeout(searchTimeout);
        const query = $(this).val().trim();

        if (query.length < 2) {
            $results.hide().empty();
            return;
        }

        searchTimeout = setTimeout(() => {
            $.get('/Search/Autocomplete', { q: query }, function (data) {
                $results.empty();
                
                if (data && data.length > 0) {
                    data.forEach(item => {
                        const icon = item.mediaType === 'tv' ? '<i class="bi bi-tv me-2 text-info"></i>' : '<i class="bi bi-film me-2 text-warning"></i>';
                        const url = item.mediaType === 'tv' ? `/TvShow/Details/${item.id}` : `/Home/Details/${item.id}`;
                        const poster = item.posterPath 
                            ? `<img src="https://image.tmdb.org/t/p/w92${item.posterPath}" style="width:30px; height:45px; object-fit:cover;" class="me-2 rounded">`
                            : `<div style="width:30px; height:45px;" class="me-2 rounded bg-secondary"></div>`;

                        $results.append(`
                            <a href="${url}" class="dropdown-item d-flex align-items-center py-2 text-wrap">
                                ${poster}
                                <div class="d-flex flex-column">
                                    <span class="fw-bold">${item.title}</span>
                                    <small class="text-muted">${icon}${item.releaseYear || ''}</small>
                                </div>
                            </a>
                        `);
                    });
                    $results.show();
                } else {
                    $results.append('<div class="dropdown-item text-muted">No results found</div>');
                    $results.show();
                }
            });
        }, 300);
    });

    $(document).on('click', function (e) {
        if (!$(e.target).closest('form').length) {
            $results.hide();
        }
    });
});
