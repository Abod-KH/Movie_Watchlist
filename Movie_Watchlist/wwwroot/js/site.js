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
// Watchlist toggle for the shared poster card (_PosterCard.cshtml)
$(document).on("click", ".toggle-watchlist", function (e) {
    e.preventDefault();

    const btn = $(this);
    const id = btn.data("id");
    const isTv = btn.data("media-type") === "tv";
    const isAdd = btn.attr("data-action") === "add";

    const controller = isTv ? "TvShowWatchlist" : "Watchlist";
    const config = {
        newAction: isAdd ? "remove" : "add",
        removeCls: isAdd ? "btn-light border" : "btn-success",
        addCls: isAdd ? "btn-success" : "btn-light border",
        icon: isAdd
            ? '<i class="bi bi-bookmark-check-fill text-white fs-5"></i>'
            : '<i class="bi bi-bookmark-plus text-primary fs-5"></i>'
    };

    $.ajax({
        url: `/${controller}/${isAdd ? "AddItem" : "RemoveFromWatchlist"}`,
        type: "POST",
        data: isTv ? (isAdd ? { tvShowId: id } : { id: id }) : { movieId: id },
        success: () => switchButtonState(btn, config)
    });
});

// ============================================================
// Search Overlay Open / Close Logic
// ============================================================
$(document).ready(function () {
    const $backdrop = $('#searchBackdrop');
    const $overlay  = $('#primeSearchOverlay');
    const $input    = $('#globalSearchInput');
    const $openBtn  = $('#openSearchOverlayBtn');
    const $closeBtn = $('#closeSearchOverlayBtn');

    function openSearchOverlay() {
        $backdrop.fadeIn(200);
        $overlay.fadeIn(200, function () {
            $input.focus();
        });
        $('body').css('overflow', 'hidden');
    }

    function closeSearchOverlay() {
        $overlay.fadeOut(180);
        $backdrop.fadeOut(180);
        $('body').css('overflow', '');
        // Clear results when closing
        $('#autocompleteResults').hide().empty();
        $input.val('');
    }

    $openBtn.on('click', function (e) {
        e.stopPropagation();
        openSearchOverlay();
    });

    $closeBtn.on('click', function () {
        closeSearchOverlay();
    });

    // Close on backdrop click
    $backdrop.on('click', function () {
        closeSearchOverlay();
    });

    // Close on Escape key
    $(document).on('keydown', function (e) {
        if (e.key === 'Escape') {
            closeSearchOverlay();
        }
    });

    // Prevent overlay panel clicks from bubbling to backdrop
    $overlay.on('click', function (e) {
        e.stopPropagation();
    });
});

// ============================================================
// Global Search Autocomplete
// ============================================================
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
                    const $grid = $('<div class="search-results-grid"></div>');

                    data.forEach(item => {
                        const url = item.mediaType === 'tv'
                            ? `/TvShow/Details/${item.id}`
                            : `/Home/Details/${item.id}`;

                        const badgeClass = item.mediaType === 'tv' ? 'bg-info' : 'bg-warning text-dark';
                        const badgeLabel = item.mediaType === 'tv' ? 'TV' : 'Movie';

                        const posterHtml = item.posterPath
                            ? `<img src="https://image.tmdb.org/t/p/w185${item.posterPath}" class="result-poster" alt="${item.title}">`
                            : `<div class="result-poster-placeholder"><i class="bi bi-image"></i></div>`;

                        $grid.append(`
                            <a href="${url}" class="search-result-item text-white d-flex flex-column">
                                ${posterHtml}
                                <div class="p-2">
                                    <div class="d-flex align-items-center gap-1 mb-1">
                                        <span class="badge ${badgeClass} rounded-pill" style="font-size:0.65rem;">${badgeLabel}</span>
                                        ${item.releaseYear ? `<small class="text-muted" style="font-size:0.7rem;">${item.releaseYear}</small>` : ''}
                                    </div>
                                    <p class="mb-0 fw-semibold small text-truncate" title="${item.title}">${item.title}</p>
                                </div>
                            </a>
                        `);
                    });

                    $results.append($grid).show();
                } else {
                    $results.append('<div class="search-no-results"><i class="bi bi-search opacity-30 fs-3 d-block mb-2"></i>No results found</div>');
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
